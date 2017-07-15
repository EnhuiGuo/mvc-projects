using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class ContainerConverter
    {
        /// <summary>
        /// convert container to view model
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public ContainerViewModel ConvertToView(Container container)
        {
            ContainerViewModel model = new ContainerViewModel();

            var _containerRepository = new ContainerRepository();

            var containerParts = _containerRepository.GetContainerParts().Where(x => x.ContainerId == container.ContainerId).ToList();

            model.ContainerId = container.ContainerId;
            model.BillOfLadingId = container.BillOfLadingId;
            model.ContainerNumber = (!string.IsNullOrEmpty(container.Number)) ? container.Number : "N/A";

            if (containerParts != null && containerParts.Count > 0)
            {
                model.ContainerParts = new List<ContainerPartViewModel>();

                foreach (var containerPart in containerParts)
                {
                    ContainerPartViewModel convertedModel = new ContainerPartConverter().ConvertToView(containerPart);

                    model.ContainerParts.Add(convertedModel);
                }
            }

            if (_containerRepository != null)
            {
                _containerRepository.Dispose();
                _containerRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert container view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Container ConvertToDomain(ContainerViewModel model)
        {
            Container container = new Container();

            container.ContainerId = model.ContainerId;
            container.BillOfLadingId = model.BillOfLadingId;
            container.Number = model.ContainerNumber;

            if (model.ContainerParts != null && model.ContainerParts.Count > 0)
            {
                var containerParts = new List<ContainerPart>();

                foreach (var containerPart in model.ContainerParts)
                {
                    ContainerPart convertedModel = new ContainerPartConverter().ConvertToDomain(containerPart);
                    containerParts.Add(convertedModel);
                }

                container.ContainerParts = containerParts;
            }

            return container;
        }
    }
}