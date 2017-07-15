using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class PartStatusRepository : IPartStatusRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public PartStatusRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selecable part states
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectablePartStates()
        {
            var partStates = new List<SelectListItem>();

            try
            {
                partStates = _db.PartStatus.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.PartStatusId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part states: {0} ", ex.ToString());
            }

            return partStates;
        }

        /// <summary>
        /// get all part states
        /// </summary>
        /// <returns></returns>
        public List<PartStatus> GetPartStates()
        {
            var partStates = new List<PartStatus>();

            try
            {
                partStates = _db.PartStatus.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part states: {0} ", ex.ToString());
            }

            return partStates;
        }

        /// <summary>
        /// get part status by id
        /// </summary>
        /// <param name="partStatusId"></param>
        /// <returns></returns>
        public PartStatus GetPartStatus(Guid partStatusId)
        {
            var partStatus = new PartStatus();

            try
            {
                partStatus = _db.PartStatus.FirstOrDefault(x => x.PartStatusId == partStatusId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part status: {0} ", ex.ToString());
            }

            return partStatus;
        }

        /// <summary>
        /// get part status by nullable id
        /// </summary>
        /// <param name="partStatusId"></param>
        /// <returns></returns>
        public PartStatus GetPartStatus(Guid? partStatusId)
        {
            var partStatus = new PartStatus();

            try
            {
                partStatus = _db.PartStatus.FirstOrDefault(x => x.PartStatusId == partStatusId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part status: {0} ", ex.ToString());
            }

            return partStatus;
        }

        /// <summary>
        /// save part status
        /// </summary>
        /// <param name="newPartStatus"></param>
        /// <returns></returns>
        public OperationResult SavePartStatus(PartStatus newPartStatus)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingPartStatus = _db.PartStatus.FirstOrDefault(x => x.Description.ToLower() == newPartStatus.Description.ToLower());

                if (existingPartStatus == null)
                {
                    logger.Debug("PartStatus is being created...");

                    newPartStatus.IsActive = true;

                    _db.PartStatus.Add(newPartStatus);

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
                logger.ErrorFormat("Error saving new part status: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update part status
        /// </summary>
        /// <param name="partStatus"></param>
        /// <returns></returns>
        public OperationResult UpdatePartStatus(PartStatus partStatus)
        {
            var operationResult = new OperationResult();

            var existingPartStatus = GetPartStatus(partStatus.PartStatusId);

            if (existingPartStatus != null)
            {
                logger.Debug("PartStatus is being updated.");

                try
                {
                    _db.PartStatus.Attach(existingPartStatus);

                    _db.Entry(existingPartStatus).CurrentValues.SetValues(partStatus);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating part status: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected part status.";
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
