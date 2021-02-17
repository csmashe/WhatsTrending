using Newtonsoft.Json;
using RestSharp;


namespace Cryptosmasher.Extentions
{
    public static class IRestClientExtentions
    {
        public static T Get<T>(
            this IRestClient restClient,
            string url            
            )
            where T : new()
        {
            return restClient.HttpRequest<T>($"/{url}", null, Method.GET);
        }

        private static T HttpRequest<T>(
            this IRestClient restClient,
            string url,
            string requestJson,
            Method method           
            )
            where T : new()
        {
            IRestRequest request = new RestRequest(url, method);
            request.AddHeader("Accept", "application/json");

            if (requestJson != null)
            {
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", requestJson, ParameterType.RequestBody);
            }

            IRestResponse<T> response = restClient.Execute<T>(request);

            return JsonConvert.DeserializeObject<T>(response.Content);            
        }
    }

    
}
