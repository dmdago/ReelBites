namespace ReelBites.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string RelatedItemId { get; set; } // ID de un drama, comentario, etc.
    }

    public enum NotificationType
    {
        Like,
        Comment,
        Follow,
        Mention,
        NewDrama,
        System
    }
}
