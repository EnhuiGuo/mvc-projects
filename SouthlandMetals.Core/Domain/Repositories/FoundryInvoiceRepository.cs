using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class FoundryInvoiceRepository : IFoundryInvoiceRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public FoundryInvoiceRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable foundry invoices
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableFoundryInvoices()
        {
            var invoices = new List<SelectListItem>();

            try
            {
                invoices = _db.FoundryInvoice.Select(y => new SelectListItem()
                {
                    Text = y.Number,
                    Value = y.FoundryInvoiceId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting invoices: {0} ", ex.ToString());
            }

            return invoices;
        }

        /// <summary>
        /// get selectable foundry invoices by foundry 
        /// </summary>
        /// <param name="foundryId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableFoundryInvoicesByFoundry(string foundryId)
        {
            var invoices = new List<SelectListItem>();

            try
            {
                invoices = _db.FoundryInvoice.Where(x => x.FoundryId.Replace(" ", string.Empty).ToLower() == foundryId.Replace(" ", string.Empty).ToLower()).Select(y => new SelectListItem()
                {
                    Text = y.Number,
                    Value = y.FoundryInvoiceId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting invoices: {0} ", ex.ToString());
            }

            return invoices;
        }

        /// <summary>
        /// get foundry invoices
        /// </summary>
        /// <returns></returns>
        public List<FoundryInvoice> GetFoundryInvoices()
        {
            var foundryInvoices = new List<FoundryInvoice>();

            try
            {
                foundryInvoices = _db.FoundryInvoice.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting foundry invoices: { 0} ", ex.ToString());
            }

            return foundryInvoices;
        }

        /// <summary>
        /// get foundry invoice
        /// </summary>
        /// <param name="foundryInvoiceId"></param>
        /// <returns></returns>
        public FoundryInvoice GetFoundryInvoice(Guid foundryInvoiceId)
        {
            var foundryInvoice = new FoundryInvoice();

            try
            {
                foundryInvoice = _db.FoundryInvoice.FirstOrDefault(x => x.FoundryInvoiceId == foundryInvoiceId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting foundry invoice: { 0} ", ex.ToString());
            }

            return foundryInvoice;
        }

        /// <summary>
        /// get foundry invoice number 
        /// </summary>
        /// <param name="foundryInvoiceNumber"></param>
        /// <returns></returns>
        public FoundryInvoice GetFoundryInvoice(string foundryInvoiceNumber)
        {
            var foundryInvoice = new FoundryInvoice();

            try
            {
                foundryInvoice = _db.FoundryInvoice.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == foundryInvoiceNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting foundry invoice: { 0} ", ex.ToString());
            }

            return foundryInvoice;
        }

        /// <summary>
        /// get foundry invoice by bill of lading
        /// </summary>
        /// <param name="billOfLadingId"></param>
        /// <returns></returns>
        public FoundryInvoice GetFoundryInvoiceByBillOfLading(Guid billOfLadingId)
        {
            var foundryInvoice = new FoundryInvoice();

            try
            {
                foundryInvoice = _db.FoundryInvoice.FirstOrDefault(x => x.FoundryInvoiceId == billOfLadingId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting foundry invoice: { 0} ", ex.ToString());
            }

            return foundryInvoice;
        }

        /// <summary>
        /// save new foundry invoice 
        /// </summary>
        /// <param name="newFoundryInvoice"></param>
        /// <returns></returns>
        public OperationResult SaveFoundryInvoice(FoundryInvoice newFoundryInvoice)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingFoundryInvoice = _db.FoundryInvoice.FirstOrDefault(x => x.Number.ToLower() == newFoundryInvoice.Number.ToLower());

                if(existingFoundryInvoice == null)
                {
                   _db.FoundryInvoice.Add(newFoundryInvoice);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Create this newFoundryInvoice success!";
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
                operationResult.Message = "Can not create this newFoundryInvoice";
                logger.ErrorFormat("Error saving new foundry invoice: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update foundry invoice
        /// </summary>
        /// <param name="foundryInvoice"></param>
        /// <returns></returns>
        public OperationResult UpdateFoundryInvoice(FoundryInvoice foundryInvoice)
        {
            var operationResult = new OperationResult();

            var existingFoundryInvoice = _db.FoundryInvoice.Find(foundryInvoice.FoundryInvoiceId);

            if (existingFoundryInvoice != null)
            {
                logger.Debug("foundry invoice is being updated.");

                try
                {
                    _db.FoundryInvoice.Attach(existingFoundryInvoice);

                    _db.Entry(existingFoundryInvoice).CurrentValues.SetValues(foundryInvoice);

                    _db.SaveChanges();

                    var existingBuckets = _db.Bucket.Where(x => x.FoundryInvoiceId == foundryInvoice.FoundryInvoiceId).ToList();

                    if (foundryInvoice.Buckets != null && foundryInvoice.Buckets.Count > 0)
                    {
                        foreach (var bucket in foundryInvoice.Buckets)
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
                            var existingBucket = foundryInvoice.Buckets.FirstOrDefault(x => x.BucketId == bucket.BucketId);

                            if (existingBucket == null)
                            {
                                _db.Bucket.Remove(bucket);

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
                    logger.ErrorFormat("Error while updating foundry invoice: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected foundry invoice.";
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
