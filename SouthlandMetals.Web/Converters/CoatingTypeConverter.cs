using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class CoatingTypeConverter
    {
        /// <summary>
        /// convert coatingType to view model
        /// </summary>
        /// <param name="coatingType"></param>
        /// <returns></returns>
        public CoatingTypeViewModel ConvertToView(CoatingType coatingType)
        {
            CoatingTypeViewModel model = new CoatingTypeViewModel();

            model.CoatingTypeId = coatingType.CoatingTypeId;
            model.CoatingTypeDescription = (!string.IsNullOrEmpty(coatingType.Description)) ? coatingType.Description : "N/A";
            model.IsActive = coatingType.IsActive;

            return model;
        }

        /// <summary>
        /// convert contingType model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CoatingType ConvertToDomain(CoatingTypeViewModel model)
        {
            CoatingType coatingType = new CoatingType();

            coatingType.CoatingTypeId = model.CoatingTypeId;
            coatingType.Description = model.CoatingTypeDescription;
            coatingType.IsActive = model.IsActive;

            return coatingType;
        }
    }
}