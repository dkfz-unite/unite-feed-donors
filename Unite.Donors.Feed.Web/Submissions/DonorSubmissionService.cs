using MongoDB.Bson;
using Npgsql.Replication;
using Unite.Cache.Configuration.Options;

namespace Unite.Donors.Feed.Web.Submissions;

public class DonorSubmissionService
{
    private readonly Repositories.DonorsSubmissionRepository _donorsSubmissionRepository;
    private readonly Repositories.TreatmentsSubmissionRepository _treatmentsSubmissionRepository;

    public DonorSubmissionService(IMongoOptions options)
	{
		_donorsSubmissionRepository = new Repositories.DonorsSubmissionRepository(options);
		_treatmentsSubmissionRepository = new Repositories.TreatmentsSubmissionRepository(options);
	}

    public string AddDonorSubmission(Models.Donors.DonorModel[] data)
	{
	 	return _donorsSubmissionRepository.Add(data);
	}

    public Models.Donors.DonorModel FindDonorSubmission(string id)
	{
		return _donorsSubmissionRepository.Find(id)?.Document.FirstOrDefault();
	}

    public void DeleteDonorSubmission(string id)
    {
        _donorsSubmissionRepository.Delete(id);
    }

    public string AddTreatmentsSubmission(Models.Donors.TreatmentsModel[] data)
	{
		return _treatmentsSubmissionRepository.Add(data);
	}

    public Models.Donors.TreatmentsModel FindTreatmentsSubmission(string id)
	{
		return _treatmentsSubmissionRepository.Find(id)?.Document.FirstOrDefault();
	}

    public void DeleteTreatmentssSubmission(string id)
    {
        _treatmentsSubmissionRepository.Delete(id);
    }
}