using AutoMapper;
using Core.Models;
using Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            //AddProperty
            CreateMap<PropertyAddDto, Properties>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            //Amenities
            CreateMap<Amenities, AmenitiesResource>().ReverseMap();
            //Properties
            CreateMap<Properties, PropertiesResource>().ReverseMap();
            //Images
            CreateMap<Images, ImagesResource>().ReverseMap();
            //Owners
            CreateMap<Owners, OwnersResource>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => string.Format("{0} {1}", src.FirstName, src.LastName))).ReverseMap();
            //Review
            CreateMap<Review, ReviewResource>().ReverseMap();
            //LoginRequest
            CreateMap<LoginRequest, LoginRequestResource>().ReverseMap();
        }

    }
}
