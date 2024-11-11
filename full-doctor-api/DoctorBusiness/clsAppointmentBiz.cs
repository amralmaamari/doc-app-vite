using DoctorDB;
using DoctorDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DoctorDB.clsAppointmentDB;

namespace DoctorBusiness
{
    public class clsAppointmentBiz
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

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

        public clsAppointmentDTO AppointmentADO
        {
            get
            {
                return new(this.AppID, this.DoctorID, this.UserID, this.CreatedAt, this.AppState,
                           this.Amount, this.PaymentStatus, this.PaymentMethod, this.SlotDate,
                           this.SlotTime,this.DocData,this.UserData ,this.IsCompleted, this.Cancelled);
            }
        }

        public clsAppointmentBiz(clsAppointmentDTO appointmentADO, enMode mode = enMode.AddNew)
        {
            this.AppID = appointmentADO.AppID;
            this.DoctorID = appointmentADO.DoctorID;
            this.UserID = appointmentADO.UserID;
            this.CreatedAt = appointmentADO.CreatedAt;
            this.AppState = appointmentADO.AppState;
            this.Amount = appointmentADO.Amount;
            this.PaymentStatus = appointmentADO.PaymentStatus;
            this.PaymentMethod = appointmentADO.PaymentMethod;
            this.SlotDate = appointmentADO.SlotDate;
            this.SlotTime = appointmentADO.SlotTime;
            this.DocData = appointmentADO.DocData;
            this.UserData = appointmentADO.UserData;
            this.IsCompleted = appointmentADO.IsCompleted;
            this.Cancelled = appointmentADO.Cancelled;
            this.Mode = mode;
        }

        private bool _AddNewAppointment()
        {
            // Call DataAccess Layer to add a new appointment
            this.AppID = clsAppointmentDB.AddAppointment(AppointmentADO);
            return (this.AppID != 0);
        }

        private bool _UpdateAppointment()
        {
            return clsAppointmentDB.UpdateAppointment(AppointmentADO);
        }

        public static List<clsAppointmentDTO> GetAllAppointments()
        {
            return clsAppointmentDB.GetAllAppointments();
        }

        public static clsAppointmentBiz Find(int appID)
        {
            clsAppointmentDTO appointmentDTO = clsAppointmentDB.GetAppointmentById(appID);
            if (appointmentDTO != null)
            {
                return new clsAppointmentBiz(appointmentDTO, enMode.Update);
            }
            else
            {
                return null;
            }
        }

        public  bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewAppointment())
                    {
                        Mode = enMode.Update; // Change mode to Update after adding
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateAppointment();
            }

            return false;
        }

        public static bool DeleteAppointment(int appID)
        {
            return clsAppointmentDB.DeleteAppointment(appID);
        }



        public static List<clsAppointmentDTO> GetAppointmentsByUserId(int userId)
        {
            if (userId < 1)
                return null;

            return clsAppointmentDB.GetAppointmentsByUserId(userId);
        }

        public static List<clsAppointmentDTO> GetAppointmentsByDoctorId(string doctorId)
        {
            if (doctorId.Length < 0)
                return null;

            return clsAppointmentDB.GetAppointmentsByDoctorId(doctorId);
        }



        public static bool CancelAppointment(int appID)
        {
            return clsAppointmentDB.CancelAppointment(appID);
        }

        public static bool CompleteAppointment(int appID)
        {
            return clsAppointmentDB.CompleteAppointment(appID);
        }



        public static List<AdminDashboardDTO> GetAllAppointmentsAdminDashboard()
        {
            return clsAppointmentDB.GetAllAppointmentsAdminDashboard();
        }

        public static int GetAppointmentCount()
        {
            return clsAppointmentDB.GetAppointmentCount();
        }
    }
}
