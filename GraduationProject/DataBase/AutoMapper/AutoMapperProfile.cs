using AutoMapper;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Allergies;
using GraduationProject.DataBase.ViewModels.Appointment;
using GraduationProject.DataBase.ViewModels.Clinic;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.Doctor_Working_Hour;
using GraduationProject.DataBase.ViewModels.DynamicAttribute;
using GraduationProject.DataBase.ViewModels.Immunization;
using GraduationProject.DataBase.ViewModels.Medical_Information;
using GraduationProject.DataBase.ViewModels.Medicine;
using GraduationProject.DataBase.ViewModels.Patient;
using GraduationProject.DataBase.ViewModels.Secretary;
using GraduationProject.DataBase.ViewModels.Service;
using GraduationProject.DataBase.ViewModels.Surgery;
using GraduationProject.DataBase.ViewModels.User;
using GraduationProject.DataBase.ViewModels.Working_Days;

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
                .ForMember(c => c.Name, c => c.MapFrom(s => s.User.Name))
                .ForMember(c => c.First_Name, c => c.MapFrom(s => s.User.First_Name))
                .ForMember(c => c.Last_Name, c => c.MapFrom(s => s.User.Last_Name))
                .ForMember(c => c.Phone_Number, c => c.MapFrom(s => s.User.Phone_Number))
                .ForMember(c => c.Telephone_Number, c => c.MapFrom(s => s.User.Telephone_Number))
                .ForMember(c => c.Email, c => c.MapFrom(s => s.User.Email));
            CreateMap<Doctor, GetAllDoctorViewModel>()
                .ForMember(c => c.Clinic_Name, c => c.MapFrom(s => s.Clinic.Name))
                .ForMember(c => c.Name, c => c.MapFrom(s => s.User.Name))
                .ForMember(c => c.First_Name, c => c.MapFrom(s => s.User.First_Name))
                .ForMember(c => c.Last_Name, c => c.MapFrom(s => s.User.Last_Name))
                .ForMember(c => c.Phone_Number, c => c.MapFrom(s => s.User.Phone_Number))
                .ForMember(c => c.Telephone_Number, c => c.MapFrom(s => s.User.Telephone_Number))
                .ForMember(c => c.Email, c => c.MapFrom(s => s.User.Email));
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
                .ForMember(c => c.UserInformation, c => c.MapFrom(s => s.User));

            // Secretary..
            CreateMap<Secretary, SecretaryViewModel>()
                .ForMember(c => c.Clinic_Name, c => c.MapFrom(s => s.Clinic.Name))
                .ForMember(c => c.User_Name, c => c.MapFrom(s => s.User.Name))
                .ForMember(c => c.First_Name, c => c.MapFrom(s => s.User.First_Name))
                .ForMember(c => c.Last_Name, c => c.MapFrom(s => s.User.Last_Name))
                .ForMember(c => c.Phone_Number, c => c.MapFrom(s => s.User.Phone_Number));
            CreateMap<Secretary, AllSecretaryDataViewModel>()
                .ForMember(c => c.Clinic_Name, c => c.MapFrom(s => s.Clinic.Name))
                .ForMember(c => c.User_Name, c => c.MapFrom(s => s.User.Name))
                .ForMember(c => c.First_Name, c => c.MapFrom(s => s.User.First_Name))
                .ForMember(c => c.Last_Name, c => c.MapFrom(s => s.User.Last_Name))
                .ForMember(c => c.Phone_Number, c => c.MapFrom(s => s.User.Phone_Number))
                .ForMember(c => c.Telephone_Number, c => c.MapFrom(s => s.User.Telephone_Number))
                .ForMember(c => c.Email, c => c.MapFrom(s => s.User.Email));
            CreateMap<Secretary, AddSecreataryViewModel>().ReverseMap();

            // Clinic..
            CreateMap<Clinic, ClinicViewModel>().ReverseMap();

            // Working_Days..
            CreateMap<Working_Day, Working_DaysViewModel>().ReverseMap();

            CreateMap<Models.Service, ServiceViewModel>()
                .ForMember(c => c.Clinic_Name, c => c.MapFrom(f => f.Clinic.Name));
            CreateMap<Models.Service, AddClinicServiceViewModel>().ReverseMap();

            // Doctor_Working_Hours..
            CreateMap<Doctor_Working_Hour, Doctor_Working_HourViewModel>()
                .ForMember(c => c.WorkingDays_Name, c => c.MapFrom(f => f.WorkingDays.Day));

            // Appointment..
            CreateMap<Appointment, AppointmentViewModel>()
                .ForMember(c => c.Service_Name, c => c.MapFrom(f => f.Service.Name))
                .ForMember(c => c.Patient_Name, c => c.MapFrom(f => f.Patient.User.Name));

            // Surgery..
            CreateMap<Surgery, AddSurgeryViewModel>().ReverseMap();

            // Allergy..
            CreateMap<Allergy, AddAllergyViewModel>().ReverseMap();

            // Immunization..
            CreateMap<Immunization, AddImmunizationViewModel>().ReverseMap();

            // Medicine..
            CreateMap<Medicine, AddMedicineViewModel>().ReverseMap();
        }
    }
}
