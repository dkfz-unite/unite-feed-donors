using System.Collections.Generic;
using System.Text;

namespace Unite.Donors.Feed.Donors.Data.Models.Audit
{
    public class DonorsUploadAudit
    {
        public int DonorsCreated;
        public int DonorsUpdated;
        public int ClinicalDataEntriesCreated;
        public int ClinicalDataEntriesUpdated;
        public int TreatmentsCreated;
        public int TreatmentsUpdated;
        public int WorkPackagesAssociated;
        public int StudiesAssociated;

        public HashSet<int> Donors;
        

        public DonorsUploadAudit()
        {
            Donors = new HashSet<int>();
        }

        public override string ToString()
        {
            var message = new StringBuilder();

            message.AppendLine($"{DonorsCreated} donors created");
            message.AppendLine($"{DonorsUpdated} donors updated");
            message.AppendLine($"{ClinicalDataEntriesCreated} donor clinical data entries created");
            message.AppendLine($"{ClinicalDataEntriesUpdated} donor clinical data entries updated");
            message.AppendLine($"{TreatmentsCreated} donor treatments created");
            message.AppendLine($"{TreatmentsUpdated} donor treatments updated");
            message.AppendLine($"{WorkPackagesAssociated} donor work packages associated");
            message.Append($"{StudiesAssociated} donor studies associated");

            return message.ToString();
        }
    }
}
