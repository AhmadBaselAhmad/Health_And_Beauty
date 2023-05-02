using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.Service.Interfaces;

namespace GraduationProject.Service.Services
{
    public class AdminService: IAdminService
    {
        private GraduationProjectDbContext _DbContext;
        public AdminService(GraduationProjectDbContext DbContext)
        {
            _DbContext = DbContext;
        }
        public ApiResponse ChangeCurrentAdmin(int AdminId)
        {
            Admin? CurrentAdmin = _DbContext.Admins.FirstOrDefault(x => !x.IsDeleted);

            if (CurrentAdmin == null)
                return new ApiResponse(false, $"No Admin Found With This Id: {AdminId}");

            CurrentAdmin.IsDeleted = true;

            Admin NewAdmin = new Admin
            {
                DoctorId = AdminId
            };
            _DbContext.Admins.Add(NewAdmin);
            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
    }
}
