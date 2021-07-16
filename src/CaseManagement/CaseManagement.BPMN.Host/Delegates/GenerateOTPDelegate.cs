using CaseManagement.BPMN.Domains;
using CaseManagement.BPMN.Exceptions;
using CaseManagement.BPMN.ProcessInstance.Processors;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaseManagement.BPMN.Host.Delegates
{
    public class GenerateOTPDelegate : IDelegateHandler
    {
        public async Task<ICollection<MessageToken>> Execute(BPMNExecutionContext context, ICollection<MessageToken> incoming, DelegateConfigurationAggregate delegateConfiguration, CancellationToken cancellationToken)
        {
            var user = incoming.FirstOrDefault(i => i.Name == "user");
            if (user == null)
            {
                throw new BPMNProcessorException("user must be passed in the request");
            }

            var userId = user.GetProperty("userId");
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new BPMNProcessorException("userId is not passed in the request");
            }

            var parameter = GenerateOTPPasswordParameter.Create(delegateConfiguration);
            using (var httpClient = new HttpClient())
            {
                var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = parameter.TokenUrl,
                    ClientId = parameter.ClientId,
                    ClientSecret = parameter.ClientSecret,
                    Scope = parameter.Scope
                }, cancellationToken);
                if (tokenResponse.IsError)
                {
                    throw new BPMNProcessorException(tokenResponse.Error);
                }

                var url = parameter.UserUrl.Replace("{id}", userId);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };
                request.Headers.Add("Authorization", $"Bearer {tokenResponse.AccessToken}");
                var httpResponse = await httpClient.SendAsync(request, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();
                var content = await httpResponse.Content.ReadAsStringAsync();
                var otp = long.Parse(content);
                ICollection<MessageToken> result = new List<MessageToken>
                {
                    MessageToken.NewMessage(context.Pointer.InstanceFlowNodeId, "otp", new JObject
                    {
                        { "otpCode", otp }
                    }.ToString())
                };
                return result;
            }
        }

        private class GenerateOTPPasswordParameter
        {
            public string TokenUrl { get; set; }
            public string UserUrl { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Scope { get; set; }

            public static GenerateOTPPasswordParameter Create(DelegateConfigurationAggregate delegateConfiguration)
            {
                return new GenerateOTPPasswordParameter
                {
                    ClientId = delegateConfiguration.GetValue("clientId"),
                    ClientSecret = delegateConfiguration.GetValue("clientSecret"),
                    TokenUrl = delegateConfiguration.GetValue("tokenUrl"),
                    UserUrl = delegateConfiguration.GetValue("userUrl"),
                    Scope = delegateConfiguration.GetValue("scope")
                };
            }
        }
    }
}
