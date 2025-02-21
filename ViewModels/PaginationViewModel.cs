using System;

namespace PizzashopMVCProject.ViewModels
{
    public class PaginationViewModel
    {
        public List<UserListViewModel> UserList { get; set; } = new List<UserListViewModel>();

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }
    }
}
