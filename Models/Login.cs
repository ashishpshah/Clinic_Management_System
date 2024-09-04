using Clinic_Management_System;

namespace Clinic_Management_System
{
    public partial class Login : EntitiesBase
    {
       
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
