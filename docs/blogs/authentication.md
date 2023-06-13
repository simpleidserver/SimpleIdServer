# Authentication

## Authentication Context Class Reference (ACR) 

The Authentication Context Class Reference (ACR) specifies a set of business rules that the authentications are being requested to satisfy. 
These rules can often be satisfied by using a number of different authentication methods, either singly or in combination.

In short, the Authentication Context Class Reference (ACR) ensures the correctness of the user identity. 
In SimpleIdServer, the ACR is similar to the Level Of Assurance. The higher your Level Of Assurance, the better the identity of a user can be trusted.  

It's up to the Relying Party to specify the ACR value. This value is passed in the `acr_values` parameter with the authorization query.

The list of Authentication Context Class Reference (ACR) can be configured in the table `dbo.Acrs`. 

By default, the following ACR values are configured 

| ACR         | Authentication Method Reference |
| ----------- | ------------------------------- |
| sid-load-01 | pwd                             |
| email       | email                           |
| sms         | sms                             |
| pwd-email   | pwd, email                      |


## Login Password Authentication

**Authentication method** : pwd

By default, the `pwd` authentication method is enabled.

## Email Authentication

**Authentication method** : email

Add the function `AddEmailAuthentication` to register the dependencies and update the properties of the `IdServerEmailOptions` options.

```
services.AddSIDIdentityServer()
        .AddEmailAuthentication(o => {});
```

`IdServerEmailOptions` has the following properties :

| Property     | Description                                              | Default Value  |
| ------------ | -------------------------------------------------------- | -------------- |
| SmtpUserName | Email is used to authenticate against the SMTP server    |                |
| SmtpPassword | Password is used to authenticate against the SMTP server |                |
| FromEmail    | Sender of the email                                      |                |
| SmtpHost     | SMTP Host                                                | smtp.gmail.com |
| SmtpPort     | SMTP Port                                                | 587            |

## SMS Authentication

**Authentication method** : sms

By default, [Twilio](https://www.twilio.com/) is used to send confirmation code to phones.

Add the function `AddSmsAuthentication` to register the dependencies and update the properties of the `IdServerSmsOptions` options.

```
services.AddSIDIdentityServer()
        .AddSmsAuthentication(o => {});
```

`IdServerSmsOptions` has the following properties :

| Property           |
| ------------------ |
| AccountSid         |
| AuthToken          |
| FromPhoneNumber    |