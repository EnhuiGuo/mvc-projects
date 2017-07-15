using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class PartStatusConverter
    {
        /// <summary>
        /// convert part status to view model
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public PartStatusViewModel ConvertToView(PartStatus status)
        {
            PartStatusViewModel model = new PartStatusViewModel();

            model.PartStatusId = status.PartStatusId;
            model.PartStatusDescription = (!string.IsNullOrEmpty(status.Description)) ? status.Description : "N/A";
            model.IsActive = status.IsActive;

            return model;
        }

        /// <summary>
        /// convert part status view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PartStatus ConvertToDomain(PartStatusViewModel model)
        {
            PartStatus status = new PartStatus();

            status.PartStatusId = model.PartStatusId;
            status.Description = model.PartStatusDescription;
            status.IsActive = model.IsActive;

            return status;
        }
    }
}