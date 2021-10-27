using BikeRental.Business.RequestModels;
using BikeRental.Data.Responses;
using BikeRental.Data.ViewModels;
using FCM.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class NotificationUtil
    {
        public static async Task<bool> SendOwnerNotification (string registrationId, CustomerRequestModel request)
        {
            string jsonConvert = JsonConvert.SerializeObject(request);
            using (var sender = new Sender("AAAA0prIK9I:APA91bEsuL_KqXRNgkFhS8MMnDDncrX2p1ZhwUGyz0AAOrUoaaiCh6m4IifKdNpY6zA-PkSzdvQ7BOOJt2PtcznQpsLHZ3Fgx5Fk3v6EEvNf6_SlYjAP_8jDR1NyvAQ4LFoob8yxOxUm"))
            {
                var message = new Message
                {
                    Data = new Dictionary<string, string>()
                    {
                        {"json",jsonConvert },
                    },
                    RegistrationIds = new List<string> { registrationId },
                    Notification = new Notification
                    {
                        Title = "You got a new Order!",
                        Body = "Check it now"
                    }
                };
                var result = await sender.SendAsync(message);
                Console.WriteLine($"Success: {result.MessageResponse.Success}");

               /* var json = "{\"notification\":{\"title\":\"json message\",\"body\":" + jsonConvert + "\"},\"to\":\"" + registrationId + "\"}";
                result = await sender.SendAsync(json);
                Console.WriteLine($"Success: {result.MessageResponse.Success}");*/
                return true;
            }
        }

        public static async Task<bool> ReplyBookingCustomerNotification(ReplyBookingRequest request)
        {
            var registrationId = await TrackingRegistrationIdUtil.GetCustomerRegistrationId(request.CustomerId);

            if (registrationId == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Cannot find registrationId of this user.");

            string jsonConvert = JsonConvert.SerializeObject(new ReplyBookingResponseViewModel
            {
                OwnerId = request.OwnerId,
                IsAccepted = request.IsAccepted
            });

            using (var sender = new Sender("AAAA0prIK9I:APA91bEsuL_KqXRNgkFhS8MMnDDncrX2p1ZhwUGyz0AAOrUoaaiCh6m4IifKdNpY6zA-PkSzdvQ7BOOJt2PtcznQpsLHZ3Fgx5Fk3v6EEvNf6_SlYjAP_8jDR1NyvAQ4LFoob8yxOxUm"))
            {
                var message = new Message
                {
                    Data = new Dictionary<string, string>()
                    {
                        {"data", jsonConvert },
                    },
                    RegistrationIds = new List<string> { registrationId.RegistrationId },
                    Notification = new Notification
                    {
                        Title = "You got an owner's response!",
                        Body = "Check it now"
                    }
                };
                var result = await sender.SendAsync(message);
                Console.WriteLine($"Success: {result.MessageResponse.Success}");

                return true;
            }
        }
    }
}
