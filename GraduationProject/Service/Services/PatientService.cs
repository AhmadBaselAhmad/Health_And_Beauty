using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.Medical_Information;
using GraduationProject.DataBase.ViewModels.Patient;
using GraduationProject.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GraduationProject.Service.Services
{
    public class PatientService : IPatientService
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public PatientService(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse AddPatientMedicalInfo(AddMedical_InformationsViewModel PatientMedicalInformationViewModel)
        {
            Medical_Information? CheckNameDuplicate = _DbContext.Medical_Informations
                .FirstOrDefault(x => x.PatientId == PatientMedicalInformationViewModel.PatientId);

            if (CheckNameDuplicate != null)
                return new ApiResponse(false, $"You Can't Add Another Medical Information For This Patient Id: " +
                    $"({PatientMedicalInformationViewModel.PatientId})");

            Medical_Information PatientMedical_InformationEntity = _Mapper.Map<Medical_Information>(PatientMedicalInformationViewModel);

            _DbContext.Medical_Informations.Add(PatientMedical_InformationEntity);
            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse EditPatientMedicalInfo(EditMedical_InformationsViewModel PatientMedicalInformationViewModel)
        {
            Medical_Information? PatientMedical_InformationEntity = _DbContext.Medical_Informations
                .FirstOrDefault(x => x.PatientId == PatientMedicalInformationViewModel.PatientId);

            if (PatientMedical_InformationEntity == null)
                return new ApiResponse(false, $"No Medical Information " +
                    $"Found For This Patient Id: ({PatientMedicalInformationViewModel.PatientId})");

            PatientMedical_InformationEntity.Height = PatientMedicalInformationViewModel.Height;
            PatientMedical_InformationEntity.BGroup = PatientMedicalInformationViewModel.BGroup;
            PatientMedical_InformationEntity.Pulse = PatientMedicalInformationViewModel.Pulse;
            PatientMedical_InformationEntity.Weight = PatientMedicalInformationViewModel.Weight;
            PatientMedical_InformationEntity.BPressure = PatientMedicalInformationViewModel.BPressure;
            PatientMedical_InformationEntity.Respiration = PatientMedicalInformationViewModel.Respiration;
            PatientMedical_InformationEntity.Diet = PatientMedicalInformationViewModel.Diet;

            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse GetMedicalInformationByPatientId(int PatientId)
        {
            Medical_Information? Medical_InformationEntity = _DbContext.Medical_Informations
                .Include(x => x.Patient).FirstOrDefault(x => x.PatientId == PatientId);

            if (Medical_InformationEntity == null)
                return new ApiResponse(false, $"No Patient Found With This Id: ({PatientId})");

            Medical_InformationViewModel PatientMedicalInformation = _Mapper.Map<Medical_InformationViewModel>(Medical_InformationEntity);

            return new ApiResponse(PatientMedicalInformation, "Succeed");
        }
        public ApiResponse GetPatientById(int PatientId)
        {
            Patient? PatientEntity = _DbContext.Patients.Include(x => x.User)
                .FirstOrDefault(x => x.UserId == PatientId);

            if (PatientEntity == null)
                return new ApiResponse(false, $"No Patient Found With This Id: ({PatientId})");

            PatientViewModel PatientViewModel = _Mapper.Map<PatientViewModel>(PatientEntity);

            PatientViewModel.VisitCount = _DbContext.Appointments
                .Where(x => x.PatientId == PatientViewModel.Id && x.Status == ((int)Constants.AppointmentStatus.complete).ToString()).Count();

            return new ApiResponse(PatientViewModel, "Succeed");
        }
        public ApiResponse GetAllDoctorsPatients(int UserId, ComplexFilter Filter)
        {
            Doctor? DoctorEntity = _DbContext.Doctors.FirstOrDefault(x => x.UserId == UserId);

            if (DoctorEntity == null)
                return new ApiResponse(false, $"No Doctor Found With This Id: ({UserId})");

            List<PatientViewModel> Patients = _Mapper.Map<List<PatientViewModel>>(_DbContext.Appointments
                .Include(x => x.Patient).Include(x => x.Patient.User)
                .Where(x => x.DoctorId == DoctorEntity.Id)
                .Select(x => x.Patient).ToList());

            foreach (PatientViewModel Patient in Patients)
            {
                Patient.VisitCount = _DbContext.Appointments
                    .Where(x => x.PatientId == Patient.Id && x.Status == ((int)Constants.AppointmentStatus.complete).ToString()).Count();
            }

            int Count = Patients.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(PatientViewModel).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    Patients = Patients.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    Patients = Patients.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                Patients = Patients.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Patients, "Succeed", Count);
            }
            else
            {
                Patients = Patients.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Patients, "Succeed", Count);
            }
        }
    }
}
