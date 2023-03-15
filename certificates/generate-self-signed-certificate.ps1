$paramsRootCertificate = @{
  DnsName = "SimpleIdServer"
  KeyLength = 2048
  KeyAlgorithm = 'RSA'
  HashAlgorithm = 'SHA256'
  KeyExportPolicy = 'Exportable'
  NotAfter = (Get-Date).AddYears(200)
  KeyUsage = 'CertSign','CRLSign'
  TextExtension = '2.5.29.19={text}cA=true&pathLength=2'
}
$rootCA = New-SelfSignedCertificate @paramsRootCertificate
$CertPassword = ConvertTo-SecureString -String "Password" -Force -AsPlainText
$rootCA | Export-PfxCertificate -FilePath "simpleIdServer.pfx" -Password $CertPassword