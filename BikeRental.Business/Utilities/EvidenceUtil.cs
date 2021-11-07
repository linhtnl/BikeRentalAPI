using BikeRental.Data.ViewModels;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class EvidenceUtil
    {
        public static async Task<bool> SaveEvidence(Guid bookingId, string path)
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");

                await firebaseClient
                    .Child("Evidences/" + bookingId)
                    .PutAsync(new EvidenceViewModel()
                    {
                        Path = path
                    });

                return await Task.Run(() => true);
            }
            catch
            {
                return await Task.Run(() => false);
            }
        }
    }
}
