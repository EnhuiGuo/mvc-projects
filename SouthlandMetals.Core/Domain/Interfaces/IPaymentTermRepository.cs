using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface IPaymentTermRepository : IDisposable
    {
        List<SelectListItem> GetSelectablePaymentTerms();

        List<PaymentTerm> GetPaymentTerms();

        PaymentTerm GetPaymentTerm(Guid paymentTermId);

        PaymentTerm GetPaymentTerm(Guid? paymentTermId);

        PaymentTerm GetPaymentTerm(string paymentTermId);

        OperationResult SavePaymentTerm(PaymentTerm newPaymentTerm);

        OperationResult UpdatePaymentTerm(PaymentTerm paymentTerm);
    }
}
