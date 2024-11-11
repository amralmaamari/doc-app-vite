using DoctorDB.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorDB
{
    public class clsAppointmentDB
    {
        public class clsAppointmentDTO
        {
            public int AppID { get; set; }
            public string DoctorID { get; set; }
            public int UserID { get; set; }
            public DateTime CreatedAt { get; set; }
            public byte AppState { get; set; }
            public decimal Amount { get; set; }
            public byte? PaymentStatus { get; set; } // Nullable
            public byte? PaymentMethod { get; set; } // Nullable
            public string SlotDate { get; set; }
            public string SlotTime { get; set; }
            public string DocData { get; set; }
            public string UserData { get; set; }
            public bool IsCompleted { get; set; }
            public bool Cancelled { get; set; }

            public clsAppointmentDTO(int appID, string doctorID, int userID, DateTime createdAt, byte appState,
                decimal amount, byte? paymentStatus, byte? paymentMethod, string slotDate, string slotTime,
                 string docData, string userData, bool isCompleted, bool cancelled)
            {
                AppID = appID;
                DoctorID = doctorID;
                UserID = userID;
                CreatedAt = createdAt;
                AppState = appState;
                Amount = amount;
                PaymentStatus = paymentStatus;
                PaymentMethod = paymentMethod;
                SlotDate = slotDate;
                SlotTime = slotTime;
                DocData = docData;
                UserData = userData;
                IsCompleted = isCompleted;
                Cancelled = cancelled;
            }
        }
        static string _connectionString = "Server=.;Database=DoctorApi;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";
        public static clsAppointmentDTO GetAppointmentById(int appId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetAppointmentById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AppID", appId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new clsAppointmentDTO
                        (
                            reader.GetInt32(reader.GetOrdinal("AppID")),
                            reader.GetString(reader.GetOrdinal("DoctorID")),
                            reader.GetInt32(reader.GetOrdinal("UserID")),
                            reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            reader.GetByte(reader.GetOrdinal("AppState")),
                            reader.GetDecimal(reader.GetOrdinal("Amount")),
                            reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? (byte?)null : reader.GetByte(reader.GetOrdinal("PaymentStatus")),
                            reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ? (byte?)null : reader.GetByte(reader.GetOrdinal("PaymentMethod")),
                            reader.GetString(reader.GetOrdinal("SlotDate")),
                            reader.GetString(reader.GetOrdinal("SlotTime")),
                            reader.GetString(reader.GetOrdinal("DocData")),
                            reader.GetString(reader.GetOrdinal("UserData")),
                            reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                            reader.GetBoolean(reader.GetOrdinal("Cancelled"))
                        );
                    }
                    return null;
                }
            }
        }

        public static int AddAppointment(clsAppointmentDTO newAppointment)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(newAppointment.DoctorID) ||
                newAppointment.UserID <= 0 ||
                newAppointment.AppState < 0 ||
                newAppointment.Amount < 0)
            {
                throw new ArgumentException("DoctorID, UserID, AppState, and Amount cannot be null or empty, and UserID must be greater than 0.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddAppointment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding parameters to the command
                    command.Parameters.Add(new SqlParameter("@DoctorID", SqlDbType.NVarChar, 50) { Value = newAppointment.DoctorID });
                    command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = newAppointment.UserID });
                    command.Parameters.Add(new SqlParameter("@AppState", SqlDbType.TinyInt) { Value = newAppointment.AppState });
                    command.Parameters.Add(new SqlParameter("@Amount", SqlDbType.Decimal) { Value = newAppointment.Amount });
                    command.Parameters.Add(new SqlParameter("@PaymentStatus", SqlDbType.TinyInt) { Value = (object)newAppointment.PaymentStatus ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@PaymentMethod", SqlDbType.TinyInt) { Value = (object)newAppointment.PaymentMethod ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@SlotDate", SqlDbType.NVarChar, 50) { Value = newAppointment.SlotDate });
                    command.Parameters.Add(new SqlParameter("@SlotTime", SqlDbType.NVarChar, 50) { Value = newAppointment.SlotTime });
                    command.Parameters.Add(new SqlParameter("@IsCompleted", SqlDbType.Bit) { Value = newAppointment.IsCompleted });
                    command.Parameters.Add(new SqlParameter("@Cancelled", SqlDbType.Bit) { Value = newAppointment.Cancelled });

                    // Adding the output parameter
                    var outputIdParam = new SqlParameter("@NewAppointmentId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputIdParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    // Return the new appointment ID
                    return (int)outputIdParam.Value;
                }
            }
        }


        public static bool UpdateAppointment(clsAppointmentDTO updatedAppointment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("SP_UpdateAppointment", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AppID", updatedAppointment.AppID);
                command.Parameters.AddWithValue("@DoctorID", updatedAppointment.DoctorID);
                command.Parameters.AddWithValue("@UserID", updatedAppointment.UserID);
                command.Parameters.AddWithValue("@AppState", updatedAppointment.AppState);
                command.Parameters.AddWithValue("@Amount", updatedAppointment.Amount);
                command.Parameters.AddWithValue("@PaymentStatus", updatedAppointment.PaymentStatus ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PaymentMethod", updatedAppointment.PaymentMethod ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SlotDate", updatedAppointment.SlotDate);
                command.Parameters.AddWithValue("@SlotTime", updatedAppointment.SlotTime);
                command.Parameters.AddWithValue("@IsCompleted", updatedAppointment.IsCompleted);
                command.Parameters.AddWithValue("@Cancelled", updatedAppointment.Cancelled);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public static bool DeleteAppointment(int appId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("SP_DeleteAppointment", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AppID", appId);
                connection.Open();

                var rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public static List<clsAppointmentDTO> GetAllAppointments()
        {
            List<clsAppointmentDTO> appointmentList = new List<clsAppointmentDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("SELECT * FROM dbo.FN_GetAllAppointments()", connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        appointmentList.Add(new clsAppointmentDTO
                        (
                            reader.GetInt32(reader.GetOrdinal("AppID")),
                            reader.GetString(reader.GetOrdinal("DoctorID")),
                            reader.GetInt32(reader.GetOrdinal("UserID")),
                            reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            reader.GetByte(reader.GetOrdinal("AppState")),
                            reader.GetDecimal(reader.GetOrdinal("Amount")),
                            reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? (byte?)null : reader.GetByte(reader.GetOrdinal("PaymentStatus")),
                            reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ? (byte?)null : reader.GetByte(reader.GetOrdinal("PaymentMethod")),
                            reader.GetString(reader.GetOrdinal("SlotDate")),
                            reader.GetString(reader.GetOrdinal("SlotTime")),
                            reader.GetString(reader.GetOrdinal("DocData")),
                            reader.GetString(reader.GetOrdinal("UserData")),
                            reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                            reader.GetBoolean(reader.GetOrdinal("Cancelled"))
                        ));
                    }
                }
            }
            return appointmentList;
        }



        public static List<clsAppointmentDTO> GetAppointmentsByUserId(int userId)
        {
            var appointments = new List<clsAppointmentDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAppointmentsByUserId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var appointment = new clsAppointmentDTO
                            (
                               reader.GetInt32(reader.GetOrdinal("AppID")),
                               reader.GetString(reader.GetOrdinal("DoctorID")),
                               reader.GetInt32(reader.GetOrdinal("UserID")),
                               reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                               reader.GetByte(reader.GetOrdinal("AppState")),
                               reader.GetDecimal(reader.GetOrdinal("Amount")),
                               reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? (byte?)null : reader.GetByte(reader.GetOrdinal("PaymentStatus")),
                               reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ? (byte?)null : reader.GetByte(reader.GetOrdinal("PaymentMethod")),
                               reader.GetString(reader.GetOrdinal("SlotDate")),
                               reader.GetString(reader.GetOrdinal("SlotTime")),
                               reader.GetString(reader.GetOrdinal("DocData")),
                               reader.GetString(reader.GetOrdinal("UserData")),
                               reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                               reader.GetBoolean(reader.GetOrdinal("Cancelled"))
                            );
                            appointments.Add(appointment);
                        }
                    }
                }
            }

            return appointments;
        }



        public static List<clsAppointmentDTO> GetAppointmentsByDoctorId(string doctorId)
        {
            var appointments = new List<clsAppointmentDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAppointmentsByDoctorId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@DoctorID", SqlDbType.NVarChar) { Value = doctorId.ToString() });

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var appointment = new clsAppointmentDTO
                            (
                               reader.GetInt32(reader.GetOrdinal("AppID")),
                               reader.GetString(reader.GetOrdinal("DoctorID")),
                               reader.GetInt32(reader.GetOrdinal("UserID")),
                               reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                               reader.GetByte(reader.GetOrdinal("AppState")),
                               reader.GetDecimal(reader.GetOrdinal("Amount")),
                               reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? (byte?)null : reader.GetByte(reader.GetOrdinal("PaymentStatus")),
                               reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ? (byte?)null : reader.GetByte(reader.GetOrdinal("PaymentMethod")),
                               reader.GetString(reader.GetOrdinal("SlotDate")),
                               reader.GetString(reader.GetOrdinal("SlotTime")),
                               reader.GetString(reader.GetOrdinal("DocData")),
                               reader.GetString(reader.GetOrdinal("UserData")),
                               reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                               reader.GetBoolean(reader.GetOrdinal("Cancelled"))
                            );
                            appointments.Add(appointment);
                        }
                    }
                }
            }

            return appointments;
        }



        public static bool CancelAppointment(int appId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("SP_CancelAppointment", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AppID", appId);

                try
                {
                    connection.Open();
                    var rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if an appointment was cancelled
                }
                catch (SqlException ex)
                {
                    // Handle exceptions (log or rethrow)
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    return false; // Return false on error
                }
                catch (Exception ex)
                {
                    // Handle general exceptions
                    Console.WriteLine($"Error: {ex.Message}");
                    return false; // Return false on error
                }
            }
        }



        public static bool CompleteAppointment(int appId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("SP_CompleteAppointment", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AppID", appId);

                try
                {
                    connection.Open();
                    var rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if an appointment was cancelled
                }
                catch (SqlException ex)
                {
                    // Handle exceptions (log or rethrow)
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    return false; // Return false on error
                }
                catch (Exception ex)
                {
                    // Handle general exceptions
                    Console.WriteLine($"Error: {ex.Message}");
                    return false; // Return false on error
                }
            }
        }





        //Start Admin 
        public static List<AdminDashboardDTO> GetAllAppointmentsAdminDashboard()
        {
            List<AdminDashboardDTO> appointmentList = new List<AdminDashboardDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("SELECT * FROM dbo.FN_GetAdminDashboard()", connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        appointmentList.Add(new AdminDashboardDTO
                        (
                            reader.GetInt32(reader.GetOrdinal("AppID")),
                            reader.IsDBNull(reader.GetOrdinal("Image")) ?  reader.GetSqlBinary(reader.GetOrdinal("Image")).Value : null,
                            reader.GetString(reader.GetOrdinal("Name")),
                            reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            reader.GetBoolean(reader.GetOrdinal("Cancelled")),
                             reader.GetBoolean(reader.GetOrdinal("IsCompleted"))

                        ));
                    }
                }
            }
            return appointmentList;
        }

        public static int GetAppointmentCount()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Use a SQL query to call the scalar function directly
                using (SqlCommand command = new SqlCommand("SELECT dbo.GetAppointmentCount()", connection))
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
