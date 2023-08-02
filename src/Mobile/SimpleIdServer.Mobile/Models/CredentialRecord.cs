using SQLite;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Mobile.Models
{
    public class CredentialRecord
    {
        public CredentialRecord()
        {

        }

        public CredentialRecord(byte[] credentialId, X509Certificate2 certificate, ECDsa privateKey)
        {
            Id = Convert.ToBase64String(credentialId);
            PublicKey = Convert.ToBase64String(certificate.RawData);
            ECDsaPublicPem = privateKey.ExportSubjectPublicKeyInfoPem();
            ECDsaPrivatePem = privateKey.ExportECPrivateKeyPem();
        }

        [PrimaryKey]
        public string Id { get; set; }
        public string PublicKey { get; private set; }
        public string ECDsaPublicPem { get; private set; }
        public string ECDsaPrivatePem { get; private set; }
        public X509Certificate2 Certificate
        {
            get
            {
                return new X509Certificate2(Convert.FromBase64String(PublicKey));
            }
        }
        public ECDsa PrivateKey
        {
            get
            {
                var ecdsa = ECDsa.Create();
                ecdsa.ImportFromPem(ECDsaPublicPem);
                ecdsa.ImportFromPem(ECDsaPrivatePem);
                return ecdsa;
            }
        }
    }
}
