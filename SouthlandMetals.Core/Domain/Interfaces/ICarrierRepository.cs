using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface ICarrierRepository : IDisposable
    {
        List<SelectListItem> GetSelectableCarriers();

        List<Carrier> GetCarriers();

        Carrier GetCarrier(Guid carrierId);

        OperationResult SaveCarrier(Carrier carrier);

        OperationResult UpdateCarrier(Carrier carrier);
    }
}
