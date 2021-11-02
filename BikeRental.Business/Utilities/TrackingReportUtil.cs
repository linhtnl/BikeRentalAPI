using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class TrackingReportUtil
    {
        FirebaseClient firebaseClient = null;

        public TrackingReportUtil()
        {
            firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");
        }

        public async Task<bool> TrackNewReport(Guid id, TrackingReportViewModel reportModel)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                await firebaseClient
                    .Child("TrackingReports")
                    .Child(date)
                    .Child(id.ToString())
                    .PutAsync(reportModel);

                return await Task.Run(() => true);
            }
            catch
            {
                throw new ErrorResponse((int)HttpStatusCode.InternalServerError, "Something went wrong");
            }
        }
    }
}
