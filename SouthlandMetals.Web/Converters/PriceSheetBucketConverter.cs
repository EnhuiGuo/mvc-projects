using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Operations.Models;

namespace SouthlandMetals.Web.Converters
{
    public class PriceSheetBucketConverter
    {
        /// <summary>
        /// convert price sheet bucket to view model
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        public PriceSheetBucketViewModel ConvertToView(PriceSheetBucket bucket)
        {
            PriceSheetBucketViewModel model = new PriceSheetBucketViewModel();

            model.PriceSheetBucketId = bucket.PriceSheetBucketId;
            model.PriceSheetId = bucket.PriceSheetId;
            model.IsAddOn = bucket.IsAddOn;
            model.IsDuty = bucket.IsDuty;
            model.IsSurcharge = bucket.IsSurcharge;
            model.Margin = bucket.Margin;
            model.Name = (!string.IsNullOrEmpty(bucket.Name)) ? bucket.Name : "N/A";
            model.PNumber = bucket.PNumber;
            model.SellValue = bucket.SellValue;
            model.Value = bucket.Value;

            return model;
        }
    }
}