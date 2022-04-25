using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModels
{
    public class ResetPasswordVm
    {
        [Display(Name = "Э-почта адресиниз")]
        [Required(ErrorMessage = "Э-почтаныз шарт")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Жаны Сыр соз")]
        [Display(Name = "Сыр соз адресиниз")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Сыр соз эн аз 8 символ болушу шарт")]
        public string NewPassword { get; set; }
    }
}