using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using SouthlandMetals.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class ProjectConverter
    {
        /// <summary>
        /// convert project to list model
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public ProjectViewModel ConvertToListView(Project project)
        {
            ProjectViewModel model = new ProjectViewModel();

            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _rfqRepository = new RfqRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(project.CustomerId);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(project.FoundryId);

            model.ProjectId = project.ProjectId;
            model.ProjectName = (!string.IsNullOrEmpty(project.Name)) ? project.Name : "N/A";
            model.CustomerId = project.CustomerId;
            model.FoundryId = project.FoundryId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.CreatedDate = (project.CreatedDate != null) ? project.CreatedDate : DateTime.MinValue;
            model.CreatedDateStr = (project.CreatedDate != null) ? project.CreatedDate.Value.ToShortDateString() : "N/A";
            model.CreatedBy = project.CreatedBy;
            model.IsOpen = project.IsOpen;
            model.IsHold = project.IsHold;
            model.HoldNotes = project.HoldNotes;
            model.HoldExpirationDate = (project.HoldExpirationDate != null) ? project.HoldExpirationDate : DateTime.MinValue;
            model.HoldExpirationDateStr = (project.HoldExpirationDate != null) ? project.HoldExpirationDate.Value.ToShortDateString() : "N/A";
            model.IsCanceled = project.IsCanceled;
            model.CanceledDate = (project.CanceledDate != null) ? project.CanceledDate : DateTime.MinValue; ;
            model.CanceledDateStr = (project.CanceledDate != null) ? project.CanceledDate.Value.ToString("M/dd/yyyy") : "N/A";
            model.IsComplete = project.IsComplete;
            model.CompletedDate = (project.CompletedDate != null) ? project.CompletedDate : DateTime.MinValue;
            model.CompletedDateStr = (project.CompletedDate != null) ? project.CompletedDate.Value.ToString("M/dd/yyyy") : "N/A";
            model.Status = project.IsOpen ? "Open" : project.IsCanceled ? "Canceled" : project.IsComplete ? "Completed" : project.IsHold ? "On Hold" : "N/A";

            if (_customerDynamicsRepository != null)
            {
                _customerDynamicsRepository.Dispose();
                _customerDynamicsRepository = null;
            }

            if (_rfqRepository != null)
            {
                _rfqRepository.Dispose();
                _rfqRepository = null;
            }

            if (_foundryDynamicsRepository != null)
            {
                _foundryDynamicsRepository.Dispose();
                _foundryDynamicsRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert project to view model
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public ProjectViewModel ConvertToView(Project project)
        {
            ProjectViewModel model = new ProjectViewModel();

            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _projectRepository = new ProjectRepository();
            var _partRepository = new PartRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(project.CustomerId);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(project.FoundryId);

            model.ProjectId = project.ProjectId;
            model.ProjectName = (!string.IsNullOrEmpty(project.Name)) ? project.Name : "N/A";
            model.CustomerId = project.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.CustomerContact = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.CNTCPRSN)) ? dynamicsCustomer.CNTCPRSN : "N/A";
            model.CustomerContactPhone = FormattingManager.FormatPhone(dynamicsCustomer.PHONE1);
            model.FoundryId = project.FoundryId;
            model.FoundryName = (dynamicsFoundry != null) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.FoundryContact = (dynamicsFoundry != null) ? dynamicsFoundry.VNDCNTCT : "N/A";
            model.FoundryContactPhone = FormattingManager.FormatPhone((dynamicsFoundry != null) ? dynamicsFoundry.PHNUMBR1 : string.Empty);
            model.CreatedDate = (project.CreatedDate != null) ? project.CreatedDate : DateTime.MinValue;
            model.CreatedDateStr = (project.CreatedDate != null) ? project.CreatedDate.Value.ToShortDateString() : "N/A";
            model.CreatedBy = project.CreatedBy;
            model.ModifiedDate = (project.ModifiedDate != null) ? project.ModifiedDate : DateTime.MinValue;
            model.ModifiedDateStr = (project.ModifiedDate != null) ? project.ModifiedDate.Value.ToShortDateString() : "N/A";
            model.ModifiedBy = project.ModifiedBy;
            model.IsOpen = project.IsOpen;
            model.IsHold = project.IsHold;
            model.HoldExpirationDate = (project.HoldExpirationDate != null) ? project.HoldExpirationDate : DateTime.MinValue; ;
            model.HoldExpirationDateStr = (project.HoldExpirationDate != null) ? project.HoldExpirationDate.Value.ToShortDateString() : "N/A";
            model.HoldNotes = (!string.IsNullOrEmpty(project.HoldNotes)) ? project.HoldNotes : "N/A";
            model.IsCanceled = project.IsCanceled;
            model.CanceledDate = (project.CanceledDate != null) ? project.CanceledDate : DateTime.MinValue;
            model.CanceledDateStr = (project.CanceledDate != null) ? project.CanceledDate.Value.ToShortDateString() : "N/A";
            model.CancelNotes = (!string.IsNullOrEmpty(project.CancelNotes)) ? project.CancelNotes : "N/A";
            model.IsComplete = project.IsComplete;
            model.CompletedDate = (project.CompletedDate != null) ? project.CompletedDate : DateTime.MinValue;
            model.CompletedDateStr = (project.CompletedDate != null) ? project.CompletedDate.Value.ToShortDateString() : "N/A";
            model.Status = project.IsOpen ? "Open" : project.IsCanceled ? "Canceled" : project.IsComplete ? "Completed" : project.IsHold ? "On Hold" : "N/A";
            model.Duration = (project.CompletedDate != null) ? (project.CompletedDate - project.CreatedDate).Value.Days : 0;

            var notes = new List<ProjectNoteViewModel>();

            var tempNotes = _projectRepository.GetProjectNotes(project.ProjectId);

            if (tempNotes != null && tempNotes.Count > 0)
            {
                foreach (var tempNote in tempNotes)
                {
                    ProjectNoteViewModel convertedModel = new ProjectNoteConverter().ConvertToView(tempNote);

                    notes.Add(convertedModel);
                }
            }

            model.ProjectNotes = notes.OrderBy(y => y.CreatedDate).ToList();

            var parts = new List<ProjectPartViewModel>();

            var tempParts = _partRepository.GetParts().Where(x => x.Projects.Any(y => y.ProjectId == project.ProjectId)).ToList();

            if (project.Parts != null && project.Parts.Count > 0)
            {
                foreach (var part in project.Parts)
                {
                    ProjectPartViewModel convertedModel = new ProjectPartConverter().ConvertToListView(part);

                    if (parts.FirstOrDefault(x => x.PartNumber == convertedModel.PartNumber) == null)
                    {
                        parts.Add(convertedModel);
                    }
                }
            }

            if (project.ProjectParts != null && project.ProjectParts.Count > 0)
            {
                foreach (var tempPart in project.ProjectParts.Where(x => x.PartId == null))
                {
                    ProjectPartViewModel convertedModel = new ProjectPartConverter().ConvertToListView(tempPart);

                    if (parts.FirstOrDefault(x => x.PartNumber == convertedModel.PartNumber) == null)
                    {
                        parts.Add(convertedModel);
                    }
                }
            }

            model.Parts = parts.OrderBy(x => x.PartNumber).ToList();

            if (project.Rfqs != null && project.Rfqs.Count > 0)
            {
                model.RFQs = new List<RfqViewModel>();
                foreach (var rfq in project.Rfqs)
                {
                    var rfqModel = new RfqConverter().ConvertToListView(rfq);
                    model.RFQs.Add(rfqModel);
                }
            }

            if (project.PriceSheets != null && project.PriceSheets.Count > 0)
            {
                model.PriceSheets = new List<PriceSheetListModel>();

                decimal profitMargin = 0;

                foreach (var priceSheet in project.PriceSheets)
                {
                    var priceSheetModel = new PriceSheetConverter().ConvertToListView(priceSheet);

                    model.PriceSheets.Add(priceSheetModel);

                    profitMargin += priceSheetModel.ProjectMargin;
                }

                model.ProjectMargin = (profitMargin / project.PriceSheets.Count).ToString("#.##") + '%';
            }

            if (project.Quotes != null && project.Quotes.Count > 0)
            {
                model.Quotes = new List<QuoteViewModel>();
                foreach (var quote in project.Quotes)
                {
                    var quoteModel = new QuoteConverter().ConvertToListView(quote);
                    model.Quotes.Add(quoteModel);
                }
            }

            if (project.CustomerOrders != null && project.CustomerOrders.Count > 0)
            {
                model.CustomerOrders = new List<CustomerOrderViewModel>();
                foreach (var customerOrder in project.CustomerOrders)
                {
                    var customerOrderModel = new CustomerOrderConverter().ConvertToListView(customerOrder);

                    model.CustomerOrders.Add(customerOrderModel);
                }
            }

            if (project.FoundryOrders != null && project.FoundryOrders.Count > 0)
            {
                model.FoundryOrders = new List<FoundryOrderViewModel>();
                foreach (var foundryOrder in project.FoundryOrders)
                {
                    var foundryOrderModel = new FoundryOrderConverter().ConvertToListView(foundryOrder);

                    model.FoundryOrders.Add(foundryOrderModel);
                }
            }

            if (project.Shipments != null && project.Shipments.Count > 0)
            {
                model.Shipments = new List<ShipmentViewModel>();
                foreach (var shipment in project.Shipments)
                {
                    var shipmentModel = new ShipmentConverter().ConvertToView(shipment);

                    model.Shipments.Add(shipmentModel);
                }
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
            if (_projectRepository != null)
            {
                _projectRepository.Dispose();
                _projectRepository = null;
            }
            if (_partRepository != null)
            {
                _partRepository.Dispose();
                _partRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert rfq view model to project for create project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Project ConvertToCreate(RfqViewModel model)
        {
            var project = new Project();

            project.ProjectId = model.ProjectId;
            project.CustomerId = model.CustomerId;
            project.FoundryId = model.FoundryId;
            project.Name = model.ProjectName;
            project.IsOpen = true;
            project.IsHold = false;
            project.HoldExpirationDate = null;
            project.HoldNotes = null;
            project.IsCanceled = false;
            project.CancelNotes = null;
            project.CanceledDate = null;
            project.IsComplete = false;
            project.CompletedDate = null;

            return project;
        }

        /// <summary>
        /// convert project view model to domain for update project 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Project ConvertToUpdate(ProjectViewModel model)
        {
            Project project = new Project();

            project.ProjectId = model.ProjectId;
            project.CustomerId = model.CustomerId;
            project.FoundryId = model.FoundryId;
            project.Name = model.ProjectName;
            project.IsOpen = model.IsOpen;
            project.IsHold = model.IsHold;
            project.IsCanceled = model.IsCanceled;
            project.IsComplete = model.IsComplete;
            project.CompletedDate = model.CompletedDate;
            project.HoldNotes = (model.IsHold) ? model.HoldNotes : null;
            project.CanceledDate = (model.IsCanceled) ? model.CanceledDate : null;
            project.CancelNotes = (model.IsCanceled) ? model.CancelNotes : null;
            project.HoldExpirationDate = (model.IsHold) ? model.HoldExpirationDate : null;

            return project;
        }
    }
}