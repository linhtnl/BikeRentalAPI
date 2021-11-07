using BikeRental.Business.Constants;
using BikeRental.Business.RequestModels;
using BikeRental.Business.Services;
using BikeRental.Data.Repositories;
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
                        {"json", jsonConvert },
                        {"click_action", "FLUTTER_NOTIFICATION_CLICK" }
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

        public static async Task<bool> CancelBooking(Guid userId, int role)
        {
            if (role == (int)RoleConstants.Admin)
                return false;

            var registrationId = (role == (int)RoleConstants.Customer) 
                ? await TrackingRegistrationIdUtil.GetCustomerRegistrationId(userId) 
                : await TrackingRegistrationIdUtil.GetOwnerRegistrationId(userId);

            if (registrationId == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Cannot find registrationId of this user.");

            using (var sender = new Sender("AAAA0prIK9I:APA91bEsuL_KqXRNgkFhS8MMnDDncrX2p1ZhwUGyz0AAOrUoaaiCh6m4IifKdNpY6zA-PkSzdvQ7BOOJt2PtcznQpsLHZ3Fgx5Fk3v6EEvNf6_SlYjAP_8jDR1NyvAQ4LFoob8yxOxUm"))
            {
                var message = new Message
                {
                    RegistrationIds = new List<string> { registrationId.RegistrationId },
                    Notification = new Notification
                    {
                        Title = "Your booking has been canceled"
                    }
                };
                var result = await sender.SendAsync(message);
                Console.WriteLine($"Success: {result.MessageResponse.Success}");

                return true;
            }
        }

        public static async Task<bool> SendTakenBike(Guid customerId, Guid bikeId, int role, IBikeService bikeService)
        {
            if (role != (int)RoleConstants.Owner)
                return false;

            var registrationId = await TrackingRegistrationIdUtil.GetCustomerRegistrationId(customerId);

            if (registrationId == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Cannot find registrationId of this user.");

            var bike = await bikeService.GetBikeById(bikeId);

            string jsonConvert = JsonConvert.SerializeObject(new TakenBikeViewModel
            {
                LicensePlate = bike.LicensePlate, 
                Color = bike.Color, 
                ImgPath = bike.ImgPath, 
                ModelYear = bike.ModelYear
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
                        Title = "Bike has been taken!"
                    }
                };
                var result = await sender.SendAsync(message);
                Console.WriteLine($"Success: {result.MessageResponse.Success}");

                return true;
            }
        }

        public static async Task<bool> SendReturnBike(Guid customerId, Guid bikeId, int role, IBikeService bikeService)
        {
            if (role != (int)RoleConstants.Owner)
                return false;

            var registrationId = await TrackingRegistrationIdUtil.GetCustomerRegistrationId(customerId);

            if (registrationId == null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Cannot find registrationId of this user.");

            var bike = await bikeService.GetBikeById(bikeId);

            string jsonConvert = JsonConvert.SerializeObject(new TakenBikeViewModel
            {
                LicensePlate = bike.LicensePlate,
                Color = bike.Color,
                ImgPath = bike.ImgPath,
                ModelYear = bike.ModelYear
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
                        Title = "Bike has been return!"
                    }
                };
                var result = await sender.SendAsync(message);
                Console.WriteLine($"Success: {result.MessageResponse.Success}");

                return true;
            }
        }
    }
}
