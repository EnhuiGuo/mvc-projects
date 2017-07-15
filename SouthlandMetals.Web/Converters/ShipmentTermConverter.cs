using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class ShipmentTermConverter
    {
        /// <summary>
        /// convert shipment term to view model
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public ShipmentTermViewModel ConvertToView(ShipmentTerm term)
        {
            ShipmentTermViewModel model = new ShipmentTermViewModel();

            model.ShipmentTermId = term.ShipmentTermId;
            model.ShipmentTermDescription = (!string.IsNullOrEmpty(term.Description)) ? term.Description : "N/A";
            model.IsActive = term.IsActive;

            return model;
        }

        /// <summary>
        /// convert shipment term view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ShipmentTerm ConvertToDomain(ShipmentTermViewModel model)
        {
            ShipmentTerm term = new ShipmentTerm();

            term.ShipmentTermId = model.ShipmentTermId;
            term.Description = model.ShipmentTermDescription;
            term.IsActive = model.IsActive;

            return term;
        }
    }
}