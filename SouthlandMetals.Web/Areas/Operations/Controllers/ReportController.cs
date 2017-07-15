using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SouthlandMetals.Common.Mail;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Reporting;
using SouthlandMetals.Reporting.Domain.Models.ReportModels;
using SouthlandMetals.Reporting.Domain.ReportModels;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Helpers;
using SouthlandMetals.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Controllers
{
    public class ReportController : ApplicationBaseController
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ApplicationUserManager _userManager;

        public ReportController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ICountryRepository _countryRepository;
        private IFoundryDynamicsRepository _foundryDynamicsRepository;
        private IRfqRepository _rfqRepository;
        private IPriceSheetRepository _priceSheetRepository;
        private IQuoteRepository _quoteRepository;
        private IFoundryOrderRepository _foundryOrderRepository;
        private IPackingListRepository _packingListRepository;
        private IDebitMemoRepository _debitMemoRepository;
        private ICreditMemoRepository _creditMemoRepository;

        public ReportController()
        {
            _countryRepository = new CountryRepository();
            _foundryDynamicsRepository = new FoundryDynamicsRepository();
            _rfqRepository = new RfqRepository();
            _priceSheetRepository = new PriceSheetRepository();
            _quoteRepository = new QuoteRepository();
            _foundryOrderRepository = new FoundryOrderRepository();
            _packingListRepository = new PackingListRepository();
            _debitMemoRepository = new DebitMemoRepository();
            _creditMemoRepository = new CreditMemoRepository();
        }

        public ReportController(ICountryRepository countryRepository,
                                IFoundryDynamicsRepository foundryDynamicsRepository,
                                IRfqRepository rfqRepository,
                                IPriceSheetRepository priceSheetRepository,
                                IQuoteRepository quoteRepository,
                                IFoundryOrderRepository foundryOrderRepository,
                                IPackingListRepository packingListRepository,
                                IDebitMemoRepository debitMemoRepository,
                                ICreditMemoRepository creditMemoRepository)
        {
            _countryRepository = countryRepository;
            _foundryDynamicsRepository = foundryDynamicsRepository;
            _rfqRepository = rfqRepository;
            _priceSheetRepository = priceSheetRepository;
            _quoteRepository = quoteRepository;
            _foundryOrderRepository = foundryOrderRepository;
            _packingListRepository = packingListRepository;
            _debitMemoRepository = debitMemoRepository;
            _creditMemoRepository = creditMemoRepository;
        }

        /// <summary>
        /// GET: Operations/Report
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Index()
        {
            var model = new ReportViewModel();

            model.SelectableCountries = _countryRepository.GetSelectableCountries();

            var defaultCountry = new SelectListItem()
            {
                Text = "--Select Country--",
                Value = ""
            };

            model.SelectableCountries.Insert(0, defaultCountry);

            return View(model);
        }

        /// <summary>
        /// rfq report page
        /// </summary>
        /// <param name="rfqId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult RfqReport(Guid rfqId)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateRfqReport(rfqId);
            }
            catch(Exception ex)
            {
                operationResult.Message = "Error occured printing RFQ";
                logger.ErrorFormat("Error occured printing RFQ: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// price sheet report page
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult PriceSheetReport(Guid priceSheetId)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreatePriceSheetReport(priceSheetId);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Price Sheet";
                logger.ErrorFormat("Error occured printing Price Sheet: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// quote report page
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult QuoteReport(Guid quoteId)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateQuoteReport(quoteId);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Quote";
                logger.ErrorFormat("Error occured printing Quote: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// foundry order report page
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult FoundryOrderReport(Guid foundryOrderId)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateFoundryOrderReport(foundryOrderId);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Foundry Order";
                logger.ErrorFormat("Error occured printing Foundry Order: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// open customre order report page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult OpenCustomerOrderReport(OpenOrdersReportModel model)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateOpenCustomerOrderReport(model);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Open Customer Orders";
                logger.ErrorFormat("Error occured printing Open Customer Orders: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// unattached customer order report page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult UnattachedCustomerOrderReport(OpenOrdersReportModel model)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateUnattachedCustomerOrderReport(model);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Unattached Customer Orders";
                logger.ErrorFormat("Error occured printing Unattached Customer Orders: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// open foundry order report page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult OpenFoundryOrderReport(OpenOrdersReportModel model)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateOpenFoundryOrderReport(model);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Open Foundry Orders";
                logger.ErrorFormat("Error occured printing Open Foundry Orders: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// packling list report page
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult PackingListReport(Guid packingListId)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreatePackingListReport(packingListId);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing PackingList";
                logger.ErrorFormat("Error occured printing PackingList: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// debit memo report page
        /// </summary>
        /// <param name="debitMemoId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult DebitMemoReport(Guid debitMemoId)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateDebitMemoReport(debitMemoId);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Debit Memo";
                logger.ErrorFormat("Error occured printing Debit Memo: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// open debit memo report page
        /// </summary>
        /// <param name="memos"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult OpenDebitMemoReport(List<DebitMemoReportModel> memos)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateOpenDebitMemoReport(memos);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Open Debit Memos";
                logger.ErrorFormat("Error occured printing Open Debit Memos: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// credit memo report page
        /// </summary>
        /// <param name="creditMemoId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult CreditMemoReport(Guid creditMemoId)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateCreditMemoReport(creditMemoId);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Credit Memo";
                logger.ErrorFormat("Error occured printing Credit Memo: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// inventory report page
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult InventoryReport(List<InventoryPartReportModel> parts)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                var model = new InventoryReportModel();

                model.InventoryParts = parts;

                ms = ReportingManager.CreateInventoryReport(model);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Inventory Report";
                logger.ErrorFormat("Error occured printing Inventory Report: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// shipment analysis report page
        /// </summary>
        /// <param name="bolId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult ShipmentAnalysisReport(Guid bolId)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            var _billOfLadingRepository = new BillOfLadingRepository();

            try
            {
                ms = ReportingManager.CreateShipmentAnalysisReport(bolId);

                if(ms != null)
                {
                    var existingBillOfLading = _billOfLadingRepository.GetBillOfLading(bolId);

                    existingBillOfLading.HasBeenAnalyzed = true;

                    operationResult = _billOfLadingRepository.UpdateBillOfLading(existingBillOfLading);
                }
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured during Shipment Analysis";
                logger.ErrorFormat("Error occured during Shipment Analysis: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// ship code invoice register report page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult ShipCodeInvoiceRegisterReport()
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateShipCodeInvoiceRegisterReport();
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Invoice Register by Ship Code Report";
                logger.ErrorFormat("Error occured printing Invoice Register by Ship Code Report: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// account expense summary report page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult AccountExpenseSummaryReport(ReportViewModel model)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                if (model.Condition == "International")
                {
                    ms = ReportingManager.CreateAccountExpenseSummaryReportForInternational(model.StartDate, model.EndDate);
                }
                else if (model.Condition == "Domestic")
                {
                    ms = ReportingManager.CreateAccountExpenseSummaryReportForDomestic(model.StartDate, model.EndDate);
                }
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Account Expense Summary Report";
                logger.ErrorFormat("Error occured printing Account Expense Summary Report: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// domestic sales report page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult DomesticSalesReport(ReportViewModel model)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateDomesticSalesReport(model.StartDate, model.EndDate);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Domestic Sales Report";
                logger.ErrorFormat("Error occured printing Domestic Sales Report: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// internation sales report page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult InternationalSalesReport(ReportViewModel model)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateInternationalSalesReport(model.StartDate, model.EndDate, model.Country);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing International Sales Report";
                logger.ErrorFormat("Error occured printing International Sales Report: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// salesperson commission report page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult SalespersonCommissionReport(ReportViewModel model)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateSalespersonCommissionReport(model.StartDate, model.EndDate, model.Country);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Salesman Commission Report";
                logger.ErrorFormat("Error occured printing Salesman Commission Report: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// invoice register report page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult InvoiceRegisterReport(ReportViewModel model)
        {
            var operationResult = new OperationResult();

            MemoryStream ms = new MemoryStream();

            try
            {
                if (model.Condition == "International")
                {
                    ms = ReportingManager.CreateInvoiceRegisterReportForInternational(model.StartDate, model.EndDate, model.ShipCode);
                }
                else if (model.Condition == "Domestic")
                {
                    ms = ReportingManager.CreateInvoiceRegisterReportForDomestic(model.StartDate, model.EndDate, model.ShipCode);
                }
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Invoice Register Report";
                logger.ErrorFormat("Error occured printing Invoice Register Report: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// foundry invoice report page
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="unscheduled"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult FoundryInvoicesReport(DateTime startDate, DateTime endDate, bool unscheduled)
        {
            var operationResult = new OperationResult();
            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateFoundryInvoicesReport(startDate, endDate, unscheduled);
            }
            catch (Exception ex)
            {
                operationResult.Message = "Error occured printing Foundry Invoices Report";
                logger.ErrorFormat("Error occured printing Foundry Invoices Report: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        /// <summary>
        /// send rfq email
        /// </summary>
        /// <param name="rfqId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SendRfqEmail(Guid rfqId, EmailModel model)
        {
            var operationResult = new OperationResult();

            var _emailManager = new MailManager();

            var ms = ReportingManager.CreateRfqReport(rfqId);

            var currentRfq = _rfqRepository.GetRfq(rfqId);

            var fromUser = UserManager.FindById(User.Identity.GetUserId());

            model.MS = ms;
            model.FromEmail = fromUser.Email;
            model.Number = currentRfq.Number;

            operationResult = _emailManager.SendEmail(model);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// send price sheet email
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SendPriceSheetEmail(Guid priceSheetId, EmailModel model)
        {
            var operationResult = new OperationResult();

            var _emailManager = new MailManager();

            var ms = ReportingManager.CreatePriceSheetReport(priceSheetId);

            var currentPriceSheet = _priceSheetRepository.GetPriceSheet(priceSheetId).Number;

            var fromUser = UserManager.FindById(User.Identity.GetUserId());

            model.MS = ms;
            model.FromEmail = fromUser.Email;
            model.Number = currentPriceSheet;

            operationResult = _emailManager.SendEmail(model);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// send quote email
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SendQuoteEmail(Guid quoteId, EmailModel model)
        {
            var operationResult = new OperationResult();

            var _emailManager = new MailManager();

            var ms = ReportingManager.CreateQuoteReport(quoteId);

            var currentQuote = _quoteRepository.GetQuote(quoteId);

            var fromUser = UserManager.FindById(User.Identity.GetUserId());

            model.MS = ms;
            model.FromEmail = fromUser.Email;
            model.Number = currentQuote.Number;

            operationResult = _emailManager.SendEmail(model);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// send foundry order email
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SendFoundryOrderEmail(Guid foundryOrderId, EmailModel model)
        {
            var operationResult = new OperationResult();

            var _emailManager = new MailManager();

            var ms = ReportingManager.CreateFoundryOrderReport(foundryOrderId);

            var foundryOrder = _foundryOrderRepository.GetFoundryOrder(foundryOrderId);

            var fromUser = UserManager.FindById(User.Identity.GetUserId());

            model.MS = ms;
            model.FromEmail = fromUser.Email;
            model.Number = foundryOrder.Number;

            operationResult = _emailManager.SendEmail(model);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// send packing list email
        /// </summary>
        /// <param name="packingListId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SendPackingListEmail(Guid packingListId, EmailModel model)
        {
            var operationResult = new OperationResult();

            var _emailManager = new MailManager();

            var ms = ReportingManager.CreatePackingListReport(packingListId);

            var fromUser = UserManager.FindById(User.Identity.GetUserId());

            var packingList = _packingListRepository.GetPackingList(packingListId);

            model.MS = ms;
            model.FromEmail = fromUser.Email;
            model.Number = packingList.TrackingNumber;

            operationResult = _emailManager.SendEmail(model);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// send debitMemo email
        /// </summary>
        /// <param name="debitMemoId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SendDebitMemoEmail(Guid debitMemoId, EmailModel model)
        {
            var operationResult = new OperationResult();

            var _emailManager = new MailManager();

            var ms = ReportingManager.CreateDebitMemoReport(debitMemoId);

            var debitMemo = _debitMemoRepository.GetDebitMemo(debitMemoId);

            var fromUser = UserManager.FindById(User.Identity.GetUserId());

            model.MS = ms;
            model.FromEmail = fromUser.Email;
            model.Number = debitMemo.Number;

            operationResult = _emailManager.SendEmail(model);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// send creditMemo email
        /// </summary>
        /// <param name="creditMemoId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SendCreditMemoEmail(Guid creditMemoId, EmailModel model)
        {
            var operationResult = new OperationResult();

            var _emailManager = new MailManager();

            var ms = ReportingManager.CreateCreditMemoReport(creditMemoId);

            var creditMemo = _creditMemoRepository.GetCreditMemo(creditMemoId);

            var fromUser = UserManager.FindById(User.Identity.GetUserId());

            model.MS = ms;
            model.FromEmail = fromUser.Email;
            model.Number = creditMemo.Number;

            operationResult = _emailManager.SendEmail(model);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// account expense summary report modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AccountExpenseSummaryReport()
        {
            return PartialView();
        }

        /// <summary>
        /// domestic sales report modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _DomesticSalesReport()
        {
            return PartialView();
        }

        /// <summary>
        /// internation sales report modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _InternationalSalesReport()
        {
            var model = new ReportViewModel();

            model.SelectableCountries = _countryRepository.GetSelectableCountries();

            var defaultCountry = new SelectListItem()
            {
                Text = "--Select Country--",
                Value = ""
            };

            model.SelectableCountries.Insert(0, defaultCountry);

            return PartialView(model);
        }

        /// <summary>
        /// sales person commission report modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _SalespersonCommissionReport()
        {
            var model = new ReportViewModel();

            model.SelectableCountries = _countryRepository.GetSelectableCountries();

            var defaultCountry = new SelectListItem()
            {
                Text = "--Select Country--",
                Value = ""
            };

            model.SelectableCountries.Insert(0, defaultCountry);

            return PartialView(model);
        }

        /// <summary>
        /// invoice register report modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _InvoiceRegisterReport()
        {
            return PartialView();
        }

        /// <summary>
        /// foundry invoice report modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _FoundryInvoicesReport()
        {
            var model = new ReportViewModel();

            model.StartDateStr = DateTime.Now.ToShortDateString();
            model.EndDateStr = DateTime.Now.ToShortDateString();

            return PartialView(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_countryRepository != null)
                {
                    _countryRepository.Dispose();
                    _countryRepository = null;
                }

                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }

                if (_rfqRepository != null)
                {
                    _rfqRepository.Dispose();
                    _rfqRepository = null;
                }

                if (_priceSheetRepository != null)
                {
                    _priceSheetRepository.Dispose();
                    _priceSheetRepository = null;
                }

                if (_quoteRepository != null)
                {
                    _quoteRepository.Dispose();
                    _quoteRepository = null;
                }

                if (_foundryOrderRepository != null)
                {
                    _foundryOrderRepository.Dispose();
                    _foundryOrderRepository = null;
                }

                if (_packingListRepository != null)
                {
                    _packingListRepository.Dispose();
                    _packingListRepository = null;
                }

                if (_debitMemoRepository != null)
                {
                    _debitMemoRepository.Dispose();
                    _debitMemoRepository = null;
                }

                if (_creditMemoRepository != null)
                {
                    _creditMemoRepository.Dispose();
                    _creditMemoRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}