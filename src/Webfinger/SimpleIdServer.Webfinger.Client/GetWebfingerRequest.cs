using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleIdServer.Webfinger.Client;

public class GetWebfingerRequest
{
    public string Resource { get; set; }
    public List<string> Rel { get; set; } = new List<string>();

    public string ToQueryParameters()
    {
        var parameters = new Dictionary<string, string>
        {
            { "resource", HttpUtility.UrlEncode(Resource) }
        };
        if(Rel != null && Rel.Any())
        {
            foreach(var r  in Rel)
            {
                parameters.Add("rel", HttpUtility.UrlEncode(r));
            }
        }

        return string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
}
