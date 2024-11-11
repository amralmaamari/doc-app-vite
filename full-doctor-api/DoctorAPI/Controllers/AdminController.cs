using DoctorAPI.Model;
using DoctorAPI.Model.Admin;
using DoctorBusiness;
using DoctorDB;
using DoctorDB.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static DoctorDB.clsAppointmentDB;

namespace DoctorAPI.Controllers
{
    [Route("api/Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly clsJwtTokenService _jwtTokenService;
        private readonly CloudinaryService _cloudinaryService;

        // Use a single constructor to inject both services
        public AdminController(clsJwtTokenService jwtTokenService, CloudinaryService cloudinaryService)
        {
            _jwtTokenService = jwtTokenService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> VerfiyAdmin(AdminDTO admin)
        {
            if (admin == null)
            {
                return BadRequest("The admin object is null"); // Use BadRequest for null admin
            }

            var isFound = clsAdminBiz.VerfiyAdmin(admin);

            if (!isFound)
            {
                return NotFound("The admin is not found");
            }

            var token = _jwtTokenService.GenerateToken(admin.Email);
            return Ok(new { success = true, token });
        }



        [HttpGet("dashboard", Name = "dashboard")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<AdminDashboardDTO>> GetAllAppointmentsAdminDashboard()
        {
            List<AdminDashboardDTO> appointmentDTOs = clsAppointmentBiz.GetAllAppointmentsAdminDashboard();
            int DoctorCount = clsDoctorBiz.GetDoctorCount();
            int AppointmentCount = clsAppointmentBiz.GetAppointmentCount();
            int UserCount = clsUserBiz.GetUserCount();


            if (appointmentDTOs == null || appointmentDTOs.Count == 0)
            {
                return NotFound("There are no appointments.");
            }
            return Ok(new { success = true, dashData = new { appointmentDTOs, Statistics = new { DoctorCount, AppointmentCount, UserCount } } });
        }


        [HttpGet("appointments", Name = "appointments")]
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
            return Ok(new { success = true, appointments = appointmentDTOs });
        }



        [HttpDelete("CancelAppointment/{appointmentId}", Name = "AdminCancelAppointment")]
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



        [HttpGet("all-doctors")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<clsDoctorDTO>> GetAllDoctors()
        {
            List<clsDoctorDTO> DoctorDTO = clsDoctorBiz.GetAllDoctors();

            if (DoctorDTO == null)
            {
                return NotFound("There is no Doctors");
            }
            return Ok(new { success = true, doctors = DoctorDTO });
        }




        [HttpPost("change-availability")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeAvailability([FromForm] ChangeAvailabilityDTO request)
        {

            if (request == null || string.IsNullOrEmpty(request.DoctorID))
            {
                return BadRequest(new { success = false, message = "Invalid doctor ID." });
            }

            try
            {
                if (clsDoctorBiz.ChangeDoctorAvailability(request.DoctorID))
                    // Return the result
                    return Ok(new { success = true, message = "change availability sucessfuly" });
                else
                    return StatusCode(500, new { success = false, message = "not change availability" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add-doctor")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddNewDoctor([FromForm] AddDoctorDTO request)
        {
            // Validate input
            if (request == null || string.IsNullOrEmpty(request.Name) || request.Fees < 0)
            {
                return BadRequest("Invalid doctor data.");
            }

            // Generate new Doctor ID
            int lastDoctorID = await clsDoctorDB.GetLastDoctorID();
            string newDoctorID = $"doc{lastDoctorID + 1}";

            // Initialize image URL
            string imageUrl = string.Empty;

            // Handle image upload if provided
            if (request.Image != null)
            {
                try
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(request.Image, "Doctor");

                    // Check if the upload was successful
                    if (uploadResult == null)
                    {
                        return StatusCode(500, "Error uploading image.");
                    }

                    // Set the image URL to be saved in the database
                    imageUrl = uploadResult.Url.ToString();
                }
                catch (Exception ex)
                {
                    // Log the exception (consider using a logging framework)
                    return StatusCode(500, $"Error uploading file: {ex.Message}");
                }
            }

            // Create a new doctor DTO
            var newDoctor = new clsDoctorDTO(
                newDoctorID,
                request.Name,
                request.Email,
                request.Password,
                imageUrl,
                request.Speciality,
                request.Degree,
                request.Experience,
                request.About,
                request.Fees,
                request.AddressLine1,
                request.AddressLine2,
                true,
                "{}"
            );

            // Save the new doctor
            clsDoctorBiz doctor = new clsDoctorBiz(newDoctor, clsDoctorBiz.enMode.Addnew);

            if (doctor.Save())
            {
                return CreatedAtAction(nameof(AddNewDoctor), new { id = newDoctorID }, new { success = true, message = "Successfully Added" });
            }

            return BadRequest(new { success = false, message = "Not added the new doctor" });
        }
    }
    }
