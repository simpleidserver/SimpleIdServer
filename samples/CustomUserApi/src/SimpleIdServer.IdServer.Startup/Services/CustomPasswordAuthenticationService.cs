// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Pwd.Services;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Services;

public class CustomPasswordAuthenticationService : GenericAuthenticationService<AuthenticatePasswordViewModel>, IPasswordAuthenticationService
{
    private readonly UserApiOptions _options;

    public CustomPasswordAuthenticationService(
        IAuthenticationHelper authenticationHelper, 
        IUserRepository userRepository, 
        IOptions<UserApiOptions> options) : base(authenticationHelper, userRepository)
    {
        _options = options.Value;
    }

    public override string Amr => "pwd";

    protected override async Task<User> GetUser(string authenticatedUserId, AuthenticatePasswordViewModel viewModel, string realm, CancellationToken cancellationToken)
    {
        var user = await FetchAuthenticatedUser(realm, authenticatedUserId, cancellationToken);
        if(user == null)
        {
            user = await AuthenticateUser(viewModel.Login, realm, cancellationToken);
        }

        return user;
    }

    protected override async Task<CredentialsValidationResult> Validate(string realm, string authenticatedUserId, AuthenticatePasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        var authenticatedUser = await GetUser(authenticatedUserId, viewModel, realm, cancellationToken);
        return await Validate(realm, authenticatedUser, viewModel, cancellationToken);
    }

    protected override async Task<CredentialsValidationResult> Validate(string realm, User authenticatedUser, AuthenticatePasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        using (var httpClient = new HttpClient())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new System.Uri($"{_options.BaseUrl}/users/authenticate"),
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    login = viewModel.Login,
                    password = viewModel.Password
                }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync();
            var authResult = JsonSerializer.Deserialize<AuthenticationResult>(json);
            if(authenticatedUser == null)
            {
                authenticatedUser = new User
                {
                    Id = authResult.UserId,
                    Name = viewModel.Login
                };
                authenticatedUser.Realms.Add(new RealmUser
                {
                    RealmsName = realm
                });
                UserRepository.Add(authenticatedUser);
                await UserRepository.SaveChanges(cancellationToken);
            }
        }

        return CredentialsValidationResult.Ok(authenticatedUser);
    }

    private record AuthenticationResult
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }
}
