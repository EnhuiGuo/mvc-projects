using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface IPackingListRepository : IDisposable
    {
        List<PackingList> GetPackingLists();

        List<PackingListPart> GetPackingListPartsByPackingList(Guid packingListId);

        PackingList GetPackingList(Guid packingListId);

        OperationResult SavePackingList(PackingList newPackingList);

        OperationResult UpdatePackingList(PackingList packingList);

        OperationResult ClosePackingList(Guid packingListId);

        OperationResult DeletePackingList(Guid packingListId);
    }
}
