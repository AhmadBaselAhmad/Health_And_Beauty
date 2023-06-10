using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.Patient;
using GraduationProject.DataBase.ViewModels.Secretary;
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
        public ApiResponse GetSecretaryById(int SecretaryId)
        {
            Secretary? SecretaryEntity = _DbContext.Secretaries
                .Include(x => x.User).Include(x => x.Clinic)
                .FirstOrDefault(x => x.Id == SecretaryId);

            if (SecretaryEntity == null)
                return new ApiResponse(false, $"No Secretary Found With This Id: ({SecretaryId})");

            AllSecretaryDataViewModel SecretaryViewModel = _Mapper.Map<AllSecretaryDataViewModel>(SecretaryEntity);

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
    }
}
