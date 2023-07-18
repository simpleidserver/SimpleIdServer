using System.Security.Cryptography.X509Certificates;

namespace Website;

public class WebsiteOptions
{
    public string CallbackUrl { get; set; } = "http://localhost:7000/callback";
    public string AccountInfoUrl { get; set; } = "http://localhost:7001/AccountInfo";
    public X509Certificate2 MTLSCertificate { get; set; }
}
