using Microsoft.EntityFrameworkCore;
using Unite.Data.Constants;
using Unite.Data.Context;
using Unite.Data.Context.Extensions.Queryable;
using Unite.Data.Context.Repositories;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Data.Entities.Genome.Analysis.Enums;
using Unite.Data.Entities.Genome.Analysis.Rna;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Analysis.Drugs;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Donors.Indices.Services.Extensions;
using Unite.Donors.Indices.Services.Mapping;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Projects;
using Unite.Indices.Entities.Projects.Stats;

using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;


namespace Unite.Donors.Indices.Services;

public class ProjectIndexCreator
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly ProjectsRepository _projectsRepository;
    private readonly DonorsRepository _donorsRepository;


    public ProjectIndexCreator(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _projectsRepository = new ProjectsRepository(dbContextFactory);
        _donorsRepository = new DonorsRepository(dbContextFactory);
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
        index.Stats =  CreateStatsIndex(project.Id); // TODO: Improve performance caching input data
        index.Data = CreateDataIndex(project.Id);

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
            Rnasc = CountRnascStats(projectId)
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
        var withAge = entries.Where(donor => donor.ClinicalData?.Age != null);
        var withoutAge = entries.Where(donor => donor.ClinicalData?.Age == null);
        var ageGroups = DefineGroups(withAge.Select(entry => entry.ClinicalData.Age.Value), desiredMin: 0);
        
        stats.PerAge = withAge
            .OrderBy(entry => entry.ClinicalData.Age.Value)
            .GroupBy(entry => GetGroupName(entry.ClinicalData.Age.Value, ageGroups))
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        if (withoutAge.Any())
            stats.PerAge.Add("Unknown", withoutAge.Count());

        // Per gender
        var withGender = entries.Where(donor => donor.ClinicalData?.GenderId != null);
        var withoutGender = entries.Where(donor => donor.ClinicalData?.GenderId == null);

        stats.PerGender = withGender
            .OrderBy(entry => (int)entry.ClinicalData.GenderId.Value)
            .GroupBy(entry => entry.ClinicalData.GenderId.Value)
            .ToDictionary(
                group => group.Key.ToDefinitionString(),
                group => group.Count()
            );

        if (withoutGender.Any())
            stats.PerGender.Add("Unknown", withoutGender.Count());

        // Per vital status
        var withVital = entries.Where(donor => donor.ClinicalData?.VitalStatus != null);
        var withoutVital = entries.Where(donor => donor.ClinicalData?.VitalStatus == null);

        stats.PerVitalStatus = withVital
            .OrderBy(entry => entry.ClinicalData.VitalStatus.Value ? 1  : 0)
            .GroupBy(entry => entry.ClinicalData.VitalStatus.Value ? "Alive" : "Deseased")
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );
        
        if (withoutVital.Any())
            stats.PerVitalStatus.Add("Unknown", withoutVital.Count());

        // Per progression status
        var withProgression = entries.Where(donor => donor.ClinicalData?.ProgressionStatus != null);
        var withoutProgression = entries.Where(donor => donor.ClinicalData?.ProgressionStatus == null);

        stats.PerProgressionStatus = withProgression
            .OrderBy(entry => entry.ClinicalData.ProgressionStatus.Value ? 1 : 0)
            .GroupBy(entry => entry.ClinicalData.ProgressionStatus.Value ? "Progression" : "No progression")
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        if (withoutProgression.Any())
            stats.PerProgressionStatus.Add("Unknown", withoutProgression.Count());

        return stats;
    }

    private ImagesStats CountImagesStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new ImagesStats();

        var ids = _projectsRepository.GetRelatedImages([projectId]).Result;

        var entries = dbContext.Set<Image>()
            .AsNoTracking()
            .Include(image => image.MriImage)
            // .Include(image => image.CtImage)
            .Where(image => ids.Contains(image.Id))
            .ToArray();

        // MRI
        stats.Mri = CountMriStats(entries.Where(image => image.TypeId == ImageType.MRI));

        // CT
        // stats.Ct = CountCtStats(entries.Where(image => image.TypeId == ImageType.CT));

        // Per type
        var typeGroups = entries.Select(image => image.TypeId).OrderBy(value => (int)value).Distinct();

        stats.PerType = typeGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => entries.Count(image => image.TypeId == value)
        );

        return stats;
    }

    private static MriStats CountMriStats(IEnumerable<Image> entries)
    {
        var stats = new MriStats();

        // Total number
        var withData = entries.Select(image => image.DonorId).Distinct().Count();

        stats.Number = [withData, entries.Count()];
        

        // Per whole tumor size
        var withSize = entries.Where(image => image.MriImage?.WholeTumor != null);
        var withoutSize = entries.Where(image => image.MriImage?.WholeTumor == null);
        
        var sizeGroups = DefineGroups(withSize.Select(image => image.MriImage.WholeTumor.Value));
        stats.PerSize = sizeGroups.ToDictionary(
            value => GetGroupName(value, sizeGroups),
            value => entries.Count(image => image.MriImage?.WholeTumor <= value)
        );

        if (withoutSize.Any())
            stats.PerSize.Add("Unknown", withoutSize.Count());

        return stats;
    }

    private static CtStats CountCtStats(IEnumerable<Image> entries)
    {
        var stats = new CtStats();

        // Total number
        var withData = entries.Select(image => image.DonorId).Distinct().Count();
        stats.Number = [withData, entries.Count()];

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
        var typeGroups = entries.Select(specimen => specimen.TypeId).OrderBy(value => (int)value).Distinct();
        stats.PerType = typeGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => entries.Count(specimen => specimen.TypeId == value)
        );

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
            Ssm = CountSsmStats(projectId),
            Cnv = CountCnvStats(projectId),
            Sv = CountSvStats(projectId)
        };
    }

    private SsmStats CountSsmStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new SsmStats();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var analyses = new AnalysisType[] { AnalysisType.WGS, AnalysisType.WES };
        var withAnalyses = dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Specimen.Donor)
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => donorIds.Contains(sample.Specimen.DonorId))
            .Where(sample => analyses.Contains(sample.Analysis.TypeId))
            .Where(sample => sample.SsmEntries.Any())
            .ToArray();

        // Total donors with the data
        stats.Number = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();

        // Per analysis
        var analysisGroups = withAnalyses.Select(sample => sample.Analysis.TypeId).Distinct().OrderBy(value => (int)value);
        stats.PerAnalysis = analysisGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => withAnalyses.Count(sample => sample.Analysis.TypeId == value)
        );

        var entryIds = _projectsRepository.GetRelatedVariants<SSM.Variant>([projectId]).Result;
        var entries = dbContext.Set<SSM.Variant>()
            .AsNoTracking()
            .IncludeAffectedTranscripts()
            .Where(variant => entryIds.Contains(variant.Id))
            .ToArray();

        // Per type
        var typeGroups = entries.Select(entry => entry.TypeId).Distinct().OrderBy(value => (int)value);
        stats.PerType = typeGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => entries.Count(entry => entry.TypeId == value)
        );

        // Per effect
        var withConsequences = entries.Where(entry => entry.AffectedTranscripts?.Count > 0).ToArray();
        var withoutConsequences = entries.Where(entry => entry.AffectedTranscripts?.Count < 1).ToArray();

        // Per effect impact
        var effectImpactGroups = withConsequences.OrderBy(entry => entry.GetMostSevereEffect().Severity).Select(entry => entry.GetMostSevereEffect().Impact).Distinct();
        stats.PerEffectImpact = effectImpactGroups.ToDictionary(
            value => value ?? "Undetermined",
            value => withConsequences.Count(entry => entry.GetMostSevereEffect().Impact == value)
        );

        if (withConsequences.Any())
            stats.PerEffectImpact.Add("No impact", withoutConsequences.Length);

        // Per effect type
        var effectTypeGroups = withConsequences.OrderBy(entry => entry.GetMostSevereEffect().Severity).Select(entry => entry.GetMostSevereEffect().Type).Distinct();
        stats.PerEffectType = effectTypeGroups.ToDictionary(
            value => value,
            value => withConsequences.Count(entry => entry.GetMostSevereEffect().Type == value)
        );

        if (withoutConsequences.Any())
            stats.PerEffectType.Add("No effect", withoutConsequences.Length);


        // Per change
        var cBases = new[] { "C", "G" };
        var tBases = new[] { "T", "A" };
        var dBases = new[] { "C", "T" };

        // Per base ref
        var withBaseRef = entries.Where(entry => entry.Ref?.Length == 1);
        stats.PerBaseRef = dBases.ToDictionary(
            value => value,
            value => value == dBases[0] ? withBaseRef.Count(entry => cBases.Contains(entry.Ref)) : withBaseRef.Count(entry => tBases.Contains(entry.Ref))
        );

        // Per base alt
        var withBaseAlt = entries.Where(entry => entry.Alt?.Length == 1);
        stats.PerBaseAlt = dBases.ToDictionary(
            value => value,
            value => value == dBases[0] ? withBaseAlt.Count(entry => cBases.Contains(entry.Alt)) : withBaseAlt.Count(entry => tBases.Contains(entry.Alt))
        );

        // Per base change
        var changeGroups = new (string[] Ref, string[] Alt)[]
        {
            ( cBases, ["A"] ), // C -> A
            ( cBases, ["G", "C"] ), // C -> G
            ( cBases, ["T"] ), // C -> T
            ( tBases, ["A", "T"] ), // T -> A
            ( tBases, ["C"] ), // T -> C
            ( tBases, ["G"] ) // T -> G
        };
        var withBaseChange = entries.Where(entry => entry.Ref?.Length == 1 && entry.Alt?.Length == 1);
        stats.PerBaseChange = changeGroups.ToDictionary(
            value => $"{value.Ref[0]} > {value.Alt[0]}",
            value => withBaseChange.Count(entry => value.Ref.Contains(entry.Ref) && value.Alt.Contains(entry.Alt))
        );

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
        stats.Number = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();

        // Per analysis
        var analysisGroups = withAnalyses.Select(sample => sample.Analysis.TypeId).Distinct().OrderBy(value => (int)value);
        stats.PerAnalysis = analysisGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => withAnalyses.Count(sample => sample.Analysis.TypeId == value)
        );

        var entryIds = _projectsRepository.GetRelatedVariants<CNV.Variant>([projectId]).Result;
        var entries = dbContext.Set<CNV.Variant>()
            .AsNoTracking()
            .Where(variant => entryIds.Contains(variant.Id))
            .ToArray();

        // Per type
        var typeGroups = entries.Select(entry => entry.TypeId).Distinct().OrderBy(value => (int)value);
        stats.PerType = typeGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => entries.Count(entry => entry.TypeId == value)
        );

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
        stats.Number = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();

        // Per analysis
        var analysisGroups = withAnalyses.Select(sample => sample.Analysis.TypeId).Distinct().OrderBy(value => (int)value);
        stats.PerAnalysis = analysisGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => withAnalyses.Count(sample => sample.Analysis.TypeId == value)
        );

        var entryIds = _projectsRepository.GetRelatedVariants<SV.Variant>([projectId]).Result;
        var entries = dbContext.Set<SV.Variant>()
            .AsNoTracking()
            .Where(variant => entryIds.Contains(variant.Id))
            .ToArray();

        // Per type
        var typeGroups = entries.Select(entry => entry.TypeId).Distinct().OrderBy(value => (int)value);
        stats.PerType = typeGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => entries.Count(entry => entry.TypeId == value)
        );

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
            .Where(sample => sample.Resources.Any(resource => 
                (resource.Type == DataTypes.Genome.Meth.Sample && resource.Format == FileTypes.Sequence.Idat) ||
                (resource.Type == DataTypes.Genome.Meth.Levels)))
            .ToArray();

        // Total donors with the data
        stats.Number = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();

        // Per analysis
        var analysisGroups = withAnalyses.Select(sample => sample.Analysis.TypeId).Distinct().OrderBy(value => (int)value);
        stats.PerAnalysis = analysisGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => withAnalyses.Count(sample => sample.Analysis.TypeId == value)
        );

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
        stats.Number = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();

        // Per analysis
        var analysisGroups = withAnalyses.Select(sample => sample.Analysis.TypeId).Distinct().OrderBy(value => (int)value);
        stats.PerAnalysis = analysisGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => withAnalyses.Count(sample => sample.Analysis.TypeId == value)
        );

        // Per variation
        // In memory calculation using standard deviation
        // I need to extract or calculate values for box plot
        var expressionGroups = dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Include(entry => entry.Entity)
            .Where(entry => donorIds.Contains(entry.Sample.Specimen.DonorId))
            .GroupBy(entry => entry.Entity.StableId)
            .Select(group => new { 
                Key = group.First().Entity.Symbol, 
                Reads = group.Select(entry => entry.FPKM),
                Count = group.Count()
            })
            .ToArray();

        stats.PerVariation = expressionGroups
            .Select(group => new {
                Key = group.Key,
                Reads = group.Reads.Order().ToArray(),
                Count = group.Count
            })
            .Select(group => new {
                Key = group.Key,
                Min = group.Reads.First(),
                Max = group.Reads.Last(),
                Mean = group.Reads.Average(),
                Q1 = group.Reads[group.Count / 4],
                Q2 = group.Reads[group.Count / 2],
                Q3 = group.Reads[group.Count * 3 / 4],
                SD = StandardDeviation(group.Reads)
            })
            .Where(stats => stats.Mean >= 1)
            .Select(stats => new {
                Key = stats.Key,
                Min = stats.Min,
                Max = stats.Max,
                Mean = stats.Mean,
                Q1 = stats.Q1,
                Q2 = stats.Q2,
                Q3 = stats.Q3,
                SD = stats.SD,
                CV = stats.SD / stats.Mean
            })
            .OrderByDescending(stats => stats.CV)
            .Take(25)
            .Select(stats => new {
                Key = stats.Key,
                Values = new double[] { stats.Min, stats.Q1, stats.Q2, stats.Q3, stats.Max, stats.Mean, stats.SD, stats.CV }
            })
            .ToDictionary(
                stats => stats.Key,
                stats => stats.Values.Select(value => Math.Round(value, 2)).ToArray()
            );


        // stats.PerVariation = a
        //     .Take(25)
        //     .ToDictionary(
        //         stats => stats.Key,
        //         stats => new double[] { stats.Min, stats.Q1, stats.Q2, stats.Q3, stats.Max, stats.Mean, stats.SD, stats.CV }

        //     );

        // In database calculation using delta (max - min)
        // stats.PerVariation = dbContext.Set<GeneExpression>()
        //     .AsNoTracking()
        //     .Include(entry => entry.Entity)
        //     .Where(entry => donorIds.Contains(entry.Sample.Specimen.DonorId))
        //     .GroupBy(entry => entry.Entity.Symbol ?? entry.Entity.StableId)
        //     .Select(group => new {
        //         Key = group.Key,
        //         Min = Math.Round(group.Min(value => value.FPKM)),
        //         Max = Math.Round(group.Max(value => value.FPKM)),
        //         Delta = Math.Round(group.Max(value => value.FPKM) - group.Min(value => value.FPKM))
        //     })
        //     .OrderByDescending(entry => entry.Delta)
        //     .Take(25)
        //     .ToDictionary(
        //         group => group.Key,
        //         group => new int[] { (int)group.Min, (int)group.Max }
        //     );


        // Per mutation
        var variantIds = _projectsRepository.GetRelatedVariants<SSM.Variant>([projectId]).Result;
        var variantEntries = dbContext.Set<SSM.VariantEntry>()
            .AsNoTracking()
            .Include(entry => entry.Entity)
            .Include(entry => entry.Entity.AffectedTranscripts)
                .ThenInclude(transcript => transcript.Feature.Gene)
            .Where(entry => variantIds.Contains(entry.EntityId))
            .Where(entry => entry.Entity.AffectedTranscripts.Any(transcript => transcript.Distance == null))
            .ToArray();

        stats.PerMutation = variantEntries
            .GroupBy(entry => entry.Entity.AffectedTranscripts.GetMostSevere().Feature.Gene.Symbol ?? entry.Entity.AffectedTranscripts.GetMostSevere().Feature.Gene.StableId)
            .Select(group => new {
                Key = group.Key,
                Count = group.Count()
            })
            .OrderByDescending(entry => entry.Count)
            .Take(25)
            .ToDictionary(
                group => group.Key,
                group => group.Count
            );

        return stats;
    }

    private RnascStats CountRnascStats(int projectId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var stats = new RnascStats();

        var donorIds = _projectsRepository.GetRelatedDonors([projectId]).Result;
        var analyses = new AnalysisType[] { AnalysisType.RNASeqSc, AnalysisType.RNASeqSn };
        var resources = new string[] { DataTypes.Genome.Rnasc.Exp };
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
        stats.Number = withAnalyses.Select(sample => sample.Specimen.DonorId).Distinct().Count();

        // Per analysis
        var analysisGroups = withAnalyses.Select(sample => sample.Analysis.TypeId).Distinct();
        stats.PerAnalysis = analysisGroups.ToDictionary(
            value => value.ToDefinitionString(),
            value => withAnalyses.Count(sample => sample.Analysis.TypeId == value));

        // Per cells
        var withCells = withAnalyses.Where(sample => sample.Cells != null);
        var withoutCells = withAnalyses.Where(sample => sample.Cells == null);
        var cellGroups = DefineGroups(withCells.Select(sample => sample.Cells.Value));
        stats.PerCells = cellGroups.ToDictionary(
            value => GetGroupName(value, cellGroups),
            value => withCells.Count(sample => sample.Cells <= value)
        );

        if (withoutCells.Any())
            stats.PerCells.Add("Unknown", withoutCells.Count());

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
            Mris = CheckImages(donorIds, ImageType.MRI),
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
            Ssms = CheckVariants<SSM.Variant, SSM.VariantEntry>(specimenIds),
            Cnvs = CheckVariants<CNV.Variant, CNV.VariantEntry>(specimenIds),
            Svs = CheckVariants<SV.Variant, SV.VariantEntry>(specimenIds),
            Meth = CheckMethylation(specimenIds),
            Exp = CheckGeneExp(specimenIds),
            ExpSc = CheckGeneExpSc(specimenIds)
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
    /// Checks if variants data of given type is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <typeparam name="TVariant">Variant type.</typeparam>
    /// <typeparam name="TVariantEntry">Variant occurrence type.</typeparam>
    /// <returns>'true' if variants data exists or 'false' otherwise.</returns>
    private bool CheckVariants<TVariant, TVariantEntry>(int[] specimenIds)
        where TVariant : Variant
        where TVariantEntry : VariantEntry<TVariant>
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<TVariantEntry>()
            .AsNoTracking()
            .Any(entry => specimenIds.Contains(entry.Sample.SpecimenId));
    }

    /// <summary>
    /// Checks if methylation data is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <returns>'true' if methylation data exists or 'false' otherwise.</returns>
    private bool CheckMethylation(int[] specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<SampleResource>()
            .AsNoTracking()
            .Any(resource => specimenIds.Contains(resource.Sample.SpecimenId) && 
                ((resource.Type == DataTypes.Genome.Meth.Sample && resource.Format == FileTypes.Sequence.Idat) ||
                 (resource.Type == DataTypes.Genome.Meth.Levels)));
    }

    /// <summary>
    /// Checks if bulk gene expression data is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <returns>'true' if gene expression data exists or 'false' otherwise.</returns>
    private bool CheckGeneExp(int[] specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Any(expression => specimenIds.Contains(expression.Sample.SpecimenId));
    }

    /// <summary>
    /// Checks if single cell gene expression data is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <returns>'true' if single cell gene expression data exists or 'false' otherwise.</returns>
    private bool CheckGeneExpSc(int[] specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<SampleResource>()
            .AsNoTracking()
            .Any(resource => specimenIds.Contains(resource.Sample.SpecimenId) && resource.Type == DataTypes.Genome.Rnasc.Exp);
    }


    private static string GetGroupName(int value, int[] groups)
    {
        if (value <= groups[0])
            return $"<{groups[0]}";
        else if (value <= groups[1])
            return $"{groups[0] + 1}-{groups[1]}";
        else if (value < groups[2])
            return $"{groups[1] + 1}-{groups[2]}";
        else if (value < groups[3])
            return $"{groups[2] + 1}-{groups[3]}";
        else
            return $">{groups[3] + 1}";
    }

    private static string GetGroupName(double value, double[] groups)
    {
        if (value <= groups[0])
            return $"<{groups[0]}";
        else if (value <= groups[1])
            return $"{groups[0] + 1}-{groups[1]}";
        else if (value < groups[2])
            return $"{groups[1] + 1}-{groups[2]}";
        else if (value < groups[3])
            return $"{groups[2] + 1}-{groups[3]}";
        else
            return $">{groups[3] + 1}";
    }

    private static int[] DefineGroups(IEnumerable<int> values, int? desiredMin = default, int? desiredMax = default)
    {
        if (!values.Any())
            return [];

        var min = desiredMin ?? values.Min();
        var max = desiredMax ?? values.Max();

        if (min == max)
            return [min];

        var step = (max - min) / 5;

        return [min + step, min + step * 2, min + step * 3, min + step * 4];
    }

    private static double[] DefineGroups(IEnumerable<double> values, int? desiredMin = default, int? desiredMax = default)
    {
        if (!values.Any())
            return [];

        var min = desiredMin ?? Math.Floor(values.Min());
        var max = desiredMax ?? Math.Ceiling(values.Max());

        if (min == max)
            return [min];

        var step = Math.Round((max - min) / 5);

        return [min + step, min + step * 2, min + step * 3, min + step * 4];
    }

    private static double StandardDeviation(IEnumerable<double> values)
    {
        var mean = values.Average();
        var count = values.Count();
        var sum = values.Sum(value => Math.Pow(value - mean, 2));
        return Math.Sqrt(sum / count);
    }
}
