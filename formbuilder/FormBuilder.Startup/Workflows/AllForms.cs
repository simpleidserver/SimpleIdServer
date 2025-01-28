using FormBuilder.Models;

namespace FormBuilder.Startup.Workflows;

public class AllForms
{
    public static List<FormRecord> GetAllForms()
        => new List<FormRecord>
        {
            MobileAuthForms.MobileAuthForm,
            PwdAuthForms.LoginPwdAuthForm,
            PwdAuthForms.ResetPwdForm
        };
}
