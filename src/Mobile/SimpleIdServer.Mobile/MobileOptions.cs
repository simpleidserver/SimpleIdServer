namespace SimpleIdServer.Mobile
{
    public class MobileOptions
    {
        public string PushType { get; set; } = "firebase";
        public bool IsDev { get; set; } = true;
        public bool IgnoreHttps { get; set; } = true;
    }
}
