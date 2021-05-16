using System;
using Unite.Data.Entities.Clinical.Enums;

namespace Unite.Donors.DataFeed.Web.Models.Donors
{
    public class ClinicalDataModel
    {
		public Gender? Gender { get; set; }
		public int? Age { get; set; }
		public string Diagnosis { get; set; }
		public DateTime? DiagnosisDate { get; set; }
		public string PrimarySite { get; set; }
		public string Localization { get; set; }
		public bool? VitalStatus { get; set; }
		public DateTime? VitalStatusChangeDate { get; set; }
		public int? KpsBaseline { get; set; }
		public bool? SteroidsBaseline { get; set; }


		public void Sanitise()
		{
			Diagnosis = Diagnosis?.Trim();
			PrimarySite = PrimarySite?.Trim();
			Localization = Localization?.Trim();
		}
	}
}
