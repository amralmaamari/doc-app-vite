using DoctorDB;
using System;
using System.Collections.Generic;

namespace DoctorBusiness
{
    public class clsUserBiz
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; } // Nullable
        public string? Address { get; set; } // Nullable
        public char? Gender { get; set; } // Nullable
        public DateTime? Dob { get; set; } // Nullable
        public string? Image { get; set; } // Nullable

        public clsUserDTO UserDTO
        {
            get
            {
                return new(this.UserID, this.Name, this.Email, this.Password,
                           this.Phone, this.Address, this.Gender,
                           this.Dob, this.Image);
            }
        }

        public clsUserBiz(clsUserDTO userDTO, enMode mode = enMode.AddNew)
        {
            this.UserID = userDTO.UserID;
            this.Name = userDTO.Name;
            this.Email = userDTO.Email;
            this.Password = userDTO.Password;
            this.Phone = userDTO.Phone;
            this.Address = userDTO.Address;
            this.Gender = userDTO.Gender;
            this.Dob = userDTO.Dob;
            this.Image = userDTO.Image;

            this.Mode = mode;
        }

        private bool _AddNewUser()
        {
            // Call DataAccess Layer to add a new user
            this.UserID = clsUserDB.AddUser(UserDTO);
            return (this.UserID != -1);
        }

        private bool _UpdateUser()
        {
            return clsUserDB.UpdateUser(UserDTO);
        }

        public static List<clsUserDTO> GetAllUsers()
        {
            return clsUserDB.GetAllUsers();
        }

        public static clsUserBiz? Find(int userId)
        {
            clsUserDTO? userDTO = clsUserDB.GetUserInfoById(userId);
            if (userDTO != null)
            {
                return new clsUserBiz(userDTO, enMode.Update);
            }
            else
            {
                return null;
            }
        }

        public static clsUserDTO? FindObject(int userId)
        {
            clsUserDTO? userDTO = clsUserDB.GetUserInfoById(userId);
            return userDTO;
        }

        public static clsUserBiz GetUserInfoByEmail(string email)
        {
            clsUserDTO? userDTO = clsUserDB.GetUserInfoByEmail(email);
            if (userDTO != null)
            {
                return new clsUserBiz(userDTO, enMode.Update);
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
                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update; // Change mode to Update after adding
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateUser();
            }

            return false;
        }

        public static bool DeleteUser(int userId)
        {
            return clsUserDB.DeleteUser(userId);
        }

        public static clsUserBiz VerifyUserByEmailAndPassword(string email, string password)
        {
            clsUserDTO? userDTO = clsUserDB.GetUserInfoByEmailAndPassword(email, password);
            if (userDTO != null)
            {
                return new clsUserBiz(userDTO, enMode.Update);
            }
            else
            {
                return null;
            }
          
        }
        public static int GetUserCount()
        {
            return clsUserDB.GetUserCount();
        }
    }
}
