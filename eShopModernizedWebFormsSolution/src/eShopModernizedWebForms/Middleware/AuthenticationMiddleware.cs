using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;


namespace eShopModernizedWebForms.Middleware
{
    public class AuthenticationMiddleware     {
RequestDelegate _next = null;        public AuthenticationMiddleware(RequestDelegate next)
        {
_next = next;        }
public async Task Invoke(HttpContext context)
        {
            var identity = new ClaimsIdentity("cookies");
            identity.AddClaim(new Claim("iat", "1234"));
            var principal = new ClaimsPrincipal(identity);
            await context.SignInAsync(principal);
            await _next(context);
        }
    }
}
