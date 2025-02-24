using System;

namespace PizzashopMVCProject.ViewModels
{
    public class EditUserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string RoleId { get; set; }
        public string Email { get; set; }

        public bool Status { get; set; }

        public IFormFile? ProfileImage { get; set; } // For Uploading Image

        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }

        public int Zipcode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

    }
}
