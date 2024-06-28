// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using razor.models;
using razor_web.models;

namespace App.Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyBlogContext _myBlogContext;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            MyBlogContext myBlogContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _myBlogContext = myBlogContext;
        }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
      
        public AppUser user { get; set; }

        [BindProperty]
        [DisplayName("Các role gán cho user")]
        public string[] RoleNames { get; set; }

        public SelectList allRoles { get; set; }

        public List<IdentityRoleClaim<string>> claimInRole { get; set; }
        public List<IdentityUserClaim<string>> claimInUserClaim { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if(string.IsNullOrEmpty(id)){
                return NotFound($"Không có user");
            }
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy user, id = {id}.");
            }

            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            List<string> roleName = await _roleManager.Roles.Select(r=>r.Name).ToListAsync();

            allRoles = new SelectList(roleName);

            await GetClaims(id);

            return Page();
        }

        async Task GetClaims(string id){
            var listRoles = from r in _myBlogContext.Roles
                            join ur in _myBlogContext.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;

            var _claimInRole = from c in _myBlogContext.RoleClaims
                                join r in listRoles on c.RoleId equals r.Id
                                select c;

            claimInRole = await _claimInRole.ToListAsync();
            claimInUserClaim = await (from c in _myBlogContext.UserClaims
            where c.UserId == id select c).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if(string.IsNullOrEmpty(id)){
                return NotFound($"Không có user");
            }

            user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound($"Không thấy user, id = {id}.");
            }

            await GetClaims(id);

            //RoleName
            var oldRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();
            var deleteRoles = oldRoleNames.Where(r => !RoleNames.Contains(r));
            var addRoles = RoleNames.Where(r => !oldRoleNames.Contains(r));

            List<string> roleName = await _roleManager.Roles.Select(r=>r.Name).ToListAsync();
            allRoles = new SelectList(roleName);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);
            if(!resultDelete.Succeeded){
                resultDelete.Errors.ToList().ForEach(error =>{
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            var resultAdd= await _userManager.AddToRolesAsync(user, addRoles);
            if(!resultAdd.Succeeded){
                resultAdd.Errors.ToList().ForEach(error =>{
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

             StatusMessage = $"Vừa cập nhật role cho user: {user.UserName}";

             return RedirectToPage("./Index");
        }
    }
}
