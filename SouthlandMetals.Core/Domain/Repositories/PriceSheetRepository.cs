using SouthlandMetals.Common.Enum;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class PriceSheetRepository : IPriceSheetRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public PriceSheetRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all price sheets
        /// </summary>
        /// <returns></returns>
        public List<PriceSheet> GetPriceSheets()
        {
            var priceSheets = new List<PriceSheet>();

            try
            {
                priceSheets = _db.PriceSheet.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting price sheets: {0} ", ex.ToString());
            }

            return priceSheets;
        }

        /// <summary>
        /// get the parts of a price sheet 
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        public List<PriceSheetPart> GetPriceSheetParts(Guid priceSheetId)
        {
            var parts = new List<PriceSheetPart>();

            try
            {
                parts = _db.PriceSheetPart.Where(x => x.PriceSheetId == priceSheetId).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting price sheet parts: {0} ", ex.ToString());
            }

            return parts;
        }

        /// <summary>
        /// get all price sheet buckets
        /// </summary>
        /// <returns></returns>
        public List<PriceSheetBucket> GetPriceSheetBuckets()
        {
            var priceSheetBuckets = new List<PriceSheetBucket>();

            try
            {
                priceSheetBuckets = _db.PriceSheetBucket.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting price sheet buckets: {0} ", ex.ToString());
            }

            return priceSheetBuckets;
        }

        /// <summary>
        /// get price sheet by id
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        public PriceSheet GetPriceSheet(Guid priceSheetId)
        {
            var priceSheet = new PriceSheet();

            try
            {
                priceSheet = _db.PriceSheet.FirstOrDefault(x => x.PriceSheetId == priceSheetId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting price sheet: {0} ", ex.ToString());
            }

            return priceSheet;
        }

        /// <summary>
        /// get quote price sheet by project 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public PriceSheet GetQuotePriceSheet(Guid projectId)
        {
            var priceSheet = new PriceSheet();

            try
            {
                priceSheet = _db.PriceSheet.FirstOrDefault(x => x.ProjectId == projectId && x.IsQuote == true);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting quote price sheet: {0} ", ex.ToString());
            }

            return priceSheet;
        }

        /// <summary>
        /// get production price sheet by project 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public PriceSheet GetProductionPriceSheet(Guid projectId)
        {
            var priceSheet = new PriceSheet();

            try
            {
                priceSheet = _db.PriceSheet.FirstOrDefault(x => x.ProjectId == projectId && x.IsProduction == true);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting quote price sheet: {0} ", ex.ToString());
            }

            return priceSheet;
        }

        /// <summary>
        /// get price sheet by number 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public PriceSheet GetPriceSheet(string number)
        {
            var priceSheet = new PriceSheet();

            try
            {
                priceSheet = _db.PriceSheet.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == number.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting price sheet: {0} ", ex.ToString());
            }

            return priceSheet;
        }

        /// <summary>
        /// get price sheet by project 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public PriceSheet GetPriceSheetByProject(Guid projectId)
        {
            var priceSheet = new PriceSheet();

            try
            {
                priceSheet = _db.PriceSheet.FirstOrDefault(x => x.ProjectId == projectId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting price sheet: {0} ", ex.ToString());
            }

            return priceSheet;
        }

        /// <summary>
        /// get price sheet part by id
        /// </summary>
        /// <param name="priceSheetPartId"></param>
        /// <returns></returns>
        public PriceSheetPart GetPriceSheetPart(Guid priceSheetPartId)
        {
            var part = new PriceSheetPart();

            try
            {
                part = _db.PriceSheetPart.FirstOrDefault(x => x.PriceSheetPartId == priceSheetPartId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting price sheet part: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get price sheet part by project part 
        /// </summary>
        /// <param name="projectPartId"></param>
        /// <returns></returns>
        public PriceSheetPart GetPriceSheetPartByProjectPart(Guid projectPartId)
        {
            var part = new PriceSheetPart();

            try
            {
                part = _db.PriceSheetPart.FirstOrDefault(x => x.ProjectPartId == projectPartId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting price sheet part: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get production price sheet part by project part
        /// </summary>
        /// <param name="projectPartId"></param>
        /// <returns></returns>
        public PriceSheetPart GetProductionPriceSheetPartByProjectPart(Guid projectPartId)
        {
            var part = new PriceSheetPart();

            try
            {
                part = _db.PriceSheetPart.FirstOrDefault(x => x.ProjectPartId == projectPartId && x.IsProduction);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting price sheet part: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get production price sheet by project 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public PriceSheet GetProductionPriceSheetByProject(Guid projectId)
        {
            var priceSheet = new PriceSheet();

            try
            {
                priceSheet = _db.PriceSheet.FirstOrDefault(x => x.ProjectId == projectId && x.IsProduction);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting production price sheet: {0} ", ex.ToString());
            }

            return priceSheet;
        }

        /// <summary>
        /// get production price sheet by rfq
        /// </summary>
        /// <param name="rfqId"></param>
        /// <returns></returns>
        public PriceSheet GetProductionPriceSheetBrRfq(Guid rfqId)
        {
            var priceSheet = new PriceSheet();

            try
            {
                priceSheet = _db.PriceSheet.FirstOrDefault(x => x.RfqId == rfqId && x.IsProduction);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting production price sheet: {0} ", ex.ToString());
            }

            return priceSheet;
        }

        /// <summary>
        /// get quote price sheet by project 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public PriceSheet GetQuotePriceSheetByProject(Guid projectId)
        {
            var priceSheet = new PriceSheet();

            try
            {
                priceSheet = _db.PriceSheet.FirstOrDefault(x => x.ProjectId == projectId && x.IsQuote);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting quote price sheet: {0} ", ex.ToString());
            }

            return priceSheet;
        }

        /// <summary>
        /// get quote price sheet by rfq
        /// </summary>
        /// <param name="rfqId"></param>
        /// <returns></returns>
        public PriceSheet GetQuotePriceSheetByRfq(Guid rfqId)
        {
            var priceSheet = new PriceSheet();

            try
            {
                priceSheet = _db.PriceSheet.FirstOrDefault(x => x.RfqId == rfqId && x.IsQuote);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting quote price sheet: {0} ", ex.ToString());
            }

            return priceSheet;
        }

        /// <summary>
        /// generate price sheet number   
        /// </summary>
        /// <returns></returns>
        public string PriceSheetNumber()
        {
            Enums.DocumentNumberType type = Enums.DocumentNumberType.PS;

            var priceSheetNumber = string.Empty;

            try
            {
                var newPriceSheetNumber = new PriceSheetNumber()
                {
                    Type = type.ToString(),
                    Number = null
                };

                var insertedPriceSheetNumber = _db.PriceSheetNumber.Add(newPriceSheetNumber);

                _db.SaveChanges();

                priceSheetNumber = insertedPriceSheetNumber.Type + String.Format("{0:D6}", insertedPriceSheetNumber.Value);

                var recentPriceSheetNumber = _db.PriceSheetNumber.FirstOrDefault(x => x.Value == insertedPriceSheetNumber.Value && x.Type == insertedPriceSheetNumber.Type);

                recentPriceSheetNumber.Number = priceSheetNumber;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error occurred generating price sheet number: { 0} ", ex.ToString());
            }

            return priceSheetNumber;
        }

        /// <summary>
        /// save price sheet 
        /// </summary>
        /// <param name="newPriceSheet"></param>
        /// <returns></returns>
        public OperationResult SavePriceSheet(PriceSheet newPriceSheet)
        {
            var operationResult = new OperationResult();

            try
            {
                var priceSheet = _db.PriceSheet.FirstOrDefault(x => x.Number.ToLower() == newPriceSheet.Number.ToLower());

                if (priceSheet == null)
                {
                    var insertedPriceSheet = _db.PriceSheet.Add(newPriceSheet);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Create Price Sheet success!";
                    operationResult.ReferenceId = insertedPriceSheet.PriceSheetId;
                }
                else
                {
                    operationResult.Success = true;
                    operationResult.Message = "Duplicate Entry";
                }
            }
            catch (Exception ex)
            {
                operationResult.Message = "can not create this price sheet";
                operationResult.Success = false;
                logger.ErrorFormat("Error saving new price sheet: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update price sheet 
        /// </summary>
        /// <param name="priceSheet"></param>
        /// <returns></returns>
        public OperationResult UpdatePriceSheet(PriceSheet priceSheet)
        {
            var operationResult = new OperationResult();

            var existingPriceSheet = GetPriceSheet(priceSheet.PriceSheetId);

            if (existingPriceSheet != null)
            {
                logger.Debug("PriceSheet is being updated.");

                try
                {
                    _db.PriceSheet.Attach(existingPriceSheet);

                    _db.Entry(existingPriceSheet).CurrentValues.SetValues(priceSheet);

                    _db.SaveChanges();

                    var existingBuckets = GetPriceSheetBuckets().Where(x => x.PriceSheetId == priceSheet.PriceSheetId).ToList();

                    if(priceSheet.PriceSheetBuckets != null && priceSheet.PriceSheetBuckets.Count > 0)
                    {
                        foreach (var bucket in priceSheet.PriceSheetBuckets)
                        {
                            var existingBucket = _db.PriceSheetBucket.FirstOrDefault(x => x.PriceSheetBucketId == bucket.PriceSheetBucketId);

                            if (existingBucket != null)
                            {
                                _db.PriceSheetBucket.Attach(existingBucket);

                                existingBucket.IsAddOn = bucket.IsAddOn;
                                existingBucket.IsDuty = bucket.IsDuty;
                                existingBucket.IsSurcharge = bucket.IsSurcharge;
                                existingBucket.Margin = bucket.Margin;
                                existingBucket.Name = bucket.Name;
                                existingBucket.PNumber = bucket.PNumber;
                                existingBucket.SellValue = bucket.SellValue;
                                existingBucket.Value = bucket.Value;
                            }
                            else
                            {
                                bucket.PriceSheetId = priceSheet.PriceSheetId;

                                _db.PriceSheetBucket.Add(bucket);

                                existingPriceSheet.PriceSheetBuckets.Add(bucket);
                            }

                            _db.SaveChanges();
                        }
                    }

                    if (existingBuckets != null && existingBuckets.Count > 0)
                    {
                        foreach (var bucket in existingBuckets)
                        {
                            var existingBucket = priceSheet.PriceSheetBuckets.FirstOrDefault(x => x.PriceSheetBucketId == bucket.PriceSheetBucketId);

                            if (existingBucket == null)
                            {
                                _db.PriceSheetBucket.Attach(bucket);

                                _db.PriceSheetBucket.Remove(bucket);

                                existingPriceSheet.PriceSheetBuckets.Remove(existingBucket);

                                _db.SaveChanges();
                            }
                        }
                    }

                    var existingParts = GetPriceSheetParts(priceSheet.PriceSheetId);

                    if (priceSheet.PriceSheetParts != null && priceSheet.PriceSheetParts.Count > 0)
                    {
                        foreach (var part in priceSheet.PriceSheetParts)
                        {
                            var existingPart = _db.PriceSheetPart.FirstOrDefault(x => x.PriceSheetPartId == part.PriceSheetPartId);

                            if (existingPart != null)
                            {
                                _db.PriceSheetPart.Attach(existingPart);

                                existingPart.AvailableQuantity = part.AvailableQuantity;
                                existingPart.AnnualUsage = part.AnnualUsage;
                                existingPart.RawCost = part.RawCost;
                                existingPart.RawPrice = part.RawPrice;
                                existingPart.AnnualRawCost = part.AnnualRawCost;
                                existingPart.AnnualRawPrice = part.AnnualRawPrice;
                                existingPart.PNumberCost = part.PNumberCost;
                                existingPart.PNumberPrice = part.PNumberPrice;
                                existingPart.MachineCost = part.MachineCost;
                                existingPart.MachinePrice = part.MachinePrice;
                                existingPart.FOBCost = part.FOBCost;
                                existingPart.FOBPrice = part.FOBPrice;
                                existingPart.AddOnCost = part.AddOnCost;
                                existingPart.AddOnPrice = part.AddOnPrice;
                                existingPart.SurchargeCost = part.SurchargeCost;
                                existingPart.SurchargePrice = part.SurchargePrice;
                                existingPart.DutyCost = part.DutyCost;
                                existingPart.DutyPrice = part.DutyPrice;
                                existingPart.Cost = part.Cost;
                                existingPart.Price = part.Price;
                                existingPart.AnnualCost = part.AnnualCost;
                                existingPart.AnnualPrice = part.AnnualPrice;
                                existingPart.FixtureCost = part.FixtureCost;
                                existingPart.FixturePrice = part.FixturePrice;
                                existingPart.PatternCost = part.PatternCost;
                                existingPart.PatternPrice = part.PatternPrice;
                            }
                            else
                            {
                                part.PriceSheetId = priceSheet.PriceSheetId;

                                _db.PriceSheetPart.Add(part);

                                existingPriceSheet.PriceSheetParts.Add(part);
                            }

                            _db.SaveChanges();
                        }
                    }

                    if (existingParts != null && existingParts.Count > 0)
                    {
                        foreach (var part in existingParts)
                        {
                            var existingPart = priceSheet.PriceSheetParts.FirstOrDefault(x => x.PriceSheetPartId == part.PriceSheetPartId);

                            if (existingPart == null)
                            {
                                _db.PriceSheetPart.Attach(part);

                                _db.PriceSheetPart.Remove(part);

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
                    logger.ErrorFormat("Error while updating priceSheet: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected priceSheet.";
            }
            return operationResult;
        }

        /// <summary>
        /// update price sheet part 
        /// </summary>
        /// <param name="updatePriceSheetPart"></param>
        /// <returns></returns>
        public OperationResult UpdatePriceSheetPart(PriceSheetPart updatePriceSheetPart)
        {
            var operationResult = new OperationResult();

            var existingPriceSheetPart = _db.PriceSheetPart.Find(updatePriceSheetPart.PriceSheetPartId);

            if (existingPriceSheetPart != null)
            {
                try
                {
                    _db.Entry(existingPriceSheetPart).CurrentValues.SetValues(updatePriceSheetPart);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Update Price Sheet Part success!";
                }
                catch (Exception ex)
                {
                    operationResult.Message = "can not update this price sheet part";
                    operationResult.Success = false;
                    logger.ErrorFormat("Error while updating price sheet part: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected priceSheetPart.";
            }

            return operationResult;
        }

        /// <summary>
        /// remover price sheet number 
        /// </summary>
        /// <param name="newPriceSheetNumber"></param>
        public void RemovePriceSheetNumber(string newPriceSheetNumber)
        {
            try
            {
                var priceSheetNumber = _db.PriceSheetNumber.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == newPriceSheetNumber.Replace(" ", string.Empty).ToLower());

                _db.PriceSheetNumber.Remove(priceSheetNumber);

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error occurred removing price sheet number: { 0} ", ex.ToString());
            }
        }

        /// <summary>
        /// delete price sheet 
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        public OperationResult DeletePriceSheet(Guid priceSheetId)
        {
            var operationResult = new OperationResult();

            var existingPriceSheet = GetPriceSheet(priceSheetId);

            if (existingPriceSheet != null)
            {
                try
                {
                    _db.PriceSheet.Attach(existingPriceSheet);

                    _db.PriceSheet.Remove(existingPriceSheet);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Delete success!";
                }
                catch (Exception ex)
                {
                    operationResult.Message = "can not Delete";
                    operationResult.Success = false;
                    logger.ErrorFormat("Error while deleting price sheet: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Message = "Can not find this price sheet!";
            }

            return operationResult;
        }

        /// <summary>
        /// save price sheet part 
        /// </summary>
        /// <param name="newPriceSheetPart"></param>
        /// <returns></returns>
        public OperationResult SavePriceSheetPart(PriceSheetPart newPriceSheetPart)
        {
            var operationResult = new OperationResult();

            try
            {
                var priceSheetPart = _db.PriceSheetPart.FirstOrDefault(x => x.ProjectPartId == newPriceSheetPart.ProjectPartId && x.PriceSheetId == newPriceSheetPart.PriceSheetId);

                if (priceSheetPart == null)
                {
                    _db.PriceSheetPart.Add(newPriceSheetPart);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Create Price Sheet success!";
                }
                else
                {
                    operationResult.Success = true;
                    operationResult.Message = "Duplicate Entry";
                }
            }
            catch (Exception ex)
            {
                operationResult.Message = "can not create this price sheet";
                operationResult.Success = false;
                logger.ErrorFormat("Error saving new price sheet: {0} ", ex.ToString());
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
