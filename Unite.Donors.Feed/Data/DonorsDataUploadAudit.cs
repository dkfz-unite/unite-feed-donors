namespace Unite.Donors.Feed.Data;

public class DonorsDataUploadAudit
{
    public int DonorsCreated;
    public int DonorsUpdated;
    public int ClinicalDataEntriesCreated;
    public int ClinicalDataEntriesUpdated;
    public int TreatmentsCreated;
    public int TreatmentsUpdated;
    public int ProjectsAssociated;
    public int StudiesAssociated;

    public HashSet<int> Donors = [];


    public override string ToString()
    {
        return string.Join(Environment.NewLine, [
            $"{StudiesAssociated} studies associated",
            $"{ProjectsAssociated} projects associated",
            $"{DonorsCreated} donors created",
            $"{DonorsUpdated} donors updated",
            $"{ClinicalDataEntriesCreated} clinical data entries created",
            $"{ClinicalDataEntriesUpdated} clinical data entries updated",
            $"{TreatmentsCreated} treatments created",
            $"{TreatmentsUpdated} treatments updated"
        ]);
    }
}
