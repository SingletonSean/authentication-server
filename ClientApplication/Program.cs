using ClientApplication.Http;
using ClientApplication.Services;
using ClientApplication.Stores;
using CoreLib.Requests;
using CoreLib.Responses;
using Refit;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string authenticationBaseUrl = "http://localhost:5000";
            string dataBaseUrl = "http://localhost:8000";

            IRefreshService refreshService = RestService.For<IRefreshService>(authenticationBaseUrl);
            TokenStore tokenStore = new TokenStore();
            AutoRefreshHttpMessageHandler autoRefreshHttpMessageHandler = new AutoRefreshHttpMessageHandler(tokenStore, refreshService);

            IRegisterService registerService = RestService.For<IRegisterService>(authenticationBaseUrl);
            ILoginService loginService = RestService.For<ILoginService>(authenticationBaseUrl);
            IDataService dataService = RestService.For<IDataService>(new HttpClient(autoRefreshHttpMessageHandler)
            {
                BaseAddress = new Uri(dataBaseUrl)
            });

            try
            {
                await registerService.Register(new RegisterRequest
                {
                    Email = "test@gmail.com",
                    Username = "SingletonSean",
                    Password = "test123",
                    ConfirmPassword = "test123"
                });
            }
            catch (ApiException ex) 
            {
                ErrorResponse errorResponse = await ex.GetContentAsAsync<ErrorResponse>();
                Console.WriteLine(errorResponse.ErrorMessages.FirstOrDefault());
            }

            AuthenticatedUserResponse loginResponse = await loginService.Login(new LoginRequest()
            {
                Username = "SingletonSean",
                Password = "test123",
            });
            tokenStore.AccessToken = loginResponse.AccessToken;
            tokenStore.AccessTokenExpirationTime = loginResponse.AccessTokenExpirationTime;
            tokenStore.RefreshToken = loginResponse.RefreshToken;
            
            DataResponse dataResponse = await dataService.GetData();

            Console.ReadKey();
        }
    }
}
