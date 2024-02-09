# Configuration

## IdentityServer

The table below, list all the possible properties present in the `appsettings.json` file. Thanks to them, you can easily customize the behavior the [IdentityServer](../installation#create-identityserver-project).

<table>
    <thead>
        <tr>
            <th>Property</th>
            <th>Description</th>
            <th>Values</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><b>AuthCookieNamePrefix</b></td>
            <td>Name of the authentication cookie</td>
            <td></td>
        </tr>
        <tr>
            <td><b>SessionCookieNamePrefix</b></td>
            <td>Name of the session cookie</td>
            <td></td>
        </tr>
        <tr>
            <td rowspan="2"><b>ForceHttps</b></td>
            <td rowspan="2">Force to use HTTPS</td>
            <td>True</td>
        </tr>
        <tr>
            <td>False</td>
        </tr>
        <tr>
            <td rowspan="2"><b>IsForwardedEnabled</b></td>
            <td rowspan="2">Enable or disable the forwarded headers</td>
            <td>true</td>
        </tr>
        <tr>
            <td>false</td>
        </tr>
        <tr>
            <td rowspan="4"><b>ClientCertificateMode</b></td>
            <td rowspan="4">
                Specifies the client certificate requirements for an HTTPS connection.<br/>
                This parameter is required when you are using the <b>tls_client_auth</b> or <b>self_signed_tls_client_auth</b> client authentication method. <br/>
                By default, the value is <b>NoCertificate</b>.
            </td>
            <td>NoCertificate</td>
        </tr>
        <tr>
            <td>AllowCertificate</td>
        </tr>
        <tr>
            <td>RequireCertificate</td>
        </tr>
        <tr>
            <td>DelayCertificate</td>
        </tr>
        <tr>
            <td rowspan="2"><b>IsRealmEnabled</b></td>
            <td rowspan="2">Enable or disable the Realm. By default, the value is <b>true</b></td>
            <td>true</td>
        </tr>
        <tr>
            <td>false</td>
        </tr>
        <tr>
            <td><b>SCIMBaseUrl</b></td>
            <td>
                Base URL of the SCIM Server. This value is used during the launch time of IdentityServer to configure Automatic Identity Provisioning with the SCIM Server..<br/>
                By default, the value is <b>https://localhost:5003</b>.
            </td>
            <td>Base URL of the SCIM Server</td>
        </tr>
        <tr>
            <td><b>Authority</b></td>
            <td>
                Base URL of the current IdentityServer. This value is used to configure OPENID authentication with the IdentityServer.<br/>
                By default, the value is <b>https://localhost:5001</b>.
            </td>
            <td>Base URL of the current IdentityServer.</td>
        </tr>
        <tr>
            <td><a href="../iam/configuration"><b>DistributedConfiguration</b></a></td>
            <td>
                Distributed configuration helps various modules within SimpleIdServer to store their settings. <br />
                This property is used to configure the configuration storage, for example, <b>Redis</b> or <b>SQL Server</b>.
            </td>
            <td>For more information, please refer to this <a href="../iam/configuration">chapter</a></td>
        </tr>
        <tr>
            <td><a href="../iam/storage"><b>StorageConfiguration</b></a></td>
            <td>This property is used to configure the data storage used by IdentityServer to store its various entities, such as <b>Clients</b> or <b>Users</b>.</td>
            <td>For more information, please refer to this <a href="../iam/storage">chapter</a></td>
        </tr>
        <tr>
            <td rowspan="6"><b>Other</b></td>
            <td rowspan="6">The other properties are used to configure the modules used by IdentityServer, such as <b>Automatic Identity Provisioning with SCIM</b> or an external Identity Provider like <b>Facebook</b></td>
            <td><a href="../iam/externalidproviders#facebook">Facebook</a></td>
        </tr>
        <tr>            
            <td><a href="../iam/automaticidentityprovisioning.md#scim">SCIM</a></td>
        </tr>
        <tr>            
            <td><a href="../iam/automaticidentityprovisioning.md#ldap">LDAP</a></td>
        </tr>
        <tr>            
            <td><a href="../iam/authmethods.md#email">IdServerEmailOptions</a></td>
        </tr>
        <tr>            
            <td><a href="../iam/authmethods.md#sms">IdServerSmsOptions</a></td>
        </tr>
        <tr>            
            <td>FidoOptions</td>
        </tr>
    </tbody>
</table>