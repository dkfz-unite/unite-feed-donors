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


    public void Delete(params int[] ids)
    {
        var images = _dbContext.Set<Image>()
            .AsNoTracking()
            .Where(entity => ids.Contains(entity.Id))
            .ToArray();

        var analyses = _dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(entity => entity.Analysis)
            .Where(entity => ids.Contains(entity.SpecimenId))
            .Select(entity => entity.Analysis)
            .Distinct()
            .ToArray();

        _dbContext.RemoveRange(analyses);
        _dbContext.RemoveRange(images);
        _dbContext.SaveChanges();
    }
}
