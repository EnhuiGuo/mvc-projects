using Microsoft.AspNet.Identity.EntityFramework;

namespace SouthlandMetals.Core.Domain.Models
{
    public class ApplicationUserRole : IdentityUserRole
    {
        public ApplicationUserRole() : base()
        { }

        public ApplicationRole Role { get; set; }
    }
}
