using System.Collections.Generic;
using Unite.Donors.DataFeed.Domain.Resources;

namespace Unite.Donors.DataFeed.Web.Services
{
    public interface IDataFeedService
    {
        void ProcessDonors(IEnumerable<Donor> donors);
    }
}