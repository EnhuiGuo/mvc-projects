using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;

namespace SouthlandMetals.Web.Converters
{
    public class PartInventoryConverter
    {
        /// <summary>
        /// convert part to part inventory list model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public PartInventoryViewModel ConvertToListView(Part part)
        {
            PartInventoryViewModel model = new PartInventoryViewModel();

            var _partRepository = new PartRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _foundryOrderRepository = new FoundryOrderRepository();
            var _dynamicsReceiptRepository = new ReceiptDynamicsRepository();

            var dynamicsPartQty = _dynamicsPartRepository.GetPartQuantityMaster(part.Number);
            var dynamicsPartStatus = _dynamicsPartRepository.GetItemStatus(part.Number);
            var dynamicsPartSales = _dynamicsPartRepository.GetItemSales(part.Number);

            model.PartId = part.PartId;
            model.PartNumber = part.Number;
            model.CustomerId = part.CustomerId;
            model.FoundryId = part.FoundryId;
            model.QuantityOnHand = (dynamicsPartStatus != null) ? Math.Round(dynamicsPartStatus.Quantity_on_Hand, 2) : 0.00m;
            model.Cost = (dynamicsPartStatus != null) ? Math.Round(dynamicsPartStatus.Cost, 2) : 0.00m;
            model.OnOrderQuantity = (dynamicsPartQty != null) ? Math.Round(dynamicsPartQty.QTYONORD, 2) : 0.00m;
            model.ReceiptDate = (dynamicsPartStatus != null && dynamicsPartStatus.Last_Receipt_Date != null) ? dynamicsPartStatus.Last_Receipt_Date : DateTime.MinValue;
            model.ReceiptDateStr = (dynamicsPartStatus != null && dynamicsPartStatus.Last_Receipt_Date != null) ? dynamicsPartStatus.Last_Receipt_Date.ToShortDateString() : "N/A";
            model.ReceiptQuantity = (dynamicsPartQty != null) ? Math.Round(dynamicsPartQty.LRCPTQTY, 2) : 0.00m;
            model.SalesDate = (dynamicsPartStatus != null && dynamicsPartStatus.Last_Sale_Date != null) ? dynamicsPartStatus.Last_Sale_Date : DateTime.MinValue;
            model.SalesDateStr = (dynamicsPartStatus != null && dynamicsPartStatus.Last_Sale_Date != null) ? dynamicsPartStatus.Last_Sale_Date.ToShortDateString() : "N/A";
            model.YearToDateSales = (dynamicsPartSales != null) ? Math.Round(dynamicsPartSales.YTD_SALES, 2) : 0.00m;

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

            return model;
        }
    }
}