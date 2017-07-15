using SouthlandMetals.Common.Enum;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class FoundryOrderRepository : IFoundryOrderRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public FoundryOrderRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get foundry order by id
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        public FoundryOrder GetFoundryOrder(Guid foundryOrderId)
        {
            var foundryOrder = new FoundryOrder();

            try
            {
                foundryOrder = _db.FoundryOrder.Find(foundryOrderId);
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while getting foundry order. Error: " + e);
            }

            return foundryOrder;
        }

        /// <summary>
        /// get foundry order by poNumber
        /// </summary>
        /// <param name="poNumber"></param>
        /// <returns></returns>
        public FoundryOrder GetFoundryOrder(string poNumber)
        {
            var foundryOrder = new FoundryOrder();

            try
            {
                foundryOrder = _db.FoundryOrder.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == poNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while getting foundry order. Error: " + e);
            }

            return foundryOrder;
        }

        /// <summary>
        /// get all foundry orders
        /// </summary>
        /// <returns></returns>
        public List<FoundryOrder> GetFoundryOrders()
        {
            var foundryOrders = new List<FoundryOrder>();

            try
            {
                foundryOrders = _db.FoundryOrder.ToList();
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while GetFoundryOrders. Error: " + e);
            }

            return foundryOrders;
        }

        /// <summary>
        /// get all foundry order parts
        /// </summary>
        /// <returns></returns>
        public List<FoundryOrderPart> GetFoundryOrderParts()
        {
            var foundryOrderParts = new List<FoundryOrderPart>();

            try
            {
                foundryOrderParts = _db.FoundryOrderPart.ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error getting Foundry Order parts: {0} ", e.ToString());
            }

            return foundryOrderParts;
        }

        /// <summary>
        /// get tooling foundry order by foundry order 
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        public FoundryOrder GetToolingFoundryOrder(Guid foundryOrderId)
        {
            var foundryOrder = new FoundryOrder();

            try
            {
                foundryOrder = _db.FoundryOrder.FirstOrDefault(x => x.FoundryOrderId == foundryOrderId &&
                                                                    x.IsTooling);
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while getting foundry order. Error: " + e);
            }

            return foundryOrder;
        }

        /// <summary>
        /// get foundry order part by id
        /// </summary>
        /// <param name="foundryOrderPartId"></param>
        /// <returns></returns>
        public FoundryOrderPart GetFoundryOrderPart(Guid foundryOrderPartId)
        {
            var part = new FoundryOrderPart();

            try
            {
                part = _db.FoundryOrderPart.FirstOrDefault(x => x.FoundryOrderPartId == foundryOrderPartId);
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while getting foundry order part. Error: " + e);
            }

            return part;
        }

        /// <summary>
        /// get foundry order part by project part 
        /// </summary>
        /// <param name="projectPartId"></param>
        /// <returns></returns>
        public FoundryOrderPart GetFoundryOrderPartByProjectPart(Guid projectPartId)
        {
            var part = new FoundryOrderPart();

            try
            {
                part = _db.FoundryOrderPart.FirstOrDefault(x => x.ProjectPartId == projectPartId);
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while getting foundry order part by project part. Error: " + e);
            }

            return part;
        }

        /// <summary>
        /// get foundry order part by part
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public FoundryOrderPart GetFoundryOrderPartByPart(Guid partId)
        {
            var part = new FoundryOrderPart();

            try
            {
                part = _db.FoundryOrderPart.FirstOrDefault(x => x.PartId == partId);
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while getting foundry order part by project part. Error: " + e);
            }

            return part;
        }

        /// <summary>
        /// save foundry order
        /// </summary>
        /// <param name="foundryOrder"></param>
        /// <returns></returns>
        public OperationResult SaveFoundryOrder(FoundryOrder foundryOrder)
        {
            var operationResult = new OperationResult();

            var existingFoundryOrder = _db.FoundryOrder.FirstOrDefault(x => x.Number.ToLower() == foundryOrder.Number.ToLower());

            if (existingFoundryOrder == null)
            {
                try
                {
                    logger.Debug("FoundryOrder is being created...");

                    var insertedFoundryOrder = _db.FoundryOrder.Add(foundryOrder);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedFoundryOrder.FoundryOrderId;
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error saving new foundryOrder: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Duplicate Entry";
            }

            return operationResult;
        }

        /// <summary>
        /// update foundry order 
        /// </summary>
        /// <param name="foundryOrder"></param>
        /// <returns></returns>
        public OperationResult UpdateFoundryOrder(FoundryOrder foundryOrder)
        {
            var operationResult = new OperationResult();

            var currentFoundryOrder = GetFoundryOrder(foundryOrder.FoundryOrderId);

            if (currentFoundryOrder != null)
            {
                logger.Debug("FoundryOrder is being updated.");

                try
                {
                    _db.FoundryOrder.Attach(currentFoundryOrder);

                    _db.Entry(currentFoundryOrder).CurrentValues.SetValues(foundryOrder);

                    _db.SaveChanges();

                    var existingParts = _db.FoundryOrderPart.Where(x => x.FoundryOrderId == foundryOrder.FoundryOrderId).ToList();

                    if (foundryOrder.FoundryOrderParts != null && foundryOrder.FoundryOrderParts.Count() > 0)
                    {
                        foreach (var part in foundryOrder.FoundryOrderParts)
                        {
                            var existingPart = _db.FoundryOrderPart.FirstOrDefault(x => x.FoundryOrderPartId == part.FoundryOrderPartId);

                            if (existingPart != null)
                            {
                                var qtyDifference = part.Quantity - existingPart.Quantity;

                                existingPart.CustomerOrderPartId = part.CustomerOrderPartId;
                                existingPart.Quantity = part.Quantity;
                                existingPart.AvailableQuantity = existingPart.AvailableQuantity + qtyDifference;
                                existingPart.EstArrivalDate = part.EstArrivalDate;
                                existingPart.ShipDate = part.ShipDate;
                                existingPart.ShipCode = part.ShipCode;
                                existingPart.ShipCodeNotes = part.ShipCodeNotes;

                                var customerOrderPartToUpdate = _db.CustomerOrderPart.FirstOrDefault(x => x.CustomerOrderPartId == part.CustomerOrderPartId);
                                customerOrderPartToUpdate.AvailableQuantity = customerOrderPartToUpdate.AvailableQuantity - qtyDifference;
                                _db.SaveChanges();
                            }
                            else
                            {
                                _db.FoundryOrderPart.Add(part);
                                _db.SaveChanges();

                                var customerOrderPartToUpdate = _db.CustomerOrderPart.FirstOrDefault(x => x.CustomerOrderPartId == part.CustomerOrderPartId);
                                customerOrderPartToUpdate.AvailableQuantity = customerOrderPartToUpdate.AvailableQuantity + part.Quantity;
                                _db.SaveChanges();
                            }
                        }
                    }

                    if (existingParts != null && existingParts.Count > 0)
                    {
                        foreach (var part in existingParts)
                        {
                            var partCheck = foundryOrder.FoundryOrderParts.FirstOrDefault(x => x.FoundryOrderPartId == part.FoundryOrderPartId);

                            if (partCheck == null)
                            {
                                _db.FoundryOrderPart.Remove(part);
                                _db.SaveChanges();

                                var customerOrderPartToUpdate = _db.CustomerOrderPart.FirstOrDefault(x => x.CustomerOrderPartId == part.CustomerOrderPartId);
                                customerOrderPartToUpdate.AvailableQuantity = customerOrderPartToUpdate.AvailableQuantity + part.Quantity;
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
                    logger.ErrorFormat("Error while updating foundry order: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected foundry order.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete foundry order 
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        public OperationResult DeleteFoundryOrder(Guid foundryOrderId)
        {
            var operationResult = new OperationResult();
            
            try
            {
                var orderToDelete = _db.FoundryOrder.FirstOrDefault(x => x.FoundryOrderId == foundryOrderId);

                if (orderToDelete != null)
                {
                    _db.FoundryOrder.Remove(orderToDelete);
                    _db.SaveChanges();

                    var parts = _db.ProjectPart.Where(x => x.FoundryOrderId == foundryOrderId).ToList();

                    foreach (var part in parts)
                    {
                        part.FoundryOrderId = null;

                        _db.SaveChanges();
                    }

                    operationResult.Success = true;
                    operationResult.Message = "Delete Success!";
                }
                else
                {
                    operationResult.Success = false;
                    operationResult.Message = "Foundry Order does not exist!";
                }
            }
            catch (Exception e)
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to delete this Foundry Order";
                logger.ErrorFormat("Error occurred while deleting Foundry Order: {0} ", e.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update foundry order part 
        /// </summary>
        /// <param name="foundryOrderPart"></param>
        /// <returns></returns>
        public OperationResult UpdateFoundryOrderPart(FoundryOrderPart foundryOrderPart)
        {
            var operationResult = new OperationResult();

            var existingPart = GetFoundryOrderPart(foundryOrderPart.FoundryOrderPartId);

            if(existingPart != null)
            {
                try
                {
                    _db.FoundryOrderPart.Attach(existingPart);

                    _db.Entry(existingPart).CurrentValues.SetValues(foundryOrderPart);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating foundry order part: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected foundry order part.";
            }

            return operationResult;
        }

        /// <summary>
        /// get selectable foundry orders
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableFoundryOrders()
        {
            var foundryOrders = new List<SelectListItem>();

            try
            {
                foundryOrders = _db.FoundryOrder.Where(x => x.IsProduction &&
                                                            !x.IsHold &&
                                                            !x.IsCanceled).Select(y => new SelectListItem()
                                                            {
                                                                Text = y.Number,
                                                                Value = y.FoundryOrderId.ToString()
                                                            }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error GetSelectableFoundryOrders: {0} ", e.ToString());
            }

            return foundryOrders;
        }

        /// <summary>
        /// generate foundry order number
        /// </summary>
        /// <returns></returns>
        public string FoundryOrderNumber()
        {
            Enums.DocumentNumberType type = Enums.DocumentNumberType.PO;
            var foundryOrderNumber = string.Empty;
            try
            {
                var newFoundryOrderNumber = new PurchaseOrderNumber()
                {
                    Type = type.ToString(),
                    Number = null
                };

                var insertedPurchaseOrderNumber = _db.PurchaseOrderNumber.Add(newFoundryOrderNumber);

                _db.SaveChanges();

                foundryOrderNumber = insertedPurchaseOrderNumber.Type + String.Format("{0:D6}", insertedPurchaseOrderNumber.Value);

                var recentPurchaseOrderNumber = _db.PurchaseOrderNumber.FirstOrDefault(x => x.Value == insertedPurchaseOrderNumber.Value && x.Type == insertedPurchaseOrderNumber.Type);

                recentPurchaseOrderNumber.Number = foundryOrderNumber;

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while retrieving FoundryOrderNumber. Error: " + e);
            }

            return foundryOrderNumber;
        }

        /// <summary>
        /// remove foundry order number
        /// </summary>
        /// <param name="orderNumber"></param>
        public void RemoveFoundryOrderNumber(string orderNumber)
        {
            try
            {
                var foundryOrderNumber = _db.PurchaseOrderNumber.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == orderNumber.Replace(" ", string.Empty).ToLower());

                _db.PurchaseOrderNumber.Remove(foundryOrderNumber);

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while removing FoundryOrderNumber. Error: " + e);
            }
        }

        /// <summary>
        /// generate ship code number 
        /// </summary>
        /// <returns></returns>
        public string ShipCodeNumber()
        {
            var shipCodeNumber = string.Empty;

            try
            {
                var newShipCodeNumber = new ShipCodeNumber()
                {
                    Number = null
                };

                var insertedShipCodeNumber = _db.ShipCodeNumber.Add(newShipCodeNumber);

                _db.SaveChanges();

                shipCodeNumber = String.Format("{0:D5}", insertedShipCodeNumber.Value);

                var recentShipCodeNumber = _db.ShipCodeNumber.FirstOrDefault(x => x.Value == insertedShipCodeNumber.Value);

                recentShipCodeNumber.Number = shipCodeNumber;

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while retrieving ShipCodeNumber. Error: " + e);
            }

            return shipCodeNumber;
        }

        /// <summary>
        /// generate ship code number with prefix and suffix
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public string ShipCodeNumber(string prefix, string suffix)
        {
            var shipCodeNumber = string.Empty;

            try
            {
                var newShipCodeNumber = new ShipCodeNumber()
                {
                    Number = null
                };

                var insertedShipCodeNumber = _db.ShipCodeNumber.Add(newShipCodeNumber);

                _db.SaveChanges();

                shipCodeNumber = prefix + String.Format("{0:D5}", insertedShipCodeNumber.Value) + suffix;

                var recentShipCodeNumber = _db.ShipCodeNumber.FirstOrDefault(x => x.Value == insertedShipCodeNumber.Value);

                recentShipCodeNumber.Number = shipCodeNumber;

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while retrieving ShipCodeNumber. Error: " + e);
            }

            return shipCodeNumber;
        }

        /// <summary>
        /// remove ship code number
        /// </summary>
        /// <param name="number"></param>
        public void RemoveShipCodeNumber(string number)
        {
            try
            {
                var shipCodeNumber = _db.ShipCodeNumber.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == number.Replace(" ", string.Empty).ToLower());

                _db.ShipCodeNumber.Remove(shipCodeNumber);

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Error("An error occurred while removing ShipCodeNumber. Error: " + e);
            }
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
