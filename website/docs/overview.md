# Overview

import Architecture from '@site/src/components/global/Architecture';

<Architecture></Architecture>

SimpleIdServer is the first open-source project, developed with .NET, which covers all the aspects of an `Identity and Access Management` software.

<table>
    <thead>
        <tr>
            <th>IAM aspects</th>
            <th>Features</th>
        </tr>
    </thead>
    <tbody>
        <tr><td rowspan="3"><b>Authentication</b></td><td>Supports multiple security protocols such as : <i>OPENID</i>, <i>SAML2.0</i> and <i>WS-Federation</i></td></tr>
        <tr><td>Configure one or more external identity providers such as <i>Facebook</i>.</td></tr>
        <tr><td>Use different authentication methods like `Mobile application`, `OTP Code`, `Email`, `SMS` and `Login & Password`.</td></tr>
        <tr><td><b>Authorization</b></td><td>`Role-Based Access control` (RBAC) is used to control the user's permission in one or more applications.</td></tr>
        <tr><td rowspan="2"><b>User management</b></td><td> One or more `automatic identity provisioning` workflow can be configured, they automatically fetch users from `LDAP` and/or `SCIM2.0`</td></tr>
        <tr><td>One or more `manual identity provisioning` workflows can be configured , the end-user must manually enters his credentials.</td></tr>
        <tr><td><b>Central user repository</b></td><td>Our SCIM2.0 server can be used as a central user-repository.</td></tr>
    </tbody>
</table>