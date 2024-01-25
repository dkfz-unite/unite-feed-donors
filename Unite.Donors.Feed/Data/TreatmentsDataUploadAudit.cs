namespace Unite.Donors.Feed.Data;

public class TreatmentsDataUploadAudit
{
    public int TreatmentsCreated;
    public int TreatmentsUpdated;

    public HashSet<int> Donors = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine, [
            $"{TreatmentsCreated} treatments created",
            $"{TreatmentsUpdated} treatments updated"
        ]);
    }
}
