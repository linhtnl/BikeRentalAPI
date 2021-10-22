using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using Firebase.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class DistanceUtil
    {
        private static async Task<List<OwnerByAreaViewModel>> GetUserLocations(List<OwnerByAreaViewModel> suitableOwners)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");

            foreach (var temp in suitableOwners)
            {
                var ownerLocations = await firebaseClient
                .Child("locations/owners/" + temp.Id)
                .OnceSingleAsync<DistanceViewModel>();

                if (ownerLocations != null)
                {
                    temp.LocationInfo = new DistanceViewModel
                    {
                        Latitude = ownerLocations.Latitude,
                        Longitude = ownerLocations.Longitude
                    };
                }
            }

            return await Task.Run(() => suitableOwners);
        }

        private static async Task<string> GetCustomerLocation(Guid customerId)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var customerLocations = await firebaseClient
                .Child("locations/customers/" + customerId)
                .OnceSingleAsync<DistanceViewModel>();

            if (customerLocations != null)
            {
                string locationString = customerLocations.Latitude + "," + customerLocations.Longitude;

                return await Task.Run(() => locationString);
            }

            return await Task.Run(() => String.Empty);
        }

        public static async Task<List<OwnerByAreaViewModel>> OrderByDistance(List<OwnerByAreaViewModel> suitableOwners, Guid customerId)
        {
            suitableOwners = await GetUserLocations(suitableOwners);

            string customerLocation = await GetCustomerLocation(customerId);

            if (String.IsNullOrEmpty(customerLocation))
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This customer haven't had location yet.");

            string key = "MkP5cLaGx6mRgulWqb7dSEkFPlZqLsCDNq1ZUku1";

            // 0 - start ; 1 - end; 2 - key
            string requestUrl = "https://rsapi.goong.io/DistanceMatrix?origins={0}&destinations={1}&vehicle=bike&api_key={2}";

            HttpClient client = new HttpClient();

            foreach (var temp in suitableOwners)
            {
                if (temp.LocationInfo == null)
                    continue;

                string ownerLocation = temp.LocationInfo.Latitude + "," + temp.LocationInfo.Longitude;

                HttpResponseMessage response = await client.GetAsync(string.Format(requestUrl, ownerLocation, customerLocation, key));

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var root = JObject.Parse(responseString);

                    var distanceValue = root.SelectToken("$.rows")[0].SelectToken("elements")[0].SelectToken("distance.value").Value<int>();

                    temp.LocationInfo.Distance = distanceValue;

                }
            }
            var result = suitableOwners.AsQueryable().OrderBy(temp => temp.LocationInfo.Distance);
            return result.ToList();
            /*return await Task.Run(() => result.ToList());*/
        }

        //private static async Task<List<OwnerByAreaViewModel>> OrderByDistance(List<OwnerByAreaViewModel> suitableOwners)
        //{
        //    List<OwnerByAreaViewModel> sortedSuitableOwners = new List<OwnerByAreaViewModel>();

        //    for (int i = 0; i < suitableOwners.Count; i++)
        //    {
        //        if (suitableOwners[i].LocationInfo == null)
        //            continue;

        //        bool isNearestOwner = true;
        //        for (int j = 0; j < suitableOwners.Count; j++)
        //        {
        //            if (i == j || suitableOwners[j].LocationInfo == null)
        //                continue;

        //            if (suitableOwners[i].LocationInfo.Distance > suitableOwners[j].LocationInfo.Distance)
        //                isNearestOwner = false;
        //        }

        //        if (isNearestOwner)
        //            sortedSuitableOwners.Add(suitableOwners[i]);
        //    }

        //    return await Task.Run(() => sortedSuitableOwners);
        //}
    }
}
