using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class ShipmentTermRepository : IShipmentTermRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public ShipmentTermRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable shipment terms
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableShipmentTerms()
        {
            var shipmentTerms = new List<SelectListItem>();

            try
            {
                shipmentTerms = _db.ShipmentTerm.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.ShipmentTermId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting shipment terms: {0} ", ex.ToString());
            }

            return shipmentTerms;
        }

        /// <summary>
        /// get all shipment terms
        /// </summary>
        /// <returns></returns>
        public List<ShipmentTerm> GetShipmentTerms()
        {
            var shipmentTerms = new List<ShipmentTerm>();

            try
            {
                shipmentTerms = _db.ShipmentTerm.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting shipment terms: {0} ", ex.ToString());
            }

            return shipmentTerms;
        }

        /// <summary>
        /// get shipment term by id
        /// </summary>
        /// <param name="shipmentTermId"></param>
        /// <returns></returns>
        public ShipmentTerm GetShipmentTerm(Guid shipmentTermId)
        {
            var shipmentTerm = new ShipmentTerm();

            try
            {
                shipmentTerm = _db.ShipmentTerm.FirstOrDefault(x => x.ShipmentTermId == shipmentTermId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting shipment term: {0} ", ex.ToString());
            }

            return shipmentTerm;
        }

        /// <summary>
        /// get shipment term by nullable id
        /// </summary>
        /// <param name="shipmentTermId"></param>
        /// <returns></returns>
        public ShipmentTerm GetShipmentTerm(Guid? shipmentTermId)
        {
            var shipmentTerm = new ShipmentTerm();

            try
            {
                shipmentTerm = _db.ShipmentTerm.FirstOrDefault(x => x.ShipmentTermId == shipmentTermId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting shipment term: {0} ", ex.ToString());
            }

            return shipmentTerm;
        }

        /// <summary>
        /// save shipment term
        /// </summary>
        /// <param name="newShipmentTerm"></param>
        /// <returns></returns>
        public OperationResult SaveShipmentTerm(ShipmentTerm newShipmentTerm)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingShipmentTerm = _db.ShipmentTerm.FirstOrDefault(x => x.Description.ToLower() == newShipmentTerm.Description.ToLower());

                if (existingShipmentTerm == null)
                {
                    logger.Debug("ShipmentTerm is being created...");

                    newShipmentTerm.IsActive = true;

                    _db.ShipmentTerm.Add(newShipmentTerm);

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
                logger.ErrorFormat("Error saving new shipment term: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update shipment term
        /// </summary>
        /// <param name="shipmentTerm"></param>
        /// <returns></returns>
        public OperationResult UpdateShipmentTerm(ShipmentTerm shipmentTerm)
        {
            var operationResult = new OperationResult();

            var existingShipmentTerm = GetShipmentTerm(shipmentTerm.ShipmentTermId);

            if (existingShipmentTerm != null)
            {
                logger.Debug("ShipmentTerm is being updated.");

                try
                {
                    _db.ShipmentTerm.Attach(existingShipmentTerm);

                    _db.Entry(existingShipmentTerm).CurrentValues.SetValues(shipmentTerm);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating shipment term: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected shipment term.";
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
