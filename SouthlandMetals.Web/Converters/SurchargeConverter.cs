using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class SurchargeConverter 
    {
        /// <summary>
        /// convert surchage to view model
        /// </summary>
        /// <param name="surcharge"></param>
        /// <returns></returns>
        public SurchargeViewModel ConvertToView(Surcharge surcharge)
        {
            SurchargeViewModel model = new SurchargeViewModel();

            model.SurchargeId = surcharge.SurchargeId;
            model.SurchargeDescription = (!string.IsNullOrEmpty(surcharge.Description)) ? surcharge.Description : "N/A";
            model.IsActive = surcharge.IsActive;

            return model;
        }

        /// <summary>
        /// convert surcharge view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Surcharge ConvertToDomain(SurchargeViewModel model)
        {
            Surcharge surcharge = new Surcharge();

            surcharge.SurchargeId = model.SurchargeId;
            surcharge.Description = model.SurchargeDescription;
            surcharge.IsActive = model.IsActive;

            return surcharge;
        }
    }
}