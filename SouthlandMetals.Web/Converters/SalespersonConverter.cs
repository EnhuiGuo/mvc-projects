using SouthlandMetals.Dynamics.Domain.Models;
using SouthlandMetals.Web.Areas.Administration.Models;
using SouthlandMetals.Web.Helpers;

namespace SouthlandMetals.Web.Converters
{
    public class SalespersonConverter
    {
        /// <summary>
        /// convert salesperson to view model
        /// </summary>
        /// <param name="salesperson"></param>
        /// <returns></returns>
        public SalespersonViewModel ConvertToView(RM00301_Salesperson salesperson)
        {
            SalespersonViewModel model = new SalespersonViewModel();

            model.SalespersonId = salesperson.SLPRSNID;
            model.SalespersonName = (!string.IsNullOrEmpty(salesperson.SLPRSNFN.Replace(" ", string.Empty)) && !string.IsNullOrEmpty(salesperson.SPRSNSLN.Replace(" ", string.Empty))) ? salesperson.SLPRSNFN + " " + salesperson.SPRSNSLN : "N/A";
            model.Phone1 = FormattingManager.FormatPhone(salesperson.PHONE1);
            model.FaxNumber = FormattingManager.FormatPhone(salesperson.FAX);
            model.SalesTerritoryDescription = salesperson.SALSTERR;
            model.IsActive = salesperson.INACTIVE != 1 ? true : false;

            return model;
        }
    }
}