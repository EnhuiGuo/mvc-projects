using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class ProjectRepository : IProjectRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public ProjectRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable part projects
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectablePartProjects(Guid partId)
        {
            var projects = new List<SelectListItem>();

            try
            {
                projects = _db.Project.Where(x => x.Parts.Any(y => y.PartId == partId)).Select(y => new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.ProjectId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting SelectableProjects: {0} ", ex.ToString());
            }

            return projects;
        }

        /// <summary>
        /// get selectable projects
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableProjects()
        {
            var projects = new List<SelectListItem>();

            try
            {
                projects = _db.Project.Select(y => new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.ProjectId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting SelectableProjects: {0} ", ex.ToString());
            }

            return projects;
        }

        /// <summary>
        /// get selectable active projects
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableActiveProjects()
        {
            var projects = new List<SelectListItem>();

            try
            {
                projects = _db.Project.Where(x => !x.IsHold && !x.IsCanceled && !x.IsComplete).Select(y => new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.ProjectId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting SelectableProjects: {0} ", ex.ToString());
            }

            return projects;
        }

        /// <summary>
        /// get all projects
        /// </summary>
        /// <returns></returns>
        public List<Project> GetProjects()
        {
            var projects = new List<Project>();

            try
            {
                projects = _db.Project.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting projects: {0} ", ex.ToString());
            }

            return projects;
        }

        /// <summary>
        /// get notes of the projects 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<ProjectNote> GetProjectNotes(Guid projectId)
        {
            var notes = new List<ProjectNote>();

            try
            {
                notes = _db.ProjectNote.Where(x => x.ProjectId == projectId).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting project notes: {0} ", ex.ToString());
            }

            return notes;
        }

        /// <summary>
        /// get project by id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Project GetProject(Guid projectId)
        {
            var project = new Project();

            try
            {
                project = _db.Project.FirstOrDefault(x => x.ProjectId == projectId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting project: {0} ", ex.ToString());
            }

            return project;
        }

        /// <summary>
        /// get project by nullable id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Project GetProject(Guid? projectId)
        {
            var project = new Project();

            try
            {
                project = _db.Project.FirstOrDefault(x => x.ProjectId == projectId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting project: {0} ", ex.ToString());
            }

            return project;
        }

        /// <summary>
        /// get project by name
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public Project GetProject(string projectName)
        {
            var project = new Project();

            try
            {
                project = _db.Project.FirstOrDefault(x => x.Name.Replace(" ", string.Empty).ToLower() == projectName.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting project: {0} ", ex.ToString());
            }

            return project;
        }

        /// <summary>
        /// get project note by id
        /// </summary>
        /// <param name="projectNoteId"></param>
        /// <returns></returns>
        public ProjectNote GetProjectNote(Guid projectNoteId)
        {
            var projectNote = new ProjectNote();

            try
            {
                projectNote = _db.ProjectNote.FirstOrDefault(x => x.ProjectNoteId == projectNoteId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting project note: {0} ", ex.ToString());
            }

            return projectNote;
        }

        /// <summary>
        /// save new project
        /// </summary>
        /// <param name="newProject"></param>
        /// <returns></returns>
        public OperationResult SaveProject(Project newProject)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingProject = _db.Project.FirstOrDefault(x => x.Name.ToLower() == newProject.Name.ToLower());

                if (existingProject == null)
                {
                    var insertedProject = _db.Project.Add(newProject);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Update Project success!";
                    operationResult.ReferenceId = insertedProject.ProjectId;
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
                operationResult.Message = "Can not update this Project!";
                logger.ErrorFormat("Error getting UpdateProject: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// save project note
        /// </summary>
        /// <param name="newProjectNote"></param>
        /// <returns></returns>
        public OperationResult SaveProjectNote(ProjectNote newProjectNote)
        {
            var operationResult = new OperationResult();

            try
            {
                logger.Debug("ProjectNote is being created...");

                _db.ProjectNote.Add(newProjectNote);

                _db.SaveChanges();

                operationResult.Success = true;
                operationResult.Message = "Success";
            }
            catch (Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = "Error";
                logger.ErrorFormat("Error saving new project note: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public OperationResult UpdateProject(Project project)
        {
            var operationResult = new OperationResult();

            var existingProject = GetProject(project.ProjectId);

            if (existingProject != null)
            {
                try
                {
                    _db.Project.Attach(existingProject);

                    _db.Entry(existingProject).CurrentValues.SetValues(project);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Update Project success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Can not update this Project!";
                    logger.ErrorFormat("Error getting UpdateProject: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find project to update";
            }

            return operationResult;
        }

        /// <summary>
        /// delete project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public OperationResult DeleteProject(Guid projectId)
        {
            var operationResult = new OperationResult();

            var existingProject = _db.Project.FirstOrDefault(x => x.ProjectId == projectId);

            if (existingProject != null)
            {
                try
                {
                    _db.Project.Attach(existingProject);

                    var existingProjectParts = _db.ProjectPart.Where(x => x.ProjectId == projectId).ToList();

                    if (existingProjectParts != null && existingProjectParts.Count > 0)
                    {
                        foreach (var projectPart in existingProjectParts)
                        {
                            _db.ProjectPart.Remove(projectPart);
                        }
                    }

                    existingProject.Parts.Clear();

                    var existingRfqs = _db.Rfq.Where(x => x.ProjectId == projectId).ToList();

                    if (existingRfqs != null && existingRfqs.Count > 0)
                    {
                        foreach (var rfq in existingRfqs)
                        {
                            _db.Rfq.Remove(rfq);
                        }
                    }

                    var existingPriceSheets = _db.PriceSheet.Where(x => x.ProjectId == projectId).ToList();

                    if (existingPriceSheets != null && existingPriceSheets.Count > 0)
                    {
                        foreach (var priceSheet in existingPriceSheets)
                        {
                            _db.PriceSheet.Remove(priceSheet);
                        }
                    }

                    var existingQuotes = _db.Quote.Where(x => x.ProjectId == projectId).ToList();

                    if (existingQuotes != null && existingQuotes.Count > 0)
                    {
                        foreach (var quote in existingQuotes)
                        {
                            _db.Quote.Remove(quote);
                        }
                    }

                    var existingCustomerOrders = _db.CustomerOrder.Where(x => x.ProjectId == projectId).ToList();

                    if (existingCustomerOrders != null && existingCustomerOrders.Count > 0)
                    {
                        foreach (var customerOrder in existingCustomerOrders)
                        {
                            _db.CustomerOrder.Remove(customerOrder);
                        }
                    }

                    _db.Project.Remove(existingProject);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "delete Project Success!";

                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error! in delete Project";
                    logger.ErrorFormat("Error getting DeleteProject: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "unable to locate project to delete.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete project note
        /// </summary>
        /// <param name="projectNoteId"></param>
        /// <returns></returns>
        public OperationResult DeleteProjectNote(Guid projectNoteId)
        {
            var operationResult = new OperationResult();

            var existingProjectNote = GetProjectNote(projectNoteId);

            if (existingProjectNote != null)
            {
                try
                {
                    _db.ProjectNote.Attach(existingProjectNote);

                    _db.ProjectNote.Remove(existingProjectNote);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";

                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "An error occurred while deleting project note";
                    logger.ErrorFormat("An error occurred while deleting project note: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "unable to locate project note to delete.";
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
