using Unite.Indices.Entities.Donors;

namespace Unite.Donors.DataFeed.Web.Services.Indices
{
    public interface IIndexCreationService
    {
        DonorIndex CreateIndex(string donorId);
    }
}