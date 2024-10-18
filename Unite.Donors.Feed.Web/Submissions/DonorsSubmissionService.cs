using Unite.Cache.Configuration.Options;

namespace Unite.Donors.Feed.Web.Submissions;

public class DonorsSubmissionService
{
    private readonly Repositories.DonorsSubmissionRepository _donorsSubmissionRepository;
    private readonly Repositories.TreatmentsSubmissionRepository _treatmentsSubmissionRepository;

    public DonorsSubmissionService(IMongoOptions options)
	{
		_donorsSubmissionRepository = new Repositories.DonorsSubmissionRepository(options);
		_treatmentsSubmissionRepository = new Repositories.TreatmentsSubmissionRepository(options);
	}

    public string AddDonorsSubmission(Models.Donors.DonorModel[] data)
	{
	 	return _donorsSubmissionRepository.Add(data);
	}

    public Models.Donors.DonorModel[] FindDonorsSubmission(string id)
	{
		return _donorsSubmissionRepository.Find(id)?.Document;
	}

    public void DeleteDonorsSubmission(string id)
    {
        _donorsSubmissionRepository.Delete(id);
    }

    public string AddTreatmentsSubmission(Models.Donors.TreatmentsModel[] data)
	{
		return _treatmentsSubmissionRepository.Add(data);
	}

    public Models.Donors.TreatmentsModel[] FindTreatmentsSubmission(string id)
	{
		return _treatmentsSubmissionRepository.Find(id)?.Document;
	}

    public void DeleteTreatmentsSubmission(string id)
    {
        _treatmentsSubmissionRepository.Delete(id);
    }
}