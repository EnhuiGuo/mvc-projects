using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class ShipmentRepository : IShipmentRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public ShipmentRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all shipments
        /// </summary>
        /// <returns></returns>
        public List<Shipment> GetShipments()
        {
            var shipments = new List<Shipment>();

            try
            {
                shipments = _db.Shipment.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting shipments: {0} ", ex.ToString());
            }

            return shipments;
        }

        /// <summary>
        /// get shipment by id
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        public Shipment GetShipment(Guid shipmentId)
        {
            var shipment = new Shipment();

            try
            {
                shipment = _db.Shipment.FirstOrDefault(x => x.ShipmentId == shipmentId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting shipment: {0} ", ex.ToString());
            }

            return shipment;
        }

        /// <summary>
        /// save shipment
        /// </summary>
        /// <param name="newShipment"></param>
        /// <returns></returns>
        public OperationResult SaveShipment(Shipment newShipment)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingShipment = _db.Shipment.FirstOrDefault(x => x.ShipmentId == newShipment.ShipmentId);

                if(existingShipment == null)
                {
                    _db.Shipment.Add(newShipment);

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
                operationResult.Message = "Can not create this shipment";
                logger.ErrorFormat("Error saving new shipment: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update shipment
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        public OperationResult UpdateShipment(Shipment shipment)
        {
            var operationResult = new OperationResult();

            var existingShipment = GetShipment(shipment.ShipmentId);

            if(existingShipment != null)
            {
                try
                {
                    _db.Shipment.Attach(existingShipment);

                    _db.Entry(existingShipment).CurrentValues.SetValues(shipment);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating shipment: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected shipment.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete shipment 
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        public OperationResult DeleteShipment(Guid shipmentId)
        {
            var operationResult = new OperationResult();

            var existingShipment = GetShipment(shipmentId);

            if (existingShipment != null)
            {
                try
                {
                    _db.Shipment.Attach(existingShipment);

                    _db.Shipment.Remove(existingShipment);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while deleting shipment: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected shipment.";
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
