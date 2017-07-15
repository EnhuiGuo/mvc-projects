using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Operations.Models;

namespace SouthlandMetals.Web.Converters
{
    public class CreditMemoItemConverter
    {
        /// <summary>
        /// convert creditMemoItem to view model
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public CreditMemoItemViewModel ConvertToView(CreditMemoItem item)
        {
            CreditMemoItemViewModel model = new CreditMemoItemViewModel();

            model.CreditMemoItemId = item.CreditMemoItemId;
            model.CreditMemoId = item.CreditMemoId;
            model.ItemQuantity = item.Quantity;
            model.ItemDescription = (!string.IsNullOrEmpty(item.Description)) ? item.Description : "N/A";
            model.ItemPrice = item.Price;
            model.ExtendedPrice = item.Quantity * item.Price;

            return model;
        }

        /// <summary>
        /// convert debitMemoItem view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CreditMemoItem ConvertFromDebitMemoItem(DebitMemoItemViewModel model)
        {
            CreditMemoItem item = new CreditMemoItem();

            item.CreditMemoItemId = model.CreditMemoItemId;
            item.CreditMemoId = model.CreditMemoId;
            item.Quantity = model.ItemQuantity;
            item.Description = model.ItemDescription;
            item.Price = model.ItemPrice;
            item.PartNumber = model.PartNumber;
            item.DateCode = model.DateCode;
            item.Reason = model.Reason;

            return item;
        }

        /// <summary>
        /// convert creditMemoItem view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CreditMemoItem ConvertToDomain(CreditMemoItemViewModel model)
        {
            CreditMemoItem item = new CreditMemoItem();

            item.CreditMemoItemId = model.CreditMemoItemId;
            item.CreditMemoId = model.CreditMemoId;
            item.Quantity = model.ItemQuantity;
            item.Description = model.ItemDescription;
            item.Price = model.ItemPrice;

            return item;
        }
    }
}