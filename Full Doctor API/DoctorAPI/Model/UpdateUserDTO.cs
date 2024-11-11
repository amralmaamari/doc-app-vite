namespace DoctorAPI.Model
{
    public class UpdateUserDTO
    {
       
        public string Name { get; set; }
        public string? Phone { get; set; } // Make nullable
        public string? Address { get; set; } // Make nullable
        public char? Gender { get; set; } // Make nullable
        public DateTime? Dob { get; set; } // Make nullable
        public IFormFile? Image { get; set; } // Make nullable

       
        public UpdateUserDTO() { }

    }
}
