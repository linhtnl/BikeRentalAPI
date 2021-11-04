using BikeRental.Data.ViewModels;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class TrackingBookingTimeUtil
    {
        public static async Task<bool> UpdateBookingTime(Guid bookingId)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");

            await firebaseClient
                    .Child("TrackingBookingTime/" + bookingId)
                    .PutAsync(new TrackingBookingTimeViewModel()
                    {
                        BookingTime = DateTime.Now
                    });

            return await Task.Run(() => true);
        }

        public static async Task<TrackingBookingTimeViewModel> GetBookingTime(Guid bookingId)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var trackingBookingTime = await firebaseClient
                .Child("TrackingBookingTime/" + bookingId)
                .OnceSingleAsync<TrackingBookingTimeViewModel>();

            if (trackingBookingTime != null)
                return await Task.Run(() => trackingBookingTime);

            return null;
        }
    }
}
