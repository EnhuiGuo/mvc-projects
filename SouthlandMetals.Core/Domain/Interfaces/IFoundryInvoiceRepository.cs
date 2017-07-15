using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface IFoundryInvoiceRepository : IDisposable
    {
        List<SelectListItem> GetSelectableFoundryInvoices();

        List<SelectListItem> GetSelectableFoundryInvoicesByFoundry(string foundryId);

        List<FoundryInvoice> GetFoundryInvoices();

        FoundryInvoice GetFoundryInvoice(Guid foundryInvoiceId);

        FoundryInvoice GetFoundryInvoice(string foundryInvoiceNumber);

        FoundryInvoice GetFoundryInvoiceByBillOfLading(Guid billOfLadingId);

        OperationResult SaveFoundryInvoice(FoundryInvoice newFoundryInvoice);

        OperationResult UpdateFoundryInvoice(FoundryInvoice foundryInvoice);
    }
}
