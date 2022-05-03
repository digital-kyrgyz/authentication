using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModels
{
    public class PasswordChangeVm
    {
        [Display(Name = "Эски сыр созунуз")]
        [Required(ErrorMessage = "Эски сыр созду жазыныз")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage ="Сыр соз эн аз 8 символ болуусу шарт")]
        public string PasswordOld { get; set; }
        [Display(Name = "Жаны сыр созунуз")]
        [Required(ErrorMessage = "Жаны сыр созду жазыныз")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Сыр соз эн аз 8 символ болуусу шарт")]
        public string PasswordNew { get; set; }

        [Display(Name = "Салыштыруучу сыр созунузду жазыныз")]
        [Required(ErrorMessage = "Жаны сыр созунду кайра жазыныз")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Сыр соз эн аз 8 символ болуусу шарт")]
        [Compare("PasswordNew", ErrorMessage = "Жаны сыр созунуз, экоо бири-бирине дал келбей жатат")]
        public string PasswordCompare { get; set; }
    }
}
