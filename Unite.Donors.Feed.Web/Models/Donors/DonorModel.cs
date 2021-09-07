using Unite.Data.Extensions;

namespace Unite.Donors.Feed.Web.Services.Donors
{
    public class DonorModel
    {
        public string Id { get; set; }
        public bool? MtaProtected { get; set; }

        public ClinicalDataModel ClinicalData { get; set; }

        public TreatmentModel[] Treatments { get; set; }
        public string[] WorkPackages { get; set; }
        public string[] Studies { get; set; }

        public void Sanitise()
        {
            Id = Id?.Trim();

            ClinicalData.Sanitise();

            Treatments?.ForEach(treatment => treatment.Sanitise());
            WorkPackages?.ForEach(workpackageName => workpackageName.Trim());
            Studies?.ForEach(studyName => studyName.Trim());
        }
    }
}
