using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class MaterialRepository : IMaterialRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public MaterialRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable materials
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableMaterials()
        {
            var materials = new List<SelectListItem>();

            try
            {
                materials = _db.Material.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description,
                    Value = y.MaterialId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting materials: {0} ", ex.ToString());
            }

            return materials;
        }

        /// <summary>
        /// get all materials
        /// </summary>
        /// <returns></returns>
        public List<Material> GetMaterials()
        {
            var materials = new List<Material>();

            try
            {
                materials = _db.Material.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting materials: {0} ", ex.ToString());
            }

            return materials;
        }

        /// <summary>
        /// get material by id
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public Material GetMaterial(Guid materialId)
        {
            var material = new Material();

            try
            {
                material = _db.Material.FirstOrDefault(x => x.MaterialId == materialId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting material: {0} ", ex.ToString());
            }

            return material;
        }

        /// <summary>
        /// get material by nullable id
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public Material GetMaterial(Guid? materialId)
        {
            var material = new Material();

            try
            {
                material = _db.Material.FirstOrDefault(x => x.MaterialId == materialId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting material: {0} ", ex.ToString());
            }

            return material;
        }

        /// <summary>
        /// save new material 
        /// </summary>
        /// <param name="newMaterial"></param>
        /// <returns></returns>
        public OperationResult SaveMaterial(Material newMaterial)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingMaterial = _db.Material.FirstOrDefault(x => x.Description.ToLower() == newMaterial.Description.ToLower());

                if (existingMaterial == null)
                {
                    logger.Debug("Material is being created...");

                    newMaterial.IsActive = true;

                    _db.Material.Add(newMaterial);

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
                logger.ErrorFormat("Error saving new material: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update material
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public OperationResult UpdateMaterial(Material material)
        {
            var operationResult = new OperationResult();

            var existingMaterial = GetMaterial(material.MaterialId);

            if (existingMaterial != null)
            {
                logger.Debug("Material is being updated.");

                try
                {
                    _db.Material.Attach(existingMaterial);

                    _db.Entry(existingMaterial).CurrentValues.SetValues(material);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating material: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected material.";
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
