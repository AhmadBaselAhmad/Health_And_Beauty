using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Appointment;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.Medicine;
using GraduationProject.DataBase.ViewModels.Prescription;
using GraduationProject.DataBase.ViewModels.Secretary;
using GraduationProject.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Transactions;

namespace GraduationProject.Service.Services
{
    public class AppointmentService : IAppointmentService
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public AppointmentService(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse GetAllAppointmentsForDoctorRole(int UserId, string AppointmentStatus, ComplexFilter Filter)
        {
            Doctor? DoctorEntity = _DbContext.Doctors
                .FirstOrDefault(x => x.UserId == UserId);

            if (DoctorEntity == null)
                return new ApiResponse(false, $"No Doctor Found With This User Id: ({UserId})");

            List<AppointmentViewModel> AppointmentsViewModel = new List<AppointmentViewModel>();

            if (!string.IsNullOrEmpty(Filter.SearchQuery))
            {
                AppointmentsViewModel = _Mapper.Map<List<AppointmentViewModel>>(_DbContext.Appointments
                    .Include(x => x.Service)
                    .Include(x => x.Patient)
                    .Include(x => x.Patient.User)
                    .Where(x => x.DoctorId == DoctorEntity.Id && x.Status.ToLower() == AppointmentStatus.ToLower() &&
                        x.Patient.User.Name.ToLower().StartsWith(Filter.SearchQuery)).ToList());
            }
            else
            {
                AppointmentsViewModel = _Mapper.Map<List<AppointmentViewModel>>(_DbContext.Appointments
                    .Include(x => x.Service)
                    .Include(x => x.Patient)
                    .Include(x => x.Patient.User)
                    .Where(x => x.DoctorId == DoctorEntity.Id && x.Status.ToLower() == AppointmentStatus.ToLower()).ToList());
            }

            int Count = AppointmentsViewModel.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(AppointmentViewModel).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    AppointmentsViewModel = AppointmentsViewModel.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    AppointmentsViewModel = AppointmentsViewModel.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                AppointmentsViewModel = AppointmentsViewModel.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();
            }
            else
                AppointmentsViewModel = AppointmentsViewModel.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

            foreach (AppointmentViewModel AppointmentViewModel in AppointmentsViewModel)
            {
                Medical_Information? PatientMedicalInfo = _DbContext.Medical_Informations
                    .FirstOrDefault(x => x.Patient.Name.ToLower() == AppointmentViewModel.Patient_Name.ToLower());

                if (PatientMedicalInfo != null)
                    AppointmentViewModel.MedicalInfoId = PatientMedicalInfo.Id;

                else
                    AppointmentViewModel.MedicalInfoId = null;
            }

            return new ApiResponse(AppointmentsViewModel, "Succeed", Count);
        }
        public ApiResponse GetAllAppointmentsForSecretaryRole(int DoctorId, string AppointmentStatus, ComplexFilter Filter)
        {
            Doctor? DoctorEntity = _DbContext.Doctors
                .FirstOrDefault(x => x.Id == DoctorId);

            if (DoctorEntity == null)
                return new ApiResponse(false, $"No Doctor Found With This User Id: ({DoctorId})");

            List<AppointmentViewModel> AppointmentsViewModel = new List<AppointmentViewModel>();

            if (!string.IsNullOrEmpty(Filter.SearchQuery))
            {
                AppointmentsViewModel = _Mapper.Map<List<AppointmentViewModel>>(_DbContext.Appointments
                    .Include(x => x.Service).Include(x => x.Patient).ThenInclude(x => x.User)
                    .Where(x => x.DoctorId == DoctorEntity.Id && x.Status.ToLower() == AppointmentStatus.ToLower() &&
                        x.Patient.User.Name.ToLower().StartsWith(Filter.SearchQuery.ToLower())).ToList());
            }
            else
            {
                AppointmentsViewModel = _Mapper.Map<List<AppointmentViewModel>>(_DbContext.Appointments
                    .Include(x => x.Service).Include(x => x.Patient).ThenInclude(x => x.User)
                    .Where(x => x.DoctorId == DoctorEntity.Id && x.Status.ToLower() == AppointmentStatus.ToLower()).ToList());
            }

            int Count = AppointmentsViewModel.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(AppointmentViewModel).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    AppointmentsViewModel = AppointmentsViewModel.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    AppointmentsViewModel = AppointmentsViewModel.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                AppointmentsViewModel = AppointmentsViewModel.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();
            }
            else
                AppointmentsViewModel = AppointmentsViewModel.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

            foreach (AppointmentViewModel AppointmentViewModel in AppointmentsViewModel)
            {
                Medical_Information? PatientMedicalInfo = _DbContext.Medical_Informations
                    .FirstOrDefault(x => x.Patient.Name.ToLower() == AppointmentViewModel.Patient_Name.ToLower());

                if (PatientMedicalInfo != null)
                    AppointmentViewModel.MedicalInfoId = PatientMedicalInfo.Id;

                else
                    AppointmentViewModel.MedicalInfoId = null;
            }

            return new ApiResponse(AppointmentsViewModel, "Succeed", Count);
        }
        public ApiResponse ChangeAppointmentStatus(int AppointmentId, string NewAppointmentStatus)
        {
            Appointment? AppointmentEntity = _DbContext.Appointments
                .FirstOrDefault(x => x.Id == AppointmentId);

            if (AppointmentEntity == null)
                return new ApiResponse(false, $"No Appointment Found With This Id: ({AppointmentId})");

            AppointmentEntity.Status = NewAppointmentStatus;
            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse AddPrescription(AddPrescriptionViewModel NewPrescription)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                Prescription NewPrescriptionEntity = _Mapper.Map<Prescription>(NewPrescription);

                _DbContext.Prescriptions.Add(NewPrescriptionEntity);
                _DbContext.SaveChanges();

                List<Medicine> MedicinesEntities = _Mapper.Map<List<Medicine>>(NewPrescription.Medicines);

                foreach (Medicine Medicine in MedicinesEntities)
                {
                    Medicine.PrescriptionId = NewPrescriptionEntity.Id;
                    Medicine.MedicalInfoId = NewPrescriptionEntity.MedicalInfoId;
                    Medicine.IsDeleted = false;
                }

                _DbContext.Medicines.AddRange(MedicinesEntities);
                _DbContext.SaveChanges();

                transaction.Complete();

                return new ApiResponse(true, "Succeed");
            }
        }
        public ApiResponse EditPrescription(EditPrescriptionViewModel NewPrescriptionData)
        {
            Prescription NewPrescriptionEntity = _Mapper.Map<Prescription>(NewPrescriptionData);

            _DbContext.Prescriptions.Add(NewPrescriptionEntity);
            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
    }
}
