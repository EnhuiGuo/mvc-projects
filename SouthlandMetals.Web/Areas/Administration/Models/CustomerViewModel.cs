using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Administration.Models
{
    public class CustomerViewModel
    {
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Address")]
        public string CustomerAddressId { get; set; }
        [Display(Name = "Address")]
        public Guid? BillToAddressId { get; set; }
        [Display(Name = "Address")]
        public Guid? ShipToAddressId { get; set; }
        [Display(Name = "Address Type")]
        public string AddressType { get; set; }
        [Display(Name = "Number")]
        public string CustomerNumber { get; set; }
        [Display(Name = "Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }
        [Display(Name = "Phone1")]
        public string Phone1 { get; set; }
        [Display(Name = "Phone2")]
        public string Phone2 { get; set; }
        [Display(Name = "Phone3")]
        public string Phone3 { get; set; }
        [Display(Name = "Fax")]
        public string FaxNumber { get; set; }
        [Display(Name = "Salesperson")]
        public string SalespersonId { get; set; }
        [Display(Name = "Salesperson")]
        public string SalespersonName { get; set; }
        [Display(Name = "Sales Territory")]
        public string SalesTerritoryDescription { get; set; }
        [Display(Name = "Payment Term")]
        public string PaymentTermId { get; set; }
        [Display(Name = "Payment Term")]
        public string PaymentTermDescription { get; set; }
        [Display(Name = "Shipment Term")]
        public Guid? ShipmentTermId { get; set; }
        [Display(Name = "Contact")]
        public string ContactName { get; set; }
        [Display(Name = "Phone")]
        public string ContactPhone { get; set; }
        [Display(Name = "Address Code")]
        public string AddressCode { get; set; }
        [Display(Name = "Address1")]
        public string Address1 { get; set; }
        [Display(Name = "Address2")]
        public string Address2 { get; set; }
        [Display(Name = "Address3")]
        public string Address3 { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "State")]
        public Guid? StateId { get; set; }
        [Display(Name = "State")]
        public string StateName { get; set; }
        [Display(Name = "Country")]
        public Guid? CountryId { get; set; }
        [Display(Name = "Country")]
        public string CountryName { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Display(Name = "Site")]
        public string SiteId { get; set; }
        [Display(Name = "Site")]
        public string SiteDescription { get; set; }
        [Display(Name = "Order Terms")]
        public string OrderTermsDescription { get; set; }
        [Display(Name = "Ship Method")]
        public string ShippingMethod { get; set; }
        [Display(Name = "Order Term")]
        public Guid? OrderTermId { get; set; }
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public List<CustomerViewModel> Customers { get; set; }
        public List<CustomerViewModel> CustomerAddresses { get; set; }
        public List<SelectListItem> SelectableAccountCodes { get; set; }
        public List<SelectListItem> SelectableCustomerAddresses { get; set; }
        public List<SelectListItem> SelectableSalespersons { get; set; }
        public List<SelectListItem> SelectableSites { get; set; }
        public List<SelectListItem> SelectablePaymentTerms { get; set; }
        public List<SelectListItem> SelectableShipmentTerms { get; set; }
        public List<SelectListItem> SelectableStates { get; set; }
        public List<SelectListItem> SelectableCountries { get; set; }
    }
}