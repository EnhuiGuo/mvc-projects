using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class CarrierRepository : ICarrierRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public CarrierRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable carriers
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableCarriers()
        {
            var carriers = new List<SelectListItem>();

            try
            {
                carriers = _db.Carrier.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.CarrierId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting carriers: {0} ", ex.ToString());
            }

            return carriers;
        }

        /// <summary>
        /// get carriers
        /// </summary>
        /// <returns></returns>
        public List<Carrier> GetCarriers()
        {
            var carriers = new List<Carrier>();

            try
            {
                carriers = _db.Carrier.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting carriers: {0} ", ex.ToString());
            }

            return carriers;
        }

        /// <summary>
        /// get carrier by id
        /// </summary>
        /// <param name="carrierId"></param>
        /// <returns></returns>
        public Carrier GetCarrier(Guid carrierId)
        {
            var carrier = new Carrier();

            try
            {
                carrier = _db.Carrier.FirstOrDefault(x => x.CarrierId == carrierId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting carrier: {0} ", ex.ToString());
            }

            return carrier;
        }

        /// <summary>
        /// save new carrier
        /// </summary>
        /// <param name="newCarrier"></param>
        /// <returns></returns>
        public OperationResult SaveCarrier(Carrier newCarrier)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingCarrier = _db.Carrier.FirstOrDefault(x => x.Name.ToLower() == newCarrier.Name.ToLower());

                if (existingCarrier == null)
                {
                    logger.Debug("Carrier is being created...");

                    newCarrier.IsActive = true;

                    _db.Carrier.Add(newCarrier);

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
                logger.ErrorFormat("Error saving new carrier: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update carrier
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        public OperationResult UpdateCarrier(Carrier carrier)
        {
            var operationResult = new OperationResult();

            var existingCarrier = GetCarrier(carrier.CarrierId);

            if (existingCarrier != null)
            {
                logger.Debug("Carrier is being updated.");

                try
                {
                    _db.Carrier.Attach(existingCarrier);

                    _db.Entry(existingCarrier).CurrentValues.SetValues(carrier);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating carrier: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected carrier.";
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
