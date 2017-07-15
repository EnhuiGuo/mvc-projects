using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Operations.Models;

namespace SouthlandMetals.Web.Converters
{
    public class BucketConverter
    {
        /// <summary>
        /// convert bucket to view model
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        public BucketViewModel ConvertToView(Bucket bucket)
        {
            BucketViewModel model = new BucketViewModel();

            model.BucketId = bucket.BucketId;
            model.FoundryInvoiceId = bucket.FoundryInvoiceId;
            model.BucketName = (!string.IsNullOrEmpty(bucket.Name)) ? bucket.Name : "N/A";
            model.BucketValue = bucket.Value;

            return model;
        }

        /// <summary>
        /// convert bucket view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Bucket ConvertToDomain(BucketViewModel model)
        {
            Bucket bucket = new Bucket();

            bucket.BucketId = model.BucketId;
            bucket.FoundryInvoiceId = model.FoundryInvoiceId;
            bucket.Name = model.BucketName;
            bucket.Value = model.BucketValue;

            AccountCode accountCode = new AccountCode();

            accountCode.CustomerId = null;
            accountCode.Description = "50100";

            bucket.AccountCode = accountCode;

            return bucket;
        }
    }
}