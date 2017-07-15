using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class PartTypeRepository : IPartTypeRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public PartTypeRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable part types
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectablePartTypes()
        {
            var partTypes = new List<SelectListItem>();

            try
            {
                partTypes = _db.PartType.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.PartTypeId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part types: {0} ", ex.ToString());
            }

            return partTypes;
        }

        /// <summary>
        /// get all part types
        /// </summary>
        /// <returns></returns>
        public List<PartType> GetPartTypes()
        {
            var partTypes = new List<PartType>();

            try
            {
                partTypes = _db.PartType.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part types: {0} ", ex.ToString());
            }

            return partTypes;
        }

        /// <summary>
        /// get part type by id 
        /// </summary>
        /// <param name="partTypeId"></param>
        /// <returns></returns>
        public PartType GetPartType(Guid partTypeId)
        {
            var partType = new PartType();

            try
            {
                partType = _db.PartType.FirstOrDefault(x => x.PartTypeId == partTypeId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part type: {0} ", ex.ToString());
            }

            return partType;
        }

        /// <summary>
        /// get part type by null id 
        /// </summary>
        /// <param name="partTypeId"></param>
        /// <returns></returns>
        public PartType GetPartType(Guid? partTypeId)
        {
            var partType = new PartType();

            try
            {
                partType = _db.PartType.FirstOrDefault(x => x.PartTypeId == partTypeId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part type: {0} ", ex.ToString());
            }

            return partType;
        }

        /// <summary>
        /// save part type
        /// </summary>
        /// <param name="newPartType"></param>
        /// <returns></returns>
        public OperationResult SavePartType(PartType newPartType)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingPartType = _db.PartType.FirstOrDefault(x => x.Description.ToLower() == newPartType.Description.ToLower());

                if (existingPartType == null)
                {
                    logger.Debug("PartType is being created...");

                    newPartType.IsActive = true;

                    _db.PartType.Add(newPartType);

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
                logger.ErrorFormat("Error saving new part type: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update part type
        /// </summary>
        /// <param name="partType"></param>
        /// <returns></returns>
        public OperationResult UpdatePartType(PartType partType)
        {
            var operationResult = new OperationResult();

            var existingPartType = GetPartType(partType.PartTypeId);

            if (existingPartType != null)
            {
                logger.Debug("PartType is being updated.");

                try
                {
                    _db.PartType.Attach(existingPartType);

                    _db.Entry(existingPartType).CurrentValues.SetValues(partType);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating part type: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected part type.";
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
