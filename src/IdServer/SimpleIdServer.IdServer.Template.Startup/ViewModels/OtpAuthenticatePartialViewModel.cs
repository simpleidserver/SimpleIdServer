using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Template.Startup.ViewModels;

public sealed class OtpAuthenticatePartialViewModel
{
    public required SidWorkflowViewModel Workflow { get; init; }
    public required BaseOTPAuthenticateViewModel Input { get; init; }

    public required string Area { get; init; }
    public required string AuthFormEltId { get; init; }
    public required string SendCodeEltId { get; init; }

    public required string Title { get; init; }
    public required string LoginLabel { get; init; }
    public required string LoginPlaceholder { get; init; }
    public required string LoginInputType { get; init; }

    public required string ConfirmationCodeLabel { get; init; }
    public required string ConfirmationCodePlaceholder { get; init; }

    public required string RememberLoginLabel { get; init; }
    public required string AuthenticateButtonLabel { get; init; }
    public required string SendCodeButtonLabel { get; init; }

    public string? CodeValidityFormat { get; init; }
    public string? TosLabel { get; init; }
    public string? PolicyLabel { get; init; }
}
