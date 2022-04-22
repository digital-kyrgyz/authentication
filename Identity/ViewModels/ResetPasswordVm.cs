using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModels
{
    public class ResetPasswordVm
    {
        [Display(Name = "Э-почта адресиниз")]
        [Required(ErrorMessage = "Э-почтаныз шарт")]
        [EmailAddress]
        public string Email { get; set; }
    }
}