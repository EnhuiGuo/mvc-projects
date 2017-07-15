using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class VesselRepository : IVesselRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public VesselRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable vessels
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableVessels()
        {
            var vessels = new List<SelectListItem>();

            try
            {
                vessels = _db.Vessel.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.VesselId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting vessels: {0} ", ex.ToString());
            }

            return vessels;
        }

        /// <summary>
        /// get all vessels
        /// </summary>
        /// <returns></returns>
        public List<Vessel> GetVessels()
        {
            var vessels = new List<Vessel>();

            try
            {
                vessels = _db.Vessel.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting vessels: {0} ", ex.ToString());
            }

            return vessels;
        }

        /// <summary>
        /// get vessel
        /// </summary>
        /// <param name="vesselId"></param>
        /// <returns></returns>
        public Vessel GetVessel(Guid vesselId)
        {
            var vessel = new Vessel();

            try
            {
                vessel = _db.Vessel.FirstOrDefault(x => x.VesselId == vesselId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting vessel: {0} ", ex.ToString());
            }

            return vessel;
        }

        /// <summary>
        /// save vessel
        /// </summary>
        /// <param name="newVessel"></param>
        /// <returns></returns>
        public OperationResult SaveVessel(Vessel newVessel)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingVessel = _db.Vessel.FirstOrDefault(x => x.Name.ToLower() == newVessel.Name.ToLower());

                if (existingVessel == null)
                {
                    logger.Debug("Vessel is being created...");

                    newVessel.IsActive = true;

                    _db.Vessel.Add(newVessel);

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
                logger.ErrorFormat("Error saving new vessel: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update vessel
        /// </summary>
        /// <param name="vessel"></param>
        /// <returns></returns>
        public OperationResult UpdateVessel(Vessel vessel)
        {
            var operationResult = new OperationResult();

            var existingVessel = GetVessel(vessel.VesselId);

            if (existingVessel != null)
            {
                logger.Debug("Vessel is being updated.");

                try
                {
                    _db.Vessel.Attach(existingVessel);

                    _db.Entry(existingVessel).CurrentValues.SetValues(vessel);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating vessel: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected vessel.";
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
