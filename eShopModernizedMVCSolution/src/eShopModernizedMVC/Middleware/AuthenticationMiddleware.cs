using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;


namespace eShopModernizedMVC.Middleware
{
    public class AuthenticationMiddleware     {
private readonly RequestDelegate _next;
private readonly IAuthenticationService _authenticationService;

public AuthenticationMiddleware(RequestDelegate next, IAuthenticationService authenticationService)
{
    _next = next;
    _authenticationService = authenticationService;
}

public async Task Invoke(HttpContext context)
{
    var identity = new ClaimsIdentity("cookies");
    identity.AddClaim(new Claim("iat", "1234"));
    var principal = new ClaimsPrincipal(identity);
    await _authenticationService.SignInAsync(context, "Cookies", principal, new AuthenticationProperties());
    await _next(context);
}
    }
}
