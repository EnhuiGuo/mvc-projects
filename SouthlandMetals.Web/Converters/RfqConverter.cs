using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class RfqConverter
    {
        /// <summary>
        /// convert rfq to list model
        /// </summary>
        /// <param name="rfq"></param>
        /// <returns></returns>
        public RfqViewModel ConvertToListView(Rfq rfq)
        {
            RfqViewModel model = new RfqViewModel();

            var _projectRepository = new ProjectRepository();

            var project = _projectRepository.GetProject(rfq.ProjectId);

            model.RfqId = rfq.RfqId;
            model.ProjectName = (project != null && !string.IsNullOrEmpty(project.Name)) ? project.Name : "N/A";
            model.RfqNumber = (!string.IsNullOrEmpty(rfq.Number)) ? rfq.Number : "N/A";
            model.RfqDate = rfq.RfqDate;
            model.HoldNotes = rfq.HoldNotes;
            model.HoldExpirationDate = rfq.HoldExpirationDate;
            model.RfqDateStr = rfq.RfqDate.ToShortDateString();
            model.IsOpen = rfq.IsOpen;
            model.IsHold = rfq.IsHold;
            model.IsCanceled = rfq.IsCanceled;
            model.Status = rfq.IsOpen ? "Open" : rfq.IsCanceled ? "Canceled" : rfq.IsHold ? "On Hold" : "N/A";
            model.CreatedDate = rfq.CreatedDate;

            if (_projectRepository != null)
            {
                _projectRepository.Dispose();
                _projectRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert rfq to view model
        /// </summary>
        /// <param name="rfq"></param>
        /// <returns></returns>
        public RfqViewModel ConvertToView(Rfq rfq)
        {
            RfqViewModel model = new RfqViewModel();

            var _projectRepository = new ProjectRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _salespersonDynamicsRepository = new SalespersonDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _countryRepository = new CountryRepository();
            var _shipmentTermRepository = new ShipmentTermRepository();
            var _coatingTypeRepository = new CoatingTypeRepository();
            var _specificationMaterialRepository = new SpecificationMaterialRepository();
            var _priceSheetRepository = new PriceSheetRepository();
            var _projectPartRepository = new ProjectPartRepository();

            var project = _projectRepository.GetProject(rfq.ProjectId);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(rfq.CustomerId);
            var dyanmicsSalesperson = _salespersonDynamicsRepository.GetSalesperson((dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SLPRSNID)) ? dynamicsCustomer.SLPRSNID : string.Empty);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(rfq.FoundryId);
            var country = _countryRepository.GetCountry(rfq.CountryId);
            var shipmentTerm = _shipmentTermRepository.GetShipmentTerm(rfq.ShipmentTermId);
            var coatingType = _coatingTypeRepository.GetCoatingType(rfq.CoatingTypeId);
            var specificationMaterial = _specificationMaterialRepository.GetSpecificationMaterial(rfq.SpecificationMaterialId);
            var quotePriceSheet = _priceSheetRepository.GetQuotePriceSheetByRfq(rfq.RfqId);
            var productionPriceSheet = _priceSheetRepository.GetProductionPriceSheetBrRfq(rfq.RfqId);

            model.RfqId = rfq.RfqId;
            model.ProjectId = rfq.ProjectId;
            model.ProjectName = (project != null && !string.IsNullOrEmpty(project.Name)) ? project.Name : "N/A";
            model.RfqNumber = (!string.IsNullOrEmpty(rfq.Number)) ? rfq.Number : "N/A";
            model.RfqDate = rfq.RfqDate;
            model.RfqDateStr = rfq.RfqDate.ToShortDateString();
            model.CustomerId = rfq.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.FoundryId = rfq.FoundryId;
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.CountryId = rfq.CountryId;
            model.SalespersonId = rfq.SalespersonId;
            model.SalespersonName = (dyanmicsSalesperson != null) ? dyanmicsSalesperson.SLPRSNFN + " " + dyanmicsSalesperson.SPRSNSLN : "N/A";
            model.CoatingTypeId = rfq.CoatingTypeId;
            model.SpecificationMaterialId = rfq.SpecificationMaterialId;
            model.ContactName = rfq.ContactName;         
            model.CountryName = (country != null && !string.IsNullOrEmpty(country.Name)) ? country.Name : "N/A";
            model.Attention = (!string.IsNullOrEmpty(rfq.Attention)) ? rfq.Attention : "N/A";
            model.PrintsSent = (rfq.PrintsSent != null) ? rfq.PrintsSent.Value.ToShortDateString() : "N/A";
            model.SentVia = rfq.SentVia;        
            model.ShipmentTermDescription = (shipmentTerm != null) ? shipmentTerm.Description : "N/A";
            model.IsMachined = rfq.IsMachined;
            model.Packaging = rfq.Packaging;
            model.NumberOfSamples = rfq.NumberOfSamples;
            model.Details = (!string.IsNullOrEmpty(rfq.Attention)) ? rfq.Details : "N/A";
            model.CoatingType = (coatingType != null && !string.IsNullOrEmpty(coatingType.Description)) ? coatingType.Description : "N/A";
            model.CoatingTypeId = rfq.CoatingTypeId;
            model.SpecificationMaterialId = rfq.SpecificationMaterialId;   
            model.SpecificationMaterialDescription = (specificationMaterial != null && !string.IsNullOrEmpty(specificationMaterial.Description)) ? specificationMaterial.Description : "N/A";
            model.ISIRRequired = rfq.ISIRRequired;
            model.SampleCastingAvailable = rfq.SampleCastingAvailable;
            model.MetalCertAvailable = rfq.MetalCertAvailable;
            model.CMTRRequired = rfq.CMTRRequired;
            model.GaugingRequired = rfq.GaugingRequired;
            model.TestBarsRequired = rfq.TestBarsRequired;
            model.Notes = (!string.IsNullOrEmpty(rfq.Notes)) ? rfq.Notes : "N/A";
            model.IsOpen = rfq.IsOpen;
            model.IsHold = rfq.IsHold;
            model.HoldExpirationDate = (rfq.HoldExpirationDate != null) ? rfq.HoldExpirationDate : DateTime.MinValue;
            model.HoldExpirationDateStr = (rfq.HoldExpirationDate != null) ? rfq.HoldExpirationDate.Value.ToShortDateString() : "N/A";
            model.HoldNotes = (!string.IsNullOrEmpty(rfq.HoldNotes)) ? rfq.HoldNotes : "N/A";
            model.IsCanceled = rfq.IsCanceled;
            model.CanceledDate = (rfq.CanceledDate != null) ? rfq.CanceledDate : DateTime.MinValue;
            model.CanceledDateStr = (rfq.CanceledDate != null) ? rfq.CanceledDate.Value.ToShortDateString() : "N/A";
            model.Status = rfq.IsOpen ? "Open" : rfq.IsCanceled ? "Canceled" : rfq.IsHold ? "On Hold" : "N/A";
            model.QuotePriceSheetId = (quotePriceSheet != null) ? quotePriceSheet.PriceSheetId : Guid.Empty;
            model.QuotePriceSheet = (quotePriceSheet != null) ? quotePriceSheet.Number : "N/A";
            model.ProductionPriceSheet = (productionPriceSheet != null) ? productionPriceSheet.Number : "N/A";
            model.MaterialId = rfq.MaterialId;
            model.ShipmentTermId = rfq.ShipmentTermId;
            model.CancelNotes = (!string.IsNullOrEmpty(rfq.CancelNotes)) ? rfq.CancelNotes : "N/A";
            model.HasPriceSheet = (quotePriceSheet != null || productionPriceSheet != null) ? true : false;

            model.RfqParts = new List<RfqPartViewModel>();
            var rfqParts = _projectPartRepository.GetProjectParts().Where(x => x.RfqId == rfq.RfqId).ToList();

            foreach (var rfqPart in rfqParts)
            {
                var rfqPartModel = new RfqPartConverter().ConvertToView(rfqPart);
                model.RfqParts.Add(rfqPartModel);
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
            if (_priceSheetRepository != null)
            {
                _priceSheetRepository.Dispose();
                _priceSheetRepository = null;
            }
            if (_projectPartRepository != null)
            {
                _projectPartRepository.Dispose();
                _projectPartRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert rfq view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Rfq ConvertToDomain(RfqViewModel model)
        {
            Rfq rfq = new Rfq();

            var _countryRepository = new CountryRepository();
            var _projectRepository = new ProjectRepository();
            var _coatingTypeRepository = new CoatingTypeRepository();
            var _specificationMaterialRepository = new SpecificationMaterialRepository();

            var project = _projectRepository.GetProject((model.ProjectName != null && model.ProjectName != null && model.ProjectName != string.Empty) ? model.ProjectName : "N/A");
            var coatingType = _coatingTypeRepository.GetCoatingType((model.CoatingType != null && model.CoatingType != null && model.CoatingType != string.Empty) ? model.CoatingType : "N/A");
            var specificationMaterial = _specificationMaterialRepository.GetSpecificationMaterial((model.SpecificationMaterialDescription != null && model.SpecificationMaterialDescription != null && model.SpecificationMaterialDescription != string.Empty) ? model.SpecificationMaterialDescription : "N/A");
  
            rfq.RfqId = model.RfqId;
            rfq.Number = model.RfqNumber;
            rfq.RfqDate = model.RfqDate;
            rfq.ProjectId = (project != null) ? project.ProjectId : Guid.Empty;
            rfq.CustomerId = model.CustomerId;
            rfq.SalespersonId = model.SalespersonId;
            rfq.FoundryId = model.FoundryId;
            rfq.ContactName = model.ContactName;
            rfq.CountryId = model.CountryId;
            rfq.Attention = model.Attention;
            rfq.PrintsSent = DateTime.Parse(model.PrintsSent);
            rfq.SentVia = model.SentVia;
            rfq.ShipmentTermId = model.ShipmentTermId;
            rfq.IsMachined = model.IsMachined;
            rfq.Packaging = model.Packaging;
            rfq.NumberOfSamples = model.NumberOfSamples;
            rfq.Details = model.Details;
            rfq.CoatingTypeId = (coatingType != null) ? coatingType.CoatingTypeId : Guid.Empty;
            rfq.SpecificationMaterialId = (specificationMaterial != null) ? specificationMaterial.SpecificationMaterialId : Guid.Empty;
            rfq.MaterialId = model.MaterialId;
            rfq.ISIRRequired = model.ISIRRequired;
            rfq.SampleCastingAvailable = model.SampleCastingAvailable;
            rfq.MetalCertAvailable = model.MetalCertAvailable;
            rfq.CMTRRequired = model.CMTRRequired;
            rfq.GaugingRequired = model.GaugingRequired;
            rfq.TestBarsRequired = model.TestBarsRequired;
            rfq.Notes = model.Notes;
            rfq.IsOpen = model.IsOpen;
            rfq.IsHold = model.IsHold;
            rfq.IsCanceled = model.IsCanceled;
            rfq.HoldExpirationDate = (model.IsHold) ? model.HoldExpirationDate : null;
            rfq.HoldNotes = (model.IsHold) ? model.HoldNotes : null;
            rfq.CanceledDate = (model.IsCanceled) ? model.CanceledDate : null;
            rfq.CancelNotes = (model.IsCanceled) ? model.CancelNotes : null;

            if (_countryRepository != null)
            {
                _countryRepository.Dispose();
                _countryRepository = null;
            }

            if (_projectRepository != null)
            {
                _projectRepository.Dispose();
                _projectRepository = null;
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

            return rfq;
        }
    }
}