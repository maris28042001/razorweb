using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using razor.models;
using razor_web.models;

namespace App.Admin.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly AppDbContext _myBlogContext;
        private readonly UserManager<AppUser> _userManager;

        public EditUserRoleClaimModel(AppDbContext myBlogContext, UserManager<AppUser> userManager)
        {
            _myBlogContext = myBlogContext;
            _userManager = userManager;
        }


        [TempData]
        public string StatusMessage { get; set; }

        public NotFoundObjectResult OnGet()
        {
            return NotFound("Không được truy cập");
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
        public AppUser user { get; set; }

        public async Task<IActionResult> OnGetAddClaimAsync(string userid){
            user = await _userManager.FindByIdAsync(userid);
            if(user == null) return NotFound("Không tìm thấy user");
            return Page();
        }

        public async Task<IActionResult> OnPostAddClaimAsync(string userid){
            user = await _userManager.FindByIdAsync(userid);
            if(user == null) return NotFound("Không tìm thấy user");
            if(!ModelState.IsValid) return Page();

            var claims = _myBlogContext.UserClaims.Where(c => c.UserId  == user.Id);

            
            if(claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue)){
                ModelState.AddModelError(string.Empty, "Đặc tính này đã có");
                return Page();
            }

            await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Đã thêm đặc tính cho user";
            return RedirectToPage("./AddROle", new {Id = user.Id});
        }

        public IdentityUserClaim<string> userClaim { get; set; }

        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid){

            if(claimid == null) return NotFound("Không tìm thấy user");

            userClaim = _myBlogContext.UserClaims.Where(c=>c.Id == claimid).FirstOrDefault();

            user = await _userManager.FindByIdAsync(userClaim.UserId);

            if(user == null) return NotFound("Không tìm thấy user");

            Input = new InputModel {
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue
            };
            return Page();
        }

        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid){

            if(claimid == null) return NotFound("Không tìm thấy user");

            userClaim = _myBlogContext.UserClaims.Where(c=>c.Id == claimid).FirstOrDefault();

            user = await _userManager.FindByIdAsync(userClaim.UserId);

            if(user == null) return NotFound("Không tìm thấy user");

            if(!ModelState.IsValid) return Page();

            if(_myBlogContext.UserClaims.Any(c=>c.UserId == user.Id && 
                c.ClaimType == Input.ClaimType && 
                c.ClaimValue == Input.ClaimValue && 
                c.Id != userClaim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có");
                return Page();
            }

            userClaim.ClaimType = Input.ClaimType;
            userClaim.ClaimValue = Input.ClaimValue;
            await _myBlogContext.SaveChangesAsync();
            StatusMessage = "Bạn vừa cập nhật claim";

            return RedirectToPage("./AddRole", new {Id = user.Id});
        }

         public async Task<IActionResult> OnPostDeleteAsync(int? claimid){

            if(claimid == null) return NotFound("Không tìm thấy user");

            userClaim = _myBlogContext.UserClaims.Where(c=>c.Id == claimid).FirstOrDefault();

            user = await _userManager.FindByIdAsync(userClaim.UserId);

            if(user == null) return NotFound("Không tìm thấy user");
            await _userManager.RemoveClaimAsync(user, new Claim(userClaim.ClaimType, userClaim.ClaimValue));
            StatusMessage = "Bạn đã xóa claim";

            return RedirectToPage("./AddRole", new {Id = user.Id});
        }
    }
}
