using DoctorBusiness;
using DoctorDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using DoctorAPI.Model;
using System.Reflection;
using static DoctorDB.clsAppointmentDB;


namespace DoctorAPI.Controllers
{
    [Route("api/UserApi")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly clsJwtTokenService _jwtTokenService;
        private readonly CloudinaryService _cloudinaryService;

        // Use a single constructor to inject both services
        public UserController(clsJwtTokenService jwtTokenService, CloudinaryService cloudinaryService)
        { 
            _jwtTokenService = jwtTokenService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet("Get", Name = "Userlist")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<clsUserDTO>> GetAllUsers()
        {
            List<clsUserDTO> userDTOs = clsUserBiz.GetAllUsers();

            if (userDTOs == null || userDTOs.Count == 0)
            {
                return NotFound("There are no users.");
            }
            return Ok(userDTOs);
        }

        [HttpGet("details/{id}", Name = "GetUserById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsUserDTO> GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("The ID must be greater than zero.");
            }

            var userDTO = clsUserBiz.Find(id);
            if (userDTO == null)
            {
                return NotFound($"No user found with ID {id}");
            }
          return Ok(new { success = true, userDTO });

        }



        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsUserDTO> AddNewUser([FromBody] clsUserDTO newUserDTO)
        {
            Console.WriteLine($"Received user: {Newtonsoft.Json.JsonConvert.SerializeObject(newUserDTO)}");

            if (newUserDTO == null || string.IsNullOrEmpty(newUserDTO.Name) || string.IsNullOrEmpty(newUserDTO.Email) || string.IsNullOrEmpty(newUserDTO.Password))
            {
                return BadRequest(new { success = false, message = "Invalid user data." });
            }

            clsUserBiz userBiz = new clsUserBiz(newUserDTO, clsUserBiz.enMode.AddNew);
            userBiz.Save();

            // Assuming that Save() method populates the UserID if successful
            if (userBiz.UserID <= 0)
            {
                return BadRequest(new { success = false, message = "User could not be created." });
            }

            var token = _jwtTokenService.GenerateToken(newUserDTO.UserID);
            

            // Return success response with token
            return Ok(new { success = true, token});
        }




        [HttpDelete("{id}", Name = "delete-profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("The ID must be greater than zero.");
            }

            var userDTO = clsUserBiz.Find(id);
            if (userDTO == null)
            {
                return NotFound($"No user found with ID {id}");
            }

            clsUserBiz.DeleteUser(id);
            return Ok(new { success = true });
        }





        [HttpPut("update-profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDTO updatedUserDTO)
        {
            if (updatedUserDTO == null || string.IsNullOrEmpty(updatedUserDTO.Name))
            {
                return BadRequest("Invalid user data.");
            }

            // Retrieve the userId from the token or another source
            var userId = clsJwtTokenService.GetUserToken(this);

            var existingUser = clsUserBiz.Find(userId);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            // Process the image if it's provided
            if (updatedUserDTO.Image != null)
            {
                try
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(updatedUserDTO.Image, "User");
                    // Further processing...
                    if (uploadResult == null)
                    {
                        return StatusCode(500, "Error uploading image.");
                    }

                    // Set the image URL to be saved in the database
                    var url = uploadResult.Url.ToString();
                    existingUser.Image = url;
                }
                catch (Exception ex)
                {
                    // Log the exception
                    return StatusCode(500, $"Error uploading file: {ex.Message}");
                }

                
            }

            // Update other user fields
            existingUser.Name = updatedUserDTO.Name;
            existingUser.Phone = updatedUserDTO.Phone;
            existingUser.Address = updatedUserDTO.Address;
            existingUser.Gender = updatedUserDTO.Gender;
            existingUser.Dob = updatedUserDTO.Dob;

            // Save the updated user info in the database
            existingUser.Save();

            return Ok(new { success = true, message = "Profile updated successfully." });
        }



        [HttpPost("login", Name = "login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Login([FromBody] UserLoginModel Login)
        {
            // Validate the input credentials
            if (Login == null || string.IsNullOrEmpty(Login.Email) || string.IsNullOrEmpty(Login.Password))
            {
                return BadRequest("Email and password cannot be null or empty.");
            }

            // Attempt to verify the user using the provided credentials
            clsUserBiz userDTO = clsUserBiz.VerifyUserByEmailAndPassword(Login.Email, Login.Password);

            // Check if the userDTO is null, meaning no user was found
            if (userDTO == null)
            {
                //return NotFound("User not found or invalid credentials.");
                return Unauthorized("Invalid credentials");
            }

            var token = _jwtTokenService.GenerateToken(userDTO.UserID);
            return Ok(new { success = true, token });
        }



        [HttpGet("get-profile")]
        [Authorize] // This requires the user to be authenticated
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<clsUserDTO> GetUserProfile()
        {

            // Get the user's identity from the token
            var userId = clsJwtTokenService.GetUserToken(this);

       

            // For demonstration, let's assume you're fetching it from a business logic layer.
            clsUserBiz userProfile = clsUserBiz.Find(Convert.ToInt32(userId)); // Adjust as necessary

            if (userProfile == null)
            {
                return NotFound(new { success = false, message = "User not found." });
            }

            return Ok(new { success = true, userData = userProfile });
        }


        [HttpPost("book-appointment")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsAppointmentDTO> AddBookAppointment([FromBody] AddBookAppointmentDTO BookAppointmentDTO)
        {


            if (BookAppointmentDTO == null || string.IsNullOrEmpty(BookAppointmentDTO.DoctorID))
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
                , DateTime.Now, 0, existingDoctor.Fees, 0, 0, BookAppointmentDTO.SlotDate, BookAppointmentDTO.SlotTime, "", "", false, false);


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

    }










}


