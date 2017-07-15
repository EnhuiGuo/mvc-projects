using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class PackingListRepository : IPackingListRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public PackingListRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all packing lists
        /// </summary>
        /// <returns></returns>
        public List<PackingList> GetPackingLists()
        {
            var packingLists = new List<PackingList>();

            try
            {
                packingLists = _db.PackingList.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting packing lists: {0} ", ex.ToString());
            }

            return packingLists;
        }

        /// <summary>
        /// get the parts of a packing list 
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        public List<PackingListPart> GetPackingListPartsByPackingList(Guid packingListId)
        {
            var packingListParts = new List<PackingListPart>();

            try
            {
                packingListParts = _db.PackingListPart.Where(x => x.PackingListId == packingListId).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting packing list parts: {0} ", ex.ToString());
            }

            return packingListParts;
        }

        /// <summary>
        /// get packing list by id
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        public PackingList GetPackingList(Guid packingListId)
        {
            var packingList = new PackingList();

            try
            {
                packingList = _db.PackingList.FirstOrDefault(x => x.PackingListId == packingListId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting packing list: {0} ", ex.ToString());
            }

            return packingList;
        }

        /// <summary>
        /// save packing list 
        /// </summary>
        /// <param name="newPackingList"></param>
        /// <returns></returns>
        public OperationResult SavePackingList(PackingList newPackingList)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingPackingList = _db.PackingList.FirstOrDefault(x => x.PackingListId == newPackingList.PackingListId);

                if (existingPackingList == null)
                {
                    logger.Debug("PackingList is being created...");

                    var insertedPackingList = _db.PackingList.Add(newPackingList);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedPackingList.PackingListId;
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
                logger.ErrorFormat("Error saving new packing list: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update packing list 
        /// </summary>
        /// <param name="packingList"></param>
        /// <returns></returns>
        public OperationResult UpdatePackingList(PackingList packingList)
        {
            var operationResult = new OperationResult();

            var existingPackingList = GetPackingList(packingList.PackingListId);

            if (existingPackingList != null)
            {
                logger.Debug("PackingList is being updated.");

                try
                {
                    _db.PackingList.Attach(existingPackingList);

                    _db.Entry(existingPackingList).CurrentValues.SetValues(packingList);

                    _db.SaveChanges();

                    var existingParts = _db.PackingListPart.Where(x => x.PackingListId == packingList.PackingListId).ToList();

                    if (packingList.PackingListParts != null && packingList.PackingListParts.Count() > 0)
                    {
                        foreach (var part in packingList.PackingListParts)
                        {
                            var existingPart = _db.PackingListPart.FirstOrDefault(x => x.PackingListPartId == part.PackingListPartId);

                            if (existingPart != null)
                            {
                                _db.PackingListPart.Attach(existingPart);

                                existingPart.ShipCode = part.ShipCode;
                                existingPart.PartNumber = part.PartNumber;
                                existingPart.PalletNumber = part.PalletNumber;
                                existingPart.PalletQuantity = part.PalletQuantity;
                                existingPart.PalletWeight = part.PalletWeight;
                                existingPart.PalletTotal = part.PalletTotal;
                                existingPart.TotalPalletQuantity = part.TotalPalletQuantity;
                                existingPart.PONumber = part.PONumber;
                                existingPart.InvoiceNumber = part.InvoiceNumber;
                            }
                            _db.SaveChanges();
                        }
                    }

                    if (existingParts != null && existingParts.Count > 0)
                    {
                        foreach (var part in existingParts)
                        {
                            var existingPart = packingList.PackingListParts.FirstOrDefault(x => x.PackingListPartId == part.PackingListPartId);

                            if (existingPart == null)
                            {
                                _db.PackingListPart.Remove(part);

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
                    logger.ErrorFormat("Error while updating packing list: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected packing list.";
            }

            return operationResult;
        }

        /// <summary>
        /// close packing list 
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        public OperationResult ClosePackingList(Guid packingListId)
        {
            var operationResult = new OperationResult();

            var existingPackingList = GetPackingList(packingListId);

            if (existingPackingList != null)
            {
                try
                {
                    logger.Debug("PackingList is being closed...");

                    _db.PackingList.Attach(existingPackingList);

                    existingPackingList.IsClosed = true;
                    existingPackingList.ClosedDate = DateTime.Now;

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error closing packing list: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find packing list to delete.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete packing list 
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        public OperationResult DeletePackingList(Guid packingListId)
        {
            var operationResult = new OperationResult();

            var existingPackingList = GetPackingList(packingListId);

            if (existingPackingList != null)
            {
                try
                {
                    logger.Debug("PackingList is being deleted...");

                    _db.PackingList.Attach(existingPackingList);

                    _db.PackingList.Remove(existingPackingList);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error saving deleting packing list: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find packing list to delete.";
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
