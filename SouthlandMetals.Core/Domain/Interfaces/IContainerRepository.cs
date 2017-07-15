using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface IContainerRepository : IDisposable
    {
         List<SelectListItem> GetSelectableContainers();

         List<Container> GetContainers();

         List<ContainerPart> GetContainerParts();

         Container GetContainer(Guid containerId);

         Container GetContainer(string containerNumber);

         ContainerPart GetContainerPart(Guid containerPartId);
    }
}
