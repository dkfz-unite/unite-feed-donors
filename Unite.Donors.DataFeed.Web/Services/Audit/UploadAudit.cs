using System.Text;

namespace Unite.Donors.DataFeed.Web.Services.Audit
{
    public class UploadAudit
    {
		public int DonorsCreated;
        public int DonorsUpdated;
		public int ClinicalDataCreated;
		public int ClinicalDataUpdated;
		public int TherapiesCreated;
		public int TreatmentsCreated;
		public int TreatmentsUpdated;
		public int WorkPackagesCreated;
		public int WorkPackagesAssociated;
		public int StudiesCreated;
		public int StudiesAssociated;

		public override string ToString()
		{
			var message = new StringBuilder();

			message.AppendLine($"{DonorsCreated} new donors created");
			message.AppendLine($"{DonorsUpdated} existing donors updated");
			message.AppendLine($"{ClinicalDataCreated} new donor clinical data created");
			message.AppendLine($"{ClinicalDataUpdated} existing donor clinical data updated");
			message.AppendLine($"{TherapiesCreated} new therapies created");
			message.AppendLine($"{TreatmentsCreated} new donor treatments created");
			message.AppendLine($"{TreatmentsUpdated} existing donor treatments updated");
			message.AppendLine($"{WorkPackagesCreated} new work packages created");
			message.AppendLine($"{WorkPackagesAssociated} new donor work package associations created");
			message.AppendLine($"{StudiesCreated} new studies created");
			message.AppendLine($"{StudiesAssociated} new donor study associations created");

			return message.ToString();
		}
	}
}
