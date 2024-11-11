namespace DoctorAPI.Model
{
    public class AddBookAppointmentDTO
    {
        public string DoctorID { get; set; }
        public string? SlotDate { get; set; } // Now nullable
        public string? SlotTime { get; set; } // Now nullable

        public AddBookAppointmentDTO(string doctorID, string? slotDate, string? slotTime)
        {
            this.DoctorID = doctorID;
            this.SlotDate = slotDate;
            this.SlotTime = slotTime;
        }
    }

}
