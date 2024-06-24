using System.Data;
using Microsoft.AspNetCore.Identity;

namespace Reddit.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string RefreshToken { get; set; }
        public DataSetDateTime RefreshTokenExpiryTime { get; set; }
    }
}
