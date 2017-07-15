using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class PartRepository : IPartRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public PartRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable parts
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableParts()
        {
            var parts = new List<SelectListItem>();

            try
            {
                parts = (from p in _db.Part
                         join s in _db.PartStatus
                         on p.PartStatusId equals s.PartStatusId
                         where s.IsActive == true
                         select new SelectListItem()
                         {
                             Value = p.PartId.ToString(),
                             Text = p.Number
                         }).OrderBy(z => z.Text).ToList();

                return parts;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting parts: {0} ", ex.ToString());

                return parts;
            }
        }

        /// <summary>
        /// get selectable parts by customer 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectablePartsByCustomer(string customerId)
        {
            var parts = new List<SelectListItem>();

            try
            {
                parts = (from p in _db.Part
                         join s in _db.PartStatus
                         on p.PartStatusId equals s.PartStatusId
                         where p.CustomerId.Replace(" ", string.Empty).ToLower() == customerId.Replace(" ", string.Empty).ToLower() &&
                         s.IsActive == true
                         select new SelectListItem()
                         {
                             Value = p.PartId.ToString(),
                             Text = p.Number
                         }).OrderBy(z => z.Text).ToList();

                return parts;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting parts by customer: {0} ", ex.ToString());

                return parts;
            }
        }

        /// <summary>
        /// get selectable parts by customer and foundry 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="foundryId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectablePartsByCustomerAndFoundry(string customerId, string foundryId)
        {
            var parts = new List<SelectListItem>();

            try
            {
                parts = (from p in _db.Part
                         join s in _db.PartStatus
                         on p.PartStatusId equals s.PartStatusId
                         where p.CustomerId.Replace(" ", string.Empty).ToLower() == customerId.Replace(" ", string.Empty).ToLower() &&
                               p.FoundryId.Replace(" ", string.Empty).ToLower() == foundryId.Replace(" ", string.Empty).ToLower() &&
                               s.IsActive == true
                         select new SelectListItem()
                         {
                             Value = p.PartId.ToString(),
                             Text = p.Number
                         }).OrderBy(z => z.Text).ToList();

                return parts;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting parts by customer: {0} ", ex.ToString());

                return parts;
            }
        }

        /// <summary>
        /// get all parts
        /// </summary>
        /// <returns></returns>
        public List<Part> GetParts()
        {
            var parts = new List<Part>();

            try
            {
                parts = _db.Part.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting parts: {0} ", ex.ToString());
            }

            return parts;
        }

        /// <summary>
        /// get part drawings 
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public List<PartDrawing> GetPartDrawings(Guid partId)
        {
            var drawings = new List<PartDrawing>();

            try
            {
                drawings = _db.PartDrawing.Where(x => x.PartId == partId).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part drawings: {0} ", ex.ToString());
            }

            return drawings;
        }

        /// <summary>
        /// get part layouts
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public List<PartLayout> GetPartLayouts(Guid partId)
        {
            var layouts = new List<PartLayout>();

            try
            {
                layouts = _db.PartLayout.Where(x => x.PartId == partId).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part layouts: {0} ", ex.ToString());
            }

            return layouts;
        }

        /// <summary>
        /// get part by id 
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public Part GetPart(Guid partId)
        {
            var part = new Part();

            try
            {
                part = _db.Part.FirstOrDefault(x => x.PartId == partId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get part by nullable id
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public Part GetPart(Guid? partId)
        {
            var part = new Part();

            try
            {
                part = _db.Part.FirstOrDefault(x => x.PartId == partId);

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get part by number 
        /// </summary>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public Part GetPart(string partNumber)
        {
            var part = new Part();

            try
            {
                part = _db.Part.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == partNumber.Replace(" ", string.Empty).ToLower());

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get part drawing by id
        /// </summary>
        /// <param name="partDrawingId"></param>
        /// <returns></returns>
        public PartDrawing GetPartDrawing(Guid partDrawingId)
        {
            var drawing = new PartDrawing();

            try
            {
                drawing = _db.PartDrawing.Find(partDrawingId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part drawing: {0} ", ex.ToString());
            }

            return drawing;
        }

        /// <summary>
        /// get part layout
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        public PartLayout GetPartLayout(Guid layoutId)
        {
            var layout = new PartLayout();

            try
            {
                layout = _db.PartLayout.Find(layoutId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part layout: {0} ", ex.ToString());
            }

            return layout;
        }

        /// <summary>
        /// save part
        /// </summary>
        /// <param name="newPart"></param>
        /// <returns></returns>
        public OperationResult SavePart(Part newPart)
        {
            var operationResult = new OperationResult();

            try
            {
                var insertedPart = _db.Part.Add(newPart);

                _db.SaveChanges();

                operationResult.Success = true;
                operationResult.Message = "Success";
                operationResult.ReferenceId = insertedPart.PartId;
            }
            catch (Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = "Error";
                logger.ErrorFormat("Error saving new part: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// save part drawing
        /// </summary>
        /// <param name="newPartDrawing"></param>
        /// <returns></returns>
        public OperationResult SavePartDrawing(PartDrawing newPartDrawing)
        {
            var operationResult = new OperationResult();

            var existingDrawing = _db.PartDrawing.FirstOrDefault(x => x.RevisionNumber.ToLower() == newPartDrawing.RevisionNumber.ToLower() && x.PartId == newPartDrawing.PartId);

            try
            {
                if (existingDrawing == null)
                {
                    var insertedDrawing = _db.PartDrawing.Add(newPartDrawing);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedDrawing.PartDrawingId;
                    operationResult.Number = insertedDrawing.RevisionNumber;
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
                logger.ErrorFormat("Error saving new part drawing: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// save part layout
        /// </summary>
        /// <param name="newPartLayout"></param>
        /// <returns></returns>
        public OperationResult SavePartLayout(PartLayout newPartLayout)
        {
            var operationResult = new OperationResult();

            var existingLayout = _db.PartLayout.FirstOrDefault(x => x.Description.ToLower() == newPartLayout.Description.ToLower() && x.PartId == newPartLayout.PartId);

            try
            {
                if (existingLayout == null)
                {
                    var insertedLayout = _db.PartLayout.Add(newPartLayout);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedLayout.PartLayoutId;
                    operationResult.Description = insertedLayout.Description;
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
                logger.ErrorFormat("Error saving new part layout: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update part
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public OperationResult UpdatePart(Part part)
        {
            var operationResult = new OperationResult();

            var existingPart = GetPart(part.PartId);

            if (existingPart != null)
            {
                try
                {
                    _db.Part.Attach(existingPart);

                    _db.Entry(existingPart).CurrentValues.SetValues(part);

                    _db.SaveChanges();

                    var existingPartDrawings = _db.PartDrawing.Where(x => x.PartId == part.PartId).ToList();
                    var existingPartLayouts = _db.PartLayout.Where(x => x.PartId == part.PartId).ToList();

                    if(part.PartDrawings != null && part.PartDrawings.Count > 0)
                    {
                        foreach (var partDrawing in part.PartDrawings)
                        {
                            var existingDrawing = existingPartDrawings.FirstOrDefault(x => x.RevisionNumber == partDrawing.RevisionNumber);
                            if (existingDrawing == null)
                            {
                                partDrawing.PartId = part.PartId;
                                _db.PartDrawing.Add(partDrawing);
                            }
                        }
                        _db.SaveChanges();
                    }

                    if (part.PartLayouts != null && part.PartLayouts.Count > 0)
                    {
                        foreach (var partLayout in part.PartLayouts)
                        {
                            var existingLayout = existingPartLayouts.FirstOrDefault(x => x.Description == partLayout.Description);
                            if (existingLayout == null)
                            {
                                partLayout.PartId = part.PartId;
                                _db.PartLayout.Add(partLayout);
                            }
                        }
                        _db.SaveChanges();
                    }

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while updating part. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected part.";
            }

            return operationResult;
        }

        /// <summary>
        /// update part drawing
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public OperationResult UpdatePartDrawing(PartDrawing drawing)
        {
            var operationResult = new OperationResult();

            var existingDrawing = GetPartDrawing(drawing.PartDrawingId);

            if (existingDrawing != null)
            {
                try
                {
                    _db.PartDrawing.Attach(existingDrawing);

                    _db.Entry(existingDrawing).CurrentValues.SetValues(drawing);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while updating part drawing. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected part drawing.";
            }

            return operationResult;
        }

        /// <summary>
        /// update part layout
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public OperationResult UpdatePartLayout(PartLayout layout)
        {
            var operationResult = new OperationResult();

            var existingLayout = GetPartLayout(layout.PartLayoutId);

            if (existingLayout != null)
            {
                try
                {
                    _db.PartLayout.Attach(existingLayout);

                    _db.Entry(existingLayout).CurrentValues.SetValues(layout);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while updating part layout. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected part layout.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete part drawing
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public OperationResult DeletePartDrawing(PartDrawing drawing)
        {
            var operationResult = new OperationResult();

            var existingDrawing = GetPartDrawing(drawing.PartDrawingId);

            if (existingDrawing != null)
            {
                try
                {
                    _db.PartDrawing.Attach(existingDrawing);

                    _db.PartDrawing.Remove(existingDrawing);

                    _db.SaveChanges();

                    operationResult.Success = true;
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while deleting part drawing. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected part drawing.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete part layout
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public OperationResult DeletePartLayout(PartLayout layout)
        {
            var operationResult = new OperationResult();

            var existingLayout = GetPartLayout(layout.PartLayoutId);

            if(existingLayout != null)
            {
                try
                {
                    _db.PartLayout.Attach(existingLayout);

                    _db.PartLayout.Remove(existingLayout);

                    _db.SaveChanges();

                    operationResult.Success = true;
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while deleting part layout. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected part layout.";
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
