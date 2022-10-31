using System.Text;

namespace Unite.Donors.Feed.Data.Donors.Models.Audit;

public class DonorsUploadAudit
{
    public int DonorsCreated;
    public int DonorsUpdated;
    public int ClinicalDataEntriesCreated;
    public int ClinicalDataEntriesUpdated;
    public int TreatmentsCreated;
    public int TreatmentsUpdated;
    public int ProjectsAssociated;
    public int StudiesAssociated;

    public HashSet<int> Donors;


    public DonorsUploadAudit()
    {
        Donors = new HashSet<int>();
    }

    public override string ToString()
    {
        var messages = new List<string>();

        messages.Add($"{DonorsCreated} donors created");
        messages.Add($"{DonorsUpdated} donors updated");
        messages.Add($"{ClinicalDataEntriesCreated} donor clinical data entries created");
        messages.Add($"{ClinicalDataEntriesUpdated} donor clinical data entries updated");
        messages.Add($"{TreatmentsCreated} donor treatments created");
        messages.Add($"{TreatmentsUpdated} donor treatments updated");
        messages.Add($"{ProjectsAssociated} donor projects associated");
        messages.Append($"{StudiesAssociated} donor studies associated");

        return string.Join(Environment.NewLine, messages);
    }
}
