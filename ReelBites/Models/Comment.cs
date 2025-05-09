﻿namespace ReelBites.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public string DramaId { get; set; }
        public string ParentCommentId { get; set; } // Para respuestas a comentarios
    }
}
