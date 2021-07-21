﻿using System.Linq;

namespace Unite.Donors.DataFeed.Web.Models.Donors.Converters
{
    public class DonorModelConverter
    {
        public Feed.Data.Donors.Models.DonorModel Convert(DonorModel source)
        {
            var donorModel = new Feed.Data.Donors.Models.DonorModel();

            Map(source, donorModel);

            if(source.ClinicalData != null)
            {
                donorModel.ClinicalData = new Feed.Data.Donors.Models.ClinicalDataModel();

                Map(source.ClinicalData, donorModel.ClinicalData);
            }

            if(source.Treatments != null)
            {
                donorModel.Treatments = source.Treatments.Select(treatment =>
                {
                    var treatmentModel = new Feed.Data.Donors.Models.TreatmentModel();

                    Map(treatment, treatmentModel);

                    return treatmentModel;

                }).ToArray();
            }

            return donorModel;
        }


        private static void Map(DonorModel source, Feed.Data.Donors.Models.DonorModel target)
        {
            target.ReferenceId = source.Id;
            target.MtaProtected = source.MtaProtected;

            target.WorkPackages = source.WorkPackages;
            target.Studies = source.Studies;
        }

        private static void Map(ClinicalDataModel source, Feed.Data.Donors.Models.ClinicalDataModel target)
        {
            target.Gender = source.Gender;
            target.Age = source.Age;
            target.Diagnosis = source.Diagnosis;
            target.PrimarySite = source.PrimarySite;
            target.Localization = source.Localization;
            target.VitalStatus = source.VitalStatus;
            target.VitalStatusChangeDay = source.VitalStatusChangeDay;
            target.KpsBaseline = source.KpsBaseline;
            target.SteroidsBaseline = source.SteroidsBaseline;
        }

        private static void Map(TreatmentModel source, Feed.Data.Donors.Models.TreatmentModel target)
        {
            target.Therapy = source.Therapy;
            target.Details = source.Details;
            target.StartDay = source.StartDay;
            target.DurationDays = source.DurationDays;
            target.ProgressionStatus = source.ProgressionStatus;
            target.ProgressionStatusChangeDay = source.ProgressionStatusChangeDay;
            target.Results = source.Results;
        }
    }
}
