using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class CustomerOrderRepository : ICustomerOrderRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public CustomerOrderRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable customer orders by part
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableCustomerOrdersByPart(Guid partId)
        {
            var customerOrders = new List<SelectListItem>();
            try
            {
                customerOrders = (from c in _db.CustomerOrder
                                  join cp in _db.CustomerOrderPart
                                  on c.CustomerOrderId equals cp.CustomerOrderId
                                  where cp.PartId == partId &&
                                        c.IsHold != true && 
                                        c.IsCanceled != true
                                  select new SelectListItem()
                                  {
                                      Value = c.CustomerOrderId.ToString(),
                                      Text = c.PONumber
                                  }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer orders: {0} ", ex.ToString());
            }

            return customerOrders;
        }

        /// <summary>
        /// get all customer orders
        /// </summary>
        /// <returns></returns>
        public List<CustomerOrder> GetCustomerOrders()
        {
            var customerOrders = new List<CustomerOrder>();

            try
            {
                customerOrders = _db.CustomerOrder.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer orders: {0} ", ex.ToString());
            }

            return customerOrders;
        }

        /// <summary>
        /// get all customer order parts
        /// </summary>
        /// <returns></returns>
        public List<CustomerOrderPart> GetCustomerOrderParts()
        {
            var customerOrderParts = new List<CustomerOrderPart>();

            try
            {
                customerOrderParts = _db.CustomerOrderPart.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer order parts: {0} ", ex.ToString());
            }

            return customerOrderParts;
        }

        /// <summary>
        /// get customer order by id
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <returns></returns>
        public CustomerOrder GetCustomerOrder(Guid customerOrderId)
        {
            var customerOrder = new CustomerOrder();

            try
            {
                customerOrder = _db.CustomerOrder.FirstOrDefault(x => x.CustomerOrderId == customerOrderId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer order: {0} ", ex.ToString());
            }

            return customerOrder;
        }

        /// <summary>
        /// get customer order by poNumber
        /// </summary>
        /// <param name="poNumber"></param>
        /// <returns></returns>
        public CustomerOrder GetCustomerOrder(string poNumber)
        {
            var customerOrder = new CustomerOrder();

            try
            {
                customerOrder = _db.CustomerOrder.FirstOrDefault(x => x.PONumber.Replace(" ", string.Empty).ToLower() == poNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer order: {0} ", ex.ToString());
            }

            return customerOrder;
        }

        /// <summary>
        /// get customer order part by id
        /// </summary>
        /// <param name="customerOrderPartId"></param>
        /// <returns></returns>
        public CustomerOrderPart GetCustomerOrderPart(Guid customerOrderPartId)
        {
            var part = new CustomerOrderPart();

            try
            {
                part = _db.CustomerOrderPart.FirstOrDefault(x => x.CustomerOrderPartId == customerOrderPartId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer order part: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get customer order part by price sheet part
        /// </summary>
        /// <param name="priceSheetPartId"></param>
        /// <returns></returns>
        public CustomerOrderPart GetCustomerOrderPartByPriceSheetPart(Guid priceSheetPartId)
        {
            var customerOrderPart = new CustomerOrderPart();

            try
            {
                customerOrderPart = _db.CustomerOrderPart.FirstOrDefault(x => x.PriceSheetPartId == priceSheetPartId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer order part: {0} ", ex.ToString());
            }

            return customerOrderPart;
        }

        /// <summary>
        /// save new customer order
        /// </summary>
        /// <param name="newCustomerOrder"></param>
        /// <returns></returns>
        public OperationResult SaveCustomerOrder(CustomerOrder newCustomerOrder)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingCustomerOrder = _db.CustomerOrder.FirstOrDefault(x => x.PONumber.ToLower() == newCustomerOrder.PONumber.ToLower());

                if (existingCustomerOrder == null)
                {
                    logger.Debug("CustomerOrder is being created...");

                    var insertedCustomerOrder = _db.CustomerOrder.Add(newCustomerOrder);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedCustomerOrder.CustomerOrderId;
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
                logger.ErrorFormat("Error saving new customerOrder: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update customer order 
        /// </summary>
        /// <param name="customerOrder"></param>
        /// <returns></returns>
        public OperationResult UpdateCustomerOrder(CustomerOrder customerOrder)
        {
            var operationResult = new OperationResult();

            var existingCustomerOrder = GetCustomerOrder(customerOrder.CustomerOrderId);

            if (existingCustomerOrder != null)
            {
                logger.Debug("CustomerOrder is being updated.");

                try
                {
                    _db.CustomerOrder.Attach(existingCustomerOrder);

                    _db.Entry(existingCustomerOrder).CurrentValues.SetValues(customerOrder);

                    _db.SaveChanges();

                    var existingParts = _db.CustomerOrderPart.Where(x => x.CustomerOrderId == customerOrder.CustomerOrderId).ToList();

                    if (customerOrder.CustomerOrderParts != null && customerOrder.CustomerOrderParts.Count() > 0)
                    {
                        foreach (var part in customerOrder.CustomerOrderParts)
                        {
                            var existingPart = _db.CustomerOrderPart.FirstOrDefault(x => x.CustomerOrderPartId == part.CustomerOrderPartId);

                            if (existingPart != null)
                            {
                                var qtyDifference = part.Quantity - existingPart.Quantity;

                                existingPart.Quantity = part.Quantity;
                                existingPart.AvailableQuantity = existingPart.AvailableQuantity + qtyDifference;
                                existingPart.EstArrivalDate = part.EstArrivalDate;
                                existingPart.ShipDate = part.ShipDate;

                            }
                            else
                            {
                                _db.CustomerOrderPart.Add(part);

                                existingCustomerOrder.CustomerOrderParts.Add(part);
                            }

                            _db.SaveChanges();
                        }
                    }

                    if (existingParts != null && existingParts.Count > 0)
                    {
                        foreach (var part in existingParts)
                        {
                            var existingPart = customerOrder.CustomerOrderParts.FirstOrDefault(x => x.CustomerOrderPartId == part.CustomerOrderPartId);

                            if (existingPart == null)
                            {
                                _db.CustomerOrderPart.Remove(part);

                                _db.SaveChanges();
                            }
                        }
                    }

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating customer order list: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected customer order.";
            }

            return operationResult;
        }

        /// <summary>
        /// update customer order part
        /// </summary>
        /// <param name="customerOrderPart"></param>
        /// <returns></returns>
        public OperationResult UpdateCustomerOrderPart(CustomerOrderPart customerOrderPart)
        {
            var operationResult = new OperationResult();

            var existingPart = GetCustomerOrderPart(customerOrderPart.CustomerOrderPartId);

            if (existingPart != null)
            {
                logger.Debug("CustomerOrderPart is being updated.");

                try
                {
                    _db.CustomerOrderPart.Attach(existingPart);

                    _db.Entry(existingPart).CurrentValues.SetValues(customerOrderPart);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating customer order part: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected customer order part.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete customer order
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <returns></returns>
        public OperationResult DeleteCustomerOrder(Guid customerOrderId)
        {
            var operationResult = new OperationResult();

            var existingCustomerOrder = GetCustomerOrder(customerOrderId);

            if (existingCustomerOrder != null)
            {
                try
                {
                    _db.CustomerOrder.Attach(existingCustomerOrder);
                    _db.CustomerOrder.Remove(existingCustomerOrder);

                    _db.SaveChanges();

                    var parts = _db.ProjectPart.Where(x => x.CustomerOrderId == customerOrderId).ToList();

                    foreach (var part in parts)
                    {
                        part.CustomerOrderId = null;

                        _db.SaveChanges();
                    }

                    operationResult.Success = true;
                    operationResult.Message = "Delete Success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Unable to delete this Customer Order";
                    logger.ErrorFormat("Error occurred while deleting Customer Order: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find customer order to delete.";
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
