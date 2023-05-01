using GraduationProject.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Xml;

namespace GraduationProject.DataBase.Context
{
    public class GraduationProjectDbContext: DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<Attribute_View_Management> Attributes_View_Management { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Data_Type> Data_Types { get; set; }
        public DbSet<Dependency> Dependencies { get; set; }
        public DbSet<Dependency_Row> Dependency_Rows { get; set; }
        public DbSet<Doctor_Working_Hour> Doctor_Working_Hours { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Dynamic_Attribute_Value> Dynamic_Attribute_Values { get; set; }
        public DbSet<Dynamic_Attribute> Dynamic_Attributes { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Immunization> Immunizations { get; set; }
        public DbSet<Medical_Information> Medical_Informations { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Password_Reset> Password_Resets { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Row_Rule> Row_Rules { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }
        public DbSet<Secretary_Working_Hour> Secretary_Working_Hours { get; set; }
        public DbSet<Models.Service> Services { get; set; }
        public DbSet<Static_Attribute> Static_Attributes { get; set; }
        public DbSet<Surgery> Surgeries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Validation> Validations { get; set; }
        public DbSet<Working_Day> Working_Days { get; set; }

        public GraduationProjectDbContext(DbContextOptions<GraduationProjectDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Appointment>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Attribute_View_Management>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Clinic>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Data_Type>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Dependency>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Dependency_Row>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Doctor>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Doctor_Working_Hour>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Dynamic_Attribute>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Dynamic_Attribute_Value>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<History>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Image>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Immunization>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Medical_Information>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Medicine>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Notification>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Operation>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Password_Reset>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Patient>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Prescription>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Row>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Row_Rule>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Rule>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Secretary>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Secretary_Working_Hour>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Models.Service>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Static_Attribute>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Surgery>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Validation>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Working_Day>().HasQueryFilter(s => !s.IsDeleted);
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is TimeStampModel && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((TimeStampModel)entityEntry.Entity).Updated_At = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((TimeStampModel)entityEntry.Entity).Created_At = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}
