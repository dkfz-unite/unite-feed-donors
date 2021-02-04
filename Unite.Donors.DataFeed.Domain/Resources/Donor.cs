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
        public EpigeneticsData EpigeneticsData { get; set; }

        public Treatment[] Treatments { get; set; }

        public string[] WorkPackages { get; set; }
        public string[] Studies { get; set; }


        public void Sanitize()
        {
            Pid = Pid?.Trim();
            Origin = Origin?.Trim();
            PrimarySite = PrimarySite?.Trim();
            Diagnosis = Diagnosis?.Trim();

            ClinicalData?.Sanitise();
            EpigeneticsData?.Sanitise();

            if(Treatments != null)
            {
                foreach(var treatment in Treatments)
                {
                    treatment.Sanitise();
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
