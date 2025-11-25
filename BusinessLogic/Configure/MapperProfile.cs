using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogic.Dtos;
using DataAccess.Data.Entities;

namespace BusinessLogic.Configure
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<EquipmentDTO, Equipment>();
            CreateMap<Equipment, EquipmentDTO>()
                .ForMember(dest => dest.OwnerName, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerAvatarUrl, opt => opt.Ignore());
        }
    }
}
