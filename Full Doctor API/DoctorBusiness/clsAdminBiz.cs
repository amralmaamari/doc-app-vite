using DoctorDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorBusiness
{
    public class clsAdminBiz
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public clsAdminBiz( string email,string password) {
            this.Email = email;
            this.Password = password;
        }

        public static bool VerfiyAdmin(AdminDTO adminDTO)
        {
            return clsAdminDB.VerfiyAdmin(adminDTO);
        }
    }
}
