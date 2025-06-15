public class UserViewModel
{
    public string Id { get; set; }  // هنا string وليس Guid
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanApprove { get; set; }
}

//namespace FinalProject.Models.ViewModels
//{
//    public class UserViewModel
//{
//        public Guid Id { get; set; }
//        public string UserName { get; set; }
//        public string Email { get; set; }
//        public string Role { get; set; }
//        public bool EmailConfirmed { get; set; }
//        public string PhoneNumber { get; set; }
//        public bool IsApproved { get; set; }

//        public bool CanEdit { get; set; }
//        public bool CanDelete { get; set; }
//        public bool CanApprove { get; set; }
//    }


//}
