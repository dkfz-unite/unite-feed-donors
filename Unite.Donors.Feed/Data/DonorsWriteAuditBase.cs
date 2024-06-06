namespace Unite.Donors.Feed.Data;

public abstract class DonorsWriteAuditBase
{
    public int TherapiesCreated;
    public int TreatmentsCreated;


    public HashSet<int> Donors = [];


    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            $"{TherapiesCreated} therapies created",
            $"{TreatmentsCreated} therapies associated"
        ]);
    }
}
