using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace A_Little_Source_Of_Hope.Data
{
    public class SessionHandler
    {
        public async Task GetSession(HttpContext context, SignInManager<AppUser> _signInManager, ILogger _logger)
        {
            var session = context.Session.GetString("AnnouncementOnce");
            if (String.IsNullOrEmpty(session))
            {
                context.Session.Remove("AnnouncementOnce");
                context.Session.Clear();
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out due to session end.");
            }
        }
        public async Task SignUserOut(SignInManager<AppUser> _signInManager, ILogger _logger)
        {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out due to session end.");
        }

    }
}
