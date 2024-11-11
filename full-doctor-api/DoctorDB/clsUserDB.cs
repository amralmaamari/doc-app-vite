using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace DoctorDB
{
    public class clsUserDTO
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; } // Make nullable
        public string? Address { get; set; } // Make nullable
        public char? Gender { get; set; } // Make nullable
        public DateTime? Dob { get; set; } // Make nullable
        public string? Image { get; set; } // Make nullable

        // Parameterized constructor
        public clsUserDTO(int userID, string name, string email, string password,
            string? phone, string? address, char? gender, DateTime? dob, string? image)
        {
            UserID = userID;
            Name = name;
            Email = email;
            Password = password;
            Phone = phone;
            Address = address;
            Gender = gender;
            Dob = dob;
            Image = image;
        }
    }

    public class clsUserDB
    {
        static string _connectionString = "Server=.;Database=DoctorApi;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

        public static List<clsUserDTO> GetAllUsers()
        {
            List<clsUserDTO> users = new List<clsUserDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAllUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clsUserDTO user = new clsUserDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("UserID")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                reader.GetString(reader.GetOrdinal("Password")),
                                reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                                reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                                reader.IsDBNull(reader.GetOrdinal("Gender")) ? (char?)null : reader.GetString(reader.GetOrdinal("Gender"))[0],
                                reader.IsDBNull(reader.GetOrdinal("Dob")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Dob")),
                                reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString(reader.GetOrdinal("Image"))
                            );

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }


        public static clsUserDTO GetUserInfoById(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetUserInfoById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new clsUserDTO
                        (
                            reader.GetInt32(reader.GetOrdinal("UserID")),
                            reader.GetString(reader.GetOrdinal("Name")),
                            reader.GetString(reader.GetOrdinal("Email")),
                            reader.GetString(reader.GetOrdinal("Password")),
                            reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                            reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                            reader.IsDBNull(reader.GetOrdinal("Gender")) ? (char?)null : reader.GetString(reader.GetOrdinal("Gender"))[0],
                            reader.IsDBNull(reader.GetOrdinal("Dob")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Dob")),
                            reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString(reader.GetOrdinal("Image"))
                        );
                    }
                    return null;
                }
            }
        }

        //here not chanage
        public static clsUserDTO GetUserInfoByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetUserInfoByEmail", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", email);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new clsUserDTO
                        (
                            reader.GetInt32(reader.GetOrdinal("UserID")),
                            reader.GetString(reader.GetOrdinal("Name")),
                            reader.GetString(reader.GetOrdinal("Email")),
                            reader.GetString(reader.GetOrdinal("Password")),
                            reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                            reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                            reader.IsDBNull(reader.GetOrdinal("Gender")) ? (char?)null : reader.GetString(reader.GetOrdinal("Gender"))[0],
                            reader.IsDBNull(reader.GetOrdinal("Dob")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Dob")),
                            reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString(reader.GetOrdinal("Image"))

                        );
                    }
                    return null;
                }
            }
        }


        public static int AddUser(clsUserDTO newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Name) || string.IsNullOrWhiteSpace(newUser.Email) || string.IsNullOrWhiteSpace(newUser.Password))
            {
                throw new ArgumentException("Name, Email, and Password cannot be null or empty.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 200) { Value = newUser.Name });
                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = newUser.Email });
                    command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Value = newUser.Password });
                    command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar, 50) { Value = (object)newUser.Phone ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 200) { Value = (object)newUser.Address ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@Gender", SqlDbType.Char, 1) { Value = (object)newUser.Gender ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@Dob", SqlDbType.Date) { Value = (object)newUser.Dob ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@Image", SqlDbType.VarBinary, -1) { Value = (object)newUser.Image ?? DBNull.Value });
                    var outputIdParam = new SqlParameter("@NewUserId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputIdParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)outputIdParam.Value;
                }
            }
        }





        public static bool UpdateUser(clsUserDTO updatedUser)
        {
            if (updatedUser.UserID <= 0)
            {
                throw new ArgumentException("User ID cannot be less than or equal to zero.", nameof(updatedUser.UserID));
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@UserID", updatedUser.UserID);
                    command.Parameters.AddWithValue("@Name", (object)updatedUser.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object)updatedUser.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Address", (object)updatedUser.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Gender", (object)updatedUser.Gender ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Dob", (object)updatedUser.Dob ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Image", (object)updatedUser.Image ?? DBNull.Value); // Ensure this matches the expected type

                    connection.Open();
                    try
                    {
                        var rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (SqlException ex)
                    {
                        // Log the SQL exception details for debugging
                        Console.WriteLine($"SQL Error: {ex.Message}");
                        throw; // Optionally re-throw or handle it as needed
                    }
                    catch (Exception ex)
                    {
                        // Log general exceptions
                        Console.WriteLine($"Error: {ex.Message}");
                        throw; // Optionally re-throw or handle it as needed
                    }
                }
            }
        }


        public static bool DeleteUser(int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();

                    var rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static clsUserDTO GetUserInfoByEmailAndPassword(string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetUserInfoByEmailAndPassword", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);


                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new clsUserDTO
                        (
                            reader.GetInt32(reader.GetOrdinal("UserID")),
                            reader.GetString(reader.GetOrdinal("Name")),
                            reader.GetString(reader.GetOrdinal("Email")),
                            reader.GetString(reader.GetOrdinal("Password")),
                            reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                            reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                            reader.IsDBNull(reader.GetOrdinal("Gender")) ? (char?)null : reader.GetString(reader.GetOrdinal("Gender"))[0],
                            reader.IsDBNull(reader.GetOrdinal("Dob")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Dob")),
                            reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString(reader.GetOrdinal("Image"))
                        );
                    }
                    return null;
                }
            }
        }


        public static int GetUserCount()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Use a SQL query to call the scalar function directly
                using (SqlCommand command = new SqlCommand("SELECT dbo.GetUserCount()", connection))
                {
                    connection.Open();

                    // Execute the command and get the value directly
                    int count = (int)command.ExecuteScalar();

                    // Return the result
                    return count;
                }
            }
        }

    }
}
