using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using Saga.Interfaces;
using Saga.Workers.Base;
using Saga.Workers.Interfaces;
using Saga.Workers.WorkLog;

namespace Saga.Processes.Base
{
    public abstract class ProcessBase
    {
        public abstract Task<WorkLog> DoWorkAsync(IList<WorkLog> completedWorkLogs, WorkItemBase item = null);

        public abstract Task<bool> CompensateAsync(WorkLog item, IRoutingSlip routingSlip);

        public abstract Uri WorkItemEndpointAddress { get; }

        public abstract Uri CompensationEndpointAddress { get; }

        public TWorkResult GetWorkLog<TWorkResult>(IList<WorkLog> workLogs) where TWorkResult : IWorkResult
        {
            return (TWorkResult)workLogs.First(f => f.Result is TWorkResult).Result;
        }

        public ILogger Logger;

        protected async Task<TResponse> PostWorkItemEndpointAddress<TResponse>(string data = "")
        {
            return await PostEndpoint<TResponse>(WorkItemEndpointAddress, data);
        }

        protected async Task<TResponse> PostCompensationEndpointAddress<TResponse>(string data = "")
        {
            return await PostEndpoint<TResponse>(CompensationEndpointAddress, data);
        }

        private async Task<TResponse> PostEndpoint<TResponse>(Uri endpoint, string data)
        {
            IHttpClientFactory clientFactory = new HttpClientFactory();

            HttpClient httpClient = clientFactory.Create();

            httpClient.BaseAddress = endpoint;
            HttpResponseMessage response = await httpClient.PostAsync(string.Empty, new StringContent(data, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<TResponse>(response.Content.ReadAsStringAsync().Result);

            throw new Exception(this.GetType().Name);
        }
    }

    // TODO -- Depois injetar no Bootstrap do projeto

    public interface IHttpClientFactory
    {
        HttpClient Create();
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Create()
        {
            HttpClient client = new HttpClient();
            SetupClientDefaults(client);
            return client;
        }

        protected virtual void SetupClientDefaults(HttpClient client)
        {
            client.Timeout = TimeSpan.FromMinutes(30);            
        }
    }
}