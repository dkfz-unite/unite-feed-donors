using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Donors.Data.Repositories
{
    internal class StudyRepository
    {
        private readonly UniteDbContext _dbContext;


        public StudyRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Study FindOrCreate(string name)
        {
            return Find(name) ?? Create(name);
        }

        public Study Find(string name)
        {
            var study = _dbContext.Studies.FirstOrDefault(study =>
                study.Name == name
            );

            return study;
        }

        public Study Create(string name)
        {
            var study = new Study
            {
                Name = name
            };

            _dbContext.Studies.Add(study);
            _dbContext.SaveChanges();

            return study;
        }
    }
}
