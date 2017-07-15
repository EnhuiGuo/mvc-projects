using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class SpecificationMaterialConverter
    {
        /// <summary>
        /// convert specification material to view model
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public SpecificationMaterialViewModel ConvertToView(SpecificationMaterial material)
        {
            SpecificationMaterialViewModel model = new SpecificationMaterialViewModel();

            model.SpecificationMaterialId = material.SpecificationMaterialId;
            model.SpecificationDescription = (!string.IsNullOrEmpty(material.Description)) ? material.Description : "N/A";
            model.IsActive = material.IsActive;

            return model;
        }

        /// <summary>
        /// convert specification material view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SpecificationMaterial ConvertToDomain(SpecificationMaterialViewModel model)
        {
            SpecificationMaterial material = new SpecificationMaterial();

            material.SpecificationMaterialId = model.SpecificationMaterialId;
            material.Description = model.SpecificationDescription;
            material.IsActive = model.IsActive;

            return material;
        }
    }
}