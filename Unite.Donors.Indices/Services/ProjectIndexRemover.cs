using Unite.Data.Context.Repositories;
using Unite.Indices.Context;

using ProjectIndex = Unite.Indices.Entities.Projects.ProjectIndex;
using DonorIndex = Unite.Indices.Entities.Donors.DonorIndex;
using ImageIndex = Unite.Indices.Entities.Images.ImageIndex;
using SpecimenIndex = Unite.Indices.Entities.Specimens.SpecimenIndex;

namespace Unite.Donors.Indices.Services;

public class ProjectIndexRemover
{
    private readonly ProjectsRepository _projectsRepository;
    private readonly IIndexService<ProjectIndex> _projectsIndexService;
    private readonly IIndexService<DonorIndex> _donorsIndexService;
    private readonly IIndexService<ImageIndex> _imagesIndexService;
    private readonly IIndexService<SpecimenIndex> _specimensIndexService;


    public ProjectIndexRemover(
        ProjectsRepository projectsRepository,
        IIndexService<ProjectIndex> projectsIndexService,
        IIndexService<DonorIndex> donorsIndexService,
        IIndexService<ImageIndex> imagesIndexService,
        IIndexService<SpecimenIndex> specimensIndexService)
    {
        _projectsRepository = projectsRepository;
        _projectsIndexService = projectsIndexService;
        _donorsIndexService = donorsIndexService;
        _imagesIndexService = imagesIndexService;
        _specimensIndexService = specimensIndexService;
    }


    public void DeleteIndex(object key)
    {
        var id = (int)key;
        var project = key.ToString();
        var donors = _projectsRepository.GetRelatedDonors([id]).Result.Select(id => id.ToString());
        var images = _projectsRepository.GetRelatedImages([id]).Result.Select(id => id.ToString());
        var specimens = _projectsRepository.GetRelatedSpecimens([id]).Result.Select(id => id.ToString());

        _projectsIndexService.Delete(project);
        _donorsIndexService.DeleteRange(donors);
        _imagesIndexService.DeleteRange(images);
        _specimensIndexService.DeleteRange(specimens);
    }
}
