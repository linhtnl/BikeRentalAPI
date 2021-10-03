
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BikeRental.Data.Models;
using BikeRental.Data.Repositories;
using BikeRental.Data.UnitOfWorks;
using BikeRental.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public interface IAreaService : IBaseService<Area>
    {
        AreaViewModel GetByName(string name);

        AreaViewModel GetAreaByPostalCode(int postalCode);

        AreaViewModel GetById(Guid id);

        List<AreaViewModel> GetAll();

        Task<Area> Update(Guid id,int postalCode, string name);

        Task<Area> Create(AreaCreateModel model);
    }
    public class AreaService : BaseService<Area>, IAreaService
    {
        private readonly IConfigurationProvider _mapper;
        public AreaService(IUnitOfWork unitOfWork, IAreaRepository repository, IMapper mapper) : base(unitOfWork, repository)
        {
            _mapper = mapper.ConfigurationProvider;
        }

        public List<AreaViewModel> GetAll()
        {
            return Get().ProjectTo<AreaViewModel>(_mapper).ToList();
        }

        public AreaViewModel GetById(Guid id)
        {
            return Get(a => a.Id.Equals(id)).ProjectTo<AreaViewModel>(_mapper).FirstOrDefault();
        }
        public AreaViewModel GetByName(string name)
        {
            return Get(a => a.Name.Equals(name)).ProjectTo<AreaViewModel>(_mapper).FirstOrDefault();
        }

        public AreaViewModel GetAreaByPostalCode(int postalCode)
        {
            return Get(a => a.PostalCode == postalCode).ProjectTo<AreaViewModel>(_mapper).FirstOrDefault();
        }

        public async Task<Area> Update(Guid id, int postalCode, string name)
        {
            Area area = Get(a => a.Id.Equals(id)).First();
            area.PostalCode = postalCode;
            area.Name = name;
            await UpdateAsync(area);
            return area;
        }

        public async Task<Area> Create(AreaCreateModel model)
        {
            var area = _mapper.CreateMapper().Map<Area>(model);
            await CreateAsync(area);
            return area;
        }
    }
}
