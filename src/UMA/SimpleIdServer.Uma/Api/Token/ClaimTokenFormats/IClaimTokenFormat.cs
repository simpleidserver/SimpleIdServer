using SimpleIdServer.OAuth.Api;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api.Token.Fetchers
{
    public interface IClaimTokenFormat
    {
        string Name { get; }
        Task<ClaimTokenFormatFetcherResult> Fetch(HandlerContext context);
        string GetSubjectName();
    }
}
