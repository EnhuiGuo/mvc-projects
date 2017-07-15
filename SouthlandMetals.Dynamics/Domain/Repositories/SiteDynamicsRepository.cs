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
    public class SiteDynamicsRepository : ISiteDynamicsRepository, IDisposable
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public SiteDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// get all selectable sites
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableSites()
        {
            var sites = new List<SelectListItem>();

            try
            {
                sites = _dynamicsContext.IV40700_Site.Where(x => x.INACTIVE != 1)
                                                                   .Select(y => new SelectListItem()
                                                                   {
                                                                       Text = y.LOCNDSCR,
                                                                       Value = y.LOCNCODE.TrimEnd()
                                                                   }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting sites: {0} ", ex.ToString());
            }

            return sites;
        }

        /// <summary>
        /// get sekectables sites by customer id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableSites(string customerId)
        {
            var sites = new List<SelectListItem>();

            try
            {
                var customerAddresses = _dynamicsContext.RM00102_CustomerAddress.Where(x => x.CUSTNMBR.Replace(" ", string.Empty) == customerId).ToList();

                if(customerAddresses != null && customerAddresses.Count > 0)
                {
                    foreach(var customerAddress in customerAddresses)
                    {
                        var site = _dynamicsContext.IV40700_Site.FirstOrDefault(x => x.LOCNCODE == customerAddress.LOCNCODE && x.INACTIVE != 1);

                        if(site != null)
                        {
                            var selectableSite = new SelectListItem()
                            {
                                Text = site.LOCNDSCR,
                                Value = site.LOCNCODE.TrimEnd()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting sites: {0} ", ex.ToString());
            }

            return sites.OrderBy(z => z.Text).ToList();
        }

        /// <summary>
        /// get all sites
        /// </summary>
        /// <returns></returns>
        public List<IV40700_Site> GetSites()
        {
            var sites = new List<IV40700_Site>();

            try
            {
                sites = _dynamicsContext.IV40700_Site.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting sites: {0} ", ex.ToString());
            }

            return sites;
        }

        /// <summary>
        /// get site by location code
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        public IV40700_Site GetSite(string locationCode)
        {
            var site = new IV40700_Site();

            try
            {
                site = _dynamicsContext.IV40700_Site.FirstOrDefault(x => x.LOCNCODE.Replace(" ", string.Empty).ToLower() == locationCode.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting site: {0} ", ex.ToString());
            }

            return site;
        }

        /// <summary>
        /// save new site
        /// </summary>
        /// <returns></returns>
        public OperationResult SaveSite()
        {
            var operationResult = new OperationResult();

                logger.Debug("Site is being created...");

                string sCustomerDocument;
                string sXsdSchema;
                string sConnectionString;

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        // Create the site data file
                        SerializeSiteObject("Site.xml");

                        // Use an XML document to create a string representation of the site
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load("Site.xml");
                        sCustomerDocument = xmldoc.OuterXml;

                        // Specify the Microsoft Dynamics GP server and database in the connection string
                        sConnectionString = @"data source=SLM-DYNAMICS;Database=STHLD;User Id=programmer;Password=programmer;initial catalog=TWO;integrated security=SSPI;persist security info=False;packet size=4096";

                        // Create an XML Document object for the schema
                        XmlDocument XsdDoc = new XmlDocument();

                        // Create a string representing the eConnect schema
                        sXsdSchema = XsdDoc.OuterXml;

                        // Pass in xsdSchema to validate against.
                        e.CreateEntity(sConnectionString, sCustomerDocument);

                        operationResult.Success = true;
                        operationResult.Message = "Successfully added a site.";
                    }
                    // The eConnectException class will catch eConnect business logic errors.
                    // display the error message on the console
                    catch (eConnectException exc)
                    {
                        Console.Write(exc.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new site: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new site: {0} ", ex.ToString());
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
                //    var newSite = new IV40700_Site()
                //    {
                //        LOCNCODE = site.LOCNCODE,
                //        LOCNDSCR = site.LOCNDSCR,
                //        NOTEINDX = site.NOTEINDX,
                //        ADDRESS1 = site.ADDRESS1,
                //        ADDRESS2 = site.ADDRESS2,
                //        ADDRESS3 = site.ADDRESS3,
                //        CITY = site.CITY,
                //        STATE = site.STATE,
                //        ZIPCODE = site.ZIPCODE,
                //        COUNTRY = site.COUNTRY,
                //        PHONE1 = site.PHONE1,
                //        PHONE2 = site.PHONE2,
                //        PHONE3 = site.PHONE3,
                //        FAXNUMBR = site.FAXNUMBR,
                //        Location_Segment = site.Location_Segment,
                //        STAXSCHD = site.STAXSCHD,
                //        PCTAXSCH = site.PCTAXSCH,
                //        INCLDDINPLNNNG = site.INCLDDINPLNNNG,
                //        PORECEIPTBIN = site.PORECEIPTBIN,
                //        PORETRNBIN = site.PORETRNBIN,
                //        SOFULFILLMENTBIN = site.SOFULFILLMENTBIN,
                //        SORETURNBIN = site.SORETURNBIN,
                //        BOMRCPTBIN = site.BOMRCPTBIN,
                //        MATERIALISSUEBIN = site.MATERIALISSUEBIN,
                //        WMSINT = site.WMSINT,
                //        PICKTICKETSITEOPT = site.PICKTICKETSITEOPT,
                //        BINBREAK = site.BINBREAK,
                //        CCode = site.CCode,
                //        DECLID = site.DECLID,
                //        INACTIVE = site.INACTIVE,
                //        DEX_ROW_ID = site.DEX_ROW_ID
                //    };

                //    _dynamicsContext.IV40700_Site.InsertOnSubmit(newSite);

                //    _dynamicsContext.SubmitChanges();

                //    operationResult.Success = true;
                //    operationResult.Message = "Success";
                //}
                //catch (Exception ex)
                //{
                //    operationResult.Success = false;
                //    operationResult.Message = "Error";
                //    logger.ErrorFormat("Error saving new site: {0} ", ex.ToString());
                //}

            return operationResult;
        }

        /// <summary>
        /// update site
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public OperationResult UpdateSite(IV40700_Site site)
        {
            var operationResult = new OperationResult();

            var existingSite = _dynamicsContext.IV40700_Site.FirstOrDefault(x => x.LOCNDSCR.ToLower() == site.LOCNDSCR.ToLower());

            if (existingSite != null)
            {
                logger.Debug("Site is being updated.");

                try
                {
                    existingSite.LOCNCODE = site.LOCNCODE;
                    existingSite.LOCNDSCR = site.LOCNDSCR;
                    existingSite.NOTEINDX = site.NOTEINDX;
                    existingSite.ADDRESS1 = site.ADDRESS1;
                    existingSite.ADDRESS2 = site.ADDRESS2;
                    existingSite.ADDRESS3 = site.ADDRESS3;
                    existingSite.CITY = site.CITY;
                    existingSite.STATE = site.STATE;
                    existingSite.ZIPCODE = site.ZIPCODE;
                    existingSite.COUNTRY = site.COUNTRY;
                    existingSite.PHONE1 = site.PHONE1;
                    existingSite.PHONE2 = site.PHONE2;
                    existingSite.PHONE3 = site.PHONE3;
                    existingSite.FAXNUMBR = site.FAXNUMBR;
                    existingSite.Location_Segment = site.Location_Segment;
                    existingSite.STAXSCHD = site.STAXSCHD;
                    existingSite.PCTAXSCH = site.PCTAXSCH;
                    existingSite.INCLDDINPLNNNG = site.INCLDDINPLNNNG;
                    existingSite.PORECEIPTBIN = site.PORECEIPTBIN;
                    existingSite.PORETRNBIN = site.PORETRNBIN;
                    existingSite.SOFULFILLMENTBIN = site.SOFULFILLMENTBIN;
                    existingSite.SORETURNBIN = site.SORETURNBIN;
                    existingSite.BOMRCPTBIN = site.BOMRCPTBIN;
                    existingSite.MATERIALISSUEBIN = site.MATERIALISSUEBIN;
                    existingSite.WMSINT = site.WMSINT;
                    existingSite.PICKTICKETSITEOPT = site.PICKTICKETSITEOPT;
                    existingSite.BINBREAK = site.BINBREAK;
                    existingSite.CCode = site.CCode;
                    existingSite.DECLID = site.DECLID;
                    existingSite.INACTIVE = site.INACTIVE;
                    existingSite.DEX_ROW_ID = site.DEX_ROW_ID;

                    _dynamicsContext.SubmitChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating site: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected site.";
            }

            return operationResult;
        }

        /// <summary>
        /// serialize site 
        /// </summary>
        /// <param name="filename"></param>
        public static void SerializeSiteObject(string filename)
        {
            try
            {
                // Instantiate an eConnectType schema object
                eConnectType eConnect = new eConnectType();

                // Instantiate a RMSalespersonMasterType schema object
                IVInventorySiteType siteType = new IVInventorySiteType();

                // Instantiate a taCreateInventorySite XML node object
                taCreateInventorySite site = new taCreateInventorySite();

                // Create an XML serializer object
                XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                // Populate elements of the taCreateInventorySite XML node object
                site.LOCNCODE = "Test Site Compsys";
                site.LOCNDSCR = "Test Site Compsys";
                //site.NOTEINDX = "Test Site Compsys";
                site.ADDRESS1 = "Test Site Compsys";
                site.ADDRESS2 = "Test Site Compsys";
                site.ADDRESS3 = "Test Site Compsys";
                site.CITY = "Test Site Compsys";
                site.STATE = "Test Site Compsys";
                site.ZIPCODE = "Test Site Compsys";
                site.COUNTRY = "Test Site Compsys";
                site.PHONE1 = "Test Site Compsys";
                site.PHONE2 = "Test Site Compsys";
                site.PHONE3 = "Test Site Compsys";
                site.FAXNUMBR = "Test Site Compsys";
                site.Location_Segment = "Test Site Compsys";
                site.STAXSCHD = "Test Site Compsys";
                site.PCTAXSCH = "Test Site Compsys";
                site.INCLDDINPLNNNG = 1;
                site.PORECEIPTBIN = "Test Site Compsys";
                site.PORETRNBIN = "Test Site Compsys";
                site.SOFULFILLMENTBIN = "Test Site Compsys";
                site.SORETURNBIN = "Test Site Compsys";
                site.BOMRCPTBIN = "Test Site Compsys";
                site.MATERIALISSUEBIN = "Test Site Compsys";
                //site.WMSINT = "Test Site Compsys";
                //site.PICKTICKETSITEOPT = "Test Site Compsys";
                //site.BINBREAK = "Test Site Compsys";
                site.CCode = "Test Site Compsys";
                //site.DECLID = "Test Site Compsys";
                //site.INACTIVE = "Test Site Compsys";
                //site.DEX_ROW_ID = "Test Site Compsys";

                // Populate the IVInventorySiteType schema with the taCreateInventorySite XML node
                siteType.taCreateInventorySite = site;
                IVInventorySiteType[] siteMaster = { siteType };

                // Populate the eConnectType object with the IVInventorySiteType schema object
                eConnect.IVInventorySiteType = siteMaster;

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
