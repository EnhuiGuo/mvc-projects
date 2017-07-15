using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class ProjectPartRepository : IProjectPartRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public ProjectPartRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get project parts 
        /// </summary>
        /// <returns></returns>
        public List<ProjectPart> GetProjectParts()
        {
            var part = new List<ProjectPart>();

            try
            {
                part = _db.ProjectPart.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("An error occurred while getting project parts. {0}", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get the drawings of the project part 
        /// </summary>
        /// <param name="projectPartId"></param>
        /// <returns></returns>
        public List<ProjectPartDrawing> GetProjectPartDrawings(Guid projectPartId)
        {
            var partDrawings = new List<ProjectPartDrawing>();

            try
            {
                partDrawings = _db.ProjectPartDrawing.Where(x => x.ProjectPartId == projectPartId).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("An error occurred while getting project part drawings. {0}", ex.ToString());
            }

            return partDrawings;
        }

        /// <summary>
        /// get the layouts of the project part  
        /// </summary>
        /// <param name="projectPartId"></param>
        /// <returns></returns>
        public List<ProjectPartLayout> GetProjectPartLayouts(Guid projectPartId)
        {
            var partLayouts = new List<ProjectPartLayout>();

            try
            {
                partLayouts = _db.ProjectPartLayout.Where(x => x.ProjectPartId == projectPartId).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("An error occurred while getting project part layouts. {0}", ex.ToString());
            }

            return partLayouts;
        }

        /// <summary>
        /// get project part by id
        /// </summary>
        /// <param name="projectPartId"></param>
        /// <returns></returns>
        public ProjectPart GetProjectPart(Guid projectPartId)
        {
            var part = new ProjectPart();

            try
            {
                part = _db.ProjectPart.FirstOrDefault(x => x.ProjectPartId == projectPartId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("An error occurred while getting project part. {0}", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get project part by nullable of id
        /// </summary>
        /// <param name="projectPartId"></param>
        /// <returns></returns>
        public ProjectPart GetProjectPart(Guid? projectPartId)
        {
            var part = new ProjectPart();

            try
            {
                part = _db.ProjectPart.FirstOrDefault(x => x.ProjectPartId == projectPartId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("An error occurred while getting project part. {0}", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get project part by part number 
        /// </summary>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public ProjectPart GetProjectPart(string partNumber)
        {
            var projectPart = new ProjectPart();

            try
            {
                projectPart = _db.ProjectPart.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == partNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("An error occurred while getting project part. {0}", ex.ToString());
            }

            return projectPart;
        }

        /// <summary>
        /// get project part drawing by id
        /// </summary>
        /// <param name="projectPartDrawingId"></param>
        /// <returns></returns>
        public ProjectPartDrawing GetProjectPartDrawing(Guid projectPartDrawingId)
        {
            var partDrawing = new ProjectPartDrawing();

            try
            {
                partDrawing = _db.ProjectPartDrawing.FirstOrDefault(x => x.ProjectPartDrawingId == projectPartDrawingId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("An error occurred while getting project part drawing. {0}", ex.ToString());
            }

            return partDrawing;
        }

        /// <summary>
        /// get project part drawing by name
        /// </summary>
        /// <param name="trimmedFileName"></param>
        /// <returns></returns>
        public ProjectPartDrawing GetProjectPartDrawing(string trimmedFileName)
        {
            var partDrawing = new ProjectPartDrawing();

            try
            {
                partDrawing = _db.ProjectPartDrawing.FirstOrDefault(x => x.RevisionNumber == trimmedFileName);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("An error occurred while getting project part drawing. {0}", ex.ToString());
            }

            return partDrawing;
        }

        /// <summary>
        /// get project part layout
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        public ProjectPartLayout GetProjectPartLayout(Guid layoutId)
        {
            var partLayout = new ProjectPartLayout();

            try
            {
                partLayout = _db.ProjectPartLayout.FirstOrDefault(x => x.ProjectPartLayoutId == layoutId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("An error occurred while getting project part layout. {0}", ex.ToString());
            }

            return partLayout;
        }

        /// <summary>
        /// save project part 
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public OperationResult SaveProjectPart(ProjectPart projectPart)
        {
            var operationResult = new OperationResult();

            try
            {
                var insertedProject = _db.ProjectPart.Add(projectPart);

                _db.SaveChanges();

                operationResult.Success = true;
                operationResult.Message = "Success";
                operationResult.ReferenceId = insertedProject.ProjectPartId;
            }
            catch (Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = "Error";
                logger.ErrorFormat("An error occurred while saving new project part. {0}", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// save project part drawing
        /// </summary>
        /// <param name="newProjectPartDrawing"></param>
        /// <returns></returns>
        public OperationResult SaveProjectPartDrawing(ProjectPartDrawing newProjectPartDrawing)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingProjectPartDrawing = _db.ProjectPartDrawing.FirstOrDefault(x => x.RevisionNumber.ToLower() == newProjectPartDrawing.RevisionNumber.ToLower() && x.ProjectPartId == newProjectPartDrawing.ProjectPartId);

                if (existingProjectPartDrawing == null)
                {
                    var insertedDrawing = _db.ProjectPartDrawing.Add(newProjectPartDrawing);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedDrawing.ProjectPartDrawingId;
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
                logger.ErrorFormat("An error occurred while saving new project part drawing. {0}", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// save project part layout
        /// </summary>
        /// <param name="newProjectPartLayout"></param>
        /// <returns></returns>
        public OperationResult SaveProjectPartLayout(ProjectPartLayout newProjectPartLayout)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingProjectPartLayout = _db.ProjectPartLayout.FirstOrDefault(x => x.Description.ToLower() == newProjectPartLayout.Description.ToLower() && x.ProjectPartId == newProjectPartLayout.ProjectPartId);

                if (existingProjectPartLayout == null)
                {
                    var insertedLayout = _db.ProjectPartLayout.Add(newProjectPartLayout);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedLayout.ProjectPartLayoutId;
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
                logger.ErrorFormat("An error occurred while saving new project part layout. {0}", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update project part
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public OperationResult UpdateProjectPart(ProjectPart projectPart)
        {
            var operationResult = new OperationResult();

            var existingProjectPart = _db.ProjectPart.Find(projectPart.ProjectPartId);

            if (existingProjectPart != null)
            {
                try
                {
                    _db.Entry(existingProjectPart).CurrentValues.SetValues(projectPart);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while updating project part. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find project part";
            }

            return operationResult;
        }

        /// <summary>
        /// update project part drawing
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public OperationResult UpdateProjectPartDrawing(ProjectPartDrawing drawing)
        {
            var operationResult = new OperationResult();

            var existingProjectPartDrawing = GetProjectPartDrawing(drawing.ProjectPartDrawingId);

            if (existingProjectPartDrawing != null)
            {
                try
                {
                    _db.ProjectPartDrawing.Attach(existingProjectPartDrawing);

                    _db.Entry(existingProjectPartDrawing).CurrentValues.SetValues(drawing);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while updating project part drawing. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find project part drawing to update";
            }

            return operationResult;
        }

        /// <summary>
        /// update project part layout
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public OperationResult UpdateProjectPartLayout(ProjectPartLayout layout)
        {
            var operationResult = new OperationResult();

            var existingProjectPartLayout = GetProjectPartLayout(layout.ProjectPartLayoutId);

            if (existingProjectPartLayout != null)
            {
                try
                {
                    _db.ProjectPartLayout.Attach(existingProjectPartLayout);

                    _db.Entry(existingProjectPartLayout).CurrentValues.SetValues(layout);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Update Success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while updating project part layout. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find project part layout to update";
            }

            return operationResult;
        }

        /// <summary>
        /// delete project part 
        /// </summary>
        /// <param name="projectPartId"></param>
        /// <returns></returns>
        public OperationResult DeleteProjectPart(Guid projectPartId)
        {
            var operationResult = new OperationResult();

            var existingProjectPart = GetProjectPart(projectPartId);

            if (existingProjectPart != null)
            {
                try
                {
                    _db.ProjectPart.Attach(existingProjectPart);

                    var drawings = _db.ProjectPartDrawing.Where(x => x.ProjectPartId == projectPartId).ToList();

                    if (drawings != null)
                    {
                        foreach (var drawing in drawings)
                            _db.ProjectPartDrawing.Remove(drawing);
                    }

                    _db.ProjectPart.Remove(existingProjectPart);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while deleting project part. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find project part to delete";
            }

            return operationResult;
        }

        /// <summary>
        /// delete project part drawing
        /// </summary>
        /// <param name="drawingId"></param>
        /// <returns></returns>
        public OperationResult DeleteProjectPartDrawing(Guid drawingId)
        {
            var operationResult = new OperationResult();

            var existingProjectPartDrawing = GetProjectPartDrawing(drawingId);

            if (existingProjectPartDrawing != null)
            {
                try
                {
                    _db.ProjectPartDrawing.Attach(existingProjectPartDrawing);

                    _db.ProjectPartDrawing.Remove(existingProjectPartDrawing);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while deleting project part drawing. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find project part drawing to delete";
            }

            return operationResult;
        }

        /// <summary>
        /// delete project part layout
        /// </summary>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        public OperationResult DeleteProjectPartLayout(Guid layoutId)
        {
            var operationResult = new OperationResult();

            var existingProjectPartLayout = GetProjectPartLayout(layoutId);

            if (existingProjectPartLayout != null)
            {
                try
                {
                    _db.ProjectPartLayout.Attach(existingProjectPartLayout);

                    _db.ProjectPartLayout.Remove(existingProjectPartLayout);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("An error occurred while deleting project part layout. {0}", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find project part layout to delete";
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
