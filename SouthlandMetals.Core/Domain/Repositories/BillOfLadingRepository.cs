using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class BillOfLadingRepository : IBillOfLadingRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public BillOfLadingRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all bill of ladings
        /// </summary>
        /// <returns></returns>
        public List<BillOfLading> GetBillOfLadings()
        {
            var billOfLadingS = new List<BillOfLading>();

            try
            {
                billOfLadingS = _db.BillOfLading.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting bill of ladings: {0} ", ex.ToString());
            }

            return billOfLadingS;
        }

        /// <summary>
        /// get bill of lading by id
        /// </summary>
        /// <param name="billOfLadingId"></param>
        /// <returns></returns>
        public BillOfLading GetBillOfLading(Guid billOfLadingId)
        {
            var billOfLading = new BillOfLading();

            try
            {
                billOfLading = _db.BillOfLading.FirstOrDefault(x => x.BillOfLadingId == billOfLadingId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting bill of lading: {0} ", ex.ToString());
            }

            return billOfLading;
        }

        /// <summary>
        /// get bill of lading by number
        /// </summary>
        /// <param name="bolNumber"></param>
        /// <returns></returns>
        public BillOfLading GetBillOfLading(string bolNumber)
        {
            var billOfLading = new BillOfLading();

            try
            {
                billOfLading = _db.BillOfLading.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == bolNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting bill of lading: {0} ", ex.ToString());
            }

            return billOfLading;
        }

        /// <summary>
        /// save new bill of lading
        /// </summary>
        /// <param name="newBillOfLading"></param>
        /// <returns></returns>
        public OperationResult SaveBillOfLading(BillOfLading newBillOfLading)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingBillOfLading = _db.BillOfLading.FirstOrDefault(x => x.Number.ToLower() == newBillOfLading.Number.ToLower());

                if (existingBillOfLading == null)
                {
                    logger.Debug("Bill of Lading is being created...");

                    _db.BillOfLading.Add(newBillOfLading);

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
                logger.ErrorFormat("Error saving new bill of lading: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update bill of lading
        /// </summary>
        /// <param name="billOfLading"></param>
        /// <returns></returns>
        public OperationResult UpdateBillOfLading(BillOfLading billOfLading)
        {
            var operationResult = new OperationResult();

            var existingBillOfLading = _db.BillOfLading.Find(billOfLading.BillOfLadingId);

            if (existingBillOfLading != null)
            {
                logger.Debug("Bill of Lading is being updated.");

                try
                {
                    _db.BillOfLading.Attach(existingBillOfLading);

                    _db.Entry(existingBillOfLading).CurrentValues.SetValues(billOfLading);

                    _db.SaveChanges();

                    var existingContainers = _db.Container.Where(x => x.BillOfLadingId == billOfLading.BillOfLadingId).ToList();

                    if (billOfLading.Containers != null && billOfLading.Containers.Count > 0)
                    {
                        foreach (var container in billOfLading.Containers)
                        {
                            var existingContainer = _db.Container.Find(container.ContainerId);

                            if (existingContainer == null)
                            {
                                existingBillOfLading.Containers.Add(container);

                                _db.SaveChanges();
                            }
                            else
                            {
                                _db.Entry(existingContainer).CurrentValues.SetValues(container);

                                var existingContainerParts = _db.ContainerPart.Where(x => x.ContainerId == container.ContainerId).ToList();

                                if (container.ContainerParts != null && container.ContainerParts.Count > 0)
                                {
                                    foreach (var containerPart in container.ContainerParts)
                                    {
                                        int qtyDifference = 0;

                                        var existingContainerPart = _db.ContainerPart.Find(containerPart.ContainerPartId);
                                        if (existingContainerPart != null)
                                        {
                                            qtyDifference = containerPart.Quantity - existingContainerPart.Quantity;
                                            _db.Entry(existingContainerPart).CurrentValues.SetValues(containerPart);
                                        }
                                        else
                                        {
                                            _db.ContainerPart.Add(containerPart);
                                            qtyDifference = containerPart.Quantity;
                                        }

                                        var foundryOrderPart = _db.FoundryOrderPart.Find(containerPart.FoundryOrderPartId);

                                        if (foundryOrderPart != null)
                                        {
                                            foundryOrderPart.AvailableQuantity = foundryOrderPart.AvailableQuantity - qtyDifference;
                                        }
                                    }
                                }

                                if (existingContainerParts != null && existingContainerParts.Count > 0)
                                {
                                    foreach (var containerPart in existingContainerParts)
                                    {
                                        var existingContainerPart = container.ContainerParts.FirstOrDefault(x => x.ContainerPartId == containerPart.ContainerPartId);
                                        if (existingContainerPart == null)
                                        {
                                            _db.ContainerPart.Remove(containerPart);
                                        }
                                    }
                                }

                                _db.SaveChanges();
                            }
                        }
                    }

                    if (existingContainers != null && existingContainers.Count > 0)
                    {
                        foreach (var container in existingContainers)
                        {
                            var existingContainer = billOfLading.Containers.FirstOrDefault(x => x.ContainerId == container.ContainerId);

                            if (existingContainer == null)
                            {
                                _db.Container.Remove(container);

                                _db.SaveChanges();
                            }
                        }
                    }

                    var existingFoundryInvoice = _db.FoundryInvoice.Find(billOfLading.FoundryInvoice.FoundryInvoiceId);

                    if (existingFoundryInvoice != null)
                    {
                        var existingBuckets = _db.Bucket.Where(x => x.FoundryInvoiceId == existingFoundryInvoice.FoundryInvoiceId).ToList();

                        if (billOfLading.FoundryInvoice != null)
                        {
                            _db.Entry(existingFoundryInvoice).CurrentValues.SetValues(billOfLading.FoundryInvoice);

                            _db.SaveChanges();

                            if (billOfLading.FoundryInvoice.Buckets != null && billOfLading.FoundryInvoice.Buckets.Count > 0)
                            {
                                foreach (var bucket in billOfLading.FoundryInvoice.Buckets)
                                {
                                    var existingBucket = _db.Bucket.Find(bucket.BucketId);

                                    if (existingBucket == null)
                                    {
                                        existingFoundryInvoice.Buckets.Add(bucket);

                                        _db.SaveChanges();
                                    }
                                }
                            }

                            if (existingBuckets != null && existingBuckets.Count > 0)
                            {
                                foreach (var bucket in existingBuckets)
                                {
                                    var existingBucket = billOfLading.FoundryInvoice.Buckets.FirstOrDefault(x => x.BucketId == bucket.BucketId);

                                    if (existingBucket == null)
                                    {
                                        _db.Bucket.Remove(bucket);

                                        _db.SaveChanges();
                                    }
                                }
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
                    logger.ErrorFormat("Error while updating bol: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected bol.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete bill of lading
        /// </summary>
        /// <param name="billOfLadingId"></param>
        /// <returns></returns>
        public OperationResult DeleteBillOfLading(Guid billOfLadingId)
        {
            var operationResult = new OperationResult();

            var existingBillOfLading = _db.BillOfLading.Find(billOfLadingId);

            if (existingBillOfLading != null)
            {
                logger.Debug("Bol is being deleted.");

                try
                {
                    var foundryInvoice = _db.FoundryInvoice.Find(billOfLadingId);

                    if (foundryInvoice != null)
                        _db.FoundryInvoice.Remove(foundryInvoice);

                    _db.BillOfLading.Remove(existingBillOfLading);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while deleting bol: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected bol.";
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