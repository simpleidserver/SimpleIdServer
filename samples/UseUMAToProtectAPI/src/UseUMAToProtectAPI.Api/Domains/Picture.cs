namespace UseUMAToProtectAPI.Api.Domains
{
    public class Picture
    {
        public Picture(string identifier, string userId)
        {
            Identifier = identifier;
            UserId = userId;
        }

        public string Identifier { get; set; }
        public string UserId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string ResourceId { get; set; }
    }
}
