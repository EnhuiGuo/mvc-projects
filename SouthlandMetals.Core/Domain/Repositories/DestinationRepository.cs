using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class DestinationRepository : IDestinationRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public DestinationRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable destinations
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableDestinations()
        {
            var destinations = new List<SelectListItem>();

            try
            {
                destinations = _db.Destination.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.DestinationId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting destinations: {0} ", ex.ToString());
            }

            return destinations;
        }

        /// <summary>
        /// get all destinations
        /// </summary>
        /// <returns></returns>
        public List<Destination> GetDestinations()
        {
            var destinations = new List<Destination>();

            try
            {
                destinations = _db.Destination.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting destinations: {0} ", ex.ToString());
            }

            return destinations;
        }

        /// <summary>
        /// get destination by id
        /// </summary>
        /// <param name="destinationId"></param>
        /// <returns></returns>
        public Destination GetDestination(Guid destinationId)
        {
            var destination = new Destination();

            try
            {
                destination = _db.Destination.FirstOrDefault(x => x.DestinationId == destinationId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting destination: {0} ", ex.ToString());
            }

            return destination;
        }

        /// <summary>
        /// save new destination
        /// </summary>
        /// <param name="newDestination"></param>
        /// <returns></returns>
        public OperationResult SaveDestination(Destination newDestination)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingDestination = _db.Destination.FirstOrDefault(x => x.Description.ToLower() == newDestination.Description.ToLower());

                if (existingDestination == null)
                {
                    logger.Debug("Destination is being created...");

                    newDestination.IsActive = true;

                    _db.Destination.Add(newDestination);

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
                logger.ErrorFormat("Error saving new destination: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update destination
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public OperationResult UpdateDestination(Destination destination)
        {
            var operationResult = new OperationResult();

            var existingDestination = GetDestination(destination.DestinationId);

            if (existingDestination != null)
            {
                logger.Debug("Destination is being updated.");

                try
                {
                    _db.Destination.Attach(existingDestination);

                    _db.Entry(existingDestination).CurrentValues.SetValues(destination);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating destination: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected destination.";
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
