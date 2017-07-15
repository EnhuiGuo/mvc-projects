using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace SouthlandMetals.Dynamics.Domain.Repositories
{
    public class CustomerDynamicsRepository : ICustomerDynamicsRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public CustomerDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// get selectable customers
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableCustomers()
        {
            var customers = new List<SelectListItem>();

            try
            {
                var tempCustomers = _dynamicsContext.RM00101_Customer.Where(x => x.INACTIVE != 1 &&
                                                                                 x.CUSTCLAS.Replace(" ", string.Empty).ToLower().Equals("foundry"))
                                                                                  .OrderBy(x => x.SHRTNAME).ToList();
                foreach (var tempCustomer in tempCustomers)
                {
                    var selectableCustomer = new SelectListItem() { Text = tempCustomer.SHRTNAME, Value = tempCustomer.CUSTNMBR.TrimEnd() };

                    customers.Add(selectableCustomer);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customers: {0} ", ex.ToString());
            }

            return customers.OrderBy(z => z.Text).ToList();
        }

        /// <summary>
        /// get selectable customers by country
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableCustomersByCountry(Guid countryId)
        {
            var customers = new List<SelectListItem>();

            var _countryRepository = new CountryRepository();

            try
            {
                var country = _countryRepository.GetCountry(countryId);

                var customerAddresses = _dynamicsContext.RM00102_CustomerAddress.Where(x => x.COUNTRY.Replace(" ", string.Empty).ToLower() == country.Name.Replace(" ", string.Empty).ToLower()).ToList();

                foreach (var customerAddress in customerAddresses)
                {
                    var customer = GetCustomer(customerAddress.CUSTNMBR);

                    if (customer != null && customer.INACTIVE != 1 && customer.CUSTCLAS.Replace(" ", string.Empty).ToLower().Equals("foundry"))
                    {
                        var selectListItem = new SelectListItem() { Text = customer.SHRTNAME, Value = customer.CUSTNMBR.TrimEnd() };

                        customers.Add(selectListItem);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customers: {0} ", ex.ToString());
            }

            return customers.OrderBy(z => z.Text).ToList();
        }

        /// <summary>
        /// get all customers 
        /// </summary>
        /// <returns></returns>
        public List<RM00101_Customer> GetCustomers()
        {
            var customers = new List<RM00101_Customer>();

            try
            {
                customers = _dynamicsContext.RM00101_Customer.Where(x => !x.CUSTCLAS.Replace(" ", string.Empty).ToLower().Equals("foundry")).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customers: {0} ", ex.ToString());
            }

            return customers;
        }

        /// <summary>
        /// get customer by customer number
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        public RM00101_Customer GetCustomer(string customerNumber)
        {
            var customer = new RM00101_Customer();

            try
            {
                customer = _dynamicsContext.RM00101_Customer.FirstOrDefault(x => x.CUSTNMBR.Replace(" ", string.Empty).ToLower() == customerNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer: {0} ", ex.ToString());
            }

            return customer;
        }

        /// <summary>
        /// save new customer
        /// </summary>
        /// <param name="newCustomer"></param>
        /// <returns></returns>
        public OperationResult SaveCustomer(RM00101_Customer newCustomer)
        {
            var operationResult = new OperationResult();

            var existingCustomer = _dynamicsContext.RM00101_Customer.FirstOrDefault(x => x.CUSTNMBR.Replace(" ", string.Empty) == newCustomer.CUSTNMBR);

            if (existingCustomer == null)
            {
                logger.Debug("Customer is being created...");

                string sCustomerDocument;
                string sXsdSchema;
                string sConnectionString;

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        //// Create the vendor data file
                        //SerializeVendorObject("Vendor.xml", foundry);

                        //// Use an XML document to create a string representation of the customer
                        //XmlDocument xmldoc = new XmlDocument();
                        //xmldoc.Load("Vendor.xml");
                        //sCustomerDocument = xmldoc.OuterXml;

                        //// Specify the Microsoft Dynamics GP server and database in the connection string
                        //sConnectionString = @"data source=localhost;initial catalog=TWO;integrated security=SSPI;persist security info=False;packet size=4096";

                        //// Create an XML Document object for the schema
                        //XmlDocument XsdDoc = new XmlDocument();

                        //// Create a string representing the eConnect schema
                        //sXsdSchema = XsdDoc.OuterXml;

                        //// Pass in xsdSchema to validate against.
                        //e.CreateEntity(sConnectionString, sCustomerDocument);

                        // Instantiate a taUpdateCreateCustomerRcd XML node object
                        taUpdateCreateCustomerRcd customer = new taUpdateCreateCustomerRcd();

                        //Populate elements of the taUpdateCreateVendorRcd XML node object
                        customer.CUSTNMBR = newCustomer.CUSTNMBR;
                        customer.UseCustomerClass = 1;
                        customer.CUSTCLAS = "blah";
                        customer.UpdateIfExists = 0;

                        // Instantiate a RMCustomerMasterType schema object
                        RMCustomerMasterType customertype = new RMCustomerMasterType();

                        // Populate the RMCustomerMasterType schema with the taUpdateCreateCustomerRcd XML node
                        customertype.taUpdateCreateCustomerRcd = customer;
                        RMCustomerMasterType[] customerMaster = { customertype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the PMVendorMasterType schema object
                        eConnect.RMCustomerMasterType = customerMaster;

                        // Serialize the eConnectType.
                        serializer.Serialize(memoryStream, eConnect);

                        // Reset the position of the memory stream to the start.              
                        memoryStream.Position = 0;

                        // Create an XmlDocument from the serialized eConnectType in memory.
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(memoryStream);
                        memoryStream.Close();

                        // Call eConnect to process the XmlDocument.
                        e.CreateEntity(_dynamicsConnection, xmlDocument.OuterXml);

                        operationResult.Success = true;
                        operationResult.Message = "Success";
                    }
                    // The eConnectException class will catch eConnect business logic errors.
                    // display the error message on the console
                    catch (eConnectException exc)
                    {
                        Console.Write(exc.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new customer: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new customer: {0} ", ex.ToString());
                    }
                    finally
                    {
                        // Call the Dispose method to release the resources
                        // of the eConnectMethds object
                        e.Dispose();
                    }
                } // end of using statement
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Duplicate Entry";
            }

            return operationResult;
        }

        /// <summary>
        /// serialize customer
        /// </summary>
        /// <param name="filename"></param>
        private void SerializeCustomerObject(string filename)
        {
            try
            {
                // Instantiate an eConnectType schema object
                eConnectType eConnect = new eConnectType();

                // Instantiate a RMCustomerMasterType schema object
                RMCustomerMasterType customerType = new RMCustomerMasterType();

                // Instantiate a taUpdateCreateCustomerRcd XML node object
                taUpdateCreateCustomerRcd customer = new taUpdateCreateCustomerRcd();

                // Create an XML serializer object
                XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                // Populate elements of the taUpdateCreateCustomerRcd XML node object
                customer.CUSTNMBR = "Customer001";
                customer.CUSTNAME = "Customer 1";
                customer.ADDRESS1 = "2002 60th St SW";
                customer.ADRSCODE = "Primary";
                customer.CITY = "NewCity";
                customer.ZIPCODE = "52302";

                // Populate the RMCustomerMasterType schema with the taUpdateCreateCustomerRcd XML node
                customerType.taUpdateCreateCustomerRcd = customer;
                RMCustomerMasterType[] customerMaster = { customerType };

                // Populate the eConnectType object with the RMCustomerMasterType schema object
                eConnect.RMCustomerMasterType = customerMaster;

                // Create objects to create file and write the customer XML to the file
                FileStream fs = new FileStream(filename, FileMode.Create);
                XmlTextWriter writer = new XmlTextWriter(fs, new UTF8Encoding());

                // Serialize the eConnectType object to a file using the XmlTextWriter.
                serializer.Serialize(writer, eConnect);
                writer.Close();
            }
            // catch any errors that occur and display them to the console
            catch (System.Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dynamicsContext.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
