using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using SouthlandMetals.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class FoundryInvoiceConverter
    {
        /// <summary>
        /// convert foundry invoice to view model
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public FoundryInvoiceViewModel ConvertToView(FoundryInvoice invoice)
        {
            FoundryInvoiceViewModel model = new FoundryInvoiceViewModel();
           
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _bucketRepository = new BucketRepository();

            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(invoice.FoundryId);
            var buckets = _bucketRepository.GetBuckets().Where(x => x.FoundryInvoiceId == invoice.FoundryInvoiceId).ToList();

            model.FoundryInvoiceId = invoice.FoundryInvoiceId;
            model.BillOfLadingId = invoice.FoundryInvoiceId;
            model.InvoiceNumber = (!string.IsNullOrEmpty(invoice.Number)) ? invoice.Number : "N/A";
            model.InvoiceAmount = invoice.Amount;
            model.ScheduledPaymentDate = (invoice.ScheduledPaymentDate != null) ? invoice.ScheduledPaymentDate : DateTime.MinValue;
            model.ScheduledPaymentDateStr = (invoice.ScheduledPaymentDate != null) ? invoice.ScheduledPaymentDate.Value.ToShortDateString() : "N/A";
            model.ActualPaymentDate = (invoice.ActualPaymentDate != null) ? invoice.ActualPaymentDate : DateTime.MinValue; ;
            model.ActualPaymentDateStr = (invoice.ActualPaymentDate != null) ? invoice.ActualPaymentDate.Value.ToShortDateString() : "N/A";
            model.Notes = (!string.IsNullOrEmpty(invoice.Notes)) ? invoice.Notes : "N/A";
            model.FoundryId = invoice.FoundryId;
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.AirFreight = invoice.AirFreight;
            model.HasBeenProcessed = invoice.HasBeenProcessed;
            model.CreateDate = (invoice.CreatedDate != null) ? invoice.CreatedDate : DateTime.MinValue;
            model.CreateDateStr = (invoice.CreatedDate != null) ? invoice.CreatedDate.Value.ToShortDateString() : "N/A";

            model.Buckets = new List<BucketViewModel>();

            if (buckets != null && buckets.Count > 0)
            {
                foreach (var bucket in buckets)
                {
                    BucketViewModel convertedModel = new BucketConverter().ConvertToView(bucket);

                    model.Buckets.Add(convertedModel);
                }
            }

            if (_foundryDynamicsRepository != null)
            {
                _foundryDynamicsRepository.Dispose();
                _foundryDynamicsRepository = null;
            }

            if (_bucketRepository != null)
            {
                _bucketRepository.Dispose();
                _bucketRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert foundry invoice view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FoundryInvoice ConvertToDomain(FoundryInvoiceViewModel model)
        {
            FoundryInvoice foundryInvoice = new FoundryInvoice();

            foundryInvoice.FoundryInvoiceId = model.FoundryInvoiceId;
            foundryInvoice.FoundryId = model.FoundryId;
            foundryInvoice.Number = model.InvoiceNumber;
            foundryInvoice.Amount = model.InvoiceAmount;
            foundryInvoice.ScheduledPaymentDate = model.ScheduledPaymentDate;
            foundryInvoice.ActualPaymentDate = model.ActualPaymentDate;
            foundryInvoice.Notes = model.Notes;
            foundryInvoice.AirFreight = model.AirFreight;
            foundryInvoice.HasBeenProcessed = model.HasBeenProcessed;

            var buckets = new List<Bucket>();

            if (model.Buckets != null && model.Buckets.Count > 0)
            {
                foreach (var bucket in model.Buckets)
                {
                    Bucket convertedModel = new BucketConverter().ConvertToDomain(bucket);

                    buckets.Add(convertedModel);
                }
            }

            foundryInvoice.Buckets = buckets;

            return foundryInvoice;
        }
    }
}