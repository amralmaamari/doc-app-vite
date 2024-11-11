using DoctorAPI.Model;
using DoctorBusiness;
using DoctorDB.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using static DoctorDB.clsAppointmentDB;

namespace DoctorAPI.Controllers
{
    [Route("api/AppointmentApi")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        // Assuming clsAppointmentBiz is a class that handles the business logic for appointments
        private readonly clsJwtTokenService _jwtTokenService;
        public AppointmentController(clsJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("GetAllAppointments", Name = "GetAllAppointments")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<clsAppointmentDTO>> GetAllAppointments()
        {
            List<clsAppointmentDTO> appointmentDTOs = clsAppointmentBiz.GetAllAppointments();

            if (appointmentDTOs == null || appointmentDTOs.Count == 0)
            {
                return NotFound("There are no appointments.");
            }
            return Ok(new { success =true , appointments = appointmentDTOs });
        }

        



        [HttpGet("details/{id}", Name = "GetAppointmentById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsAppointmentDTO> GetAppointmentById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("The ID must be greater than zero.");
            }

            var appointmentDTO = clsAppointmentBiz.Find(id);
            if (appointmentDTO == null)
            {
                return NotFound($"No appointment found with ID {id}");
            }
            return Ok(new { success = true, appointmentDTO });
        }

        [HttpPost("book-appointment", Name = "AddAppointment")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsAppointmentDTO> AddBookAppointment([FromBody] AddBookAppointmentDTO BookAppointmentDTO)
        {
            

            if (BookAppointmentDTO == null || string.IsNullOrEmpty(BookAppointmentDTO.DoctorID) )
            {
                return BadRequest(new { success = false, message = "Invalid appointment data." });
            }

            // Get the user's identity from the token
            var userId = clsJwtTokenService.GetUserToken(this);
            var existingUser = clsUserBiz.Find(userId);


            if (existingUser == null)
            {
                return NotFound(new { success = false, message = "No user found." });
            }

            var existingDoctor = clsDoctorBiz.Find(BookAppointmentDTO.DoctorID);
            if (existingDoctor == null)
            {
                return NotFound(new { success = false, message = "No Doctor found." });
            }


            clsAppointmentDTO newAppointmentDTO = new clsAppointmentDTO(0, existingDoctor.DoctorID, existingUser.UserID
                ,DateTime.Now, 0, existingDoctor.Fees, 0, 0, BookAppointmentDTO.SlotDate, BookAppointmentDTO.SlotTime,"","",false,false);


            // Assuming clsAppointmentBiz has a method to add a new appointment
            var newAppointment = new clsAppointmentBiz(newAppointmentDTO, clsAppointmentBiz.enMode.AddNew);
             newAppointment.Save();

            int newAppointmentId = newAppointment.AppID;
            if (newAppointmentId <= 0)
            {
                return BadRequest(new { success = false, message = "Appointment could not be created." });
            }
            
            return Ok(new { success = true, appointment = newAppointment });


        }

        [HttpDelete("{id}", Name = "DeleteAppointment")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> DeleteAppointment(int id)
        {
            if (id <= 0)
            {
                return BadRequest("The ID must be greater than zero.");
            }

            var appointmentDTO = clsAppointmentBiz.Find(id);
            if (appointmentDTO == null)
            {
                return NotFound($"No appointment found with ID {id}");
            }

            clsAppointmentBiz.DeleteAppointment(id);
            return Ok(new { success = true });
        }

        [HttpPut("update/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> UpdateAppointment(int id, [FromBody] clsAppointmentDTO updatedAppointmentDTO)
        {
            if (updatedAppointmentDTO == null || id <= 0)
            {
                return BadRequest("Invalid appointment data or ID.");
            }

            var existingAppointment = clsAppointmentBiz.Find(id);
            if (existingAppointment == null)
            {
                return NotFound($"No appointment found with ID {id}");
            }

            // Update the appointment details
            existingAppointment.DoctorID = updatedAppointmentDTO.DoctorID;
            existingAppointment.UserID = updatedAppointmentDTO.UserID;
            existingAppointment.AppState = updatedAppointmentDTO.AppState;
            existingAppointment.Amount = updatedAppointmentDTO.Amount;
            existingAppointment.PaymentStatus = updatedAppointmentDTO.PaymentStatus;
            existingAppointment.PaymentMethod = updatedAppointmentDTO.PaymentMethod;
            existingAppointment.SlotDate = updatedAppointmentDTO.SlotDate;
            existingAppointment.SlotTime = updatedAppointmentDTO.SlotTime;
            existingAppointment.IsCompleted = updatedAppointmentDTO.IsCompleted;
            existingAppointment.Cancelled = updatedAppointmentDTO.Cancelled;

            bool isUpdated = existingAppointment.Save();
            if (!isUpdated)
            {
                return BadRequest(new { success = false, message = "Failed to update appointment." });
            }

            return Ok(new { success = true, message = "Appointment updated successfully." });
        }


        [HttpGet("user/appointments", Name = "GetAppointmentsByUserId")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<clsAppointmentDTO>> GetAppointmentsByUserId()
        {
            var userId = clsJwtTokenService.GetUserToken(this);

            clsUserBiz userInfo = clsUserBiz.Find(userId);

            // Validate user ID
            if (userInfo == null)
            {
                return BadRequest("The User not found.");
            }

            // Retrieve appointments for the specified user ID
            var appointments = clsAppointmentBiz.GetAppointmentsByUserId(userId);

            // Check if any appointments were found
            if (appointments == null || !appointments.Any())
            {
                return NotFound($"No appointments found for User ID {userId}");
            }

            

            return Ok(new { success = true, appointments= appointments });
        }



        //here i will check from the Doctot Panel 
        //When i Will come to Them I will do it < will make it without the doctorID parmater 
        [HttpGet("doctor/{doctorId}", Name = "GetAppointmentsByDoctorId")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<clsAppointmentDTO>> GetAppointmentsByDoctorId(string doctorId)
        {
            // Validate doctor ID
            if (doctorId.Length < 0)
            {
                return BadRequest("The Doctor ID must be greater than zero.");
            }

            // Retrieve appointments for the specified doctor ID
            var appointments = clsAppointmentBiz.GetAppointmentsByDoctorId(doctorId);

            // Check if any appointments were found
            if (appointments == null || !appointments.Any())
            {
                return NotFound($"No appointments found for Doctor ID {doctorId}");
            }

            return Ok(new { success = true, appointments });
        }


        [HttpDelete("CancelAppointment/{appointmentId}", Name = "CancelAppointment")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> CancelAppointment(int appointmentId)
        {
            if (appointmentId <= 0)
            {
                return BadRequest("The ID must be greater than zero.");
            }

            var appointmentDTO = clsAppointmentBiz.Find(appointmentId);
            if (appointmentDTO == null)
            {
                return NotFound($"No appointment found with ID {appointmentId}");
            }

            clsAppointmentBiz.CancelAppointment(appointmentId);
            return Ok(new { success = true, message = "Successfully Cancel" });
        }




    }
}
