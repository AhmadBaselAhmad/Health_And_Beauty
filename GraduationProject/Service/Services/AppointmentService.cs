using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Secretary;
using GraduationProject.Service.Interfaces;

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
        public ApiResponse GetAllAppointments(int UserId, string Role, ComplexFilter Filter)
        {
            if (Role.ToLower() == Constants.Roles.Secretary.ToString().ToLower())
            {
                Secretary? SecretaryEntity = _DbContext.Secretaries
                    .FirstOrDefault(x => x.UserId == UserId);

                if (SecretaryEntity == null)
                    return new ApiResponse(false, $"No Secretary Found With This User Id: ({UserId})");

                //var Appointments = _DbContext.Appointments.Where(x => x.)
            }
            return new ApiResponse(false, $"No Secretary Found With This User Id: ({UserId})");

        }
    }
}
