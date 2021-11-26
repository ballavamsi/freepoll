using System;

namespace freepoll.Models
{
    public partial class User
    {
        public int Userid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Github { get; set; }
        public string Google { get; set; }
        public string Facebook { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UserGuid { get; set; }
        public int Status { get; set; }
        public string PhotoUrl { get; set; }
    }
}