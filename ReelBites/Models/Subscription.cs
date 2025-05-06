namespace ReelBites.Models
{
    public class Subscription
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public SubscriptionType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; }
    }

    public enum SubscriptionType
    {
        Free,
        Monthly,
        Annual,
        Lifetime
    }
}
