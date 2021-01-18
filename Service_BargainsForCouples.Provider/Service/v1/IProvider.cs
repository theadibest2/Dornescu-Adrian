using Service_BargainsForCouples.Data.Entities.v1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service_BargainsForCouples.Provider.Service.v1
{
    public interface IProvider
    {
        public Task<IEnumerable<HotelEntity>> ProcessUrlAsync(string url, int noNights, CancellationToken token);
    }
}
