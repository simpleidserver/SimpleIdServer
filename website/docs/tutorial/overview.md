---
hide_table_of_contents: true
---

import DocsCards from '@site/src/components/global/DocsCards';
import DocsCard from '@site/src/components/global/DocsCard';

# Start building

## Choose your application

<DocsCards>
    <DocsCard header="Single-Page Application" href="/docs/tutorial/spa">
        <p>A client-side application running in a browser.</p>
    </DocsCard>
    <DocsCard header="Regular Web Application" href="/docs/tutorial/regularweb">
        <p>A server-side application running on your infrastructure.</p>
    </DocsCard>
    <DocsCard header="Protect a REST.API Service" href="/docs/tutorial/protectapi">
        <p>An API protected by SimpleIdServer.</p>
    </DocsCard>
    <DocsCard header="Machine to Machine" href="/docs/tutorial/m2m">
        <p>Machine-to-machine method of communication.</p>
    </DocsCard>
</DocsCards>

# Start building applications compliant with FAPI

According to the [specification](https://openid.bitbucket.io/fapi/fapi-2_0-security-profile.html), there is no mechanism that would allow `public clients` to be secured to the same degree.

<DocsCards>
    <DocsCard header="Highly secured Regular Web Application" href="/docs/tutorial/highlysecuredregularweb">
        <p>A server-side application running on your infrastructure compliant with the FAPI recommendations.</p>
    </DocsCard>
    <DocsCard header="Client-Initiated Backchannel" href="/docs/tutorial/ciba">
        <p>Authentication process on behalf of the end-user.</p>
    </DocsCard>
    <DocsCard header="Grant Management" href="/docs/tutorial/grantmgt">
        <p>Consent management APIs that have been developed in open banking markets.</p>
    </DocsCard>
</DocsCards>