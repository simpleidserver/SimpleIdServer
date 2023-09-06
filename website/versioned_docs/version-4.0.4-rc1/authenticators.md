# Authenticators

:::info

Credentials enrollment will be supported in a future release. While awaiting the release, those authentication mechanisms can be utilized by employing a custom `Authentication Context Class Reference`.

:::

SimpleIdServer supports multiple authentication mechanisms.

| Code     | Name               | Description                                           |
| -------- | ------------------ | ----------------------------------------------------- |
| pwd      | Password           | Login & Password                                      |
| sms      | SMS                | Send an OTP code via SMS                              |
| email    | Email              | Send an OTP code via email                            |
| webauthn | WebAuthn           | Utilize a FIDO-compliant device from your web browser |
| mobile   | Mobile application | Scan the QR code with the mobile application          |