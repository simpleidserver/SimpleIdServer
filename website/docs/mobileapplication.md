# Mobile application

SimpleIdServer offers a mobile application that is both free and open-source.

## Download

The application can be downloaded for Android [here](https://install.appcenter.ms/users/agentsimpleidserver-gmail.com/apps/SimpleIdServer).

## Enroll

The mobile application utilizes SimpleIdServer's FIDO U2F endpoints to enroll a public key, while the private key is securely stored on the device.

During the authentication process, SimpleIdServer sends a challenge response to the device to verify the corresponding private key. 

Follow these steps to enroll your mobile application:

1. Go to the following URL to register a new user [https://openid.simpleidserver.com/master/mobile/Register](https://openid.simpleidserver.com/master/mobile/Register).
2. Enter a random username and click on the `Generate QR Code` button.

If you are using the [mobile application](https://install.appcenter.ms/users/agentsimpleidserver-gmail.com/apps/SimpleIdServer), follow these steps:

1. Open the mobile application.
2. Select the `Enroll` tab.
3. Click on the `Scan QR Code` button and scan the QR code displayed on the website.

If you are using the [demo application](https://appetize.io/app/pndcdf5lgtouyv72pdegfij2ri?device=pixel4&osVersion=11.0&scale=75), follow these steps:

1. Open the demo application.
2. Select the `Settings` tab.
3. Enable `Developer mode`.
4. Choose the `Enroll` tab, click on `Enter QR Code`, then copy and paste the QR Code JSON into the field and click on `Save`.

Your mobile application is now successfully enrolled and ready to be used for user authentication.