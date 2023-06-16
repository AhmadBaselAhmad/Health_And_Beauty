using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Appointment;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.Secretary;
using GraduationProject.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

            List<AppointmentViewModel> AppointmentsViewModel = _Mapper.Map<List<AppointmentViewModel>>(_DbContext.Appointments
                .Include(x => x.Service).Include(x => x.Patient).ThenInclude(x => x.User)
                .Where(x => x.DoctorId == DoctorEntity.Id && x.Status.ToLower() == AppointmentStatus.ToLower()).ToList());

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

            return new ApiResponse(AppointmentsViewModel, "Succeed", Count);
        }
        public ApiResponse GetAllAppointmentsForSecretaryRole(int DoctorId, string AppointmentStatus, ComplexFilter Filter)
        {
            Doctor? DoctorEntity = _DbContext.Doctors
                .FirstOrDefault(x => x.Id == DoctorId);

            if (DoctorEntity == null)
                return new ApiResponse(false, $"No Doctor Found With This User Id: ({DoctorId})");

            List<AppointmentViewModel> AppointmentsViewModel = _Mapper.Map<List<AppointmentViewModel>>(_DbContext.Appointments
                .Include(x => x.Service).Include(x => x.Patient).ThenInclude(x => x.User)
                .Where(x => x.DoctorId == DoctorEntity.Id && x.Status.ToLower() == AppointmentStatus.ToLower()).ToList());

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

            return new ApiResponse(AppointmentsViewModel, "Succeed", Count);
        }
    }
}
