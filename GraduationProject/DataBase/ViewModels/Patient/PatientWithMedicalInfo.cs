namespace GraduationProject.DataBase.ViewModels.Patient
{
    public class PatientWithMedicalInfo
    {
        // Patient Information

        public int Id { get; set; }
        public DateTime Birthdate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public int VisitCount { get; set; }

        // User Information

        public int UserId { get; set; }
        public string Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Telephone_Number { get; set; }
        public string Email { get; set; }

        // Medical Information

        public int Medical_InfoId { get; set; }
        public string Height { get; set; }
        public string BGroup { get; set; }
        public string Pulse { get; set; }
        public string Weight { get; set; }
        public string BPressure { get; set; }
        public string Respiration { get; set; }
        public string Diet { get; set; }

        // Medicine

        public string MedicineName { get; set; }
        public string MedicineDescription { get; set; }
    }
}
