using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Models;
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
    public class CustomerController : ApplicationBaseController
    {
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private ICustomerAddressDynamicsRepository _customerAddressDynamicsRepository;
        private IPaymentTermRepository _paymentTermRepository;
        private IShipmentTermRepository _shipmentTermRepository;
        private ICountryRepository _countryRepository;
        private IStateRepository _stateRepository;
        private IOrderTermRepository _orderTermRepository;
        private ISalespersonDynamicsRepository _salespersonDynamicsRepository;
        private ISiteDynamicsRepository _siteDynamicsRepository;
        private IAccountCodeRepository _accountCodeRepository;
        private IProjectRepository _projectRepository;
        private IPartRepository _partRepository;

        public CustomerController()
        {
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            _paymentTermRepository = new PaymentTermRepository();
            _shipmentTermRepository = new ShipmentTermRepository();
            _countryRepository = new CountryRepository();
            _stateRepository = new StateRepository();
            _orderTermRepository = new OrderTermRepository();
            _salespersonDynamicsRepository = new SalespersonDynamicsRepository();
            _siteDynamicsRepository = new SiteDynamicsRepository();
            _accountCodeRepository = new AccountCodeRepository();
            _projectRepository = new ProjectRepository();
            _partRepository = new PartRepository();
        }

        public CustomerController(ICustomerDynamicsRepository customerDynamicsRepository,
                                  ICustomerAddressDynamicsRepository customerAddressDynamicsRepository,
                                  IPaymentTermRepository paymentTermRepository,
                                  IShipmentTermRepository shipmentTermRepository,
                                  ICountryRepository countryRepository,
                                  IStateRepository stateRepository,
                                  IOrderTermRepository orderTermRepository,
                                  ISalespersonDynamicsRepository salespersonDynamicsRepository,
                                  ISiteDynamicsRepository siteDynamicsRepository,
                                  IAccountCodeRepository accountCodeRepository,
                                  IProjectRepository projectRepository,
                                  IPartRepository partRepository)
        {
            _customerDynamicsRepository = customerDynamicsRepository;
            _customerAddressDynamicsRepository = customerAddressDynamicsRepository;
            _paymentTermRepository = paymentTermRepository;
            _shipmentTermRepository = shipmentTermRepository;
            _countryRepository = countryRepository;
            _stateRepository = stateRepository;
            _orderTermRepository = orderTermRepository;
            _salespersonDynamicsRepository = salespersonDynamicsRepository;
            _siteDynamicsRepository = siteDynamicsRepository;
            _accountCodeRepository = accountCodeRepository;
            _projectRepository = projectRepository;
            _partRepository = partRepository;
        }

        /// <summary>
        /// GET: Administration/Customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = new CustomerViewModel();

            var customers = new List<CustomerViewModel>();

            var tempCustomers = _customerDynamicsRepository.GetCustomers().Where(x => x.INACTIVE != 1).ToList();

            if (tempCustomers != null && tempCustomers.Count > 0)
            {
                foreach (var tempCustomer in tempCustomers)
                {
                    CustomerViewModel convertedModel = new CustomerConverter().ConvertToListView(tempCustomer);

                    customers.Add(convertedModel);
                }
            }

            model.Customers = customers.OrderBy(x => x.ShortName).ToList();

            model.SelectableShipmentTerms = _shipmentTermRepository.GetSelectableShipmentTerms();

            var defaultShipmentTerm = new SelectListItem()
            {
                Text = "--Select Shipment Term--",
                Value = null
            };

            model.SelectableShipmentTerms.Insert(0, defaultShipmentTerm);

            model.SelectablePaymentTerms = _paymentTermRepository.GetSelectablePaymentTerms();

            var defaultPaymentTerm = new SelectListItem()
            {
                Text = "--Select Payment Term--",
                Value = null
            };

            model.SelectablePaymentTerms.Insert(0, defaultPaymentTerm);

            model.SelectableStates = _stateRepository.GetSelectableStates();

            var defaultState = new SelectListItem()
            {
                Text = "--Select State--",
                Value = null
            };

            model.SelectableStates.Insert(0, defaultState);

            model.SelectableCountries = _countryRepository.GetSelectableCountries();

            var defaultCountry = new SelectListItem()
            {
                Text = "--Select Country--",
                Value = null
            };

            model.SelectableCountries.Insert(0, defaultCountry);

            model.SelectableSalespersons = _salespersonDynamicsRepository.GetSelectableSalespersons();

            var defaultSalesperson = new SelectListItem()
            {
                Text = "--Select Salesperson--",
                Value = null
            };

            model.SelectableSalespersons.Insert(0, defaultSalesperson);

            model.SelectableSites = _siteDynamicsRepository.GetSelectableSites();

            var defaultSite = new SelectListItem()
            {
                Text = "--Select Site--",
                Value = null
            };

            model.SelectableSites.Insert(0, defaultSite);

            return View(model);
        }

        /// <summary>
        /// GET: Administration/Customer/Detail
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Detail(string customerId)
        {
            var customer = _customerDynamicsRepository.GetCustomer(customerId);

            CustomerViewModel model = new CustomerConverter().ConvertToView(customer);

            model.CustomerAddresses = GetCustomerAddresses(customerId);

            return View(model);
        }

        /// <summary>
        /// GET: Administration/Customer/AddressDetail
        /// </summary>
        /// <param name="customerAddressId"></param>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult AddressDetail(string customerAddressId, string customerNumber)
        {
            var address = _customerAddressDynamicsRepository.GetCustomerAddress(customerAddressId, customerNumber);

            CustomerViewModel model = new CustomerAddressConverter().ConvertToView(address);

            return View(model);
        }

        /// <summary>
        /// add customer modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddCustomer()
        {
            var model = new CustomerViewModel();

            model.SelectableShipmentTerms = _shipmentTermRepository.GetSelectableShipmentTerms();

            model.SelectablePaymentTerms = _paymentTermRepository.GetSelectablePaymentTerms();

            model.SelectableStates = _stateRepository.GetSelectableStates();

            model.SelectableCountries = _countryRepository.GetSelectableCountries();

            return PartialView(model);
        }

        /// <summary>
        /// add customer 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddCustomer(CustomerViewModel model)
        {
            var result = new OperationResult();

            RM00101_Customer newCustomer = new CustomerConverter().ConvertToDomain(model);

            result = _customerDynamicsRepository.SaveCustomer(newCustomer);

            model.Success = result.Success;
            model.Message = result.Message;

            var customers = new List<CustomerViewModel>();

            model.Customers = customers.OrderBy(x => x.ShortName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// orde terms modal
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _OrderTerms(string customerId)
        {
            var model = new CustomerViewModel();

            var orderTerms = _orderTermRepository.GetOrderTermsByCustomer(customerId);

            if(orderTerms != null)
            {
                model = new OrderTermConverter().ConvertToCustomerView(orderTerms);
            }

            var customer = _customerDynamicsRepository.GetCustomer(customerId);

            model.CustomerName = customer.SHRTNAME;

            return PartialView(model);
        }

        /// <summary>
        /// edit order terms
        /// </summary>
        /// <param name="terms"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditOrderTerms(OrderTermViewModel terms)
        {
            var result = new OperationResult();

            OrderTerm orderTerms = new OrderTermConverter().ConvertToDomain(terms);

            result = _orderTermRepository.UpdateOrderTerm(orderTerms);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// view customer addresses modal
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _ViewCustomerAddresses(string customerId)
        {
            var model = new CustomerViewModel();

            var customer = _customerDynamicsRepository.GetCustomer(customerId);

            model.CustomerName = customer.SHRTNAME;

            return PartialView(model);
        }

        /// <summary>
        /// get customer addresses by customer 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public List<CustomerViewModel> GetCustomerAddresses(string customerId)
        {
            var addresses = new List<CustomerViewModel>();

            var customer = _customerDynamicsRepository.GetCustomer(customerId);

            var customerAddresses = _customerAddressDynamicsRepository.GetCustomerAddressesByCustomer(customer.CUSTNMBR);

            if (customerAddresses != null && customerAddresses.Count > 0)
            {
                foreach (var customerAddress in customerAddresses)
                {
                    CustomerViewModel convertedModel = new CustomerAddressConverter().ConvertToView(customerAddress);

                    addresses.Add(convertedModel);
                }
            }
            return addresses;
        }

        /// <summary>
        /// get active customers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveCustomers()
        {
            var model = new CustomerViewModel();

            var customers = new List<CustomerViewModel>();

            var tempCustomers = _customerDynamicsRepository.GetCustomers().Where(x => x.INACTIVE != 1).ToList();

            foreach (var tempCustomer in tempCustomers)
            {
                CustomerViewModel convertedModel = new CustomerConverter().ConvertToListView(tempCustomer);

                customers.Add(convertedModel);
            }

            model.Customers = customers.OrderBy(x => x.ShortName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive customers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveCustomers()
        {
            var model = new CustomerViewModel();

            var customers = new List<CustomerViewModel>();

            var tempCustomers = _customerDynamicsRepository.GetCustomers().Where(x => x.INACTIVE != 0).ToList();

            foreach (var tempCustomer in tempCustomers)
            {
                CustomerViewModel convertedModel = new CustomerConverter().ConvertToListView(tempCustomer);

                customers.Add(convertedModel);
            }

            model.Customers = customers.OrderBy(x => x.ShortName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get customer by id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCustomer(string customerId)
        {
            var model = new CustomerViewModel();

            var customer = _customerDynamicsRepository.GetCustomer(customerId);

            if (customer != null)
            {
                model = new CustomerConverter().ConvertToView(customer);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get customer by customer address
        /// </summary>
        /// <param name="customerAddressId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCustomerByAddress(string customerAddressId)
        {
            var model = new CustomerViewModel();

            var dynamicsAddress = _customerAddressDynamicsRepository.GetCustomerAddress(customerAddressId);

            model.ContactName = dynamicsAddress.CNTCPRSN;

            model.SelectableShipmentTerms = _shipmentTermRepository.GetSelectableShipmentTerms();

            var defaultShipmentTerm = new SelectListItem()
            {
                Text = "--Select Shipment Term--",
                Value = null
            };

            model.SelectableShipmentTerms.Insert(0, defaultShipmentTerm);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get customer addresses and account codes by customer 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetAddressesAndAccountCodesByCustomer(string customerId)
        {
            var model = new CustomerViewModel();

            model.SelectableCustomerAddresses = _customerAddressDynamicsRepository.GetSelectableCustomerAddresses(customerId);

            model.SelectableAccountCodes = _accountCodeRepository.GetSelectableAccountCodesByCustomer(customerId);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get customer address by customer for json result
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetAddressesByCustomer(string customerId)
        {
            var selectableCustomerAddresses = _customerAddressDynamicsRepository.GetSelectableCustomerAddresses(customerId);

            var defaultAddress = new SelectListItem()
            {
                Text = "--Select Address--",
                Value = null
            };

            selectableCustomerAddresses.Insert(0, defaultAddress);

            return Json(selectableCustomerAddresses, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get projects by customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetProjectsByCustomer(string customerId)
        {
            var selectableProjects = _projectRepository.GetProjects().Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == customerId.Replace(" ", string.Empty).ToLower() &&
                                                           !x.IsHold &&
                                                           !x.IsCanceled)
                                                            .Select(y => new SelectListItem()
                                                            {
                                                                Text = y.Name,
                                                                Value = y.ProjectId.ToString()
                                                            }).OrderBy(z => z.Text).ToList();

            return Json(selectableProjects, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get sites by customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetSitesByCustomer(string customerId)
        {
            var selectableSites = _siteDynamicsRepository.GetSelectableSites(customerId);

            return Json(selectableSites, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get selectable customer by country
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetSelectableCustomersByCountry(Guid countryId)
        {
            var selectableCustomers = _customerDynamicsRepository.GetSelectableCustomersByCountry(countryId);

            return Json(selectableCustomers, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get selectable parts by customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetSelectablePartsByCustomer(string customerId)
        {
            var selectableParts = _partRepository.GetSelectablePartsByCustomer(customerId);

            var defaultPart = new SelectListItem()
            {
                Text = "--Select Part--",
                Value = null
            };

            selectableParts.Insert(0, defaultPart);

            return Json(selectableParts, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get salesperson by customer 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetSalespersonByCustomer(string customerId)
        {
            var customer = _customerDynamicsRepository.GetCustomer(customerId);

            var salesperson = _salespersonDynamicsRepository.GetSalespersons().FirstOrDefault(x => x.SLPRSNID.Replace(" ", string.Empty).ToLower() == customer.SLPRSNID.Replace(" ", string.Empty).ToLower());

            SalespersonViewModel model = new SalespersonConverter().ConvertToView(salesperson);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get terms by customer 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetTermsByCustomer(string customerId)
        {
            OrderTermViewModel model = new OrderTermViewModel();

            var term = _orderTermRepository.GetOrderTermsByCustomer(customerId);

            if (term != null)
                model = new OrderTermConverter().ConvertToView(term);

            return Json(model, JsonRequestBehavior.AllowGet);
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

                if (_customerAddressDynamicsRepository != null)
                {
                    _customerAddressDynamicsRepository.Dispose();
                    _customerAddressDynamicsRepository = null;
                }

                if (_paymentTermRepository != null)
                {
                    _paymentTermRepository.Dispose();
                    _paymentTermRepository = null;
                }

                if (_shipmentTermRepository != null)
                {
                    _shipmentTermRepository.Dispose();
                    _shipmentTermRepository = null;
                }

                if (_countryRepository != null)
                {
                    _countryRepository.Dispose();
                    _countryRepository = null;
                }

                if (_stateRepository != null)
                {
                    _stateRepository.Dispose();
                    _stateRepository = null;
                }

                if (_salespersonDynamicsRepository != null)
                {
                    _salespersonDynamicsRepository.Dispose();
                    _salespersonDynamicsRepository = null;
                }

                if (_siteDynamicsRepository != null)
                {
                    _siteDynamicsRepository.Dispose();
                    _siteDynamicsRepository = null;
                }

                if (_accountCodeRepository != null)
                {
                    _accountCodeRepository.Dispose();
                    _accountCodeRepository = null;
                }

                if (_projectRepository != null)
                {
                    _projectRepository.Dispose();
                    _projectRepository = null;
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