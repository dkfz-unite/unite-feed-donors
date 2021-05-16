using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Donors.Data.Repositories
{
    internal class StudyDonorRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly StudyRepository _studyRepository;


        public StudyDonorRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _studyRepository = new StudyRepository(dbContext);
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
                Study = _studyRepository.FindOrCreate(studyName)
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
                        Study = _studyRepository.FindOrCreate(studyName)
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
    }
}
