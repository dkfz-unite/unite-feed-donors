using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Data.Donors.Repositories
{
    internal class StudyDonorRepository
    {
        private readonly DomainDbContext _dbContext;


        public StudyDonorRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public StudyDonor FindOrCreate(int donorId, string studyName)
        {
            return Find(donorId, studyName) ?? Create(donorId, studyName);
        }

        public StudyDonor Find(int donorId, string studyName)
        {
            var entity = _dbContext.Set<StudyDonor>()
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.Study.Name == studyName
                );

            return entity;
        }

        public StudyDonor Create(int donorId, string studyName)
        {
            var entity = new StudyDonor
            {
                DonorId = donorId,
                Study = GetStudy(studyName)
            };

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public IEnumerable<StudyDonor> CreateOrUpdate(int donorId, IEnumerable<string> studyNames)
        {
            RemoveRedundant(donorId, studyNames);

            var created = CreateMissing(donorId, studyNames);

            return created;
        }

        public IEnumerable<StudyDonor> CreateMissing(int donorId, IEnumerable<string> studyNames)
        {
            var entitiesToAdd = new List<StudyDonor>();

            foreach (var studyName in studyNames)
            {
                var entity = Find(donorId, studyName);

                if (entity == null)
                {
                    entity = new StudyDonor
                    {
                        DonorId = donorId,
                        Study = GetStudy(studyName)
                    };

                    entitiesToAdd.Add(entity);
                }
            }

            if (entitiesToAdd.Any())
            {
                _dbContext.AddRange(entitiesToAdd);
                _dbContext.SaveChanges();
            }

            return entitiesToAdd;
        }

        public void RemoveRedundant(int donorId, IEnumerable<string> studyNames)
        {
            var entitiesToRemove = _dbContext.Set<StudyDonor>()
                .Where(entity => entity.DonorId == donorId && !studyNames.Contains(entity.Study.Name))
                .ToArray();

            if (entitiesToRemove.Any())
            {
                _dbContext.RemoveRange(entitiesToRemove);
                _dbContext.SaveChanges();
            }
        }


        private Study GetStudy(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var entity = _dbContext.Set<Study>()
                .FirstOrDefault(study =>
                    study.Name == name
                );

            if (entity == null)
            {
                entity = new Study { Name = name };

                _dbContext.Add(entity);
                _dbContext.SaveChanges();
            }

            return entity;
        }
    }
}
