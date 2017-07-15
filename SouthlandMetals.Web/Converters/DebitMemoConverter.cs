using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class DebitMemoConverter
    {
        /// <summary>
        /// convert debitMemo to list model
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        public DebitMemoViewModel ConvertToListView(DebitMemo memo)
        {
            DebitMemoViewModel model = new DebitMemoViewModel();

            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _foundryInvoiceRepository = new FoundryInvoiceRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _creditMemoRepository = new CreditMemoRepository();

            var foundryInvoice = _foundryInvoiceRepository.GetFoundryInvoice(memo.FoundryInvoiceId ?? Guid.Empty);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(memo.FoundryId);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(memo.CustomerId);
            var creditMemo = _creditMemoRepository.GetCreditMemoByDebitMemo(memo.DebitMemoId);

            model.DebitMemoId = memo.DebitMemoId;
            model.CustomerId = memo.CustomerId;
            model.FoundryId = (dynamicsFoundry != null) ? dynamicsFoundry.VENDORID : "N/A";
            model.DebitMemoNumber = (!string.IsNullOrEmpty(memo.Number)) ? memo.Number : "N/A";
            model.InvoiceNumber = (foundryInvoice != null && !string.IsNullOrEmpty(foundryInvoice.Number)) ? foundryInvoice.Number : "N/A";
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.DebitMemoDate = memo.DebitMemoDate;
            model.DebitMemoDateStr = memo.DebitMemoDate.ToShortDateString();
            model.DebitAmount = memo.Amount;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.CreditMemoId = (creditMemo != null) ? creditMemo.CreditMemoId : Guid.Empty;
            model.CreditMemoNumber = (creditMemo != null && !string.IsNullOrEmpty(creditMemo.Number)) ? creditMemo.Number : "N/A";
            model.IsOpen = memo.IsOpen;
            model.IsClosed = memo.IsClosed;
            model.CreatedDate = (memo.CreatedDate != null) ? memo.CreatedDate : DateTime.MinValue;
            model.CreatedBy = memo.CreatedBy;

            if (_foundryDynamicsRepository != null)
            {
                _foundryDynamicsRepository.Dispose();
                _foundryDynamicsRepository = null;
            }

            if (_foundryInvoiceRepository != null)
            {
                _foundryInvoiceRepository.Dispose();
                _foundryInvoiceRepository = null;
            }

            if (_customerDynamicsRepository != null)
            {
                _customerDynamicsRepository.Dispose();
                _customerDynamicsRepository = null;
            }

            if (_creditMemoRepository != null)
            {
                _creditMemoRepository.Dispose();
                _creditMemoRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert debitMemo to view model
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        public DebitMemoViewModel ConvertToView(DebitMemo memo)
        {
            DebitMemoViewModel model = new DebitMemoViewModel();

            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _foundryInvoiceRepository = new FoundryInvoiceRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _salespersonDynamicsRepository = new SalespersonDynamicsRepository();
            var _debitMemoRepository = new DebitMemoRepository();
            var _creditMemoRepository = new CreditMemoRepository();

            var foundryInvoice = _foundryInvoiceRepository.GetFoundryInvoice(memo.FoundryInvoiceId ?? Guid.Empty);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(memo.FoundryId);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(memo.CustomerId);
            var dyanmicsSalesperson = _salespersonDynamicsRepository.GetSalesperson((dynamicsCustomer != null) ? dynamicsCustomer.SLPRSNID : string.Empty);
            var creditMemo = _creditMemoRepository.GetCreditMemoByDebitMemo(memo.DebitMemoId);
            var items = _debitMemoRepository.GetDebitMemoItems().Where(x => x.DebitMemoId == memo.DebitMemoId).ToList();
            var attachments = _debitMemoRepository.GetDebitMemoAttachments().Where(x => x.DebitMemoId == memo.DebitMemoId).ToList();

            model.DebitMemoId = memo.DebitMemoId;
            model.FoundryInvoiceId = memo.FoundryInvoiceId;
            model.InvoiceNumber = (foundryInvoice != null && !string.IsNullOrEmpty(foundryInvoice.Number)) ? foundryInvoice.Number : "N/A";
            model.DebitMemoNumber = (!string.IsNullOrEmpty(memo.Number)) ? memo.Number : "N/A";
            model.DebitMemoDate = memo.DebitMemoDate; 
            model.DebitMemoDateStr = (memo.DebitMemoDate != null) ? memo.DebitMemoDate.ToShortDateString() : "N/A";
            model.FoundryId = memo.FoundryId;
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.CustomerId = memo.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.SalespersonId = memo.SalespersonId;
            model.SalespersonName = (dyanmicsSalesperson != null && !string.IsNullOrEmpty(dyanmicsSalesperson.SLPRSNFN)) ? dyanmicsSalesperson.SLPRSNFN + " " + dyanmicsSalesperson.SPRSNSLN : "N/A";
            model.CreditMemoId = (creditMemo != null) ? creditMemo.CreditMemoId : Guid.Empty;
            model.CreditMemoNumber = (creditMemo != null && !string.IsNullOrEmpty(creditMemo.Number)) ? creditMemo.Number : "N/A";
            model.RmaNumber = (!string.IsNullOrEmpty(memo.RmaNumber)) ? memo.RmaNumber : "N/A";
            model.TrackingNumber = (!string.IsNullOrEmpty(memo.TrackingNumber)) ? memo.TrackingNumber : "N/A";
            model.DebitAmount = memo.Amount;
            model.DebitMemoNotes = (!string.IsNullOrEmpty(memo.Notes)) ? memo.Notes : "N/A";
            model.IsOpen = memo.IsOpen;
            model.IsClosed = memo.IsClosed;
            model.Status = memo.IsOpen ? "Open" : memo.IsClosed ? "Closed" : "N/A";

            if (items != null && items.Count > 0)
            {
                var debitMemoItems = new List<DebitMemoItemViewModel>();

                foreach (var item in items)
                {
                    DebitMemoItemViewModel debitMemoItem = new DebitMemoItemConverter().ConvertToView(item);

                    debitMemoItems.Add(debitMemoItem);
                }

                model.DebitMemoItems = debitMemoItems;
            }

            if(attachments != null && attachments.Count > 0)
            {
                var debitMemoAttachments = new List<DebitMemoAttachmentViewModel>();

                foreach (var attachment in attachments)
                {
                    var attachmentModel = new DebitMemoAttachmentConverter().ConvertToView(attachment);

                    debitMemoAttachments.Add(attachmentModel);
                }

                model.Attachments = debitMemoAttachments;
            }

            if (_foundryDynamicsRepository != null)
            {
                _foundryDynamicsRepository.Dispose();
                _foundryDynamicsRepository = null;
            }

            if (_foundryInvoiceRepository != null)
            {
                _foundryInvoiceRepository.Dispose();
                _foundryInvoiceRepository = null;
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

            if (_creditMemoRepository != null)
            {
                _creditMemoRepository.Dispose();
                _creditMemoRepository = null;
            }

            if (_debitMemoRepository != null)
            {
                _debitMemoRepository.Dispose();
                _debitMemoRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert debitMemo view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DebitMemo ConvertToDomain(DebitMemoViewModel model)
        {
            DebitMemo debitMemo = new DebitMemo();

            debitMemo.DebitMemoId = model.DebitMemoId;
            debitMemo.FoundryInvoiceId = model.FoundryInvoiceId;
            debitMemo.Number = model.DebitMemoNumber;
            debitMemo.DebitMemoDate = DateTime.Parse(model.DebitMemoDateStr);
            debitMemo.FoundryId = model.FoundryId;
            debitMemo.CustomerId = model.CustomerId;
            debitMemo.SalespersonId = model.SalespersonId;
            debitMemo.RmaNumber = model.RmaNumber;
            debitMemo.TrackingNumber = model.TrackingNumber;
            debitMemo.Amount = model.DebitAmount;
            debitMemo.Notes = model.DebitMemoNotes;
            debitMemo.IsOpen = model.Status.Equals("Open") ? true : false;
            debitMemo.IsClosed = model.Status.Equals("Closed") ? true : false;

            if (model.DebitMemoItems != null && model.DebitMemoItems.Count > 0)
            {
                var debitMemoItems = new List<DebitMemoItem>();

                foreach (var item in model.DebitMemoItems)
                {
                    DebitMemoItem debitMemoItem = new DebitMemoItemConverter().ConvertToDomain(item);

                    debitMemoItems.Add(debitMemoItem);
                }

                debitMemo.DebitMemoItems = debitMemoItems;
            }

            return debitMemo;
        }
    }
}