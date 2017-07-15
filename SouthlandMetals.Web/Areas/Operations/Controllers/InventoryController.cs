using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Controllers
{
    public class InventoryController : ApplicationBaseController
    {

        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private ISalespersonDynamicsRepository _salespersonDynamicsRepository;
        private IFoundryDynamicsRepository _foundryDynamicsRepository;
        private IPartRepository _partRepository;

        public InventoryController()
        {
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _salespersonDynamicsRepository = new SalespersonDynamicsRepository();
            _foundryDynamicsRepository = new FoundryDynamicsRepository();
            _partRepository = new PartRepository();
        }

        public InventoryController(ICustomerDynamicsRepository customerDynamicsRepository,
                                   ISalespersonDynamicsRepository salespersonDynamicsRepository,
                                   IFoundryDynamicsRepository foundryDynamicsRepository,
                                   IPartRepository partRepository)
        {
            _customerDynamicsRepository = customerDynamicsRepository;
            _salespersonDynamicsRepository = salespersonDynamicsRepository;
            _foundryDynamicsRepository = foundryDynamicsRepository;
            _partRepository = partRepository;
        }

        private List<PartInventoryViewModel> CalculateReorderStatus(List<PartInventoryViewModel> parts)
        {
            var calculatedParts = new List<PartInventoryViewModel>();

            foreach (var part in parts)
            {
                var reorderPoint = (part.AverageDailySales * (part.OrderCycle + part.LeadTime + part.TransitTime)) + part.SafetyQuantity;

                var reorderQuantity = decimal.MinValue;

                if (part.QuantityOnHand > part.MinimumQuantity)
                {
                    reorderQuantity = reorderPoint - (part.QuantityOnHand + part.OnOrderQuantity);
                }
                else
                {
                    reorderQuantity = reorderPoint - ((part.MinimumQuantity - part.QuantityOnHand) + part.OnOrderQuantity);
                }

                var newCalculatedPart = part;

                calculatedParts.Add(newCalculatedPart);
            }

            return calculatedParts;
        }

        // GET: Operations/Inventory
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: Operations/Inventory/Planning
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Planning()
        {
            var model = new PlanningViewModel();

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            return View(model);
        }

        /// <summary>
        /// get part inventories with a filter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult FilterPartPlanning(InventoryViewModel model)
        {
            var parts = new List<PartInventoryViewModel>();

            var tempParts = _partRepository.GetParts().Where(x => x.IsActive).ToList();
            
            if(tempParts != null && tempParts.Count > 0)
            {
                foreach (var tempPart in tempParts)
                {
                    PartInventoryViewModel convertedModel = new PartInventoryConverter().ConvertToListView(tempPart);
                    parts.Add(convertedModel);
                }
            }

            if (model.CustomerId != null && model.CustomerId != string.Empty)
            {
                parts = parts.Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == model.CustomerId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (model.FoundryId != null && model.FoundryId != string.Empty)
            {
                parts = parts.Where(x => x.FoundryId.Replace(" ", string.Empty).ToLower() == model.FoundryId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(model.PartNumber))
            {
                parts = parts.Where(x => x.PartNumber.ToLower() == model.PartNumber.ToLower()).ToList();
            }

            parts = CalculateReorderStatus(parts);

            if (model.ZeroOnHand)
            {
                parts = parts.Where(x => x.QuantityOnHand < 1).ToList();
            }

            if (model.BelowMinimum)
            {
                parts = parts.Where(x => x.QuantityOnHand < x.MinimumQuantity).ToList();
            }

            if (model.BelowReorderPoint)
            {
                parts = parts.Where(x => x.QuantityOnHand < x.ReorderPoint).ToList();
            }

            parts = parts.OrderBy(x => x.PartNumber).ToList();

            var jsonResult = Json(parts, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// GET: Operations/Inventory/Status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Status()
        {
            var model = new InventoryViewModel();

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            return View(model);
        }

        /// <summary>
        /// get status with a fileter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SearchStatus(InventoryViewModel model)
        {
            var parts = new List<PartInventoryViewModel>();

            var tempParts = _partRepository.GetParts().Where(x => x.IsActive).ToList();

            if (tempParts != null && tempParts.Count > 0)
            {
                foreach (var tempPart in tempParts)
                {
                    PartInventoryViewModel convertedModel = new PartInventoryConverter().ConvertToListView(tempPart);
                    parts.Add(convertedModel);
                }
            }

            if (model.CustomerId != "--Select Customer--")
            {
                parts = parts.Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == model.CustomerId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (model.FoundryId != "--Select Foundry--")
            {
                parts = parts.Where(x => x.FoundryId.Replace(" ", string.Empty).ToLower() == model.FoundryId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(model.PartNumber))
            {
                parts = parts.Where(x => x.PartNumber.Replace(" ", string.Empty).ToLower() == model.PartNumber.Replace(" ", string.Empty).ToLower()).ToList();
            }

            parts = parts.OrderBy(x => x.PartNumber).ToList();

            var jsonResult = Json(parts, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// GET: Operations/Inventory/Sales
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Sales()
        {
            var model = new InventoryViewModel();

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }

                if (_salespersonDynamicsRepository != null)
                {
                    _salespersonDynamicsRepository.Dispose();
                    _salespersonDynamicsRepository = null;
                }

                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }

                if (_partRepository != null)
                {
                    _partRepository.Dispose();
                    _partRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}