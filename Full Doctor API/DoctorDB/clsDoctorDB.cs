using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace DoctorDB
{

    public class clsDoctorDTO
    {
       
        public string DoctorID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public string Speciality { get; set; }
        public string Degree { get; set; }
        public string Experience { get; set; }
        public string About { get; set; }
        public decimal Fees { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public bool Available { get; set; }
        public string SlotsBooked { get; set; }
        
       

    public clsDoctorDTO(string doctorID, string name, string email, string password, string image,
            string speciality, string degree, string experience,
            string about, decimal fees, string addressLine1,
            string addressLine2,bool available, string slotsBooked)
        {
            DoctorID = doctorID;
            Name = name;
            Email = email;
            Password = password;
            Image = image;
            Speciality = speciality;
            Degree = degree;
            Experience = experience;
            About = about;
            Fees = fees;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            Available = available;
            SlotsBooked = slotsBooked;
        }
    }

    public class clsDoctorDB
    {
        static string _connectionString = "Server=.;Database=DoctorApi;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";


        public static clsDoctorDTO GetDoctorById(string doctorId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetDoctorById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new clsDoctorDTO
                        (
                            reader.GetString(reader.GetOrdinal("DoctorID")),
                            reader.GetString(reader.GetOrdinal("Name")),
                            //reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")),
                            //reader.IsDBNull(reader.GetOrdinal("Password")) ? "" : reader.GetString(reader.GetOrdinal("Password")),

                            reader.GetString(reader.GetOrdinal("Email")),
                            reader.GetString(reader.GetOrdinal("Password")),
                            reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString(reader.GetOrdinal("Image")),
                            reader.IsDBNull(reader.GetOrdinal("Speciality")) ? null : reader.GetString(reader.GetOrdinal("Speciality")),
                            reader.IsDBNull(reader.GetOrdinal("Degree")) ? null : reader.GetString(reader.GetOrdinal("Degree")),
                            reader.IsDBNull(reader.GetOrdinal("Experience")) ? null : reader.GetString(reader.GetOrdinal("Experience")),
                            reader.IsDBNull(reader.GetOrdinal("About")) ? null : reader.GetString(reader.GetOrdinal("About")),
                            reader.IsDBNull(reader.GetOrdinal("Fees")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Fees")),
                            reader.IsDBNull(reader.GetOrdinal("AddressLine1")) ? null : reader.GetString(reader.GetOrdinal("AddressLine1")),
                            reader.IsDBNull(reader.GetOrdinal("AddressLine2")) ? null : reader.GetString(reader.GetOrdinal("AddressLine2")),
                            reader.GetBoolean(reader.GetOrdinal("Available")),
                            reader.IsDBNull(reader.GetOrdinal("SlotsBooked")) ? null : reader.GetString(reader.GetOrdinal("SlotsBooked"))

                        );
                    }
                    return null;
                }
            }
        }

        public static List<clsDoctorDTO> GetAllDoctors()
        {
            List<clsDoctorDTO> doctorList = new List<clsDoctorDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM dbo.FN_GetDoctors()", connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // Loop through all records
                        {
                            doctorList.Add(new clsDoctorDTO
                            (
                                reader.GetString(reader.GetOrdinal("DoctorID")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                reader.IsDBNull(reader.GetOrdinal("Password")) ? null : reader.GetString(reader.GetOrdinal("Password")),
                            reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString(reader.GetOrdinal("Image")),
                                reader.IsDBNull(reader.GetOrdinal("Speciality")) ? null : reader.GetString(reader.GetOrdinal("Speciality")),
                                reader.IsDBNull(reader.GetOrdinal("Degree")) ? null : reader.GetString(reader.GetOrdinal("Degree")),
                                reader.IsDBNull(reader.GetOrdinal("Experience")) ? null : reader.GetString(reader.GetOrdinal("Experience")),
                                reader.IsDBNull(reader.GetOrdinal("About")) ? null : reader.GetString(reader.GetOrdinal("About")),
                                reader.IsDBNull(reader.GetOrdinal("Fees")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Fees")),
                                reader.IsDBNull(reader.GetOrdinal("AddressLine1")) ? null : reader.GetString(reader.GetOrdinal("AddressLine1")),
                                reader.IsDBNull(reader.GetOrdinal("AddressLine2")) ? null : reader.GetString(reader.GetOrdinal("AddressLine2")),
                                reader.GetBoolean(reader.GetOrdinal("Available")),
                                reader.IsDBNull(reader.GetOrdinal("SlotsBooked")) ? null : reader.GetString(reader.GetOrdinal("SlotsBooked"))


                            ));
                        }
                    }
                }
            }
            return doctorList;
        }

        public static string AddDoctor(clsDoctorDTO newDoctor)
        {
            if (string.IsNullOrWhiteSpace(newDoctor.Name))
            {
                throw new ArgumentException("Doctor name cannot be null or empty.", nameof(newDoctor.Name));
            }

            // Ensure that DoctorID is provided by the user
            if (string.IsNullOrWhiteSpace(newDoctor.DoctorID))
            {
                throw new ArgumentException("Doctor ID cannot be null or empty.", nameof(newDoctor.DoctorID));
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewDoctor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Passing the user-entered DoctorID
                    command.Parameters.AddWithValue("@DoctorID", newDoctor.DoctorID);
                    command.Parameters.AddWithValue("@Name", newDoctor.Name);
                    command.Parameters.AddWithValue("@Email", newDoctor.Email);
                    command.Parameters.AddWithValue("@Password", newDoctor.Password);
                    command.Parameters.AddWithValue("@Image", newDoctor.Image ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Speciality", newDoctor.Speciality ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Degree", newDoctor.Degree ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Experience", newDoctor.Experience ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@About", newDoctor.About ?? (object)DBNull.Value);
                    command.Parameters.Add("@Fees", SqlDbType.Decimal).Value = newDoctor.Fees;
                    command.Parameters.AddWithValue("@AddressLine1", newDoctor.AddressLine1 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AddressLine2", newDoctor.AddressLine2 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Available", true);
                    command.Parameters.AddWithValue("@SlotsBooked", newDoctor.SlotsBooked ?? (object)DBNull.Value);


                    // Output parameter is no longer needed since we are passing DoctorID as input
                    connection.Open();
                    command.ExecuteNonQuery();

                    // Return the DoctorID entered by the user
                    return newDoctor.DoctorID;
                }
            }
        }


        public static bool UpdateDoctor(clsDoctorDTO updatedDoctor)
        {
            if (string.IsNullOrWhiteSpace(updatedDoctor.DoctorID))
            {
                throw new ArgumentException("Doctor ID cannot be null or empty.", nameof(updatedDoctor.DoctorID));
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateDoctor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DoctorID", updatedDoctor.DoctorID);
                    command.Parameters.AddWithValue("@Name", updatedDoctor.Name);
                    command.Parameters.AddWithValue("@Image", updatedDoctor.Image ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Speciality", updatedDoctor.Speciality ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Degree", updatedDoctor.Degree ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Experience", updatedDoctor.Experience ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@About", updatedDoctor.About ?? (object)DBNull.Value);
                    command.Parameters.Add("@Fees", SqlDbType.Decimal).Value = updatedDoctor.Fees;
                    command.Parameters.AddWithValue("@AddressLine1", updatedDoctor.AddressLine1 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AddressLine2", updatedDoctor.AddressLine2 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SlotsBooked", updatedDoctor.SlotsBooked ?? (object)DBNull.Value);


                    connection.Open();
                    var rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool DeleteDoctor(string doctorId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteDoctor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DoctorID", doctorId);
                    connection.Open();

                    var rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Return true if any rows were affected
                }
            }
        }


        public static int GetDoctorCount()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Use a SQL query to call the scalar function directly
                using (SqlCommand command = new SqlCommand("SELECT dbo.GetDoctorCount()", connection))
                {
                    connection.Open();

                    // Execute the command and get the value directly
                    int count = (int)command.ExecuteScalar();

                    // Return the result
                    return count;
                }
            }
        }

        public static async Task<int> GetLastDoctorID()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Use a SQL query to call the scalar function directly
                using (SqlCommand command = new SqlCommand("SP_GetLastDoctorID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    // Execute the command and get the value directly
                    int lastDoctorId = (int)await command.ExecuteScalarAsync();

                    // Return the result
                    return lastDoctorId;
                }
            }
        }


        public static bool ChangeDoctorAvailability(string doctorID)
        {
            int rowAffected = 0;

            // Establish the SQL connection
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Create the SQL command and specify the stored procedure
                using (SqlCommand command = new SqlCommand("SP_DoctorChangeAvailability", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Better practice: Explicitly specify parameter type and size
                    command.Parameters.Add(new SqlParameter("@DoctorID", SqlDbType.NVarChar, 100)).Value = doctorID;

                    // Open the connection
                    connection.Open();

                    // Execute the command (non-query since we are performing an UPDATE)
                    rowAffected = command.ExecuteNonQuery();
                }
            }

            // If one or more rows were affected, return true
            return (rowAffected > 0);
        }
    }
    }
