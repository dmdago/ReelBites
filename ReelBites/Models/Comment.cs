using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReelBites.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public User Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public string DramaId { get; set; }
        public string ParentCommentId { get; set; } // Para respuestas a comentarios
    }
}
