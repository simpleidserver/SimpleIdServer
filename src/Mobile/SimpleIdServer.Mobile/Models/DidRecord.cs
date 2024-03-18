using SQLite;

namespace SimpleIdServer.Mobile.Models
{
    public class DidRecord
    {
        [PrimaryKey]
        public string Did { get; set; }
        public byte[] PrivaterKey { get; set; }
    }
}
