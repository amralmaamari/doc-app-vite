using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorDB
{

    public class AdminDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public AdminDTO(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }
    }
    public class clsAdminDB
    {
        static string _connectionString = "Server=.;Database=DoctorApi;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

        public static bool VerfiyAdmin(AdminDTO adminDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("SP_VerfiyAdmin", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", adminDTO.Email);
                command.Parameters.AddWithValue("@Password", adminDTO.Password);

                // Declare the output parameter
                SqlParameter isExistParameter = new SqlParameter("@IsExist", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(isExistParameter);

                connection.Open();
                command.ExecuteNonQuery();

                // Retrieve the output parameter value
                var isExist = (bool)isExistParameter.Value;
                return isExist; // This will be true or false based on the existence
            }
        }


    }
}
