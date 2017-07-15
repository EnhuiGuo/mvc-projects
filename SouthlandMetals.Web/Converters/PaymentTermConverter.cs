using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class PaymentTermConverter
    {
        /// <summary>
        /// convert payment term to view madel
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public PaymentTermViewModel ConvertToView(PaymentTerm term)
        {
            PaymentTermViewModel model = new PaymentTermViewModel();

            model.PaymentTermId = term.PaymentTermId;
            model.PaymentTermDescription = (!string.IsNullOrEmpty(term.Description)) ? term.Description : "N/A";
            model.IsActive = term.IsActive;

            return model;
        }

        /// <summary>
        /// convert payment term view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PaymentTerm ConvertToDomain(PaymentTermViewModel model)
        {
            PaymentTerm term = new PaymentTerm();

            term.PaymentTermId = model.PaymentTermId;
            term.Description = model.PaymentTermDescription;
            term.IsActive = model.IsActive;

            return term;
        }
    }
}