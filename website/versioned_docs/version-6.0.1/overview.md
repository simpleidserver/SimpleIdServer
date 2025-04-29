# Overview

import ClientCertificate from './images/clientcertificate.png';
import Architecture from '@site/static/img/architecture.svg'; 

:::warning

When attempting to authenticate with the Identity Provider or access the administration website, a prompt will appear, requesting user credentials. Please click the 'Cancel' button to proceed to the authentication screen. SimpleIdServer is configured by default to accept client certificates.

<div style={{textAlign:"center"}}>
    <img src={ClientCertificate} style={{width: 400}} />
</div>
:::

<Architecture />

SimpleIdServer is the first open-source project developed with .NET that comprehensively covers all aspects of an **Identity and Access Management** software

<table>
    <thead>
        <tr>
            <th>IAM aspects</th>
            <th>SimpleIdServer's features</th>
        </tr>
    </thead>
    <tbody>
        <tr><td rowspan="3"><b>Authentication</b></td><td>Supports multiple security protocols such as : <b>OPENID</b>, <b>SAML2.0</b> and <b>WS-Federation</b></td></tr>
        <tr><td>Configure one or more external identity providers, such as <b>Facebook</b>.</td></tr>
        <tr><td>Utilize various authentication methods such as <b>Mobile application</b>, <b>OTP Code</b>, <b>Email</b>, <b>SMS</b> and <b>Login & Password</b>.</td></tr>
        <tr><td><b>Authorization</b></td><td>Control the user's permissions in one or more applications using <b>Role-Based Access Control (RBAC)</b>.</td></tr>
        <tr><td rowspan="2"><b>User management</b></td><td>Configure one or more automatic identity provisioning workflows to fetch users automatically from <b>LDAP</b> and/or <b>SCIM 2.0</b> REST API.</td></tr>
        <tr><td>Configure one or more <b>manual identity provisioning workflows</b> that can be used by the end-user to create an account.</td></tr>
        <tr><td><b>Central user repository</b></td><td>Our SCIM 2.0 server can be used to store users and groups.</td></tr>
    </tbody>
</table>