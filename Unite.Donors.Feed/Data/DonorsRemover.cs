using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services;
using Unite.Data.Entities.Donors;
using Unite.Donors.Feed.Data.Repositories;

namespace Unite.Donors.Feed.Data;

public class DonorsRemover : DataWriter<Donor>
{
    private readonly DonorsRepository _donorsRepository;
    private DonorRepository _donorRepository;
    private ImageRepository _imageRepository;
    private SpecimenRepository _specimenRepository;


    public DonorsRemover(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _donorsRepository = new DonorsRepository(dbContextFactory);

        var dbContext = dbContextFactory.CreateDbContext();

        Initialize(dbContext);
    }


    public Donor Find(int id)
    {
        return _donorRepository.Find(id);
    }

    protected override void Initialize(DomainDbContext dbContext)
    {
        _donorRepository = new DonorRepository(dbContext);
        _imageRepository = new ImageRepository(dbContext);
        _specimenRepository = new SpecimenRepository(dbContext);
    }

    protected override void ProcessModel(Donor donor)
    {
        var specimenIds = _donorsRepository.GetRelatedSpecimens([donor.Id]).Result;
        var imageIds = _donorsRepository.GetRelatedImages([donor.Id]).Result;
        
        _specimenRepository.Delete(specimenIds);
        _imageRepository.Delete(imageIds);
        _donorRepository.Delete(donor.Id);
    }
}
