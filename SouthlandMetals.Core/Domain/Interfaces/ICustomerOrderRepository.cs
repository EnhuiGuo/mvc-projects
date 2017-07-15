using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface ICustomerOrderRepository : IDisposable
    {
        List<SelectListItem> GetSelectableCustomerOrdersByPart(Guid partId);

        List<CustomerOrder> GetCustomerOrders();

        List<CustomerOrderPart> GetCustomerOrderParts();

        CustomerOrder GetCustomerOrder(Guid customerOrderId);

        CustomerOrder GetCustomerOrder(string poNumber);

        CustomerOrderPart GetCustomerOrderPart(Guid customerOrderPartId);

        CustomerOrderPart GetCustomerOrderPartByPriceSheetPart(Guid priceSheetPartId);

        OperationResult SaveCustomerOrder(CustomerOrder newCustomerOrder);

        OperationResult UpdateCustomerOrder(CustomerOrder customerOrder);

        OperationResult UpdateCustomerOrderPart(CustomerOrderPart customerOrderPart);

        OperationResult DeleteCustomerOrder(Guid customerOrderId);
    }
}
