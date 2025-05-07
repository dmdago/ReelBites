// Añade estas propiedades adicionales a tu modelo User existente (Models/User.cs)

namespace ReelBites.Models
{
    public class User
    {
        // Propiedades existentes
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

        // Propiedades adicionales para el perfil
        public string FullName { get; set; }
        public bool NotificationsEnabled { get; set; }
        public bool IsPrivate { get; set; }

        // Otras propiedades que podrían ser útiles
        public string Website { get; set; }
        public string Location { get; set; }
        public List<string> Interests { get; set; }
        public bool IsFollowing { get; set; }
    }
}