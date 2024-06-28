using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using razor.models;

namespace App.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {
        }

        public class RoleModel : IdentityRole{
            public string[] Claims { get; set; }
        }

        public List<RoleModel> roles {set; get; }

        public async Task OnGet()
        {
            var r = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            roles = new List<RoleModel>();
            foreach (var _r in r){

                var Claims = await _roleManager.GetClaimsAsync(_r);
                var claimsString = Claims.Select(c=> c.Type + "=" + c.Value);
                var rm = new RoleModel(){
                    Name = _r.Name,
                    Id = _r.Id,
                    Claims = claimsString.ToArray(),
                };
                roles.Add(rm);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
