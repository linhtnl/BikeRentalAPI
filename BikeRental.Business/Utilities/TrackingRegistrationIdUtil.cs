using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class TrackingRegistrationIdUtil
    {
        public static async Task<TrackingRegistrationIdViewModel> GetCustomerRegistrationId(Guid id)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var registrationId = await firebaseClient
                .Child("TrackingRegistrationId/customer/" + id)
                .OnceSingleAsync<TrackingRegistrationIdViewModel>();

            if (registrationId == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Cannot find registrationId of this user.");

            return await Task.Run(() => registrationId);
        }

        public static async Task<TrackingRegistrationIdViewModel> GetOwnerRegistrationId(Guid id)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var registrationId = await firebaseClient
                .Child("TrackingRegistrationId/owner/" + id)
                .OnceSingleAsync<TrackingRegistrationIdViewModel>();

            if (registrationId == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Cannot find registrationId of this user.");

            return await Task.Run(() => registrationId);
        }
    }
}
