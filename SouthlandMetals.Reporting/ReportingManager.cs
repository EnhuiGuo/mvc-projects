using Microsoft.Reporting.WebForms;
using SouthlandMetals.Reporting.Domain.Models.ReportModels;
using SouthlandMetals.Reporting.Domain.ReportModels;
using SouthlandMetals.Reporting.Domain.Repositories;
using SouthlandMetals.Reporting.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SouthlandMetals.Reporting
{
    public class ReportingManager
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static MemoryStream CreateRfqReport(Guid rfqId)
        {
            var _reportRepository = new ReportRepository();

            var rfq = _reportRepository.GetRfqData(rfqId);
            var rfqParts = _reportRepository.GetRfqPartsData(rfqId);

            DataTable rfqData = DataTableConverter.ToDataTable(rfq);
            DataTable rfqPartsData = DataTableConverter.ToDataTable(rfqParts);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource rfqDataSource = new ReportDataSource("RFQDS", rfqData);
            ReportDataSource rfqPartDataSource = new ReportDataSource("RFQPartDS", rfqPartsData);

            dataSources.Add(rfqDataSource);
            dataSources.Add(rfqPartDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("RFQReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreatePriceSheetReport(Guid priceSheetId)
        {
            var _reportRepository = new ReportRepository();

            var priceSheet = _reportRepository.GetPriceSheetData(priceSheetId);
            var priceSheetAddOns = _reportRepository.GetPriceSheetAddOnData(priceSheetId);
            var priceSheetSurcharges = _reportRepository.GetPriceSheetSurchargeData(priceSheetId);
            var priceSheetDutys = _reportRepository.GetPriceSheetDutyData(priceSheetId);
            var priceSheetCosts = _reportRepository.GetPriceSheetCostData(priceSheetId);
            var priceSheetPrices = _reportRepository.GetPriceSheetPriceData(priceSheetId);

            decimal totalAnnaulCost = 0;
            foreach (var cost in priceSheetCosts)
            {
                totalAnnaulCost += cost.AnnualCost;
            }

            priceSheet.TotalAnnualCost = totalAnnaulCost;

            decimal totalAnnualPrice = 0;
            foreach (var price in priceSheetPrices)
            {
                totalAnnualPrice += price.AnnualPrice;
            }

            priceSheet.TotalAnnualCost = totalAnnaulCost;
            priceSheet.TotalAnnualPrice = totalAnnualPrice;
            var margin = (totalAnnualPrice - totalAnnaulCost) / totalAnnualPrice * 100;
            priceSheet.OverallMargin = margin.ToString("#.##") + '%';

            DataTable priceSheetData = DataTableConverter.ToDataTable(priceSheet);
            DataTable priceSheetAddOnData = DataTableConverter.ToDataTable(priceSheetAddOns);
            DataTable priceSheetSurchargeData = DataTableConverter.ToDataTable(priceSheetSurcharges);
            DataTable priceSheetDutyData = DataTableConverter.ToDataTable(priceSheetDutys);
            DataTable priceSheetCostData = DataTableConverter.ToDataTable(priceSheetCosts);
            DataTable priceSheetPriceData = DataTableConverter.ToDataTable(priceSheetPrices);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource priceSheetDataSource = new ReportDataSource("PriceSheet", priceSheetData);
            ReportDataSource priceSheetAddOnDataSource = new ReportDataSource("AddOn", priceSheetAddOnData);
            ReportDataSource priceSheetSurchargeDataSource = new ReportDataSource("Surcharge", priceSheetSurchargeData);
            ReportDataSource priceSheetDutyDataSource = new ReportDataSource("Duty", priceSheetDutyData);
            ReportDataSource priceSheetCostDataSource = new ReportDataSource("Cost", priceSheetCostData);
            ReportDataSource priceSheetPriceDataSource = new ReportDataSource("Price", priceSheetPriceData);

            dataSources.Add(priceSheetDataSource);
            dataSources.Add(priceSheetAddOnDataSource);
            dataSources.Add(priceSheetSurchargeDataSource);
            dataSources.Add(priceSheetDutyDataSource);
            dataSources.Add(priceSheetCostDataSource);
            dataSources.Add(priceSheetPriceDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("PriceSheetReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateQuoteReport(Guid quoteId)
        {
            var _reportRepository = new ReportRepository();

            var quote = _reportRepository.GetQuoteData(quoteId);
            var quoteParts = _reportRepository.GetQuotePartsData(quoteId);

            DataTable quoteData = DataTableConverter.ToDataTable(quote);
            DataTable quotePartsData = DataTableConverter.ToDataTable(quoteParts);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource quoteDataSource = new ReportDataSource("QuoteDS", quoteData);
            ReportDataSource quotePartDataSource = new ReportDataSource("QuotePartDS", quotePartsData);

            dataSources.Add(quoteDataSource);
            dataSources.Add(quotePartDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("QuoteReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateFoundryOrderReport(Guid foundryOrderId)
        {
            var _reportRepository = new ReportRepository();

            var foundryOrder = _reportRepository.GetFoundryOrderData(foundryOrderId);
            var foundryOrderParts = _reportRepository.GetFoundryOrderPartsData(foundryOrderId);

            DataTable foundryOrderData = DataTableConverter.ToDataTable(foundryOrder);
            DataTable foundryOrderPartsData = DataTableConverter.ToDataTable(foundryOrderParts);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource foundryOrderDataSource = new ReportDataSource("FoundryOrderDataSet", foundryOrderData);
            ReportDataSource foundryOrderPartDataSource = new ReportDataSource("FoundryOrderPartDataSet", foundryOrderPartsData);

            dataSources.Add(foundryOrderDataSource);
            dataSources.Add(foundryOrderPartDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("FoundryOrderReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateOpenCustomerOrderReport(OpenOrdersReportModel model)
        {
            var _reportRepository = new ReportRepository();

            var customerOrders = _reportRepository.GetOpenCustomerOrderData(model);

            DataTable customerOrderData = DataTableConverter.ToDataTable(customerOrders);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource customerOrderDataSource = new ReportDataSource("OpenOrdersDS", customerOrderData);

            dataSources.Add(customerOrderDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("CustomerOpenOrdersReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateUnattachedCustomerOrderReport(OpenOrdersReportModel model)
        {
            var _reportRepository = new ReportRepository();

            var customerOrders = _reportRepository.GetUnattachedCustomerOrderData(model);

            DataTable customerOrderData = DataTableConverter.ToDataTable(customerOrders);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource customerOrderDataSource = new ReportDataSource("OpenOrdersDS", customerOrderData);

            dataSources.Add(customerOrderDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("CustomerOpenOrdersReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateOpenFoundryOrderReport(OpenOrdersReportModel model)
        {
            var _reportRepository = new ReportRepository();

            var foundryOrders = _reportRepository.GetOpenFoundryOrderData(model);

            DataTable foundryOrderData = DataTableConverter.ToDataTable(foundryOrders);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource foundryOrderDataSource = new ReportDataSource("OpenOrdersDS", foundryOrderData);

            dataSources.Add(foundryOrderDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("FoundryOpenOrdersReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreatePackingListReport(Guid packingListId)
        {
            var _reportRepository = new ReportRepository();

            var packingList = _reportRepository.GetPackingListData(packingListId);
            var packingListParts = _reportRepository.GetPackingListPartsData(packingListId);

            DataTable packingListData = DataTableConverter.ToDataTable(packingList);
            DataTable packingListPartsData = DataTableConverter.ToDataTable(packingListParts);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource packingListDataSource = new ReportDataSource("PackingListDS", packingListData);
            ReportDataSource packingListPartDataSource = new ReportDataSource("PackingListPartDS", packingListPartsData);

            dataSources.Add(packingListDataSource);
            dataSources.Add(packingListPartDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("PackingListReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateDebitMemoReport(Guid debitMemoId)
        {
            var _reportRepository = new ReportRepository();

            var debitMemo = _reportRepository.GetDebitMemoData(debitMemoId);

            DataTable debitMemoData = DataTableConverter.ToDataTable(debitMemo);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource debitMemoDataSource = new ReportDataSource("DebitMemoDS", debitMemoData);

            dataSources.Add(debitMemoDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("DebitMemoReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateOpenDebitMemoReport(List<DebitMemoReportModel> memos)
        {
            DataTable debitMemoData = DataTableConverter.ToDataTable(memos);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource debitMemoDataSource = new ReportDataSource("DebitMemoDS", debitMemoData);

            dataSources.Add(debitMemoDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("OpenDebitMemoReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateCreditMemoReport(Guid creditMemoId)
        {
            var _reportRepository = new ReportRepository();

            var creditMemo = _reportRepository.GetCreditMemoData(creditMemoId);

            DataTable creditMemoData = DataTableConverter.ToDataTable(creditMemo);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource creditMemoDataSource = new ReportDataSource("CreditMemoDS", creditMemoData);

            dataSources.Add(creditMemoDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("CreditMemoReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateShipmentAnalysisReport(Guid bolId)
        {
            var _reportRepository = new ReportRepository();

            var shipmentAnalysis = _reportRepository.GetShipmentAnalysisData(bolId);

            DataTable shipmentAnalysisData = DataTableConverter.ToDataTable(shipmentAnalysis);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource shipmentAnalysisDataSource = new ReportDataSource("ShipmentAnalysisDS", shipmentAnalysisData);

            dataSources.Add(shipmentAnalysisDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("ShipmentAnalysisReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateInventoryReport(InventoryReportModel model)
        {
            var _reportRepository = new ReportRepository();

            DataTable inventoryPartsData = DataTableConverter.ToDataTable(model.InventoryParts);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource inventoryPartDataSource = new ReportDataSource("InventoryDS", inventoryPartsData);

            dataSources.Add(inventoryPartDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("InventoryReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }


        public static MemoryStream CreateShipCodeInvoiceRegisterReport()
        {
            var _reportRepository = new ReportRepository();

            var invoices = _reportRepository.GetShipCodeInvoiceRegisterData();

            DataTable invoicesData = DataTableConverter.ToDataTable(invoices);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource invoicesDataSource = new ReportDataSource("InvoiceDS", invoicesData);

            dataSources.Add(invoicesDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("ShipCodeInvoiceRegisterReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateAccountExpenseSummaryReportForInternational(DateTime startDate, DateTime endDate)
        {
            var _reportRepository = new ReportRepository();

            var accountExpenses = _reportRepository.GetAccountExpenseSummaryDataForInternational(startDate, endDate);

            DataTable accountExpensesData = DataTableConverter.ToDataTable(accountExpenses);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource accountExpensesDataSource = new ReportDataSource("AccountSummaryDS", accountExpensesData);

            dataSources.Add(accountExpensesDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("AccountExpenseSummaryReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }


        public static MemoryStream CreateAccountExpenseSummaryReportForDomestic(DateTime startDate, DateTime endDate)
        {
            var _reportRepository = new ReportRepository();

            var accountExpenses = _reportRepository.GetAccountExpenseSummaryDataForDomestic(startDate, endDate);

            DataTable accountExpensesData = DataTableConverter.ToDataTable(accountExpenses);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource accountExpensesDataSource = new ReportDataSource("AccountSummaryDS", accountExpensesData);

            dataSources.Add(accountExpensesDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("AccountExpenseSummaryReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateDomesticSalesReport(DateTime startDate, DateTime endDate)
        {
            var _reportRepository = new ReportRepository();

            var domesticSales = _reportRepository.GetDomesticSalesData(startDate, endDate);

            DataTable domesticSalesData = DataTableConverter.ToDataTable(domesticSales);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource domesticSalesDataSource = new ReportDataSource("SalesDS", domesticSalesData);

            dataSources.Add(domesticSalesDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("DomesticSalesReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateInternationalSalesReport(DateTime startDate, DateTime endDate, string country)
        {
            var _reportRepository = new ReportRepository();

            var internationalSales = _reportRepository.GetInternationalSalesData(startDate, endDate, country);

            DataTable internationalSalesData = DataTableConverter.ToDataTable(internationalSales);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource internationalSalesDataSource = new ReportDataSource("SalesDS", internationalSalesData);

            dataSources.Add(internationalSalesDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("InternationalSalesReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateSalespersonCommissionReport(DateTime startDate, DateTime endDate, string country)
        {
            var _reportRepository = new ReportRepository();

            var commissions = _reportRepository.GetSalespersonCommissionData(startDate, endDate, country);

            DataTable commissionsData = DataTableConverter.ToDataTable(commissions);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource commissionsDataSource = new ReportDataSource("CommissionDS", commissionsData);

            dataSources.Add(commissionsDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("SalespersonCommissionReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateInvoiceRegisterReportForInternational(DateTime startDate, DateTime endDate, string shipCode)
        {
            var _reportRepository = new ReportRepository();

            var invoiceRegister = _reportRepository.GetInvoiceRegisterDataForInternational(startDate, endDate, shipCode);

            DataTable invoiceRegisterData = DataTableConverter.ToDataTable(invoiceRegister);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource invoiceRegisterDataSource = new ReportDataSource("InvoiceDS", invoiceRegisterData);

            dataSources.Add(invoiceRegisterDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("InvoiceRegisterReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateInvoiceRegisterReportForDomestic(DateTime startDate, DateTime endDate, string shipCode)
        {
            var _reportRepository = new ReportRepository();

            var invoiceRegister = _reportRepository.GetInvoiceRegisterDataForDomestic(startDate, endDate, shipCode);

            DataTable invoiceRegisterData = DataTableConverter.ToDataTable(invoiceRegister);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource invoiceRegisterDataSource = new ReportDataSource("InvoiceDS", invoiceRegisterData);

            dataSources.Add(invoiceRegisterDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("InvoiceRegisterReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }

        public static MemoryStream CreateFoundryInvoicesReport(DateTime startDate, DateTime endDate, bool unscheduled)
        {
            var _reportRepository = new ReportRepository();

            var foundryInvoices = _reportRepository.GetFoundryInvoicesData(startDate, endDate, unscheduled);

            DataTable foundryInvoicesData = DataTableConverter.ToDataTable(foundryInvoices);

            var dataSources = new List<ReportDataSource>();
            ReportDataSource foundryInvoicesDataSource = new ReportDataSource("InvoiceDS", foundryInvoicesData);

            dataSources.Add(foundryInvoicesDataSource);

            byte[] reportData = ReportViewerManager.ShowPdfReport("FoundryInvoicesReport", dataSources);
            var ms = new MemoryStream(reportData);

            return ms;
        }
    }
}
