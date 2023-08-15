using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.Doctor_Working_Hour;
using GraduationProject.DataBase.ViewModels.Patient;
using GraduationProject.DataBase.ViewModels.Secretary;
using GraduationProject.DataBase.ViewModels.Secretary_Working_Hour;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Transactions;

namespace GraduationProject.Service.Services
{
    public class SecretaryService : ISecretaryService
    {
        private GraduationProjectDbContext _DbContext;
        private readonly IMapper _Mapper;
        public SecretaryService(GraduationProjectDbContext DbContext, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Mapper = Mapper;
        }
        public ApiResponse GetSecretaryById(int UserId)
        {
            Secretary? SecretaryEntity = _DbContext.Secretaries
                .Include(x => x.User).Include(x => x.Clinic)
                .FirstOrDefault(x => x.UserId == UserId);

            if (SecretaryEntity == null)
                return new ApiResponse(false, $"No Secretary Found With This User Id: ({UserId})");

            AllSecretaryDataViewModel SecretaryViewModel = _Mapper.Map<AllSecretaryDataViewModel>(SecretaryEntity);

            List<Secretary_Working_Hour> Secretary_Working_HoursEntities = _DbContext.Secretary_Working_Hours
                .Include(x => x.WorkingDays).Include(x => x.Secretary)
                .Where(x => x.SecretaryId == SecretaryEntity.UserId).ToList();

            SecretaryViewModel.Secretary_Working_Hours = _Mapper.Map<List<Secretary_Working_HourViewModel>>(Secretary_Working_HoursEntities);

            return new ApiResponse(SecretaryViewModel, "Succeed");
        }
        public ApiResponse DeleteSecreatry(int SecretaryId)
        {
            Secretary? SecretaryEntity = _DbContext.Secretaries
                .Include(x => x.User).Include(x => x.Clinic)
                .FirstOrDefault(x => x.Id == SecretaryId);

            if (SecretaryEntity == null)
                return new ApiResponse(false, $"No Secretary Found With This Id: ({SecretaryId})");

            SecretaryEntity.IsDeleted = true;
            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse AddNewSecretary(AddSecreataryViewModel NewSecretary)
        {
            using (TransactionScope Transaction = new TransactionScope())
            {
                User UserInfo = _Mapper.Map<User>(NewSecretary.UserInfo);

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

                Secretary SecretaryInfo = _Mapper.Map<Secretary>(NewSecretary);
                SecretaryInfo.UserId = UserInfo.Id;

                _DbContext.Secretaries.Add(SecretaryInfo);
                _DbContext.SaveChanges();

                List<Secretary_Working_Hour> Secretary_Working_Hours = _Mapper.Map<List<Secretary_Working_Hour>>(NewSecretary.Secretary_Working_Hours);

                foreach (Secretary_Working_Hour Secretary_Working_Hour in Secretary_Working_Hours)
                {
                    Secretary_Working_Hour.SecretaryId = SecretaryInfo.UserId;
                }

                _DbContext.Secretary_Working_Hours.AddRange(Secretary_Working_Hours);
                _DbContext.SaveChanges();

                Transaction.Complete();

                return new ApiResponse(true, "Succeed");
            }
        }
        public ApiResponse EditClinicSecretary(int SecretaryId, int NewClinicId)
        {
            Secretary? SecretaryEntity = _DbContext.Secretaries
                .Include(x => x.User).Include(x => x.Clinic)
                .FirstOrDefault(x => x.Id == SecretaryId);

            if (SecretaryEntity == null)
                return new ApiResponse(false, $"No Secretary Found With This Id: ({SecretaryId})");

            SecretaryEntity.ClinicId = NewClinicId;
            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
        public ApiResponse GetAllSecretaries(int ClinicId, ComplexFilter Filter)
        {
            List<SecretaryViewModel> Secretaries = _Mapper.Map<List<SecretaryViewModel>>(_DbContext.Secretaries
                .Include(x => x.User).Include(x => x.Clinic)
                .Where(x => x.ClinicId == ClinicId).ToList());

            int Count = Secretaries.Count();

            if (!string.IsNullOrEmpty(Filter.Sort))
            {
                PropertyInfo? SortProperty = typeof(SecretaryViewModel).GetProperty(Filter.Sort);

                if (SortProperty != null && Filter.Order == "asc")
                    Secretaries = Secretaries.OrderBy(x => SortProperty.GetValue(x)).ToList();

                else if (SortProperty != null && Filter.Order == "desc")
                    Secretaries = Secretaries.OrderByDescending(x => SortProperty.GetValue(x)).ToList();

                Secretaries = Secretaries.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Secretaries, "Succeed", Count);
            }
            else
            {
                Secretaries = Secretaries.Skip((Filter.PageIndex - 1) * Filter.PageSize)
                    .Take(Filter.PageSize).ToList();

                return new ApiResponse(Secretaries, "Succeed", Count);
            }
        }
        public ApiResponse GetAllDoctorsByClinicId(int ClinicId)
        {
            List<DoctorViewModel> Doctors = _Mapper.Map<List<DoctorViewModel>>(_DbContext.Doctors
                .Include(x => x.User).Include(x => x.Clinic)
                .Where(x => x.ClinicId == ClinicId).ToList());

            return new ApiResponse(Doctors, "Succeed", Doctors.Count());
        }
        public ApiResponse EditSecretary(EditSecretaryViewModel SecretaryNewData)
        {
            Secretary? OldSecretaryData = _DbContext.Secretaries.FirstOrDefault(x => x.Id == SecretaryNewData.Id);

            if (OldSecretaryData == null)
                return new ApiResponse(false, $"No Secretary Found With This Id: {SecretaryNewData.Id}");

            User? SecretaryUser = _DbContext.Users
                .FirstOrDefault(x => x.Id == OldSecretaryData.UserId);

            if (SecretaryUser == null)
                return new ApiResponse(false, $"No User Found With This Id: {OldSecretaryData.UserId}");

            bool CheckUserNameIfAlreadyExit = _DbContext.Users
                .Any(x => x.Name.ToLower() == SecretaryNewData.User_Name.ToLower() && x.Id != SecretaryNewData.Id);

            if (CheckUserNameIfAlreadyExit)
                return new ApiResponse(false, $"This User Name: {SecretaryNewData.User_Name} is Already Used");

            bool CheckEmailIfAlreadyExit = _DbContext.Users
                .Any(x => x.Email.ToLower() == SecretaryNewData.Email.ToLower() && x.Id != SecretaryNewData.Id);

            if (CheckEmailIfAlreadyExit)
                return new ApiResponse(false, $"This Email: {SecretaryNewData.Email} is Already Used");

            SecretaryUser.Telephone_Number = SecretaryNewData.Telephone_Number;
            SecretaryUser.Phone_Number = SecretaryNewData.Phone_Number;
            SecretaryUser.Last_Name = SecretaryNewData.Last_Name;
            SecretaryUser.First_Name = SecretaryNewData.First_Name;
            SecretaryUser.Email = SecretaryNewData.Email;
            SecretaryUser.Name = SecretaryNewData.User_Name;

            List<Secretary_Working_Hour> SecretaryWorkingHourEntities = _DbContext.Secretary_Working_Hours
                .Where(x => x.SecretaryId == SecretaryUser.Id).ToList();

            _DbContext.SaveChanges();

            return new ApiResponse(true, "Succeed");
        }
    }
}
