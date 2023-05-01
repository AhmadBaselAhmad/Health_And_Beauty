using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public GraduationProjectDbContext _DbContext;
        public TestController(GraduationProjectDbContext DbContext)
        {
            _DbContext = DbContext;
        }
        [HttpGet]
        public List<User> Get()
        {
            var xx = "";
            return (this._DbContext.Users.ToList());
        }
        public class AddUserViewModel
        {
            public string Name { get; set; }
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
            public string Phone_Number { get; set; }
            public string Telephone_Number { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public DateTime? Blocked_Date { get; set; }
        }
        public class EditUserViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
            public string Phone_Number { get; set; }
            public string Telephone_Number { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public DateTime? Blocked_Date { get; set; }
        }
        [HttpPost("AddNewUser")]
        public void AddNewUser(AddUserViewModel AddUserViewModel)
        {
            User User = new User
            {
                Name = AddUserViewModel.Name,
                Blocked_Date = null,
                Email = "srhan's",
                First_Name = AddUserViewModel.First_Name,
                IsDeleted = false,
                Last_Name = AddUserViewModel.Last_Name,
                Password = AddUserViewModel.Password,
                Phone_Number = AddUserViewModel.Phone_Number,
                Telephone_Number = AddUserViewModel.Telephone_Number
            };
            _DbContext.Users.Add(User);
            _DbContext.SaveChanges();
        }
        [HttpPost("UpdateUser")]
        public void UpdateUser(EditUserViewModel EditUser)
        {
            User? User = _DbContext.Users.FirstOrDefault(x => x.Id == EditUser.Id);
            if (User != null)
            {
                User.Id = EditUser.Id;
                User.Name = EditUser.Name;
                User.Blocked_Date = null;
                User.Email = "srhan's";
                User.First_Name = EditUser.First_Name;
                User.IsDeleted = false;
                User.Last_Name = EditUser.Last_Name;
                User.Password = EditUser.Password;
                User.Phone_Number = EditUser.Phone_Number;
                User.Telephone_Number = EditUser.Telephone_Number;
                _DbContext.SaveChanges();
            }
        }
        public class AddDoctorViewModel
        {
            public string Degree { get; set; }
            public string AboutMe { get; set; }
            public string Specialization { get; set; }
            public bool IsHeadOfClinic { get; set; }

            public int UserId { get; set; }
            public int ClinicId { get; set; }
        }
        [HttpPost("AddNewDoctor")]
        public void AddNewDoctor(AddDoctorViewModel AddDoctorViewModel)
        {
            Doctor NewDoctor = new Doctor
            {
                Degree = AddDoctorViewModel.Degree,
                AboutMe = AddDoctorViewModel.AboutMe,
                Specialization = AddDoctorViewModel.Specialization,
                IsHeadOfClinic = false,
                IsDeleted = false,
                UserId = 1,
                ClinicId = 1
            };

            _DbContext.Doctors.Add(NewDoctor);
            _DbContext.SaveChanges();
        }
        public class DoctorViewModel
        {
            public int Id { get; set; }
            public string Degree { get; set; }
            public string AboutMe { get; set; }
            public string Specialization { get; set; }
            public bool IsDeleted { get; set; }
            public bool IsHeadOfClinic { get; set; }

            public int UserId { get; set; }
            public int UserName { get; set; }
            public int ClinicId { get; set; }
            [ForeignKey("ClinicId")]
            public Clinic? Clinic { get; set; }
        }
        [HttpGet("GetDoctorById")]
        public DoctorViewModel AddNewDoctor(int DoctorId)
        {
            var DoctorEntity = _DbContext.Doctors.Include(x => x.User).FirstOrDefault(x => x.Id == DoctorId);

            return null;
        }
        [HttpPost("DeleteClinic")]
        public void DeleteClinic(int ClinicId)
        {
            Clinic? Clinic = _DbContext.Clinics.FirstOrDefault(x => x.Id == ClinicId);
            Clinic.IsDeleted = true;
            _DbContext.SaveChanges();
        }
    }
}
