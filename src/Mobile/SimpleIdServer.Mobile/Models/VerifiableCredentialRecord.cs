using SQLite;

namespace SimpleIdServer.Mobile.Models;

public class VerifiableCredentialRecord
{
    [PrimaryKey]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Format { get; set; }
    public string BackgroundColor { get; set; }
    public string TextColor { get; set; }
    public string Logo { get; set; }
    public string Description { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string SerializedVc { get; set; }
}
