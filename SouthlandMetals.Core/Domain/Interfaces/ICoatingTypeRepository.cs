using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface ICoatingTypeRepository : IDisposable
    {
        List<SelectListItem> GetSelectableCoatingTypes();

        List<CoatingType> GetCoatingTypes();

        CoatingType GetCoatingType(Guid coatingTypeId);

        CoatingType GetCoatingType(Guid? coatingTypeId);

        CoatingType GetCoatingType(string description);

        OperationResult SaveCoatingType(CoatingType newCoatingType);

        OperationResult UpdateCoatingType(CoatingType coatingType);
    }
}
