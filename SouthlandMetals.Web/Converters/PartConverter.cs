using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Models;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Models;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class PartConverter
    {
        /// <summary>
        /// convert part to list model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public PartViewModel ConvertToListView(Part part)
        {
            PartViewModel model = new PartViewModel();

            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _projectRepository = new ProjectRepository();
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
            model.PartStatusId = partStatus.PartStatusId;
            model.PartStatusDescription = (partStatus != null && !string.IsNullOrEmpty(partStatus.Description)) ? partStatus.Description : "N/A";
            model.PartTypeId = part.PartTypeId;
            model.PartTypeDescription = (partType != null && !string.IsNullOrEmpty(partType.Description)) ? partType.Description : "N/A";

            if (_dynamicsPartRepository != null)
            {
                _dynamicsPartRepository.Dispose();
                _dynamicsPartRepository = null;
            }

            if (_projectRepository != null)
            {
                _projectRepository.Dispose();
                _projectRepository = null;
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
        /// convert part to view model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public PartViewModel ConvertToView(Part part)
        {
            PartViewModel model = new PartViewModel();

            var _partRepository = new PartRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _projectPartRepository = new ProjectPartRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _htsNumberRepository = new HtsNumberRepository();
            var _partTypeRepository = new PartTypeRepository();
            var _partStatusRepository = new PartStatusRepository();
            var _foundryOrderRepository = new FoundryOrderRepository();

            var dynamicsPart = _dynamicsPartRepository.GetPartMaster(part.Number);
            var dynamicsPartQty = _dynamicsPartRepository.GetPartQuantityMaster(part.Number);
            var dynamicsPartCurrency = _dynamicsPartRepository.GetPartCurrency(part.Number);
            var dynamicsPartRollingSales = _dynamicsPartRepository.GetItemRollingSales(part.Number);
            var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderPartByPart(part.PartId);

            if (foundryOrderPart != null)
            {
                var toolingOrder = _foundryOrderRepository.GetToolingFoundryOrder(foundryOrderPart.FoundryOrderId);
                model.FoundryOrderId = (toolingOrder != null) ? foundryOrderPart.FoundryOrderId : Guid.Empty;
                model.ToolingOrderNumber = (toolingOrder != null && !string.IsNullOrEmpty(toolingOrder.Number)) ? toolingOrder.Number : "N/A";
                model.ToolingDescription = (!string.IsNullOrEmpty(part.ToolingDescription)) ? part.ToolingDescription : "N/A";
            }

            var latestDrawing = _partRepository.GetPartDrawings(part.PartId).FirstOrDefault(x => x.IsLatest == true);

            var weight = (dynamicsPart != null) ? Math.Round((dynamicsPart.ITEMSHWT / 100.00m), 2) : 0.00m;

            var yearlySalesTotal = (dynamicsPartRollingSales != null) ? (dynamicsPartRollingSales.Sales_1_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_2_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_3_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_4_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_5_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_6_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_7_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_8_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_9_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_10_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_11_mo_ago +
                                                                           dynamicsPartRollingSales.Sales_12_mo_ago) : 0.00m;

            model.ProjectPartId = Guid.Empty;
            model.PartId = part.PartId;
            model.PartNumber = (!string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.AccountCode = (!string.IsNullOrEmpty(part.AccountCode)) ? part.AccountCode : "N/A";
            model.PartDescription = (dynamicsPart != null && !string.IsNullOrEmpty(dynamicsPart.ITEMDESC)) ? dynamicsPart.ITEMDESC : "N/A";
            model.RevisionNumber = (latestDrawing != null && !string.IsNullOrEmpty(latestDrawing.RevisionNumber)) ? latestDrawing.RevisionNumber : "N/A";
            model.CustomerId = part.CustomerId;
            model.CustomerAddressId = part.CustomerAddressId;
            model.FoundryId = part.FoundryId;
            model.SubFoundryId = part.SubFoundryId;
            model.HtsNumberId = part.HtsNumberId;
            model.MaterialId = part.MaterialId;
            model.SpecificationMaterialId = part.MaterialSpecificationId;
            model.PartStatusId = part.PartStatusId;
            model.PartTypeId = part.PartTypeId;
            model.ShipmentTermId = part.ShipmentTermId;
            model.PaymentTermId = part.PaymentTermId;
            model.SurchargeId = part.SurchargeId;
            model.SiteId = part.SiteId;
            model.DestinationId = part.DestinationId;
            model.CoatingTypeId = part.CoatingTypeId;
            model.PatternMaterialId = part.PatternMaterialId;
            model.IsRaw = part.IsRaw;
            model.IsMachined = part.IsMachined;
            model.PalletQuantity = (part.PalletQuantity != 0) ? part.PalletQuantity : 0;
            model.PalletWeight = (part.PalletQuantity != 0) ? part.PalletQuantity * weight : 0.00m;
            model.Weight = weight;
            model.Cost = (dynamicsPart != null && (Math.Round(dynamicsPart.STNDCOST, 2) != 0.00m)) ? Math.Round(dynamicsPart.STNDCOST, 2) : 0.00m;
            model.Price = (dynamicsPartCurrency != null && (Math.Round(dynamicsPartCurrency.LISTPRCE, 2) != 0.00m)) ? Math.Round(dynamicsPartCurrency.LISTPRCE, 2) : 0.00m;
            model.AdditionalCost = (part.AdditionalCost != 0.00m) ? part.AdditionalCost : 0.00m;
            model.YearlySalesTotal = Math.Round(yearlySalesTotal, 2);
            model.SixtyDaysSalesTotal = (yearlySalesTotal != 0.00m) ? Math.Round((yearlySalesTotal / 6), 2) : 0.00m;
            model.MonthlySalesTotal = (yearlySalesTotal != 0.00m) ? Math.Round((yearlySalesTotal / 12), 2) : 0.00m;
            model.AverageDailySales = (yearlySalesTotal != 0.00m) ? Math.Round((yearlySalesTotal / 365), 2) : 0.00m;
            model.QuantityOnHand = (dynamicsPartQty != null && (Math.Round(dynamicsPartQty.QTYONHND, 2) != 0.00m)) ? Math.Round(dynamicsPartQty.QTYONHND, 2) : 0.00m;
            model.FixtureDate = (part.FixtureDate != null) ? part.FixtureDate : DateTime.MinValue;
            model.FixtureDateStr = (part.FixtureDate != null) ? part.FixtureDate.Value.ToShortDateString() : "N/A";
            model.FixtureCost = (Math.Round(part.FixtureCost, 2) != 0.00m) ? Math.Round(part.FixtureCost, 2) : 0.00m;
            model.FixturePrice = (Math.Round(part.FixturePrice, 2) != 0.00m) ? Math.Round(part.FixturePrice, 2) : 0.00m;
            model.PatternDate = (part.PatternDate != null) ? part.PatternDate : DateTime.MinValue;
            model.PatternDateStr = (part.PatternDate != null) ? part.PatternDate.Value.ToShortDateString() : "N/A";
            model.PatternCost = (Math.Round(part.PatternCost, 2) != 0.00m) ? Math.Round(part.PatternCost, 2) : 0.00m;
            model.PatternPrice = (Math.Round(part.PatternPrice, 2) != 0.00m) ? Math.Round(part.PatternPrice, 2) : 0.00m;
            model.IsFamilyPattern = part.IsFamilyPattern;
            model.Notes = (!string.IsNullOrEmpty(part.Notes)) ? part.Notes : "N/A";
            model.AnnualUsage = (part.AnnualUsage != 0) ? part.AnnualUsage : 0;
            model.IsActive = part.IsActive;
            model.IsProjectPart = false;

            var partDrawings = _partRepository.GetPartDrawings(model.PartId ?? Guid.Empty);

            if (part.PartDrawings != null && part.PartDrawings.Count > 0)
            {
                var drawings = new List<DrawingViewModel>();

                foreach (var partDrawing in partDrawings)
                {
                    DrawingViewModel convertedModel = new PartDrawingConverter().ConvertToView(partDrawing);
                    drawings.Add(convertedModel);
                }

                model.Drawings = drawings;
            }

            var partLayouts = _partRepository.GetPartLayouts(model.PartId ?? Guid.Empty);

            if (part.PartLayouts != null && part.PartLayouts.Count > 0)
            {
                var layouts = new List<LayoutViewModel>();

                foreach (var partLayout in partLayouts)
                {
                    LayoutViewModel convertedModel = new PartLayoutConverter().ConvertToView(partLayout);
                    layouts.Add(convertedModel);
                }

                model.Layouts = layouts;
            }

            if (_partRepository != null)
            {
                _partRepository.Dispose();
                _partRepository = null;
            }

            if (_dynamicsPartRepository != null)
            {
                _dynamicsPartRepository.Dispose();
                _dynamicsPartRepository = null;
            }

            if (_projectPartRepository != null)
            {
                _projectPartRepository.Dispose();
                _projectPartRepository = null;
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

            if (_foundryOrderRepository != null)
            {
                _foundryOrderRepository.Dispose();
                _foundryOrderRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert project part to part model for Create part
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public Part ConvertToCreatePart(ProjectPart projectPart)
        {
            Part part = new Part();

            var _projectPartRepository = new ProjectPartRepository();

            var projectPartDrawings = _projectPartRepository.GetProjectPartDrawings(projectPart.ProjectPartId);
            var projectPartLayouts = _projectPartRepository.GetProjectPartLayouts(projectPart.ProjectPartId);

            part.PartId = projectPart.PartId ?? Guid.Empty;
            part.Number = projectPart.Number;
            part.AccountCode = projectPart.AccountCode;
            part.CustomerId = projectPart.CustomerId;
            part.CustomerAddressId = projectPart.CustomerAddressId;
            part.FoundryId = projectPart.FoundryId;
            part.SubFoundryId = projectPart.SubFoundryId;
            part.HtsNumberId = projectPart.HtsNumberId;
            part.MaterialId = projectPart.MaterialId;
            part.MaterialSpecificationId = projectPart.MaterialSpecificationId;
            part.PartStatusId = projectPart.PartStatusId;
            part.PartTypeId = projectPart.PartTypeId;
            part.ShipmentTermId = projectPart.ShipmentTermId;
            part.PaymentTermId = projectPart.PaymentTermId;
            part.SurchargeId = projectPart.SurchargeId;
            part.SiteId = projectPart.SiteId;
            part.DestinationId = projectPart.DestinationId;
            part.CoatingTypeId = projectPart.CoatingTypeId;
            part.PatternMaterialId = projectPart.PatternMaterialId;
            part.IsRaw = projectPart.IsRaw;
            part.IsMachined = projectPart.IsMachined;
            part.PalletQuantity = projectPart.PalletQuantity;
            part.AdditionalCost = projectPart.AdditionalCost;
            part.FixtureDate = projectPart.FixtureDate;
            part.FixtureCost = projectPart.FixtureCost;
            part.FixturePrice = projectPart.FixturePrice;
            part.PatternDate = projectPart.PatternDate;
            part.PatternCost = projectPart.PatternCost;
            part.PatternPrice = projectPart.PatternPrice;
            part.IsFamilyPattern = projectPart.IsFamilyPattern;
            part.FoundryOrderId = projectPart.FoundryOrderId;
            part.ToolingDescription = projectPart.ToolingDescription;
            part.Notes = projectPart.Notes;
            part.AnnualUsage = projectPart.AnnualUsage;
            part.IsActive = true;

            if (projectPartDrawings != null && projectPartDrawings.Count > 0)
            {
                part.PartDrawings = new List<PartDrawing>();
                foreach (var projectPartDrawing in projectPartDrawings)
                {
                    var partDrawing = new PartDrawingConverter().ConvertToDomain(projectPartDrawing);
                    part.PartDrawings.Add(partDrawing);
                }
            }

            if (projectPartLayouts != null && projectPartLayouts.Count > 0)
            {
                part.PartLayouts = new List<PartLayout>();
                foreach (var projectPartLayout in projectPartLayouts)
                {
                    var partLayout = new PartLayoutConverter().ConvertToDomain(projectPartLayout);
                    part.PartLayouts.Add(partLayout);
                }
            }

            if (_projectPartRepository != null)
            {
                _projectPartRepository.Dispose();
                _projectPartRepository = null;
            }

            return part;
        }

        /// <summary>
        /// convert project part to part master
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public IV00101_Part_Master ConvertToCreateMaster(ProjectPart projectPart)
        {
            IV00101_Part_Master part = new IV00101_Part_Master();

            part.ITEMNMBR = projectPart.Number;
            part.ITEMDESC = projectPart.Description;
            part.ITEMSHWT = Convert.ToInt32(projectPart.Weight * 100.00m);
            part.STNDCOST = projectPart.Cost;
            part.LOCNCODE = projectPart.SiteId;
            part.ITMCLSCD = "STANDARD";
            part.PRICMTHD = 2;
            part.INACTIVE = 0;

            return part;
        }

        /// <summary>
        /// convert project part to part currency
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public IV00105_Part_Currency ConvertToCreateCurrency(ProjectPart projectPart)
        {
            IV00105_Part_Currency part = new IV00105_Part_Currency();

            part.ITEMNMBR = projectPart.Number;
            part.CURNCYID = string.Empty;
            part.LISTPRCE = projectPart.Price;

            return part;
        }

        /// <summary>
        /// convert projectPart to part price option
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public IV00107_Part_Price_Option ConvertToCreatePriceOption(ProjectPart projectPart)
        {
            IV00107_Part_Price_Option part = new IV00107_Part_Price_Option();

            part.ITEMNMBR = projectPart.Number;
            part.PRCLEVEL = "STANDARD";
            part.UOFM = "part";
            part.CURNCYID = string.Empty;

            return part;
        }

        /// <summary>
        /// convert project part to part price
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public IV00108_Part_Price ConvertToCreatePrice(ProjectPart projectPart)
        {
            IV00108_Part_Price part = new IV00108_Part_Price();

            part.ITEMNMBR = projectPart.Number;
            part.PRCLEVEL = "STANDARD";
            part.UOFM = "part";

            return part;
        }

        /// <summary>
        /// convert project part to vendor master
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public IV00103_Part_Vendor_Master ConvertToCreateVendor(ProjectPart projectPart)
        {
            IV00103_Part_Vendor_Master part = new IV00103_Part_Vendor_Master();

            part.ITEMNMBR = projectPart.Number;
            part.VENDORID = projectPart.FoundryId;
            part.VNDITNUM = projectPart.Number;

            return part;
        }

        /// <summary>
        /// convert part model to domain for update part
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Part ConvertToUpdatePart(PartOperationModel model)
        {
            Part part = new Part();

            var _partStatusRepository = new PartStatusRepository();

            var partStatus = _partStatusRepository.GetPartStatus(model.PartStatusId);

            var active = (partStatus != null && (!partStatus.Description.ToLower().Replace(" ", string.Empty).Equals("inactive") ||
                                                 !partStatus.Description.ToLower().Replace(" ", string.Empty).Equals("archive") ||
                                                 !partStatus.Description.ToLower().Replace(" ", string.Empty).Equals("purge"))) ? true : false;

            part.PartId = model.PartId ?? Guid.Empty;
            part.Number = model.PartNumber;
            part.AccountCode = model.AccountCode;
            part.CustomerId = model.CustomerId;
            part.CustomerAddressId = model.CustomerAddressId;
            part.FoundryId = model.FoundryId;
            part.SubFoundryId = model.SubFoundryId;
            part.HtsNumberId = model.HtsNumberId;
            part.MaterialId = model.MaterialId;
            part.MaterialSpecificationId = model.MaterialSpecificationId;
            part.PartStatusId = model.PartStatusId;
            part.PartTypeId = model.PartTypeId;
            part.ShipmentTermId = model.ShipmentTermId;
            part.PaymentTermId = model.PaymentTermId;
            part.SurchargeId = model.SurchargeId;
            part.SiteId = model.SiteId;
            part.DestinationId = model.DestinationId;
            part.CoatingTypeId = model.CoatingTypeId;
            part.PatternMaterialId = model.PatternMaterialId;
            part.IsRaw = model.IsRaw;
            part.IsMachined = model.IsMachined;
            part.PalletQuantity = model.PalletQuantity;
            part.AdditionalCost = model.AdditionalCost;
            part.FixtureDate = model.FixtureDate;
            part.FixtureCost = model.FixtureCost;
            part.FixturePrice = model.FixturePrice;
            part.PatternDate = model.PatternDate;
            part.PatternCost = model.PatternCost;
            part.PatternPrice = model.PatternPrice;
            part.IsFamilyPattern = model.IsFamilyPattern;
            part.Notes = model.Notes;
            part.MinimumQuantity = model.MinimumQuantity;
            part.SafetyQuantity = model.SafetyQuantity;
            part.OrderCycle = model.OrderCycle;
            part.LeadTime = model.LeadTime;
            part.AnnualUsage = model.AnnualUsage;
            part.FoundryOrderId = model.FoundryOrderId;
            part.ToolingDescription = model.ToolingDescription;
            part.IsActive = active;

            if (_partStatusRepository != null)
            {
                _partStatusRepository.Dispose();
                _partStatusRepository = null;
            }

            return part;
        }

        /// <summary>
        /// convert part model to part master for update part master
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IV00101_Part_Master ConvertToUpdateMaster(PartOperationModel model)
        {
            IV00101_Part_Master partMaster = new IV00101_Part_Master();

            var _partStatusRepository = new PartStatusRepository();

            var partStatus = _partStatusRepository.GetPartStatus(model.PartStatusId);

            var active = (partStatus != null && (!partStatus.Description.ToLower().Replace(" ", string.Empty).Equals("inactive") ||
                                                 !partStatus.Description.ToLower().Replace(" ", string.Empty).Equals("archive") ||
                                                 !partStatus.Description.ToLower().Replace(" ", string.Empty).Equals("purge"))) ? true : false;

            partMaster.ITEMNMBR = model.PartNumber;
            partMaster.ITEMDESC = model.PartDescription;
            partMaster.ITEMSHWT = Convert.ToInt32(model.Weight * 100.00m);
            partMaster.STNDCOST = model.Cost;
            partMaster.LOCNCODE = model.SiteId;
            partMaster.INACTIVE = Convert.ToByte(active);
            partMaster.ITMCLSCD = "STANDRRD";

            if (_partStatusRepository != null)
            {
                _partStatusRepository.Dispose();
                _partStatusRepository = null;
            }

            return partMaster;
        }

        /// <summary>
        /// convert part model to part currency for update part currency
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IV00105_Part_Currency ConvertToUpdateCurrency(PartOperationModel model)
        {
            IV00105_Part_Currency partCurrency = new IV00105_Part_Currency();

            partCurrency.ITEMNMBR = model.PartNumber;
            partCurrency.LISTPRCE = model.Price;
            partCurrency.CURNCYID = string.Empty;

            return partCurrency;
        }

        /// <summary>
        /// convert part model to part vendor master for update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IV00103_Part_Vendor_Master ConvertToUpdatePartVendor(PartOperationModel model)
        {
            IV00103_Part_Vendor_Master partVendor = new IV00103_Part_Vendor_Master();

            partVendor.ITEMNMBR = model.PartNumber;
            partVendor.VENDORID = model.FoundryId;
            partVendor.VNDITNUM = model.PartNumber;

            return partVendor;
        }

        /// <summary>
        /// convert part model to part quantity master for update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IV00102_Part_Quantity_Master ConvertToUpdatePartQuantity(PartOperationModel model)
        {
            IV00102_Part_Quantity_Master partQuantity = new IV00102_Part_Quantity_Master();

            partQuantity.ITEMNMBR = model.PartNumber;
            partQuantity.PRIMVNDR = model.FoundryId;

            return partQuantity;
        }
    }
}