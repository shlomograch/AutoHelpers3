using System.Collections.Generic;
using AutomationHelpers3.TestRun.Models;
using Newtonsoft.Json;
using RestSharp;

namespace AutomationHelpers3.Helpers
{
    public class Api
    {
        public string Host { get; set; }
        public string Request { get; set; }

        public Api(string request, string host = null)
        {
            Host = host ?? "http://api.autodash.nelnet.net/";
            Request = request;
        }

        public RestResponse GetApi()
        {
            var client = new RestClient(Host);
            var request = new RestRequest(Request, Method.GET);

            return client.Execute(request) as RestResponse;
        }

        public RestResponse PostApi(string objToPost, List<HttpHeader> headers = null)
        {
            var client = new RestClient(Host);
            var request = new RestRequest(Request, Method.POST);
            request.AddJsonBody(objToPost);

            if (headers == null || headers.Count <= 0)
                return client.Execute(request) as RestResponse;

            foreach (var header in headers)
                request.AddHeader(header.Name, header.Value);

            return client.Execute(request) as RestResponse;
        }

        public RestResponse UpdateTestRun(TestRunModel testRun)
        {
            return PostApi(JsonConvert.SerializeObject(testRun));
        }
    }
}
