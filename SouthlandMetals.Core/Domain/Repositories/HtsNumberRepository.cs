using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class HtsNumberRepository : IHtsNumberRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public HtsNumberRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable htsNumbers
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableHtsNumbers()
        {
            var htsNumbers = new List<SelectListItem>();

            try
            {
                htsNumbers = _db.HtsNumber.Where(x => x.IsActive).Select(y => new SelectListItem()
                {
                    Text = y.Description + " " + (y.DutyRate * 100).ToString() + "%",
                    Value = y.HtsNumberId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting hts numbers: {0} ", ex.ToString());
            }

            return htsNumbers;
        }

        /// <summary>
        /// get all htsNumbers
        /// </summary>
        /// <returns></returns>
        public List<HtsNumber> GetHtsNumbers()
        {
            var htsNumbers = new List<HtsNumber>();

            try
            {
                htsNumbers = _db.HtsNumber.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting hts numbers: {0} ", ex.ToString());
            }

            return htsNumbers;
        }

        /// <summary>
        /// get htsNumber by id
        /// </summary>
        /// <param name="htsNumberId"></param>
        /// <returns></returns>
        public HtsNumber GetHtsNumber(Guid htsNumberId)
        {
            var htsNumber = new HtsNumber();

            try
            {
                htsNumber = _db.HtsNumber.FirstOrDefault(x => x.HtsNumberId == htsNumberId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting hts numbers: {0} ", ex.ToString());
            }

            return htsNumber;
        }

        /// <summary>
        /// get htsNumber by nullable id
        /// </summary>
        /// <param name="htsNumberId"></param>
        /// <returns></returns>
        public HtsNumber GetHtsNumber(Guid? htsNumberId)
        {
            var htsNumber = new HtsNumber();

            try
            {
                htsNumber = _db.HtsNumber.FirstOrDefault(x => x.HtsNumberId == htsNumberId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting hts number: {0} ", ex.ToString());
            }

            return htsNumber;
        }

        /// <summary>
        /// save htsNumber 
        /// </summary>
        /// <param name="newHtsNumber"></param>
        /// <returns></returns>
        public OperationResult SaveHtsNumber(HtsNumber newHtsNumber)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingHtsNumber = _db.HtsNumber.FirstOrDefault(x => x.Description.ToLower() == newHtsNumber.Description.ToLower());

                if (existingHtsNumber == null)
                {
                    logger.Debug("HtsNumber is being created...");

                    newHtsNumber.IsActive = true;

                    _db.HtsNumber.Add(newHtsNumber);

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
                logger.ErrorFormat("Error saving new hts number: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update htsNumber
        /// </summary>
        /// <param name="htsNumber"></param>
        /// <returns></returns>
        public OperationResult UpdateHtsNumber(HtsNumber htsNumber)
        {
            var operationResult = new OperationResult();

            var existingHtsNumber = GetHtsNumber(htsNumber.HtsNumberId);

            if (existingHtsNumber != null)
            {
                logger.Debug("HtsNumber is being updated.");

                try
                {
                    _db.HtsNumber.Attach(existingHtsNumber);

                    _db.Entry(existingHtsNumber).CurrentValues.SetValues(htsNumber);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating hts number: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected hts number.";
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
