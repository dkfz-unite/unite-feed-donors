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
            var studyDonor = _dbContext.StudyDonors.FirstOrDefault(studyDonor =>
                studyDonor.DonorId == donorId &&
                studyDonor.Study.Name == studyName
            );

            return studyDonor;
        }

        public StudyDonor Create(int donorId, string studyName)
        {
            var studyDonor = new StudyDonor
            {
                DonorId = donorId,
                Study = GetStudy(studyName)
            };

            _dbContext.StudyDonors.Add(studyDonor);
            _dbContext.SaveChanges();

            return studyDonor;
        }

        public IEnumerable<StudyDonor> CreateMissing(int donorId, IEnumerable<string> studyNames)
        {
            var studyDonorsToAdd = new List<StudyDonor>();

            foreach (var studyName in studyNames)
            {
                var studyDonor = Find(donorId, studyName);

                if (studyDonor == null)
                {
                    studyDonor = new StudyDonor
                    {
                        DonorId = donorId,
                        Study = GetStudy(studyName)
                    };

                    studyDonorsToAdd.Add(studyDonor);
                }
            }

            if (studyDonorsToAdd.Any())
            {
                _dbContext.StudyDonors.AddRange(studyDonorsToAdd);
                _dbContext.SaveChanges();
            }

            return studyDonorsToAdd;
        }


        private Study GetStudy(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var study = _dbContext.Studies.FirstOrDefault(study =>
                study.Name == name
            );

            if (study == null)
            {
                study = new Study { Name = name };

                _dbContext.Studies.Add(study);
                _dbContext.SaveChanges();
            }

            return study;
        }
    }
}
