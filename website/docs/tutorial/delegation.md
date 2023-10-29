# Delegation or Impersonation

TODO : Difference between delegation and Impersonation

TODO : Explains the architecture. Explains a use case, a REST.API wants to use the user claims to access to an another API.

## 1. Define the first permission

1. Open the Identity Server website at [https://localhost:5002](https://localhost:5002).
2. Open the Scopes screen, click on the `Add scope` button.
3. Select `API value` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter   | Value   |
| ----------- | ------- |
| Name        | shopApi |
| Description | shopApi |

5. Navigate to the new scope, then select the `API Resources` tab and click on the `Add API resource` button.
6. Fill-in the form like this and click on the `Add` button to confirm the creation.

| Parameter   | Value   |
| ----------- | ------- |
| Name        | shopApi |
| Audience    | shopApi |
| Description | shopApi |

## 2. Define the second permission

1. Open the Identity Server website at [https://localhost:5002](https://localhost:5002).
2. Open the Scopes screen, click on the `Add scope` button.
3. Select `API value` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter   | Value        |
| ----------- | ------------ |
| Name        | shopApiOther |
| Description | shopApiOther |

5. Navigate to the new scope, then select the `API Resources` tab and click on the `Add API resource` button.
6. Fill-in the form like this and click on the `Add` button to confirm the creation.

| Parameter   | Value        |
| ----------- | ------------ |
| Name        | shopApiOther |
| Audience    | shopApiOther |
| Description | shopApiOther |

## 2. Configure the website

1. Open the Identity Server website at [https://localhost:5002](https://localhost:5002).
2. On the Client screen, click on the `Add client button`.
3. Select `web application` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter        | Value                             |
| ---------------- | --------------------------------- |
| Identifier       | delegationWebsite                 |
| Secret           | password                          |
| Name             | delegationWebsite                 |
| Redirection URLS | http://localhost:5004/signin-oidc |

5. Click on the new client, then select the `Client scopes` tab and click on `Add client scope` button. Choose the `shopApi` scope and click on the `Save` button.

## 3. Configure the API

1. Open the Identity Server website at [https://localhost:5002](https://localhost:5002).
2. On the Client screen, click on the `Add client button`.
3. Select `machine` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter        | Value                             |
| ---------------- | --------------------------------- |
| Identifier       | firstDelegationApi                |
| Secret           | password                          |
| Name             | firstDelegationApi                |

5. Click on the new client, in the `Details` tab, select the `Token exchange` grant-type, set the token exchange type to `Delegation` and click on the `Save` button.
6. Select the `Client scopes` tab and click on `Add client scope` button. Choose the `shopApiOther` scope and click on the `Save` button.

## 4. Create website

TODO

## 5. Create API

TODO

## 6. Create source API

TODO