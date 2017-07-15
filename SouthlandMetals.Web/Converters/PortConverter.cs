using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class PortConverter
    {
        /// <summary>
        /// convert port to view model
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public PortViewModel ConvertToView(Port port)
        {
            PortViewModel model = new PortViewModel();

            model.PortId = port.PortId;
            model.PortName = (!string.IsNullOrEmpty(port.Name)) ? port.Name : "N/A";
            model.IsActive = port.IsActive;

            return model;
        }

        /// <summary>
        /// convert port view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Port ConvertToDomain(PortViewModel model)
        {
            Port port = new Port();

            port.PortId = model.PortId;
            port.Name = model.PortName;
            port.IsActive = model.IsActive;

            return port;
        }
    }
}