---
slug: enroll-mobile-application
title: Enroll mobile application
authors: SimpleIdServer
tags: [fido, ctap2, u2f, mobile, enrollment]
---

## Introduction

One of the biggest challenges in developing an [Identity And Access Management](../glossary) solution like SimpleIdServer was choosing the best enrollment process for our mobile application.

Before explaining it, we must define what an [Enrollment Process](../docs/glossary) is.


## Enrollment Process

In some scenarios, an application needs the consent of the end-user to obtain their personal information, such as their bank account, or to authorize a payment.

The standard [OpenID Connect Client-Initiated Backchannel Authentication Flow](https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html) exists to fulfill this need. 

The Holder of personal information, for example, a bank, must be able to notify the end-user that a third party is trying to access their personal information.

Notifications are sent to mobile devices to obtain the consent of the end-users. Therefore, mobile devices must be known by the Holder to receive notifications.

We chose [Firebase Cloud Messaging tool](https://firebase.google.com/docs/cloud-messaging?hl=fr) to send push notifications to devices because it is the most widely used.

An Enrollment Process for the mobile application is needed to register the [Firebase Token](https://firebase.google.com/docs/reference/unity/class/firebase/messaging/firebase-messaging#gettokenasync).

Now that we understand the necessity to implement an Enrollment Process, we must choose the best approach to implement it.

There are two approaches :
* **Naive** implementation, similar to a commercial product.
* Use **standard** security protocols like FIDO and pass the Firebase Token as metadata.

## Naive implementation

### Forgerock

Generate a [QR Code](https://backstage.forgerock.com/docs/idcloud/latest/am-authentication/authn-mfa-download-app.html) with the following parameters. 

| Parameter  | Description                      |
| ---------- | -------------------------------- |
| userId     | The ID of the user               |
| a          | The authentication endpoint      |
| image      | Image to display                 |
| b          | Hex code of the background color |
| r          | Registration endpoint            |
| s          | Random shared                    |
| c          | Random challenge                 |
| l          | Load balancer cookie             |
| m          | Message ID                       |
| issuer     | Name of the issuer               |

### Ping Identity

Generate a [QR Code](https://docs.pingidentity.com/r/en-us/pingid-user-guide/pingid_setup_android_device) containing a unique pairing key code.

We did not opt for a naive approach due to security risks.

Instead, we selected the [FIDO standard Protocol](https://fidoalliance.org/) as it has been specifically designed for registering a user's device, such as a smartphone or security key.

## Standard approach

### CTAP2

[FIDO CTAP](https://fidoalliance.org/specs/fido-v2.0-ps-20190130/fido-client-to-authenticator-protocol-v2.0-ps-20190130.html) enables an external and portable authenticator, such as a hardware security key, to interoperate with a client platform, such as a computer.

The following transports are supported :

* USB Human Interface Device (USB HID)
* Near Field Communication (NFC)
* Bluetooth Smart
* Bluetooth Low Energy Technology (BLE)

While attempting to implement CTAP2 in our mobile application, we encountered constraints with Android that prevented us from completing the development.

| Protocol        | Limitation                                                                                                                                                                                                                                                       |
| --------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Bluetooth Smart | There is a strong dependency on the driver installed on the computer. If the driver `Bluetooth HID` is not installed, the computer is not discoverable by the smartphone.                                                                                        |
| BLE             | [Android actively prevents the implementation of a FIDO over BLE for non system application.](https://android.googlesource.com/platform/packages/apps/Bluetooth/+/6f7f9bbf46acaaf266537256da4d0345909ea1c4/src/com/android/bluetooth/gatt/GattService.java#3217) |

Due to the issues encountered during development, we decided to choose U2F and bypass the different protocol transports, such as USB, Bluetooth, BLE, etc

### U2F

[FIDO Universal 2nd Factor (U2F)](https://fidoalliance.org/specs/u2f-specs-master/fido-u2f-overview.html) is an open authentication standard that enhances and simplifies two-factor authentication by employing specialized USB or NFC devices.

Our mobile application utilizes public-key encryption in accordance with the [FIDO U2F Authentication standard](https://fidoalliance.org/specifications/).
 During device enrollment, the mobile application registers its public key with our FIDO U2F Endpoint and includes the Firebase token in the metadata. 
When authentication occurs, a challenge-response mechanism is employed to verify that the device possesses the corresponding key.

## SimpleIdServer's implementation

Our mobile enrollment process comprises the following steps:

1. The mobile device scans the QR code, which contains the following parameters:

| Parameter       | Description                                                                                                                         |
| --------------- | ----------------------------------------------------------------------------------------------------------------------------------- | 
| session_id      | A unique session identifier created when the enrollment process starts.                                                             |
| read_qrcode_url | The URL used by the mobile device to fetch the information necessary to generate an enrollment response.                            |
| action          | The type of action, with possible values being `register` or `authentication`.  During enrollment, the action is set to `register`. |

2. Information is retrieved from the `read_qrcode_url`, which includes the following data:

| Parameter                 | Description                                                                  |
| ------------------------- | ---------------------------------------------------------------------------- |
| session_id                | A unique session identifier created when the enrollment process begins.      |
| login                     | Login of the user                                                            |
| credential_create_options | Includes a challenge to create a new credential                              |
| end_register_url          | URL called by the mobile application to finish the enrollment                |

3. The `credential_create_options` are used to construct an attestation response.
4. The attestation response, along with the [Firebase token](https://firebase.google.com/docs/reference/unity/class/firebase/messaging/firebase-messaging#gettokenasync) and mobile information, is sent to the `end_register_url`. 
5. The private key used to generate the attestation response is stored in a database. The password required to access the database is secure and can only be accessed by the mobile application.

The mobile device is now enrolled and can receive push notifications from the [Identity Provider](../docs/glossary).

## Conclusion

U2F is the best choice for implementing your enrollment process for several reasons :

* **Enhanced Security**: U2F adheres to the FIDO Security Standard, providing a higher level of security and preventing various attacks, including Man-in-the-Middle attacks.
* **Streamlined Connectivity**: U2F bypasses the need for specific transports like USB, BLE, or Bluetooth, allowing direct utilization of the API.