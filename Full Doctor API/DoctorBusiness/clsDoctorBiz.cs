using DoctorDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorBusiness
{
    public class clsDoctorBiz
    {
        public enum enMode { Addnew = 0, Update = 1 };
        public enMode Mode = enMode.Addnew;


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

        public clsDoctorDTO DoctorADO
        {
            get
            {
                return new(this.DoctorID, this.Name,this.Email,this.Password,
                 this.Image,   this.Speciality, this.Degree,
                this.Experience, this.About, this.Fees,
                this.AddressLine1, this.AddressLine2,this.Available,this.SlotsBooked);
            }
        }

        public clsDoctorBiz(clsDoctorDTO doctorADO, enMode mode = enMode.Addnew)
        {
            this.DoctorID = doctorADO.DoctorID;
            this.Name = doctorADO.Name;
            this.Email = doctorADO.Email;
            this.Password = doctorADO.Password;
            this.Image = doctorADO.Image;
            this.Speciality = doctorADO.Speciality;
            this.Degree = doctorADO.Degree;
            this.Experience = doctorADO.Experience;
            this.About = doctorADO.About;
            this.Fees = doctorADO.Fees;
            this.AddressLine1 = doctorADO.AddressLine1;
            this.AddressLine2 = doctorADO.AddressLine2;
            this.Available = doctorADO.Available;
            this.SlotsBooked= doctorADO.SlotsBooked;
            this.Mode = mode;
        }

        private bool _AddNewDoctor()
        {
            // Call DataAccess Layer to add a new doctor
            this.DoctorADO.DoctorID = clsDoctorDB.AddDoctor(DoctorADO);
            return (this.DoctorADO.DoctorID != null);
        }

        private bool _UpdateDoctor()
        {
            return clsDoctorDB.UpdateDoctor(DoctorADO);
        }

        public static List<clsDoctorDTO> GetAllDoctors()
        {
            return clsDoctorDB.GetAllDoctors();
        }

        public static clsDoctorBiz Find(string doctorID)
        {
            clsDoctorDTO doctorDTO = clsDoctorDB.GetDoctorById(doctorID);
            if (doctorDTO != null)
            {
                return new clsDoctorBiz(doctorDTO, enMode.Update);
            }
            else
            {
                return null;
            }
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.Addnew:
                    if (_AddNewDoctor())
                    {
                        Mode = enMode.Update; // Change mode to Update after adding
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateDoctor();
            }

            return false;
        }

        public static bool DeleteDoctor(string doctorID)
        {
            return clsDoctorDB.DeleteDoctor(doctorID);
        }

        public static int GetDoctorCount()
        {
            return clsDoctorDB.GetDoctorCount();
        }

        public static async Task<int> GetLastDoctorID()
        {
            return await clsDoctorDB.GetLastDoctorID();
        }

        public static bool ChangeDoctorAvailability(string doctorID)
        {
            return clsDoctorDB.ChangeDoctorAvailability(doctorID);
        }
    }
}
