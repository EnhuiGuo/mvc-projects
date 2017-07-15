using SouthlandMetals.Core.Domain.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Models
{
    public class UserRolesViewModel
    {
        public UserRolesViewModel()
        {
            Roles = new List<RoleViewModel>();
        }

        // Enable initialization with an instance of ApplicationUser:
        public UserRolesViewModel(ApplicationUser user)
            : this()
        {
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;

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

        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public List<RoleViewModel> Roles { get; set; }
    }
}