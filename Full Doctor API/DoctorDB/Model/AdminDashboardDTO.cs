using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorDB.Model
{
    public class AdminDashboardDTO
    {
        public int AppID { get; set; }
        public byte[]? Image { get; set; } // Make nullable
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Cancelled { get; set; }
        public bool IsCompleted { get; set; }

 
        public AdminDashboardDTO(int appID, byte[]? image, string name,
            DateTime createdAt, bool cancelled, bool isCompleted)
        {
            AppID = appID;
            Image = image;
            Name = name;
            CreatedAt = createdAt;
            Cancelled = cancelled;
            IsCompleted = isCompleted;
           
        }
    }
}
