using AutoMapper;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Clinic;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.DynamicAttribute;
using GraduationProject.DataBase.ViewModels.Medical_Information;
using GraduationProject.DataBase.ViewModels.Patient;
using GraduationProject.DataBase.ViewModels.Secretary;
using GraduationProject.DataBase.ViewModels.User;

namespace GraduationProject.DataBase.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User..
            CreateMap<User, AddUserViewModel>().ReverseMap();
            CreateMap<User, UserViewModel>().ReverseMap();

            // Doctor..
            CreateMap<Doctor, AddDoctorViewModel>().ReverseMap();
            CreateMap<Doctor, DoctorViewModel>()
                .ForMember(c => c.Clinic_Name, c => c.MapFrom(s => s.Clinic.Name))
                .ForMember(c => c.Doctor_Name, c => c.MapFrom(s => s.User.Name));
            CreateMap<Doctor, EditDoctorViewModel>().ReverseMap();

            // Dynamic Attribute..
            CreateMap<AddDependencyInstViewModel, Dynamic_AttributeViewModel>().ReverseMap();
            CreateMap<Dynamic_Attribute, Dynamic_AttributeViewModel>()
                .ForMember(c => c.Clinic_Name, c => c.MapFrom(s => s.Clinic.Name))
                .ForMember(c => c.DataType_Name, c => c.MapFrom(s => s.DataType.Name));

            // Medical Information..
            CreateMap<Medical_Information, Medical_InformationViewModel>()
                .ForMember(c => c.Patient_Name, c => c.MapFrom(s => s.Patient.Name));
            CreateMap<Medical_Information, AddMedical_InformationsViewModel>().ReverseMap();
            CreateMap<Medical_Information, EditMedical_InformationsViewModel>().ReverseMap();
            
            // Patient..
            CreateMap<Patient, PatientViewModel>()
                .ForMember(c => c.UserInformation, c => c.MapFrom(s => s.User)).ReverseMap();

            // Secretary..
            CreateMap<Secretary, SecretaryViewModel>()
                .ForMember(c => c.User_Name, c => c.MapFrom(s => s.User.Name))
                .ForMember(c => c.Clinic_Name, c => c.MapFrom(s => s.Clinic.Name)).ReverseMap();
            CreateMap<Secretary, AddSecreataryViewModel>().ReverseMap();
        }
    }
}
