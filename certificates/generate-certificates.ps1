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

$paramsFirstMtlsClient = @{
  DnsName = "firstMtlsClient"
  Signer = $rootCA
  KeyLength = 2048
  KeyAlgorithm = 'RSA'
  HashAlgorithm = 'SHA256'
  KeyExportPolicy = 'Exportable'
  NotAfter = (Get-date).AddYears(2)
}
$firstMtlsClient = New-SelfSignedCertificate @paramsFirstMtlsClient
Export-Certificate -Cert $firstMtlsClient -FilePath "firstMtlsClient.crt"

$paramsSecondMtlsClient = @{
  DnsName = "secondMtlsClient"
  Signer = $rootCA
  KeyLength = 2048
  KeyAlgorithm = 'RSA'
  HashAlgorithm = 'SHA256'
  KeyExportPolicy = 'Exportable'
  NotAfter = (Get-date).AddYears(2)
}
$secondMtlsClient = New-SelfSignedCertificate @paramsSecondMtlsClient
Export-Certificate -Cert $secondMtlsClient -FilePath "secondMtlsClient.crt"