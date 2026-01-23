using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Template.Startup.ViewModels;

public sealed class OtpRegisterPartialViewModel
{
    public required SidWorkflowViewModel Workflow { get; init; }
    public required OTPRegisterViewModel Input { get; init; }

    public required string Area { get; init; }
    public required string SendConfirmationCodeFormId { get; init; }
    public required string RegisterFormId { get; init; }

    public required string Title { get; init; }
    public required string ValueLabel { get; init; }
    public required string ValuePlaceholder { get; init; }
    public required string ValueInputType { get; init; }

    public required string ConfirmationCodeLabel { get; init; }
    public required string ConfirmationCodePlaceholder { get; init; }

    public required string RegisterButtonLabel { get; init; }
    public required string SendCodeButtonLabel { get; init; }
}
