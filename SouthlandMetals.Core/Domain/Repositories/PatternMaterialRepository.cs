using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class PatternMaterialRepository : IPatternMaterialRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public PatternMaterialRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable parttern materials
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectablePatternMaterials()
        {
            var patternMaterials = new List<SelectListItem>();

            try
            {
                patternMaterials = _db.PatternMaterial.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.PatternMaterialId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting pattern materials: {0} ", ex.ToString());
            }

            return patternMaterials;
        }

        /// <summary>
        /// get all parttern materials
        /// </summary>
        /// <returns></returns>
        public List<PatternMaterial> GetPatternMaterials()
        {
            var patternMaterials = new List<PatternMaterial>();

            try
            {
                patternMaterials = _db.PatternMaterial.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting pattern materials: {0} ", ex.ToString());
            }

            return patternMaterials;
        }

        /// <summary>
        /// get pattern material
        /// </summary>
        /// <param name="patternMaterialId"></param>
        /// <returns></returns>
        public PatternMaterial GetPatternMaterial(Guid patternMaterialId)
        {
            var patternMaterial = new PatternMaterial();

            try
            {
                patternMaterial = _db.PatternMaterial.FirstOrDefault(x => x.PatternMaterialId == patternMaterialId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting pattern material: {0} ", ex.ToString());
            }

            return patternMaterial;
        }

        /// <summary>
        /// save parttern material
        /// </summary>
        /// <param name="newPatternMaterial"></param>
        /// <returns></returns>
        public OperationResult SavePatternMaterial(PatternMaterial newPatternMaterial)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingPatternMaterial = _db.PatternMaterial.FirstOrDefault(x => x.Description.ToLower() == newPatternMaterial.Description.ToLower());

                if (existingPatternMaterial == null)
                {
                    logger.Debug("PatternMaterial is being created...");

                    newPatternMaterial.IsActive = true;

                    _db.PatternMaterial.Add(newPatternMaterial);

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
                logger.ErrorFormat("Error saving new pattern material: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update parttern material
        /// </summary>
        /// <param name="patternMaterial"></param>
        /// <returns></returns>
        public OperationResult UpdatePatternMaterial(PatternMaterial patternMaterial)
        {
            var operationResult = new OperationResult();

            var existingPatternMaterial = GetPatternMaterial(patternMaterial.PatternMaterialId);

            if (existingPatternMaterial != null)
            {
                logger.Debug("PatternMaterial is being updated.");

                try
                {
                    _db.PatternMaterial.Attach(existingPatternMaterial);

                    _db.Entry(existingPatternMaterial).CurrentValues.SetValues(patternMaterial);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating pattern material: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected pattern material.";
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
