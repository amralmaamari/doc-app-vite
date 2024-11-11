using DoctorBusiness;
using DoctorDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using DoctorAPI.Model;


namespace DoctorAPI.Controllers
{
    [Route("api/DoctorApi")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        


        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<clsDoctorDTO>> GetAllDoctors()
        {
            List<clsDoctorDTO> DoctorDTO = clsDoctorBiz.GetAllDoctors();

            if (DoctorDTO == null)
            {
                return NotFound("There is no Doctors");
            }
            return Ok(new { success = true, doctors= DoctorDTO });
        }



        [HttpGet("details/{id}", Name = "GetDoctorById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsDoctorDTO> GetDoctorById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("The ID cannot be null or empty.");
            }

            var doctorDTO = clsDoctorBiz.Find(id);
            if (doctorDTO == null)
            {
                return NotFound($"No doctor found with ID {id}");
            }
            return Ok(doctorDTO);
        }

        [HttpGet("fees/{id}", Name = "GetDoctorFeesById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<decimal> GetDoctorFeesById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("The ID cannot be null or empty.");
            }

            var doctorDTO = clsDoctorBiz.Find(id);
            if (doctorDTO == null)
            {
                return NotFound($"No doctor found with ID {id}");
            }
            return Ok(doctorDTO.Fees);
        }

      

        [HttpDelete("{id}", Name = "DeleteDoctor")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> DeleteDoctor(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("The ID cannot be null or empty.");
            }

            var doctorDTO = clsDoctorBiz.Find(id);
            if (doctorDTO == null)
            {
                return NotFound($"No doctor found with ID {id}");
            }

            clsDoctorBiz.DeleteDoctor(id);
            return Ok("Doctor deleted successfully.");
        }

        [HttpPut("update", Name = "UpdateDoctor")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsDoctorDTO> UpdateDoctor(clsDoctorDTO updatedDoctorDTO)
        {
            if (updatedDoctorDTO == null || string.IsNullOrEmpty(updatedDoctorDTO.DoctorID) || string.IsNullOrEmpty(updatedDoctorDTO.Name) || updatedDoctorDTO.Fees < 0)
            {
                return BadRequest("Invalid doctor data.");
            }

            var existingDoctor = clsDoctorBiz.Find(updatedDoctorDTO.DoctorID);
            if (existingDoctor == null)
            {
                return NotFound($"No doctor found with ID {updatedDoctorDTO.DoctorID}");
            }

            existingDoctor.Name = updatedDoctorDTO.Name;
            existingDoctor.Speciality = updatedDoctorDTO.Speciality;
            existingDoctor.Degree = updatedDoctorDTO.Degree;
            existingDoctor.Experience = updatedDoctorDTO.Experience;
            existingDoctor.About = updatedDoctorDTO.About;
            existingDoctor.Fees = updatedDoctorDTO.Fees;
            existingDoctor.AddressLine1 = updatedDoctorDTO.AddressLine1;
            existingDoctor.AddressLine2 = updatedDoctorDTO.AddressLine2;

            existingDoctor.Save();

            return Ok(existingDoctor); // Assuming Save method updates existingDoctor properties
        }

    }
}
