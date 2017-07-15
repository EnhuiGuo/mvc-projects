using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using SouthlandMetals.Common.Models;
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
    public class SalespersonDynamicsRepository : ISalespersonDynamicsRepository, IDisposable
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public SalespersonDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// get all selectable salespersons
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableSalespersons()
        {
            var salespersons = new List<SelectListItem>();

            try
            {
                salespersons = _dynamicsContext.RM00301_Salesperson.Where(x => x.INACTIVE != 1)
                                                                   .Select(y => new SelectListItem()
                                                                   {
                                                                       Text = y.SLPRSNFN + " " + y.SPRSNSLN,
                                                                       Value = y.SLPRSNID.TrimEnd()
                                                                   }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting salespersons: {0} ", ex.ToString());
            }

            return salespersons;
        }

        /// <summary>
        /// get all salespersons
        /// </summary>
        /// <returns></returns>
        public List<RM00301_Salesperson> GetSalespersons()
        {
            var salespersons = new List<RM00301_Salesperson>();

            try
            {
                salespersons = _dynamicsContext.RM00301_Salesperson.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting salespersons: {0} ", ex.ToString());
            }

            return salespersons;
        }

        /// <summary>
        /// get salesperson by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RM00301_Salesperson GetSalesperson(string id)
        {
            var salesperson = new RM00301_Salesperson();

            try
            {
                salesperson = _dynamicsContext.RM00301_Salesperson.FirstOrDefault(x => x.SLPRSNID.Replace(" ", string.Empty).ToLower() == id.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting salesperson: {0} ", ex.ToString());
            }

            return salesperson;
        }

        /// <summary>
        /// get salesperson by customer 
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        public RM00301_Salesperson GetSalespersonByCustomer(string customerNumber)
        {
            var salesperson = new RM00301_Salesperson();

            try
            {
                var customer = _dynamicsContext.RM00101_Customer.FirstOrDefault(x => x.CUSTNMBR.Replace(" ", string.Empty).ToLower() == customerNumber.Replace(" ", string.Empty).ToLower());
                salesperson = _dynamicsContext.RM00301_Salesperson.FirstOrDefault(x => x.SLPRSNID.Replace(" ", string.Empty).ToLower() == customer.SLPRSNID.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting salesperson: {0} ", ex.ToString());
            }

            return salesperson;
        }

        /// <summary>
        /// save new salesperson
        /// </summary>
        /// <param name="salesperson"></param>
        /// <returns></returns>
        public OperationResult SaveSalesperson(RM00301_Salesperson salesperson)
        {
            var operationResult = new OperationResult();

            var existingSalesperson = _dynamicsContext.RM00301_Salesperson.FirstOrDefault(x => x.SLPRSNID.Replace(" ", string.Empty) == salesperson.SLPRSNID);

            if (existingSalesperson == null)
            {
                logger.Debug("Salesperson is being created...");

                string sCustomerDocument;
                string sXsdSchema;
                string sConnectionString;

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        // Create the customer data file
                        SerializeSalespersonObject("Customer.xml");

                        // Use an XML document to create a string representation of the customer
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load("Customer.xml");
                        sCustomerDocument = xmldoc.OuterXml;

                        // Specify the Microsoft Dynamics GP server and database in the connection string
                        sConnectionString = @"data source=localhost;initial catalog=TWO;integrated security=SSPI;persist security info=False;packet size=4096";

                        // Create an XML Document object for the schema
                        XmlDocument XsdDoc = new XmlDocument();

                        // Create a string representing the eConnect schema
                        sXsdSchema = XsdDoc.OuterXml;

                        // Pass in xsdSchema to validate against.
                        e.CreateEntity(sConnectionString, sCustomerDocument);

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
                        logger.ErrorFormat("Error saving new salesperson: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new salesperson: {0} ", ex.ToString());
                    }
                    finally
                    {
                        // Call the Dispose method to release the resources
                        // of the eConnectMethds object
                        e.Dispose();
                    }
                } // end of using statement

                //try
                //{
                //    var newSalesperson = new RM00301_Salesperson()
                //    {
                //        SLPRSNID = salesperson.SLPRSNID,
                //        EMPLOYID = salesperson.EMPLOYID,
                //        VENDORID = salesperson.VENDORID,
                //        SLPRSNFN = salesperson.SLPRSNFN,
                //        SPRSNSMN = salesperson.SPRSNSMN,
                //        SPRSNSLN = salesperson.SPRSNSLN,
                //        ADDRESS1 = salesperson.ADDRESS1,
                //        ADDRESS2 = salesperson.ADDRESS2,
                //        ADDRESS3 = salesperson.ADDRESS3,
                //        CITY = salesperson.CITY,
                //        STATE = salesperson.STATE,
                //        ZIP = salesperson.ZIP,
                //        COUNTRY = salesperson.COUNTRY,
                //        PHONE1 = salesperson.PHONE1,
                //        PHONE2 = salesperson.PHONE2,
                //        PHONE3 = salesperson.PHONE3,
                //        FAX = salesperson.FAX,
                //        INACTIVE = salesperson.INACTIVE,
                //        SALSTERR = salesperson.SALSTERR,
                //        COMMCODE = salesperson.COMMCODE,
                //        COMPRCNT = salesperson.COMPRCNT,
                //        STDCPRCT = salesperson.STDCPRCT,
                //        COMAPPTO = salesperson.COMAPPTO,
                //        COSTTODT = salesperson.COSTTODT,
                //        CSTLSTYR = salesperson.CSTLSTYR,
                //        TTLCOMTD = salesperson.TTLCOMTD,
                //        TTLCOMLY = salesperson.TTLCOMLY,
                //        COMSLTDT = salesperson.COMSLTDT,
                //        COMSLLYR = salesperson.COMSLLYR,
                //        NCOMSLTD = salesperson.NCOMSLTD,
                //        NCOMSLYR = salesperson.NCOMSLYR,
                //        KPCALHST = salesperson.KPCALHST,
                //        KPERHIST = salesperson.KPERHIST,
                //        NOTEINDX = salesperson.NOTEINDX,
                //        MODIFDT = salesperson.MODIFDT,
                //        CREATDDT = salesperson.CREATDDT,
                //        COMMDEST = salesperson.COMMDEST,
                //        DEX_ROW_TS = salesperson.DEX_ROW_TS,
                //        DEX_ROW_ID = salesperson.DEX_ROW_ID
                //    };

                //    _dynamicsContext.RM00301_Salesperson.InsertOnSubmit(newSalesperson);

                //    _dynamicsContext.SubmitChanges();

                //    operationResult.Success = true;
                //    operationResult.Message = "Success";
                //}
                //catch (Exception ex)
                //{
                //    operationResult.Success = false;
                //    operationResult.Message = "Error";
                //    logger.ErrorFormat("Error saving new salesperson: {0} ", ex.ToString());
                //}
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Duplicate Entry";
            }

            return operationResult;
        }

        /// <summary>
        /// update salesperson
        /// </summary>
        /// <param name="salesperson"></param>
        /// <returns></returns>
        public OperationResult UpdateSalesperson(RM00301_Salesperson salesperson)
        {
            var operationResult = new OperationResult();

            var existingSalesperson = _dynamicsContext.RM00301_Salesperson.FirstOrDefault(x => x.SLPRSNID.Replace(" ", string.Empty) == salesperson.SLPRSNID);

            if (existingSalesperson != null)
            {
                logger.Debug("Salesperson is being updated.");

                try
                {
                    existingSalesperson.SLPRSNID = salesperson.SLPRSNID;
                    existingSalesperson.EMPLOYID = salesperson.EMPLOYID;
                    existingSalesperson.VENDORID = salesperson.VENDORID;
                    existingSalesperson.SLPRSNFN = salesperson.SLPRSNFN;
                    existingSalesperson.SPRSNSMN = salesperson.SPRSNSMN;
                    existingSalesperson.SPRSNSLN = salesperson.SPRSNSLN;
                    existingSalesperson.ADDRESS1 = salesperson.ADDRESS1;
                    existingSalesperson.ADDRESS2 = salesperson.ADDRESS2;
                    existingSalesperson.ADDRESS3 = salesperson.ADDRESS3;
                    existingSalesperson.CITY = salesperson.CITY;
                    existingSalesperson.STATE = salesperson.STATE;
                    existingSalesperson.ZIP = salesperson.ZIP;
                    existingSalesperson.COUNTRY = salesperson.COUNTRY;
                    existingSalesperson.PHONE1 = salesperson.PHONE1;
                    existingSalesperson.PHONE2 = salesperson.PHONE2;
                    existingSalesperson.PHONE3 = salesperson.PHONE3;
                    existingSalesperson.FAX = salesperson.FAX;
                    existingSalesperson.INACTIVE = salesperson.INACTIVE;
                    existingSalesperson.SALSTERR = salesperson.SALSTERR;
                    existingSalesperson.COMMCODE = salesperson.COMMCODE;
                    existingSalesperson.COMPRCNT = salesperson.COMPRCNT;
                    existingSalesperson.STDCPRCT = salesperson.STDCPRCT;
                    existingSalesperson.COMAPPTO = salesperson.COMAPPTO;
                    existingSalesperson.COSTTODT = salesperson.COSTTODT;
                    existingSalesperson.CSTLSTYR = salesperson.CSTLSTYR;
                    existingSalesperson.TTLCOMTD = salesperson.TTLCOMTD;
                    existingSalesperson.TTLCOMLY = salesperson.TTLCOMLY;
                    existingSalesperson.COMSLTDT = salesperson.COMSLTDT;
                    existingSalesperson.COMSLLYR = salesperson.COMSLLYR;
                    existingSalesperson.NCOMSLTD = salesperson.NCOMSLTD;
                    existingSalesperson.NCOMSLYR = salesperson.NCOMSLYR;
                    existingSalesperson.KPCALHST = salesperson.KPCALHST;
                    existingSalesperson.KPERHIST = salesperson.KPERHIST;
                    existingSalesperson.NOTEINDX = salesperson.NOTEINDX;
                    existingSalesperson.MODIFDT = salesperson.MODIFDT;
                    existingSalesperson.CREATDDT = salesperson.CREATDDT;
                    existingSalesperson.COMMDEST = salesperson.COMMDEST;
                    existingSalesperson.DEX_ROW_TS = salesperson.DEX_ROW_TS;
                    existingSalesperson.DEX_ROW_ID = salesperson.DEX_ROW_ID;

                    _dynamicsContext.SubmitChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating salesperson: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected salesperson.";
            }

            return operationResult;
        }

        /// <summary>
        /// serialize sales person
        /// </summary>
        /// <param name="filename"></param>
        public static void SerializeSalespersonObject(string filename)
        {
            try
            {
                // Instantiate an eConnectType schema object
                eConnectType eConnect = new eConnectType();

                // Instantiate a RMSalespersonMasterType schema object
                RMSalespersonMasterType salespersonType = new RMSalespersonMasterType();

                // Instantiate a taCreateSalesperson XML node object
                taCreateSalesperson salesperson = new taCreateSalesperson();

                // Create an XML serializer object
                XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                // Populate elements of the taCreateSalesperson XML node object
                //salesperson.CUSTNMBR = "Customer001";
                //salesperson.CUSTNAME = "Customer 1";
                //salesperson.ADDRESS1 = "2002 60th St SW";
                //salesperson.ADRSCODE = "Primary";
                //salesperson.CITY = "NewCity";
                //salesperson.ZIPCODE = "52302";

                // Populate the RMSalespersonMasterType schema with the taCreateSalesperson XML node
                salespersonType.taCreateSalesperson = salesperson;
                RMSalespersonMasterType[] salespersonMaster = { salespersonType };

                // Populate the eConnectType object with the RMSalespersonMasterType schema object
                eConnect.RMSalespersonMasterType = salespersonMaster;

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
