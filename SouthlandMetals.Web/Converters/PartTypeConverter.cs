using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class PartTypeConverter
    {
        /// <summary>
        /// convert part type to view model
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public PartTypeViewModel ConvertToView(PartType type)
        {
            PartTypeViewModel model = new PartTypeViewModel();

            model.PartTypeId = type.PartTypeId;
            model.PartTypeDescription = (!string.IsNullOrEmpty(type.Description)) ? type.Description : "N/A";
            model.IsActive = type.IsActive;

            return model;
        }

        /// <summary>
        /// convert part type view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PartType ConvertToDomain(PartTypeViewModel model)
        {
            PartType type = new PartType();

            type.PartTypeId = model.PartTypeId;
            type.Description = model.PartTypeDescription;
            type.IsActive = model.IsActive;

            return type;
        }
    }
}