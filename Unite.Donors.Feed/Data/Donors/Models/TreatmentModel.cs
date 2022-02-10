﻿using System;

namespace Unite.Donors.Feed.Data.Donors.Models
{
    public class TreatmentModel
    {
        public string Therapy { get; set; }
        public string Details { get; set; }
        public DateTime? StartDate { get; set; }
        public int? StartDay { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DurationDays { get; set; }
        public bool? ProgressionStatus { get; set; }
        public DateTime? ProgressionStatusChangeDate { get; set; }
        public int? ProgressionStatusChangeDay { get; set; }
        public string Results { get; set; }
    }
}
