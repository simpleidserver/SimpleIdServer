namespace SimpleIdServer.Jwt.Jws.Handlers
{
    public class NoneSignHandler : ISignHandler
    {
        public string AlgName => "none";

        public string Sign(string payload, JsonWebKey jsonWebKey)
        {
            return string.Empty;
        }

        public bool Verify(string payload, byte[] signature, JsonWebKey jsonWebKey)
        {
            return true;
        }
    }
}
