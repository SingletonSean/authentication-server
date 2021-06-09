using ClientApplication.Services;
using ClientApplication.Stores;
using CoreLib.Requests;
using CoreLib.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientApplication.Http
{
    public class AutoRefreshHttpMessageHandler : DelegatingHandler
    {
        private readonly TokenStore _tokenStore;
        private readonly IRefreshService _refreshService;

        public AutoRefreshHttpMessageHandler(TokenStore tokenStore, IRefreshService refreshService) : base(new HttpClientHandler())
        {
            _tokenStore = tokenStore;
            _refreshService = refreshService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (IsAccessTokenExpired())
            {
                await RefreshAccessToken();
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStore.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private bool IsAccessTokenExpired()
        {
            return _tokenStore.AccessTokenExpirationTime <= DateTime.UtcNow;
        }

        private async Task RefreshAccessToken()
        {
            AuthenticatedUserResponse authenticatedUserResponse = await _refreshService.Refresh(new RefreshRequest
            {
                RefreshToken = _tokenStore.RefreshToken
            });

            _tokenStore.AccessToken = authenticatedUserResponse.AccessToken;
            _tokenStore.AccessTokenExpirationTime = authenticatedUserResponse.AccessTokenExpirationTime;
            _tokenStore.RefreshToken = authenticatedUserResponse.RefreshToken;
        }
    }
}
