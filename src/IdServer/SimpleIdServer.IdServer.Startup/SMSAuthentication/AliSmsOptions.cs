using SimpleIdServer.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.UI;

namespace SimpleIdServer.IdServer.Startup.SMSAuthentication;

public class AliSmsOptions : IOTPRegisterOptions
{
    [ConfigurationRecord("AccessId", null, order: 0)]
    public string AccessId { get; set; }
    [ConfigurationRecord("AccessKey", null, 1, null, CustomConfigurationRecordType.PASSWORD)]
    public string AccessKey { get; set; }
    [ConfigurationRecord("The phone number for receiving text messages", null, order: 2)]
    public string PhoneNumber { get; set; }
    [ConfigurationRecord("Content of the message", null, order: 3)]
    public string Message { get; set; } = "the confirmation code is {0}";
    [ConfigurationRecord("Template Code", null, order: 4)]
    public string TemplateCode { get; set; }
    [ConfigurationRecord("Signature Name", null, order: 5)]
    public string SignatureName { get; set; }
    [ConfigurationRecord("OTP Algorithm", null, order: 6)]
    public OTPTypes OTPType { get; set; } = OTPTypes.TOTP;
    [ConfigurationRecord("OTP Value", null, 7, null, CustomConfigurationRecordType.OTPVALUE)]
    public string OTPValue { get; set; } = null;
    [ConfigurationRecord("OTP Counter", null, 8, "OTPType=HOTP")]
    public int OTPCounter { get; set; } = 10;
    [ConfigurationRecord("TOTP Step", null, 9, "OTPType=TOTP")]
    public int TOTPStep { get; set; } = 30;
    [ConfigurationRecord("HOTP Window", null, 10, "OTPType=HOTP")]
    public int HOTPWindow { get; set; } = 5;
    public OTPAlgs OTPAlg => (OTPAlgs)OTPType;
    public string HttpBody => Message;
}

public enum OTPTypes
{
    [ConfigurationRecordEnum("HOTP")]
    HOTP = 0,
    [ConfigurationRecordEnum("TOTP")]
    TOTP = 1
}
