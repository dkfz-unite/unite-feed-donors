using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Donors;

namespace Unite.Donors.Feed.Data.Repositories;

internal class StudyDonorRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly StudyRepository _studyRepository;


    public StudyDonorRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _studyRepository = new StudyRepository(dbContext);
    }


    public StudyDonor Find(int donorId, string name)
    {
        var study = _studyRepository.Find(name);

        if (study == null)
            return null;

        return _dbContext.Set<StudyDonor>().AsNoTracking().FirstOrDefault(entity =>
            entity.DonorId == donorId &&
            entity.StudyId == study.Id
        );
    }

    public IEnumerable<StudyDonor> CreateAll(int donorId, IEnumerable<string> names)
    {
        var entities = new List<StudyDonor>();

        foreach (var name in names)
        {
            var studyId = _studyRepository.FindOrCreate(name).Id;

            var entity = new StudyDonor()
            {
                DonorId = donorId,
                StudyId = studyId
            };

            entities.Add(entity);
        }

        if (entities.Any())
        {
            _dbContext.AddRange(entities);
            _dbContext.SaveChanges();
        }

        return entities;
    }

    public IEnumerable<StudyDonor> RecreateAll(int donorId, IEnumerable<string> names)
    {
        RemoveAll(donorId);

        return CreateAll(donorId, names);
    }


    private void RemoveAll(int donorId)
    {
        var entities = _dbContext.Set<StudyDonor>()
            .Where(entity => entity.DonorId == donorId)
            .ToArray();

        if (entities.Any())
        {
            _dbContext.RemoveRange(entities);
            _dbContext.SaveChanges();
        }
    }
}
