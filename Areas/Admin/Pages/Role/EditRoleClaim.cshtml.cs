using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor.models;
using Humanizer;

namespace App.Admin.Role
{
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {
        }

        public class InputModel{
            [Display(Name ="Kiểu (Tên) claim")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256,MinimumLength = 3 ,ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string ClaimType { get; set; }

            [Display(Name ="Giá trị")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256,MinimumLength = 3 ,ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string ClaimValue { get; set; }

        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IdentityRole role { get; set; }

        IdentityRoleClaim<string> claim { get; set; }

        public async Task<IActionResult> OnGet(int? claimid)    
        {  
            if(claimid == null){
                return NotFound("Không tìm thấy role");
            }

            claim = _myBlogContext.RoleClaims.Where(c => c.Id  == claimid).FirstOrDefault();

            if(claim == null){
                return NotFound("Không tìm thấy role");
            }

            role = await  _roleManager.FindByIdAsync(claim.RoleId);
            if(role == null){
                return NotFound("Không tìm thấy role");
            }

            Input = new InputModel(){
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int claimid){
            
            if(claimid == null){
                return NotFound("Không tìm thấy role");
            }

            claim = _myBlogContext.RoleClaims.Where(c => c.Id  == claimid).FirstOrDefault();

            if(claim == null){
                return NotFound("Không tìm thấy role");
            }

            role = await  _roleManager.FindByIdAsync(claim.RoleId);
            if(role == null){
                return NotFound("Không tìm thấy role");
            }

            if(!ModelState.IsValid){
                return Page();
            }

            if(_myBlogContext.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id)){
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return Page();
            }

            claim.ClaimValue = Input.ClaimValue;
            claim.ClaimType = Input.ClaimType;

            await _myBlogContext.SaveChangesAsync();

            StatusMessage = "Vừa cập nhật claim";
            return RedirectToPage("./Edit",  new {roleid = role.Id});
        }


        public async Task<IActionResult> OnPostDelete(int claimid){
            
            if(claimid == null){
                return NotFound("Không tìm thấy role");
            }

            claim = _myBlogContext.RoleClaims.Where(c => c.Id  == claimid).FirstOrDefault();

            if(claim == null){
                return NotFound("Không tìm thấy role");
            }

            role = await  _roleManager.FindByIdAsync(claim.RoleId);
            if(role == null){
                return NotFound("Không tìm thấy role");
            }

            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));

            StatusMessage = "Vừa xóa claim";
            return RedirectToPage("./Edit",  new {roleid = role.Id});
        }
    }
}
