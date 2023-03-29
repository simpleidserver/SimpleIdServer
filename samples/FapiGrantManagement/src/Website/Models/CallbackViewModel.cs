namespace Website.Models
{
    public class CallbackViewModel
    {
        public string GrantId { get; set; }
        public IEnumerable<string> Accounts { get; set; }
    }
}
