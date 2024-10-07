using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Donors.Feed.Data.Exceptions;
using Unite.Donors.Feed.Data.Models;
using Unite.Donors.Feed.Data.Repositories;

namespace Unite.Donors.Feed.Data;

public class TreatmentsWriter : DonorsWriterBase<DonorModel, TreatmentsWriteAudit>
{
    private DonorRepository _donorRepository;


    public TreatmentsWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();
        Initialize(dbContext);
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _donorRepository = new DonorRepository(dbContext);
        _therapyRepository = new TherapyRepository(dbContext);
        _treatmentRepository = new TreatmentRepository(dbContext);
    }

    protected override void ProcessModel(DonorModel model, ref TreatmentsWriteAudit audit)
    {
        var donor = _donorRepository.Find(model) 
            ?? throw new NotFoundException($"Donor with referenceId '{model.ReferenceId}' was not found");

        WriteTreatments(donor.Id, model.Treatments, ref audit);

        audit.Donors.Add(donor.Id);
    }
}
