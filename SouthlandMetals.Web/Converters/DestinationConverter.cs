using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class DestinationConverter
    {
        /// <summary>
        /// convert destination to view model
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public DestinationViewModel ConvertToView(Destination destination)
        {
            DestinationViewModel model = new DestinationViewModel();

            model.DestinationId = destination.DestinationId;
            model.DestinationDescription = (!string.IsNullOrEmpty(destination.Description)) ? destination.Description : "N/A";
            model.IsActive = destination.IsActive;

            return model;
        }

        /// <summary>
        /// convert destination view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Destination ConvertToDomain(DestinationViewModel model)
        {
            Destination destination = new Destination();

            destination.DestinationId = model.DestinationId;
            destination.Description = model.DestinationDescription;
            destination.IsActive = model.IsActive;

            return destination;
        }
    }
}