using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.DTOs;

namespace SimpleIdServer.IdServer.VerifiablePresentation.DTOs;

public class IdTokenResponse
{
    [FromQuery(Name = AuthorizationResponseParameters.IdToken)]
    public string IdToken { get; set; }
}
