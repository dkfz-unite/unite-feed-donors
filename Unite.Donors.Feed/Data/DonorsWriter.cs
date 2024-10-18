using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Donors;
using Unite.Donors.Feed.Data.Models;
using Unite.Donors.Feed.Data.Repositories;
using Unite.Essentials.Extensions;

namespace Unite.Donors.Feed.Data;

public class DonorsWriter : DonorsWriterBase<DonorModel, DonorsWriteAudit>
{
    private DonorRepository _donorRepository;
    private ProjectRepository _projectRepository;
    private ProjectDonorRepository _projectDonorRepository;
    private StudyRepository _studyRepository;
    private StudyDonorRepository _studyDonorRepository;


    public DonorsWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();
        Initialize(dbContext);
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _donorRepository = new DonorRepository(dbContext);
        _therapyRepository = new TherapyRepository(dbContext);
        _treatmentRepository = new TreatmentRepository(dbContext);
        _projectRepository = new ProjectRepository(dbContext);
        _projectDonorRepository = new ProjectDonorRepository(dbContext);
        _studyRepository = new StudyRepository(dbContext);
        _studyDonorRepository = new StudyDonorRepository(dbContext);
    }

    protected override void ProcessModel(DonorModel model, ref DonorsWriteAudit audit)
    {
        var donor = FindDonor(model);

        if (donor == null)
            donor = CreateDonor(model, ref audit);
        else
            donor = UpdateDonor(donor, model, ref audit);

        if (model.Treatments.IsNotEmpty())
            WriteTreatments(donor.Id, model.Treatments, ref audit);

        if (model.Projects.IsNotEmpty())
            WriteProjects(donor.Id, model.Projects, ref audit);
        
        if (model.Studies.IsNotEmpty())
            WriteStudies(donor.Id, model.Studies, ref audit);

        audit.Donors.Add(donor.Id);
    }


    private Donor FindDonor(DonorModel model)
    {
        return _donorRepository.Find(model);
    }

    private Donor CreateDonor(DonorModel model, ref DonorsWriteAudit audit)
    {
        var entity = _donorRepository.Create(model);

        audit.DonorsCreated++;

        return entity;
    }

    private Donor UpdateDonor(Donor entity, DonorModel model, ref DonorsWriteAudit audit)
    {
        _donorRepository.Update(entity, model);

        audit.DonorsUpdated++;

        return entity;
    }

    private void WriteProjects(int donorId, string[] names, ref DonorsWriteAudit audit)
    {
        var entities = _projectRepository.CreateMissing(names);
        audit.ProjectsCreated += entities.Count();

        var entries = _projectDonorRepository.RecreateAll(donorId, names);
        audit.ProjectDonorsCreated += entries.Count();
    }

    private void WriteStudies(int donorId, string[] names, ref DonorsWriteAudit audit)
    {
        var entities = _studyRepository.CreateMissing(names);
        audit.StudiesCreated += entities.Count();

        var entries = _studyDonorRepository.RecreateAll(donorId, names);
        audit.StudyDonorsCreated += entries.Count();
    }
}
