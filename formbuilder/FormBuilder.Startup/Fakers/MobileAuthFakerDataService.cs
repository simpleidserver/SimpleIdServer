using FormBuilder.Startup.Workflows;

namespace FormBuilder.Startup.Fakers;

public class MobileAuthFakerDataService : IFakerDataService
{
    public string FormRecordName => MobileAuthForms.MobileAuthForm.Name;

    public object Generate()
    {
        return new object();
    }
}
