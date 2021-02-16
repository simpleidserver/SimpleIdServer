$paramsRootCertificate = @{
  DnsName = "SimpleIdServer Client"
  KeyLength = 2048
  KeyAlgorithm = 'RSA'
  HashAlgorithm = 'SHA256'
  KeyExportPolicy = 'Exportable'
  NotAfter = (Get-Date).AddYears(5)
  KeyUsage = 'CertSign','CRLSign'
}
$rootCA = New-SelfSignedCertificate @paramsRootCertificate
Export-Certificate -Cert $rootCA -FilePath "simpleIdServerClient.crt"
Import-Certificate -CertStoreLocation 'Cert:\LocalMachine\Root' -FilePath "simpleIdServerClient.crt"
$paramsMtlsClient = @{
  DnsName = "mtlsClient"
  Signer = $rootCA
  KeyLength = 2048
  KeyAlgorithm = 'RSA'
  HashAlgorithm = 'SHA256'
  KeyExportPolicy = 'Exportable'
  NotAfter = (Get-date).AddYears(2)
}
$mtlsClient = New-SelfSignedCertificate @paramsMtlsClient
Export-Certificate -Cert $mtlsClient -FilePath "mtlsClient.crt"