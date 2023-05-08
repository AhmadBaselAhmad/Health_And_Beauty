using AutoMapper;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Clinic;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.DynamicAttribute;
using GraduationProject.DataBase.ViewModels.User;

namespace GraduationProject.DataBase.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, AddUserViewModel>().ReverseMap();

            CreateMap<Doctor, AddDoctorViewModel>().ReverseMap();
            CreateMap<Doctor, DoctorViewModel>()
                .ForMember(c => c.Clinic_Name, c => c.MapFrom(s => s.Clinic.Name))
                .ForMember(c => c.Doctor_Name, c => c.MapFrom(s => s.User.Name));
            CreateMap<Doctor, EditDoctorViewModel>().ReverseMap();

            CreateMap<AddDependencyInstViewModel, Dynamic_AttributeViewModel>().ReverseMap();
            CreateMap<Dynamic_Attribute, Dynamic_AttributeViewModel>()
                .ForMember(c => c.Clinic_Name, c => c.MapFrom(s => s.Clinic.Name))
                .ForMember(c => c.DataType_Name, c => c.MapFrom(s => s.DataType.Name));
        }
    }
}
