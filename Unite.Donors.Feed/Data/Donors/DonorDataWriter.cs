using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Services;
using Unite.Donors.Feed.Data.Donors.Models;
using Unite.Donors.Feed.Data.Donors.Models.Audit;
using Unite.Donors.Feed.Data.Donors.Repositories;

namespace Unite.Donors.Feed.Data.Donors;

public class DonorDataWriter : DataWriter<DonorModel, DonorsUploadAudit>
{
    private readonly DonorRepository _donorRepository;
    private readonly ClinicalDataRepository _clinicalDataRepository;
    private readonly TreatmentRepository _treatmentRepository;
    private readonly ProjectDonorRepository _projectDonorRepository;
    private readonly StudyDonorRepository _studyDonorRepository;


    public DonorDataWriter(DomainDbContext dbContext) : base(dbContext)
    {
        _donorRepository = new DonorRepository(dbContext);
        _clinicalDataRepository = new ClinicalDataRepository(dbContext);
        _treatmentRepository = new TreatmentRepository(dbContext);
        _projectDonorRepository = new ProjectDonorRepository(dbContext);
        _studyDonorRepository = new StudyDonorRepository(dbContext);
    }


    protected override void ProcessModel(DonorModel model, ref DonorsUploadAudit audit)
    {
        var donor = CreateorUpdateDonor(model, ref audit);

        if (model.ClinicalData != null)
        {
            CreateOrUpdateClinicalData(donor.Id, model.ClinicalData, ref audit);
        }

        if (model.Treatments != null)
        {
            foreach (var treatmentModel in model.Treatments)
            {
                CreateOrUpdateTreatment(donor.Id, treatmentModel, ref audit);
            }
        }

        if (model.WorkPackages != null)
        {
            CreateOrUpdateProjects(donor.Id, model.WorkPackages, ref audit);
        }

        if (model.Studies != null)
        {
            CreateOrUpdateStudies(donor.Id, model.Studies, ref audit);
        }
    }


    private Donor CreateorUpdateDonor(DonorModel model, ref DonorsUploadAudit audit)
    {
        var entity = _donorRepository.Find(model);

        if (entity == null)
        {
            entity = _donorRepository.Create(model);

            audit.DonorsCreated++;
            audit.Donors.Add(entity.Id);
        }
        else
        {
            _donorRepository.Update(entity, model);

            audit.DonorsUpdated++;
            audit.Donors.Add(entity.Id);
        }

        return entity;
    }

    private ClinicalData CreateOrUpdateClinicalData(int donorId, ClinicalDataModel model, ref DonorsUploadAudit audit)
    {
        var entity = _clinicalDataRepository.Find(donorId);

        if (entity == null)
        {
            _clinicalDataRepository.Create(donorId, model);
            audit.ClinicalDataEntriesCreated++;
        }
        else
        {
            _clinicalDataRepository.Update(entity, model);
            audit.ClinicalDataEntriesUpdated++;
        }

        return entity;
    }

    private Treatment CreateOrUpdateTreatment(int donorId, TreatmentModel model, ref DonorsUploadAudit audit)
    {
        var entity = _treatmentRepository.Find(donorId, model);

        if (entity == null)
        {
            _treatmentRepository.Create(donorId, model);
            audit.TreatmentsCreated++;
        }
        else
        {
            _treatmentRepository.Update(entity, model);
            audit.TreatmentsUpdated++;
        }

        return entity;
    }

    private IEnumerable<ProjectDonor> CreateOrUpdateProjects(int donorId, IEnumerable<string> projectNames, ref DonorsUploadAudit audit)
    {
        var entities = _projectDonorRepository.CreateOrUpdate(donorId, projectNames);

        audit.ProjectsAssociated++;

        return entities;
    }

    private IEnumerable<StudyDonor> CreateOrUpdateStudies(int donorId, IEnumerable<string> studyNames, ref DonorsUploadAudit audit)
    {
        var entities = _studyDonorRepository.CreateOrUpdate(donorId, studyNames);

        audit.StudiesAssociated++;

        return entities;
    }
}
