using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs
{
    public class QRCodeResult
    {
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; } = null!;
        [JsonPropertyName("read_qrcode_url")]
        public string ReadQRCodeURL { get; set; } = null!;
        [JsonPropertyName("action")]
        public string Action { get; set; } = null!;

        public string GetOrigin()
        {
            var uri = new Uri(ReadQRCodeURL); 
            return uri.GetLeftPart(UriPartial.Authority);
        }
    }
}
