namespace ReelBites.Models
{
    public class Drama
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public User Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public int SharesCount { get; set; }
        public int ViewsCount { get; set; }
        public List<string> Tags { get; set; }
        public string CoverImageUrl { get; set; }
        public DramaCategory Category { get; set; }
        public bool IsPremium { get; set; }
        public double Duration { get; set; } // En minutos
    }
}
