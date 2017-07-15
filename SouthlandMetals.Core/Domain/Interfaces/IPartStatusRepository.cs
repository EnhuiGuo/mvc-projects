using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface IPartStatusRepository : IDisposable
    {
        List<SelectListItem> GetSelectablePartStates();

        List<PartStatus> GetPartStates();

        PartStatus GetPartStatus(Guid partStatusId);

        PartStatus GetPartStatus(Guid? partStatusId);

        OperationResult SavePartStatus(PartStatus newPartStatus);

        OperationResult UpdatePartStatus(PartStatus partStatus);
    }
}
