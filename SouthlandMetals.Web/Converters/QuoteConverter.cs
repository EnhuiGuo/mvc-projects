using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class QuoteConverter
    {
        /// <summary>
        /// convert quote to list model
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public QuoteViewModel ConvertToListView(Quote quote)
        {
            QuoteViewModel model = new QuoteViewModel();

            var _rfqRepository = new RfqRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();

            var rfq = _rfqRepository.GetRfq(quote.RfqId);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(quote.CustomerId);

            model.QuoteId = quote.QuoteId;
            model.QuoteNumber = (!string.IsNullOrEmpty(quote.Number)) ? quote.Number : "N/A";
            model.QuoteDate = quote.QuoteDate;
            model.QuoteDateStr = quote.QuoteDate.ToShortDateString();
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.RfqNumber = (rfq != null && !string.IsNullOrEmpty(rfq.Number)) ? rfq.Number : "N/A";
            model.IsOpen = quote.IsOpen;
            model.IsHold = quote.IsHold;
            model.IsCanceled = quote.IsCanceled;
            model.Status = quote.IsOpen ? "Open" : quote.IsCanceled ? "Canceled" : quote.IsHold ? "On Hold" : "N/A";
            model.CreatedDate = quote.CreatedDate;
            model.HoldNotes = quote.HoldNotes;
            model.HoldExpirationDate = quote.HoldExpirationDate;

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
        /// convert quote to view model
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public QuoteViewModel ConvertToView(Quote quote)
        {
            QuoteViewModel model = new QuoteViewModel();

            var _rfqRepository = new RfqRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            var _stateRepository = new StateRepository();
            var _shipmentTermRepository = new ShipmentTermRepository();
            var _paymentTermRepository = new PaymentTermRepository();
            var _materialRepository = new MaterialRepository();
            var _coatingTypeRepository = new CoatingTypeRepository();
            var _htsNumberRepository = new HtsNumberRepository();
            var _projectReposiotry = new ProjectPartRepository();

            var rfq = _rfqRepository.GetRfq(quote.RfqId);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(quote.CustomerId);
            var dynamicsCustomerAddress = _customerAddressDynamicsRepository.GetCustomerAddress(quote.CustomerAddressId);
            var state = _stateRepository.GetState((dynamicsCustomerAddress != null && !string.IsNullOrEmpty(dynamicsCustomerAddress.STATE)) ? dynamicsCustomerAddress.STATE : string.Empty);
            var stateName = (state != null) ? ", " + state.Name : string.Empty;
            var shipmentTerm = _shipmentTermRepository.GetShipmentTerm(quote.ShipmentTermId);
            var paymentTerm = _paymentTermRepository.GetPaymentTerm(quote.PaymentTermId);
            var material = _materialRepository.GetMaterial(quote.MaterialId);
            var coatingType = _coatingTypeRepository.GetCoatingType(quote.CoatingTypeId);
            var htsNumber = _htsNumberRepository.GetHtsNumber(quote.HtsNumberId);
            var quoteParts = _projectReposiotry.GetProjectParts().Where(x => x.QuoteId == quote.QuoteId).ToList();

            model.QuoteId = quote.QuoteId;
            model.RfqId = quote.RfqId;
            model.PriceSheetId = quote.PriceSheetId;
            model.ProjectId = quote.ProjectId;
            model.QuoteNumber = (!string.IsNullOrEmpty(quote.Number)) ? quote.Number : "N/A";
            model.QuoteDate = quote.QuoteDate;
            model.QuoteDateStr = quote.QuoteDate.ToShortDateString();
            model.RfqNumber = (rfq != null && !string.IsNullOrEmpty(rfq.Number)) ? rfq.Number : "N/A";
            model.Validity = quote.Validity;
            model.ContactName = (!string.IsNullOrEmpty(quote.ContactName)) ? quote.ContactName : "N/A";
            model.CustomerId = quote.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.CustomerAddressId = quote.CustomerAddressId;
            model.CustomerAddress = (dynamicsCustomerAddress != null) ? dynamicsCustomerAddress.ADDRESS1 + " " + dynamicsCustomerAddress.CITY + stateName : "N/A";
            model.ContactCopy = (!string.IsNullOrEmpty(quote.ContactCopy)) ? quote.ContactCopy : "N/A";
            model.ShipmentTermId = quote.ShipmentTermId;
            model.ShipmentTermDescription = (shipmentTerm != null && !string.IsNullOrEmpty(shipmentTerm.Description)) ? shipmentTerm.Description : "N/A";
            model.PaymentTermId = quote.PaymentTermId;
            model.PaymentTermDescription = (paymentTerm != null && !string.IsNullOrEmpty(paymentTerm.Description)) ? paymentTerm.Description : "N/A";
            model.MinimumShipment = (!string.IsNullOrEmpty(quote.MinimumShipment)) ? quote.MinimumShipment : "N/A";
            model.ToolingTermDescription = (!string.IsNullOrEmpty(quote.ToolingTermDescription)) ? quote.ToolingTermDescription : "N/A";
            model.SampleLeadTime = (!string.IsNullOrEmpty(quote.SampleLeadTime)) ? quote.SampleLeadTime : "N/A";
            model.ProductionLeadTime = (!string.IsNullOrEmpty(quote.ProductionLeadTime)) ? quote.ProductionLeadTime : "N/A";
            model.MaterialId = quote.MaterialId;
            model.MaterialDescription = (material != null && !string.IsNullOrEmpty(material.Description)) ? material.Description : "N/A";
            model.CoatingTypeId = quote.CoatingTypeId;
            model.CoatingTypeDescription = (coatingType != null && !string.IsNullOrEmpty(coatingType.Description)) ? coatingType.Description : "N/A";
            model.HtsNumberId = quote.HtsNumberId;    
            model.HtsNumberDescription = (htsNumber != null && !string.IsNullOrEmpty(htsNumber.Description)) ? htsNumber.Description + "(" + (htsNumber.DutyRate * 100).ToString() + "%)" : "N/A";
            model.IsMachined = quote.IsMachined;
            model.Notes = quote.Notes;
            model.IsOpen = quote.IsOpen;
            model.IsHold = quote.IsHold;
            model.HoldExpirationDate = (quote.HoldExpirationDate != null) ? quote.HoldExpirationDate : DateTime.MinValue;
            model.HoldExpirationDateStr = (quote.HoldExpirationDate != null) ? quote.HoldExpirationDate.Value.ToShortDateString() : null;
            model.HoldNotes = (!string.IsNullOrEmpty(quote.HoldNotes)) ? quote.HoldNotes : "N/A";
            model.IsCanceled = quote.IsCanceled;
            model.CanceledDate = (quote.CanceledDate != null) ? quote.CanceledDate : DateTime.MinValue;
            model.CanceledDateStr = (quote.CanceledDate != null) ? quote.CanceledDate.Value.ToShortDateString() : null;
            model.CancelNotes = (!string.IsNullOrEmpty(quote.CancelNotes)) ? quote.CancelNotes : "N/A";
            model.Status = quote.IsOpen ? "Open" : quote.IsCanceled ? "Canceled" : quote.IsHold ? "On Hold" : "N/A";
            model.Machining = quote.IsMachined == true ? "Included" : "Not Included";

            if (quoteParts != null)
            {
                model.QuoteParts = new List<QuotePartViewModel>();
                foreach (var quotePart in quoteParts)
                {
                    var quotePartModel = new QuotePartConverter().ConvertToView(quotePart);
                    model.QuoteParts.Add(quotePartModel);
                }
            }

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

            if (_projectReposiotry != null)
            {
                _projectReposiotry.Dispose();
                _projectReposiotry = null;
            }

            return model;
        }

        /// <summary>
        /// convert rfq to quote view model
        /// </summary>
        /// <param name="rfq"></param>
        /// <returns></returns>
        public QuoteViewModel ConvertToView(Rfq rfq)
        {
            QuoteViewModel model = new QuoteViewModel();

            var _priceSheetRepository = new PriceSheetRepository();
            var _projectPartRepository = new ProjectPartRepository();        
            var _customerDynamicsRepository = new CustomerDynamicsRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(rfq.CustomerId);
            var priceSheet = _priceSheetRepository.GetPriceSheets().FirstOrDefault(x => x.RfqId == rfq.RfqId && x.IsQuote);
            var projectParts = _projectPartRepository.GetProjectParts().Where(x => x.PriceSheetId == ((priceSheet != null) ? priceSheet.PriceSheetId : Guid.Empty));

            model.ProjectId = rfq.ProjectId;
            model.RfqId = rfq.RfqId;
            model.RfqNumber = rfq.Number;
            model.ContactName = rfq.ContactName;
            model.CustomerId = rfq.CustomerId;
            model.CustomerName = (dynamicsCustomer != null) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.MaterialId = rfq.MaterialId;
            model.CoatingTypeId = rfq.CoatingTypeId;
            model.IsMachined = rfq.IsMachined;
            model.Machining = rfq.IsMachined ? "Included" : "Not Included";
            model.PriceSheetId = (priceSheet != null) ? priceSheet.PriceSheetId : Guid.Empty;
            model.Date = DateTime.Now.ToShortDateString();
            model.QuoteDateStr = DateTime.Now.ToShortDateString();

            if (projectParts != null && projectParts.Count() > 0)
            {
                model.MaterialId = projectParts.FirstOrDefault().MaterialId;

                model.QuoteParts = new List<QuotePartViewModel>();
                foreach (var projectPart in projectParts)
                {
                    var quotePartModel = new QuotePartConverter().ConvertToView(projectPart);
                    model.QuoteParts.Add(quotePartModel);
                }
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

            if (_customerDynamicsRepository != null)
            {
                _customerDynamicsRepository.Dispose();
                _customerDynamicsRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert quote view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Quote ConvertToDomain(QuoteViewModel model)
        {
            var _quoteRepository = new QuoteRepository();

            Quote quote = new Quote();

            quote.QuoteId = model.QuoteId;
            quote.ProjectId = model.ProjectId;
            quote.RfqId = model.RfqId;
            quote.PriceSheetId = model.PriceSheetId;
            quote.Number = model.QuoteNumber;
            quote.QuoteDate = model.QuoteDate;
            quote.Validity = model.Validity;
            quote.ContactName = model.ContactName;
            quote.CustomerId = model.CustomerId;
            quote.CustomerAddressId = model.CustomerAddressId;
            quote.ContactCopy = model.ContactCopy;
            quote.ShipmentTermId = model.ShipmentTermId;
            quote.PaymentTermId = model.PaymentTermId;
            quote.MinimumShipment = model.MinimumShipment;
            quote.ToolingTermDescription = model.ToolingTermDescription;
            quote.SampleLeadTime = model.SampleLeadTime;
            quote.ProductionLeadTime = model.ProductionLeadTime;
            quote.MaterialId = model.MaterialId;
            quote.CoatingTypeId = model.CoatingTypeId;
            quote.HtsNumberId = model.HtsNumberId;
            quote.IsMachined = model.IsMachined;
            quote.Notes = model.Notes;
            quote.IsOpen = model.IsOpen;
            quote.IsHold = model.IsHold;
            quote.IsCanceled = model.IsCanceled;
            quote.HoldExpirationDate = (model.IsHold) ? model.HoldExpirationDate : null;
            quote.HoldNotes = (model.IsHold) ? model.HoldNotes : null;
            quote.CanceledDate = (model.IsCanceled) ? model.CanceledDate : null;
            quote.CancelNotes = (model.IsCanceled) ? model.CancelNotes : null;

            return quote;
        }
    }
}