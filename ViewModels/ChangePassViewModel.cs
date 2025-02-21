using System.ComponentModel.DataAnnotations;

namespace PizzashopMVCProject.ViewModels;

public class ChangePassViewModel
{
    public string  Email { get; set; }
    
    [Required(ErrorMessage = "The current password is required")]
    public string  CurrentPassword { get; set; }

    [Required(ErrorMessage = "This field is required")]
    public string  NewPassword { get; set; }

    [Required(ErrorMessage = "This field is required")]
    public string  ConfirmNewPassword { get; set; }
}
