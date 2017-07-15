using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class PriceSheetConverter
    {
        /// <summary>
        /// convert price sheet to list model
        /// </summary>
        /// <param name="priceSheet"></param>
        /// <returns></returns>
        public PriceSheetListModel ConvertToListView(PriceSheet priceSheet)
        {
            PriceSheetListModel model = new PriceSheetListModel();

            var _rfqRepository = new RfqRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();

            var rfq = _rfqRepository.GetRfq(priceSheet.RfqId);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer((rfq != null) ? rfq.CustomerId : string.Empty);

            model.PriceSheetId = priceSheet.PriceSheetId;
            model.Number = (!string.IsNullOrEmpty(priceSheet.Number)) ? priceSheet.Number : "N/A";
            model.Date = (priceSheet.CreatedDate != null) ? priceSheet.CreatedDate : DateTime.MinValue;
            model.DateStr = (priceSheet.CreatedDate != null) ? priceSheet.CreatedDate.Value.ToShortDateString() : "N/A";
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.RfqNumber = (rfq != null && !string.IsNullOrEmpty(rfq.Number)) ? rfq.Number : "N/A";
            model.ProjectMargin = priceSheet.ProjectMargin;
            model.WAF = priceSheet.WAF;
            model.Status = priceSheet.IsQuote ? "Quote" : priceSheet.IsProduction ? "Production" : "N/A";
            model.CreatedDate = (priceSheet.CreatedDate != null) ? priceSheet.CreatedDate : DateTime.MinValue;

            if (_rfqRepository != null)
            {
                _rfqRepository.Dispose();
                _rfqRepository = null;
            }

            if (_customerDynamicsRepository != null)
            {
                _customerDynamicsRepository.Dispose();
                _customerDynamicsRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert price sheet to view model
        /// </summary>
        /// <param name="priceSheet"></param>
        /// <returns></returns>
        public PriceSheetViewModel ConvertToView(PriceSheet priceSheet)
        {
            PriceSheetViewModel model = new PriceSheetViewModel();

            var _projectRepository = new ProjectRepository();
            var _rfqRepository = new RfqRepository();
            var _countryRepository = new CountryRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _priceSheetRepository = new PriceSheetRepository();
            var _quoteRepository = new QuoteRepository();

            var project = _projectRepository.GetProject(priceSheet.ProjectId);
            var rfq = _rfqRepository.GetRfq(priceSheet.RfqId);
            var country = _countryRepository.GetCountry((rfq != null) ? rfq.CountryId : Guid.Empty);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry((rfq != null) ? rfq.FoundryId : string.Empty);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer((rfq != null) ? rfq.CustomerId : string.Empty);

            model.PriceSheetId = priceSheet.PriceSheetId;
            model.Number = (!string.IsNullOrEmpty(priceSheet.Number)) ? priceSheet.Number : "N/A";
            model.RfqId = priceSheet.RfqId;
            model.ProjectMargin = priceSheet.ProjectMargin;
            model.ProjectName = (project != null && !string.IsNullOrEmpty(project.Name)) ? project.Name : "N/A";
            model.RfqNumber = (rfq != null && !string.IsNullOrEmpty(rfq.Number)) ? rfq.Number : "N/A";
            model.Country = (country != null && !string.IsNullOrEmpty(country.Name)) ? country.Name : "N/A";
            model.Foundry = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.Customer = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.WAF = priceSheet.WAF;
            model.AnnualContainer = priceSheet.AnnualContainer;
            model.AnnualDollars = priceSheet.AnnualDollars;
            model.AnnualMargin = priceSheet.AnnualMargin;
            model.AnnualWeight = priceSheet.AnnualWeight;
            model.DollarContainer = priceSheet.DollarContainer;
            model.InsuranceDivisor = priceSheet.InsuranceDivisor;
            model.InsuranceDuty = priceSheet.InsuranceDuty;
            model.InsuranceFreight = priceSheet.InsuranceFreight;
            model.InsurancePercentage = priceSheet.InsurancePercentage;
            model.InsurancePremium = priceSheet.InsurancePremium;
            model.ToolingMargin = priceSheet.ToolingMargin;
            model.FixtureMargin = priceSheet.FixtureMargin;
            model.DueDate = priceSheet.ModifiedDate;
            model.PriceSheetType = priceSheet.IsQuote ? "Quote" : priceSheet.IsProduction ? "Production" : "N/A";
            model.TotalWeight = priceSheet.AnnualWeight;

            model.PriceSheetParts = new List<PriceSheetPartViewModel>();
            model.CostDetailList = new List<PriceSheetCostDetailViewModel>();
            model.PriceDetailList = new List<PriceSheetPriceDetailViewModel>();

            var priceSheetParts = _priceSheetRepository.GetPriceSheetParts(priceSheet.PriceSheetId).ToList();

            if (priceSheetParts != null && priceSheetParts.Count > 0)
            {
                foreach (var priceSheetPart in priceSheetParts)
                {
                    PriceSheetPartViewModel priceSheetPartModel = new PriceSheetPartViewModel();
                    if (priceSheet.IsQuote)
                    {
                        priceSheetPartModel = new PriceSheetPartConverter().ConvertToProjectPartView(priceSheetPart);
                    }
                    else if (priceSheet.IsProduction)
                    {
                        priceSheetPartModel = new PriceSheetPartConverter().ConvertToPartView(priceSheetPart);
                    }
                    model.PriceSheetParts.Add(priceSheetPartModel);

                    PriceSheetCostDetailViewModel costDetail = new PriceSheetPartConverter().ConvertToCostView(priceSheetPart);
                    model.CostDetailList.Add(costDetail);
                    var priceDetail = new PriceSheetPartConverter().ConvertToPriceView(priceSheetPart);
                    model.PriceDetailList.Add(priceDetail);

                    model.TotalAnnualCost += costDetail.AnnualCost;
                    model.TotalAnnualPrice += priceDetail.AnnualPrice;

                    model.PriceSheetParts = model.PriceSheetParts.OrderBy(y => y.PartNumber).ToList();
                    var margin = (model.TotalAnnualPrice - model.TotalAnnualCost) / model.TotalAnnualPrice * 100;
                    model.OverallMargin = margin.ToString("#.##") + '%';
                }
            }

            model.BucketList = _priceSheetRepository.GetPriceSheetBuckets().Where(x => x.PriceSheetId == priceSheet.PriceSheetId).ToList();

            var quotes = _quoteRepository.GetQuotes();

            if (quotes != null && quotes.Count > 0)
            {
                foreach (var quote in quotes)
                {
                    if (quote.PriceSheetId == priceSheet.PriceSheetId)
                    {
                        model.NoEdit = true;
                        break;
                    }
                }
            }

            if (_projectRepository != null)
            {
                _projectRepository.Dispose();
                _projectRepository = null;
            }

            if (_rfqRepository != null)
            {
                _rfqRepository.Dispose();
                _rfqRepository = null;
            }

            if (_countryRepository != null)
            {
                _countryRepository.Dispose();
                _countryRepository = null;
            }

            if (_foundryDynamicsRepository != null)
            {
                _foundryDynamicsRepository.Dispose();
                _foundryDynamicsRepository = null;
            }

            if (_customerDynamicsRepository != null)
            {
                _customerDynamicsRepository.Dispose();
                _customerDynamicsRepository = null;
            }

            if (_priceSheetRepository != null)
            {
                _priceSheetRepository.Dispose();
                _priceSheetRepository = null;
            }

            if (_quoteRepository != null)
            {
                _quoteRepository.Dispose();
                _quoteRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert price sheet view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PriceSheet ConvertToDomain(PriceSheetViewModel model)
        {
            var _rfqRepository = new RfqRepository();

            var rfq = _rfqRepository.GetRfq(model.RfqId);

            var priceSheet = new PriceSheet();

            priceSheet.PriceSheetId = model.PriceSheetId;
            priceSheet.Number = model.Number;
            priceSheet.WAF = model.WAF;
            priceSheet.ProjectMargin = model.ProjectMargin;
            priceSheet.AnnualDollars = model.AnnualDollars;
            priceSheet.AnnualMargin = model.AnnualMargin;
            priceSheet.AnnualWeight = model.AnnualWeight;
            priceSheet.AnnualContainer = model.AnnualContainer;
            priceSheet.DollarContainer = model.DollarContainer;
            priceSheet.InsuranceFreight = model.InsuranceFreight;
            priceSheet.InsurancePercentage = model.InsurancePercentage;
            priceSheet.InsuranceDuty = model.InsuranceDuty;
            priceSheet.InsuranceDivisor = model.InsuranceDivisor;
            priceSheet.InsurancePremium = model.InsurancePremium;
            priceSheet.ToolingMargin = model.ToolingMargin;
            priceSheet.FixtureMargin = model.FixtureMargin;
            priceSheet.IsQuote = model.IsQuote;
            priceSheet.IsProduction = model.IsProduction;
            priceSheet.RfqId = model.RfqId;
            priceSheet.ProjectId = (rfq != null) ? rfq.ProjectId : (Guid?)null;
            priceSheet.PriceSheetBuckets = model.BucketList;

            if (model.CostDetailList != null && model.CostDetailList.Count > 0)
            {
                priceSheet.PriceSheetParts = new List<PriceSheetPart>();

                foreach (var costDetail in model.CostDetailList)
                {
                    var priceSheetPart = new PriceSheetPart();

                    priceSheetPart.PriceSheetPartId = costDetail.PriceSheetPartId;
                    priceSheetPart.PriceSheetId = model.PriceSheetId;
                    priceSheetPart.AddOnCost = costDetail.AddOn;
                    priceSheetPart.AnnualCost = costDetail.AnnualCost;
                    priceSheetPart.AnnualRawCost = costDetail.AnnualRawCost;
                    priceSheetPart.AnnualUsage = costDetail.AnnualUsage;
                    priceSheetPart.AvailableQuantity = (int)costDetail.AnnualUsage;
                    priceSheetPart.Cost = costDetail.Cost;
                    priceSheetPart.DutyCost = costDetail.Duty;
                    priceSheetPart.FixtureCost = costDetail.FixtureCost;
                    priceSheetPart.FOBCost = costDetail.FOBCost;
                    priceSheetPart.MachineCost = costDetail.MachineCost;
                    priceSheetPart.PatternCost = costDetail.PatternCost;
                    priceSheetPart.PNumberCost = costDetail.PNumber;
                    priceSheetPart.ProjectPartId = costDetail.ProjectPartId;
                    priceSheetPart.PartId = null;
                    priceSheetPart.RawCost = costDetail.RawCost;
                    priceSheetPart.SurchargeCost = costDetail.Surcharge;
                    priceSheetPart.IsQuote = model.IsQuote;
                    priceSheetPart.IsProduction = model.IsProduction;               

                    var priceDetail = model.PriceDetailList.FirstOrDefault(x => x.ProjectPartId == costDetail.ProjectPartId);
                    if (priceDetail != null)
                    {
                        priceSheetPart.AddOnPrice = priceDetail.AddOn;
                        priceSheetPart.AnnualPrice = priceDetail.AnnualPrice;
                        priceSheetPart.AnnualRawPrice = priceDetail.AnnualRawPrice;
                        priceSheetPart.DutyPrice = priceDetail.Duty;
                        priceSheetPart.FixturePrice = priceDetail.FixturePrice;
                        priceSheetPart.FOBPrice = priceDetail.FOBPrice;
                        priceSheetPart.MachinePrice = priceDetail.MachinePrice;
                        priceSheetPart.PatternPrice = priceDetail.PatternPrice;
                        priceSheetPart.PNumberPrice = priceDetail.PNumber;
                        priceSheetPart.Price = priceDetail.Price;
                        priceSheetPart.RawPrice = priceDetail.RawPrice;
                        priceSheetPart.SurchargePrice = priceDetail.Surcharge;
                    }

                    priceSheet.PriceSheetParts.Add(priceSheetPart);
                }
            }

            if (_rfqRepository != null)
            {
                _rfqRepository.Dispose();
                _rfqRepository = null;
            }

            return priceSheet;
        }
    }
}