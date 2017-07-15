using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class TrackingCodeRepository : ITrackingCodeRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public TrackingCodeRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all tracking codes
        /// </summary>
        /// <returns></returns>
        public List<TrackingCode> GetTrackingCodes()
        {
            var trackingCodes = new List<TrackingCode>();

            try
            {
                trackingCodes = _db.TrackingCode.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting tracking codes: {0} ", ex.ToString());
            }

            return trackingCodes;
        }

        /// <summary>
        /// get tracking code by id
        /// </summary>
        /// <param name="trackingCodeId"></param>
        /// <returns></returns>
        public TrackingCode GetTrackingCode(Guid trackingCodeId)
        {
            var trackingCode = new TrackingCode();

            try
            {
                trackingCode = _db.TrackingCode.FirstOrDefault(x => x.TrackingCodeId == trackingCodeId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting tracking code: {0} ", ex.ToString());
            }

            return trackingCode;
        }

        /// <summary>
        /// save tracking code
        /// </summary>
        /// <param name="newTrackingCode"></param>
        /// <returns></returns>
        public OperationResult SaveTrackingCode(TrackingCode newTrackingCode)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingTrackingCode = _db.TrackingCode.FirstOrDefault(x => x.Code.ToLower() == newTrackingCode.Code.ToLower());

                if (existingTrackingCode == null)
                {
                    logger.Debug("TrackingCode is being created...");

                    newTrackingCode.IsActive = true;

                    _db.TrackingCode.Add(newTrackingCode);

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
                logger.ErrorFormat("Error saving new tracking code: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update tracking code
        /// </summary>
        /// <param name="trackingCode"></param>
        /// <returns></returns>
        public OperationResult UpdateTrackingCode(TrackingCode trackingCode)
        {
            var operationResult = new OperationResult();

            var existingTrackingCode = GetTrackingCode(trackingCode.TrackingCodeId);

            if (existingTrackingCode != null)
            {
                logger.Debug("TrackingCode is being updated.");

                try
                {
                    _db.TrackingCode.Attach(existingTrackingCode);

                    _db.Entry(existingTrackingCode).CurrentValues.SetValues(trackingCode);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating tracking code: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected tracking code.";
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
