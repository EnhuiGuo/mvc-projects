using SouthlandMetals.Core.Domain.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Models
{
    public class ManageViewModel : RoleViewModel
    {
        public ManageViewModel()
        {
            Roles = new List<RoleViewModel>();
        }

        // Enable initialization with an instance of ApplicationUser:
        public ManageViewModel(ApplicationUser user)
            : this()
        {
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Phone = user.Phone;
            Department = user.Department;
            Position = user.Position;
            IsActive = user.IsActive;

            var db = new ApplicationDbContext();

            // Add all available roles to the list of EditorViewModels:
            var allRoles = db.Roles;
            var users = db.Users;

            foreach (var role in allRoles)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new RoleViewModel(role);
                Roles.Add(rvm);
            }

            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            foreach (var role in Roles)
            {
                foreach (var userRole in user.Roles)
                {
                    if (userRole.RoleId == role.RoleId)
                    {
                        role.Selected = true;
                    }
                }
            }
        }

        [Required]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Position")]
        public string Position { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public List<RoleViewModel> Roles { get; set; }
    }
}