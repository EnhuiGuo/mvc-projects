using SouthlandMetals.Common.Enum;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class RfqRepository : IRfqRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public RfqRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all rfqs
        /// </summary>
        /// <returns></returns>
        public List<Rfq> GetRfqs()
        {
            var rfqs = new List<Rfq>();

            try
            {
                rfqs = _db.Rfq.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting rfqs: {0} ", ex.ToString());
            }

            return rfqs;
        }

        /// <summary>
        /// get rfq by id
        /// </summary>
        /// <param name="rfqId"></param>
        /// <returns></returns>
        public Rfq GetRfq(Guid rfqId)
        {
            var rfq = new Rfq();

            try
            {
                rfq = _db.Rfq.FirstOrDefault(x => x.RfqId == rfqId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting rfq: {0} ", ex.ToString());
            }

            return rfq;
        }

        /// <summary>
        /// get rfq by nullable id
        /// </summary>
        /// <param name="rfqId"></param>
        /// <returns></returns>
        public Rfq GetRfq(Guid? rfqId)
        {
            var rfq = new Rfq();

            try
            {
                rfq = _db.Rfq.FirstOrDefault(x => x.RfqId == rfqId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting rfq: {0} ", ex.ToString());
            }

            return rfq;
        }

        /// <summary>
        /// get rfq by number 
        /// </summary>
        /// <param name="rfqNumber"></param>
        /// <returns></returns>
        public Rfq GetRfq(string rfqNumber)
        {
            var rfq = new Rfq();

            try
            {
                rfq = _db.Rfq.Where(b => b.Number.Replace(" ", string.Empty).ToLower() == rfqNumber.Replace(" ", string.Empty).ToLower()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting rfq: {0} ", ex.ToString());
            }

            return rfq;
        }

        /// <summary>
        /// get rfq by project 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Rfq GetRfqByProject(Guid projectId)
        {
            var rfq = new Rfq();

            try
            {
                rfq = _db.Rfq.FirstOrDefault(x => x.ProjectId == projectId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting rfq: {0} ", ex.ToString());
            }

            return rfq;
        }

        /// <summary>
        /// generate rfq number 
        /// </summary>
        /// <returns></returns>
        public string RfqNumber()
        {
            Enums.DocumentNumberType type = Enums.DocumentNumberType.RFQ;

            var rfqNumber = string.Empty;

            try
            {
                var newRfqNumber = new RfqNumber()
                {
                    Type = type.ToString(),
                    Number = null
                };

                var insertedRfqNumber = _db.RfqNumber.Add(newRfqNumber);

                _db.SaveChanges();

                rfqNumber = insertedRfqNumber.Type + String.Format("{0:D6}", insertedRfqNumber.Value);

                var recentRfqNumber = _db.RfqNumber.FirstOrDefault(x => x.Value == insertedRfqNumber.Value && x.Type == insertedRfqNumber.Type);

                recentRfqNumber.Number = rfqNumber;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error generating rfq number: {0} ", ex.ToString());
            }

            return rfqNumber;
        }

        /// <summary>
        /// save rfq 
        /// </summary>
        /// <param name="rfq"></param>
        /// <returns></returns>
        public OperationResult SaveRfq(Rfq rfq)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingRfq = _db.Rfq.FirstOrDefault(x => x.Number.ToLower() == rfq.Number.ToLower());

                if (existingRfq == null)
                {
                    var insertedRfq = _db.Rfq.Add(rfq);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedRfq.RfqId;
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
                operationResult.Message = "Can not create this RFQ";
                logger.ErrorFormat("Error saving new rfq: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update rfq
        /// </summary>
        /// <param name="rfq"></param>
        /// <returns></returns>
        public OperationResult UpdateRfq(Rfq rfq)
        {
            var operationResult = new OperationResult();

            var existingRfq = GetRfq(rfq.RfqId);

            if (existingRfq != null)
            {
                try
                {
                    _db.Rfq.Attach(existingRfq);

                    _db.Entry(existingRfq).CurrentValues.SetValues(rfq);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Edit this RFQ success!";

                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "can not edit this RFQ!";
                    logger.ErrorFormat("Error updating rfq: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "can not find this RFQ!";
            }

            return operationResult;
        }

        /// <summary>
        /// remove rfq number 
        /// </summary>
        /// <param name="rfqNumber"></param>
        public void RemoveRfqNumber(string rfqNumber)
        {
            try
            {
                var temp = _db.RfqNumber.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == rfqNumber.Replace(" ", string.Empty).ToLower());

                _db.RfqNumber.Remove(temp);

                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error removing rfq number: {0} ", ex.ToString());
            }
        }

        /// <summary>
        /// delete rfq
        /// </summary>
        /// <param name="rfqId"></param>
        /// <returns></returns>
        public OperationResult DeleteRfq(Guid rfqId)
        {
            var operationResult = new OperationResult();

            var existingRfq = GetRfq(rfqId);

            if (existingRfq != null)
            {
                try
                {
                    _db.Rfq.Attach(existingRfq);

                    _db.Rfq.Remove(existingRfq);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Delete this RFQ success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Can not delete this RFQ!";
                    logger.ErrorFormat("Error deleting rfq: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "can not find this RFQ!";
            }

            return operationResult;
        }

        /// <summary>
        /// update price sheet 
        /// </summary>
        /// <param name="rfqId"></param>
        /// <param name="priceSheetName"></param>
        /// <returns></returns>
        public OperationResult UpdatePriceSheet(Guid rfqId, string priceSheetName)
        {
            var operationResult = new OperationResult();

            var existingRfq = GetRfq(rfqId);

            if (existingRfq != null)
            {
                try
                {
                    _db.Rfq.Attach(existingRfq);

                    existingRfq.PriceSheet = priceSheetName;

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Import price sheet success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Can not delete this RFQ!";
                    logger.ErrorFormat("Error getting rfq: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "can not find this RFQ!";
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
