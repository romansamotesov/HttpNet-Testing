using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace REST_Testing
{
    public sealed class MyReadHttpClient: HttpClient
    {
        private static MyReadHttpClient instance;
        private string _clientId = HttpUtility.UrlEncode("0oa157tvtugfFXEhU4x7");
        private string _clientSecret = HttpUtility.UrlEncode("X7eBCXqlFC7x-mjxG5H91IRv_Bqe1oq7ZwXNA8aq");
        FormUrlEncodedContent authorizationRequestContent = new(new Dictionary<string, string>
        {
        { "grant_type", "client_credentials" },
        { "scope", "read" }
        });

        public MyReadHttpClient() : base() { }

        public async static Task<MyReadHttpClient> getInstance()
        {
            if (instance == null)
                instance = new MyReadHttpClient();
            instance.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await instance.GetToken());
            return instance;
        }
        public async Task<string> GetToken()
        {
            HttpClient thishttpClient = new HttpClient();
            string authorizationString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            thishttpClient.BaseAddress = new("http://localhost:49000/");
            thishttpClient.DefaultRequestHeaders.Authorization = new("Basic", authorizationString);
            thishttpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
            HttpResponseMessage responseMessage = await thishttpClient.PostAsync("oauth/token", authorizationRequestContent);
            string deserializedjson = responseMessage.Content.ReadAsStringAsync().Result;
            AuthentificationResponse authentificationResponse = JsonSerializer.Deserialize<AuthentificationResponse>(deserializedjson);

            return authentificationResponse.AccessToken;
        }
    }
}
