namespace FormBuilder.Helpers
{
    public class TranslationKeyParser
    {
        public static string Serialize(string code, params string[] args)
        {
            var lst = new List<string> { code };
            if(args != null)
            {
                lst.AddRange(args);
            }

            return string.Join(".", lst);
        }

        public static (string Code, string[] Args) Parse(string translationKey)
        {
            var parts = translationKey.Split('.');
            var code = parts[0];
            if (parts.Length == 1)
            {
                return (code, Array.Empty<string>());
            }

            var args = parts.Skip(1).ToArray();
            return (code, args);
        }
    }
}
