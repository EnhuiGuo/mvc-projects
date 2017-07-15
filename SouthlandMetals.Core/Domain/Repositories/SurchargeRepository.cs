using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class SurchargeRepository : ISurchargeRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public SurchargeRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable surcharges
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableSurcharges()
        {
            var surcharges = new List<SelectListItem>();

            try
            {
                surcharges = _db.Surcharge.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.SurchargeId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting surcharges: {0} ", ex.ToString());
            }

            return surcharges;
        }

        /// <summary>
        /// get all surcharges
        /// </summary>
        /// <returns></returns>
        public List<Surcharge> GetSurcharges()
        {
            var surcharges = new List<Surcharge>();

            try
            {
                surcharges = _db.Surcharge.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting surcharges: {0} ", ex.ToString());
            }

            return surcharges;
        }

        /// <summary>
        /// get surcharge
        /// </summary>
        /// <param name="surchargeId"></param>
        /// <returns></returns>
        public Surcharge GetSurcharge(Guid surchargeId)
        {
            var surcharge = new Surcharge();

            try
            {
                surcharge = _db.Surcharge.FirstOrDefault(x => x.SurchargeId == surchargeId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting surcharge: {0} ", ex.ToString());
            }

            return surcharge;
        }

        /// <summary>
        /// save surcharge
        /// </summary>
        /// <param name="newSurcharge"></param>
        /// <returns></returns>
        public OperationResult SaveSurcharge(Surcharge newSurcharge)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingSurcharge = _db.Surcharge.FirstOrDefault(x => x.Description.ToLower() == newSurcharge.Description.ToLower());

                if (existingSurcharge == null)
                {
                    logger.Debug("Surcharge is being created...");

                    newSurcharge.IsActive = true;

                    _db.Surcharge.Add(newSurcharge);

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
                logger.ErrorFormat("Error saving new surcharge: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update surcharge
        /// </summary>
        /// <param name="surcharge"></param>
        /// <returns></returns>
        public OperationResult UpdateSurcharge(Surcharge surcharge)
        {
            var operationResult = new OperationResult();

            var existingSurcharge = GetSurcharge(surcharge.SurchargeId);

            if (existingSurcharge != null)
            {
                logger.Debug("Surcharge is being updated.");

                try
                {
                    _db.Surcharge.Attach(existingSurcharge);

                    _db.Entry(existingSurcharge).CurrentValues.SetValues(surcharge);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating surcharge: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected surcharge.";
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
