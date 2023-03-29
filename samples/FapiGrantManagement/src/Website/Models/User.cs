namespace Website.Models
{
    public class User
    {
        public string Id { get; set; }
        public ICollection<UserGrant> Grants { get; set; } = new List<UserGrant>();

        public UserGrant GetGrant(string bankAccount) => Grants.FirstOrDefault(g => g.BankAccount == bankAccount);
    }

    public class UserGrant
    {
        public string GrantId { get; set; }
        public string BankAccount { get; set; }
    }
}
