namespace FormBuilder.Extensions;

public static class UriExtensions
{
    public static Uri GetBaseUri(this Uri uri)
        => new Uri($"{uri.Scheme}://{uri.Authority}");
}
