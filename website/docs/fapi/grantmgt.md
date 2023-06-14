# Grant Management for OAuth 2.0

The FAPI specification introduces the concept of a Grant Management API as part of its security profile.

The FAPI Grant Management API provides a standardized approach to managing and controlling authorization grants in the context of financial-grade applications. 

The basic design principle is that creation and update of grants is always requested using an OAuth authorization request while querying the status of a grant and revoking it are performed using the new Grant Management API.

According to the specification, a grant consists of `scopes`, `claims`, and `authorization_details`.

Here is a high-level overview of the typical flow:

1. Create grant : The client application sends an authorization request to the authorization server, including the query parameter `grant_management_action` with the value `create`.
2. Get the grant identifier : The client application utilizes the authorization code grant type to obtain an access token. Upon successful completion, the authorization server returns a `grant_id`, which will be used to manage the grant associated with the authorization.
3. Get a grant : The client application utilizes the `grant_id` to retrieve a grant from the Grant endpoint.

The client can utilize the new authorization request parameter called `grant_management_action` to update a grant.
It can have one of the following values 

| Action  | Description                                                                                             |
| ------- | ------------------------------------------------------------------------------------------------------- |
| create  | Create a fresh grant                                                                                    |
| merge   | Merge the permissions consented by the resource owner                                                   |
| replace | Change the grant to be only the permissions requested by the client and consented by the resource owner |