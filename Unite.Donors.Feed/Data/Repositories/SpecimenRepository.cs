using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Specimens;
using Unite.Essentials.Extensions;

namespace Unite.Donors.Feed.Data.Repositories;

public class SpecimenRepository
{
    private readonly DomainDbContext _dbContext;


    public SpecimenRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public void DeleteByDonorId(int donorId)
    {
        var specimens = _dbContext.Set<Specimen>()
            .AsNoTracking()
            .Include(entity => entity.Children)
            .Where(entity => entity.ParentId == null)
            .Where(entity => entity.DonorId == donorId)
            .ToArray();

        specimens.ForEach(Delete);
        _dbContext.SaveChanges();
    }

    
    private void Delete(Specimen specimen)
    {
        specimen.Children.ForEach(Delete);

        var analyses = _dbContext.Set<AnalysedSample>()
            .AsNoTracking()
            .Include(entity => entity.Analysis)
            .Where(entity => entity.TargetSampleId == specimen.Id)
            .Select(entity => entity.Analysis)
            .DistinctBy(entity => entity.Id)
            .ToArray();

        _dbContext.Remove(specimen);
        _dbContext.RemoveRange(analyses);
    }
}
