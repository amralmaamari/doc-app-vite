namespace DoctorAPI.Model.Admin
{
    public class AddDoctorDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile? Image { get; set; } // Make nullable
         public string Speciality { get; set; }
        public string Degree { get; set; }
        public string Experience { get; set; }
        public string About { get; set; }
        public decimal Fees { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
    }



}
