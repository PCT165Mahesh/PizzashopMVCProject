using System.ComponentModel.DataAnnotations;

namespace PizzashopMVCProject.ViewModels;

public class ResetPassViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    public string? Email { get; set; }
}
