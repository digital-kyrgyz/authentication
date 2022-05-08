using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModels
{
    public class ResetPasswordByAdminVm
    {
        public string UserId { get; set; }
        [Display(Name = "Жаны сыр соз")]
        public string NewPassword { get; set; }
    }
}
