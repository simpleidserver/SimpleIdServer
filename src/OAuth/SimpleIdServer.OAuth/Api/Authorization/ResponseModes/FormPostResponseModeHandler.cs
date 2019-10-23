using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using SimpleIdServer.OAuth.Extensions;

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseModes
{
    /// <summary>
    /// Implementation https://openid.net/specs/oauth-v2-form-post-response-mode-1_0.html
    /// </summary>
    public class FormPostResponseModeHandler : IOAuthResponseModeHandler
    {
        public const string NAME = "form_post";
        public string ResponseMode => NAME;

        public void Handle(RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext)
        {
            var queryBuilder = new QueryBuilder(authorizationResponse.QueryParameters);
            queryBuilder.Add("redirect_url", authorizationResponse.RedirectUrl);
            var issuer = httpContext.Request.GetAbsoluteUriWithVirtualPath();
            var redirectUrl = $"{issuer}/{Constants.EndPoints.Form}{queryBuilder.ToQueryString().ToString()}";
            httpContext.Response.Redirect(redirectUrl);            
        }
    }
}