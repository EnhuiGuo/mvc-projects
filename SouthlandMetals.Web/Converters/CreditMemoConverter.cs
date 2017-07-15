using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class CreditMemoConverter
    {
        /// <summary>
        /// convert memo to list model
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        public CreditMemoViewModel ConvertToListView(CreditMemo memo)
        {
            CreditMemoViewModel model = new CreditMemoViewModel();

            var _debitMemoRepository = new DebitMemoRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();

            var debitMemo = _debitMemoRepository.GetDebitMemo(memo.DebitMemoId);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(memo.CustomerId);

            model.CreditMemoId = memo.CreditMemoId;
            model.CreditMemoNumber = (!string.IsNullOrEmpty(memo.Number)) ? memo.Number : "N/A";
            model.CustomerId = memo.CustomerId;
            model.FoundryId = (debitMemo != null) ? debitMemo.FoundryId : string.Empty;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.DebitMemoId = memo.DebitMemoId;
            model.DebitMemoNumber = (debitMemo != null && !string.IsNullOrEmpty(debitMemo.Number)) ? debitMemo.Number : "N/A";
            model.CreditMemoDate = memo.CreditMemoDate;
            model.CreditMemoDateStr = memo.CreditMemoDate.ToShortDateString();
            model.CreditAmount = memo.Amount;
            model.RmaNumber = (debitMemo != null && !string.IsNullOrEmpty(debitMemo.RmaNumber)) ? debitMemo.RmaNumber : "N/A";

            if (_debitMemoRepository != null)
            {
                _debitMemoRepository.Dispose();
                _debitMemoRepository = null;
            }

            if (_customerDynamicsRepository != null)
            {
                _customerDynamicsRepository.Dispose();
                _customerDynamicsRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert memo to view model
        /// </summary>
        /// <param name="memo"></param>
        /// <returns></returns>
        public CreditMemoViewModel ConvertToView(CreditMemo memo)
        {
            CreditMemoViewModel model = new CreditMemoViewModel();

            var _debitMemoRepository = new DebitMemoRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _salespersonDynamicsRepository = new SalespersonDynamicsRepository();
            var _creditMemoRepository = new CreditMemoRepository();

            var debitMemo = _debitMemoRepository.GetDebitMemo(memo.DebitMemoId);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(memo.CustomerId);
            var dyanmicsSalesperson = _salespersonDynamicsRepository.GetSalesperson(!string.IsNullOrEmpty(memo.SalespersonId) ? memo.SalespersonId : "N/A");
            var items = _creditMemoRepository.GetCreditMemoItems().Where(x => x.CreditMemoId == memo.CreditMemoId).ToList();

            model.CreditMemoId = memo.CreditMemoId;
            model.DebitMemoId = memo.DebitMemoId;
            model.DebitMemoNumber = (debitMemo != null && !string.IsNullOrEmpty(debitMemo.Number)) ? debitMemo.Number : "N/A";
            model.CreditMemoNumber = (!string.IsNullOrEmpty(memo.Number)) ? memo.Number : "N/A";
            model.CreditMemoDate = memo.CreditMemoDate; 
            model.CreditMemoDateStr = memo.CreditMemoDate.ToShortDateString();
            model.CustomerId = memo.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.SalespersonId = memo.SalespersonId;
            model.SalespersonName = (dyanmicsSalesperson != null && !string.IsNullOrEmpty(memo.SalespersonId)) ? dyanmicsSalesperson.SLPRSNFN + " " + dyanmicsSalesperson.SPRSNSLN : "N/A";
            model.CreditAmount = memo.Amount;
            model.CreditMemoNotes = (!string.IsNullOrEmpty(memo.Number)) ? memo.Notes : "N/A";

            if (items != null && items.Count > 0)
            {
                var creditMemoItems = new List<CreditMemoItemViewModel>();

                foreach (var item in items)
                {
                    CreditMemoItemViewModel creditMemoItem = new CreditMemoItemConverter().ConvertToView(item);

                    creditMemoItems.Add(creditMemoItem);
                }

                model.CreditMemoItems = creditMemoItems;
            }

            if (_debitMemoRepository != null)
            {
                _debitMemoRepository.Dispose();
                _debitMemoRepository = null;
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

            return model;
        }

        /// <summary>
        /// convert debitMemo view model to creditMemo domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CreditMemo ConvertFromDebitMemo(DebitMemoViewModel model)
        {
            CreditMemo creditMemo = new CreditMemo();

            var creditMemoNumber = model.DebitMemoNumber.Replace("D", "C");

            creditMemo.CreditMemoId = model.CreditMemoId;
            creditMemo.DebitMemoId = model.DebitMemoId;
            creditMemo.Number = creditMemoNumber;
            creditMemo.CreditMemoDate = DateTime.Parse(model.DebitMemoDateStr);
            creditMemo.CustomerId = model.CustomerId;
            creditMemo.SalespersonId = model.SalespersonId;
            creditMemo.Amount = model.CreditAmount;
            creditMemo.Notes = model.DebitMemoNotes;

            if (model.DebitMemoItems != null && model.DebitMemoItems.Count > 0)
            {
                var creditMemoItems = new List<CreditMemoItem>();

                foreach (var item in model.DebitMemoItems)
                {
                    CreditMemoItem creditMemoItem = new CreditMemoItemConverter().ConvertFromDebitMemoItem(item);

                    creditMemoItems.Add(creditMemoItem);
                }

                creditMemo.CreditMemoItems = creditMemoItems;
            }

            return creditMemo;
        }

        /// <summary>
        /// convert creditMemo view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CreditMemo ConvertToDomain(CreditMemoViewModel model)
        {
            CreditMemo creditMemo = new CreditMemo();

            creditMemo.CreditMemoId = model.CreditMemoId;
            creditMemo.DebitMemoId = model.DebitMemoId;
            creditMemo.Number = model.CreditMemoNumber;
            creditMemo.CreditMemoDate = DateTime.Parse(model.CreditMemoDateStr);
            creditMemo.CustomerId = model.CustomerId;
            creditMemo.SalespersonId = model.SalespersonId;
            creditMemo.Amount = model.CreditAmount;
            creditMemo.Notes = model.CreditMemoNotes;

            if (model.CreditMemoItems != null && model.CreditMemoItems.Count > 0)
            {
                var creditMemoItems = new List<CreditMemoItem>();

                foreach (var item in model.CreditMemoItems)
                {
                    CreditMemoItem creditMemoItem = new CreditMemoItemConverter().ConvertToDomain(item);

                    creditMemoItems.Add(creditMemoItem);
                }

                creditMemo.CreditMemoItems = creditMemoItems;
            }

            return creditMemo;
        }
    }
}