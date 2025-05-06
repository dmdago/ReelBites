using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReelBites.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int DramasCount { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsVerified { get; set; }
    }
}
