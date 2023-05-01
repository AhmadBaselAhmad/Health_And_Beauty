using AutoMapper;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.User;

namespace GraduationProject.DataBase.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, AddUserViewModel>().ReverseMap();
            CreateMap<Doctor, AddDoctorViewModel>().ReverseMap();

        }
    }
}
