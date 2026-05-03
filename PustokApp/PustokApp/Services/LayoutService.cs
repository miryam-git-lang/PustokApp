using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Models;

namespace PustokApp.Services;

public class LayoutService(
    IHttpContextAccessor httpContextAccessor,
    UserManager<AppUser> userManager,AppDbContext context
)
{
    public async Task<AppUser> GetUserInfo()
    {
        var context = httpContextAccessor.HttpContext;
        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            var userName = httpContextAccessor.HttpContext.User.Identity.Name;
            var user = await userManager.FindByNameAsync(userName);
            return user;
        }
        return null;
    }

    public Dictionary<string,string> GetSettings()
    {
        return context.Settings.ToDictionary(x => x.Key, x => x.Value);
    }
}