using BikeRental.Data.ViewModels;
using Firebase.Database;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class PointConversionRateUtil
    {
        public static async Task<ConversionRateViewModel> GetConversionRate()
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient("https://chothuexemay-35838-default-rtdb.asia-southeast1.firebasedatabase.app/");

                var conversionRate = await firebaseClient
                    .Child("PointConversionRate")
                    .OnceSingleAsync<ConversionRateViewModel>();

                return await Task.Run(() => conversionRate);
            }
            catch
            {
                return null;
            }
        }
    }
}
