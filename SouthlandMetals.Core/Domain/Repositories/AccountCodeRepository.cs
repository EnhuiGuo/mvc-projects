using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class AccountCodeRepository : IAccountCodeRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public AccountCodeRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable account codes by customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableAccountCodesByCustomer(string customerId)
        {
            var accountCodes = new List<SelectListItem>();

            try
            {
                accountCodes = _db.AccountCode.Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == customerId.Replace(" ", string.Empty).ToLower()).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.Description.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting account codes: {0} ", ex.ToString());
            }

            return accountCodes;
        }

        /// <summary>
        /// get all account codes
        /// </summary>
        /// <returns></returns>
        public List<AccountCode> GetAccountCodes()
        {
            var accountCodes = new List<AccountCode>();

            try
            {
                accountCodes = _db.AccountCode.ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error getting account codes: {0} ", e.ToString());
            }

            return accountCodes;
        }

        /// <summary>
        /// get account code by id
        /// </summary>
        /// <param name="accountCodeId"></param>
        /// <returns></returns>
        public AccountCode GetAccountCode(Guid accountCodeId)
        {
            var accountCode = new AccountCode();

            try
            {
                accountCode = _db.AccountCode.FirstOrDefault(x => x.AccountCodeId == accountCodeId);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error getting account code: {0} ", e.ToString());
            }

            return accountCode;
        }

        /// <summary>
        /// save new account code 
        /// </summary>
        /// <param name="newAccountCode"></param>
        /// <returns></returns>
        public OperationResult SaveAccountCode(AccountCode newAccountCode)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingAccountCode = _db.AccountCode.FirstOrDefault(x => x.AccountCodeId == newAccountCode.AccountCodeId);

                if (existingAccountCode == null)
                {
                    _db.AccountCode.Add(newAccountCode);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                else
                {
                    operationResult.Success = false;
                    operationResult.Message = "Duplicate Entry";
                }  
            }
            catch (Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = "Error";
                logger.ErrorFormat("Error saving new account code: { 0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update account code
        /// </summary>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        public OperationResult UpdateAccountCode(AccountCode accountCode)
        {
            var operationResult = new OperationResult();

            var existingAccountCode = GetAccountCode(accountCode.AccountCodeId);

            if (existingAccountCode != null)
            {
                logger.Debug("Account Code is being updated....");

                try
                {
                    _db.AccountCode.Attach(existingAccountCode);

                    _db.Entry(existingAccountCode).CurrentValues.SetValues(accountCode);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error updating account code: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected account code.";
            }

            return operationResult;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
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
