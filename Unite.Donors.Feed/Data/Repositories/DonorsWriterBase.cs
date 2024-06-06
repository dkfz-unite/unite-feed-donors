using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Donors.Feed.Data.Models;

namespace Unite.Donors.Feed.Data.Repositories;

public abstract class DonorsWriterBase<TModel, TAudit> : Unite.Data.Context.Services.DataWriter<TModel, TAudit>
    where TModel : class
    where TAudit : DonorsWriteAuditBase, new()
{
    protected TherapyRepository _therapyRepository;
    protected TreatmentRepository _treatmentRepository;


    protected DonorsWriterBase(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }


    protected virtual void WriteTreatments(int donorId, TreatmentModel[] models, ref TAudit audit)
    {
        var names = models.Select(model => model.Therapy).Distinct().ToArray();
        var entities = _therapyRepository.CreateMissing(names);
        audit.TherapiesCreated += entities.Count();

        var entries = _treatmentRepository.RecreateAll(donorId, models);
        audit.TreatmentsCreated += entries.Count();
    }
}
