using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Models;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Reporting.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SouthlandMetals.Reporting.Domain.ReportModels;
using SouthlandMetals.Reporting.Domain.Models.ReportModels;

namespace SouthlandMetals.Reporting.Domain.Repositories
{
    public class ReportRepository : IReportRepository, IDisposable
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private SouthlandDb _db;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public ReportRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
            _db = new SouthlandDb();
        }

        /// <summary>
        /// consolidate pallet parts
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        private IEnumerable<PackingListPartReportModel> ConsolidatePalletParts(List<PackingListPartReportModel> parts)
        {
            var packingListParts = new List<PackingListPartReportModel>();
            var pallets = new List<KeyValuePair<string, string>>();
            var palletParts = new List<PackingListPartReportModel>();

            foreach (var part in parts)
            {
                pallets.Add(new KeyValuePair<string, string>(part.PartNumber, part.PalletNumber));
            }

            foreach (var part in parts)
            {
                var palletsByPart = pallets.Where(x => x.Key == part.PartNumber).ToList();
                var distinctPalletsByPart = palletsByPart.Distinct().ToList();
                var partsByPallet = palletsByPart.GroupBy(x => x.Value).ToList();

                if (palletsByPart[0].Value.ToString().Equals(palletsByPart[palletsByPart.Count - 1].Value.ToString()))
                {
                    var newPalletPart = new PackingListPartReportModel()
                    {
                        ShipCode = part.ShipCode,
                        PartId = part.PartId,
                        PartNumber = part.PartNumber,
                        PalletNumber = part.PalletNumber,
                        PalletQuantity = partsByPallet[0].Count(),
                        PalletWeight = (part.PartWeight * (partsByPallet[0].Count() * distinctPalletsByPart.Count)),
                        PalletTotal = distinctPalletsByPart.Count,
                        TotalPalletQuantity = partsByPallet[0].Count() * distinctPalletsByPart.Count,
                        PONumber = part.PONumber,
                        InvoiceNumber = part.InvoiceNumber
                    };

                    palletParts.Add(newPalletPart);
                }
                else
                {
                    var newPalletPart = new PackingListPartReportModel()
                    {
                        ShipCode = part.ShipCode,
                        PartId = part.PartId,
                        PartNumber = part.PartNumber,
                        PalletNumber = palletsByPart[0].Value.ToString() + "-" + palletsByPart[palletsByPart.Count - 1].Value.ToString(),
                        PalletQuantity = partsByPallet[0].Count(),
                        PalletWeight = (part.PartWeight * (partsByPallet[0].Count() * distinctPalletsByPart.Count)),
                        PalletTotal = distinctPalletsByPart.Count,
                        TotalPalletQuantity = partsByPallet[0].Count() * distinctPalletsByPart.Count,
                        PONumber = part.PONumber,
                        InvoiceNumber = part.InvoiceNumber
                    };

                    palletParts.Add(newPalletPart);
                }
            }

            foreach (var part in parts)
            {
                foreach (var palletPart in palletParts)
                {
                    if (part.PartNumber.Equals(palletPart.PartNumber))
                    {
                        var newPart = new PackingListPartReportModel()
                        {
                            ShipCode = part.ShipCode,
                            PartId = part.PartId,
                            PartNumber = part.PartNumber,
                            PalletNumber = palletPart.PalletNumber,
                            PalletQuantity = palletPart.PalletQuantity,
                            PalletWeight = palletPart.PalletWeight,
                            PalletTotal = palletPart.PalletTotal,
                            TotalPalletQuantity = palletPart.TotalPalletQuantity,
                            PONumber = part.PONumber,
                            InvoiceNumber = part.InvoiceNumber
                        };

                        packingListParts.Add(newPart);

                        break;
                    }
                }
            }

            var orderedParts = packingListParts.OrderBy(x => x.ShipCode).ThenBy(y => y.PartNumber).ToList();

            var consolidatedParts = orderedParts.GroupBy(x => x.PartNumber).Select(y => y.FirstOrDefault());

            return consolidatedParts;
        }

        /// <summary>
        /// get rfq data for report
        /// </summary>
        /// <param name="rfqId"></param>
        /// <returns></returns>
        public RfqReportModel GetRfqData(Guid rfqId)
        {
            logger.Debug("Getting rfq data.");

            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _salespersonDynamicsRepository = new SalespersonDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _projectRepository = new ProjectRepository();
            var _countryRepository = new CountryRepository();
            var _shipmentTermRepository = new ShipmentTermRepository();
            var _coatingTypeRepository = new CoatingTypeRepository();
            var _specificationMaterialRepository = new SpecificationMaterialRepository();
            var _priceSheetReposiotry = new PriceSheetRepository();

            var model = new RfqReportModel();

            try
            {
                var existingRfq = _db.Rfq.FirstOrDefault(x => x.RfqId == rfqId);
                var project = _projectRepository.GetProject(existingRfq.ProjectId);
                var country = _countryRepository.GetCountry(existingRfq.CountryId);
                var shipmentTerm = _shipmentTermRepository.GetShipmentTerm(existingRfq.ShipmentTermId ?? Guid.Empty);
                var coatingType = _coatingTypeRepository.GetCoatingType(existingRfq.CoatingTypeId);
                var specificationMaterial = _specificationMaterialRepository.GetSpecificationMaterial(existingRfq.SpecificationMaterialId);
                var quotePriceSheet = _priceSheetReposiotry.GetQuotePriceSheetByRfq(existingRfq.RfqId);
                var productionPriceSheet = _priceSheetReposiotry.GetProductionPriceSheetBrRfq(existingRfq.RfqId);
                var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(existingRfq.CustomerId);
                var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(existingRfq.FoundryId);
                var dyanmicsSalesperson = _salespersonDynamicsRepository.GetSalesperson((dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SLPRSNID)) ? dynamicsCustomer.SLPRSNID : string.Empty);

                model.Date = DateTime.Now.ToShortDateString();
                model.ProjectName = project.Name;
                model.RfqNumber = existingRfq.Number;
                model.RfqDateStr = existingRfq.RfqDate.ToShortDateString();
                model.CustomerName = (dynamicsCustomer != null && (!string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME))) ? dynamicsCustomer.SHRTNAME : "N/A";
                model.FoundryName = (dynamicsFoundry != null && (!string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM))) ? dynamicsFoundry.VENDSHNM : "N/A";
                model.SalespersonName = (dyanmicsSalesperson != null) ? dyanmicsSalesperson.SLPRSNFN + " " + dyanmicsSalesperson.SPRSNSLN : "N/A";
                model.ContactName = existingRfq.ContactName;
                model.CountryName = (country != null) ? country.Name : "N/A";
                model.Attention = existingRfq.Attention;
                model.PrintsSent = existingRfq.PrintsSent.Value.ToShortDateString();
                model.SentVia = existingRfq.SentVia;
                model.ShipmentTermDescription = (shipmentTerm != null) ? shipmentTerm.Description : "N/A";
                model.IsMachined = existingRfq.IsMachined;
                model.Packaging = existingRfq.Packaging;
                model.NumberOfSamples = existingRfq.NumberOfSamples;
                model.Details = existingRfq.Details;
                model.CoatingTypeDescription = (coatingType != null) ? coatingType.Description : "N/A";
                model.SpecificationMaterialDescription = (specificationMaterial != null) ? specificationMaterial.Description : "N/A";
                model.ISIRRequired = existingRfq.ISIRRequired;
                model.SampleCastingAvailable = existingRfq.SampleCastingAvailable;
                model.MetalCertAvailable = existingRfq.MetalCertAvailable;
                model.CMTRRequired = existingRfq.CMTRRequired;
                model.GaugingRequired = existingRfq.GaugingRequired;
                model.TestBarsRequired = existingRfq.TestBarsRequired;
                model.Notes = existingRfq.Notes;
                model.IsCanceled = existingRfq.IsCanceled;
                model.Status = existingRfq.IsOpen ? "Open" : existingRfq.IsCanceled ? "Canceled" : existingRfq.IsHold ? "Hold" : "N/A";
                model.QuotePriceSheet = (quotePriceSheet != null) ? quotePriceSheet.Number : "N/A";
                model.ProductionPriceSheet = (productionPriceSheet != null) ? productionPriceSheet.Number : "N/A";
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting rfq data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }

                if (_salespersonDynamicsRepository != null)
                {
                    _salespersonDynamicsRepository.Dispose();
                    _salespersonDynamicsRepository = null;
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

                if (_countryRepository != null)
                {
                    _countryRepository.Dispose();
                    _countryRepository = null;
                }

                if (_shipmentTermRepository != null)
                {
                    _shipmentTermRepository.Dispose();
                    _shipmentTermRepository = null;
                }

                if (_coatingTypeRepository != null)
                {
                    _coatingTypeRepository.Dispose();
                    _coatingTypeRepository = null;
                }

                if (_specificationMaterialRepository != null)
                {
                    _specificationMaterialRepository.Dispose();
                    _specificationMaterialRepository = null;
                }

                if (_priceSheetReposiotry != null)
                {
                    _priceSheetReposiotry.Dispose();
                    _priceSheetReposiotry = null;
                }
            }

            return model;
        }

        /// <summary>
        /// get rfq parts data for report
        /// </summary>
        /// <param name="rfqId"></param>
        /// <returns></returns>
        public List<RfqPartReportModel> GetRfqPartsData(Guid rfqId)
        {
            logger.Debug("Getting rfq parts data.");

            var rfqParts = new List<RfqPartReportModel>();

            try
            {
                rfqParts = (from p in _db.ProjectPart
                            join m in _db.Material
                                on p.MaterialId equals m.MaterialId into me
                            from materialEntity in me.DefaultIfEmpty()
                            where p.RfqId == rfqId
                            select new RfqPartReportModel()
                            {
                                PartNumber = p.Number,
                                RevisionNumber = p.RevisionNumber,
                                PartDescription = p.Description,
                                Type = p.IsRaw ? "Raw" : "Machine",
                                Weight = p.Weight,
                                AnnualUsage = p.AnnualUsage,
                                MaterialDescription = (materialEntity != null) ? materialEntity.Description : "N/A",
                            }).OrderBy(y => y.PartNumber).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting rfq parts data: { 0} ", ex.ToString());
            }

            return rfqParts;
        }

        /// <summary>
        /// get price sheet data for report
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        public PriceSheetReportModel GetPriceSheetData(Guid priceSheetId)
        {
            logger.Debug("Getting price sheet data.");

            var model = new PriceSheetReportModel();
            var _priceSheetRepository = new PriceSheetRepository();
            var _rfqRepository = new RfqRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _countyRepository = new CountryRepository();

            try
            {
                var priceSheet = _priceSheetRepository.GetPriceSheet(priceSheetId);
                var rfq = _rfqRepository.GetRfq(priceSheet.RfqId);
                var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry((rfq != null) ? rfq.FoundryId : string.Empty);
                var dynamicsCustomer = _customerDynamicsRepository.GetCustomer((rfq != null) ? rfq.CustomerId : string.Empty);
                var country = _countyRepository.GetCountry((rfq != null) ? rfq.CountryId : Guid.Empty);

                model.Number = priceSheet.Number;
                model.WAF = priceSheet.WAF;
                model.ProjectMargin = priceSheet.ProjectMargin;
                model.AnnualDollars = priceSheet.AnnualDollars;
                model.AnnualMargin = priceSheet.AnnualMargin;
                model.AnnualWeight = priceSheet.AnnualWeight;
                model.AnnualContainer = priceSheet.AnnualContainer;
                model.DollarContainer = priceSheet.DollarContainer;
                model.InsuranceFreight = priceSheet.InsuranceFreight;
                model.InsurancePercentage = priceSheet.InsurancePercentage;
                model.InsuranceDuty = priceSheet.InsuranceDuty;
                model.InsuranceDivisor = priceSheet.InsuranceDivisor;
                model.InsurancePremium = priceSheet.InsurancePremium;
                model.ToolingMargin = priceSheet.ToolingMargin;
                model.FixtureMargin = priceSheet.FixtureMargin;
                model.Foundry = (dynamicsFoundry != null && (!string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM))) ? dynamicsFoundry.VENDSHNM : "N/A";
                model.Customer = (dynamicsCustomer != null && (!string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME))) ? dynamicsCustomer.SHRTNAME : "N/A";
                model.Country = (country != null && (!string.IsNullOrEmpty(country.Name))) ? country.Name : "N/A";
                model.AnnualMarginText = model.AnnualMargin.ToString() + '%';
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting price sheet data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_priceSheetRepository != null)
                {
                    _priceSheetRepository.Dispose();
                    _priceSheetRepository = null;
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

                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }

                if (_countyRepository != null)
                {
                    _countyRepository.Dispose();
                    _countyRepository = null;
                }
            }

            return model;
        }

        /// <summary>
        /// get price sheet addon buckets data for report
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        public List<PriceSheetAddOnReportModel> GetPriceSheetAddOnData(Guid priceSheetId)
        {
            logger.Debug("Getting price sheet add-on buckets data.");

            var buckets = new List<PriceSheetAddOnReportModel>();

            try
            {
                buckets = _db.PriceSheetBucket.Where(x => x.PriceSheetId == priceSheetId && x.IsAddOn)
                    .Select(x => new PriceSheetAddOnReportModel
                    {
                        PriceSheetAddOnId = x.PriceSheetBucketId,
                        PriceSheetId = x.PriceSheetId,
                        Value = x.Value,
                        Margin = x.Margin,
                        SellValue = x.SellValue,
                        PNumber = x.PNumber,
                        Name = x.Name,
                    }).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting price sheet add-on buckets data: { 0} ", ex.ToString());
            }

            return buckets;
        }

        /// <summary>
        /// get price sheet surcharge buckets data for report 
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        public List<PriceSheetSurchargeReportModel> GetPriceSheetSurchargeData(Guid priceSheetId)
        {
            logger.Debug("Getting price sheet surcharge buckets data.");

            var buckets = new List<PriceSheetSurchargeReportModel>();

            try
            {
                buckets = _db.PriceSheetBucket.Where(x => x.PriceSheetId == priceSheetId && x.IsSurcharge)
                    .Select(x => new PriceSheetSurchargeReportModel
                    {

                        PriceSheetSurchargeId = x.PriceSheetBucketId,
                        PriceSheetId = x.PriceSheetId,
                        Name = x.Name,
                        Value = x.Value,
                        Margin = x.Margin,
                        SellValue = x.SellValue,

                    }).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting price sheet surcharge buckets data: { 0} ", ex.ToString());
            }

            return buckets;
        }

        /// <summary>
        /// get price sheet duty buckets data for report
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        public List<PriceSheetDutyReportModel> GetPriceSheetDutyData(Guid priceSheetId)
        {
            logger.Debug("Getting price sheet duty buckets data.");

            var buckets = new List<PriceSheetDutyReportModel>();

            try
            {
                buckets = _db.PriceSheetBucket.Where(x => x.PriceSheetId == priceSheetId && x.IsDuty)
                    .Select(x => new PriceSheetDutyReportModel
                    {
                        PriceSheetDutyId = x.PriceSheetBucketId,
                        PriceSheetId = x.PriceSheetId,
                        Name = x.Name,
                        Value = x.Value,
                        Margin = x.Margin,
                        SellValue = x.SellValue,
                    }).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting price sheet duty buckets data: { 0} ", ex.ToString());
            }

            return buckets;
        }

        /// <summary>
        /// get price sheet cost parts data for report
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        public List<PriceSheetCostDetailReportModel> GetPriceSheetCostData(Guid priceSheetId)
        {
            logger.Debug("Getting price sheet cost data.");

            var costs = new List<PriceSheetCostDetailReportModel>();

            try
            {
                costs = (from ps in _db.PriceSheetPart
                         join pp in _db.ProjectPart
                         on ps.ProjectPartId equals pp.ProjectPartId
                         where ps.PriceSheetId == priceSheetId
                         select new PriceSheetCostDetailReportModel
                         {
                             PriceSheetCostDetailId = ps.PriceSheetPartId,
                             PriceSheetId = ps.PriceSheetId,
                             PartNumber = pp.Number,
                             Weight = pp.Weight,
                             AnnualUsage = ps.AnnualUsage,
                             RawCost = ps.RawCost,
                             AnnualRawCost = ps.AnnualRawCost,
                             PNumber = ps.PNumberCost,
                             MachineCost = ps.MachineCost,
                             FOBCost = ps.FOBCost,
                             AddOn = ps.AddOnCost,
                             Surcharge = ps.SurchargeCost,
                             Duty = ps.DutyCost,
                             Cost = ps.Cost,
                             AnnualCost = ps.AnnualCost,
                             FixtureCost = ps.FixtureCost,
                             PatternCost = ps.PatternCost
                         }).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting price sheet cost data: { 0} ", ex.ToString());
            }

            return costs;
        }

        /// <summary>
        /// get price sheet price parts data for report
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        public List<PriceSheetPriceDetailReportModel> GetPriceSheetPriceData(Guid priceSheetId)
        {
            logger.Debug("Getting price sheet pricing data.");

            var prices = new List<PriceSheetPriceDetailReportModel>();

            try
            {
                prices = (from ps in _db.PriceSheetPart
                          join pp in _db.ProjectPart
                          on ps.ProjectPartId equals pp.ProjectPartId
                          where ps.PriceSheetId == priceSheetId
                          select new PriceSheetPriceDetailReportModel
                          {
                              PriceSheetPriceDetailId = ps.PriceSheetPartId,
                              PriceSheetId = ps.PriceSheetId,
                              PartNumber = pp.Number,
                              Weight = pp.Weight,
                              AnnualUsage = ps.AnnualUsage,
                              RawPrice = ps.RawPrice,
                              AnnualRawPrice = ps.AnnualRawPrice,
                              PNumber = ps.PNumberPrice,
                              MachinePrice = ps.MachinePrice,
                              FOBPrice = ps.FOBPrice,
                              AddOn = ps.AddOnPrice,
                              Surcharge = ps.SurchargePrice,
                              Duty = ps.DutyPrice,
                              Price = ps.Price,
                              AnnualPrice = ps.AnnualPrice,
                              FixturePrice = ps.FixturePrice,
                              PatternPrice = ps.PatternPrice
                          }).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting price sheet pricing data: { 0} ", ex.ToString());
            }

            return prices;
        }

        /// <summary>
        /// get quote data for report
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public QuoteReportModel GetQuoteData(Guid quoteId)
        {
            logger.Debug("Getting quote data.");

            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            var _stateRepository = new StateRepository();
            var _salespersonDynamicsRepository = new SalespersonDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _rfqRepository = new RfqRepository();
            var _shipmentTermRepository = new ShipmentTermRepository();
            var _paymentTermRepository = new PaymentTermRepository();
            var _materialRepository = new MaterialRepository();
            var _coatingTypeRepository = new CoatingTypeRepository();
            var _htsNumberRepository = new HtsNumberRepository();

            var model = new QuoteReportModel();

            try
            {
                var existingQuote = _db.Quote.FirstOrDefault(x => x.QuoteId == quoteId);
                var rfq = _rfqRepository.GetRfq(existingQuote.RfqId);
                var shipmentTerm = _shipmentTermRepository.GetShipmentTerm(existingQuote.ShipmentTermId);
                var paymentTerm = _shipmentTermRepository.GetShipmentTerm(existingQuote.PaymentTermId);
                var material = _materialRepository.GetMaterial(existingQuote.MaterialId);
                var coatingType = _coatingTypeRepository.GetCoatingType(existingQuote.CoatingTypeId);
                var htsNumber = _htsNumberRepository.GetHtsNumber(existingQuote.HtsNumberId);
                var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(existingQuote.CustomerId);
                var dynamicsAddress = _customerAddressDynamicsRepository.GetCustomerAddress(dynamicsCustomer.ADRSCODE);

                model.Date = DateTime.Now.ToShortDateString();
                model.QuoteNumber = existingQuote.Number;
                model.QuoteDateStr = existingQuote.QuoteDate.ToShortDateString();
                model.RfqNumber = (rfq != null) ? rfq.Number : string.Empty;
                model.Validity = existingQuote.Validity;
                model.ContactName = existingQuote.ContactName;
                model.CustomerName = (dynamicsCustomer != null && (!string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME))) ? dynamicsCustomer.SHRTNAME : "N/A";
                var stateName = (dynamicsAddress != null && (!string.IsNullOrEmpty(dynamicsAddress.STATE))) ? dynamicsAddress.STATE : string.Empty;
                model.CustomerAddress = (dynamicsAddress != null) ? dynamicsAddress.ADDRESS1 + " " + dynamicsAddress.CITY + ", " + stateName : "N/A";
                model.ContactCopy = existingQuote.ContactCopy;
                model.ShipmentTermDescription = (shipmentTerm != null) ? shipmentTerm.Description : "N/A";
                model.PaymentTermDescription = (paymentTerm != null) ? paymentTerm.Description : "N/A";
                model.MinimumShipment = existingQuote.MinimumShipment;
                model.ToolingTermDescription = existingQuote.ToolingTermDescription;
                model.SampleLeadTime = existingQuote.SampleLeadTime;
                model.ProductionLeadTime = existingQuote.ProductionLeadTime;
                model.MaterialDescription = (material != null) ? material.Description : "N/A";
                model.CoatingTypeDescription = (coatingType != null) ? coatingType.Description : "N/A";
                model.HtsNumberDescription = (htsNumber != null) ? htsNumber.Description + "(" + (htsNumber.DutyRate * 100).ToString() + "%)" : "N/A";
                model.IsMachined = existingQuote.IsMachined;
                model.Notes = existingQuote.Notes;
                model.IsCanceled = existingQuote.IsCanceled;
                model.Status = existingQuote.IsOpen ? "Open" : existingQuote.IsCanceled ? "Canceled" : existingQuote.IsHold ? "Hold" : "N/A";
                model.Machining = existingQuote.IsMachined ? "Included" : "Not Included";
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting quote data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }

                if (_customerAddressDynamicsRepository != null)
                {
                    _customerAddressDynamicsRepository.Dispose();
                    _customerAddressDynamicsRepository = null;
                }

                if (_stateRepository != null)
                {
                    _stateRepository.Dispose();
                    _stateRepository = null;
                }
                if (_salespersonDynamicsRepository != null)
                {
                    _salespersonDynamicsRepository.Dispose();
                    _salespersonDynamicsRepository = null;
                }

                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }

                if (_rfqRepository != null)
                {
                    _rfqRepository.Dispose();
                    _rfqRepository = null;
                }

                if (_shipmentTermRepository != null)
                {
                    _shipmentTermRepository.Dispose();
                    _shipmentTermRepository = null;
                }

                if (_paymentTermRepository != null)
                {
                    _paymentTermRepository.Dispose();
                    _paymentTermRepository = null;
                }

                if (_materialRepository != null)
                {
                    _materialRepository.Dispose();
                    _materialRepository = null;
                }

                if (_coatingTypeRepository != null)
                {
                    _coatingTypeRepository.Dispose();
                    _coatingTypeRepository = null;
                }

                if (_htsNumberRepository != null)
                {
                    _htsNumberRepository.Dispose();
                    _htsNumberRepository = null;
                }
            }

            return model;
        }

        /// <summary>
        /// get quote parts data for report
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public List<QuotePartReportModel> GetQuotePartsData(Guid quoteId)
        {
            logger.Debug("Getting quote parts data.");

            var quoteParts = new List<QuotePartReportModel>();

            try
            {
                quoteParts = _db.ProjectPart.Where(x => x.QuoteId == quoteId)
                                            .Select(y => new QuotePartReportModel()
                                            {
                                                PartNumber = y.Number,
                                                RevisionNumber = y.RevisionNumber,
                                                PartDescription = y.Description,
                                                Weight = y.Weight,
                                                AnnualUsage = y.AnnualUsage,
                                                Price = y.Price,
                                                Cost = y.Cost,
                                                PatternPrice = y.PatternPrice,
                                                PatternCost = y.PatternCost,
                                                FixturePrice = y.FixturePrice,
                                                FixtureCost = y.FixtureCost
                                            })
                                            .OrderBy(y => y.PartNumber).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting quote parts data: { 0} ", ex.ToString());
            }

            return quoteParts;
        }

        /// <summary>
        /// get foundry order data for report
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        public FoundryOrderReportModel GetFoundryOrderData(Guid foundryOrderId)
        {
            logger.Debug("Getting foundry order data.");

            FoundryOrderReportModel model = new FoundryOrderReportModel();

            var _foundryOrderRepository = new FoundryOrderRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            var _orderTermRepository = new OrderTermRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _stateRepository = new StateRepository();

            try
            {
                var foundryOrder = _foundryOrderRepository.GetFoundryOrder(foundryOrderId);
                var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(foundryOrder.CustomerId);
                var dynamicsCustomerAddress = _customerAddressDynamicsRepository.GetCustomerAddress((dynamicsCustomer != null) ? dynamicsCustomer.ADRSCODE : "N/A");
                var orderTerm = _orderTermRepository.GetOrderTerm(foundryOrder.ShipmentTermsId);
                var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(foundryOrder.FoundryId);

                model.Address = (dynamicsFoundry != null) ? dynamicsFoundry.VENDSHNM + Environment.NewLine + dynamicsFoundry.ADDRESS1 + Environment.NewLine + dynamicsFoundry.CITY + dynamicsFoundry.STATE + " " + dynamicsFoundry.ZIPCODE : "N/A";
                model.Customer = (dynamicsCustomer != null && (!string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME))) ? dynamicsCustomer.SHRTNAME : "N/A";
                model.Number = foundryOrder.Number;
                model.ShipDate = (foundryOrder.ShipDate != null) ? foundryOrder.ShipDate.Value.ToShortDateString() : "N/A";
                model.Destination = (dynamicsCustomerAddress != null) ? dynamicsCustomerAddress.ADDRESS1 + Environment.NewLine + dynamicsCustomerAddress.CITY : "N/A";
                model.Date = (foundryOrder.OrderDate != null) ? foundryOrder.OrderDate.ToShortDateString() : "N/A";
                model.ShipVia = foundryOrder.ShipVia;
                model.Notes = foundryOrder.Notes;
                model.ShipmentTerm = (orderTerm != null) ? orderTerm.Description : "N/A";

                var customerOrderNumbers = (from fp in _db.FoundryOrderPart
                                            join cp in _db.CustomerOrderPart
                                             on fp.CustomerOrderPartId equals cp.CustomerOrderPartId
                                            join c in _db.CustomerOrder
                                             on cp.CustomerOrderId equals c.CustomerOrderId
                                            where fp.FoundryOrderId == foundryOrderId
                                            select c.PONumber).ToList();

                customerOrderNumbers = customerOrderNumbers.Distinct().ToList();

                if (customerOrderNumbers != null)
                {
                    for (var i = 0; i < customerOrderNumbers.Count(); i++)
                    {
                        if (i == 0)
                        {
                            model.CustomerOrderNumber += customerOrderNumbers[i];
                        }
                        else
                        {
                            model.CustomerOrderNumber += $", {customerOrderNumbers[i]}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting foundry order data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_foundryOrderRepository != null)
                {
                    _foundryOrderRepository.Dispose();
                    _foundryOrderRepository = null;
                }

                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }

                if (_customerAddressDynamicsRepository != null)
                {
                    _customerAddressDynamicsRepository.Dispose();
                    _customerAddressDynamicsRepository = null;
                }

                if (_orderTermRepository != null)
                {
                    _orderTermRepository.Dispose();
                    _orderTermRepository = null;
                }

                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }

                if (_stateRepository != null)
                {
                    _stateRepository.Dispose();
                    _stateRepository = null;
                }
            }

            return model;
        }

        /// <summary>
        /// get foundry order parts data for report
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        public List<FoundryOrderPartReportModel> GetFoundryOrderPartsData(Guid foundryOrderId)
        {
            logger.Debug("Getting foundry order parts data.");

            var parts = new List<FoundryOrderPartReportModel>();
            var _dynamicsPartRepository = new PartDynamicsRepository();

            try
            {
                var foundryOrder = _db.FoundryOrder.Find(foundryOrderId);

                if (foundryOrder.IsTooling || foundryOrder.IsSample)
                {
                    parts = (from f in _db.FoundryOrderPart
                             join p in _db.ProjectPart
                             on f.ProjectPartId equals p.ProjectPartId
                             where f.FoundryOrderId == foundryOrderId
                             select new FoundryOrderPartReportModel()
                             {
                                 Quantity = f.Quantity,
                                 Number = p.Number,
                                 Description = p.Description,
                                 UnitPrice = p.Price,
                                 Amount = Math.Round(p.Price * f.Quantity),
                             }).OrderBy(x => x.Number).ToList();
                }
                else if (foundryOrder.IsProduction)
                {
                    var masterParts = _dynamicsPartRepository.GetPartMasters().Select(x => new { x.ITEMNMBR, x.ITEMDESC }).ToList();
                    var foundryOrderParts = _db.FoundryOrderPart.ToList();
                    var tempParts = _db.Part.ToList();

                    parts = (from f in foundryOrderParts
                             join p in tempParts
                             on f.PartId equals p.PartId
                             join dp in masterParts
                             on p.Number.Replace(" ", string.Empty).ToLower() equals dp.ITEMNMBR.Replace(" ", string.Empty).ToLower()
                             where f.FoundryOrderId == foundryOrderId
                             select new FoundryOrderPartReportModel()
                             {
                                 Quantity = f.Quantity,
                                 Number = p.Number,
                                 Description = dp.ITEMDESC,
                             }).OrderBy(x => x.Number).ToList();

                    if (parts != null && parts.Count > 0)
                    {
                        foreach (var part in parts)
                        {
                            var dynamicsPartCurrency = _dynamicsPartRepository.GetPartCurrency(part.Number);
                            part.UnitPrice = (dynamicsPartCurrency != null) ? Math.Round(dynamicsPartCurrency.LISTPRCE, 2) : 0.00m;
                            part.Amount = (part.UnitPrice * part.Quantity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting foundry order parts data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_dynamicsPartRepository != null)
                {
                    _dynamicsPartRepository.Dispose();
                    _dynamicsPartRepository = null;
                }
            }

            return parts;
        }

        /// <summary>
        /// get open customer orders data for report
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<OpenOrdersReportModel> GetOpenCustomerOrderData(OpenOrdersReportModel model)
        {
            logger.Debug("Getting open customer orders data.");

            var orderQuery = new List<OpenOrdersReportModel>();

            var _customerOrderRepository = new CustomerOrderRepository();
            var _projectPartRepository = new ProjectPartRepository();
            var _foundryOrderReposiory = new FoundryOrderRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();

            try
            {
                var customerOrders = _customerOrderRepository.GetCustomerOrders().Where(x => !x.IsComplete).ToList();

                if (customerOrders != null && customerOrders.Count > 0)
                {
                    foreach (var customerOrder in customerOrders)
                    {
                        if (customerOrder.CustomerOrderParts != null && customerOrder.CustomerOrderParts.Count > 0)
                        {
                            foreach (var customerOrderPart in customerOrder.CustomerOrderParts)
                            {
                                var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(customerOrder.CustomerId);
                                var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(customerOrder.FoundryId);
                                var projectPart = _projectPartRepository.GetProjectPart(customerOrderPart.ProjectPartId);
                                var foundryOrderParts = _foundryOrderReposiory.GetFoundryOrderParts().Where(x => x.CustomerOrderPartId == customerOrderPart.CustomerOrderPartId).ToList();

                                if (foundryOrderParts != null && foundryOrderParts.Count > 0)
                                {
                                    foreach (var foundryOrderPart in foundryOrderParts)
                                    {
                                        if (!foundryOrderPart.HasBeenReceived)
                                        {
                                            OpenOrdersReportModel temp = new OpenOrdersReportModel();

                                            temp.PartNumber = (projectPart != null) ? projectPart.Number : "N/A";
                                            temp.Quantity = customerOrderPart.Quantity;
                                            temp.OrderNumber = customerOrder.PONumber;
                                            temp.PONumber = (foundryOrderPart.FoundryOrder != null) ? foundryOrderPart.FoundryOrder.Number : "N/A";
                                            temp.ShipDate = (customerOrder.ShipDate != null) ? customerOrder.ShipDate.Value.ToShortDateString() : "N/A";
                                            temp.FoundryId = (dynamicsFoundry != null) ? dynamicsFoundry.VENDORID : string.Empty;
                                            temp.CustomerId = (dynamicsCustomer != null) ? dynamicsCustomer.CUSTNMBR : string.Empty;
                                            temp.OrderType = customerOrder.IsSample ? "Sample" : customerOrder.IsTooling ? "Tooling" : customerOrder.IsProduction ? "Production" : "N/A";
                                            temp.IsSample = customerOrder.IsSample;
                                            temp.IsTooling = customerOrder.IsTooling;
                                            temp.IsProduction = customerOrder.IsProduction;
                                            temp.Customer = (dynamicsCustomer != null && (!string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME))) ? dynamicsCustomer.SHRTNAME : "N/A";
                                            temp.PODate = customerOrder.PODate.ToShortDateString();
                                            temp.DueDate = customerOrder.DueDate.Value.ToShortDateString();
                                            temp.Status =  customerOrder.IsHold ? "Hold" : customerOrder.IsCanceled ? "Canceled" : "Open";
                                            temp.ShipCode = foundryOrderPart.ShipCode;
                                            temp.Date = DateTime.Now.ToShortDateString();
                                            temp.IsScheduled = foundryOrderPart.IsScheduled;
                                            orderQuery.Add(temp);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (model.FoundryId != null && (!string.IsNullOrEmpty(model.FoundryId)))
                {
                    orderQuery = orderQuery.Where(x => x.FoundryId.Replace(" ", string.Empty).ToLower() == model.FoundryId.Replace(" ", string.Empty).ToLower()).ToList();
                }

                if (model.CustomerId != null && (!string.IsNullOrEmpty(model.CustomerId)))
                {
                    orderQuery = orderQuery.Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == model.CustomerId.Replace(" ", string.Empty).ToLower()).ToList();
                }

                if (model.OrderTypeId != null && model.OrderTypeId != string.Empty)
                {
                    if (model.OrderTypeId.Equals("Sample"))
                    {
                        orderQuery = orderQuery.Where(x => x.IsSample).ToList();
                    }
                    else if (model.OrderTypeId.Equals("Tooling"))
                    {
                        orderQuery = orderQuery.Where(x => x.IsTooling).ToList();
                    }
                    else if (model.OrderTypeId.Equals("Production"))
                    {
                        orderQuery = orderQuery.Where(x => x.IsProduction).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting open customer orders data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_customerOrderRepository != null)
                {
                    _customerOrderRepository.Dispose();
                    _customerOrderRepository = null;
                }

                if (_projectPartRepository != null)
                {
                    _projectPartRepository.Dispose();
                    _projectPartRepository = null;
                }

                if (_foundryOrderReposiory != null)
                {
                    _foundryOrderReposiory.Dispose();
                    _foundryOrderReposiory = null;
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

            return orderQuery;
        }

        /// <summary>
        /// get unattached customer orders data for report
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<OpenOrdersReportModel> GetUnattachedCustomerOrderData(OpenOrdersReportModel model)
        {
            logger.Debug("Getting unattached customer orders data.");

            var orders = new List<OpenOrdersReportModel>();

            var _customerOrderRepository = new CustomerOrderRepository();
            var _projectPartRepository = new ProjectPartRepository();
            var _foundryOrderReposiory = new FoundryOrderRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();

            try
            {
                var customerOrders = _customerOrderRepository.GetCustomerOrders().Where(x => !x.IsComplete).ToList();

                if (customerOrders != null && customerOrders.Count > 0)
                {
                    foreach (var customerOrder in customerOrders)
                    {
                        var customerOrderParts = _customerOrderRepository.GetCustomerOrderParts().Where(x => x.CustomerOrderId == customerOrder.CustomerOrderId).ToList();

                        if (customerOrder.CustomerOrderParts != null && customerOrder.CustomerOrderParts.Count > 0)
                        {
                            foreach (var customerOrderPart in customerOrder.CustomerOrderParts)
                            {
                                var foundryOrderParts = _foundryOrderReposiory.GetFoundryOrderParts().Where(x => x.CustomerOrderPartId == customerOrderPart.CustomerOrderPartId).ToList();

                                if(foundryOrderParts == null && foundryOrderParts.Count < 1)
                                {
                                    var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(customerOrder.CustomerId);
                                    var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(customerOrder.FoundryId);
                                    var projectPart = _projectPartRepository.GetProjectPart(customerOrderPart.ProjectPartId);

                                    OpenOrdersReportModel temp = new OpenOrdersReportModel();

                                    temp.Date = DateTime.Now.ToShortDateString();
                                    temp.CustomerId = (dynamicsCustomer != null) ? dynamicsCustomer.CUSTNMBR : string.Empty;
                                    temp.Customer = (dynamicsCustomer != null && (!string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME))) ? dynamicsCustomer.SHRTNAME : "N/A";
                                    temp.OrderNumber = customerOrder.PONumber;
                                    temp.PODate = customerOrder.PODate.ToShortDateString();
                                    temp.OrderType = customerOrder.IsSample ? "Sample" : customerOrder.IsTooling ? "Tooling" : customerOrder.IsProduction ? "Production" : "N/A";
                                    temp.IsSample = customerOrder.IsSample;
                                    temp.IsTooling = customerOrder.IsTooling;
                                    temp.IsProduction = customerOrder.IsProduction;
                                    temp.Status = customerOrder.IsHold ? "Hold" : customerOrder.IsCanceled ? "Canceled" : "Open";
                                    temp.PartNumber = (projectPart != null) ? projectPart.Number : "N/A";
                                    temp.Quantity = customerOrderPart.Quantity;

                                    orders.Add(temp);
                                }
                            }
                        }
                    }

                    if (model.CustomerId != null && (!string.IsNullOrEmpty(model.CustomerId)))
                    {
                        orders = orders.Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == model.CustomerId.Replace(" ", string.Empty).ToLower()).ToList();
                    }

                    if (model.OrderTypeId != null && model.OrderTypeId != string.Empty)
                    {
                        if (model.OrderTypeId.Equals("Sample"))
                        {
                            orders = orders.Where(x => x.IsSample).ToList();
                        }
                        else if (model.OrderTypeId.Equals("Tooling"))
                        {
                            orders = orders.Where(x => x.IsTooling).ToList();
                        }
                        else if (model.OrderTypeId.Equals("Production"))
                        {
                            orders = orders.Where(x => x.IsProduction).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting open customer orders data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_customerOrderRepository != null)
                {
                    _customerOrderRepository.Dispose();
                    _customerOrderRepository = null;
                }

                if (_projectPartRepository != null)
                {
                    _projectPartRepository.Dispose();
                    _projectPartRepository = null;
                }

                if (_foundryOrderReposiory != null)
                {
                    _foundryOrderReposiory.Dispose();
                    _foundryOrderReposiory = null;
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

            return orders;
        }

        /// <summary>
        /// get open foundry orders data for report
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<OpenOrdersReportModel> GetOpenFoundryOrderData(OpenOrdersReportModel model)
        {
            logger.Debug("Getting open foundry orders data.");

            var orders = new List<OpenOrdersReportModel>();

            var _foundryOrderRepository = new FoundryOrderRepository();
            var _customerOrderRepository = new CustomerOrderRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _projectPartRepository = new ProjectPartRepository();

            try
            {
                var foundryOrders = _foundryOrderRepository.GetFoundryOrders().Where(x => !x.IsComplete).ToList();

                if (foundryOrders != null && foundryOrders.Count > 0)
                {
                    foreach (var foundryOrder in foundryOrders)
                    {
                        if (foundryOrder.FoundryOrderParts != null && foundryOrder.FoundryOrderParts.Count > 0)
                        {
                            foreach (var foundryOrderPart in foundryOrder.FoundryOrderParts)
                            {
                                if (!foundryOrderPart.HasBeenReceived)
                                {
                                    OpenOrdersReportModel temp = new OpenOrdersReportModel();

                                    var customerOrderPart = _customerOrderRepository.GetCustomerOrderPart(foundryOrderPart.CustomerOrderPartId);
                                    var projectPart = _projectPartRepository.GetProjectPart(foundryOrderPart.ProjectPartId);
                                    var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(foundryOrder.CustomerId);
                                    var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(foundryOrder.FoundryId);

                                    temp.PartNumber = (projectPart != null) ? projectPart.Number : "N/A";
                                    temp.Quantity = foundryOrderPart.Quantity;
                                    temp.PONumber = foundryOrder.Number;
                                    temp.OrderNumber = (customerOrderPart != null) ? (customerOrderPart.CustomerOrder != null) ? customerOrderPart.CustomerOrder.PONumber : "N/A" : "N/A";
                                    temp.OrderType = foundryOrder.IsSample ? "Sample" : foundryOrder.IsTooling ? "Tooling" : foundryOrder.IsProduction ? "Production" : "N/A";
                                    temp.ShipDate = (foundryOrder.ShipDate != null) ? foundryOrder.ShipDate.Value.ToShortDateString() : "N/A";
                                    temp.FoundryId = (dynamicsFoundry != null) ? dynamicsFoundry.VENDORID : string.Empty;
                                    temp.CustomerId = (dynamicsCustomer != null) ? dynamicsCustomer.CUSTNMBR : string.Empty;
                                    temp.IsSample = foundryOrder.IsSample;
                                    temp.IsTooling = foundryOrder.IsTooling;
                                    temp.IsProduction = foundryOrder.IsProduction;
                                    temp.Customer = (dynamicsCustomer != null && (!string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME))) ? dynamicsCustomer.SHRTNAME : "N/A";
                                    temp.PODate = (foundryOrder.PortDate != null) ? foundryOrder.PortDate.Value.ToShortDateString() : "N/A";
                                    temp.DueDate = (foundryOrder.DueDate != null) ? foundryOrder.DueDate.Value.ToShortDateString() : "N/A";
                                    temp.Foundry = (dynamicsFoundry != null && (!string.IsNullOrEmpty(dynamicsFoundry.VENDNAME))) ? dynamicsFoundry.VENDNAME : "N/A";
                                    temp.Status = foundryOrder.IsHold ? "On Hold" : foundryOrder.IsCanceled ? "Canceled" : "Open";
                                    temp.IsScheduled = foundryOrderPart.IsScheduled;
                                    temp.ShipCode = foundryOrderPart.ShipCode;
                                    temp.Date = DateTime.Now.ToShortDateString();

                                    orders.Add(temp);
                                }
                            }
                        }
                    }
                }

                if (model.FoundryId != null && (!string.IsNullOrEmpty(model.FoundryId)))
                {
                    orders = orders.Where(x => x.FoundryId.Replace(" ", string.Empty).ToLower() == model.FoundryId.Replace(" ", string.Empty).ToLower()).OrderBy(y => y.OrderType).ToList();
                }

                if (model.CustomerId != null && (!string.IsNullOrEmpty(model.CustomerId)))
                {
                    orders = orders.Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == model.CustomerId.Replace(" ", string.Empty).ToLower()).OrderBy(y => y.OrderType).ToList();
                }

                if (model.OrderTypeId != null && model.OrderTypeId != string.Empty)
                {
                    if (model.OrderTypeId.Equals("Sample"))
                    {
                        orders = orders.Where(x => x.IsSample).ToList();
                    }
                    else if (model.OrderTypeId.Equals("Tooling"))
                    {
                        orders = orders.Where(x => x.IsTooling).ToList();
                    }
                    else if (model.OrderTypeId.Equals("Production"))
                    {
                        orders = orders.Where(x => x.IsProduction).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting open foundry orders data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_foundryOrderRepository != null)
                {
                    _foundryOrderRepository.Dispose();
                    _foundryOrderRepository = null;
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

            return orders;
        }

        /// <summary>
        /// get packing list data for report
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        public PackingListReportModel GetPackingListData(Guid packingListId)
        {
            logger.Debug("Getting packing list data.");

            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _customerAddressDynamicsReposiotry = new CustomerAddressDynamicsRepository();

            var model = new PackingListReportModel();

            try
            {
                var packingList = _db.PackingList.FirstOrDefault(x => x.PackingListId == packingListId);

                var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(packingList.CustomerId);
                model.CustomerName = (dynamicsCustomer != null && (!string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME))) ? dynamicsCustomer.SHRTNAME : "N/A";
                var dynamicsCustomerAddress = _customerAddressDynamicsReposiotry.GetCustomerAddress((dynamicsCustomer != null) ? dynamicsCustomer.ADRSCODE : string.Empty);
                model.CustomerAddress = (dynamicsCustomerAddress != null) ? dynamicsCustomerAddress.ADDRESS1 + " " + dynamicsCustomerAddress.CITY + dynamicsCustomerAddress.STATE : "N/A";

                model.ShipDate = packingList.ShipDate.ToShortDateString();

                var carrier = _db.Carrier.FirstOrDefault(x => x.CarrierId == packingList.CarrierId);
                model.CarrierName = (carrier != null) ? carrier.Name : "N/A";

                model.Notes = packingList.Notes;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting packing list data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }

                if (_customerAddressDynamicsReposiotry != null)
                {
                    _customerAddressDynamicsReposiotry.Dispose();
                    _customerAddressDynamicsReposiotry = null;
                }
            }

            return model;
        }

        /// <summary>
        /// get packling list parts data for report
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        public IEnumerable<PackingListPartReportModel> GetPackingListPartsData(Guid packingListId)
        {
            logger.Debug("Getting packing list parts data.");

            var parts = new List<PackingListPartReportModel>();

            try
            {
                parts = _db.PackingListPart.Where(x => x.PackingListId == packingListId)
                                                          .Select(y => new PackingListPartReportModel()
                                                          {
                                                              ShipCode = y.ShipCode,
                                                              PalletNumber = y.PalletNumber,
                                                              PartNumber = y.PartNumber,
                                                              PONumber = y.PONumber,
                                                              InvoiceNumber = y.InvoiceNumber,
                                                              PalletQuantity = y.PalletQuantity,
                                                              PalletWeight = y.PalletWeight,
                                                              PalletTotal = y.PalletTotal,
                                                              TotalPalletQuantity = y.TotalPalletQuantity
                                                          }).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting packing list parts data: { 0} ", ex.ToString());
            }

            return parts;
        }

        /// <summary>
        /// get debit memo data for report
        /// </summary>
        /// <param name="debitMemoId"></param>
        /// <returns></returns>
        public List<DebitMemoReportModel> GetDebitMemoData(Guid debitMemoId)
        {
            logger.Debug("Getting debit memo data.");

            var modelList = new List<DebitMemoReportModel>();

            var _foundryDynamicsRepostiory = new FoundryDynamicsRepository();
            var _foundryInvoiceRepository = new FoundryInvoiceRepository();
            var _debitMemoRepository = new DebitMemoRepository();

            try
            {
                var selectedDebitMemo = _debitMemoRepository.GetDebitMemo(debitMemoId);
                var dynamicsFoundry = _foundryDynamicsRepostiory.GetFoundry(selectedDebitMemo.FoundryId);
                var foundryInvoice = _foundryInvoiceRepository.GetFoundryInvoice(selectedDebitMemo.FoundryInvoiceId ?? Guid.Empty);

                var debitMemoItems = _debitMemoRepository.GetDebitMemoItems(debitMemoId);

                if (debitMemoItems != null && debitMemoItems.Count > 0)
                {
                    foreach (var debitMemoItem in debitMemoItems)
                    {
                        DebitMemoReportModel model = new DebitMemoReportModel();

                        model.DebitMemoNumber = selectedDebitMemo.Number;
                        model.InvoiceNumber = (foundryInvoice != null && (!string.IsNullOrEmpty(foundryInvoice.Number))) ? foundryInvoice.Number : "N/A";
                        model.FoundryName = (dynamicsFoundry != null && (!string.IsNullOrEmpty(dynamicsFoundry.VENDNAME))) ? dynamicsFoundry.VENDNAME : "N/A";
                        model.RmaNumber = selectedDebitMemo.RmaNumber;
                        model.TrackingNumber = selectedDebitMemo.TrackingNumber;
                        model.DebitAmount = selectedDebitMemo.Amount;
                        model.DebitMemoDateStr = selectedDebitMemo.CreatedDate.HasValue ? selectedDebitMemo.CreatedDate.Value.ToShortDateString() : "N/A";
                        model.DebitMemoNotes = selectedDebitMemo.Notes;
                        model.Quantity = debitMemoItem.Quantity;
                        model.UnitCost = debitMemoItem.Cost;
                        model.ExtendedCost = debitMemoItem.Quantity * debitMemoItem.Cost;
                        model.Description = debitMemoItem.Description;
                        model.PartNumber = debitMemoItem.PartNumber;
                        model.DateCode = (debitMemoItem.DateCode != null) ? debitMemoItem.DateCode.Value.ToShortDateString() : "N/A";
                        model.Reason = debitMemoItem.Reason;

                        if (selectedDebitMemo.IsOpen)
                        {
                            model.Status = "Open";
                        }
                        if (selectedDebitMemo.IsClosed)
                        {
                            model.Status = "Closed";
                        }

                        model.CreatedBy = selectedDebitMemo.CreatedBy;

                        modelList.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting debit memo data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_foundryDynamicsRepostiory != null)
                {
                    _foundryDynamicsRepostiory.Dispose();
                    _foundryDynamicsRepostiory = null;
                }

                if (_foundryInvoiceRepository != null)
                {
                    _foundryInvoiceRepository.Dispose();
                    _foundryInvoiceRepository = null;
                }

                if (_debitMemoRepository != null)
                {
                    _debitMemoRepository.Dispose();
                    _debitMemoRepository = null;
                }
            }

            return modelList;
        }

        /// <summary>
        /// get open debit memos data for report
        /// </summary>
        /// <returns></returns>
        public List<DebitMemoReportModel> GetOpenDebitMemosData()
        {
            logger.Debug("Getting open debit memos data.");

            var debitMemos = new List<DebitMemoReportModel>();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();

            try
            {
                debitMemos = (from d in _db.DebitMemo
                              join i in _db.FoundryInvoice
                                on d.FoundryInvoiceId equals i.FoundryInvoiceId
                              where d.IsOpen
                              select new DebitMemoReportModel()
                              {
                                  DebitMemoNumber = d.Number,
                                  FoundryId = d.FoundryId,
                                  InvoiceNumber = i.Number,
                                  DebitMemoDate = d.CreatedDate,
                                  DebitAmount = d.Amount,
                                  CreatedBy = d.CreatedBy
                              }).ToList();

                if (debitMemos != null)
                {
                    foreach (var debitMemo in debitMemos)
                    {
                        var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(debitMemo.FoundryId);
                        debitMemo.FoundryName = (dynamicsFoundry != null && (!string.IsNullOrEmpty(dynamicsFoundry.VENDNAME)) ? dynamicsFoundry.VENDNAME : "N/A");
                        debitMemo.DebitMemoDateStr = (debitMemo.DebitMemoDate != null) ? debitMemo.DebitMemoDate.Value.ToShortDateString() : "N/A";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting open debit memos data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }
            }

            return debitMemos;
        }

        /// <summary>
        /// get credit memo data for report
        /// </summary>
        /// <param name="creditMemoId"></param>
        /// <returns></returns>
        public List<CreditMemoReportModel> GetCreditMemoData(Guid creditMemoId)
        {
            logger.Debug("Getting credit memo data.");

            List<CreditMemoReportModel> modelList = new List<CreditMemoReportModel>();

            var _creditMemoRepository = new CreditMemoRepository();

            var memo = _creditMemoRepository.GetCreditMemo(creditMemoId);

            if (memo != null)
            {
                var _customerDynamicsRepository = new CustomerDynamicsRepository();
                var _salespersonDynamicsRepository = new SalespersonDynamicsRepository();

                try
                {
                    var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(memo.CustomerId);
                    var dyanmicsSalesperson = _salespersonDynamicsRepository.GetSalesperson(memo.SalespersonId);
                    var items = _creditMemoRepository.GetCreditMemoItems().Where(x => x.CreditMemoId == memo.CreditMemoId).ToList();

                    foreach (var item in items)
                    {
                        var model = new CreditMemoReportModel();

                        model.CreditMemoId = memo.CreditMemoId;
                        model.CreditMemoNumber = memo.Number;
                        model.CreditMemoDate = memo.CreditMemoDate; ;
                        model.CreditMemoDateStr = memo.CreditMemoDate.ToShortDateString();
                        model.CustomerName = (dynamicsCustomer != null && (!string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME))) ? dynamicsCustomer.SHRTNAME : "N/A";
                        model.SalespersonName = (dyanmicsSalesperson != null) ? dyanmicsSalesperson.SLPRSNFN + " " + dyanmicsSalesperson.SPRSNSLN : "N/A";
                        model.Amount = memo.Amount;
                        model.Notes = memo.Notes;
                        model.Quantity = item.Quantity;
                        model.Description = item.Description;
                        model.CreatedBy = memo.CreatedBy;
                        model.UnitPrice = item.Price;
                        model.ExtendedPrice = item.Price * item.Quantity;

                        modelList.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("Error while getting credit memo data: { 0} ", ex.ToString());
                }
                finally
                {
                    if (_customerDynamicsRepository != null)
                    {
                        _customerDynamicsRepository.Dispose();
                        _customerDynamicsRepository = null;
                    }

                    if (_salespersonDynamicsRepository != null)
                    {
                        _salespersonDynamicsRepository.Dispose();
                        _salespersonDynamicsRepository = null;
                    }

                    if (_creditMemoRepository != null)
                    {
                        _creditMemoRepository.Dispose();
                        _creditMemoRepository = null;
                    }
                }
            }

            return modelList;
        }

        /// <summary>
        /// get shipment analysis data for report
        /// </summary>
        /// <param name="bolId"></param>
        /// <returns></returns>
        public List<ShipmentAnalysisReoprtModel> GetShipmentAnalysisData(Guid bolId)
        {
            logger.Debug("Getting shipment analysis data.");

            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _foundryInvoiceRepository = new FoundryInvoiceRepository();
            var _buckRepository = new BucketRepository();

            var parts = new List<ShipmentAnalysisReoprtModel>();

            try
            {
                var billOfLading = _db.BillOfLading.FirstOrDefault(x => x.BillOfLadingId == bolId);

                var buckets = _buckRepository.GetBuckets().Where(x => x.FoundryInvoiceId == bolId).Select(y => new ShipmentAnalysisReoprtModel()
                {
                    BucketName = y.Name,
                    BucketValue = y.Value
                }).ToList();

                var bucketsTotalValue = buckets.Select(x => x.BucketValue).Sum();
                var foundryInvoice = _foundryInvoiceRepository.GetFoundryInvoice(bolId);
                var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry((foundryInvoice != null) ? foundryInvoice.FoundryId : string.Empty);
                var foundryName = (dynamicsFoundry != null) ? dynamicsFoundry.VENDSHNM : "N/A";
                var vessel = _db.Vessel.Find(billOfLading.Shipment.VesselId);

                parts = (from cp in _db.ContainerPart
                         join c in _db.Container
                         on cp.ContainerId equals c.ContainerId
                         join fp in _db.FoundryOrderPart
                         on cp.FoundryOrderPartId equals fp.FoundryOrderPartId
                         join f in _db.FoundryOrder
                         on fp.FoundryOrderId equals f.FoundryOrderId
                         join pt in _db.Part
                         on fp.PartId equals pt.PartId
                         where c.BillOfLadingId == bolId
                         select new ShipmentAnalysisReoprtModel
                         {
                             ShipCode = fp.ShipCode,
                             PurchaseOrder = f.Number,
                             PartNumber = pt.Number,
                             Quantity = cp.Quantity,
                             Cost = fp.Cost,
                             Extension = fp.Cost * cp.Quantity
                         }).ToList();

                var tempParts = new List<ShipmentAnalysisReoprtModel>();

                if (parts != null && parts.Count() > 0)
                {
                    var shipCodes = parts.Select(x => x.ShipCode).Distinct().ToList();

                    foreach (var shipCode in shipCodes)
                    {
                        var partTotalCost = parts.Where(x => x.ShipCode == shipCode).Select(y => y.Extension).Sum();

                        tempParts = parts.Where(x => x.ShipCode == shipCode).ToList();

                        foreach(var tempPart in tempParts)
                        {
                            tempPart.RealCAndF = foundryInvoice.Amount - bucketsTotalValue;
                            tempPart.Overcharge = foundryInvoice.Amount - partTotalCost;
                        }
                    }
                }

                parts = (from s in tempParts
                         join b in buckets
                                 on s.JoinEqual equals b.JoinEqual
                         select new ShipmentAnalysisReoprtModel
                         {
                             ShipCode = s.ShipCode,
                             Customer = billOfLading.CustomsNumber,
                             BillOfLadingDate = billOfLading.BolDate.ToShortDateString(),
                             CITotal = foundryInvoice.Amount,
                             OceanFrt = 0.00m,
                             RealCAndF = s.RealCAndF,
                             TotalDebit = 0.0m,
                             BucketName = b.BucketName,
                             BucketValue = b.BucketValue,
                             Date = DateTime.Now.ToShortDateString(),
                             Foundry = foundryName,
                             Shipped = billOfLading.BolDate.ToShortDateString(),
                             Vessel = vessel.Name,
                             TotalCost = foundryInvoice.Amount,
                             Overcharge = s.Overcharge,
                             FoundryInvoice = foundryInvoice.Number,
                             PurchaseOrder = s.PurchaseOrder,
                             PartNumber = s.PartNumber,
                             Quantity = s.Quantity,
                             Cost = s.Cost,
                             Extension = s.Extension
                         }).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting shipment analysis data: { 0} ", ex.ToString());
            }
            finally
            {
                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }

                if (_dynamicsPartRepository != null)
                {
                    _dynamicsPartRepository.Dispose();
                    _dynamicsPartRepository = null;
                }

                if (_foundryInvoiceRepository != null)
                {
                    _foundryInvoiceRepository.Dispose();
                    _foundryInvoiceRepository = null;
                }

                if (_buckRepository != null)
                {
                    _buckRepository.Dispose();
                    _buckRepository = null;
                }
            }

            return parts;
        }

        /// <summary>
        /// get internation sales data for report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        public List<SalesReportModel> GetInternationalSalesData(DateTime startDate, DateTime endDate, string country)
        {
            var internationalSaleses = new List<SalesReportModel>();

            string cnnString = ConfigurationManager.AppSettings["dynamicsConnection"];

            try
            {
                SqlConnection cnn = new SqlConnection(cnnString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "mti.GetInvoiceSalesForInternational";
                cmd.Parameters.Add("@startDate", SqlDbType.DateTime);
                cmd.Parameters["@startDate"].Value = startDate;
                cmd.Parameters.Add("@endDate", SqlDbType.DateTime);
                cmd.Parameters["@endDate"].Value = endDate;
                cmd.Parameters.Add("@country", SqlDbType.Char);
                cmd.Parameters["@country"].Value = country;
                cmd.Parameters.Add("@filterSalesTerritory", SqlDbType.Bit);
                cmd.Parameters["@filterSalesTerritory"].Value = 0;


                //add any parameters the stored procedure might require
                cnn.Open();
                var o = cmd.ExecuteReader();
                while (o.Read())
                {
                    SalesReportModel model = new SalesReportModel()
                    {
                        ShipCodeOrder = (o[0] != DBNull.Value) ? o.GetString(0) : "N/A",
                        ShipCode = (o[1] != DBNull.Value) ? o.GetString(1) : "N/A",
                        Customer = (o[3] != DBNull.Value) ? o.GetString(3) : "N/A",
                        Foundry = (o[5] != DBNull.Value) ? o.GetString(5) : "N/A",
                        Country = (o[7] != DBNull.Value) ? o.GetString(7) : "N/A",
                        ExpenseAmount = (o[8] != DBNull.Value) ? o.GetDecimal(8) : 0.00m,
                        CmmCommission = (o[9] != DBNull.Value) ? o.GetDecimal(9) : 0.00m,
                        AssociateCommission = (o[10] != DBNull.Value) ? o.GetDecimal(10) : 0.00m,
                        IncomeAmount = (o[11] != DBNull.Value) ? o.GetDecimal(11) : 0.00m,
                        CurrentDateStr = DateTime.Now.ToShortDateString(),
                        StartDateStr = startDate.ToShortDateString(),
                        EndDateStr = endDate.ToShortDateString()
                    };
                    internationalSaleses.Add(model);
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting international sales data: { 0 } ", ex.ToString());
            }

            return internationalSaleses;
        }

        /// <summary>
        /// get invoice register data for domestic report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="shipCode"></param>
        /// <returns></returns>
        public List<InvoiceReportModel> GetInvoiceRegisterDataForDomestic(DateTime startDate, DateTime endDate, string shipCode)
        {
            var invoices = new List<InvoiceReportModel>();

            string cnnString = ConfigurationManager.AppSettings["dynamicsConnection"];

            try
            {
                SqlConnection cnn = new SqlConnection(cnnString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "mti.GetInvoiceRegisterForDomestic";
                cmd.Parameters.Add("@startDate", SqlDbType.DateTime);
                cmd.Parameters["@startDate"].Value = startDate;
                cmd.Parameters.Add("@endDate", SqlDbType.DateTime);
                cmd.Parameters["@endDate"].Value = endDate;
                cmd.Parameters.Add("@shipCode", SqlDbType.Char);
                cmd.Parameters["@shipCode"].Value = shipCode;
                cmd.Parameters.Add("@filterSalesTerritory", SqlDbType.Bit);
                cmd.Parameters["@filterSalesTerritory"].Value = 0;


                //add any parameters the stored procedure might require
                cnn.Open();
                var o = cmd.ExecuteReader();
                while (o.Read())
                {
                    InvoiceReportModel model = new InvoiceReportModel()
                    {
                        InvoiceNumber = (o[0] != DBNull.Value) ? o.GetString(0) : "N/A",
                        InvoiceAmount = (o[1] != DBNull.Value) ? o.GetDecimal(1) : 0.00m,
                        ShipCode = (o[2] != DBNull.Value) ? o.GetString(2) : "N/A",
                        CustomerName = (o[3] != DBNull.Value) ? o.GetString(3) : "N/A",
                        ExpenseAmount = (o[5] != DBNull.Value) ? o.GetDecimal(5) : 0.00m,
                        ExpenseNotes = (o[6] != DBNull.Value) ? o.GetString(6) : "N/A",
                        ExpenseAccountNumber = (o[7] != DBNull.Value) ? o.GetString(7) : "N/A",
                        ExpenseVendor = (o[8] != DBNull.Value) ? o.GetString(8) : "N/A",
                        ExpenseDateStr = (o[10] != DBNull.Value) ? o.GetDateTime(10).ToShortDateString() : "N/A",
                        IncomeAmount = (o[11] != DBNull.Value) ? o.GetDecimal(11) : 0.00m,
                        IncomeNotes = (o[12] != DBNull.Value) ? o.GetString(12) : "N/A",
                        IncomeAccountNumber = (o[13] != DBNull.Value) ? o.GetString(13) : "N/A",
                        CurrentDateStr = DateTime.Now.ToShortDateString(),
                        StartDateStr = startDate.ToShortDateString(),
                        EndDateStr = endDate.ToShortDateString()
                    };

                    invoices.Add(model);
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting invoice register domestic data: { 0} ", ex.ToString());
            }
            return invoices;
        }

        /// <summary>
        /// get invoice register data for international report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="shipCode"></param>
        /// <returns></returns>
        public List<InvoiceReportModel> GetInvoiceRegisterDataForInternational(DateTime startDate, DateTime endDate, string shipCode)
        {
            var invoices = new List<InvoiceReportModel>();

            string cnnString = ConfigurationManager.AppSettings["dynamicsConnection"];

            try
            {
                SqlConnection cnn = new SqlConnection(cnnString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "mti.GetInvoiceRegisterForInternational";
                cmd.Parameters.Add("@startDate", SqlDbType.DateTime);
                cmd.Parameters["@startDate"].Value = startDate;
                cmd.Parameters.Add("@endDate", SqlDbType.DateTime);
                cmd.Parameters["@endDate"].Value = endDate;
                cmd.Parameters.Add("@shipCode", SqlDbType.Char);
                cmd.Parameters["@shipCode"].Value = shipCode;
                cmd.Parameters.Add("@filterShipCode", SqlDbType.Bit);
                cmd.Parameters["@filterShipCode"].Value = 0;


                //add any parameters the stored procedure might require
                cnn.Open();
                var o = cmd.ExecuteReader();
                while (o.Read())
                {
                    InvoiceReportModel model = new InvoiceReportModel()
                    {
                        InvoiceNumber = (o[0] != DBNull.Value) ? o.GetString(0) : "N/A",
                        InvoiceAmount = (o[1] != DBNull.Value) ? o.GetDecimal(1) : 0.00m,
                        ShipCode = (o[2] != DBNull.Value) ? o.GetString(2) : "N/A",
                        CustomerName = (o[3] != DBNull.Value) ? o.GetString(3) : "N/A",
                        ExpenseAmount = (o[5] != DBNull.Value) ? o.GetDecimal(5) : 0.00m,
                        ExpenseNotes = (o[6] != DBNull.Value) ? o.GetString(6) : "N/A",
                        ExpenseAccountNumber = (o[7] != DBNull.Value) ? o.GetString(7) : "N/A",
                        ExpenseVendor = (o[8] != DBNull.Value) ? o.GetString(8) : "N/A",
                        ExpenseDateStr = (o[10] != DBNull.Value) ? o.GetDateTime(10).ToShortDateString() : "N/A",
                        IncomeAmount = (o[11] != DBNull.Value) ? o.GetDecimal(11) : 0.00m,
                        IncomeNotes = (o[12] != DBNull.Value) ? o.GetString(12) : "N/A",
                        IncomeAccountNumber = (o[13] != DBNull.Value) ? o.GetString(13) : "N/A",
                        CurrentDateStr = DateTime.Now.ToShortDateString(),
                        StartDateStr = startDate.ToShortDateString(),
                        EndDateStr = endDate.ToShortDateString()
                    };

                    invoices.Add(model);
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting invoice register international data: { 0} ", ex.ToString());
            }
            return invoices;
        }

        /// <summary>
        /// get ship code invoice register data report
        /// </summary>
        /// <returns></returns>
        public List<InvoiceReportModel> GetShipCodeInvoiceRegisterData()
        {
            var invoices = new List<InvoiceReportModel>();

            return invoices;
        }

        /// <summary>
        /// get account expense summary data for International report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<AccountSummaryReportModel> GetAccountExpenseSummaryDataForInternational(DateTime startDate, DateTime endDate)
        {
            var expenses = new List<AccountSummaryReportModel>();

            string cnnString = ConfigurationManager.AppSettings["dynamicsConnection"];

            try
            {
                SqlConnection cnn = new SqlConnection(cnnString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "mti.GetInvoiceAccountExpenseSummaryForInternational";
                cmd.Parameters.Add("@startDate", SqlDbType.DateTime);
                cmd.Parameters["@startDate"].Value = startDate; 
                cmd.Parameters.Add("@endDate", SqlDbType.DateTime);
                cmd.Parameters["@endDate"].Value = endDate;


                //add any parameters the stored procedure might require
                cnn.Open();
                var o = cmd.ExecuteReader();
                while (o.Read())
                {
                    AccountSummaryReportModel model = new AccountSummaryReportModel()
                    {
                        Customer = (o[0] != DBNull.Value) ? o.GetString(0) : "N/A",
                        Account = (o[1] != DBNull.Value) ? o.GetString(1) : "N/A",
                        Amount = (o[2] != DBNull.Value) ? o.GetDecimal(2) : 0.00m,
                        CurrentDateStr = DateTime.Now.ToShortDateString(),
                        StartDateStr = startDate.ToShortDateString(),
                        EndDateStr = endDate.ToShortDateString()
                    };
                    expenses.Add(model);
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting account expense summary international data: { 0} ", ex.ToString());
            }

            return expenses;
        }

        /// <summary>
        /// get account expense summary data for domestic report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<AccountSummaryReportModel> GetAccountExpenseSummaryDataForDomestic(DateTime startDate, DateTime endDate)
        {
            var expenses = new List<AccountSummaryReportModel>();

            string cnnString = ConfigurationManager.AppSettings["dynamicsConnection"];

            try
            {
                SqlConnection cnn = new SqlConnection(cnnString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "mti.GetInvoiceAccountExpenseSummaryForDomestic";
                cmd.Parameters.Add("@startDate", SqlDbType.DateTime);
                cmd.Parameters["@startDate"].Value = startDate;
                cmd.Parameters.Add("@endDate", SqlDbType.DateTime);
                cmd.Parameters["@endDate"].Value = endDate;


                //add any parameters the stored procedure might require
                cnn.Open();
                var o = cmd.ExecuteReader();
                while (o.Read())
                {
                    AccountSummaryReportModel model = new AccountSummaryReportModel()
                    {
                        Customer = (o[0] != DBNull.Value) ? o.GetString(0) : "N/A",
                        Account = (o[1] != DBNull.Value) ? o.GetString(1) : "N/A",
                        Amount = (o[2] != DBNull.Value) ? o.GetDecimal(2) : 0.00m,
                        CurrentDateStr = DateTime.Now.ToShortDateString(),
                        StartDateStr = startDate.ToShortDateString(),
                        EndDateStr = endDate.ToShortDateString()
                    };
                    expenses.Add(model);
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting account expense summary domestic data: { 0} ", ex.ToString());
            }

            return expenses;
        }

        /// <summary>
        /// get domestic sales data for report 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<SalesReportModel> GetDomesticSalesData(DateTime startDate, DateTime endDate)
        {
            var domesticSaleses = new List<SalesReportModel>();

            string cnnString = ConfigurationManager.AppSettings["dynamicsConnection"];

            try
            {
                SqlConnection cnn = new SqlConnection(cnnString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "mti.GetInvoiceSalesForDomestic";
                cmd.Parameters.Add("@startDate", SqlDbType.DateTime);
                cmd.Parameters["@startDate"].Value = startDate;
                cmd.Parameters.Add("@endDate", SqlDbType.DateTime);
                cmd.Parameters["@endDate"].Value = endDate;

                //add any parameters the stored procedure might require
                cnn.Open();
                var o = cmd.ExecuteReader();
                while (o.Read())
                {
                    SalesReportModel model = new SalesReportModel()
                    {
                        ShipCodeOrder = (o[0] != DBNull.Value) ? o.GetString(0) : "N/A",
                        Customer = (o[1] != DBNull.Value) ? o.GetString(1) : "N/A",
                        Foundry = (o[2] != DBNull.Value) ? o.GetString(2) : "N/A",
                        ExpenseAmount = (o[3] != DBNull.Value) ? o.GetDecimal(3) : 0.00m,
                        IncomeAmount = (o[4] != DBNull.Value) ? o.GetDecimal(4) : 0.00m,
                        Margin = (o[5] != DBNull.Value) ? o.GetDecimal(5) : 0.00m,
                        CurrentDateStr = DateTime.Now.ToShortDateString(),
                        StartDateStr = startDate.ToShortDateString(),
                        EndDateStr = endDate.ToShortDateString()
                    };

                    domesticSaleses.Add(model);
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting domestic sales data: { 0} ", ex.ToString());
            }

            return domesticSaleses;
        }

        /// <summary>
        /// get sales person commission data for report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        public List<CommissionReportModel> GetSalespersonCommissionData(DateTime startDate, DateTime endDate, string country)
        {
            var commissions = new List<CommissionReportModel>();
            string cnnString = ConfigurationManager.AppSettings["dynamicsConnection"];

            try
            {
                SqlConnection cnn = new SqlConnection(cnnString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "mti.GetInvoiceSalesForInternational";
                cmd.Parameters.Add("@startDate", SqlDbType.DateTime);
                cmd.Parameters["@startDate"].Value = startDate;
                cmd.Parameters.Add("@endDate", SqlDbType.DateTime);
                cmd.Parameters["@endDate"].Value = endDate;
                cmd.Parameters.Add("@country", SqlDbType.Char);
                cmd.Parameters["@country"].Value = country;
                cmd.Parameters.Add("@filterSalesTerritory", SqlDbType.Bit);
                cmd.Parameters["@filterSalesTerritory"].Value = 0;

                //add any parameters the stored procedure might require
                cnn.Open();
                var o = cmd.ExecuteReader();
                while (o.Read())
                {
                    CommissionReportModel model = new CommissionReportModel()
                    {
                        Customer = (o[3] != DBNull.Value) ? o.GetString(3) : "N/A",
                        Foundry = (o[5] != DBNull.Value) ? o.GetString(5) : "N/A",
                        ExpenseAmount = (o[8] != DBNull.Value) ? o.GetDecimal(8) : 0.00m,
                        IncomeAmount = (o[11] != DBNull.Value) ? o.GetDecimal(11) : 0.00m,
                        Margin = (o[5] != DBNull.Value) ? o.GetDecimal(5) : 0.00m,
                        Country = (o[7] != DBNull.Value) ? o.GetString(7) : "N/A",
                        CommissionAmount = (o[9] != DBNull.Value) ? o.GetDecimal(9) : 0.00m,
                        CurrentDateStr = DateTime.Now.ToShortDateString(),
                        StartDateStr = startDate.ToShortDateString(),
                        EndDateStr = endDate.ToShortDateString()
                    };

                    commissions.Add(model);
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting salesperson commission data: { 0} ", ex.ToString());
            }

            return commissions;
        }

        /// <summary>
        /// get foundry invoices data for report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="unscheduled"></param>
        /// <returns></returns>
        public List<InvoiceReportModel> GetFoundryInvoicesData(DateTime startDate, DateTime endDate, bool unscheduled)
        {
            var foundryInvoices = new List<FoundryInvoice>();
            var modelList = new List<InvoiceReportModel>();

            try
            {
                if (unscheduled)
                {
                    foundryInvoices = _db.FoundryInvoice.Where(x => x.ScheduledPaymentDate == null).ToList();
                }
                else
                {
                    foundryInvoices = _db.FoundryInvoice.Where(x => x.ScheduledPaymentDate >= startDate && x.ScheduledPaymentDate <= endDate).ToList();
                }

                if (foundryInvoices != null && foundryInvoices.Count > 0)
                {
                    foreach (var foundryInvoice in foundryInvoices)
                    {
                        InvoiceReportModel model = new InvoiceReportModel();

                        var containerParts = (from fi in foundryInvoices
                                              join b in _db.BillOfLading
                                              on fi.FoundryInvoiceId equals b.BillOfLadingId
                                              join c in _db.Container
                                              on b.BillOfLadingId equals c.BillOfLadingId
                                              join cp in _db.ContainerPart
                                              on c.ContainerId equals cp.ContainerId
                                              select cp).ToList();

                        if (containerParts != null && containerParts.Count > 0)
                        {
                            var shipCodes = new List<string>();
                            foreach (var containerPart in containerParts)
                            {
                                var foundryOrderPart = _db.FoundryOrderPart.FirstOrDefault(x => x.FoundryOrderPartId == containerPart.FoundryOrderPartId);
                                if (foundryOrderPart != null)
                                {
                                    shipCodes.Add(foundryOrderPart.ShipCode);
                                }
                            }

                            if (shipCodes != null)
                            {
                                shipCodes = shipCodes.Distinct().ToList();

                                for (var i = 0; i < shipCodes.Count(); i++)
                                {
                                    if (i == 0)
                                    {
                                        model.ShipCode = shipCodes[i];
                                    }
                                    else
                                    {
                                        model.ShipCode = model.ShipCode + "/" + shipCodes[i];
                                    }
                                }
                            }
                        }

                        model.InvoiceNumber = foundryInvoice.Number;
                        model.ScheduledPaymentDateStr = (foundryInvoice.ScheduledPaymentDate != null) ? foundryInvoice.ScheduledPaymentDate.Value.ToShortDateString() : "N/A";
                        model.InvoiceAmount = foundryInvoice.Amount;
                        model.StartDateStr = startDate.ToShortDateString();
                        model.EndDateStr = endDate.ToShortDateString();

                        modelList.Add(model);
                    }
                }

                modelList = modelList.OrderBy(x => x.ShipCode).ToList();

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error while getting foundry invoices data: { 0} ", ex.ToString());
            }
            finally
            {
                _db.Dispose();
            }

            return modelList;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dynamicsContext.Dispose();
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
