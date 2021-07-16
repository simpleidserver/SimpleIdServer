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
    public class AssignHumanTaskInstanceDelegate : IDelegateHandler
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

            var humanTaskInstanceId = incoming.First(i => i.Name == "humanTaskCreated").JObjMessageContent.SelectToken("humanTaskInstance.id").ToString();
            using (var httpClient = new HttpClient())
            {

                var parameter = AssignHumanTaskInstanceParameter.Create(delegateConfiguration);
                var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = parameter.TokenUrl,
                    ClientId = parameter.ClientId,
                    ClientSecret = parameter.ClientSecret,
                    Scope = "manage_humantaskinstance"
                }, cancellationToken);
                await Claim(httpClient, parameter, humanTaskInstanceId, userId, tokenResponse.AccessToken, cancellationToken);
                await Start(httpClient, parameter, humanTaskInstanceId, userId, tokenResponse.AccessToken, cancellationToken);
            }

            ICollection<MessageToken> result = new List<MessageToken>
            {
                MessageToken.EmptyMessage(context.Pointer.InstanceFlowNodeId, "assignHumanTaskInstance")
            };
            return result;
        }

        private static async Task Claim(HttpClient httpClient, AssignHumanTaskInstanceParameter parameter, string humanTaskInstanceId, string userId, string accessToken, CancellationToken cancellationToken)
        {
            var obj = new JObject
            {
                { "userId", userId }
            };
            var content = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");
            var url = parameter.HumanTaskInstanceClaimUrl.Replace("{id}", humanTaskInstanceId);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = new Uri(url)
            };
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpResponse = await httpClient.SendAsync(request, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();
        }

        private static async Task Start(HttpClient httpClient, AssignHumanTaskInstanceParameter parameter, string humanTaskInstanceId, string userId, string accessToken, CancellationToken cancellationToken)
        {
            var obj = new JObject
            {
                { "userId", userId }
            };
            var content = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");
            var url = parameter.HumanTaskInstanceStartUrl.Replace("{id}", humanTaskInstanceId);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = new Uri(url)
            };
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpResponse = await httpClient.SendAsync(request, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();
        }

        private class AssignHumanTaskInstanceParameter
        {
            public string TokenUrl { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string HumanTaskInstanceClaimUrl { get; set; }
            public string HumanTaskInstanceStartUrl { get; set; }

            public static AssignHumanTaskInstanceParameter Create(DelegateConfigurationAggregate delegateConfiguration)
            {
                return new AssignHumanTaskInstanceParameter
                {
                    ClientId = delegateConfiguration.GetValue("clientId"),
                    ClientSecret = delegateConfiguration.GetValue("clientSecret"),
                    TokenUrl = delegateConfiguration.GetValue("tokenUrl"),
                    HumanTaskInstanceClaimUrl = delegateConfiguration.GetValue("humanTaskInstanceClaimUrl"),
                    HumanTaskInstanceStartUrl = delegateConfiguration.GetValue("humanTaskInstanceStartUrl")
                };
            }
        }
    }
}
