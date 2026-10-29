using Microsoft.EntityFrameworkCore;
using Unite.Data.Constants;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Repositories.Extensions.Queryable;
using Unite.Data.Context.Services.Stats;
using Unite.Data.Entities;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Omics;
using Unite.Data.Entities.Omics.Analysis;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Omics.Analysis.Prot;
using Unite.Data.Entities.Omics.Analysis.Rna;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Analysis.Drugs;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Donors.Indices.Services.Extensions;
using Unite.Donors.Indices.Services.Mapping;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Basic;
using Unite.Indices.Entities.Projects;
using Unite.Indices.Entities.Projects.Stats;
using Unite.Indices.Entities.Projects.Stats.Base;

using SM = Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using CNV = Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Omics.Analysis.Dna.Sv;

namespace Unite.Donors.Indices.Services;

public class ProjectIndexCreator
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly ProjectsRepository _projectsRepository;
    private readonly DonorsRepository _donorsRepository;
    private readonly SpecimensRepository _specimensRepository;


    public ProjectIndexCreator(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _projectsRepository = new ProjectsRepository(dbContextFactory);
        _donorsRepository = new DonorsRepository(dbContextFactory);
        _specimensRepository = new SpecimensRepository(dbContextFactory);
    }


    public ProjectIndex CreateIndex(object key)
    {
        var projectId = (int)key;

        return CreateProjectIndex(projectId);
    }


    private ProjectIndex CreateProjectIndex(int projectId)
    {
        var project = LoadProject(projectId);

        if (project == null)
            return null;

        return CreateProjectIndex(project);
    }

    private ProjectIndex CreateProjectIndex(Project project)
    {
        var index = ProjectIndexMapper.CreateFrom<ProjectIndex>(project);

        index.Donors = CreateDonorIndices(project.Id);
        index.Users = CreateUserIndices(project.Id);
        index.Stats =  CreateStatsIndex(project.Id); // TODO: Improve performance caching input data
        index.Data = CreateDataIndex(project.Id);
        index.IsPublic = project.IsPublic;

        return index;
    }

    private Project LoadProject(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Project>()
            .AsNoTracking()
            .FirstOrDefault(project => project.Id == projectId);
    }


    private DonorIndex[] CreateDonorIndices(int projectId)
    {
        var donors = LoadDonors(projectId);

        return donors.Select(donor => CreateDonorIndex(donor)).ToArrayOrNull();
    }

    private DonorIndex CreateDonorIndex(Donor donor)
    {
        var index = DonorNavIndexMapper.CreateFrom<DonorIndex>(donor);

        index.Images = CreateImageIndices(donor.Id);
        index.Specimens = CreateSpecimenIndices(donor.Id);

        return index;
    }

    private Donor[] LoadDonors(int projectId)
    {
        var donorsIds = _projectsRepository.GetRelatedDonors([projectId]).Result;

        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Donor>()
            .AsNoTracking()
            .Where(donor => donorsIds.Contains(donor.Id))
            .ToArray();
    }
    
    private DataUserNavIndex[] CreateUserIndices(int projectId)
    {
        var users = LoadUsers(projectId);

        return users.Select(dataUser => CreateUserIndex(dataUser)).ToArrayOrNull();
    }

    private DataUserNavIndex CreateUserIndex(DataUser dataUser)
    {
        var index = new DataUserNavIndex
        {
            Id = dataUser.Id,
            UserId =  dataUser.UserId
        };

        return index;
    }
    
    private DataUser[] LoadUsers(int projectId)
    {
        var userIds = _projectsRepository.GetRelatedUsers([projectId]).Result;

        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<DataUser>()
            .AsNoTracking()
            .Where(user => userIds.Contains(user.Id))
            .ToArray();
    }
    
    private ImageIndex[] CreateImageIndices(int donorId)
    {
        var images = LoadImages(donorId);

        return images.Select(image => CreateImageIndex(image)).ToArrayOrNull();
    }

    private ImageIndex CreateImageIndex(Image image)
    {
        return ImageNavIndexMapper.CreateFrom<ImageIndex>(image, null);
    }

    private Image[] LoadImages(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var imageIds = _donorsRepository.GetRelatedImages([donorId]).Result;

        return dbContext.Set<Image>()
            .AsNoTracking()
            .Where(image => imageIds.Contains(image.Id))
            .ToArray();
    }


    private SpecimenIndex[] CreateSpecimenIndices(int donorId)
    {
        var specimens = LoadSpecimens(donorId);

        return specimens.Select(specimen => CreateSpecimenIndex(specimen)).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen)
    {
        return SpecimenNavIndexMapper.CreateFrom<SpecimenIndex>(specimen, null);
    }

    private Specimen[] LoadSpecimens(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToArray();
    }


    private StatsIndex CreateStatsIndex(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return new StatsIndex
        {
            Donors = CountDonorsStats(projectId),
            Images = CountImagesStats(projectId),
            Specimens = CountSpecimensStats(projectId),
            Dna = CountDnaStats(projectId),
            Meth = CountMethStats(projectId),
            Rna = CountRnaStats(projectId),
            Rnasc = CountRnascStats(projectId),
            Prot = CountProtStats(projectId)
        };
    }

    private DonorsStats CountDonorsStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new DonorsStats();

        var ids = _projectsRepository.GetRelatedDonors([projectId]).Result;

        var entries = dbContext.Set<Donor>()
            .AsNoTracking()
            .Include(donor => donor.ClinicalData)
            .Where(donor => ids.Contains(donor.Id))
            .ToArray();

        // Total number
        stats.Number = entries.Length;

        // Per age
        if (entries.Any(entry => entry.ClinicalData?.EnrollmentAge != null))
        {
            stats.PerAge = StatsService.GetRangeBreakdown(entries, donor => donor.ClinicalData?.EnrollmentAge)
                .Select(stat => new Stat<int?, int>(stat.Key, stat.Count))
                .ToArrayOrNull();
        }

        // Per sex
        if (entries.Any(entry => entry.ClinicalData?.SexId != null))
        {
            stats.PerSex = StatsService.GetPropertyBreakdown(entries, donor => donor.ClinicalData?.SexId)
                .Select(stat => new Stat<string, int>(stat.Key?.ToDefinitionString(), stat.Count))
                .ToArrayOrNull();
        }

        // Per diagnosis
        if (entries.Any(entry => !string.IsNullOrWhiteSpace(entry.ClinicalData?.Diagnosis)))
        {
            stats.PerDiagnosis = GetTopBreakdown(entries.Select(entry => entry.ClinicalData?.Diagnosis), 10);
        }

        // Per vital status
        if (entries.Any(entry => entry.ClinicalData?.VitalStatus != null))
        {
            stats.PerVitalStatus = StatsService.GetPropertyBreakdown(entries, donor => donor.ClinicalData?.VitalStatus)
                .Select(stat => new Stat<bool?, int>(stat.Key, stat.Count))
                .ToArrayOrNull();
        }
        
        // Per progression status
        if (entries.Any(entry => entry.ClinicalData?.ProgressionStatus != null))
        {
            stats.PerProgressionStatus = StatsService.GetPropertyBreakdown(entries, donor => donor.ClinicalData?.ProgressionStatus)
                .Select(stat => new Stat<bool?, int>(stat.Key, stat.Count))
                .ToArrayOrNull();
        }
        
        return stats;
    }

    private ImagesStats CountImagesStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new ImagesStats();

        var ids = _projectsRepository.GetRelatedImages([projectId]).Result;

        var entries = dbContext.Set<Image>()
            .AsNoTracking()
            .Include(image => image.MrImage)
            .Where(image => ids.Contains(image.Id))
            .ToArray();

        // MR
        stats.Mr = CountMrStats(entries.Where(image => image.TypeId == ImageType.MR));

        // Per type
        stats.PerType = StatsService.GetPropertyBreakdown(entries, image => image.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        return stats;
    }

    private static MrStats CountMrStats(IEnumerable<Image> entries)
    {
        var stats = new MrStats();

        // Total number
        var withData = entries.Select(image => image.DonorId).Distinct().Count();
        stats.Number = [withData, entries.Count()];
        
        // Per whole tumor size
        stats.PerSize = StatsService
            .GetRangeBreakdown(entries, image => image.MrImage.WholeTumor)
            .Select(stat => new Stat<double?, int>(stat.Key, stat.Count))
            .ToArrayOrNull();

        return stats;
    }

    private SpecimensStats CountSpecimensStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new SpecimensStats();

        var ids = _projectsRepository.GetRelatedSpecimens([projectId]).Result;

        var entries = dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => ids.Contains(specimen.Id))
            .ToArray();

        // Materials
        stats.Material = CountMaterialStats(entries.Where(specimen => specimen.TypeId == SpecimenType.Material));

        // Lines
        stats.Line = CountLineStats(entries.Where(specimen => specimen.TypeId == SpecimenType.Line));

        // Organoids
        stats.Organoid = CountOrganoidStats(entries.Where(specimen => specimen.TypeId == SpecimenType.Organoid));

        // Xenoenograft
        stats.Xenograft = CountXenograftStats(entries.Where(specimen => specimen.TypeId == SpecimenType.Xenograft));

        // Per type
        stats.PerType = StatsService.GetPropertyBreakdown(entries, specimen => specimen.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        return stats;
    }

    private static MaterialStats CountMaterialStats(IEnumerable<Specimen> entries)
    {
        var stats = new MaterialStats();

        // Total number
        var withData = entries.Select(specimen => specimen.DonorId).Distinct().Count();
        stats.Number = [withData, entries.Count()];

        return stats;
    }

    private static LineStats CountLineStats(IEnumerable<Specimen> entries)
    {
        var stats = new LineStats();

        // Total number
        var withData = entries.Select(specimen => specimen.DonorId).Distinct().Count();
        stats.Number = [withData, entries.Count()];

        return stats;
    }

    private static OrganoidStats CountOrganoidStats(IEnumerable<Specimen> entries)
    {
        var stats = new OrganoidStats();

        // Total number
        var withData = entries.Select(specimen => specimen.DonorId).Distinct().Count();
        stats.Number = [withData, entries.Count()];

        return stats;
    }

    private static XenograftStats CountXenograftStats(IEnumerable<Specimen> entries)
    {
        var stats = new XenograftStats();
        
        // Total number
        var withData = entries.Select(specimen => specimen.DonorId).Distinct().Count();
        stats.Number = [withData, entries.Count()];

        return stats;
    }

    private DnaStats CountDnaStats(int projectId)
    {
        return new DnaStats
        {
            Sm = CountSmStats(projectId),
            Cnv = CountCnvStats(projectId),
            Sv = CountSvStats(projectId),
            Cnvp = CountCnvpStats(projectId)
        };
    }

    private SmStats CountSmStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new SmStats();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var analyses = new AnalysisType[] { AnalysisType.WGS, AnalysisType.WES };
        var withAnalyses = dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Specimen.Donor)
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => donorIds.Contains(sample.Specimen.DonorId))
            .Where(sample => analyses.Contains(sample.Analysis.TypeId))
            .Where(sample => sample.SmEntries.Any())
            .ToArray();

        // Total donors with the data
        var donors = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();
        var specimens = withAnalyses.Select(sample => sample.SpecimenId).Distinct().Count();
        stats.Number = [donors, specimens];

        // Per analysis
        stats.PerAnalysis = StatsService.GetPropertyBreakdown(withAnalyses, sample => sample.Analysis.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        var entryIds = _projectsRepository.GetRelatedVariants<SM.Variant>([projectId]).Result;
        var entries = dbContext.Set<SM.Variant>()
            .AsNoTracking()
            .IncludeAffectedTranscripts()
            .Where(variant => entryIds.Contains(variant.Id))
            .ToArray();

        // Per type
        stats.PerType = StatsService.GetPropertyBreakdown(entries, entry => entry.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        // Per effect
        var withConsequences = entries.Where(entry => entry.AffectedTranscripts?.Count > 0).ToArray();
        var withoutConsequences = entries.Where(entry => entry.AffectedTranscripts?.Count < 1).ToArray();

        // Per effect impact
        stats.PerEffectImpact = StatsService.GetPropertyBreakdown(entries, entry => entry.MostAffectedTranscript?.MostSevereEffect.Impact)
            .Select(stat => new Stat<string, int>(stat.Key ?? "No impact", stat.Count))
            .ToArrayOrNull();

        // Per effect type
        stats.PerEffectType = StatsService.GetPropertyBreakdown(withConsequences, entry => entry.GetMostSevereEffect().Type)
            .Select(stat => new Stat<string, int>(stat.Key ?? "No effect", stat.Count))
            .ToArrayOrNull();

        // Per change
        var cBases = new[] { "C", "G" };
        var tBases = new[] { "T", "A" };
        var dBases = new[] { "C", "T" };
        var changeGroups = new (string[] Ref, string[] Alt)[]
        {
            ( cBases, ["A"] ), // C -> A
            ( cBases, ["G", "C"] ), // C -> G
            ( cBases, ["T"] ), // C -> T
            ( tBases, ["A", "T"] ), // T -> A
            ( tBases, ["C"] ), // T -> C
            ( tBases, ["G"] ) // T -> G
        };

        // Per base ref
        var withBaseRef = entries.Where(entry => entry.Ref?.Length == 1);
        stats.PerBaseRef = dBases.Select(value => new Stat<string, int>(
            value, value == dBases[0] ? withBaseRef.Count(entry => cBases.Contains(entry.Ref)) : withBaseRef.Count(entry => tBases.Contains(entry.Ref))
        ))
        .ToArrayOrNull();

        // Per base alt
        var withBaseAlt = entries.Where(entry => entry.Alt?.Length == 1);
        stats.PerBaseAlt = dBases.Select(value => new Stat<string, int>(
            value, value == dBases[0] ? withBaseAlt.Count(entry => cBases.Contains(entry.Alt)) : withBaseAlt.Count(entry => tBases.Contains(entry.Alt))
        ))
        .ToArrayOrNull();

        // Per base change
        var withBaseChange = entries.Where(entry => entry.Ref?.Length == 1 && entry.Alt?.Length == 1);
        stats.PerBaseChange = changeGroups.Select(value => new Stat<string, int>(
            $"{value.Ref[0]} > {value.Alt[0]}", withBaseChange.Count(entry => value.Ref.Contains(entry.Ref) && value.Alt.Contains(entry.Alt))
        ))
        .ToArrayOrNull();

        return stats;
    }

    private CnvStats CountCnvStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new CnvStats();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var analyses = new AnalysisType[] { AnalysisType.WGS, AnalysisType.WES };
        var withAnalyses = dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Specimen.Donor)
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => donorIds.Contains(sample.Specimen.DonorId))
            .Where(sample => analyses.Contains(sample.Analysis.TypeId))
            .Where(sample => sample.CnvEntries.Any())
            .ToArray();

        // Total donors with the data
        var donors = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();
        var specimens = withAnalyses.Select(sample => sample.SpecimenId).Distinct().Count();
        stats.Number = [donors, specimens];

        // Per analysis
        stats.PerAnalysis = StatsService.GetPropertyBreakdown(withAnalyses, sample => sample.Analysis.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        var entryIds = _projectsRepository.GetRelatedVariants<CNV.Variant>([projectId]).Result;
        var entries = dbContext.Set<CNV.Variant>()
            .AsNoTracking()
            .Where(variant => entryIds.Contains(variant.Id))
            .ToArray();

        // Per type
        stats.PerType = StatsService.GetPropertyBreakdown(entries, entry => entry.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        return stats;
    }

    private SvStats CountSvStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new SvStats();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var analyses = new AnalysisType[] { AnalysisType.WGS, AnalysisType.WES };
        var withAnalyses = dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Specimen.Donor)
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => donorIds.Contains(sample.Specimen.DonorId))
            .Where(sample => analyses.Contains(sample.Analysis.TypeId))
            .Where(sample => sample.SvEntries.Any())
            .ToArray();

        // Total donors with the data
        var donors = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();
        var specimens = withAnalyses.Select(sample => sample.SpecimenId).Distinct().Count();
        stats.Number = [donors, specimens];

        // Per analysis
        stats.PerAnalysis = StatsService.GetPropertyBreakdown(withAnalyses, sample => sample.Analysis.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        var entryIds = _projectsRepository.GetRelatedVariants<SV.Variant>([projectId]).Result;
        var entries = dbContext.Set<SV.Variant>()
            .AsNoTracking()
            .Where(variant => entryIds.Contains(variant.Id))
            .ToArray();

        // Per type
        stats.PerType = StatsService.GetPropertyBreakdown(entries, entry => entry.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        return stats;
    }

    private CnvpStats CountCnvpStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new CnvpStats();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var analyses = new AnalysisType[] { AnalysisType.WGS, AnalysisType.WES, AnalysisType.MethArray };
        var withAnalyses = dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Specimen.Donor)
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => donorIds.Contains(sample.Specimen.DonorId))
            .Where(sample => analyses.Contains(sample.Analysis.TypeId))
            .Where(sample => sample.CnvProfiles.Any())
            .ToArray();

        // Total donors with the data
        var donors = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();
        var specimens = withAnalyses.Select(sample => sample.SpecimenId).Distinct().Count();
        stats.Number = [donors, specimens];

        // Per analysis
        stats.PerAnalysis = StatsService.GetPropertyBreakdown(withAnalyses, sample => sample.Analysis.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        // Per chromosome arm (dominant state of each sample: [loss, neutral, gain])
        var sampleIds = withAnalyses.Select(sample => sample.Id).ToArray();
        var profiles = dbContext.Set<CNV.Profile>()
            .AsNoTracking()
            .Where(profile => sampleIds.Contains(profile.SampleId))
            .Where(profile => profile.ChromosomeArmId != null)
            .Select(profile => new { profile.ChromosomeId, profile.ChromosomeArmId, profile.Loss, profile.Neutral, profile.Gain })
            .ToArray();

        stats.PerChromosomeArm = profiles
            .GroupBy(profile => new { profile.ChromosomeId, profile.ChromosomeArmId })
            .OrderBy(group => group.Key.ChromosomeId)
            .ThenBy(group => group.Key.ChromosomeArmId)
            .Select(group => new Stat<string, int[]>(
                $"{group.Key.ChromosomeId.ToDefinitionString()}{group.Key.ChromosomeArmId.Value.ToDefinitionString().ToLower()}",
                [
                    group.Count(profile => profile.Loss > profile.Neutral && profile.Loss >= profile.Gain),
                    group.Count(profile => profile.Neutral >= profile.Loss && profile.Neutral >= profile.Gain),
                    group.Count(profile => profile.Gain > profile.Neutral && profile.Gain > profile.Loss)
                ]
            ))
            .ToArrayOrNull();

        return stats;
    }

    private MethStats CountMethStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new MethStats ();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var analyses = new AnalysisType[] { AnalysisType.MethArray, AnalysisType.WGBS, AnalysisType.RRBS };
        var withAnalyses = dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Specimen.Donor)
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => donorIds.Contains(sample.Specimen.DonorId))
            .Where(sample => analyses.Contains(sample.Analysis.TypeId))
            .Where(sample => sample.Resources.Any(resource => resource.Type == DataTypes.Omics.Methylation.Sample && resource.Format == FileTypes.Sequence.Idat))
            .ToArray();

        // Total donors with the data
        var donors = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();
        var specimens = withAnalyses.Select(sample => sample.SpecimenId).Distinct().Count();
        stats.Number = [donors, specimens];

        // Per analysis
        stats.PerAnalysis = StatsService.GetPropertyBreakdown(withAnalyses, sample => sample.Analysis.TypeId)
            .Select(stat => new Stat<string, int>() { Key = stat.Key.ToDefinitionString(), Value = stat.Count })
            .ToArrayOrNull();

        return stats;
    }

    private RnaStats CountRnaStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new RnaStats();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var analyses = new AnalysisType[] { AnalysisType.RNASeq };
        var withAnalyses = dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Specimen.Donor)
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => donorIds.Contains(sample.Specimen.DonorId))
            .Where(sample => analyses.Contains(sample.Analysis.TypeId))
            .Where(sample => sample.GeneExpressions.Any())
            .ToArray();

        // Total donors with the data
        var donors = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();
        var specimens = withAnalyses.Select(sample => sample.SpecimenId).Distinct().Count();
        stats.Number = [donors, specimens];

        // Per analysis
        stats.PerAnalysis = StatsService.GetPropertyBreakdown(withAnalyses, sample => sample.Analysis.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        // Per variation
        var expressionGroups = dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Include(entry => entry.Entity)
            .Where(entry => donorIds.Contains(entry.Sample.Specimen.DonorId))
            .GroupBy(entry => entry.EntityId)
            .Select(group => new { 
                Key = group.Key,
                Name = group.First().Entity.Symbol ?? group.First().Entity.StableId,
                Reads = group.Select(entry => entry.TPM),
                Count = group.Count()
            })
            .ToArray();

        stats.PerVariation = expressionGroups
            .Select(group => new { group.Name, Values = GetBoxValues(group.Reads) })
            .Where(group => group.Values[3] != 0) // Skip groups with low variation (Q3 is 0)
            .OrderByDescending(group => group.Values[7]) // CV
            .Take(25)
            .Select(group => new Stat<string, double[]>(group.Name, group.Values.Select(value => Math.Round(value, 2)).ToArray()))
            .ToArrayOrNull();

        // Per mutation
        var variantIds = _projectsRepository.GetRelatedVariants<SM.Variant>([projectId]).Result;
        var variantEntries = dbContext.Set<SM.VariantEntry>()
            .AsNoTracking()
            .Include(entry => entry.Entity)
            .Include(entry => entry.Entity.AffectedTranscripts)
                .ThenInclude(transcript => transcript.Feature.Gene)
            .Where(entry => variantIds.Contains(entry.EntityId))
            .Where(entry => entry.Entity.AffectedTranscripts.Any(transcript => transcript.Distance == null))
            .ToArray();

        stats.PerMutation = variantEntries
            .GroupBy(entry => entry.Entity.MostAffectedTranscript.Feature.GeneId)
            .Select(group => new {
                Key = group.Key,
                Name = group.First().Entity.MostAffectedTranscript.Feature.Gene.Symbol ?? group.First().Entity.MostAffectedTranscript.Feature.Gene.StableId,
                Count = group.Count()
            })
            .OrderByDescending(entry => entry.Count)
            .Take(25)
            .Select(entry => new Stat<string, int>(entry.Name, entry.Count))
            .ToArrayOrNull();

        return stats;
    }

    private RnascStats CountRnascStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new RnascStats();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var analyses = new AnalysisType[] { AnalysisType.RNASeqSc, AnalysisType.RNASeqSn };
        var resources = new string[] { DataTypes.Omics.Rnasc.Expression };
        var withAnalyses = dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Specimen.Donor)
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => donorIds.Contains(sample.Specimen.DonorId))
            .Where(sample => analyses.Contains(sample.Analysis.TypeId))
            .Where(sample => sample.Resources.Any(resource => resources.Contains(resource.Type)))
            .ToArray();

        // Total donors with the data
        var donors = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();
        var specimens = withAnalyses.Select(sample => sample.SpecimenId).Distinct().Count();
        stats.Number = [donors, specimens];

        // Per analysis
        stats.PerAnalysis = StatsService.GetPropertyBreakdown(withAnalyses, sample => sample.Analysis.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        // Per cells
        stats.PerCells = StatsService
            .GetRangeBreakdown(withAnalyses, sample => sample.Cells)
            .Select(stat => new Stat<int?, int>(stat.Key, stat.Count))
            .ToArrayOrNull();

        return stats;
    }

    private ProtStats CountProtStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new ProtStats();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var analyses = new AnalysisType[] { AnalysisType.MS };
        var withAnalyses = dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Specimen.Donor)
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => donorIds.Contains(sample.Specimen.DonorId))
            .Where(sample => analyses.Contains(sample.Analysis.TypeId))
            .Where(sample => sample.ProteinExpressions.Any())
            .ToArray();

        // Total donors with the data
        var donors = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();
        var specimens = withAnalyses.Select(sample => sample.SpecimenId).Distinct().Count();
        stats.Number = [donors, specimens];

        // Per analysis
        stats.PerAnalysis = StatsService.GetPropertyBreakdown(withAnalyses, sample => sample.Analysis.TypeId)
            .Select(stat => new Stat<string, int>(stat.Key.ToDefinitionString(), stat.Count))
            .ToArrayOrNull();

        // Per variation (normalized intensities are log2 centered, so proteins are ranked by SD, not by CV)
        // Proteins are merged by UniProt accession: one accession can match several Ensembl proteins with the same values.
        // Only proteins measured in at least half of the samples (and in at least 3) are ranked, fewer values give unstable SD.
        var sampleIds = withAnalyses.Select(sample => sample.Id).ToArray();
        var minSamples = Math.Max(3, (sampleIds.Length + 1) / 2);
        var expressions = dbContext.Set<ProteinExpression>()
            .AsNoTracking()
            .Where(entry => sampleIds.Contains(entry.SampleId))
            .Select(entry => new { entry.SampleId, entry.Entity.AccessionId, Name = entry.Entity.Symbol ?? entry.Entity.StableId, entry.Normalized })
            .ToArray();

        stats.PerVariation = expressions
            .GroupBy(entry => entry.AccessionId)
            .Select(group => new { Name = group.First().Name, Values = group.DistinctBy(entry => entry.SampleId).Select(entry => entry.Normalized).ToArray() })
            .Where(group => group.Values.Length >= minSamples)
            .Select(group => new { group.Name, Values = GetBoxValues(group.Values) })
            .OrderByDescending(group => group.Values[6]) // SD
            .Take(25)
            .Select(group => new Stat<string, double[]>(group.Name, group.Values.Select(value => Math.Round(value, 2)).ToArray()))
            .ToArrayOrNull();

        // Per mutation (protein changing SMs in donors with proteomics data)
        var proteinDonorIds = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().ToArray();
        var variantEntries = dbContext.Set<SM.VariantEntry>()
            .AsNoTracking()
            .Include(entry => entry.Entity.AffectedTranscripts)
                .ThenInclude(transcript => transcript.Feature.Protein)
            .Where(entry => proteinDonorIds.Contains(entry.Sample.Specimen.DonorId))
            .Where(entry => entry.Entity.AffectedTranscripts.Any(transcript => transcript.ProteinChange != null && transcript.Feature.Protein != null))
            .ToArray();

        stats.PerMutation = variantEntries
            .Select(entry => entry.Entity.MostAffectedTranscript)
            .Where(transcript => transcript.ProteinChange != null && transcript.Feature.Protein != null)
            .GroupBy(transcript => transcript.Feature.Protein.AccessionId)
            .Select(group => new {
                Name = group.First().Feature.Protein.Symbol ?? group.First().Feature.Protein.StableId,
                Count = group.Count()
            })
            .OrderByDescending(entry => entry.Count)
            .Take(25)
            .Select(entry => new Stat<string, int>(entry.Name, entry.Count))
            .ToArrayOrNull();

        return stats;
    }


    private DataIndex CreateDataIndex(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var specimenIds = _projectsRepository.GetRelatedSpecimens([projectId]).Result;

        return new DataIndex
        {
            Donors = donorIds.Length > 0,
            Clinical = CheckClinicalData(donorIds),
            Treatments = CheckTreatments(donorIds),
            Mrs = CheckImages(donorIds, ImageType.MR),
            Cts = CheckImages(donorIds, ImageType.CT),
            Materials = CheckSpecimens(donorIds, SpecimenType.Material),
            MaterialsMolecular = CheckMolecularData(donorIds, SpecimenType.Material),
            Lines = CheckSpecimens(donorIds, SpecimenType.Line),
            LinesMolecular = CheckMolecularData(donorIds, SpecimenType.Line),
            LinesInterventions = CheckInterventions(donorIds, SpecimenType.Line),
            LinesDrugs = CheckDrugScreenings(donorIds, SpecimenType.Line),
            Organoids = CheckSpecimens(donorIds, SpecimenType.Organoid),
            OrganoidsMolecular = CheckMolecularData(donorIds, SpecimenType.Organoid),
            OrganoidsInterventions = CheckInterventions(donorIds, SpecimenType.Organoid),
            OrganoidsDrugs = CheckDrugScreenings(donorIds, SpecimenType.Organoid),
            Xenografts = CheckSpecimens(donorIds, SpecimenType.Xenograft),
            XenograftsMolecular = CheckMolecularData(donorIds, SpecimenType.Xenograft),
            XenograftsInterventions = CheckInterventions(donorIds, SpecimenType.Xenograft),
            XenograftsDrugs = CheckDrugScreenings(donorIds, SpecimenType.Xenograft),
            Sms =  _specimensRepository.HaveVariants<SM.VariantEntry, SM.Variant>(specimenIds).Result,
            Cnvs = _specimensRepository.HaveVariants<CNV.VariantEntry, CNV.Variant>(specimenIds).Result,
            Svs = _specimensRepository.HaveVariants<SV.VariantEntry, SV.Variant>(specimenIds).Result,
            Cnvps = _specimensRepository.HaveProfiles(specimenIds).Result,
            Meth = _specimensRepository.HaveMethylation(specimenIds).Result,
            Exp = _specimensRepository.HaveGeneExpressions(specimenIds).Result,
            ExpSc = _specimensRepository.HaveGeneExpressionsPerCells(specimenIds).Result,
            Prot = _specimensRepository.HaveProteinExpressions(specimenIds).Result
        };
    }

    private bool CheckClinicalData(int[] donorIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<ClinicalData>()
            .AsNoTracking()
            .Where(clinical => donorIds.Contains(clinical.DonorId))
            .Any();
    }

    private bool CheckTreatments(int[] donorIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Treatment>()
            .AsNoTracking()
            .Where(treatment => donorIds.Contains(treatment.DonorId))
            .Any();
    }

    private bool CheckImages(int[] donorIds, ImageType type)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Image>()
            .AsNoTracking()
            .Where(image => donorIds.Contains(image.DonorId))
            .Where(image => image.TypeId == type)
            .Any();
    }

    private bool CheckSpecimens(int[] donorIds, SpecimenType type)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => donorIds.Contains(specimen.DonorId))
            .Where(specimen => specimen.TypeId == type)
            .Any();
    }

    private bool CheckMolecularData(int[] donorIds, SpecimenType type)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMolecularData()
            .Where(specimen => donorIds.Contains(specimen.DonorId))
            .Where(specimen => specimen.TypeId == type)
            .Where(specimen => specimen.MolecularData != null)
            .Any();
    }

    private bool CheckInterventions(int[] donorIds, SpecimenType type)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Intervention>()
            .AsNoTracking()
            .Where(intervention => donorIds.Contains(intervention.Specimen.DonorId))
            .Where(intervention => intervention.Specimen.TypeId == type)
            .Any();
    }

    private bool CheckDrugScreenings(int[] donorIds, SpecimenType type)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<DrugScreening>()
            .AsNoTracking()
            .Where(entry => donorIds.Contains(entry.Sample.Specimen.DonorId))
            .Where(entry => entry.Sample.Specimen.TypeId == type)
            .Any();
    }


    /// <summary>
    /// Breakdown of the most frequent text values (trimmed, case insensitive), the rest is grouped as "Other".
    /// </summary>
    private static Stat<string, int>[] GetTopBreakdown(IEnumerable<string> values, int number)
    {
        var groups = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .GroupBy(value => value, StringComparer.OrdinalIgnoreCase)
            .Select(group => new Stat<string, int>(group.First(), group.Count()))
            .OrderByDescending(stat => stat.Value)
            .ThenBy(stat => stat.Key)
            .ToArray();

        if (groups.Length <= number + 1)
            return groups.ToArrayOrNull();

        var other = new Stat<string, int>("Other", groups.Skip(number).Sum(stat => stat.Value));

        return groups.Take(number).Append(other).ToArray();
    }

    /// <summary>
    /// Values for the box plot: [whisker min, Q1, median, Q3, whisker max, mean, SD, CV].
    /// </summary>
    private static double[] GetBoxValues(IEnumerable<double> values)
    {
        var sorted = values.Order().ToArray();

        var q1 = sorted[sorted.Length / 4];
        var q2 = sorted[sorted.Length / 2];
        var q3 = sorted[sorted.Length * 3 / 4];
        var iqr = q3 - q1;

        var lowerFence = q1 - 1.5 * iqr;
        var upperFence = q3 + 1.5 * iqr;

        var min = sorted.FirstOrDefault(value => value >= lowerFence);
        var max = sorted.LastOrDefault(value => value <= upperFence);

        var mean = sorted.Average();
        var sd = StandardDeviation(sorted);
        var cv = mean != 0 ? sd / mean : 0;

        return [min, q1, q2, q3, max, mean, sd, cv];
    }

    private static double StandardDeviation(IEnumerable<double> values)
    {
        var mean = values.Average();
        var count = values.Count();
        var sum = values.Sum(value => Math.Pow(value - mean, 2));
        return Math.Sqrt(sum / (count - 1));
    }
}
