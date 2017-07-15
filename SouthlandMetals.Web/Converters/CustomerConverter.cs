using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Models;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;
using SouthlandMetals.Web.Helpers;

namespace SouthlandMetals.Web.Converters
{
    public class CustomerConverter
    {
        /// <summary>
        /// convert customer to list model
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public CustomerViewModel ConvertToListView(RM00101_Customer customer)
        {
            CustomerViewModel model = new CustomerViewModel();

            var _salespersonDynamicsRepository = new SalespersonDynamicsRepository();

            var dyanmicsSalesperson = _salespersonDynamicsRepository.GetSalesperson(customer.SLPRSNID);

            model.CustomerId = customer.CUSTNMBR;
            model.CustomerNumber = customer.CUSTNMBR;
            model.ShortName = (!string.IsNullOrEmpty(customer.SHRTNAME.Replace(" ", string.Empty))) ? customer.SHRTNAME : "N/A";
            model.ContactName = (!string.IsNullOrEmpty(customer.CNTCPRSN.Replace(" ", string.Empty))) ? customer.CNTCPRSN : "N/A";
            model.SalespersonName = (dyanmicsSalesperson != null) ? dyanmicsSalesperson.SLPRSNFN + " " + dyanmicsSalesperson.SPRSNSLN : "N/A";
            model.IsActive = customer.INACTIVE != 1 ? true : false;

            if (_salespersonDynamicsRepository != null)
            {
                _salespersonDynamicsRepository.Dispose();
                _salespersonDynamicsRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert customer to view model
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public CustomerViewModel ConvertToView(RM00101_Customer customer)
        {
            CustomerViewModel model = new CustomerViewModel();

            var _salespersonDynamicsRepository = new SalespersonDynamicsRepository();
            var _paymentTermRepository = new PaymentTermRepository();
            var _shipmentTermRepository = new ShipmentTermRepository();
            var _stateRepository = new StateRepository();
            var _countryRepository = new CountryRepository();

            var dyanmicsSalesperson = _salespersonDynamicsRepository.GetSalesperson(customer.SLPRSNID);
            var state = _stateRepository.GetState(customer.STATE);
            var country = _countryRepository.GetCountry(customer.COUNTRY);
            var paymentTerm = _paymentTermRepository.GetPaymentTerm(customer.PYMTRMID);

            model.CustomerId = customer.CUSTNMBR;
            model.CustomerNumber = customer.CUSTNMBR;
            model.CustomerName = (!string.IsNullOrEmpty(customer.CUSTNAME.Replace(" ", string.Empty))) ? customer.CUSTNAME : "N/A";
            model.ShortName = (!string.IsNullOrEmpty(customer.SHRTNAME.Replace(" ", string.Empty))) ? customer.SHRTNAME : "N/A";
            model.SalespersonName = (dyanmicsSalesperson != null) ? dyanmicsSalesperson.SLPRSNFN + " " + dyanmicsSalesperson.SPRSNSLN : "N/A";
            model.SalesTerritoryDescription = (!string.IsNullOrEmpty(customer.SALSTERR.Replace(" ", string.Empty))) ? customer.SALSTERR : "N/A";
            model.ContactName = (!string.IsNullOrEmpty(customer.CNTCPRSN)) ? customer.CNTCPRSN : "N/A";
            model.ContactPhone = FormattingManager.FormatPhone(customer.PHONE1);
            model.FaxNumber = FormattingManager.FormatPhone(customer.FAX);
            model.Address1 = (!string.IsNullOrEmpty(customer.ADDRESS1)) ? customer.ADDRESS1 : "N/A";
            model.City = (!string.IsNullOrEmpty(customer.CITY)) ? customer.CITY : "N/A";
            model.StateName = (state != null) ? state.Name : "N/A";
            model.CountryName = (country != null) ? country.Name : "N/A";
            model.PostalCode = (!string.IsNullOrEmpty(customer.ZIP)) ? customer.ZIP : "N/A";
            model.PaymentTermDescription = (paymentTerm != null && !string.IsNullOrEmpty(paymentTerm.Description)) ? paymentTerm.Description : "N/A";
            model.IsActive = customer.INACTIVE != 1 ? true : false;

            if (_salespersonDynamicsRepository != null)
            {
                _salespersonDynamicsRepository.Dispose();
                _salespersonDynamicsRepository = null;
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

            if (_stateRepository != null)
            {
                _stateRepository.Dispose();
                _stateRepository = null;
            }

            if (_countryRepository != null)
            {
                _countryRepository.Dispose();
                _countryRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert customer view model to dynamics
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RM00101_Customer ConvertToDomain(CustomerViewModel model)
        {
            RM00101_Customer customer = new RM00101_Customer();

        //    var _customerDynamicsRepository = new CustomerDynamicsRepository();
        //    var _countryRepository = new CountryRepository();
        //    var _stateRepository = new StateRepository();
        //    var _salespersonRepository = new SalespersonRepository();
        //    var _paymentTermRepository = new PaymentTermRepository();

        //    var existingCustomer = _customerDynamicsRepository.GetCustomer(model.CustomerNumber);
        //    var country = _countryRepository.GetCountry(model.CountryId);
        //    var state = _stateRepository.GetState(model.StateId);
        //    var salesperson = _salespersonRepository.GetSalesperson(model.SalespersonId);
        //    var paymentTerm = _paymentTermRepository.GetPaymentTerm(model.PaymentTermId);

        //    customer.CUSTNMBR = (existingCustomer != null) ? existingCustomer.CUSTNMBR : model.CustomerNumber;
        //    customer.CUSTNAME = (existingCustomer != null) ? existingCustomer.CUSTNAME : model.CustomerName;
        //    customer.CUSTCLAS = (existingCustomer != null) ? existingCustomer.CUSTCLAS : " ";
        //    customer.CPRCSTNM = (existingCustomer != null) ? existingCustomer.CPRCSTNM : " ";
        //    customer.CNTCPRSN = (existingCustomer != null) ? existingCustomer.CNTCPRSN : model.ContactName;
        //    customer.STMTNAME = (existingCustomer != null) ? existingCustomer.STMTNAME : " ";
        //    customer.SHRTNAME = (existingCustomer != null) ? existingCustomer.SHRTNAME : model.ShortName;
        //    customer.ADRSCODE = (existingCustomer != null) ? existingCustomer.ADRSCODE : model.AddressCode;
        //    customer.UPSZONE = (existingCustomer != null) ? existingCustomer.UPSZONE : " ";
        //    customer.SHIPMTHD = (existingCustomer != null) ? existingCustomer.SHIPMTHD : " ";
        //    customer.TAXSCHID = (existingCustomer != null) ? existingCustomer.TAXSCHID : " ";
        //    customer.ADDRESS1 = (existingCustomer != null) ? existingCustomer.ADDRESS1 : model.Address1;
        //    customer.ADDRESS2 = (existingCustomer != null) ? existingCustomer.ADDRESS2 : model.Address2;
        //    customer.ADDRESS3 = (existingCustomer != null) ? existingCustomer.ADDRESS3 : model.Address3;
        //    customer.COUNTRY = (country != null) ? country.Name : " ";
        //    customer.CITY = (existingCustomer != null) ? existingCustomer.CITY : model.City;
        //    customer.STATE = (state != null) ? state.Name : " ";
        //    customer.ZIP = (existingCustomer != null) ? existingCustomer.ZIP : model.PostalCode;
        //    customer.PHONE1 = (existingCustomer != null) ? existingCustomer.PHONE1 : model.Phone1;
        //    customer.PHONE2 = (existingCustomer != null) ? existingCustomer.PHONE2 : model.Phone2;
        //    customer.PHONE3 = (existingCustomer != null) ? existingCustomer.PHONE3 : model.Phone3;
        //    customer.FAX = (existingCustomer != null) ? existingCustomer.FAX : model.FaxNumber;
        //    customer.PRBTADCD = (existingCustomer != null) ? existingCustomer.PRBTADCD : model.AddressCode;//same as address code
        //    customer.PRSTADCD = (existingCustomer != null) ? existingCustomer.PRSTADCD : model.AddressCode;//same as address code
        //    customer.STADDRCD = (existingCustomer != null) ? existingCustomer.STADDRCD : model.AddressCode;//same as address code
        //    customer.SLPRSNID = (salesperson != null) ? salesperson.Id : " ";
        //    customer.CHEKBKID = (existingCustomer != null) ? existingCustomer.CHEKBKID : " ";
        //    customer.PYMTRMID = (paymentTerm != null) ? paymentTerm.Description : " ";
        //    customer.CRLMTTYP = (existingCustomer != null) ? existingCustomer.CRLMTTYP : (Int16)1;
        //    customer.CRLMTAMT = (existingCustomer != null) ? existingCustomer.CRLMTAMT : 0.000000m;
        //    customer.CRLMTPER = (existingCustomer != null) ? existingCustomer.CRLMTPER : (Int16)0;
        //    customer.CRLMTPAM = (existingCustomer != null) ? existingCustomer.CRLMTPAM : 0.000000m;
        //    customer.CURNCYID = (existingCustomer != null) ? existingCustomer.CURNCYID : " ";
        //    customer.RATETPID = (existingCustomer != null) ? existingCustomer.RATETPID : " ";
        //    customer.CUSTDISC = (existingCustomer != null) ? existingCustomer.CUSTDISC : (Int16)0;
        //    customer.PRCLEVEL = (existingCustomer != null) ? existingCustomer.PRCLEVEL : " ";
        //    customer.MINPYTYP = (existingCustomer != null) ? existingCustomer.MINPYTYP : (Int16)0;
        //    customer.MINPYDLR = (existingCustomer != null) ? existingCustomer.MINPYDLR : 0.000000m;
        //    customer.MINPYPCT = (existingCustomer != null) ? existingCustomer.MINPYPCT : (Int16)0;
        //    customer.FNCHATYP = (existingCustomer != null) ? existingCustomer.FNCHATYP : (Int16)0;
        //    customer.FNCHPCNT = (existingCustomer != null) ? existingCustomer.FNCHPCNT : (Int16)0;
        //    customer.FINCHDLR = (existingCustomer != null) ? existingCustomer.FINCHDLR : 0.000000m;
        //    customer.MXWOFTYP = (existingCustomer != null) ? existingCustomer.MXWOFTYP : (Int16)0;
        //    customer.MXWROFAM = (existingCustomer != null) ? existingCustomer.MXWROFAM : 0.000000m;
        //    customer.COMMENT1 = (existingCustomer != null) ? existingCustomer.COMMENT1 : " ";
        //    customer.COMMENT2 = (existingCustomer != null) ? existingCustomer.COMMENT2 : " ";
        //    customer.USERDEF1 = (existingCustomer != null) ? existingCustomer.USERDEF1 : " ";
        //    customer.USERDEF2 = (existingCustomer != null) ? existingCustomer.USERDEF2 : " ";
        //    customer.TAXEXMT1 = (existingCustomer != null) ? existingCustomer.TAXEXMT1 : " ";
        //    customer.TAXEXMT2 = (existingCustomer != null) ? existingCustomer.TAXEXMT2 : " ";
        //    customer.TXRGNNUM = (existingCustomer != null) ? existingCustomer.TXRGNNUM : " ";
        //    customer.BALNCTYP = (existingCustomer != null) ? existingCustomer.BALNCTYP : (Int16)0;
        //    customer.STMTCYCL = (existingCustomer != null) ? existingCustomer.STMTCYCL : (Int16)5;
        //    customer.BANKNAME = (existingCustomer != null) ? existingCustomer.BANKNAME : " ";
        //    customer.BNKBRNCH = (existingCustomer != null) ? existingCustomer.BNKBRNCH : " ";
        //    //customer.SALSTERR = model.SALSTERR;
        //    customer.DEFCACTY = (existingCustomer != null) ? existingCustomer.DEFCACTY : (Int16)0;
        //    customer.RMCSHACC = (existingCustomer != null) ? existingCustomer.RMCSHACC : 0;
        //    customer.RMARACC = (existingCustomer != null) ? existingCustomer.RMARACC : 0;
        //    customer.RMSLSACC = (existingCustomer != null) ? existingCustomer.RMSLSACC : 0;
        //    customer.RMIVACC = (existingCustomer != null) ? existingCustomer.RMIVACC : 0;
        //    customer.RMCOSACC = (existingCustomer != null) ? existingCustomer.RMCOSACC : 0;
        //    customer.RMTAKACC = (existingCustomer != null) ? existingCustomer.RMTAKACC : 0;
        //    customer.RMAVACC = (existingCustomer != null) ? existingCustomer.RMAVACC : 0;
        //    customer.RMFCGACC = (existingCustomer != null) ? existingCustomer.RMFCGACC : 0;
        //    customer.RMWRACC = (existingCustomer != null) ? existingCustomer.RMWRACC : 0;
        //    customer.RMSORACC = (existingCustomer != null) ? existingCustomer.RMSORACC : 0;
        //    customer.FRSTINDT = (existingCustomer != null) ? existingCustomer.FRSTINDT : DateTime.Parse("1900-01-01 00:00:00.000");
        //    customer.INACTIVE = (existingCustomer != null) ? existingCustomer.INACTIVE : (byte)0;
        //    customer.HOLD = (existingCustomer != null) ? existingCustomer.HOLD : (byte)0;
        //    customer.CRCARDID = (existingCustomer != null) ? existingCustomer.CRCARDID : " ";
        //    customer.CRCRDNUM = (existingCustomer != null) ? existingCustomer.CRCRDNUM : " ";
        //    customer.CCRDXPDT = (existingCustomer != null) ? existingCustomer.CCRDXPDT : DateTime.Parse("1900-01-01 00:00:00.000");
        //    customer.KPDSTHST = (existingCustomer != null) ? existingCustomer.KPDSTHST : (byte)1;
        //    customer.KPCALHST = (existingCustomer != null) ? existingCustomer.KPCALHST : (byte)1;
        //    customer.KPERHIST = (existingCustomer != null) ? existingCustomer.KPERHIST : (byte)1;
        //    customer.KPTRXHST = (existingCustomer != null) ? existingCustomer.KPTRXHST : (byte)1;
        //    customer.NOTEINDX = (existingCustomer != null) ? existingCustomer.NOTEINDX : 0.00000m;
        //    customer.CREATDDT = (existingCustomer != null) ? existingCustomer.CREATDDT : model.CreatedDate;
        //    customer.MODIFDT = (existingCustomer != null) ? existingCustomer.MODIFDT : model.ModifiedDate;
        //    customer.Revalue_Customer = (existingCustomer != null) ? existingCustomer.Revalue_Customer : (byte)1;
        //    customer.Post_Results_To = (existingCustomer != null) ? existingCustomer.Post_Results_To : (Int16)0;
        //    customer.FINCHID = (existingCustomer != null) ? existingCustomer.FINCHID : " ";
        //    customer.GOVCRPID = (existingCustomer != null) ? existingCustomer.GOVCRPID : " ";
        //    customer.GOVINDID = (existingCustomer != null) ? existingCustomer.GOVINDID : " ";
        //    customer.DISGRPER = (existingCustomer != null) ? existingCustomer.DISGRPER : (Int16)0;
        //    customer.DUEGRPER = (existingCustomer != null) ? existingCustomer.DUEGRPER : (Int16)0;
        //    customer.DOCFMTID = (existingCustomer != null) ? existingCustomer.DOCFMTID : " ";
        //    customer.Send_Email_Statements = (existingCustomer != null) ? existingCustomer.Send_Email_Statements : (byte)0;
        //    customer.USERLANG = (existingCustomer != null) ? existingCustomer.USERLANG : (Int16)0;
        //    customer.GPSFOINTEGRATIONID = (existingCustomer != null) ? existingCustomer.GPSFOINTEGRATIONID : " ";
        //    customer.INTEGRATIONSOURCE = (existingCustomer != null) ? existingCustomer.INTEGRATIONSOURCE : (Int16)0;
        //    customer.INTEGRATIONID = (existingCustomer != null) ? existingCustomer.INTEGRATIONID : " ";
        //    customer.ORDERFULFILLDEFAULT = (existingCustomer != null) ? existingCustomer.ORDERFULFILLDEFAULT : (Int16)1;
        //    customer.CUSTPRIORITY = (existingCustomer != null) ? existingCustomer.CUSTPRIORITY : (Int16)1;
        //    customer.CCode = (existingCustomer != null) ? existingCustomer.CCode : " ";
        //    customer.DECLID = (existingCustomer != null) ? existingCustomer.DECLID : " ";
        //    customer.RMOvrpymtWrtoffAcctIdx = (existingCustomer != null) ? existingCustomer.RMOvrpymtWrtoffAcctIdx : 0;
        //    customer.SHIPCOMPLETE = (existingCustomer != null) ? existingCustomer.SHIPCOMPLETE : (byte)0;
        //    customer.CBVAT = (existingCustomer != null) ? existingCustomer.CBVAT : (byte)0;
        //    customer.INCLUDEINDP = (existingCustomer != null) ? existingCustomer.INCLUDEINDP : (byte)0;
        //    //customer.DEX_ROW_TS = model.DEX_ROW_TS;
        //    //customer.DEX_ROW_ID = (existingCustomer != null) ? existingCustomer.DEX_ROW_ID : customerCount + 1;

        //    if (_customerDynamicsRepository != null)
        //    {
        //        _customerDynamicsRepository.Dispose();
        //        _customerDynamicsRepository = null;
        //    }

        //    if (_countryRepository != null)
        //    {
        //        _countryRepository.Dispose();
        //        _countryRepository = null;
        //    }

        //    if (_stateRepository != null)
        //    {
        //        _stateRepository.Dispose();
        //        _stateRepository = null;
        //    }

        //    if (_salespersonRepository != null)
        //    {
        //        _salespersonRepository.Dispose();
        //        _salespersonRepository = null;
        //    }

        //    if (_paymentTermRepository != null)
        //    {
        //        _paymentTermRepository.Dispose();
        //        _paymentTermRepository = null;
        //    }

            return customer;
        }
    }
}