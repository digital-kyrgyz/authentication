using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModels
{
    public class RoleVm
    {
        public string Id { get; set; }
        [Display(Name = "Role аттын")]
        [Required(ErrorMessage = "Role аты созсуз турдо керек")]
        public string Name { get; set; }
    }
}
