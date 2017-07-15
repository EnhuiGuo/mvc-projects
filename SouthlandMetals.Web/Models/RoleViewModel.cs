using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Models
{
    //Used to display a single role with a checkbox, within a list structure:
    public class RoleViewModel
    {
        public RoleViewModel() { }
        public RoleViewModel(IdentityRole role)
        {
            RoleId = role.Id;
            RoleName = role.Name;
        }

        [Display(Name = "Role")]
        public string RoleId { get; set; }
        [Display(Name = "Role")]
        public string RoleName { get; set; }
        [Display(Name = "Selected")]
        public bool Selected { get; set; }
    }
}