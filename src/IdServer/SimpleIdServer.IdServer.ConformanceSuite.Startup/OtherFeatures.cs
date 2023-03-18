using Org.BouncyCastle.Ocsp;
using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.ConformanceSuite.Startup
{
    public static class OtherFeatures
    {
        private const string DIRECTORYPATH = @"c:\Projects\SimpleIdServer\certificates";

        public static void CreateCertificateAuthority(string caName)
        {
            using (RSA parent = RSA.Create(2048))
            using (RSA rsa = RSA.Create(2048))
            {
                CertificateRequest parentReq = new CertificateRequest(
                "CN=sidCA",
                parent,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

                parentReq.CertificateExtensions.Add(
                    new X509BasicConstraintsExtension(true, false, 0, true));

                parentReq.CertificateExtensions.Add(
                    new X509SubjectKeyIdentifierExtension(parentReq.PublicKey, false));

                using (X509Certificate2 parentCert = parentReq.CreateSelfSigned(
                    DateTimeOffset.UtcNow.AddDays(-45),
                    DateTimeOffset.UtcNow.AddDays(365)))
                {
                    var exported = new X509Certificate2(parentCert.Export(X509ContentType.Pfx, "password"), "password", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                    var caPrivateKey = parentCert.GetRSAPrivateKey().ExportRSAPrivateKeyPem();
                    File.WriteAllText(Path.Combine(DIRECTORYPATH, "sidCA.crt"), SanitizePem(parentCert.ExportCertificatePem()));
                    File.WriteAllText(Path.Combine(DIRECTORYPATH, "sidCA.key"), SanitizePem(caPrivateKey));
                    File.WriteAllBytes(Path.Combine(DIRECTORYPATH, "sidCA.pfx"), exported.Export(X509ContentType.Pfx, "password"));
                    CertificateRequest req = new CertificateRequest(
                        "CN=sidClient",
                        rsa,
                        HashAlgorithmName.SHA256,
                        RSASignaturePadding.Pkcs1);

                    req.CertificateExtensions.Add(
                        new X509BasicConstraintsExtension(false, false, 0, false));

                    req.CertificateExtensions.Add(
                        new X509KeyUsageExtension(
                            X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DataEncipherment,
                            false));

                    req.CertificateExtensions.Add(
                        new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

                    using (X509Certificate2 cert = req.Create(
                        parentCert,
                        DateTimeOffset.UtcNow.AddDays(-1),
                        DateTimeOffset.UtcNow.AddDays(90),
                        new byte[] { 1, 2, 3, 4 }))
                    {
                        var privatePem = new string(PemEncoding.Write("PRIVATE KEY", rsa.ExportPkcs8PrivateKey()));
                        File.WriteAllText(Path.Combine(DIRECTORYPATH, "client.crt"), SanitizePem(cert.ExportCertificatePem()));
                        File.WriteAllText(Path.Combine(DIRECTORYPATH, "client.key"), SanitizePem(privatePem));
                        // Do something with these certs, like export them to PFX,
                        // or add them to an X509Store, or whatever.
                    }
                }
            }

            string SanitizePem(string pem) => pem.Replace("\n", "\r\n");
        }
    }
}
