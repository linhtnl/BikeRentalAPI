using BikeRental.Data.ViewModels;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class TrackingBookingUtil
    {
        public static async Task<TrackingBookingViewModel> GetTrackingBookingByDate(Guid ownerId, DateTime date)
        {
            string formatedDate = date.ToString("yyyy-MM-dd");

            FirebaseClient firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var trackingBooking = await firebaseClient
                .Child("TrackingBookingPriority/" + ownerId + "/" + formatedDate)
                .OnceSingleAsync<TrackingBookingViewModel>();

            if (trackingBooking != null)
                return await Task.Run(() => trackingBooking);

            return null;
        }
    }
}
