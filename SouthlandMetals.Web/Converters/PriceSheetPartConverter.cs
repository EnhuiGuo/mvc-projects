using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;

namespace SouthlandMetals.Web.Converters
{
    public class PriceSheetPartConverter
    {
        /// <summary>
        /// convert project part to price sheet part
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public PriceSheetPart ConvertToDomain(ProjectPart projectPart)
        {
            PriceSheetPart priceSheetPart = new PriceSheetPart();

            priceSheetPart.PriceSheetId = projectPart.PriceSheetId ?? Guid.Empty;
            priceSheetPart.ProjectPartId = projectPart.ProjectPartId;
            priceSheetPart.PartId = null;
            priceSheetPart.Cost = projectPart.Cost;
            priceSheetPart.Price = projectPart.Price;
            priceSheetPart.AnnualUsage = projectPart.AnnualUsage;
            priceSheetPart.PatternCost = projectPart.PatternCost;
            priceSheetPart.PatternPrice = projectPart.PatternPrice;
            priceSheetPart.FixturePrice = projectPart.FixturePrice;
            priceSheetPart.FixtureCost = projectPart.FixtureCost;

            return priceSheetPart;
        }

        /// <summary>
        /// convert project part to price sheet part cost view model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public PriceSheetPartViewModel ConvertToCostView(ProjectPart part)
        {
            PriceSheetPartViewModel model = new PriceSheetPartViewModel();

            model.AnnualUsage = Decimal.ToInt32(part.AnnualUsage);
            model.PartNumber = part.Number;
            model.Weight = part.Weight;
            model.ProjectPartId = part.ProjectPartId;

            return model;
        }

        /// <summary>
        /// convert price sheet part to price sheet part cost detail view model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public PriceSheetCostDetailViewModel ConvertToCostView(PriceSheetPart part)
        {
            PriceSheetCostDetailViewModel model = new PriceSheetCostDetailViewModel();

            var _projectPartRepository = new ProjectPartRepository();

            var projectPart = _projectPartRepository.GetProjectPart(part.ProjectPartId);

            model.PriceSheetPartId = part.PriceSheetPartId;
            model.PartNumber = (projectPart != null) ? projectPart.Number : "N/A";
            model.Weight = (projectPart != null) ? projectPart.Weight : 0;
            model.AnnualUsage = part.AnnualUsage;
            model.RawCost = part.RawCost;
            model.AnnualRawCost = part.AnnualRawCost;
            model.PNumber = part.PNumberCost;
            model.MachineCost = part.MachineCost;
            model.FOBCost = part.FOBCost;
            model.AddOn = part.AddOnCost;
            model.Surcharge = part.SurchargeCost;
            model.Duty = part.DutyCost;
            model.Cost = part.Cost;
            model.AnnualCost = part.AnnualCost;
            model.FixtureCost = part.FixtureCost;
            model.PatternCost = part.PatternCost;
            model.ProjectPartId = part.ProjectPartId ?? Guid.Empty;

            if (_projectPartRepository != null)
            {
                _projectPartRepository.Dispose();
                _projectPartRepository = null;
            }

            return model;
            
        }

        /// <summary>
        /// convert price sheet part to price sheet part price detail view model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public PriceSheetPriceDetailViewModel ConvertToPriceView(PriceSheetPart part)
        {
            PriceSheetPriceDetailViewModel model = new PriceSheetPriceDetailViewModel();

            var _projectPartRepository = new ProjectPartRepository();

            var projectPart = _projectPartRepository.GetProjectPart(part.ProjectPartId);

            model.PriceSheetPartId = part.PriceSheetPartId;
            model.PartNumber = (projectPart != null) ? projectPart.Number : "N/A";
            model.Weight = (projectPart != null) ? projectPart.Weight : 0;
            model.AnnualUsage = part.AnnualUsage;
            model.RawPrice = part.RawPrice;
            model.AnnualRawPrice = part.AnnualRawPrice;
            model.PNumber = part.PNumberPrice;
            model.MachinePrice = part.MachinePrice;
            model.FOBPrice = part.FOBPrice;
            model.AddOn = part.AddOnPrice;
            model.Surcharge = part.SurchargePrice;
            model.Duty = part.DutyCost;
            model.Price = part.Price;
            model.AnnualPrice = part.AnnualPrice;
            model.FixturePrice = part.FixturePrice;
            model.PatternPrice = part.PatternPrice;
            model.ProjectPartId = part.ProjectPartId ?? Guid.Empty;

            if (_projectPartRepository != null)
            {
                _projectPartRepository.Dispose();
                _projectPartRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert price sheet part to view model for quote price sheet
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public PriceSheetPartViewModel ConvertToProjectPartView(PriceSheetPart part)
        {
            PriceSheetPartViewModel model = new PriceSheetPartViewModel();

            var _projectPartRepository = new ProjectPartRepository();
            var _priceSheetRepository = new PriceSheetRepository();

            var priceSheet = _priceSheetRepository.GetPriceSheet(part.PriceSheetId);
            var projectPart = _projectPartRepository.GetProjectPart(part.ProjectPartId);

            model.ProjectPartId = part.ProjectPartId ?? Guid.Empty;
            model.PriceSheetPartId = part.PriceSheetPartId;
            model.PriceSheetId = part.PriceSheetId;
            model.PriceSheetNumber = (priceSheet != null) ? priceSheet.Number : "N/A";
            model.PartNumber = (projectPart != null) ? projectPart.Number : "N/A";
            model.PartDescription = (projectPart != null) ? projectPart.Description : "N/A";
            model.AvailableQuantity = part.AvailableQuantity;
            model.CustomerOrderQuantity = part.AvailableQuantity;
            model.UnitPrice = part.Price;
            model.UnitCost = part.Cost;

            if (_projectPartRepository != null)
            {
                _projectPartRepository.Dispose();
                _projectPartRepository = null;
            }

            if (_priceSheetRepository != null)
            {
                _priceSheetRepository.Dispose();
                _priceSheetRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert price sheet part to view model for production price sheet
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public PriceSheetPartViewModel ConvertToPartView(PriceSheetPart part)
        {
            PriceSheetPartViewModel model = new PriceSheetPartViewModel();

            var _partRepository = new PartRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _priceSheetRepository = new PriceSheetRepository();
            var _projectPartRepository = new ProjectPartRepository();

            var priceSheet = _priceSheetRepository.GetPriceSheet(part.PriceSheetId);
            var projectPart = _projectPartRepository.GetProjectPart(part.ProjectPartId);
            var tempPart = _partRepository.GetPart((projectPart != null) ? projectPart.PartId : null);
            var dynamicsPart = _dynamicsPartRepository.GetPartMaster((tempPart != null) ? tempPart.Number : null);

            model.ProjectPartId = part.ProjectPartId ?? Guid.Empty;
            model.PartId = (tempPart != null) ? tempPart.PartId : Guid.Empty;
            model.PriceSheetPartId = part.PriceSheetPartId;
            model.PriceSheetId = part.PriceSheetId;
            model.PriceSheetNumber = (priceSheet != null) ? priceSheet.Number : "N/A";
            model.PartNumber = (tempPart != null) ? tempPart.Number : "N/A";
            model.PartDescription = (dynamicsPart != null && !string.IsNullOrEmpty(dynamicsPart.ITEMDESC)) ? dynamicsPart.ITEMDESC : "N/A";
            model.AvailableQuantity = part.AvailableQuantity;
            model.CustomerOrderQuantity = part.AvailableQuantity;
            model.UnitPrice = part.Price;
            model.UnitCost = part.Cost;

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

            if (_priceSheetRepository != null)
            {
                _priceSheetRepository.Dispose();
                _priceSheetRepository = null;
            }

            return model;
        }
    }
}