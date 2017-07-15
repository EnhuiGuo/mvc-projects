using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class MaterialConverter
    {
        /// <summary>
        /// convert material to view model
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public MaterialViewModel ConvertToView(Material material)
        {
            MaterialViewModel model = new MaterialViewModel();

            model.MaterialId = material.MaterialId;
            model.MaterialDescription = (!string.IsNullOrEmpty(material.Description)) ? material.Description : "N/A";
            model.IsActive = material.IsActive;

            return model;
        }

        /// <summary>
        /// convert material view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Material ConvertToDomain(MaterialViewModel model)
        {
            Material material = new Material();

            material.MaterialId = model.MaterialId;
            material.Description = model.MaterialDescription;
            material.IsActive = model.IsActive;

            return material;
        }
    }
}