using SimpleIdServer.Did;
using SQLite;

namespace SimpleIdServer.Mobile.Models
{
    public class DidRecord
    {
        [PrimaryKey]
        public string Did { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string SerializedPrivateKey { get; set; }
        public bool IsActive { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        [Ignore]
        public string DisplayName
        {
            get
            {
                var extractor = DidExtractor.Extract(Did);
                return $"{CreateDateTime.ToString()} - {extractor.Method}";
            }
        }
    }
}
