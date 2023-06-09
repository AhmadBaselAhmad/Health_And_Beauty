﻿using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Authenticate;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GraduationProject.Service.Services
{
    public class LoginService : ILoginService
    {
        private GraduationProjectDbContext _DbContext;
        private IConfiguration _Config;
        public LoginService(GraduationProjectDbContext DbContext, IConfiguration Config)
        {
            _DbContext = DbContext;
            _Config = Config;
        }
        public ApiResponse CreateToken(LoginViewModel Login)
        {
            User? User = _DbContext.Users.FirstOrDefault(x => x.Name.ToLower() == Login.UserName.ToLower());

            if (User == null)
                return new ApiResponse(false, "Invalid User Name");

            bool CheckIfItsPatient = _DbContext.Patients.Any(x => x.UserId == User.Id);

            if (CheckIfItsPatient)
                return new ApiResponse(false, "Invalid User Name");

            byte[] Salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

            // Derive a 256-bit Subkey (Use HMACSHA256 With 100,000 Iterations)
            string Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Login.Password,
                salt: Salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            bool Verified = (User.Password == Password);

            if (!Verified)
                return new ApiResponse(false, "Invalid Password");

            string Role = string.Empty;
            int ClinicId = 0;
            Secretary? CheckSecretaryRole = _DbContext.Secretaries.FirstOrDefault(x => x.UserId == User.Id);

            if (CheckSecretaryRole != null)
            {
                Role = Constants.Roles.Secretary.ToString();
                ClinicId = CheckSecretaryRole.ClinicId;
            }
            else
            {
                Admin? SuperAdmin = _DbContext.Admins
                    .Include(x => x.Doctor)
                    .FirstOrDefault(x => x.Doctor.UserId == User.Id);

                if (SuperAdmin != null)
                {
                    Role = Constants.Roles.SuperAdmin.ToString();
                    ClinicId = SuperAdmin.Doctor.ClinicId;
                }
                if (SuperAdmin == null)
                {
                    Doctor? Doctor = _DbContext.Doctors
                        .FirstOrDefault(x => x.UserId == User.Id);

                    if (Doctor != null ? Doctor.IsHeadOfClinic : false)
                        Role = Constants.Roles.ClinicAdmin.ToString();

                    else
                        Role = Constants.Roles.Doctor.ToString();

                    ClinicId = Doctor.ClinicId;
                }
            }

            List<Claim> Claims = new List<Claim> {
                new Claim("UserId", User.Id.ToString()),
                new Claim("Last_Name", User.Last_Name),
                new Claim("RandomGuid", Guid.NewGuid().ToString()),
                new Claim("Role", Role),
                new Claim("ClinicId", ClinicId.ToString())
            };

            SymmetricSecurityKey Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Config["JWT:Key"]));
            SigningCredentials Credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken TokenDetails = new JwtSecurityToken("http://localhost:53174/",
              "http://localhost:53174/",
              Claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: Credentials);

            string TokenString = new JwtSecurityTokenHandler().WriteToken(TokenDetails);

            return new ApiResponse(TokenString, "Succeed");
        }
    }
}
