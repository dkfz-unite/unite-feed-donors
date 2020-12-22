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

		public Data.Entities.Donors.ClinicalData ToEntity(string donorId)
        {
			var clinicalData = new Data.Entities.Donors.ClinicalData();

			clinicalData.DonorId = donorId;
			clinicalData.GenderId = Gender;
			clinicalData.Age = Age;
			clinicalData.AgeCategoryId = AgeCategory;
			clinicalData.Localization = GetLocalization(Localization);
			clinicalData.VitalStatusId = VitalStatus;
			clinicalData.VitalStatusChangeDate = VitalStatusChangeDate;
			clinicalData.SurvivalDays = SurvivalDays;
			clinicalData.ProgressionDate = ProgressionDate;
			clinicalData.ProgressionFreeDays = ProgressionFreeDays;
			clinicalData.RelapseDate = RelapseDate;
			clinicalData.RelapseFreeDays = RelapseFreeDays;
			clinicalData.KpsBaseline = KpsBaseline;
			clinicalData.SteroidsBaseline = SteroidsBaseline;

			return clinicalData;
        }

		public Data.Entities.Donors.Localization GetLocalization(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
				return null;
            }

			var localization = new Data.Entities.Donors.Localization();

			localization.Value = value;

			return localization;
        }
	}
}
