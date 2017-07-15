using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class CoatingTypeRepository : ICoatingTypeRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public CoatingTypeRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable coating types
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableCoatingTypes()
        {
            var coatingTypes = new List<SelectListItem>();

            try
            {
                coatingTypes = _db.CoatingType.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.CoatingTypeId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting coating types: {0} ", ex.ToString());
            }

            return coatingTypes;
        }

        /// <summary>
        /// get all coating types
        /// </summary>
        /// <returns></returns>
        public List<CoatingType> GetCoatingTypes()
        {
            var coatingTypes = new List<CoatingType>();

            try
            {
                coatingTypes = _db.CoatingType.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting coating types: {0} ", ex.ToString());
            }

            return coatingTypes;
        }

        /// <summary>
        /// get coating type by id
        /// </summary>
        /// <param name="coatingTypeId"></param>
        /// <returns></returns>
        public CoatingType GetCoatingType(Guid coatingTypeId)
        {
            var coatingType = new CoatingType();

            try
            {
                coatingType = _db.CoatingType.FirstOrDefault(x => x.CoatingTypeId == coatingTypeId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting coating type: {0} ", ex.ToString());
            }

            return coatingType;
        }

        /// <summary>
        /// get coating type by nullable id
        /// </summary>
        /// <param name="coatingTypeId"></param>
        /// <returns></returns>
        public CoatingType GetCoatingType(Guid? coatingTypeId)
        {
            var coatingType = new CoatingType();

            try
            {
                coatingType = _db.CoatingType.FirstOrDefault(x => x.CoatingTypeId == coatingTypeId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting coating type: {0} ", ex.ToString());
            }

            return coatingType;
        }

        /// <summary>
        /// get coating type by description
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public CoatingType GetCoatingType(string description)
        {
            var coatingType = new CoatingType();

            try
            {
                coatingType = _db.CoatingType.FirstOrDefault(x => x.Description.Replace(" ", string.Empty).ToLower() == description.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting coating type: {0} ", ex.ToString());
            }

            return coatingType;
        }

        /// <summary>
        /// save new coating type
        /// </summary>
        /// <param name="newCoatingType"></param>
        /// <returns></returns>
        public OperationResult SaveCoatingType(CoatingType newCoatingType)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingCoatingType = _db.CoatingType.FirstOrDefault(x => x.Description.ToLower() == newCoatingType.Description.ToLower());

                if (existingCoatingType == null)
                {
                    logger.Debug("CoatingType is being created...");

                    newCoatingType.IsActive = true;

                    _db.CoatingType.Add(newCoatingType);

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
                logger.ErrorFormat("Error saving new coating type: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update coating type
        /// </summary>
        /// <param name="coatingType"></param>
        /// <returns></returns>
        public OperationResult UpdateCoatingType(CoatingType coatingType)
        {
            var operationResult = new OperationResult();

            var existingCoatingType = GetCoatingType(coatingType.CoatingTypeId);

            if (existingCoatingType != null)
            {
                logger.Debug("CoatingType is being updated.");

                try
                {
                    _db.CoatingType.Attach(existingCoatingType);

                    _db.Entry(existingCoatingType).CurrentValues.SetValues(coatingType);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating coating type: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected coating type.";
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
