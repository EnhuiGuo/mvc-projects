using SouthlandMetals.Common.Models;
using SouthlandMetals.Dynamics.Domain.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Dynamics.Domain.Interfaces
{
    public interface ISiteDynamicsRepository : IDisposable
    {
        List<SelectListItem> GetSelectableSites();

        List<SelectListItem> GetSelectableSites(string customerId);

        List<IV40700_Site> GetSites();

        IV40700_Site GetSite(string locationCode);

        OperationResult SaveSite();

        OperationResult UpdateSite(IV40700_Site site);
    }
}
