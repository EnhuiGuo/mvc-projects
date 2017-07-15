using SouthlandMetals.Reporting.Domain.Models.ReportModels;
using SouthlandMetals.Reporting.Domain.ReportModels;
using System;
using System.Collections.Generic;

namespace SouthlandMetals.Reporting.Domain.Interfaces
{
    public interface IReportRepository : IDisposable
    {
        RfqReportModel GetRfqData(Guid rfqId);

        List<RfqPartReportModel> GetRfqPartsData(Guid rfqId);

        PriceSheetReportModel GetPriceSheetData(Guid priceSheetId);

        List<PriceSheetAddOnReportModel> GetPriceSheetAddOnData(Guid priceSheetId);

        List<PriceSheetSurchargeReportModel> GetPriceSheetSurchargeData(Guid priceSheetId);

        List<PriceSheetDutyReportModel> GetPriceSheetDutyData(Guid priceSheetId);

        List<PriceSheetCostDetailReportModel> GetPriceSheetCostData(Guid priceSheetId);

        List<PriceSheetPriceDetailReportModel> GetPriceSheetPriceData(Guid priceSheetId);

        QuoteReportModel GetQuoteData(Guid quoteId);

        List<QuotePartReportModel> GetQuotePartsData(Guid quoteId);

        List<OpenOrdersReportModel> GetOpenCustomerOrderData(OpenOrdersReportModel model);

        List<OpenOrdersReportModel> GetUnattachedCustomerOrderData(OpenOrdersReportModel model);

        List<OpenOrdersReportModel> GetOpenFoundryOrderData(OpenOrdersReportModel model);

        PackingListReportModel GetPackingListData(Guid packingListId);

        IEnumerable<PackingListPartReportModel> GetPackingListPartsData(Guid packingListId);

        List<DebitMemoReportModel> GetDebitMemoData(Guid debitMemoId);

        List<DebitMemoReportModel> GetOpenDebitMemosData();

        List<ShipmentAnalysisReoprtModel> GetShipmentAnalysisData(Guid bolId);
    }
}
