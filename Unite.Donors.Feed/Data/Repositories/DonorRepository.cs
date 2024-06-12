using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Donors.Feed.Data.Models;

namespace Unite.Donors.Feed.Data.Repositories;

internal class DonorRepository
{
    private readonly DomainDbContext _dbContext;


    public DonorRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Donor Find(int id)
    {
        return _dbContext.Set<Donor>()
            .FirstOrDefault(entity =>
                entity.Id == id
            );
    }

    public Donor Find(DonorModel model)
    {
        return _dbContext.Set<Donor>()
            .Include(donor => donor.ClinicalData)
            .FirstOrDefault(entity =>
                entity.ReferenceId == model.ReferenceId
            );
    }

    public Donor Create(DonorModel model)
    {
        var entity = new Donor
        {
            ReferenceId = model.ReferenceId
        };

        Map(model, entity);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public void Update(Donor entity, DonorModel model)
    {
        Map(model, entity);

        _dbContext.Update(entity);
        _dbContext.SaveChanges();
    }

    public void Delete(Donor entity)
    {
        _dbContext.Remove(entity);
        _dbContext.SaveChanges();
    }


    private void Map(DonorModel source, Donor target)
    {
        target.MtaProtected = source.MtaProtected;

        if (source.ClinicalData != null)
        {
            if (target.ClinicalData == null)
                target.ClinicalData = new ClinicalData();

            target.ClinicalData.GenderId = source.ClinicalData.Gender;
            target.ClinicalData.Age = source.ClinicalData.Age;
            target.ClinicalData.Diagnosis = source.ClinicalData.Diagnosis;
            target.ClinicalData.DiagnosisDate = source.ClinicalData.DiagnosisDate;
            target.ClinicalData.PrimarySite = FindOrCreatePrimarySite(source.ClinicalData.PrimarySite);
            target.ClinicalData.Localization = FindOrCreateLocalization(source.ClinicalData.Localization);
            target.ClinicalData.VitalStatus = source.ClinicalData.VitalStatus;
            target.ClinicalData.VitalStatusChangeDate = source.ClinicalData.VitalStatusChangeDate;
            target.ClinicalData.VitalStatusChangeDay = source.ClinicalData.VitalStatusChangeDay;
            target.ClinicalData.ProgressionStatus = source.ClinicalData.ProgressionStatus;
        }
    }

    private TumorPrimarySite FindOrCreatePrimarySite(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var entity = _dbContext.Set<TumorPrimarySite>()
            .FirstOrDefault(entity =>
                entity.Value == value
            );

        if (entity == null)
        {
            entity = new TumorPrimarySite { Value = value };

            _dbContext.Add(entity);
            _dbContext.SaveChanges();
        }

        return entity;
    }

    private TumorLocalization FindOrCreateLocalization(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var entity = _dbContext.Set<TumorLocalization>()
            .FirstOrDefault(entity =>
                entity.Value == value
            );

        if (entity == null)
        {
            entity = new TumorLocalization { Value = value };

            _dbContext.Add(entity);
            _dbContext.SaveChanges();
        }

        return entity;
    }
}
