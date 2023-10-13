namespace SimpleIdServer.Mobile.Extensions;

public static class DateTimeExtensions
{
    public static double ConvertToUnixTimestamp(this DateTime date)
    {
        var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        var diff = date.ToUniversalTime() - origin;
        return Math.Floor(diff.TotalSeconds);
    }
}
