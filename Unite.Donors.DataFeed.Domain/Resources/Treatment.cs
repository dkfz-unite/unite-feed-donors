using System;

namespace Unite.Donors.DataFeed.Domain.Resources
{
    public class Treatment
    {
        public string Therapy { get; set; }
        public string Details { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Results { get; set; }

        public Data.Entities.Donors.Treatment ToEntity()
        {
            var treatment = new Data.Entities.Donors.Treatment();

            treatment.Details = Details;
            treatment.StartDate = StartDate;
            treatment.EndDate = EndDate;
            treatment.Results = Results;

            return treatment;
        }

        public Data.Entities.Donors.Therapy GetTherapy()
        {
            var therapy = new Data.Entities.Donors.Therapy();

            therapy.Name = Therapy;

            return therapy;
        }
    }
}
