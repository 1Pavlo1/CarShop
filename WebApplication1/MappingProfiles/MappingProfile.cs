using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CarShop.Controllers.Resources;
using CarShop.Models;

namespace CarShop.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Model, ModelResources>();
        }
    }
}
