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

        //Thêm tham số truyền vào (denied or accept)
        public static async Task<bool> UpdateTrackingBooking(Guid ownerId, DateTime date, bool isAccepted)
        {
            string formatedDate = date.ToString("yyyy-MM-dd");

            FirebaseClient firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var trackingBooking = await firebaseClient
                .Child("TrackingBookingPriority/" + ownerId + "/" + formatedDate)
                .OnceSingleAsync<TrackingBookingViewModel>();

            if (trackingBooking == null)
            {
                await firebaseClient
                    .Child("TrackingBookingPriority/" + ownerId + "/" + formatedDate)
                    .PutAsync(new TrackingBookingViewModel()
                    {
                        BookingTimes = (isAccepted) ? 1 : 0,
                        DeniedTimes = (!isAccepted) ? 1 : 0
                    });

                return await Task.Run(() => true);
            } else
            {
                await firebaseClient
                    .Child("TrackingBookingPriority/" + ownerId + "/" + formatedDate)
                    .PutAsync(new TrackingBookingViewModel()
                    {
                        BookingTimes = (isAccepted) ? trackingBooking.BookingTimes + 1 : trackingBooking.BookingTimes,
                        DeniedTimes = (!isAccepted) ? trackingBooking.DeniedTimes + 1 : trackingBooking.DeniedTimes
                    });
                return await Task.Run(() => true);
            }
        }

        public static async Task<bool> UpdateTrackingBooking(Guid ownerId, DateTime dayRent, DateTime dayReturn)
        {
            int count = 0;

            while (DateTime.Compare(dayRent.AddDays(count), dayReturn) <= 0)
            {
                var isCompleted = await UpdateTrackingBooking(ownerId, dayRent.AddDays(count), true);

                count++;
            }

            return await Task.Run(() => true);
        }
    }
}
