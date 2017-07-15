using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class PatternMaterialConverter
    {
        /// <summary>
        /// convert pattern material to view model
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public PatternMaterialViewModel ConvertToView(PatternMaterial material)
        {
            PatternMaterialViewModel model = new PatternMaterialViewModel();

            model.PatternMaterialId = material.PatternMaterialId;
            model.PatternDescription = (!string.IsNullOrEmpty(material.Description)) ? material.Description : "N/A";
            model.IsActive = material.IsActive;

            return model;
        }

        /// <summary>
        /// convert part material view madel to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PatternMaterial ConvertToDomain(PatternMaterialViewModel model)
        {
            PatternMaterial material = new PatternMaterial();

            material.PatternMaterialId = model.PatternMaterialId;
            material.Description = model.PatternDescription;
            material.IsActive = model.IsActive;

            return material;
        }
    }
}