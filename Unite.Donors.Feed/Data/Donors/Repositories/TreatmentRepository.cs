using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Clinical;
using Unite.Data.Services;
using Unite.Donors.Feed.Data.Donors.Models;

namespace Unite.Donors.Feed.Data.Donors.Repositories
{
    internal class TreatmentRepository
    {
        private readonly UniteDbContext _dbContext;

        public TreatmentRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Treatment Find(int donorId, TreatmentModel treatmentModel)
        {
            var treatment = _dbContext.Treatments
                .Include(treatment => treatment.Therapy)
                .FirstOrDefault(treatment =>
                    treatment.DonorId == donorId &&
                    treatment.Therapy.Name == treatmentModel.Therapy &&
                    treatment.StartDay == treatmentModel.StartDay &&
                    treatment.DurationDays == treatmentModel.DurationDays
                );

            return treatment;
        }

        public Treatment Create(int donorId, TreatmentModel treatmentModel)
        {
            var treatment = new Treatment
            {
                DonorId = donorId
            };

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
            treatment.StartDay = treatmentModel.StartDay;
            treatment.DurationDays = treatmentModel.DurationDays;
            treatment.ProgressionStatus = treatmentModel.ProgressionStatus;
            treatment.ProgressionStatusChangeDay = treatmentModel.ProgressionStatusChangeDay;
            treatment.Results = treatmentModel.Results;
        }

        private Therapy GetTherapy(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var therapy = _dbContext.Therapies.FirstOrDefault(therapy =>
                therapy.Name == name
            );

            if (therapy == null)
            {
                therapy = new Therapy { Name = name };

                _dbContext.Therapies.Add(therapy);
                _dbContext.SaveChanges();
            }

            return therapy;
        }
    }
}
