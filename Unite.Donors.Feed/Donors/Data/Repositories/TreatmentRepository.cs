using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Clinical;
using Unite.Data.Services;
using Unite.Donors.Feed.Donors.Data.Models;

namespace Unite.Donors.Feed.Donors.Data.Repositories
{
    internal class TreatmentRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly TherapyRepository _therapyRepository;

        public TreatmentRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _therapyRepository = new TherapyRepository(dbContext);
        }


        public Treatment Find(int donorId, TreatmentModel treatmentModel)
        {
            var treatment = _dbContext.Treatments
                .Include(treatment => treatment.Therapy)
                .FirstOrDefault(treatment =>
                    treatment.DonorId == donorId &&
                    treatment.Therapy.Name == treatmentModel.Therapy &&
                    treatment.StartDate == treatmentModel.StartDate
                );

            return treatment;
        }

        public Treatment Create(int donorId, TreatmentModel treatmentModel)
        {
            var treatment = new Treatment();

            treatment.DonorId = donorId;

            Map(treatmentModel, treatment);

            _dbContext.Add(treatment);
            _dbContext.SaveChanges();

            return treatment;
        }

        public void Update(Treatment treatment, TreatmentModel treatmentModel)
        {
            Map(treatmentModel, treatment);

            _dbContext.Update(treatment);
            _dbContext.SaveChanges();
        }


        private void Map(TreatmentModel treatmentModel, Treatment treatment)
        {
            treatment.Therapy = GetTherapy(treatmentModel.Therapy);
            treatment.Details = treatmentModel.Details;
            treatment.StartDate = treatmentModel.StartDate;
            treatment.EndDate = treatmentModel.EndDate;
            treatment.ProgressionStatus = treatmentModel.ProgressionStatus;
            treatment.ProgressionStatusChangeDate = treatmentModel.ProgressionStatusChangeDate;
            treatment.Results = treatmentModel.Results;
        }

        private Therapy GetTherapy(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return _therapyRepository.FindOrCreate(name);
        }
    }
}
