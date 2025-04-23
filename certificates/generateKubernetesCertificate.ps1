$cert = New-SelfSignedCertificate `
  -DnsName "sid.svc.cluster.local", "scim.sid.svc.cluster.local", "idserver.sid.svc.cluster.local", `
           "website.sid.svc.cluster.local", "credentialissuer.sid.svc.cluster.local", "credentialissuerwebsite.sid.svc.cluster.local" `
  -CertStoreLocation "cert:\LocalMachine\My" `
  -NotAfter (Get-Date).AddYears(5) `
  -FriendlyName "Cluster Local Cert"
  
$pwd = ConvertTo-SecureString -String "password" -Force -AsPlainText

Export-PfxCertificate `
  -Cert $cert `
  -FilePath "sid.pfx" `
  -Password $pwd