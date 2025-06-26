// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;
using SimpleIdServer.IdServer.Resources;

namespace SimpleIdServer.IdServer.Pwd.Services
{
    public class PasswordValidationService : IPasswordValidationService
    {
        private readonly IConfiguration _configuration;

        public PasswordValidationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IdentityErrorDescriber Describer { get; private set; }

        public List<(string code, string errorMessage)>? Validate(string password)
        {
            var options = GetOptions();
            if (!options.EnableValidation)
            {
                return null;
            }

            var pwdOptions = new PasswordOptions
            {
                RequiredLength = options.RequiredLength,
                RequiredUniqueChars = options.RequiredUniqueChars,
                RequireNonAlphanumeric = options.RequireNonAlphanumeric,
                RequireLowercase = options.RequireLowercase,
                RequireUppercase = options.RequireUppercase,
                RequireDigit = options.RequireDigit
            };
            return Validate(password, pwdOptions);
        }

        private List<(string code, string errorMessage)>? Validate(string password, PasswordOptions options)
        {
            List<(string code, string errorMessage)>? errors = null;
            if (string.IsNullOrWhiteSpace(password) || password.Length < options.RequiredLength)
            {
                errors ??= new List<(string code, string errorMessage)>();
                errors.Add((TranslationKeyParser.Serialize(AuthFormErrorMessages.PasswordTooShort, options.RequiredLength.ToString()), string.Format(Global.PasswordTooShort, options.RequiredLength)));
            }
            if (options.RequireNonAlphanumeric && password.All(IsLetterOrDigit))
            {
                errors ??= new List<(string code, string errorMessage)>();
                errors.Add((AuthFormErrorMessages.PasswordRequiresNonAlphanumeric, Global.PasswordRequiresNonAlphanumeric));
            }
            if (options.RequireDigit && !password.Any(IsDigit))
            {
                errors ??= new List<(string code, string errorMessage)>();
                errors.Add((AuthFormErrorMessages.PasswordRequiresDigit, Global.PasswordRequiresDigit));
            }
            if (options.RequireLowercase && !password.Any(IsLower))
            {
                errors ??= new List<(string code, string errorMessage)>();
                errors.Add((AuthFormErrorMessages.PasswordRequiresLower, Global.PasswordRequiresLower));
            }
            if (options.RequireUppercase && !password.Any(IsUpper))
            {
                errors ??= new List<(string code, string errorMessage)>();
                errors.Add((AuthFormErrorMessages.PasswordRequiresUpper, Global.PasswordRequiresUpper));
            }
            if (options.RequiredUniqueChars >= 1 && password.Distinct().Count() < options.RequiredUniqueChars)
            {
                errors ??= new List<(string code, string errorMessage)>();
                errors.Add((TranslationKeyParser.Serialize(AuthFormErrorMessages.RequiredUniqueChars, options.RequiredLength.ToString()), string.Format(Global.RequiredUniqueChars, options.RequiredUniqueChars)));
            }

            return errors;
        }

        public virtual bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        public virtual bool IsLower(char c)
        {
            return c >= 'a' && c <= 'z';
        }

        public virtual bool IsUpper(char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        public virtual bool IsLetterOrDigit(char c)
        {
            return IsUpper(c) || IsLower(c) || IsDigit(c);
        }

        private IdServerPasswordOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(IdServerPasswordOptions).Name);
            return section.Get<IdServerPasswordOptions>() ?? new IdServerPasswordOptions();
        }
    }
}
