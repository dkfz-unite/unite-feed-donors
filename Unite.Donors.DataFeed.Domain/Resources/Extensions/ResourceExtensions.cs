namespace Unite.Donors.DataFeed.Domain.Resources.Extensions
{
    public static class ResourceExtensions
    {
		public static Data.Entities.Donors.Donor GetDonor(this Donor donorResource)
		{
			var donor = new Data.Entities.Donors.Donor();

			donor.Id = donorResource.Pid;
			donor.Origin = donorResource.Origin;
			donor.MtaProtected = donorResource.MtaProtected;
			donor.PrimarySite = GetPrimarySite(donorResource.PrimarySite);
			donor.Diagnosis = donorResource.Diagnosis;
			donor.DiagnosisDate = donorResource.DiagnosisDate;

			return donor;
		}

		private static Data.Entities.Donors.PrimarySite GetPrimarySite(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return null;
			}

			var primarySite = new Data.Entities.Donors.PrimarySite();

			primarySite.Value = value;

			return primarySite;
		}


		public static Data.Entities.Donors.ClinicalData GetClinicalData(this ClinicalData clinicalDataResource, string donorId)
		{
			var clinicalData = new Data.Entities.Donors.ClinicalData();

			clinicalData.DonorId = donorId;
			clinicalData.GenderId = clinicalDataResource.Gender;
			clinicalData.Age = clinicalDataResource.Age;
			clinicalData.AgeCategoryId = clinicalDataResource.AgeCategory;
			clinicalData.Localization = GetLocalization(clinicalDataResource.Localization);
			clinicalData.VitalStatusId = clinicalDataResource.VitalStatus;
			clinicalData.VitalStatusChangeDate = clinicalDataResource.VitalStatusChangeDate;
			clinicalData.SurvivalDays = clinicalDataResource.SurvivalDays;
			clinicalData.ProgressionDate = clinicalDataResource.ProgressionDate;
			clinicalData.ProgressionFreeDays = clinicalDataResource.ProgressionFreeDays;
			clinicalData.RelapseDate = clinicalDataResource.RelapseDate;
			clinicalData.RelapseFreeDays = clinicalDataResource.RelapseFreeDays;
			clinicalData.KpsBaseline = clinicalDataResource.KpsBaseline;
			clinicalData.SteroidsBaseline = clinicalDataResource.SteroidsBaseline;

			return clinicalData;
		}

		public static Data.Entities.Epigenetics.EpigeneticsData GetEpigeneticsData(this EpigeneticsData epigeneticsDataResource, string donorId)
        {
			var epigeneticsData = new Data.Entities.Epigenetics.EpigeneticsData();

			epigeneticsData.DonorId = donorId;
			epigeneticsData.GeneExpressionSubtypeId = epigeneticsDataResource.GeneExpressionSubtype;
			epigeneticsData.IdhStatusId = epigeneticsDataResource.IdhStatus;
			epigeneticsData.IdhMutationId = epigeneticsDataResource.IdhMutation;
			epigeneticsData.MethylationStatusId = epigeneticsDataResource.MethylationStatus;
			epigeneticsData.MethylationSubtypeId = epigeneticsDataResource.MethylationSubtype;
			epigeneticsData.GcimpMethylation = epigeneticsDataResource.GcimpMethylation;

			return epigeneticsData;
		}

		private static Data.Entities.Donors.Localization GetLocalization(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return null;
			}

			var localization = new Data.Entities.Donors.Localization();

			localization.Value = value;

			return localization;
		}


		public static Data.Entities.Donors.Treatment GetTreatment(this Treatment treatmentResource)
		{
			var treatment = new Data.Entities.Donors.Treatment();

			treatment.Details = treatmentResource.Details;
			treatment.StartDate = treatmentResource.StartDate;
			treatment.EndDate = treatmentResource.EndDate;
			treatment.Results = treatmentResource.Results;

			return treatment;
		}

		public static Data.Entities.Donors.Therapy GetTherapy(this Treatment treatmentResource)
		{
			var therapy = new Data.Entities.Donors.Therapy();

			therapy.Name = treatmentResource.Therapy;

			return therapy;
		}
	}
}
