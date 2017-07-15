using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Models;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class ProjectPartConverter
    {
        /// <summary>
        /// convert project part to list model
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public ProjectPartViewModel ConvertToListView(ProjectPart projectPart)
        {
            ProjectPartViewModel model = new ProjectPartViewModel();
  
            var _customerDynamicsRepository = new CustomerDynamicsRepository();      
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _htsNumberRepository = new HtsNumberRepository();
            var _partTypeRepository = new PartTypeRepository();
            var _partStatusRepository = new PartStatusRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(projectPart.CustomerId);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(projectPart.FoundryId);
            var htsNumber = _htsNumberRepository.GetHtsNumber(projectPart.HtsNumberId);
            var partType = _partTypeRepository.GetPartType(projectPart.PartTypeId);
            var partStatus = _partStatusRepository.GetPartStatus(projectPart.PartStatusId);

            model.PartId = projectPart.PartId;
            model.ProjectPartId = projectPart.ProjectPartId;
            model.PartNumber = (!string.IsNullOrEmpty(projectPart.Number)) ? projectPart.Number : "N/A";
            model.PartDescription = (!string.IsNullOrEmpty(projectPart.Description)) ? projectPart.Description : "N/A";
            model.CustomerId = projectPart.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.FoundryId = projectPart.FoundryId;
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.HtsNumberId = projectPart.HtsNumberId;
            model.HtsNumber = (htsNumber != null && !string.IsNullOrEmpty(htsNumber.Description)) ? htsNumber.Description + "(" + (htsNumber.DutyRate * 100).ToString() + "%)" : "N/A";
            model.PartTypeId = projectPart.PartTypeId;
            model.PartTypeDescription = (partType != null && !string.IsNullOrEmpty(partType.Description)) ? partType.Description : "N/A";
            model.PartStatusId = projectPart.PartStatusId;
            model.PartStatusDescription = (partStatus != null && !string.IsNullOrEmpty(partStatus.Description)) ? partStatus.Description : "N/A";
            model.IsProject = true;

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

            if (_htsNumberRepository != null)
            {
                _htsNumberRepository.Dispose();
                _htsNumberRepository = null;
            }

            if (_partTypeRepository != null)
            {
                _partTypeRepository.Dispose();
                _partTypeRepository = null;
            }

            if (_partStatusRepository != null)
            {
                _partStatusRepository.Dispose();
                _partStatusRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert part to project part list model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public ProjectPartViewModel ConvertToListView(Part part)
        {
            ProjectPartViewModel model = new ProjectPartViewModel();

            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _htsNumberRepository = new HtsNumberRepository();
            var _partTypeRepository = new PartTypeRepository();
            var _partStatusRepository = new PartStatusRepository();

            var dynamicsPart = _dynamicsPartRepository.GetPartMaster(part.Number);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(part.CustomerId);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(part.FoundryId);
            var htsNumber = _htsNumberRepository.GetHtsNumber(part.HtsNumberId);
            var partType = _partTypeRepository.GetPartType(part.PartTypeId);
            var partStatus = _partStatusRepository.GetPartStatus(part.PartStatusId);

            model.PartId = part.PartId;
            model.PartNumber = (!string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.PartDescription = (dynamicsPart != null && !string.IsNullOrEmpty(dynamicsPart.ITEMDESC)) ? dynamicsPart.ITEMDESC : "N/A";
            model.CustomerId = part.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.FoundryId = part.FoundryId;
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.HtsNumberId = part.HtsNumberId;
            model.HtsNumber = (htsNumber != null && !string.IsNullOrEmpty(htsNumber.Description)) ? htsNumber.Description + "(" + (htsNumber.DutyRate * 100).ToString() + "%)" : "N/A";
            model.PartTypeId = part.PartTypeId;
            model.PartTypeDescription = (partType != null && !string.IsNullOrEmpty(partType.Description)) ? partType.Description : "N/A";
            model.PartStatusId = part.PartStatusId;
            model.PartStatusDescription = (partStatus != null && !string.IsNullOrEmpty(partStatus.Description)) ? partStatus.Description : "N/A";
            model.IsProject = false;

            if (_dynamicsPartRepository != null)
            {
                _dynamicsPartRepository.Dispose();
                _dynamicsPartRepository = null;
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

            if (_htsNumberRepository != null)
            {
                _htsNumberRepository.Dispose();
                _htsNumberRepository = null;
            }

            if (_partTypeRepository != null)
            {
                _partTypeRepository.Dispose();
                _partTypeRepository = null;
            }

            if (_partStatusRepository != null)
            {
                _partStatusRepository.Dispose();
                _partStatusRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert project part to part view model
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public PartViewModel ConvertToView(ProjectPart projectPart)
        {
            PartViewModel model = new PartViewModel();

            var _projectRepository = new ProjectRepository();
            var _foundryOrderRepository = new FoundryOrderRepository();
            var _partTypeRepository = new PartTypeRepository();
            var _partStatusRepository = new PartStatusRepository();

            var partType = _partTypeRepository.GetPartType(projectPart.PartTypeId);
            var statusEntity = _partStatusRepository.GetPartStatus(projectPart.PartStatusId);
            var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderPartByProjectPart(projectPart.ProjectPartId);

            if (foundryOrderPart != null)
            {
                var toolingOrder = _foundryOrderRepository.GetToolingFoundryOrder(foundryOrderPart.FoundryOrderId);
                model.FoundryOrderId = (toolingOrder != null) ? foundryOrderPart.FoundryOrderId : Guid.Empty;
                model.ToolingOrderNumber = (toolingOrder != null && !string.IsNullOrEmpty(toolingOrder.Number)) ? toolingOrder.Number : "N/A";
                model.ToolingDescription = projectPart.ToolingDescription;
            }

            model.ProjectPartId = projectPart.ProjectPartId;
            model.PartId = projectPart.PartId;
            model.PartNumber = (!string.IsNullOrEmpty(projectPart.Number)) ? projectPart.Number : "N/A";
            model.PartDescription = (!string.IsNullOrEmpty(projectPart.Description)) ? projectPart.Description : "N/A";
            model.AccountCode = (!string.IsNullOrEmpty(projectPart.AccountCode)) ? projectPart.AccountCode : "N/A";
            model.RevisionNumber = (!string.IsNullOrEmpty(projectPart.RevisionNumber)) ? projectPart.RevisionNumber : "N/A";
            model.CustomerId = projectPart.CustomerId;
            model.CustomerAddressId = projectPart.CustomerAddressId;
            model.FoundryId = projectPart.FoundryId;
            model.SubFoundryId = projectPart.SubFoundryId;
            model.HtsNumberId = projectPart.HtsNumberId;
            model.MaterialId = projectPart.MaterialId;
            model.SpecificationMaterialId = projectPart.MaterialSpecificationId;
            model.PartStatusId = projectPart.PartStatusId;
            model.PartStatusDescription = (statusEntity != null && !string.IsNullOrEmpty(statusEntity.Description)) ? statusEntity.Description : "N/A";
            model.PartTypeId = projectPart.PartTypeId;
            model.PartTypeDescription = (partType != null && !string.IsNullOrEmpty(partType.Description)) ? partType.Description : "N/A";
            model.ShipmentTermId = projectPart.ShipmentTermId;
            model.PaymentTermId = projectPart.PaymentTermId;
            model.SurchargeId = projectPart.SurchargeId;
            model.SiteId = projectPart.SiteId;
            model.DestinationId = projectPart.DestinationId;
            model.CoatingTypeId = projectPart.CoatingTypeId;
            model.PatternMaterialId = projectPart.PatternMaterialId;
            model.IsRaw = projectPart.IsRaw;
            model.IsMachined = projectPart.IsMachined;
            model.PalletQuantity = (projectPart.PalletQuantity != 0) ? projectPart.PalletQuantity : 0;
            model.PalletWeight = (projectPart.PalletQuantity != 0) ? projectPart.PalletQuantity * projectPart.Weight : 0.00m;
            model.Weight = (projectPart.Weight != 0.00m) ? projectPart.Weight : 0.00m;
            model.Cost = (projectPart.Cost != 0.00m) ? projectPart.Cost : 0.00m;
            model.Price = (projectPart.Price != 0.00m) ? projectPart.Price : 0.00m;
            model.AdditionalCost = (projectPart.AdditionalCost != 0.00m) ? projectPart.AdditionalCost : 0.00m;
            model.YearlySalesTotal = 0.00m;
            model.SixtyDaysSalesTotal = 0.00m;
            model.MonthlySalesTotal = 0.00m;
            model.AverageDailySales = 0.00m;
            model.QuantityOnHand = 0;
            model.FixtureDate = (projectPart.FixtureDate != null) ? projectPart.FixtureDate : DateTime.MinValue;
            model.FixtureCost = (projectPart.FixtureCost != 0.00m) ? projectPart.FixtureCost : 0.00m;
            model.FixturePrice = (projectPart.FixturePrice != 0.00m) ? projectPart.FixturePrice : 0.00m;
            model.PatternDate = projectPart.PatternDate;
            model.PatternCost = (projectPart.PatternCost != 0.00m) ? projectPart.PatternCost : 0.00m;
            model.PatternPrice = (projectPart.PatternPrice != 0.00m) ? projectPart.PatternPrice : 0.00m;
            model.IsFamilyPattern = projectPart.IsFamilyPattern;
            model.Notes = (projectPart.Notes != null) ? projectPart.Notes : "N/A";
            model.AnnualUsage = (projectPart.PatternPrice != 0) ? projectPart.AnnualUsage : 0;
            model.IsActive = true;
            model.IsProjectPart = true;

            if (projectPart.ProjectPartDrawings != null && projectPart.ProjectPartDrawings.Count > 0)
            {
                var drawings = new List<DrawingViewModel>();

                foreach (var partDrawing in projectPart.ProjectPartDrawings)
                {
                    DrawingViewModel convertedModel = new ProjectPartDrawingConverter().ConvertToView(partDrawing);
                    drawings.Add(convertedModel);
                }

                model.Drawings = drawings;
            }

            if (projectPart.ProjectPartLayouts != null && projectPart.ProjectPartLayouts.Count > 0)
            {
                var layouts = new List<LayoutViewModel>();

                foreach (var partLayout in projectPart.ProjectPartLayouts)
                {
                    LayoutViewModel convertedModel = new ProjectPartLayoutConverter().ConvertToView(partLayout);
                    layouts.Add(convertedModel);
                }

                model.Layouts = layouts;
            }

            if (_projectRepository != null)
            {
                _projectRepository.Dispose();
                _projectRepository = null;
            }

            if (_foundryOrderRepository != null)
            {
                _foundryOrderRepository.Dispose();
                _foundryOrderRepository = null;
            }

            if (_partTypeRepository != null)
            {
                _partTypeRepository.Dispose();
                _partTypeRepository = null;
            }

            if (_partStatusRepository != null)
            {
                _partStatusRepository.Dispose();
                _partStatusRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert rfq part view model to project part
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ProjectPart ConvertToDomain(RfqPartViewModel model)
        {
            ProjectPart projectPart = new ProjectPart();

            var _partRepository = new PartRepository();
            var _projectPartRepository = new ProjectPartRepository();
            var _partStatusRepository = new PartStatusRepository();

            var active = _partStatusRepository.GetPartStates().FirstOrDefault(x => x.Description == "Active");

            projectPart.ProjectPartId = model.ProjectPartId;
            projectPart.PartId = model.PartId;
            projectPart.Number = model.PartNumber;
            projectPart.AccountCode = null;
            projectPart.Description = model.PartDescription;
            projectPart.RevisionNumber = model.RevisionNumber;
            projectPart.PriceSheetId = null;
            projectPart.QuoteId = null;
            projectPart.CustomerId = model.CustomerId;
            projectPart.CustomerAddressId = null;
            projectPart.FoundryId = model.FoundryId;
            projectPart.SubFoundryId = (model.IsNew) ? model.FoundryId : model.SubFoundryId;
            projectPart.HtsNumberId = null;
            projectPart.MaterialId = model.MaterialId;
            projectPart.PartStatusId = (model.IsNew) ? (active != null) ? active.PartStatusId : Guid.Empty : model.PartStatusId;
            projectPart.PartTypeId = (model.IsNew) ? null : model.PartTypeId;
            projectPart.ShipmentTermId = null;
            projectPart.PaymentTermId = null;
            projectPart.SurchargeId = (model.IsNew) ? null : model.SurchargeId;
            projectPart.SiteId = null;
            projectPart.DestinationId = (model.IsNew) ? null : model.DestinationId;
            projectPart.PatternMaterialId = null;
            projectPart.IsRaw = model.IsRaw;
            projectPart.IsMachined = model.IsMachined;
            projectPart.Weight = model.Weight;
            projectPart.Cost = 0.00m;
            projectPart.Price = 0.00m;
            projectPart.PalletQuantity = 0;
            projectPart.AdditionalCost = 0.00m;
            projectPart.AnnualUsage = model.AnnualUsage;
            projectPart.FixtureDate = null;
            projectPart.FixtureCost = 0.00m;
            projectPart.FixturePrice = 0.00m;
            projectPart.PatternDate = null;
            projectPart.PatternCost = 0.00m;
            projectPart.PatternPrice = 0.00m;
            projectPart.IsFamilyPattern = false;
            projectPart.Notes = null;
            projectPart.CustomerOrderId = null;
            projectPart.FoundryOrderId = null;
            projectPart.ToolingDescription = null;

            if (!string.IsNullOrEmpty(model.RevisionNumber) && model.RevisionNumber != "N/A")
            {
                projectPart.ProjectPartDrawings = new List<ProjectPartDrawing>();

                ProjectPartDrawing projectPartDrawing = new ProjectPartDrawing();

                if (model.Drawing != null)
                {
                    projectPartDrawing.ProjectPartId = model.ProjectPartId;
                    projectPartDrawing.RevisionNumber = model.Drawing.RevisionNumber;
                    projectPartDrawing.Type = model.Drawing.Type;
                    projectPartDrawing.Length = model.Drawing.Length;
                    projectPartDrawing.Content = model.Drawing.Content;
                    projectPartDrawing.IsLatest = model.Drawing.IsLatest;
                    projectPartDrawing.IsMachined = model.Drawing.IsMachined;
                    projectPartDrawing.IsRaw = model.Drawing.IsRaw;
                    projectPartDrawing.IsActive = model.Drawing.IsActive;
                }
                else
                {
                    if (model.PartId != null)
                    {
                        var drawing = _partRepository.GetPartDrawings(model.PartId ?? Guid.Empty).FirstOrDefault(x => x.RevisionNumber == model.RevisionNumber);
                        projectPartDrawing.ProjectPartId = model.ProjectPartId;
                        if (drawing != null)
                        {
                            projectPartDrawing.RevisionNumber = drawing.RevisionNumber;
                            projectPartDrawing.Type = drawing.Type;
                            projectPartDrawing.Length = drawing.Length;
                            projectPartDrawing.Content = drawing.Content;
                            projectPartDrawing.IsLatest = drawing.IsLatest;
                            projectPartDrawing.IsMachined = drawing.IsMachined;
                            projectPartDrawing.IsRaw = drawing.IsRaw;
                        }
                        projectPartDrawing.IsActive = true;
                    }
                    else
                    {
                        var drawing = _projectPartRepository.GetProjectPartDrawings(model.ProjectPartId).FirstOrDefault(x => x.RevisionNumber == model.RevisionNumber);
                        projectPartDrawing.ProjectPartId = model.ProjectPartId;
                        projectPartDrawing.RevisionNumber = drawing.RevisionNumber;
                        projectPartDrawing.Type = drawing.Type;
                        projectPartDrawing.Length = drawing.Length;
                        projectPartDrawing.Content = drawing.Content;
                        projectPartDrawing.IsLatest = drawing.IsLatest;
                        projectPartDrawing.IsMachined = drawing.IsMachined;
                        projectPartDrawing.IsRaw = drawing.IsRaw;
                        projectPartDrawing.IsActive = true;
                    }
                }

                projectPart.ProjectPartDrawings.Add(projectPartDrawing);
            }

            if (_partRepository != null)
            {
                _partRepository.Dispose();
                _partRepository = null;
            }

            if (_projectPartRepository != null)
            {
                _projectPartRepository.Dispose();
                _projectPartRepository = null;
            }

            if (_partStatusRepository != null)
            {
                _partStatusRepository.Dispose();
                _partStatusRepository = null;
            }

            return projectPart;
        }

        /// <summary>
        /// convert quote part view model to project part 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ProjectPart ConvertToDomain(QuotePartViewModel model)
        {
            ProjectPart projectPart = new ProjectPart();

            projectPart.QuoteId = model.QuoteId;
            projectPart.ProjectPartId = model.ProjectPartId;
            projectPart.PartId = model.PartId;
            projectPart.Cost = model.Cost;
            projectPart.Price = model.Price;
            projectPart.Number = model.PartNumber;
            projectPart.Description = model.PartDescription;
            projectPart.CustomerId = model.CustomerId;
            projectPart.FoundryId = model.FoundryId;
            projectPart.IsRaw = projectPart.IsRaw;
            projectPart.IsMachined = projectPart.IsMachined;
            projectPart.RevisionNumber = model.RevisionNumber;
            projectPart.AnnualUsage = model.AnnualUsage;
            projectPart.Weight = model.Weight;
            projectPart.MaterialId = projectPart.MaterialId;
            projectPart.PatternCost = model.PatternCost;
            projectPart.PatternPrice = model.PatternPrice;
            projectPart.FixturePrice = model.FixturePrice;
            projectPart.FixtureCost = model.FixtureCost;
            projectPart.IsRaw = model.IsRaw;
            projectPart.IsMachined = model.IsMachined;

            return projectPart;
        }

        /// <summary>
        /// convert part model to project part 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ProjectPart ConvertToDomain(PartOperationModel model)
        {
            ProjectPart projectPart = new ProjectPart();

            projectPart.ProjectPartId = model.ProjectPartId;
            projectPart.PartId = model.PartId;
            projectPart.Number = model.PartNumber;
            projectPart.CustomerId = model.CustomerId;
            projectPart.CustomerAddressId = model.CustomerAddressId;
            projectPart.FoundryId = model.FoundryId;
            projectPart.SubFoundryId = model.SubFoundryId;
            projectPart.HtsNumberId = model.HtsNumberId;
            projectPart.MaterialId = model.MaterialId;
            projectPart.MaterialSpecificationId = model.MaterialSpecificationId;
            projectPart.PartStatusId = model.PartStatusId;
            projectPart.PartTypeId = model.PartTypeId;
            projectPart.ShipmentTermId = model.ShipmentTermId;
            projectPart.PaymentTermId = model.PaymentTermId;
            projectPart.SurchargeId = model.SurchargeId;
            projectPart.SiteId = model.SiteId;
            projectPart.DestinationId = model.DestinationId;
            projectPart.CoatingTypeId = model.CoatingTypeId;
            projectPart.PatternMaterialId = model.PatternMaterialId;
            projectPart.IsRaw = model.IsRaw;
            projectPart.IsMachined = model.IsMachined;
            projectPart.Weight = model.Weight;
            projectPart.Cost = model.Cost;
            projectPart.Price = model.Price;
            projectPart.PalletQuantity = model.PalletQuantity;
            projectPart.AdditionalCost = model.AdditionalCost;
            projectPart.AnnualUsage = model.AnnualUsage;
            projectPart.FixtureDate = model.FixtureDate;
            projectPart.FixtureCost = model.FixtureCost;
            projectPart.FixturePrice = model.FixturePrice;
            projectPart.PatternDate = model.PatternDate;
            projectPart.PatternCost = model.PatternCost;
            projectPart.PatternPrice = model.PatternPrice;
            projectPart.IsFamilyPattern = model.IsFamilyPattern;
            projectPart.Notes = model.Notes;
            projectPart.FoundryOrderId = model.FoundryOrderId;
            projectPart.ToolingDescription = model.ToolingDescription;

            return projectPart;
        }
    }
}