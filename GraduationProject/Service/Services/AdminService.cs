using GraduationProject.DataBase.Context;
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
        public void ChangeCurrentAdmin(int DoctorId)
        {
            Admin CurrentAdmin = _DbContext.Admins.FirstOrDefault(x => !x.IsDeleted);

            CurrentAdmin.IsDeleted = true;

            Admin NewAdmin = new Admin
            {
                DoctorId = DoctorId
            };
            _DbContext.Admins.Add(NewAdmin);
            _DbContext.SaveChanges();
        }
        //public List<DoctorViewModel> GetAllDoctorsWithoutCurrentAdmin()
        //{
        //    var NotAdminDoctors = _DbContext.Doctors.Where(x => !x.IsDeleted)
        //}
    }
}
