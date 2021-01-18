using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service_BargainsForCouples.Provider.Domain.v1
{
    public enum RateType
    {
        PerNight,
        Stay
    }

    public class RateData
    {
        [JsonProperty("rateType")]
        public string rateType { get; set; } = string.Empty;
        [JsonProperty("boardType")]
        public string boardType { get; set; } = string.Empty;
        [JsonProperty("value")]
        public decimal value { get; set; }
    }
}
