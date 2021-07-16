using CaseManagement.BPMN.Domains;
using CaseManagement.BPMN.Exceptions;
using CaseManagement.BPMN.ProcessInstance.Processors;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaseManagement.BPMN.Host.Delegates
{
    public class UpdateUserPasswordDelegate : IDelegateHandler
    {
        private const string ActivityName = "Activity_0fhwdxz";

        public async Task<ICollection<MessageToken>> Execute(BPMNExecutionContext context, ICollection<MessageToken> incoming, DelegateConfigurationAggregate delegateConfiguration, CancellationToken cancellationToken)
        {
            var user = incoming.FirstOrDefault(i => i.Name == "user");
            if (user == null)
            {
                throw new BPMNProcessorException("userMessage must be passed in the request");
            }

            var userId = user.GetProperty("userId");
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new BPMNProcessorException("userId is not passed in the request");
            }

            var messageToken = incoming.FirstOrDefault(m => m.Name == ActivityName);
            if (messageToken == null)
            {
                throw new BPMNProcessorException($"incoming token '{ActivityName}' doesn't exist");
            }

            var password = messageToken.GetProperty("pwd");
            var parameter = UpdateUserPasswordParameter.Create(delegateConfiguration);
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

                var obj = new JObject
                {
                    { "password", password }
                };
                var content = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");
                var url = parameter.UserUrl.Replace("{id}", userId);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    Content = content,
                    RequestUri = new Uri(url)
                };
                request.Headers.Add("Authorization", $"Bearer {tokenResponse.AccessToken}");
                var httpResponse = await httpClient.SendAsync(request, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();
            }

            ICollection<MessageToken> result = new List<MessageToken>
            {
                MessageToken.EmptyMessage(context.Pointer.InstanceFlowNodeId, "updatePassword")
            };
            return result;
        }

        private class UpdateUserPasswordParameter
        {
            public string TokenUrl { get; set; }
            public string UserUrl { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Scope { get; set; } 

            public static UpdateUserPasswordParameter Create(DelegateConfigurationAggregate delegateConfiguration)
            {
                return new UpdateUserPasswordParameter
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
