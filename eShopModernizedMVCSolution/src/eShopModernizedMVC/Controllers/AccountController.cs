using log4net;
using System.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace eShopModernizedMVC.Controllers
{
    public class AccountController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task SignIn()
        {
            _log.Info($"Now processing... AccountController.SignIn");
            // Send an OpenID Connect sign-in request.
            if (!User.Identity.IsAuthenticated)
            {
                /* Added by CTA: Please use your AuthenticationProperties object with this ChallengeAsync api if needed. You can also include a scheme. */
await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
;            }
        }
        public async Task SignOut()
        {
            _log.Info($"Now processing... AccountController.SignOut");
            // Send an OpenID Connect sign-out request.
            /* Added by CTA: You can only pass in a single scheme as a parameter and you can also include an AuthenticationProperties object as well. */
await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
;        }

        public async Task EndSession()
        {
            _log.Info($"Now processing... AccountController.EndSession");
            // If AAD sends a single sign-out message to the app, end the user's session, but don't redirect to AAD for sign out.
            /* Added by CTA: You can only pass in a single scheme as a parameter and you can also include an AuthenticationProperties object as well. */
await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
;        }
    }
}
