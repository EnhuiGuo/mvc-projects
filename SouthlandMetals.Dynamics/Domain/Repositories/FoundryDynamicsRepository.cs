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
    public class FoundryDynamicsRepository : IFoundryDynamicsRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public FoundryDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// get selectable foundries
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableFoundries()
        {
            var foundries = new List<SelectListItem>();

            try
            {
                var tempFoundries = _dynamicsContext.PM00200_Foundry.Where(x => !x.VNDCLSID.Replace(" ", string.Empty).ToLower().Equals("trade") && 
                                                                           !x.VNDCLSID.Replace(" ", string.Empty).ToLower().Equals("tr/fdry") &&
                                                                           x.VENDSTTS == 1)
                                                                           .OrderBy(y => y.VENDSHNM).ToList();

                foreach (var tempFoundry in tempFoundries)
                {
                    var selectableFoundry = new SelectListItem() { Text = tempFoundry.VENDSHNM, Value = tempFoundry.VENDORID.TrimEnd() };

                    foundries.Add(selectableFoundry);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting foundries: {0} ", ex.ToString());
            }

            return foundries;
        }

        /// <summary>
        /// get all foundries
        /// </summary>
        /// <returns></returns>
        public List<PM00200_Foundry> GetFoundries()
        {
            var foundries = new List<PM00200_Foundry>();

            try
            {
                foundries = _dynamicsContext.PM00200_Foundry.Where(x => !x.VNDCLSID.Replace(" ", string.Empty).ToLower().Equals("trade") && 
                                                                        !x.VNDCLSID.Replace(" ", string.Empty).ToLower().Equals("tr/fdry")).OrderBy(y => y.VENDSHNM).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting foundries: {0} ", ex.ToString());
            }

            return foundries;
        }

        /// <summary>
        /// get foundry by id
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public PM00200_Foundry GetFoundry(string vendorId)
        {
            var foundry = new PM00200_Foundry();

            try
            {
                foundry = _dynamicsContext.PM00200_Foundry.FirstOrDefault(x => x.VENDORID.Replace(" ", string.Empty).ToLower() == vendorId.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting foundry: {0} ", ex.ToString());
            }

            return foundry;
        }

        /// <summary>
        /// save new foundry
        /// </summary>
        /// <param name="foundry"></param>
        /// <returns></returns>
        public OperationResult SaveFoundry(PM00200_Foundry foundry)
        {
            var operationResult = new OperationResult();

            var existingFoundry = _dynamicsContext.PM00200_Foundry.FirstOrDefault(x => x.VENDORID.Replace(" ", string.Empty) == foundry.VENDORID);

            if (existingFoundry == null)
            {
                logger.Debug("Foundry is being created...");

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        // Instantiate a taUpdateCreateCustomerRcd XML node object
                        taUpdateCreateVendorRcd vendor = new taUpdateCreateVendorRcd();

                        //Populate elements of the taUpdateCreateVendorRcd XML node object
                        vendor.VENDORID = foundry.VENDORID;
                        vendor.VENDNAME = foundry.VENDNAME;
                        vendor.VENDSHNM = foundry.VENDSHNM;
                        vendor.UseVendorClass = 1;
                        vendor.VNDCLSID = "SUO";
                        vendor.UpdateIfExists = 0;

                        // Instantiate a PMVendorMasterType schema object
                        PMVendorMasterType vendortype = new PMVendorMasterType();

                        // Populate the PMVendorMasterType schema with the taUpdateCreateVendorRcd XML node
                        vendortype.taUpdateCreateVendorRcd = vendor;
                        PMVendorMasterType[] vendorMaster = { vendortype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the PMVendorMasterType schema object
                        eConnect.PMVendorMasterType = vendorMaster;

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
                        logger.ErrorFormat("Error saving new foundry: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new foundry: {0} ", ex.ToString());
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
        /// update foundry
        /// </summary>
        /// <param name="foundry"></param>
        /// <returns></returns>
        public OperationResult UpdateFoundry(PM00200_Foundry foundry)
        {
            var operationResult = new OperationResult();

            var existingFoundry = _dynamicsContext.PM00200_Foundry.FirstOrDefault(x => x.VENDORID.Replace(" ", string.Empty) == foundry.VENDORID);

            if (existingFoundry != null)
            {
                logger.Debug("Foundry is being updated.");

                string sCustomerDocument;
                string sXsdSchema;
                string sConnectionString;

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        // Instantiate a taUpdateCreateCustomerRcd XML node object
                        taUpdateCreateVendorRcd vendor = new taUpdateCreateVendorRcd();

                        //Populate elements of the taUpdateCreateVendorRcd XML node object
                        vendor.VENDORID = foundry.VENDORID;
                        vendor.VENDNAME = foundry.VENDNAME;
                        vendor.VENDSHNM = foundry.VENDSHNM;
                        vendor.UseVendorClass = 0;
                        vendor.UpdateIfExists = 1;

                        // Instantiate a PMVendorMasterType schema object
                        PMVendorMasterType vendortype = new PMVendorMasterType();

                        // Populate the PMVendorMasterType schema with the taUpdateCreateVendorRcd XML node
                        vendortype.taUpdateCreateVendorRcd = vendor;
                        PMVendorMasterType[] vendorMaster = { vendortype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the PMVendorMasterType schema object
                        eConnect.PMVendorMasterType = vendorMaster;

                        // Serialize the eConnectType.
                        serializer.Serialize(memoryStream, eConnect);

                        // Reset the position of the memory stream to the start.              
                        memoryStream.Position = 0;

                        // Create an XmlDocument from the serialized eConnectType in memory.
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(memoryStream);
                        memoryStream.Close();

                        // Call eConnect to process the XmlDocument.
                        e.UpdateEntity(_dynamicsConnection, xmlDocument.OuterXml);

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
                        logger.ErrorFormat("Error while updating foundry: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error while updating foundry: {0} ", ex.ToString());
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
                operationResult.Message = "Unable to find selected foundry.";
            }

            return operationResult;
        }

        /// <summary>
        /// serialize vendor 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="foundry"></param>
        public static void SerializeVendorObject(string filename, PM00200_Foundry foundry)
        {
            try
            {
                // Instantiate an eConnectType schema object
                eConnectType eConnect = new eConnectType();

                // Instantiate a PMVendorMasterType schema object
                PMVendorMasterType vendortype = new PMVendorMasterType();

                // Instantiate a taUpdateCreateCustomerRcd XML node object
                taUpdateCreateVendorRcd vendor = new taUpdateCreateVendorRcd();

                // Create an XML serializer object
                XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                //Populate elements of the taUpdateCreateVendorRcd XML node object
                vendor.VENDORID = foundry.VENDORID;
                vendor.VENDNAME = foundry.VENDNAME;
                vendor.VENDSHNM = foundry.VENDSHNM;
                vendor.UseVendorClass = 1;
                vendor.VNDCLSID = "SUO";

                // Populate the PMVendorMasterType schema with the taUpdateCreateVendorRcd XML node
                vendortype.taUpdateCreateVendorRcd = vendor;
                PMVendorMasterType[] vendorMaster = { vendortype };

                // Populate the eConnectType object with the PMVendorMasterType schema object
                eConnect.PMVendorMasterType = vendorMaster;

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
