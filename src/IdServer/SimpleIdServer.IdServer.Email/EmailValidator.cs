// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Globalization;
using System.Text.RegularExpressions;

namespace SimpleIdServer.IdServer.Email;

public class EmailValidator
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                  RegexOptions.None, TimeSpan.FromMilliseconds(200));
            string DomainMapper(Match match)
            {
                var idn = new IdnMapping();
                string domainName = idn.GetAscii(match.Groups[2].Value);
                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException e)
        {
            return false;
        }
        catch (ArgumentException e)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}
