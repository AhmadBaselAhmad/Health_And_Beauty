using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Clinic;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Transactions;

namespace GraduationProject.Service.Services
{
    public class DoctorService : IDoctorService
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public DoctorService(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse AddNewDoctor(AddDoctorViewModel NewDoctor)
        {
            using (TransactionScope Transaction = new TransactionScope())
            {
                User UserInfo = _Mapper.Map<User>(NewDoctor.UserInfo);

                bool CheckUserNameConstraint = _DbContext.Users
                    .Any(x => x.Name.ToLower() == UserInfo.Name.ToLower());

                if (CheckUserNameConstraint)
                    return new ApiResponse(false, $"This User Name {UserInfo.Name} is Used");

                bool CheckPhoneNumberConstraint = _DbContext.Users
                    .Any(x => x.Phone_Number == UserInfo.Phone_Number);

                if (CheckUserNameConstraint)
                    return new ApiResponse(false, $"This Phone Number {UserInfo.Phone_Number} is Used");

                bool CheckEmailConstraint = _DbContext.Users
                    .Any(x => x.Email == UserInfo.Email);

                if (CheckEmailConstraint)
                    return new ApiResponse(false, $"This Phone Number {UserInfo.Email} is Used");

                byte[] Salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

                // Derive a 256-bit Subkey (Use HMACSHA256 With 100,000 Iterations)
                string Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: UserInfo.Password,
                    salt: Salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                UserInfo.Password = Password;
                UserInfo.Blocked_Date = null;

                _DbContext.Users.Add(UserInfo);
                _DbContext.SaveChanges();

                Doctor DoctorInfo = _Mapper.Map<Doctor>(NewDoctor);
                DoctorInfo.UserId = UserInfo.Id;

                _DbContext.Doctors.Add(DoctorInfo);
                _DbContext.SaveChanges();

                Transaction.Complete();

                return new ApiResponse(true, "Succeed");
            }
        }
        public ApiResponse GetAllDoctors(ComplexFilter Filter, int? ClinicId)
        {
            List<DoctorViewModel> Doctors = _Mapper.Map<List<DoctorViewModel>>(_DbContext.Doctors
                .Include(x => x.User).Include(x => x.Clinic)
                .Where(x => (!string.IsNullOrEmpty(Filter.SearchQuery) ?
                    x.User.Name.ToLower().StartsWith(Filter.SearchQuery) : true) &&
                    (ClinicId != null ? x.ClinicId == ClinicId.Value : true)).ToList());

            int Count = Doctors.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(DoctorViewModel).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    Doctors = Doctors.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    Doctors = Doctors.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                Doctors = Doctors.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Doctors, "Succeed", Count);
            }
            else
            {
                Doctors = Doctors.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Doctors, "Succeed", Count);
            }
        }
        public ApiResponse EditDoctor(EditDoctorViewModel DoctorNewData)
        {
            Doctor? OldDoctorData = _DbContext.Doctors.FirstOrDefault(x => x.Id == DoctorNewData.Id);

            if (OldDoctorData == null)
                return new ApiResponse(false, $"No Doctor Found With This Id: {DoctorNewData.Id}");

            OldDoctorData.AboutMe = DoctorNewData.AboutMe;
            OldDoctorData.Specialization = DoctorNewData.Specialization;
            OldDoctorData.Degree = DoctorNewData.Degree;

            _DbContext.Doctors.Update(OldDoctorData);
            _DbContext.SaveChanges();
            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse GetDoctorById(int DoctorId)
        {
            Doctor? Doctor = _DbContext.Doctors.Include(x => x.User).Include(x => x.Clinic)
                .FirstOrDefault(x => x.Id == DoctorId);

            if (Doctor == null)
                return new ApiResponse(false, $"No Doctor Found With This Id: {DoctorId}");

            DoctorViewModel DoctorViewModel = _Mapper.Map<DoctorViewModel>(Doctor);
            return new ApiResponse(DoctorViewModel, "Succeed");
        }
        public ApiResponse ChangeHeaderOfClinic(int NewHeadOfSectionId, int ClinicId)
        {
            Doctor? OldHeadOfClinic = _DbContext.Doctors
                .FirstOrDefault(x => x.ClinicId == ClinicId && x.IsHeadOfClinic);

            Doctor? NewHeadOfClinic = _DbContext.Doctors.FirstOrDefault(x => x.Id == NewHeadOfSectionId);

            if (OldHeadOfClinic != null && NewHeadOfClinic != null)
            {
                OldHeadOfClinic.IsHeadOfClinic = false;
                NewHeadOfClinic.IsHeadOfClinic = true;
            }

            _DbContext.SaveChanges();
            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse DeleteDoctor(int DoctorId)
        {
            Doctor? Doctor = _DbContext.Doctors.FirstOrDefault(x => x.Id == DoctorId);

            if (Doctor == null)
                return new ApiResponse(false, $"No Doctor Found With This Id: {DoctorId}");

            Doctor.IsDeleted = true;
            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
    }
}
