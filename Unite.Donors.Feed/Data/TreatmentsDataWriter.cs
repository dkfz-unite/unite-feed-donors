using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Donors.Feed.Data.Exceptions;
using Unite.Donors.Feed.Data.Models;
using Unite.Donors.Feed.Data.Repositories;

namespace Unite.Donors.Feed.Data;

public class TreatmentsDataWriter : DataWriter<DonorModel, TreatmentsDataUploadAudit>
{
    private DonorRepository _donorRepository;
    private TreatmentRepository _treatmentRepository;
    

    public TreatmentsDataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _donorRepository = new DonorRepository(dbContext);
        _treatmentRepository = new TreatmentRepository(dbContext);
    }

    protected override void ProcessModel(DonorModel model, ref TreatmentsDataUploadAudit audit)
    {
        var donor = _donorRepository.Find(model) 
            ?? throw new NotFoundException($"Donor with id '{model.ReferenceId}' was not found");

        foreach (var treatmentModel in model.Treatments)
        {
            CreateOrUpdateTreatment(donor.Id, treatmentModel, ref audit);
        }

        audit.Donors.Add(donor.Id);
    }


    private Treatment CreateOrUpdateTreatment(int donorId, TreatmentModel model, ref TreatmentsDataUploadAudit audit)
    {
        var entity = _treatmentRepository.Find(donorId, model);

        if (entity == null)
        {
            _treatmentRepository.Create(donorId, model);
            audit.TreatmentsCreated++;
        }
        else
        {
            _treatmentRepository.Update(entity, model);
            audit.TreatmentsUpdated++;
        }

        return entity;
    }
}
