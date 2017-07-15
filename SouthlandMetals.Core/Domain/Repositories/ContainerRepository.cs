using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class ContainerRepository : IContainerRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public ContainerRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable containers
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableContainers()
        {
            var containers = new List<SelectListItem>();

            try
            {
                containers = _db.Container.Select(y => new SelectListItem()
                {
                    Text = y.Number,
                    Value = y.ContainerId.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting containers: {0} ", ex.ToString());
            }

            return containers;
        }

        /// <summary>
        /// get all containers
        /// </summary>
        /// <returns></returns>
        public List<Container> GetContainers()
        {
            var containers = new List<Container>();

            try
            {
                containers = _db.Container.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting containers: {0} ", ex.ToString());
            }

            return containers;
        }

        /// <summary>
        /// get container parts
        /// </summary>
        /// <returns></returns>
        public List<ContainerPart> GetContainerParts()
        {
            var containerParts = new List<ContainerPart>();

            try
            {
                containerParts = _db.ContainerPart.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting container parts: {0} ", ex.ToString());
            }

            return containerParts;
        }

        /// <summary>
        /// get container by id
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns></returns>
        public Container GetContainer(Guid containerId)
        {
            var container = new Container();

            try
            {
                container = _db.Container.FirstOrDefault(x => x.ContainerId == containerId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting container: {0} ", ex.ToString());
            }

            return container;
        }

        /// <summary>
        /// get container number
        /// </summary>
        /// <param name="containerNumber"></param>
        /// <returns></returns>
        public Container GetContainer(string containerNumber)
        {
            var container = new Container();

            try
            {
                container = _db.Container.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == containerNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting container: {0} ", ex.ToString());
            }

            return container;
        }

        /// <summary>
        /// get container part by id
        /// </summary>
        /// <param name="containerPartId"></param>
        /// <returns></returns>
        public ContainerPart GetContainerPart(Guid containerPartId)
        {
            var containerPart = new ContainerPart();

            try
            {
                containerPart = _db.ContainerPart.FirstOrDefault(x => x.ContainerPartId == containerPartId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting container part: {0} ", ex.ToString());
            }

            return containerPart;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
