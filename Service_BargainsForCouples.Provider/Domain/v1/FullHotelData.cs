using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service_BargainsForCouples.Provider.Domain.v1
{
    public class FullHotelData
    {
        [JsonProperty("hotel")]
        public HotelData hotel { get; set; }

        [JsonProperty("rates")]
        public List<RateData> rates { get; set; }
    }
    
}
