using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class PortRepository : IPortRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public PortRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable ports
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectablePorts()
        {
            var ports = new List<SelectListItem>();

            try
            {
                ports = _db.Port.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.PortId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting ports: {0} ", ex.ToString());
            }

            return ports;
        }

        /// <summary>
        /// get all ports
        /// </summary>
        /// <returns></returns>
        public List<Port> GetPorts()
        {
            var ports = new List<Port>();

            try
            {
                ports = _db.Port.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting ports: {0} ", ex.ToString());
            }

            return ports;
        }

        /// <summary>
        /// get port by id
        /// </summary>
        /// <param name="portId"></param>
        /// <returns></returns>
        public Port GetPort(Guid portId)
        {
            var port = new Port();

            try
            {
                port = _db.Port.FirstOrDefault(x => x.PortId == portId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting port: {0} ", ex.ToString());
            }

            return port;
        }

        /// <summary>
        /// save port
        /// </summary>
        /// <param name="newPort"></param>
        /// <returns></returns>
        public OperationResult SavePort(Port newPort)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingPort = _db.Port.FirstOrDefault(x => x.Name.ToLower() == newPort.Name.ToLower());

                if (existingPort == null)
                {
                    logger.Debug("Port is being created...");

                    newPort.IsActive = true;

                    _db.Port.Add(newPort);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                else
                {
                    operationResult.Success = false;
                    operationResult.Message = "Duplicate Entry";
                }
            }
            catch (Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = "Error";
                logger.ErrorFormat("Error saving new port: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public OperationResult UpdatePort(Port port)
        {
            var operationResult = new OperationResult();

            var existingPort = GetPort(port.PortId);

            if (existingPort != null)
            {
                logger.Debug("Port is being updated.");

                try
                {
                    _db.Port.Attach(existingPort);

                    _db.Entry(existingPort).CurrentValues.SetValues(port);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating port: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected port.";
            }

            return operationResult;
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
