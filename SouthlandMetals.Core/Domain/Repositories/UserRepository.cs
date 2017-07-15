using Microsoft.AspNet.Identity.EntityFramework;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ApplicationDbContext _dbContext;
        private bool disposed = false;

        public UserRepository()
        {
            _dbContext = new ApplicationDbContext();
        }

        /// <summary>
        /// logUser entered
        /// </summary>
        /// <param name="user"></param>
        public void LogUserIn(ApplicationUser user)
        {
            logger.Debug(user.UserName + " has entered the application.");
        }

        /// <summary>
        /// logUser returned
        /// </summary>
        /// <param name="user"></param>
        public void LogUserReturn(IPrincipal user)
        {
            logger.Debug(user.Identity.Name + " has returned to the application.");
        }

        /// <summary>
        /// logUser register
        /// </summary>
        /// <param name="user"></param>
        public void LogUserRegister(ApplicationUser user)
        {
            logger.Debug(user + " is registered.");
        }

        /// <summary>
        /// logUser update 
        /// </summary>
        /// <param name="user"></param>
        public void LogUserUpdate(ApplicationUser user)
        {
            logger.Debug(user.UserName + " is being updated.");
        }

        /// <summary>
        /// logUser delete 
        /// </summary>
        /// <param name="user"></param>
        public void LogUserDelete(ApplicationUser user)
        {
            logger.Debug(user.UserName + " is being deleted.");
        }

        /// <summary>
        /// logUser out 
        /// </summary>
        /// <param name="user"></param>
        public void LogUserOut(IPrincipal user)
        {
            logger.Debug(user.Identity.Name + " is logging out.");
        }

        /// <summary>
        /// get all users
        /// </summary>
        /// <returns></returns>
        public List<ApplicationUser> GetUsers()
        {
            logger.Debug("Getting users...");

            var users = new List<ApplicationUser>();

            try
            {
                users = _dbContext.Users.OrderBy(y => y.UserName).ToList();
            }
            catch(Exception ex)
            {
                logger.ErrorFormat("Error getting users: {0} ", ex.ToString());
            }

            return users;
        }

        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ApplicationUser GetUserByUserId(string userId)
        {
            logger.Debug("Getting user by userId...");

            var user = new ApplicationUser();

            try
            {
                user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting user by userId: {0} ", ex.ToString());
            }

            return user;
        }

        /// <summary>
        /// get user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ApplicationUser GetUserByUserName(string userName)
        {
            logger.Debug("Getting user by userName...");

            var user = new ApplicationUser();

            try
            {
                user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting user by user name: {0} ", ex.ToString());
            }

            return user;
        }

        /// <summary>
        /// get all roles
        /// </summary>
        /// <returns></returns>
        public List<IdentityRole> GetRoles()
        {
            logger.Debug("Getting roles...");

            var roles = new List<IdentityRole>();

            try
            {
                roles = _dbContext.Roles.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting roles: {0} ", ex.ToString());
            }

            return roles;
        }

        /// <summary>
        /// update user 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public OperationResult UpdateUser(ApplicationUser user)
        {
            var operationResult = new OperationResult();

            logger.Debug("Updating user...");

            var existingUser = _dbContext.Users.First(u => u.UserName == user.UserName);

            if (existingUser != null)
            {
                try
                {
                    LogUserUpdate(existingUser);

                    _dbContext.Entry(existingUser).CurrentValues.SetValues(user);

                    _dbContext.SaveChangesAsync();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error updating user: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find user to update.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete user 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public OperationResult DeleteUser(string userName)
        {
            var operationResult = new OperationResult();

            var existingUser = _dbContext.Users.First(u => u.UserName == userName);

            if (existingUser != null)
            {
                logger.Debug("Deleting user...");

                try
                {
                    LogUserDelete(existingUser);

                    _dbContext.Users.Remove(existingUser);

                    _dbContext.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error deleting user: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find user to delete.";
            }

            return operationResult;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
