---
hide_table_of_contents: true
---

import DocsCards from '@site/src/components/global/DocsCards';
import DocsCard from '@site/src/components/global/DocsCard';

# Start building

## Standard applications

<DocsCards>
    <DocsCard header="Single-Page Application" href="spa">
        <p>A client-side application running in a browser.</p>
    </DocsCard>
    <DocsCard header="Regular Web Application" href="regularweb">
        <p>A server-side application running on your infrastructure.</p>
    </DocsCard>
    <DocsCard header="Protect a REST.API Service" href="protectapi">
        <p>An API protected by SimpleIdServer.</p>
    </DocsCard>
    <DocsCard header="Machine to Machine" href="m2m">
        <p>Machine-to-machine method of communication.</p>
    </DocsCard>
</DocsCards>

# Applications compliant with FAPI 2.0

According to the [specification](https://openid.bitbucket.io/fapi/fapi-2_0-security-profile.html), there is no mechanism that would allow `public clients` to be secured to the same degree.

<DocsCards>
    <DocsCard header="Highly secured Regular Web Application" href="highlysecuredregularweb">
        <p>A server-side application running on your infrastructure compliant with the FAPI recommendations.</p>
    </DocsCard>
    <DocsCard header="Client-Initiated Backchannel" href="ciba">
        <p>Authentication process on behalf of the end-user.</p>
    </DocsCard>
    <DocsCard header="Grant Management" href="grantmgt">
        <p>Consent management APIs that have been developed in open banking markets.</p>
    </DocsCard>
</DocsCards>

# Other applications

<DocsCards>
    <DocsCard header="WS-Federation RP" href="wsfederation">
        <p>A server-side application running on your infrastructure that utilizes WS-Federation.</p>
    </DocsCard>
    <DocsCard header="SAML2.0 SP" href="saml">
        <p>A Service Provider (SP) is the entity providing the service, typically in the form of an application</p>
    </DocsCard>
</DocsCards>

## Identity provisioning

<DocsCards>
    <DocsCard header="LDAP Provisioning" href="ldap">
        <p>Configure automatic provisioning with LDAP.</p>
    </DocsCard>
    <DocsCard header="SCIM Provisioning" href="scim">
        <p>Configure automatic provisioning with SCIM</p>
    </DocsCard>
</DocsCards>

## Delegation

<DocsCards>
    <DocsCard header="Delegation" href="delegation">
        <p>Configure impersonation or delegation.</p>
    </DocsCard>
</DocsCards>