using System;
using PizzashopMVCProject.Models;

namespace PizzashopMVCProject.ViewModels
{
    public class UserListViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int  Phone { get; set; }
        public string RoleName { get; set; }
        public bool Status { get; set; }
        
    }
}
