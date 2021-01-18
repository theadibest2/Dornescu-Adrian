using System;
using System.Collections.Generic;
using System.Text;

namespace Service_BargainsForCouples.Data.Entities.v1
{
    public class HotelEntity : IEntity
    {
        public int id { get; set; }
      
        public string name { get; set; } = string.Empty;

        public List<RateEntity> rates { get; set; } = new List<RateEntity>();
    }
}
