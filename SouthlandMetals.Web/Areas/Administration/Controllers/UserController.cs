using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Models;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Administration.Controllers
{
    public class UserController : ApplicationBaseController
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private IUserRepository _userRepository;

        public UserController()
        {
            _userRepository = new UserRepository();
        }

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private OperationResult EditUserRoles(UserViewModel model)
        {
            var operationResult = new OperationResult();
            operationResult.Success = true;

            if (model.UserId != null)
            {
                var idManager = new ApplicationDbContext.IdentityManager();

                var roles = model.Roles.OrderBy(x => x.RoleName).ToList();

                foreach (var role in roles)
                {
                    if (role.Selected)
                    {
                        if (!UserManager.IsInRole(model.UserId, role.RoleName))
                        {
                            try
                            {
                                operationResult.Success = idManager.AddUserToRole(model.UserId, role.RoleName);

                                if (!operationResult.Success)
                                {
                                    operationResult.Message = "Error adding user to role " + role.RoleName;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                operationResult.Success = false;
                                operationResult.Message = "Error adding user to role " + role.RoleName;
                                logger.ErrorFormat("Error adding user to role " + role.RoleName + ": { 0} ", ex.ToString());
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (UserManager.IsInRole(model.UserId, role.RoleName))
                        {
                            try
                            {
                                var result = UserManager.RemoveFromRole(model.UserId, role.RoleName);

                                if (!result.Succeeded)
                                {
                                    operationResult.Success = false;
                                    operationResult.Message = "Error removing user from role " + role.RoleName;
                                    break;
                                }
                                else {
                                    operationResult.Success = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                operationResult.Success = false;
                                operationResult.Message = "Error removing user from role " + role.RoleName;
                                logger.ErrorFormat("Error removing user to role " + role.RoleName + ": { 0} ", ex.ToString());
                                break;
                            }
                        }
                    }
                }
            }

            return operationResult;
        }

        // GET: Administration/User
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = new UserViewModel();

            model.Users = _userRepository.GetUsers();

            return View(model);
        }

        /// <summary>
        /// GET: Administration/User/Add
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Add()
        {
            var model = new NewAccountViewModel();

            model.Roles = _userRepository.GetRoles().Select(x => new RoleViewModel()
            {
                RoleId = x.Id,
                RoleName = x.Name
            }).OrderBy(y => y.RoleName).ToList();

            model.CurrentUser = User.Identity.GetUserName();

            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        [AllowAnonymous]
        public async Task<JsonResult> Add(NewAccountViewModel model)
        {
            var operationResult = new OperationResult();

            var user = _userRepository.GetUserByUserName(model.UserName);

            if (user == null)
            {
                try
                {
                    ApplicationUser newUser = new UserConverter().ConvertToDomain(model);

                    var result = await UserManager.CreateAsync(newUser, model.NewPassword);

                    if (result.Succeeded)
                    {
                        _userRepository.LogUserRegister(user);

                        operationResult.Success = true;
                    }
                    else
                    {
                        operationResult.Success = false;
                        operationResult.Message = "Error occurred trying to create new user.";
                    }
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error occurred trying to create new user.";
                    logger.ErrorFormat("Error occurred trying to create new user: {0} ", ex.ToString());
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/User/Detail
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Detail(string userName)
        {
            var user = _userRepository.GetUserByUserName(userName);

            var model = new UserViewModel(user);

            model.Roles = model.Roles.OrderBy(x => x.RoleName).ToList();

            return View(model);
        }

        /// <summary>
        /// edit user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult Edit(UserViewModel model)
        {
            var operationResult = new OperationResult();

            ApplicationUser user = new UserConverter().ConvertToDomain(model);

            operationResult = _userRepository.UpdateUser(user);

            if (operationResult.Success)
            {
                operationResult = EditUserRoles(model);
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// reset Password modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _ResetPassword()
        {
            return PartialView();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        [AllowAnonymous]
        public async Task<JsonResult> ResetPassword(UserViewModel model)
        {
            var operationResult = new OperationResult();

            var user = await UserManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var result = await UserManager.ResetPasswordAsync(user.Id, resetToken, model.Password);

                if (result.Succeeded)
                {
                    operationResult.Success = true;
                }
                else
                {
                    operationResult.Success = false;
                }
            }
            else
            {
                operationResult.Success = false;
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Delete(string userName)
        {
            var operationResult = new OperationResult();

            operationResult = _userRepository.DeleteUser(userName);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userRepository != null)
                {
                    _userRepository.Dispose();
                    _userRepository = null;
                }

                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}