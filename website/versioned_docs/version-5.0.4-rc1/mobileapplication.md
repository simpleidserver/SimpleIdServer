# Mobile application

import MobileSettings from './images/mobile-settings.jpg';
import MobileQrCode from './images/mobile-scan-qrcode.jpg';
import ProfileQrCode from './images/profile-qrcode.png';
import MobileOtpCodes from './images/mobile-otpcodes.jpg';
import MobileCreds from './images/mobile-creds.jpg';
import MobileGotify from './images/mobile-gotify.jpg';
import MobileWallet from './images/mobile-wallet.jpg';
import PlayStore from './images/play-store.png';
import AppStore from './images/app-store.png';

SimpleIdServer offers a mobile application that is both free and open-source.

## Download

The application can be downloaded from the Play Store for Android or the App Store for iPhone.

<div style={{textAlign:"center"}}>
    <a href="#"><img src={PlayStore} style={{width: 200}} /></a>
    <a href="#" style={{paddingLeft: 10}}><img src={AppStore} style={{width: 200}} /></a>
</div>

The mobile application supports various features.

## One-Time Password

The mobile application can be used as a two-factor authentication device and supports both types of One-Time Passwords:

* **TOTP** : Time-based One-Time Password, which is valid for one use and only for a limited time.
* **HOTP** : HMAC-based One-Time Password, where the moving factor is not time-based but a counter that increments upon request.

If there is no One-Time Password configured in the user account, you can create one by following the steps below:

1. Navigate to the [administration website](https://website.simpleidserver.com/master).
2. Go to the `Users` screen.
3. Select a user and then select the `Credentials` tab.
4. Click on the `Add credential` button.
5. In the popup-window, select the `One Time Password` option, check the `Is default` checkbox, and click on `Next`.
6. Select the type of OTP, for example `TOTP`, and click on the `Save` button to confirm its creation.

Now, the OTP can be enrolled in the mobile application:

1. Navigate to the [Identity server website](https://openid.simpleidserver.com/master) and authenticate with your user account.
2. Open the mobile application.
3. Go to the `Enroll` tab and click on the `Scan QR Code` button.

<div style={{textAlign:"center"}}>
    <img src={MobileQrCode} style={{width: 300}} />
</div>

4. Scan the QR code. Once OTP enrollment is successfully completed, the message `The One-Time Password has been enrolled` will be displayed.

<div style={{textAlign:"center"}}>
    <img src={ProfileQrCode} style={{width: 300}} />
</div>

To access all the One-Time Passwords enrolled in the mobile application, click on the `One Time Password` option.

<div style={{textAlign:"center"}}>
    <img src={MobileOtpCodes} style={{width: 300}} />
</div>

The OTP is now available in the mobile application and can be used in an Authentication Context Class Reference (ACR).
 To create a new ACR with both authentication methods (password and OTP) configured, follow the steps below:

1. Navigate to the [administration website](https://website.simpleidserver.com/master).
2. Go to the `Authentication Context` screen.
3. Click on the `Add Authentication Context Reference` button.
4. Fill in the form with the following values and click on the `Add` button to confirm the creation.

| Key | Value |
| --- | ----- |
| Name | pwd-otp |
| Display name | pwd-otp |
| Select authentication methods | pwd,otp |

Click on the new ACR method and authenticate using your password and OTP.

## Authentication device

The mobile application can be used as an authentication device.

It utilizes SimpleIdServer's FIDO U2F endpoints to enroll a public key, while the private key is securely stored on the device. 
During the authentication process, SimpleIdServer sends a challenge to the device to verify the corresponding private key.

The enrollment process is important because, during this phase, information about the device, such as the `Firebase token` and `Gotify token`, is transferred to the Identity Server.
This allows the backend to send notifications to mobile applications and is a prerequisite for supporting Back Channel Authentication Device (CIBA).

Follow these steps to enroll your mobile application:

1. Go to the following URL to register a new user [https://openid.simpleidserver.com/master/registration?workflowName=mobile](https://openid.simpleidserver.com/master/registration?workflowName=mobile).
2. Enter a random username and click on the `Generate QR Code` button.
3. Open the mobile application.
4. Select the `Enroll` tab.
5. Click on the `Scan QR Code` button and scan the QR code displayed on the website.

Your mobile application is now successfully enrolled and ready to be used for user authentication.

The list of enrolled public keys is available in the `Credentials` option.

<div style={{textAlign:"center"}}>
    <img src={MobileCreds} style={{width: 300}} />
</div>

## Back Channel Authentication Device

The mobile application can be configured to use one of the following notification methods:

* **Firebase** : [Firebase Cloud Message (FCM)](https://firebase.google.com/docs/cloud-messaging) is a cross-platform messaging solution from Google.
* **Gotify** : [Gotify](https://gotify.net/) is a simple server for sending and receiving messages. Our Gotify server is hosted [here](https://gotify.simpleidserver.com/#/).

By default, `Gotify` is configured as the notification method. The default notification method can be selected on the `Profile` page.

<div style={{textAlign:"center"}}>
    <img src={MobileSettings} style={{width: 300}} />
</div>

To receive notifications on your mobile application, follow the [Client-Initiated Backchannel Authentication (CIBA) tutorial](tutorial/ciba).

When the `Gotify` notification method is used, the mobile application must be active; otherwise, the back channel notification cannot be received. 
The status of the Gotify listener can be controlled on the `Profile` page. There is a button to start or stop the listener.

<div style={{textAlign:"center"}}>
    <img src={MobileGotify} style={{width: 300}} />
</div>

## Electronic wallet

The mobile application can be used as an electronic wallet compliant with the [ESBI standard](https://hub.ebsi.eu/wallet-conformance).

It supports the following features:

* **Request Verifiable Credentials** : Receive Verifiable Credentials from one or more issuers and store them securely. For example, a diploma or a social security card.
* **Present Verifiable Presentation** : Share Verifiable Presentations with a verifier.

The electronic wallet is accessible in the `Wallet` tab.

<div style={{textAlign:"center"}}>
    <img src={MobileWallet} style={{width: 300}} />
</div>