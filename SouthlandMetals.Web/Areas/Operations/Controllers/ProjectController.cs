using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Controllers
{
    public class ProjectController : ApplicationBaseController
    {
        private IProjectRepository _projectRepository;
        private IProjectPartRepository _projectPartRepository;
        private IFoundryOrderRepository _foundryOrderRepository;
        private IRfqRepository _rfqRepository;
        private IQuoteRepository _quoteRepository;
        private ICustomerOrderRepository _customerOrderRepository;
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private IFoundryDynamicsRepository _foundryDynamicsRepository;

        public ProjectController()
        {
            _projectRepository = new ProjectRepository();
            _projectPartRepository = new ProjectPartRepository();
            _foundryOrderRepository = new FoundryOrderRepository();
            _rfqRepository = new RfqRepository();
            _quoteRepository = new QuoteRepository();
            _customerOrderRepository = new CustomerOrderRepository();
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _foundryDynamicsRepository = new FoundryDynamicsRepository();
        }

        public ProjectController(IProjectRepository projectRepository,
                                 IProjectPartRepository projectPartRepository,
                                 IFoundryOrderRepository foundryOrderRepository,
                                 IRfqRepository rfqRepository,
                                 IQuoteRepository quoteRepository,
                                 ICustomerOrderRepository customerRepository,
                                 ICustomerDynamicsRepository customerDynamicsRepository,
                                 IFoundryDynamicsRepository foundryDynamicsRepository)
        {
            _projectRepository = projectRepository;
            _projectPartRepository = projectPartRepository;
            _foundryOrderRepository = foundryOrderRepository;
            _rfqRepository = rfqRepository;
            _quoteRepository = quoteRepository;
            _customerOrderRepository = customerRepository;
            _customerDynamicsRepository = customerDynamicsRepository;
            _foundryDynamicsRepository = foundryDynamicsRepository;
        }

        /// <summary>
        /// GET: Operations/project
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Index()
        {
            var model = new ProjectViewModel();

            var projects = new List<ProjectViewModel>();

            var tempProjects = _projectRepository.GetProjects().Where(x => x.IsOpen).ToList();

            if (tempProjects != null && tempProjects.Count > 0)
            {
                foreach (var tempProject in tempProjects)
                {
                    ProjectViewModel convertedModel = new ProjectConverter().ConvertToListView(tempProject);

                    projects.Add(convertedModel);
                }

                model.Projects = projects;
            }

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Poject/summary
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Summary(Guid projectId)
        {
            var project = _projectRepository.GetProject(projectId);

            ProjectViewModel model = new ProjectConverter().ConvertToView(project);

            model.CurrentUser = User.Identity.Name;

            return View(model);
        }

        /// <summary>
        /// edit status modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _EditStatus()
        {
            var model = new ProjectViewModel();

            return PartialView(model);
        }
        
        /// <summary>
        /// edit status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult EditStatus(ProjectViewModel model)
        {
            var result = new OperationResult();

            var projectToUpdate = _projectRepository.GetProject(model.ProjectId);

            model.CustomerId = projectToUpdate.CustomerId;
            model.ProjectName = projectToUpdate.Name;
            model.FoundryId = projectToUpdate.FoundryId;

            switch (model.Status)
            {
                case "Open":
                    model.IsOpen = true;
                    model.IsHold = false;
                    model.HoldExpirationDate = null;
                    model.HoldNotes = null;
                    model.IsCanceled = false;
                    model.CancelNotes = null;
                    model.CanceledDate = null;
                    model.IsComplete = false;
                    model.CompletedDate = null;
                    break;
                case "On Hold":
                    model.IsOpen = false;
                    model.IsHold = true;
                    model.HoldExpirationDate = model.HoldExpirationDate;
                    model.HoldNotes = model.HoldNotes;
                    model.IsCanceled = false;
                    model.CancelNotes = null;
                    model.CanceledDate = null;
                    model.IsComplete = false;
                    model.CompletedDate = null;

                    foreach (var rfq in projectToUpdate.Rfqs)
                    {
                        rfq.IsOpen = false;
                        rfq.IsCanceled = false;
                        rfq.CancelNotes = null;
                        rfq.CanceledDate = null;
                        rfq.IsHold = true;
                        rfq.HoldExpirationDate = model.HoldExpirationDate;
                        rfq.HoldNotes = model.HoldNotes;
                        _rfqRepository.UpdateRfq(rfq);
                    }
                    foreach (var quote in projectToUpdate.Quotes)
                    {
                        quote.IsOpen = false;
                        quote.IsCanceled = false;
                        quote.CancelNotes = null;
                        quote.CanceledDate = null;
                        quote.IsHold = true;
                        quote.HoldExpirationDate = model.HoldExpirationDate;
                        quote.HoldNotes = model.HoldNotes;
                        _quoteRepository.UpdateQuote(quote);
                    }
                    foreach (var customerOrder in projectToUpdate.CustomerOrders)
                    {
                        customerOrder.IsOpen = false;
                        customerOrder.IsCanceled = false;
                        customerOrder.CancelNotes = null;
                        customerOrder.CanceledDate = null;
                        customerOrder.IsComplete = false;
                        customerOrder.IsHold = true;
                        customerOrder.HoldExpirationDate = model.HoldExpirationDate;
                        customerOrder.HoldNotes = model.HoldNotes;
                        _customerOrderRepository.UpdateCustomerOrder(customerOrder);
                    }
                    foreach (var foundryOrder in projectToUpdate.FoundryOrders)
                    {
                        foundryOrder.IsOpen = false;
                        foundryOrder.IsCanceled = false;
                        foundryOrder.CancelNotes = null;
                        foundryOrder.CanceledDate = null;
                        foundryOrder.IsComplete = false;
                        foundryOrder.IsHold = true;
                        foundryOrder.HoldExpirationDate = model.HoldExpirationDate;
                        foundryOrder.HoldNotes = model.HoldNotes;
                        _foundryOrderRepository.UpdateFoundryOrder(foundryOrder);
                    }
                    break;
                case "Canceled":
                    model.IsOpen = false;
                    model.IsHold = false;
                    model.HoldExpirationDate = null;
                    model.HoldNotes = null;
                    model.IsCanceled = true;
                    model.CancelNotes = model.CancelNotes;
                    model.CanceledDate = DateTime.Now;
                    model.IsComplete = false;
                    model.CompletedDate = null;

                    foreach (var rfq in projectToUpdate.Rfqs)
                    {
                        rfq.IsOpen = false;
                        rfq.IsHold = false;
                        rfq.HoldExpirationDate = null;
                        rfq.HoldNotes = null;
                        rfq.IsCanceled = true;
                        rfq.CancelNotes = model.CancelNotes;
                        rfq.CanceledDate = DateTime.Now;
                        _rfqRepository.UpdateRfq(rfq);
                    }
                    foreach (var quote in projectToUpdate.Quotes)
                    {
                        quote.IsOpen = false;
                        quote.IsHold = false;
                        quote.HoldExpirationDate = null;
                        quote.HoldNotes = null;
                        quote.IsCanceled = true;
                        quote.CancelNotes = model.CancelNotes;
                        quote.CanceledDate = DateTime.Now;
                        _quoteRepository.UpdateQuote(quote);
                    }
                    foreach (var customerOrder in projectToUpdate.CustomerOrders)
                    {
                        customerOrder.IsOpen = false;
                        customerOrder.IsHold = false;
                        customerOrder.HoldExpirationDate = null;
                        customerOrder.HoldNotes = null;
                        customerOrder.IsCanceled = true;
                        customerOrder.CancelNotes = model.CancelNotes;
                        customerOrder.CanceledDate = DateTime.Now;
                        _customerOrderRepository.UpdateCustomerOrder(customerOrder);
                    }
                    foreach (var foundryOrder in projectToUpdate.FoundryOrders)
                    {
                        foundryOrder.IsOpen = false;
                        foundryOrder.IsHold = false;
                        foundryOrder.HoldExpirationDate = null;
                        foundryOrder.HoldNotes = null;
                        foundryOrder.IsCanceled = true;
                        foundryOrder.CancelNotes = model.CancelNotes;
                        foundryOrder.CanceledDate = DateTime.Now;
                        _foundryOrderRepository.UpdateFoundryOrder(foundryOrder);
                    }
                    break;
                case "Complete":
                    model.IsOpen = false;
                    model.IsHold = false;
                    model.HoldExpirationDate = null;
                    model.HoldNotes = null;
                    model.IsCanceled = true;
                    model.CancelNotes = null;
                    model.CanceledDate = null;
                    model.IsComplete = true;
                    model.CompletedDate = DateTime.Now;
                    break;
            }

            Project project = new ProjectConverter().ConvertToUpdate(model);

            result = _projectRepository.UpdateProject(project);

            if (result.Success)
            {
                model.Success = true;
                model.Status = project.IsCanceled ? "Canceled" : project.IsComplete ? "Complete" : project.IsHold ? "On Hold" : "N/A";
                model.IsHold = project.IsHold;
                model.IsCanceled = project.IsCanceled;
            }
            else
            {
                model.Success = false;
                model.Message = result.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// add notes modal
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddNote(Guid projectId)
        {
            var model = new ProjectViewModel();

            model.ProjectId = projectId;

            return PartialView(model);
        }

        /// <summary>
        /// add project notes
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult AddProjectNote(ProjectNoteViewModel model)
        {
            var result = new OperationResult();

            ProjectNote newProjectNote = new ProjectNoteConverter().ConvertToDomain(model);

            result = _projectRepository.SaveProjectNote(newProjectNote);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete project note
        /// </summary>
        /// <param name="projectNoteId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult DeleteProjectNote(Guid projectNoteId)
        {
            var operationResult = new OperationResult();

            operationResult = _projectRepository.DeleteProjectNote(projectNoteId);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult Delete(Guid projectId)
        {
            var operationResult = new OperationResult();

            operationResult = _projectRepository.DeleteProject(projectId);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// check if can change the status of project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ValidateUpdate(Guid projectId)
        {
            var operationResult = new OperationResult();

            operationResult.Success = true;

            var projectParts = _projectPartRepository.GetProjectParts().Where(x => x.ProjectId == projectId).ToList();

            if (projectParts != null && projectParts.Count > 0)
            {
                foreach (var part in projectParts)
                {
                    var received = _foundryOrderRepository.GetFoundryOrderPartByProjectPart(part.ProjectPartId);

                    if (received != null)
                    {
                        operationResult.Success = false;
                        operationResult.Message = "Unable to put on hold or cancel, There are parts included in this project that have been received!";
                        break;
                    }
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// view rfqs modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewRfqs()
        {
            return PartialView("_ViewRfqs");
        }

        /// <summary>
        /// view tooling orders modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewToolingOrders()
        {
            return PartialView("_ViewToolingOrders");
        }

        /// <summary>
        /// view sample orders modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewSampleOrders()
        {
            return PartialView("_ViewSampleOrders");
        }

        /// <summary>
        /// view production price sheets modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewProductionPriceSheets()
        {
            return PartialView("_ViewProductionPriceSheets");
        }

        /// <summary>
        /// view quotes modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewQuotes()
        {
            return PartialView("_ViewQuotes");
        }

        /// <summary>
        /// view production orders modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewProductionOrders()
        {
            return PartialView("_ViewProductionOrders");
        }

        /// <summary>
        /// view shipments modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewShipments()
        {
            return PartialView("_ViewShipments");
        }

        /// <summary>
        /// get project by name
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetProject(string projectName)
        {
            var model = new ProjectViewModel();

            var selectableCustomers = new List<SelectListItem>();
            var selectableFoundries = new List<SelectListItem>();

            var projects = _projectRepository.GetProjects().Where(x => x.Name.Replace(" ", string.Empty).ToLower() == projectName.Replace(" ", string.Empty).ToLower()).ToList();

            if (projects != null && projects.Count > 0)
            {
                foreach (var project in projects)
                {
                    if (project.CustomerId != null)
                    {
                        var tempCustomer = _customerDynamicsRepository.GetCustomer(project.CustomerId);

                        if (tempCustomer != null)
                        {
                            var customer = selectableCustomers.FirstOrDefault(x => x.Value == tempCustomer.CUSTNMBR.TrimEnd());

                            if (customer == null)
                            {
                                var selectableCustomer = new SelectListItem() { Text = tempCustomer.SHRTNAME, Value = tempCustomer.CUSTNMBR.TrimEnd() };

                                selectableCustomers.Add(selectableCustomer);
                            }
                        }
                    }

                    if (project.FoundryId != null)
                    {
                        var tempFoundry = _foundryDynamicsRepository.GetFoundry(project.FoundryId);

                        if (tempFoundry != null)
                        {
                            var foundry = selectableFoundries.FirstOrDefault(x => x.Value == tempFoundry.VENDORID.TrimEnd());

                            if (foundry == null)
                            {
                                var selectableFoundry = new SelectListItem() { Text = tempFoundry.VENDSHNM, Value = tempFoundry.VENDORID.TrimEnd() };

                                selectableFoundries.Add(selectableFoundry);
                            }
                        }
                    }
                }

                model.SelectableCustomers = selectableCustomers.Distinct().ToList();
                model.SelectableFoundries = selectableFoundries.Distinct().ToList();
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get open projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetOpenProjects()
        {
            var model = new ProjectViewModel();

            var projects = new List<ProjectViewModel>();

            var tempProjects = _projectRepository.GetProjects().Where(x => x.IsOpen).ToList();

            if (tempProjects != null && tempProjects.Count > 0)
            {
                foreach (var tempProject in tempProjects)
                {
                    ProjectViewModel convertedModel = new ProjectConverter().ConvertToListView(tempProject);

                    projects.Add(convertedModel);
                }
            }

            model.Projects = projects.OrderByDescending(x => x.CreatedDate).ThenBy(y => y.ProjectName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get hold projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetHoldProjects()
        {
            var model = new ProjectViewModel();

            var projects = new List<ProjectViewModel>();

            var tempProjects = _projectRepository.GetProjects().Where(x => x.IsHold).ToList();

            if (tempProjects != null && tempProjects.Count > 0)
            {
                foreach (var tempProject in tempProjects)
                {
                    ProjectViewModel convertedModel = new ProjectConverter().ConvertToListView(tempProject);

                    projects.Add(convertedModel);
                }
            }

            model.Projects = projects.OrderByDescending(x => x.CreatedDate).ThenBy(y => y.ProjectName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get canceled projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCanceledProjects()
        {
            var model = new ProjectViewModel();

            var projects = new List<ProjectViewModel>();

            var tempProjects = _projectRepository.GetProjects().Where(x => x.IsCanceled).ToList();

            if (tempProjects != null && tempProjects.Count > 0)
            {
                foreach (var tempProject in tempProjects)
                {
                    ProjectViewModel convertedModel = new ProjectConverter().ConvertToListView(tempProject);

                    projects.Add(convertedModel);
                }
            }

            model.Projects = projects.OrderByDescending(x => x.CreatedDate).ThenBy(y => y.ProjectName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get complete projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCompleteProjects()
        {
            var model = new ProjectViewModel();

            var projects = new List<ProjectViewModel>();

            var tempProjects = _projectRepository.GetProjects().Where(x => x.IsComplete).ToList();

            if (tempProjects != null && tempProjects.Count > 0)
            {
                foreach (var tempProject in tempProjects)
                {
                    ProjectViewModel convertedModel = new ProjectConverter().ConvertToListView(tempProject);

                    projects.Add(convertedModel);
                }
            }

            model.Projects = projects.OrderByDescending(x => x.CreatedDate).ThenBy(y => y.ProjectName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get selectable projects list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetSelectableProjects()
        {
            var selectableProjects = _projectRepository.GetSelectableActiveProjects();

            return Json(selectableProjects, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_projectRepository != null)
                {
                    _projectRepository.Dispose();
                    _projectRepository = null;
                }

                if (_projectPartRepository != null)
                {
                    _projectPartRepository.Dispose();
                    _projectPartRepository = null;
                }

                if (_foundryOrderRepository != null)
                {
                    _foundryOrderRepository.Dispose();
                    _foundryOrderRepository = null;
                }

                if (_rfqRepository != null)
                {
                    _rfqRepository.Dispose();
                    _rfqRepository = null;
                }

                if (_quoteRepository != null)
                {
                    _quoteRepository.Dispose();
                    _quoteRepository = null;
                }

                if (_customerOrderRepository != null)
                {
                    _customerOrderRepository.Dispose();
                    _customerOrderRepository = null;
                }

                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }

                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}