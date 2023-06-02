using Unite.Data.Entities.Images;
using Unite.Data.Extensions;
using Unite.Indices.Entities.Basic.Images;

namespace Unite.Donors.Indices.Services.Mappers;

internal class ImageIndexMapper
{
    internal void Map(in Image image, ImageIndex index, DateOnly? diagnosisDate)
    {
        if (image == null)
        {
            return;
        }

        index.Id = image.Id;
        // index.ReferenceId - resolved by property getter
        // index.Type - resolved by property getter
        index.ScanningDay = image.ScanningDate.RelativeFrom(diagnosisDate) ?? image.ScanningDay;

        index.Mri = CreateFrom(image.MriImage);
    }


    private static MriImageIndex CreateFrom(in MriImage mriImage)
    {
        if (mriImage == null)
        {
            return null;
        }

        var index = new MriImageIndex();

        index.ReferenceId = mriImage.ReferenceId;

        index.WholeTumor = mriImage.WholeTumor;
        index.ContrastEnhancing = mriImage.ContrastEnhancing;
        index.NonContrastEnhancing = mriImage.NonContrastEnhancing;

        index.MedianAdcTumor = mriImage.MedianAdcTumor;
        index.MedianAdcCe = mriImage.MedianAdcCe;
        index.MedianAdcEdema = mriImage.MedianAdcEdema;

        index.MedianCbfTumor = mriImage.MedianCbfTumor;
        index.MedianCbfCe = mriImage.MedianCbfCe;
        index.MedianCbfEdema = mriImage.MedianCbfEdema;

        index.MedianCbvTumor = mriImage.MedianCbvTumor;
        index.MedianCbvCe = mriImage.MedianCbvCe;
        index.MedianCbvEdema = mriImage.MedianCbvEdema;

        index.MedianMttTumor = mriImage.MedianMttTumor;
        index.MedianMttCe = mriImage.MedianMttCe;
        index.MedianMttEdema = mriImage.MedianMttEdema;

        return index;
    }
}
