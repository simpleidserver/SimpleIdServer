---
hide_table_of_contents: true
---

import DocsCards from '@site/src/components/global/DocsCards';
import DocsCard from '@site/src/components/global/DocsCard';

# Start building

## Standard applications

<DocsCards>
    <DocsCard header="Single-Page Application" icon="fa-brands fa-angular" href="spa">
        <p>A client-side application running in a browser.</p>
    </DocsCard>
    <DocsCard header="Regular Web Application" icon="fa-solid fa-server" href="regularweb">
        <p>A server-side application running on your infrastructure.</p>
    </DocsCard>
    <DocsCard header="Protect a REST.API Service" icon="fas fa-exchange-alt" href="protectapi">
        <p>An API protected by SimpleIdServer.</p>
    </DocsCard>
    <DocsCard header="Machine to Machine" icon="fa-solid fa-terminal" href="m2m">
        <p>Machine-to-machine method of communication.</p>
    </DocsCard>
</DocsCards>

## Applications compliant with FAPI 2.0

According to the [specification](https://openid.bitbucket.io/fapi/fapi-2_0-security-profile.html), there is no mechanism that would allow `public clients` to be secured to the same degree.

<DocsCards>
    <DocsCard header="Highly secured Regular Web Application" icon="fa-solid fa-user-shield" href="highlysecuredregularweb">
        <p>A server-side application running on your infrastructure compliant with the FAPI recommendations.</p>
    </DocsCard>
    <DocsCard header="Client-Initiated Backchannel" icon="fa-solid fa-bell" href="ciba">
        <p>Authentication process on behalf of the end-user.</p>
    </DocsCard>
    <DocsCard header="Grant Management" icon="fa-solid fa-square-check" href="grantmgt">
        <p>Consent management APIs that have been developed in open banking markets.</p>
    </DocsCard>
</DocsCards>

## Credential issuer

<DocsCards>
    <DocsCard header="Credential issuer" href="credentialissuer" icon="fas fa-barcode text-xl">
        <p>Issue Verifiable Credentials</p>
    </DocsCard>
</DocsCards>

## Other applications

<DocsCards>
    <DocsCard header="WS-Federation RP" icon="fas fa-user-shield" href="wsfederation">
        <p>A server-side application running on your infrastructure that utilizes WS-Federation.</p>
    </DocsCard>
    <DocsCard header="SAML2.0 SP" href="saml" icon="fas fa-shield-alt">
        <p>A Service Provider (SP) is the entity providing the service, typically in the form of an application</p>
    </DocsCard>
    <DocsCard header="Openid federation RP" icon="fa-solid fa-people-group" href="openidfederation">
        <p>A server-side application running on your infrastructure that utilizes OPENID federation.</p>
    </DocsCard>
</DocsCards>

## Identity provisioning

<DocsCards>
    <DocsCard header="LDAP Provisioning" href="ldap" icon="fa-solid fa-folder-tree">
        <p>Configure automatic provisioning with LDAP.</p>
    </DocsCard>
    <DocsCard header="SCIM Provisioning" icon="fas fa-users-cog" href="scim">
        <p>Configure automatic provisioning with SCIM</p>
    </DocsCard>
    <DocsCard header="Openid FastFed" icon="fas fa-exchange-alt" href="fastfed">
        <p>Configure Openid FastFed</p>
    </DocsCard>
</DocsCards>

## Delegation

<DocsCards>
    <DocsCard header="Delegation" icon="fa-solid fa-people-group" href="delegation">
        <p>Configure impersonation or delegation.</p>
    </DocsCard>
</DocsCards>