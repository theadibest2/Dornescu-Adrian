using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service_BargainsForCouples.Provider.Domain.v1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Service_BargainsForCouples.Data.Entities.v1;

namespace Service_BargainsForCouples.Provider.Service.v1
{
    public class Provider : IProvider
    {
        private IHttpClientFactory _clientFactory;

        public Provider(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }


        /// <summary>
        /// ProcessUrlAsync - Loads the hotels data from a specific url and calculate the rates based on the number of nights
        /// </summary>
        /// <param name="url"></param>
        /// <param name="noNights"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<HotelEntity>> ProcessUrlAsync(string url, int noNights, CancellationToken token)
        {
            List<HotelEntity> returnList = new List<HotelEntity>();
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request, token);
            if (!response.IsSuccessStatusCode)
                return returnList;

            var rawData = await Task.Run(async () => JsonConvert.DeserializeObject<List<FullHotelData>>(await response.Content.ReadAsStringAsync()), token); // May take a long time, so that is why we should have a way to cancel the deserialization process            
            if (rawData != null)
            {
                rawData.ForEach(hotel =>
                {
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    //For each hotel we may have the same board type multiple types( Vlad answer -  board type-ul poate aparea de mai multe ori pentru acelasi hotel)                    
                    //We are grouping them in order to calculate their total sum
                    var rates = (from rate in hotel.rates
                                 group rate by rate.boardType into g
                                 select new RateEntity { boardType = g.Key, finalPrice = g.Sum(a => a.rateType == RateType.PerNight.ToString() ? a.value * noNights : a.value) }).ToList();
                    returnList.Add(new HotelEntity() { id = hotel.hotel.propertyId, name = hotel.hotel.name, rates = rates });
                });
            }//endif
            return returnList;
        }//ProcessUrlAsync



        /// <summary>
        /// Performing the request with zip compression (because server supports it)
        /// For the current set of data this method is slower
        /// But in theory for large amount of data the it should be faster
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> ProcessUrlZipAsync(string url, CancellationToken token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept-Encoding", "gzip");

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request, token);
            var rawData = await Task.Run(async () =>
            {
                var bytes = Decompress(await response.Content.ReadAsByteArrayAsync());
                using (var ms = new MemoryStream(bytes))
                using (var streamReader = new StreamReader(ms))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    return (JArray)JToken.ReadFrom(jsonReader);
                }
            }, token); // May take a long time, so that is why we should have a way to cancel the deserialization process

            return string.Empty;//No results are processed. The method was used for testing request/response speed 
        }//ProcessUrlZipAsync

        private static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }//Decompress

    }
}
