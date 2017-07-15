using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Administration.Controllers
{
    public class AccountCodeController : ApplicationBaseController
    {
        private IAccountCodeRepository _accountCodeRepository;
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private IBucketRepository _bucketRepository;

        public AccountCodeController()
        {
            _accountCodeRepository = new AccountCodeRepository();
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _bucketRepository = new BucketRepository();
        }

        public AccountCodeController(IAccountCodeRepository accountCodeRepository,
                                     ICustomerDynamicsRepository customerDynamicsRepository,
                                     IBucketRepository bucketRepository)
        {
            _accountCodeRepository = accountCodeRepository;
            _customerDynamicsRepository = customerDynamicsRepository;
            _bucketRepository = bucketRepository;
        }

        /// <summary>
        /// GET: Administration/AccountCode
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = new AccountCodeViewModel();

            var accountCodes = new List<AccountCodeViewModel>();

            var tempCodes = _accountCodeRepository.GetAccountCodes();

            foreach (var tempCode in tempCodes)
            {
                AccountCodeViewModel convertedModel = new AccountCodeConverter().ConvertToView(tempCode);

                accountCodes.Add(convertedModel);
            }

            model.AccountCodes = accountCodes.OrderBy(x => x.Description).ToList();

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableBuckets = _bucketRepository.GetSelectableBuckets();

            return View(model);
        }

        /// <summary>
        /// edit accountCode modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditAccountCode(Guid accountCodeId)
        {
            var accountCode = _accountCodeRepository.GetAccountCode(accountCodeId);

            AccountCodeViewModel model = new AccountCodeConverter().ConvertToView(accountCode);

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableBuckets = _bucketRepository.GetSelectableBuckets();

            return PartialView(model);
        }

        /// <summary>
        /// edit accountCode
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditAccountCode(AccountCodeViewModel model)
        {
            var operationResult = new OperationResult();

            AccountCode code = new AccountCodeConverter().ConvertToDomain(model);

            operationResult = _accountCodeRepository.UpdateAccountCode(code);

            if (operationResult.Success)
            {
                model.Success = true;

                var accountCodes = new List<AccountCodeViewModel>();

                var tempCodes = _accountCodeRepository.GetAccountCodes();

                foreach (var tempCode in tempCodes)
                {
                    AccountCodeViewModel convertedModel = new AccountCodeConverter().ConvertToView(tempCode);

                    accountCodes.Add(convertedModel);
                }

                model.AccountCodes = accountCodes.OrderBy(x => x.Description).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_accountCodeRepository != null)
                {
                    _accountCodeRepository.Dispose();
                    _accountCodeRepository = null;
                }

                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }

                if (_bucketRepository != null)
                {
                    _bucketRepository.Dispose();
                    _bucketRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}