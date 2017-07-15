using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class DebitMemoItemConverter
    {
        /// <summary>
        /// convert debitMemo item to view model
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public DebitMemoItemViewModel ConvertToView(DebitMemoItem item)
        {
            DebitMemoItemViewModel model = new DebitMemoItemViewModel();

            var _creditMemoRepository = new CreditMemoRepository();

            var creditMemo = _creditMemoRepository.GetCreditMemoByDebitMemo(item.DebitMemoId);
            var creditMemoItems = _creditMemoRepository.GetCreditMemoItems((creditMemo != null) ? creditMemo.CreditMemoId : Guid.Empty);


            model.DebitMemoItemId = item.DebitMemoItemId;
            model.DebitMemoId = item.DebitMemoId;
            model.ItemQuantity = item.Quantity;
            model.ItemDescription = (!string.IsNullOrEmpty(item.Description)) ? item.Description : "N/A";
            model.ItemCost = item.Cost;
            model.ExtendedCost = item.Cost * item.Quantity;
            model.PartNumber = (!string.IsNullOrEmpty(item.PartNumber)) ? item.PartNumber : "N/A";
            model.Reason = (!string.IsNullOrEmpty(item.Reason)) ? item.Reason : "N/A";
            model.DateCode = item.DateCode;

            if (creditMemoItems != null)
            {
                var creditMemoItem = creditMemoItems.FirstOrDefault(x => x.Description == item.Description);
                if (creditMemoItem != null)
                {
                    model.CreditMemoId = creditMemoItem.CreditMemoId;
                    model.CreditMemoItemId = creditMemoItem.CreditMemoItemId;
                    model.ItemPrice = creditMemoItem.Price;
                }
            }

            if (_creditMemoRepository != null)
            {
                _creditMemoRepository.Dispose();
                _creditMemoRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert debitMemo item view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DebitMemoItem ConvertToDomain(DebitMemoItemViewModel model)
        {
            DebitMemoItem item = new DebitMemoItem();

            item.DebitMemoItemId = model.DebitMemoItemId;
            item.DebitMemoId = model.DebitMemoId;
            item.PartNumber = model.PartNumber;
            item.Quantity = model.ItemQuantity;
            item.Description = model.ItemDescription;
            item.Cost = model.ItemCost;
            item.DateCode = model.DateCode;
            item.Reason = model.Reason;

            return item;
        }
    }
}