using System;
using PizzashopMVCProject.Models;

namespace PizzashopMVCProject.ViewModels
{
    public class UserListViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string  Phone { get; set; }
        public string RoleName { get; set; }

        public string ImgUrl { get; set; }
        public bool Status { get; set; }
        
    }
}