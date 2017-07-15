using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class PaymentTermRepository : IPaymentTermRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public PaymentTermRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable payment terms
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectablePaymentTerms()
        {
            var paymentTerms = new List<SelectListItem>();

            try
            {
                paymentTerms = _db.PaymentTerm.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.PaymentTermId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting payment terms: {0} ", ex.ToString());
            }

            return paymentTerms;
        }

        /// <summary>
        /// get payment terms
        /// </summary>
        /// <returns></returns>
        public List<PaymentTerm> GetPaymentTerms()
        {
            var paymentTermList = new List<PaymentTerm>();

            try
            {
                paymentTermList = _db.PaymentTerm.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting payment terms: {0} ", ex.ToString());
            }

            return paymentTermList;
        }

        /// <summary>
        /// get payment term by id
        /// </summary>
        /// <param name="paymentTermId"></param>
        /// <returns></returns>
        public PaymentTerm GetPaymentTerm(Guid paymentTermId)
        {
            var paymentTerm = new PaymentTerm();

            try
            {
                paymentTerm = _db.PaymentTerm.FirstOrDefault(x => x.PaymentTermId == paymentTermId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting payment term: {0} ", ex.ToString());
            }

            return paymentTerm;
        }

        /// <summary>
        /// get payment by nullable id 
        /// </summary>
        /// <param name="paymentTermId"></param>
        /// <returns></returns>
        public PaymentTerm GetPaymentTerm(Guid? paymentTermId)
        {
            var paymentTerm = new PaymentTerm();

            try
            {
                paymentTerm = _db.PaymentTerm.FirstOrDefault(x => x.PaymentTermId == paymentTermId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting payment term: {0} ", ex.ToString());
            }

            return paymentTerm;
        }

        /// <summary>
        /// get payment term by id string 
        /// </summary>
        /// <param name="paymentTermId"></param>
        /// <returns></returns>
        public PaymentTerm GetPaymentTerm(string paymentTermId)
        {
            var paymentTerm = new PaymentTerm();

            try
            {
                paymentTerm = _db.PaymentTerm.FirstOrDefault(x => x.Description.Replace(" ", string.Empty).ToLower() == paymentTermId.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting payment term: {0} ", ex.ToString());
            }

            return paymentTerm;
        }

        /// <summary>
        /// save payment term
        /// </summary>
        /// <param name="newPaymentTerm"></param>
        /// <returns></returns>
        public OperationResult SavePaymentTerm(PaymentTerm newPaymentTerm)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingPaymentTerm = _db.PaymentTerm.FirstOrDefault(x => x.Description.ToLower() == newPaymentTerm.Description.ToLower());

                if (existingPaymentTerm == null)
                {
                    logger.Debug("PaymentTerm is being created...");

                    newPaymentTerm.IsActive = true;

                    _db.PaymentTerm.Add(newPaymentTerm);

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
                logger.ErrorFormat("Error saving new payment term: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update payment term
        /// </summary>
        /// <param name="paymentTerm"></param>
        /// <returns></returns>
        public OperationResult UpdatePaymentTerm(PaymentTerm paymentTerm)
        {
            var operationResult = new OperationResult();

            var existingPaymentTerm = GetPaymentTerm(paymentTerm.PaymentTermId);

            if (existingPaymentTerm != null)
            {
                logger.Debug("PaymentTerm is being updated.");

                try
                {
                    _db.PaymentTerm.Attach(existingPaymentTerm);

                    _db.Entry(existingPaymentTerm).CurrentValues.SetValues(paymentTerm);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating payment term: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected payment term.";
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
