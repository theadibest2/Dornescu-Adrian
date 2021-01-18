using System;
using System.Collections.Generic;
using System.Text;

namespace Service_BargainsForCouples.Data.Entities.v1
{
    public class RateEntity : IEntity
    {
        public string boardType { get; set; } = string.Empty;
        public decimal finalPrice { get; set; }
    }
}
