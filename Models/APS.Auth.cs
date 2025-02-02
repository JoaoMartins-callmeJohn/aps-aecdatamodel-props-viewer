using System;
using System.Threading.Tasks;
using Autodesk.Authentication;
using Autodesk.Authentication.Model;

public partial class APS
{
  public string GetAuthorizationURL()
  {
    var authenticationClient = new AuthenticationClient(_sdkManager);
    return authenticationClient.Authorize(_clientId, ResponseType.Code, _callbackUri, InternalTokenScopes);
  }

  public async Task<Tokens> GenerateTokens(string code)
  {
    var authenticationClient = new AuthenticationClient(_sdkManager);
    var internalAuth = await authenticationClient.GetThreeLeggedTokenAsync(_clientId, _clientSecret, code, _callbackUri);
    var publicAuth = await authenticationClient.GetRefreshTokenAsync(_clientId, _clientSecret, internalAuth.RefreshToken, PublicTokenScopes);
    return new Tokens
    {
      PublicToken = publicAuth.AccessToken,
      InternalToken = internalAuth.AccessToken,
      RefreshToken = publicAuth._RefreshToken,
      ExpiresAt = DateTime.Now.ToUniversalTime().AddSeconds((double)internalAuth.ExpiresIn)
    };
  }

  public async Task<Tokens> RefreshTokens(Tokens tokens)
  {
    var authenticationClient = new AuthenticationClient(_sdkManager);
    var internalAuth = await authenticationClient.GetRefreshTokenAsync(_clientId, _clientSecret, tokens.RefreshToken, InternalTokenScopes);
    var publicAuth = await authenticationClient.GetRefreshTokenAsync(_clientId, _clientSecret, internalAuth._RefreshToken, PublicTokenScopes);
    return new Tokens
    {
      PublicToken = publicAuth.AccessToken,
      InternalToken = internalAuth.AccessToken,
      RefreshToken = publicAuth._RefreshToken,
      ExpiresAt = DateTime.Now.ToUniversalTime().AddSeconds((double)internalAuth.ExpiresIn)
    };
  }

  public async Task<UserInfo> GetUserProfile(Tokens tokens)
  {
    var authenticationClient = new AuthenticationClient(_sdkManager);
    UserInfo userInfo = await authenticationClient.GetUserInfoAsync(tokens.InternalToken);
    return userInfo;
  }
}