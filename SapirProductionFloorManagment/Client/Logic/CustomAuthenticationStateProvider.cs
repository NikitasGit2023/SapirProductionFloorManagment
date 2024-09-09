using SapirProductionFloorManagment.Shared.Authentication___Autherization;
using Microsoft.AspNetCore.Components.Authorization;
using SapirProductionFloorManagment.Shared;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Blazored.SessionStorage;
using SapirProductionFloorManagment.Client.Logic;




public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorageService;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(HttpClient httpClient, ISessionStorageService sessionStorageService)
    {
         _httpClient = httpClient;
        _sessionStorageService = sessionStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
       
        try
        {
            var userSession = _sessionStorageService.ReadEncryptedItemAsync("UserSession");


            if (userSession == null)
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }

            else
            {

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.Result.UserName),
                    new Claim(ClaimTypes.Role, userSession.Result.Role),

                }, "JwtAuth"));

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));


            }
        }
        catch(Exception ex) 
        {
            // Token is invalid or error occurred
            Console.WriteLine(ex.Message);  
            return new AuthenticationState(new ClaimsPrincipal(_anonymous));
        }
    }
    

    public async Task UpdateAuthenticationState(UserSession userSession)
    {
        ClaimsPrincipal claimsPrincipal;

        if(userSession != null)
        {
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Role, userSession.Role),

                }));
            userSession.ExpityTimeStamp = DateTime.Now.AddSeconds(userSession.ExpiresIn);
            await _sessionStorageService.SaveEncryptedItemAsync("UserSession", userSession);

        }
        else
        {
            claimsPrincipal = _anonymous;
            _sessionStorageService.RemoveItemAsync("UserSession");
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));  

    }


    public async Task<string> GetToken()
    {
        var result = string.Empty;
        try
        {
            var userSession = await _sessionStorageService.ReadEncryptedItemAsync("UserSession");
            if (userSession != null && DateTime.Now < userSession.ExpityTimeStamp)
                result = userSession.Token;
        }
        catch (Exception ex)
        {
            //TODO
        }
        return result;

    }
}