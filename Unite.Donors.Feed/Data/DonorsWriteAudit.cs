namespace Unite.Donors.Feed.Data;

public class DonorsWriteAudit : DonorsWriteAuditBase
{
    public int DonorsCreated;
    public int DonorsUpdated;
    public int ProjectsCreated;
    public int ProjectDonorsCreated;
    public int StudiesCreated;
    public int StudyDonorsCreated;


    public override string ToString()
    {
        return string.Join(Environment.NewLine, [
            $"{DonorsCreated} donors created",
            $"{DonorsUpdated} donors updated",
            base.ToString(),
            $"{ProjectsCreated} projects created",
            $"{ProjectDonorsCreated} projects associated",
            $"{StudiesCreated} studies created",
            $"{StudyDonorsCreated} studies associated"
        ]);
    }
}
