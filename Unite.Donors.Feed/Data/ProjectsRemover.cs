using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services;
using Unite.Data.Entities.Donors;
using Unite.Donors.Feed.Data.Repositories;

namespace Unite.Donors.Feed.Data;

public class ProjectsRemover : DataWriter<Project>
{
    private readonly ProjectsRepository _projectsRepository;
    private ProjectRepository _projectRepository;
    private DonorRepository _donorRepository;
    private ImageRepository _imageRepository;
    private SpecimenRepository _specimenRepository;


    public ProjectsRemover(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _projectsRepository = new ProjectsRepository(dbContextFactory);

        var dbContext = dbContextFactory.CreateDbContext();

        Initialize(dbContext);
    }


    public Project Find(int id)
    {
        return _projectRepository.Find(id);
    }

    protected override void Initialize(DomainDbContext dbContext)
    {
        _projectRepository = new ProjectRepository(dbContext);
        _donorRepository = new DonorRepository(dbContext);
        _imageRepository = new ImageRepository(dbContext);
        _specimenRepository = new SpecimenRepository(dbContext);
    }

    protected override void ProcessModel(Project project)
    {
        var donorIds = _projectsRepository.GetRelatedDonors([project.Id]).Result;
        var specimenIds = _projectsRepository.GetRelatedSpecimens([project.Id]).Result;
        var imageIds = _projectsRepository.GetRelatedImages([project.Id]).Result;

        _specimenRepository.Delete(specimenIds);
        _imageRepository.Delete(imageIds);
        _donorRepository.Delete(donorIds);
        _projectRepository.Delete(project.Id);
    }
}
