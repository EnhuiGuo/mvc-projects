using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class VesselConverter
    {
        /// <summary>
        /// convert vessel to view model
        /// </summary>
        /// <param name="vessel"></param>
        /// <returns></returns>
        public VesselViewModel ConvertToView(Vessel vessel)
        {
            VesselViewModel model = new VesselViewModel();

            model.VesselId = vessel.VesselId;
            model.VesselName = (!string.IsNullOrEmpty(vessel.Name)) ? vessel.Name : "N/A";
            model.IsActive = vessel.IsActive;

            return model;
        }

        /// <summary>
        /// convert vessel view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Vessel ConvertToDomain(VesselViewModel model)
        {
            Vessel vessel = new Vessel();

            vessel.VesselId = model.VesselId;
            vessel.Name = model.VesselName;
            vessel.IsActive = model.IsActive;

            return vessel;
        }
    }
}