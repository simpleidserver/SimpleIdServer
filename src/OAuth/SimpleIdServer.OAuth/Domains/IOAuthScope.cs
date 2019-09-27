using System;

namespace SimpleIdServer.OAuth.Domains
{
    public interface IOAuthScope
    {
        string Name { get; set; }
        bool IsExposedInConfigurationEdp { get; set; }
        DateTime CreateDateTime { get; set; }
        DateTime UpdateDateTime { get; set; }
    }
}