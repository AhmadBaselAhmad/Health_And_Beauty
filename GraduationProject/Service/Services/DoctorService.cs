using AutoMapper;
using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Transactions;

namespace GraduationProject.Service.Services
{
    public class DoctorService : IDoctorService
    {
        private GraduationProjectDbContext _DbContext;
        private IConfiguration _Config;
        private readonly IMapper _Mapper;

        public DoctorService(GraduationProjectDbContext DbContext, IConfiguration Config, IMapper Mapper)
        {
            _DbContext = DbContext;
            _Config = Config;
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

                return new ApiResponse(true, string.Empty);
            }
        }
    }
}
