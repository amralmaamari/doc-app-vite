namespace DoctorAPI.Model
{
    public class UserLoginModel
    {        
            public string Email { get; set; }
            public string Password { get; set; }

        public UserLoginModel(string Email, string Password) {
                this.Email = Email;
                this.Password = Password;
        }
                         
    }
}
