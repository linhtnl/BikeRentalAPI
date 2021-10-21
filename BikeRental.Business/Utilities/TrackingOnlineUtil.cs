using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class TrackingOnlineUtil
    {
        FirebaseClient firebaseClient = null;

        public TrackingOnlineUtil()
        {
            firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");
        }

        public async Task<bool> TrackNewUserLogin(Guid id)
        {
            try
            {
                await firebaseClient
                    .Child("TrackingOnline/" + id)
                    .PutAsync(new TrackingOnlineViewModel()
                    {
                        LoginTime = DateTime.UtcNow,
                        ExpiredTime = DateTime.UtcNow.AddHours(2)
                    });

                return true;
            }
            catch
            {
                throw new ErrorResponse((int)HttpStatusCode.InternalServerError, "Something went wrong");
            }
        }

        public async Task<DateTime?> GetUserExpiredTime(Guid id)
        {
            var ownerLogins = await firebaseClient
                .Child("TrackingOnline/" + id)
                .OnceSingleAsync<TrackingOnlineViewModel>();

            if (ownerLogins == null)
                return null;

            return await Task.Run(() => ownerLogins.ExpiredTime);
        }

        public static bool IsExpired(DateTime? expiredTime)
        {
            if (expiredTime != null && DateTime.Compare(DateTime.UtcNow, expiredTime.Value) >= 0)
            {
                return true;
            }
            return false;
        }
    }
}
