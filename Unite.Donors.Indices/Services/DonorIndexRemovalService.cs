using Unite.Data.Context.Repositories;
using Unite.Indices.Context;

using DonorIndex = Unite.Indices.Entities.Donors.DonorIndex;
using ImageIndex = Unite.Indices.Entities.Images.ImageIndex;
using SpecimenIndex = Unite.Indices.Entities.Specimens.SpecimenIndex;

namespace Unite.Donors.Indices.Services;

public class DonorIndexRemovalService
{
    private readonly DonorsRepository _donorsRepository;
    private readonly IIndexService<DonorIndex> _donorsIndexService;
    private readonly IIndexService<ImageIndex> _imagesIndexService;
    private readonly IIndexService<SpecimenIndex> _specimensIndexService;
    

    public DonorIndexRemovalService(
        DonorsRepository donorsRepository,
        IIndexService<DonorIndex> donorsIndexService,
        IIndexService<ImageIndex> imagesIndexService,
        IIndexService<SpecimenIndex> specimensIndexService)
    {
        _donorsRepository = donorsRepository;
        _donorsIndexService = donorsIndexService;
        _imagesIndexService = imagesIndexService;
        _specimensIndexService = specimensIndexService;
    }


    public void DeleteIndex(object key)
    {
        var id = (int)key;
        var donor = key.ToString();
        var images = _donorsRepository.GetRelatedImages([id]).Result.Select(id => id.ToString());
        var specimens = _donorsRepository.GetRelatedSpecimens([id]).Result.Select(id => id.ToString());

        _donorsIndexService.Delete(donor);
        _imagesIndexService.DeleteRange(images);
        _specimensIndexService.DeleteRange(specimens);
    }
}
