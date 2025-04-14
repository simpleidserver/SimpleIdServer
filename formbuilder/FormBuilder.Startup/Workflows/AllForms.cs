using FormBuilder.Builders;
using FormBuilder.Models;

namespace FormBuilder.Startup.Workflows;

public class AllForms
{
    public static List<FormRecord> GetAllForms()
        => new List<FormRecord>
        {
            PwdAuthForms.LoginPwdAuthForm,
            FormRecordBuilder.New("email", "email", Constants.DefaultRealm, true).Build(),
            FormRecordBuilder.New("sms", "sms", Constants.DefaultRealm, true).Build()
        };
}
