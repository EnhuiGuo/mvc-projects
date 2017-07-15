using SouthlandMetals.Web.Areas.Operations.Models;
using System.Collections.Generic;

namespace SouthlandMetals.Web.Models
{
    public class DashboardViewModel
    {
        public List<PackingListViewModel> OpenPackingLists { get; set; }
        public List<ProjectViewModel> OnHoldProjects { get; set; }
        public List<RfqViewModel> OnHoldRfqs { get; set; }
        public List<QuoteViewModel> OnHoldQuotes { get; set; }
        public List<CustomerOrderViewModel> OnHoldCustomerOrders { get; set; }
        public List<FoundryOrderViewModel> OnHoldFoundryOrders { get; set; }
        public List<FoundryInvoiceViewModel> NeedScheduledFoundryInvoices { get; set; }
    }
}