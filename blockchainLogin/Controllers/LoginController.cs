using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using blockchainLogin.Models;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace blockchainLogin.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _logger;

        private String code;
        private String id_token;

        private String tokenURI = "https://login.microsoftonline.com/vaccinebc.onmicrosoft.com/oauth2/authorize?response_type=id_token%20code" +
                       "&client_id=33e00c77-4ae3-4466-91ad-cbf1b9bcc182&redirect_uri=https%3A%2F%2Flocalhost%3A44313/&nonce=1afe3bd7-3deb-418f-9b6a-dcb635c61e4b";
        public LoginController(IHttpClientFactory httpClientFactory, ILogger<LoginController> logger)
        {
            _clientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (Request.Query.ContainsKey("code"))
            {
                code = Request.Query["code"];
                Debug.WriteLine($@"Receive the code from MSA,code:{Request.Query["code"]}");
            }
            if (Request.Query.ContainsKey("id_token"))
            {
                id_token = Request.Query["id_token"];
                Debug.WriteLine($@"Receive the code from MSA,code:{Request.Query["id_token"]}");
                var client = _clientFactory.CreateClient();
                
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // Pass the Azure AD token to the request
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", id_token);
                var response = await client.GetAsync("https://vaccinebc-qo4b5w-api.azurewebsites.net/api/v1/applications");
                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();

                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string userName)
        {
            if (userName == null)
            {
                return View();
            }
            else
            {
                var client = _clientFactory.CreateClient();

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // Pass the Azure AD token to the request
                
                var response = await client.GetAsync(tokenURI);
                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();
                }
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
