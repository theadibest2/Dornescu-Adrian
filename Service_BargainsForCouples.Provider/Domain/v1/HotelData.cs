using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service_BargainsForCouples.Provider.Domain.v1
{    
    public class HotelData
    {
        [JsonProperty("propertyID")]
        public int propertyId { get; set; }
        [JsonProperty("name")]
        public string name { get; set; } = string.Empty;
        [JsonProperty("geoId")]
        public int geoId { get; set; }
        [JsonProperty("rating")]
        public int rating { get; set; }
    }
}
