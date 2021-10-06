using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.RequestModels;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IPaymentService : IBaseService<Payment>
    {
        Task<Payment> CreateNew(PaymentCreateRequest request);
        Task<Payment> UpdatePayment(Guid id, PaymentUpdateRequest request);
        Task<PaymentViewModel> GetById(Guid id);
    }
    public class PaymentService : BaseService<Payment>, IPaymentService
    {
        IConfigurationProvider _mapper;
        public PaymentService(IUnitOfWork unitOfWork, IPaymentRepository paymentRepository, IMapper mapper) : base(unitOfWork, paymentRepository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<Payment> CreateNew(PaymentCreateRequest request)
        {
            try
            {
                var payment = _mapper.CreateMapper().Map<Payment>(request);
                await CreateAsync(payment);
                return payment;
            }
            catch
            {
                return null;
            }
        }

        public async Task<PaymentViewModel> GetById(Guid id)
        {
            return await Get().Where(tempPayment => tempPayment.Id.Equals(id))
                .ProjectTo<PaymentViewModel>(_mapper)
                .FirstOrDefaultAsync();
        }

        public async Task<Payment> UpdatePayment(Guid id, PaymentUpdateRequest request)
        {
            var tempPayment = await GetById(id);
            if (tempPayment == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Payment not found.");

            var targetPayment = _mapper.CreateMapper().Map<Payment>(request);
            await UpdateAsync(targetPayment);
            return targetPayment;
        }
    }
}
