using SouthlandMetals.Core.Domain.Models;
using SouthlandMetals.Web.Areas.Administration.Models;
using System.Collections.Generic;

namespace SouthlandMetals.Web.Converters
{
    public class UserConverter
    {
        /// <summary>
        /// convert user view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ApplicationUser ConvertToDomain(UserViewModel model)
        {
            ApplicationUser user = new ApplicationUser();

            user.Id = model.UserId;
            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Department = model.Department;
            user.Position = model.Position;
            user.IsActive = model.IsActive;
            user.ModifiedDate = model.ModifiedDate;
            user.ModifiedBy = model.ModifiedBy;

            return user;
        }

        /// <summary>
        /// convert new account view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ApplicationUser ConvertToDomain(NewAccountViewModel model)
        {
            ApplicationUser user = new ApplicationUser();

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Department = model.Department;
            user.Position = model.Position;
            user.IsActive = true;

            if (model.Roles != null)
            {
                user.UserRoles = new List<ApplicationUserRole>();

                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        ApplicationUserRole userRole = new ApplicationUserRole();
                        {
                            userRole.UserId = user.Id;
                            userRole.RoleId = role.RoleId;
                        }

                        user.UserRoles.Add(userRole);
                    }
                }
            }

            return user;
        }
    }
}