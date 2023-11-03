# Configuration

## IdServer

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
            <td rowspan="4">ClientCertificateMode</td>
            <td rowspan="4">
                Specifies the client certificate requirements for a HTTPS connection.<br/>
                This parameter is required when you are using the <b>tls_client_auth</b> or <b>self_signed_tls_client_auth</b> Client authentication method
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
    </tbody>
</table>

## IdServer website

TODO