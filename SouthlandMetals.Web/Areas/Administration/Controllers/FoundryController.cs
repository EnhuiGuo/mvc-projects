using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Administration.Controllers
{
    public class FoundryController : ApplicationBaseController
    {
        private IFoundryDynamicsRepository _foundryDynamicsRepository;
        private IPartDynamicsRepository _partDynamicsRepository;
        private IReceiptDynamicsRepository _receiptDynamicsRepository;
        private IPayablesDynamicsRepository _payablesDynamicsRepository;
        private ICountryRepository _countryRepository;

        public FoundryController()
        {
            _foundryDynamicsRepository = new FoundryDynamicsRepository();
            _partDynamicsRepository = new PartDynamicsRepository();
            _receiptDynamicsRepository = new ReceiptDynamicsRepository();
            _payablesDynamicsRepository = new PayablesDynamicsRepository();
            _countryRepository = new CountryRepository();
        }

        public FoundryController(IFoundryDynamicsRepository foundryDynamicsRepository,
                                 IPartDynamicsRepository partDynamicsRepository,
                                 IReceiptDynamicsRepository receiptDynamicsRepository,
                                 IPayablesDynamicsRepository payablesDynamicsRepository,
                                 ICountryRepository countryRepository)
        {
            _foundryDynamicsRepository = foundryDynamicsRepository;
            _partDynamicsRepository = partDynamicsRepository;
            _receiptDynamicsRepository = receiptDynamicsRepository;
            _payablesDynamicsRepository = payablesDynamicsRepository;
            _countryRepository = countryRepository;
        }

        /// <summary>
        /// GET: Administration/Foudnry
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = new FoundryViewModel();

            var result = new OperationResult();

            //var newPartMaster = new IV00101_Part_Master()
            //{
            //    ITEMNMBR = "COMPSYS",
            //    ITEMDESC = "compsys test",
            //    ITMCLSCD = "STANDARD",
            //    PRICMTHD = 2
            //};

            //var newPartPriceOption = new IV00107_Part_Price_Option()
            //{
            //    ITEMNMBR = "COMPSYS",
            //    PRCLEVEL = "STANDARD",
            //    UOFM = "part",
            //    CURNCYID = string.Empty
            //};

            //var newPartPrice = new IV00108_Part_Price()
            //{
            //    ITEMNMBR = "COMPSYS",
            //    PRCLEVEL = "STANDARD",
            //    UOFM = "part"
            //};

            //result = _partDynamicsRepository.SavePartMaster(newPartMaster);

            //result = _partDynamicsRepository.SavePartPrice(newPartPrice);

            //result = _partDynamicsRepository.SavePartPriceOption(newPartPriceOption);

            //var part = _partDynamicsRepository.GetPartCurrency(newPartMaster.ITEMNMBR);

            //if (part != null)
            //{
            //    part.LISTPRCE = 25.00000m;
            //    result = _partDynamicsRepository.UpdatePartCurrency(part);
            //}

            //var receiptLines = new List<POP10310>();

            //var newReceiptHeader = new POP10300_Receipt_Work()
            //{
            //    POPRCTNM = "PHANTOM201",
            //    POPTYPE = 1,
            //    receiptdate = DateTime.Now.ToShortDateString(),
            //    BACHNUMB = "COMPSYS",
            //    VENDORID = "SUO"
            //};

            //var newReceiptLine1 = new POP10310()
            //{
            //    POPTYPE = 1,
            //    POPRCTNM = "PHANTOM201",
            //    SERLTNUM = "B99999",
            //    ITEMNMBR = "083198-101A",
            //    VENDORID = "SUO",
            //    PONUMBER = "A014007",
            //    VNDITNUM = "083198-101A",
            //    QTYSHPPD = 5
            //};

            //var newReceiptLine2 = new POP10310()
            //{
            //    POPTYPE = 1,
            //    POPRCTNM = "PHANTOM201",
            //    SERLTNUM = "B99999",
            //    ITEMNMBR = "083198-101A",
            //    VENDORID = "SUO",
            //    PONUMBER = "A014007",
            //    VNDITNUM = "083198-101A",
            //    QTYSHPPD = 7
            //};

            //receiptLines.Add(newReceiptLine1);
            //receiptLines.Add(newReceiptLine2);

            //result = _receiptDynamicsRepository.SaveReceipt(newReceiptHeader, receiptLines);

            //var newPayable = new PM10000_Payables_Work()
            //{
            //    BACHNUMB = "B99999",
            //    VCHNUMWK = "99999999999999",
            //    VENDORID = "ITAF",
            //    DOCNUMBR = "DOC999999",
            //    DOCTYPE = 1,
            //    DOCAMNT = 1000.00m,
            //    DOCDATE = DateTime.Now,
            //    MSCCHAMT = 0.00m,
            //    PRCHAMNT = 1000.00m,
            //    CHRGAMNT = 1000.00m,
            //    TAXAMNT = 0.00m,
            //    FRTAMNT = 0.00m,
            //    TRDISAMT = 0.00m,
            //    CASHAMNT = 0.00m,
            //    CHEKAMNT = 0.00m,
            //    CRCRDAMT = 0.00m,
            //    DISTKNAM = 0.00m
            //};

            //result = _payablesDynamicsRepository.SavePayable(newPayable);


            var foundries = new List<FoundryViewModel>();

            var tempFoundries = _foundryDynamicsRepository.GetFoundries().Where(x => x.VENDSTTS == 1).ToList();

            if (tempFoundries != null && tempFoundries.Count > 0)
            {
                foreach (var tempFoundry in tempFoundries)
                {
                    FoundryViewModel convertedModel = new FoundryConverter().ConvertToListView(tempFoundry);

                    foundries.Add(convertedModel);
                }
            }

            model.Foundries = foundries.OrderBy(x => x.ShortName).ToList();

            return View(model);
        }

        /// <summary>
        /// GET: Administration/Foudnry/Detail
        /// </summary>
        /// <param name="foundryId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Detail(string foundryId)
        {
            var foundry = _foundryDynamicsRepository.GetFoundry(foundryId);

            FoundryViewModel model = new FoundryConverter().ConvertToView(foundry);

            return View(model);
        }

        /// <summary>
        /// get active foundries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveFoundries()
        {
            var model = new FoundryViewModel();

            var foundries = new List<FoundryViewModel>();

            var tempFoundries = _foundryDynamicsRepository.GetFoundries().Where(x => x.VENDSTTS == 1).ToList();

            foreach (var tempFoundry in tempFoundries)
            {
                FoundryViewModel convertedModel = new FoundryConverter().ConvertToListView(tempFoundry);

                foundries.Add(convertedModel);
            }

            model.Foundries = foundries.OrderBy(x => x.ShortName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive foundries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveFoundries()
        {
            var model = new FoundryViewModel();

            var foundries = new List<FoundryViewModel>();

            var tempFoundries = _foundryDynamicsRepository.GetFoundries().Where(x => x.VENDSTTS != 1).ToList();

            foreach (var tempFoundry in tempFoundries)
            {
                FoundryViewModel convertedModel = new FoundryConverter().ConvertToListView(tempFoundry);

                foundries.Add(convertedModel);
            }

            model.Foundries = foundries.OrderBy(x => x.ShortName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get country by founry
        /// </summary>
        /// <param name="foundryId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCountryByFoundry(string foundryId)
        {
            var model = new FoundryViewModel();

            var foundry = _foundryDynamicsRepository.GetFoundry(foundryId);

            var country = _countryRepository.GetCountry(foundry.COUNTRY);

            model.CountryName = (country != null && (!string.IsNullOrEmpty(country.Name))) ? country.Name : "N/A";

            model.CountryId = (country != null) ? country.CountryId : Guid.Empty;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }

                if (_partDynamicsRepository != null)
                {
                    _partDynamicsRepository.Dispose();
                    _partDynamicsRepository = null;
                }

                if (_receiptDynamicsRepository != null)
                {
                    _receiptDynamicsRepository.Dispose();
                    _receiptDynamicsRepository = null;
                }

                if (_payablesDynamicsRepository != null)
                {
                    _payablesDynamicsRepository.Dispose();
                    _payablesDynamicsRepository = null;
                }

                if (_countryRepository != null)
                {
                    _countryRepository.Dispose();
                    _countryRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}