using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class SpecificationMaterialRepository : ISpecificationMaterialRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public SpecificationMaterialRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable specification materials
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableSpecificationMaterials()
        {
            var specificationMaterials = new List<SelectListItem>();

            try
            {
                specificationMaterials = _db.SpecificationMaterial.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.SpecificationMaterialId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting specification materials: {0} ", ex.ToString());
            }

            return specificationMaterials;
        }

        /// <summary>
        /// get specification materials
        /// </summary>
        /// <returns></returns>
        public List<SpecificationMaterial> GetSpecificationMaterials()
        {
            var specificationMaterials = new List<SpecificationMaterial>();

            try
            {
                specificationMaterials = _db.SpecificationMaterial.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting specification materials: {0} ", ex.ToString());
            }

            return specificationMaterials;
        }

        /// <summary>
        /// get specification material by id
        /// </summary>
        /// <param name="specificationMaterialId"></param>
        /// <returns></returns>
        public SpecificationMaterial GetSpecificationMaterial(Guid specificationMaterialId)
        {
            var specificationMaterial = new SpecificationMaterial();

            try
            {
                specificationMaterial = _db.SpecificationMaterial.FirstOrDefault(x => x.SpecificationMaterialId == specificationMaterialId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting specification material: {0} ", ex.ToString());
            }

            return specificationMaterial;
        }

        /// <summary>
        /// get specification material by nullable id
        /// </summary>
        /// <param name="specificationMaterialId"></param>
        /// <returns></returns>
        public SpecificationMaterial GetSpecificationMaterial(Guid? specificationMaterialId)
        {
            var specificationMaterial = new SpecificationMaterial();

            try
            {
                specificationMaterial = _db.SpecificationMaterial.FirstOrDefault(x => x.SpecificationMaterialId == specificationMaterialId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting specification material: {0} ", ex.ToString());
            }

            return specificationMaterial;
        }

        /// <summary>
        /// get specification material by description
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public SpecificationMaterial GetSpecificationMaterial(string description)
        {
            var specificationMaterial = new SpecificationMaterial();

            try
            {
                specificationMaterial = _db.SpecificationMaterial.FirstOrDefault(x => x.Description == description);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting specification material: {0} ", ex.ToString());
            }

            return specificationMaterial;
        }

        /// <summary>
        /// save specification material
        /// </summary>
        /// <param name="newSpecificationMaterial"></param>
        /// <returns></returns>
        public OperationResult SaveSpecificationMaterial(SpecificationMaterial newSpecificationMaterial)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingHtsNumber = _db.SpecificationMaterial.FirstOrDefault(x => x.Description.ToLower() == newSpecificationMaterial.Description.ToLower());

                if (existingHtsNumber == null)
                {
                    logger.Debug("Specification Material is being created...");

                    newSpecificationMaterial.IsActive = true;

                    _db.SpecificationMaterial.Add(newSpecificationMaterial);

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
                logger.ErrorFormat("Error saving new specification material: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update specification matrial
        /// </summary>
        /// <param name="specificationMaterial"></param>
        /// <returns></returns>
        public OperationResult UpdateSpecificationMaterial(SpecificationMaterial specificationMaterial)
        {
            var operationResult = new OperationResult();

            var existingSpecificationMaterial = GetSpecificationMaterial(specificationMaterial.SpecificationMaterialId);

            if (existingSpecificationMaterial != null)
            {
                logger.Debug("Specification Material is being updated.");

                try
                {
                    _db.SpecificationMaterial.Attach(existingSpecificationMaterial);

                    _db.Entry(existingSpecificationMaterial).CurrentValues.SetValues(specificationMaterial);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating spcification material: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected spcification material.";
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
