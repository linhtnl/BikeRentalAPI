
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Business.Constants;
using BikeRental.Business.Utilities;
using BikeRental.Data.Enums;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.Responses;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IAreaService : IBaseService<Area>
    {
        Task<AreaViewModel> GetById(Guid id);

        Task<DynamicModelResponse<AreaViewModel>> GetAll(AreaViewModel model, int pageNum);

        Task<AreaViewModel> Update(Guid id,int postalCode, string name);

        Task<AreaViewModel> Create(AreaCreateModel model);
    }
    public class AreaService : BaseService<Area>, IAreaService
    {
        private readonly IConfigurationProvider _mapper;
        public AreaService(IUnitOfWork unitOfWork, IAreaRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public async Task<DynamicModelResponse<AreaViewModel>> GetAll(AreaViewModel model, int pageNum)
        {
            //customer&owner => Status = Available
            //Admin => all

            //customer&owner
            var areas = Get().ProjectTo<AreaViewModel>(_mapper)
                .DynamicFilter(model)
                .PagingIQueryable(pageNum, GlobalConstants.GROUP_ITEM_NUM, CommonConstants.LimitPaging, CommonConstants.DefaultPaging);
            if (areas.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            var rs = new DynamicModelResponse<AreaViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = pageNum,
                    Size = GlobalConstants.GROUP_ITEM_NUM,
                    Total = areas.Item1
                },
                Data = await areas.Item2.ToListAsync()
            };
            return rs;
        }

        public async Task<AreaViewModel> Update(Guid id, int postalCode, string name)
        {
            //only admin
            var listArea = Get().ToList();
            foreach (var a in listArea)
            {
                if (a.Name.Equals(name) || a.PostalCode == postalCode) throw new ErrorResponse((int)HttpStatusCode.UnprocessableEntity, "Invalid Data");
            }
            Area area = Get(a => a.Id.Equals(id)).First();
            area.PostalCode = postalCode;
            area.Name = name;
            await UpdateAsync(area);
            var result = _mapper.CreateMapper().Map<AreaViewModel>(area);
            return result;
        }

        public async Task<AreaViewModel> Create(AreaCreateModel model)
        {
            //only admin
            var listArea = Get().ToList();
            foreach(var a in listArea)
            {
                if(a.Name.Equals(model.Name)||a.PostalCode==model.PostalCode) throw new ErrorResponse((int)HttpStatusCode.UnprocessableEntity, "Invalid Data");
            }
            var area = _mapper.CreateMapper().Map<Area>(model);
            await CreateAsync(area);
            var result = _mapper.CreateMapper().Map<AreaViewModel>(area);
            return result;
        }

        public async Task<AreaViewModel> GetById(Guid id)
        {
            var area = await Get(a => a.Id.Equals(id)).ProjectTo<AreaViewModel>(_mapper).FirstOrDefaultAsync();
            if(area == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not Found");
            return area;
        }
    }
}
