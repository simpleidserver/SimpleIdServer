using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace SimpleIdServer.Mobile.Stores
{
    public interface ICertificateStore
    {
        Task Add(byte[] credentialId, CertificateRecord certificateRecord);
    }

    public class CertificateStore : ICertificateStore
    {
        public Task Add(byte[] credentialId, CertificateRecord certificateRecord) => SecureStorage.Default.SetAsync(Convert.ToBase64String(credentialId), JsonSerializer.Serialize(certificateRecord));
    }

    public record CertificateRecord
    {
        public CertificateRecord(X509Certificate2 certificate, ECDsa privateKey)
        {
            PublicKey = certificate.RawData;
            PrivateKeyParameters = privateKey.ExportParameters(true);
        }

        public byte[] PublicKey { get; private set; }
        public ECParameters PrivateKeyParameters { get; private set; }
        public X509Certificate2 Certificate
        {
            get
            {
                return new X509Certificate2(PublicKey);
            }
        }
        public ECDsa PrivateKey
        {
            get
            {
                return ECDsa.Create(PrivateKeyParameters);
            }
        }
    }
}