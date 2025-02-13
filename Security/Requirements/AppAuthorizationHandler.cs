using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using razor.models;
using razor_web.models;

namespace App.Security.Requirements{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandler> _logger;
        private readonly  UserManager<AppUser> _userManager;
        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger, UserManager<AppUser> userManager){
            _logger = logger;
            _userManager = userManager;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();
            _logger.LogInformation("context.Resource ~ " + context.Resource?.GetType().Name);
            foreach (var requirement in requirements){
                if(requirement is GenZRequirement){
                    if(IsGenZ(context.User, (GenZRequirement)requirement)){
                        context.Succeed(requirement);
                    }
                }
                if (requirement is ArticleUpdateRequirement)
                {
                    bool canupdate = CanUpdateArticle(context.User, context.Resource, (ArticleUpdateRequirement)requirement);
                    if (canupdate) context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }

        private bool IsGenZ(ClaimsPrincipal user, GenZRequirement requirement)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser = appUserTask.Result;
            
            if(appUser.BirthDate == null) {
                _logger.LogInformation($"{appUser.UserName} không có ngày sinh, không thỏa mãn GenZRequirement");
                return false;
            }

            int year = appUser.BirthDate.Value.Year;
            
            var success = (year >= requirement.FromYear && year <= requirement.ToYear);
            if(success) {
                _logger.LogInformation($"{appUser.UserName} thỏa mãn GenZRequirement");
            }else{
                _logger.LogInformation($"{appUser.UserName} không thỏa mãn GenZRequirement");
            }
            return success;
        }

        private bool CanUpdateArticle(ClaimsPrincipal user, object resource, ArticleUpdateRequirement requirement)
    {
        if (user.IsInRole("Admin")) 
        {
            _logger.LogInformation("Admin cap nhat ...");
            return true;
        }
        var article = resource as Article;
        var datecreated = article.Created;
        var dateCanUpdate = new DateTime(requirement.Year, requirement.Month, requirement.Date);
        if  (datecreated < dateCanUpdate)
        {
            _logger.LogInformation("Qua ngay cap nhat");
            return false;
        }

        
        return true;
    }
    }
}
