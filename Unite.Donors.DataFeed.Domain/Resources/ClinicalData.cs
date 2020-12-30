using System;
using Unite.Data.Entities.Donors.Enums;

namespace Unite.Donors.DataFeed.Domain.Resources
{
    public class ClinicalData
    {
		public Gender? Gender { get; set; }
		public int? Age { get; set; }
		public AgeCategory? AgeCategory { get; set; }
		public string Localization { get; set; }
		public VitalStatus? VitalStatus { get; set; }
		public DateTime? VitalStatusChangeDate { get; set; }
		public int? SurvivalDays { get; set; }
		public DateTime? ProgressionDate { get; set; }
		public int? ProgressionFreeDays { get; set; }
		public DateTime? RelapseDate { get; set; }
		public int? RelapseFreeDays { get; set; }
		public int? KpsBaseline { get; set; }
		public bool? SteroidsBaseline { get; set; }


		public void Sanitise()
        {
			Localization = Localization?.Trim();
        }
	}
}
