using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class OrderTermRepository : IOrderTermRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public OrderTermRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get order terms by customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public OrderTerm GetOrderTermsByCustomer(string customerId)
        {
            var orderTerms = new OrderTerm();

            try
            {
                orderTerms = _db.OrderTerm.FirstOrDefault(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == customerId.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting order term by customer: {0} ", ex.ToString());
            }

            return orderTerms;
        }

        /// <summary>
        /// get order term by id
        /// </summary>
        /// <param name="orderTermId"></param>
        /// <returns></returns>
        public OrderTerm GetOrderTerm(Guid orderTermId)
        {
            var orderTerm = new OrderTerm();

            try
            {
                orderTerm = _db.OrderTerm.FirstOrDefault(x => x.OrderTermId == orderTermId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting order term: {0} ", ex.ToString());
            }

            return orderTerm;
        }

        /// <summary>
        /// update order term
        /// </summary>
        /// <param name="orderTerm"></param>
        /// <returns></returns>
        public OperationResult UpdateOrderTerm(OrderTerm orderTerm)
        {
            var operationResult = new OperationResult();

            var existingTerms = GetOrderTerm(orderTerm.OrderTermId);

            if(existingTerms != null)
            {
                logger.Debug("order terms are being updated.");

                try
                {
                    _db.OrderTerm.Attach(existingTerms);

                    _db.Entry(existingTerms).CurrentValues.SetValues(orderTerm);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error updating order term: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected order terms.";
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
