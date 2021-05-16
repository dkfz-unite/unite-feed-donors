using System.Linq;

namespace Unite.Donors.DataFeed.Web.Models.Donors.Converters
{
    public class DonorModelConverter
    {
        public Feed.Donors.Data.Models.DonorModel Convert(DonorModel source)
        {
            var donorModel = new Feed.Donors.Data.Models.DonorModel();

            Map(source, donorModel);

            if(source.ClinicalData != null)
            {
                donorModel.ClinicalData = new Feed.Donors.Data.Models.ClinicalDataModel();

                Map(source.ClinicalData, donorModel.ClinicalData);
            }

            if(source.Treatments != null)
            {
                donorModel.Treatments = source.Treatments.Select(treatment =>
                {
                    var treatmentModel = new Feed.Donors.Data.Models.TreatmentModel();

                    Map(treatment, treatmentModel);

                    return treatmentModel;

                }).ToArray();
            }

            return donorModel;
        }


        private static void Map(DonorModel source, Feed.Donors.Data.Models.DonorModel target)
        {
            target.ReferenceId = source.Id;
            target.MtaProtected = source.MtaProtected;

            target.WorkPackages = source.WorkPackages;
            target.Studies = source.Studies;
        }

        private static void Map(ClinicalDataModel source, Feed.Donors.Data.Models.ClinicalDataModel target)
        {
            target.Gender = source.Gender;
            target.Age = source.Age;
            target.Diagnosis = source.Diagnosis;
            target.DiagnosisDate = source.DiagnosisDate;
            target.PrimarySite = source.PrimarySite;
            target.Localization = source.Localization;
            target.VitalStatus = source.VitalStatus;
            target.VitalStatusChangeDate = source.VitalStatusChangeDate;
            target.KpsBaseline = source.KpsBaseline;
            target.SteroidsBaseline = source.SteroidsBaseline;
        }

        private static void Map(TreatmentModel source, Feed.Donors.Data.Models.TreatmentModel target)
        {
            target.Therapy = source.Therapy;
            target.Details = source.Details;
            target.StartDate = source.StartDate;
            target.EndDate = source.EndDate;
            target.ProgressionStatus = source.ProgressionStatus;
            target.ProgressionStatusChangeDate = source.ProgressionStatusChangeDate;
        }
    }
}
