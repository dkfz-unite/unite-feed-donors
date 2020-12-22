using System;
using System.Linq;

namespace Unite.Donors.DataFeed.Domain.Resources
{
    public class Donor
    {
        public string Pid { get; set; }
        public string Origin { get; set; }
        public bool? MtaProtected { get; set; }
        public string PrimarySite { get; set; }
        public string Diagnosis { get; set; }
        public DateTime? DiagnosisDate { get; set; }

        public ClinicalData ClinicalData { get; set; }

        public Treatment[] Treatments { get; set; }

        public string[] WorkPackages { get; set; }
        public string[] Studies { get; set; }


        public Data.Entities.Donors.Donor ToEntity()
        {
            var donor = new Data.Entities.Donors.Donor();

            donor.Id = Pid;
            donor.Origin = Origin;
            donor.MtaProtected = MtaProtected;
            donor.PrimarySite = GetPrimarySite(PrimarySite);
            donor.Diagnosis = Diagnosis;
            donor.DiagnosisDate = DiagnosisDate;

            return donor;
        }

        public Data.Entities.Donors.PrimarySite GetPrimarySite(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var primarySite = new Data.Entities.Donors.PrimarySite();

            primarySite.Value = value;

            return primarySite;
        }

        public void Sanitize()
        {
            Pid = Pid?.Trim();
            Origin = Origin?.Trim();
            PrimarySite = PrimarySite?.Trim();
            Diagnosis = Diagnosis?.Trim();

            if(ClinicalData != null)
            {
                ClinicalData.Localization = ClinicalData.Localization?.Trim();
            }

            if(Treatments != null)
            {
                foreach(var treatment in Treatments)
                {
                    treatment.Therapy = treatment.Therapy?.Trim();
                    treatment.Details = treatment.Details?.Trim();
                    treatment.Results = treatment.Results?.Trim();
                }
            }

            if(WorkPackages != null)
            {
                WorkPackages = WorkPackages
                    .Select(value => value.Trim())
                    .ToArray();
            }

            if(Studies != null)
            {
                Studies = Studies
                    .Select(value => value.Trim())
                    .ToArray();
            }
        }
    }
}
