namespace FormBuilder;

public class AntiforgeryTokenRecord
{
    public string CookieName { get; set; }
    public string CookieValue { get; set; }
    public string FormField { get; set; }
    public string FormValue { get; set; }

    public AntiforgeryTokenRecord Clone()
    {
        return new AntiforgeryTokenRecord
        {
            CookieName = CookieName,
            CookieValue = CookieValue,
            FormField = FormField,
            FormValue = FormValue
        };
    }
}
