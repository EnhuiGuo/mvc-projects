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
    public class CustomerAddressDynamicsRepository : ICustomerAddressDynamicsRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public CustomerAddressDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// get all selectable customer addresses
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableCustomerAddresses()
        {
            var addresses = new List<SelectListItem>();

            var _stateRepository = new StateRepository();

            try
            {
                var customerAddresses = GetCustomerAddresses().OrderBy(y => y.ADRSCODE).ToList();

                if (customerAddresses != null && customerAddresses.Count > 0)
                {
                    foreach (var customerAddress in customerAddresses)
                    {
                        var state = _stateRepository.GetState(customerAddress.STATE);

                        var selectListItem = new SelectListItem()
                        {
                            Text = customerAddress.ADDRESS1 + " " + customerAddress.CITY + ((state != null) ? ", " + state.Name : string.Empty),
                            Value = customerAddress.ADRSCODE.TrimEnd()
                        };

                        addresses.Add(selectListItem);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer addresses: {0} ", ex.ToString());
            }
            finally
            {
                if (_stateRepository != null)
                {
                    _stateRepository.Dispose();
                    _stateRepository = null;
                }
            }

            return addresses;
        }

        /// <summary>
        /// get selectable customer address by customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableCustomerAddresses(string customerId)
        {
            var addresses = new List<SelectListItem>();

            var _stateRepository = new StateRepository();

            try
            {
                var customerAddresses = GetCustomerAddresses().Where(x => x.CUSTNMBR.Replace(" ", string.Empty).ToLower() == customerId.Replace(" ", string.Empty).ToLower()).OrderBy(y => y.ADRSCODE).ToList();

                if (customerAddresses != null && customerAddresses.Count > 0)
                {
                    foreach (var customerAddress in customerAddresses)
                    {
                        var state = _stateRepository.GetState(customerAddress.STATE);

                            var selectListItem = new SelectListItem()
                            {
                                Text = customerAddress.ADDRESS1 + " " + customerAddress.CITY + ((state != null) ? ", " + state.Name : string.Empty),
                                Value = customerAddress.ADRSCODE.TrimEnd()
                            };

                            addresses.Add(selectListItem);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer addresses: {0} ", ex.ToString());
            }
            finally
            {
                if (_stateRepository != null)
                {
                    _stateRepository.Dispose();
                    _stateRepository = null;
                }
            }

            return addresses;
        }

        /// <summary>
        /// get all customer addresses
        /// </summary>
        /// <returns></returns>
        public List<RM00102_CustomerAddress> GetCustomerAddresses()
        {
            var addresses = new List<RM00102_CustomerAddress>();

            try
            {
                addresses = _dynamicsContext.RM00102_CustomerAddress.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer addresses: {0} ", ex.ToString());
            }

            return addresses;
        }

        /// <summary>
        /// get customer address by custoemr  
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        public List<RM00102_CustomerAddress> GetCustomerAddressesByCustomer(string customerNumber)
        {
            var addresses = new List<RM00102_CustomerAddress>();

            try
            {
                addresses = _dynamicsContext.RM00102_CustomerAddress.Where(x => x.CUSTNMBR.Replace(" ", string.Empty).ToLower() == customerNumber.Replace(" ", string.Empty).ToLower()).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer addresses: {0} ", ex.ToString());
            }

            return addresses;
        }

        /// <summary>
        /// get customer address by addresscode and customer
        /// </summary>
        /// <param name="addressCode"></param>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        public RM00102_CustomerAddress GetCustomerAddress(string addressCode, string customerNumber)
        {
            var address = new RM00102_CustomerAddress();

            try
            {
                address = _dynamicsContext.RM00102_CustomerAddress.FirstOrDefault(x => x.ADRSCODE.Replace(" ", string.Empty).ToLower() == addressCode.Replace(" ", string.Empty).ToLower() &&
                                                                                       x.CUSTNMBR.Replace(" ", string.Empty).ToLower() == customerNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer address: {0} ", ex.ToString());
            }

            return address;
        }

        /// <summary>
        /// get customer address by address code
        /// </summary>
        /// <param name="addressCode"></param>
        /// <returns></returns>
        public RM00102_CustomerAddress GetCustomerAddress(string addressCode)
        {
            var address = new RM00102_CustomerAddress();

            try
            {
                address = _dynamicsContext.RM00102_CustomerAddress.FirstOrDefault(x => x.ADRSCODE.Replace(" ", string.Empty).ToLower() == addressCode.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting customer address: {0} ", ex.ToString());
            }

            return address;
        }

        /// <summary>
        /// save customer address
        /// </summary>
        /// <param name="newAddress"></param>
        /// <returns></returns>
        public OperationResult SaveCustomerAddress(RM00102_CustomerAddress newAddress)
        {
            var operationResult = new OperationResult();

            var existingAddress = _dynamicsContext.RM00102_CustomerAddress.FirstOrDefault(x => x.ADDRESS1.ToLower() == newAddress.ADDRESS1.ToLower());

            if (existingAddress == null)
            {
                logger.Debug("Customer Address is being created...");

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

                        // Instantiate a taCreateCustomerAddress_ItemsTaCreateCustomerAddress XML node object
                        taCreateCustomerAddress_ItemsTaCreateCustomerAddress address = new taCreateCustomerAddress_ItemsTaCreateCustomerAddress();

                        //Populate elements of the taCreateCustomerAddress_ItemsTaCreateCustomerAddress XML node object
                        address.ADRSCODE = newAddress.ADRSCODE;
                        address.CUSTNMBR = newAddress.CUSTNMBR;
                        address.UpdateIfExists = 0;

                        // Instantiate a RMCustomerAddressType schema object
                        RMCustomerAddressType addresstype = new RMCustomerAddressType();

                        // Populate the RMCustomerAddressType schema with the taCreateCustomerAddress_Items XML node
                        addresstype.taCreateCustomerAddress_Items = new taCreateCustomerAddress_ItemsTaCreateCustomerAddress[1] { address };
                        RMCustomerAddressType[] customerAddress = { addresstype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the RMCustomerAddressType schema object
                        eConnect.RMCustomerAddressType = customerAddress;

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
                        logger.ErrorFormat("Error saving new customer address: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new customer address: {0} ", ex.ToString());
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
        /// serialize customer address
        /// </summary>
        /// <param name="filename"></param>
        public static void SerializeCustomerAddressObject(string filename)
        {
            try
            {
                // Instantiate an eConnectType schema object
                eConnectType eConnect = new eConnectType();

                // Instantiate a RMCustomerAddressType schema object
                RMCustomerAddressType addressType = new RMCustomerAddressType();

                // Instantiate a taCreateCustomerAddress_ItemsTaCreateCustomerAddress XML node object
                taCreateCustomerAddress_ItemsTaCreateCustomerAddress address = new taCreateCustomerAddress_ItemsTaCreateCustomerAddress();

                // Create an XML serializer object
                XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                // Populate elements of the taUpdateCreateCustomerRcd XML node object
                address.CUSTNMBR = "Customer001";
                //address.CUSTNAME = "Customer 1";
                //address.ADDRESS1 = "2002 60th St SW";
                //address.ADRSCODE = "Primary";
                //address.CITY = "NewCity";
                //address.ZIPCODE = "52302";

                // Populate the RMCustomerAddressType schema with the taCreateCustomerAddress_ItemsTaCreateCustomerAddress XML node
                addressType.taCreateCustomerAddress_Items = new taCreateCustomerAddress_ItemsTaCreateCustomerAddress[1] { address };
                RMCustomerAddressType[] customerAddress = { addressType };

                // Populate the eConnectType object with the RMCustomerAddressType schema object
                eConnect.RMCustomerAddressType = customerAddress;

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
