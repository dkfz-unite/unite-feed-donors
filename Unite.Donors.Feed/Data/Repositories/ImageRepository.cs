using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Analysis;

namespace Unite.Donors.Feed.Data.Repositories;

public class ImageRepository
{
    private readonly DomainDbContext _dbContext;


    public ImageRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public void DeleteByDonorId(int donorId)
    {
        var images = _dbContext.Set<Image>()
            .AsNoTracking()
            .Where(entity => entity.DonorId == donorId)
            .ToArray();

        var analyses = _dbContext.Set<AnalysedSample>()
            .AsNoTracking()
            .Include(entity => entity.Analysis)
            .Where(entity => entity.TargetSample.DonorId == donorId)
            .Select(entity => entity.Analysis)
            .Distinct()
            .ToArray();

        _dbContext.RemoveRange(images);
        _dbContext.RemoveRange(analyses);
        _dbContext.SaveChanges();
    }
}
