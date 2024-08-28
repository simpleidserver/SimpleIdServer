# Mobile application

import MobileSettings from './images/mobile-settings.jpg';
import MobileQrCode from './images/mobile-scan-qrcode.jpg';
import ProfileQrCode from './images/profile-qrcode.jpg';
import MobileOtpCodes from './images/mobile-otpcodes.jpg';
import MobileCreds from './images/mobile-creds.jpg';
import MobifleGotify from './images/mobile-gotify.jpg';

SimpleIdServer offers a mobile application that is both free and open-source.

## Download

The application can be downloaded from the Play store (Android) or from the App store (Iphone).

The mobile application supports differents features.

## One time password

The mobile application can be used as a two factor authentication device, it supports both types of One Time Password :

* TOTP : Time-based one time password, a password that works once and only for a limited time.
* HOTP : HMAC-based one-time password, meaning that the moving factor is not the ticking hands of a clock but a counter that moves upon request.

If there is no One-Time-Password configured in the user account. It can be created by following the steps below :

1. Navigate to the [administration website](https://website.simpleidserver.com/master).
2. Go to the `Users` screen.
3. Select one user and select the `Credentials` tab.
4. Click on the `Add credential` button.
5. In the popup-window, select the `One Time Password` option , check the `Is default` checkbox and click on `Next`.
6. Select the type of OTP, for example `TOTP` and click on the `Save` button to confirm the creation.

Now, the OTP can be enrolled by the mobile application :

1. Navigate to the [Identity server website](https://openid.simpleidserver.com/master) and authenticate with your user account.
2. Open the mobile application.
3. Navigate to the `Enroll tab` and click on the `Scan QR Code` button.

<div style={{textAlign:"center"}}>
    <img src={MobileQrCode} style={{width: 300}} />
</div>

4. Scan the QR code. When the OTP enrollement is successfully finished then the message `The One Time Password has been enrolled` will be displayed.

<div style={{textAlign:"center"}}>
    <img src={ProfileQrCode} style={{width: 300}} />
</div>

To access to all the One Time Passwords enrolled by the mobile application, click on the `One Time Password` option.

<div style={{textAlign:"center"}}>
    <img src={MobileOtpCodes} style={{width: 300}} />
</div>

Now, the OTP is available in the mobile application. It can be used in an Authentication Context Class Reference (ACR).
To create a new ACR with the two authentication methods (pwd and otp) configured, follow the steps below :

1. Navigate to the [administration website](https://website.simpleidserver.com/master).
2. Go to the `Authentication Context` screen.
3. Click on the `Add Authentication Context Reference` button.
4. Fill-in the form with the following values and click on the `Add` button to confirm the creation.

| Key | Value |
| --- | ----- |
| Name | pwd-otp |
| Display name | pwd-otp |
| Select authentication methods | pwd,otp |

Click on the new ACR method and authenticate with your password and OTP.

## Authentication device

The mobile application can be used as an authentication device. 

It utilizes SimpleIdServer's FIDO U2F endpoints to enroll a public key, while the private key is securely stored on the device. 
During the authentication process, SimpleIdServer sends a challenge response to the device to verify the corresponding private key. 

The enrollement process is important, because during this phase the informations about the device such as `Firebase token`, `Gotify token` are transfered to the Identity Server. 
It allows the backend to send notifications to mobile applications, it is a pre-requisite to support Back Channel Authentication Device (CIBA).

Follow these steps to enroll your mobile application:

1. Go to the following URL to register a new user [https://openid.simpleidserver.com/master/registration?workflowName=mobile](https://openid.simpleidserver.com/master/registration?workflowName=mobile).
2. Enter a random username and click on the `Generate QR Code` button.
3. Open the mobile application.
4. Select the `Enroll` tab.
5. Click on the `Scan QR Code` button and scan the QR code displayed on the website.

Your mobile application is now successfully enrolled and ready to be used for user authentication.

The list of enrolled public keys are available in the `Credentials` option.

<div style={{textAlign:"center"}}>
    <img src={MobileCreds} style={{width: 300}} />
</div>

## Back Channel Authentication Device

The mobile application can be configured to use one of the following notification methods:

* **Firebase** : [Firebase Cloud Message (FCM)](https://firebase.google.com/docs/cloud-messaging) is a cross-platform messaging solution from Google.
* **Gotify** : [Gotify](https://gotify.net/) is a simple server for sending and receiving messages. Our Gotify server is hosted [here](https://gotify.simpleidserver.com/#/).

By default `Gotify` is used configured, the default notification method can be selected in the `Profile` page.

<div style={{textAlign:"center"}}>
    <img src={MobileSettings} style={{width: 300}} />
</div>

To receive a notification in your mobile application, you can follow the [Client-Initiated Backchannel Authentication (CIBA) tutorial](tutorial/ciba).

When the `Gotify` notification method is used, the mobile application must be active; otherwise the back channel notification cannot be received.
The status of the gotify listener can be controlled in the `Profile` page. There is a button to start or stop the listener.

<div style={{textAlign:"center"}}>
    <img src={MobileGotify} style={{width: 300}} />
</div>


## Electronical wallet

The mobile application can be used as an electronical wallet. It is able to request and store Verifiable Credentials from trusted issuer, and present them to trusted verifiers.
The wallet is conformed with the ESBI standard.

COMPRENDRE COMMENT FONCTIONNE CREDENTIAL OFFER.