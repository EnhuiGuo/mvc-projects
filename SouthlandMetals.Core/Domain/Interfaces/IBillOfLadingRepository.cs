using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface IBillOfLadingRepository : IDisposable
    {
        List<BillOfLading> GetBillOfLadings();

        BillOfLading GetBillOfLading(Guid billOfLadingId);

        BillOfLading GetBillOfLading(string bolNumber);

        OperationResult SaveBillOfLading(BillOfLading newBillOfLading);

        OperationResult UpdateBillOfLading(BillOfLading billOfLading);

        OperationResult DeleteBillOfLading(Guid billOfLadingId);
    }
}
