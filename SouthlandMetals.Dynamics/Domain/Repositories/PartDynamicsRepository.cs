using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SouthlandMetals.Dynamics.Domain.Repositories
{
    public class PartDynamicsRepository : IPartDynamicsRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public PartDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// get all part masters
        /// </summary>
        /// <returns></returns>
        public List<IV00101_Part_Master> GetPartMasters()
        {
            var parts = new List<IV00101_Part_Master>();

            try
            {
                parts = _dynamicsContext.IV00101_Part_Master.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting master parts: {0} ", ex.ToString());
            }

            return parts;
        }

        /// <summary>
        /// get part quantity masters
        /// </summary>
        /// <returns></returns>
        public List<IV00102_Part_Quantity_Master> GetPartQuantityMasters()
        {
            var parts = new List<IV00102_Part_Quantity_Master>();

            try
            {
                parts = _dynamicsContext.IV00102_Part_Quantity_Master.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting quantity master parts: {0} ", ex.ToString());
            }

            return parts;
        }

        /// <summary>
        /// get part vendor masters
        /// </summary>
        /// <returns></returns>
        public List<IV00103_Part_Vendor_Master> GetPartVendorMasters()
        {
            var parts = new List<IV00103_Part_Vendor_Master>();

            try
            {
                parts = _dynamicsContext.IV00103_Part_Vendor_Master.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting vendor master parts: {0} ", ex.ToString());
            }

            return parts;
        }

        /// <summary>
        /// get all part currencies
        /// </summary>
        /// <returns></returns>
        public List<IV00105_Part_Currency> GetPartCurrencies()
        {
            var parts = new List<IV00105_Part_Currency>();

            try
            {
                parts = _dynamicsContext.IV00105_Part_Currency.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting parts currency: {0} ", ex.ToString());
            }

            return parts;
        }

        /// <summary>
        /// get all part purchasings
        /// </summary>
        /// <returns></returns>
        public List<IV00106_Part_Purchasing> GetPartPurchasings()
        {
            var parts = new List<IV00106_Part_Purchasing>();

            try
            {
                parts = _dynamicsContext.IV00106_Part_Purchasing.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting parts purchasing: {0} ", ex.ToString());
            }

            return parts;
        }

        /// <summary>
        /// get all part price options
        /// </summary>
        /// <returns></returns>
        public List<IV00107_Part_Price_Option> GetPartPriceOptions()
        {
            var parts = new List<IV00107_Part_Price_Option>();

            try
            {
                parts = _dynamicsContext.IV00107_Part_Price_Option.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting parts price options: {0} ", ex.ToString());
            }

            return parts;
        }

        /// <summary>
        /// get all part prices
        /// </summary>
        /// <returns></returns>
        public List<IV00108_Part_Price> GetPartPrices()
        {
            var parts = new List<IV00108_Part_Price>();

            try
            {
                parts = _dynamicsContext.IV00108_Part_Price.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting parts pricing: {0} ", ex.ToString());
            }

            return parts;
        }

        /// <summary>
        /// get part master by item number
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public IV00101_Part_Master GetPartMaster(string itemNumber)
        {
            var part = new IV00101_Part_Master();

            try
            {
                part = _dynamicsContext.IV00101_Part_Master.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == itemNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting master part: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get part quantity master by item Number
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public IV00102_Part_Quantity_Master GetPartQuantityMaster(string itemNumber)
        {
            var part = new IV00102_Part_Quantity_Master();

            try
            {
                part = _dynamicsContext.IV00102_Part_Quantity_Master.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == itemNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting quantity master part: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get part vendor master by item number
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public IV00103_Part_Vendor_Master GetPartVendorMaster(string itemNumber)
        {
            var part = new IV00103_Part_Vendor_Master();

            try
            {
                part = _dynamicsContext.IV00103_Part_Vendor_Master.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == itemNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting vendor master part: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get part currency by item number
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public IV00105_Part_Currency GetPartCurrency(string itemNumber)
        {
            var part = new IV00105_Part_Currency();

            try
            {
                part = _dynamicsContext.IV00105_Part_Currency.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == itemNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part currency: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get part purchasing by item number
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public IV00106_Part_Purchasing GetPartPurchasing(string itemNumber)
        {
            var part = new IV00106_Part_Purchasing();

            try
            {
                part = _dynamicsContext.IV00106_Part_Purchasing.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == itemNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part purchasing: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get part price option by item number
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public IV00107_Part_Price_Option GetPartPriceOption(string itemNumber)
        {
            var part = new IV00107_Part_Price_Option();

            try
            {
                part = _dynamicsContext.IV00107_Part_Price_Option.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == itemNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part price option: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get part price by item number
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public IV00108_Part_Price GetPartPrice(string itemNumber)
        {
            var part = new IV00108_Part_Price();

            try
            {
                part = _dynamicsContext.IV00108_Part_Price.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == itemNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting part price: {0} ", ex.ToString());
            }

            return part;
        }

        /// <summary>
        /// get item status by item number
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public DBO_ItemStatus GetItemStatus(string itemNumber)
        {
            var status = new DBO_ItemStatus();

            try
            {
                status = _dynamicsContext.DBO_ItemStatus.FirstOrDefault(x => x.Item_Number.Replace(" ", string.Empty).ToLower() == itemNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting item status: {0} ", ex.ToString());
            }

            return status;
        }

        /// <summary>
        /// get item sales by item number
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public DBO_ItemSales GetItemSales(string itemNumber)
        {
            var sales = new DBO_ItemSales();

            try
            {
                sales = _dynamicsContext.DBO_ItemSales.FirstOrDefault(x => x.Item_Number.Replace(" ", string.Empty).ToLower() == itemNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting item sales: {0} ", ex.ToString());
            }

            return sales;
        }

        /// <summary>
        /// get item rolling sales by item number
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        public DBO_ItemRollingSales GetItemRollingSales(string itemNumber)
        {
            var rollingSales = new DBO_ItemRollingSales();

            try
            {
                rollingSales = _dynamicsContext.DBO_ItemRollingSales.FirstOrDefault(x => x.Item_Number.Replace(" ", string.Empty).ToLower() == itemNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting item rolling sales: {0} ", ex.ToString());
            }

            return rollingSales;
        }

        /// <summary>
        /// save new part master
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public OperationResult SavePartMaster(IV00101_Part_Master part)
        {
            var operationResult = new OperationResult();

            var existingPart = _dynamicsContext.IV00101_Part_Master.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == part.ITEMNMBR.Replace(" ", string.Empty).ToLower());

            if (existingPart == null)
            {
                logger.Debug("Master Part is being created...");

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        // Instantiate a taUpdateCreateItemRcd XML node object
                        taUpdateCreateItemRcd item = new taUpdateCreateItemRcd();

                        //Populate elements of the taUpdateCreateItemRcd XML node object
                        item.ITEMNMBR = part.ITEMNMBR;
                        item.ITEMDESC = part.ITEMDESC;
                        item.UseItemClass = 1;
                        item.ITMCLSCD = part.ITMCLSCD;
                        item.UpdateIfExists = 0;

                        // Instantiate a IVItemMasterType schema object
                        IVItemMasterType itemtype = new IVItemMasterType();

                        // Populate the IVItemMasterType schema with the taUpdateCreateItemRcd XML node
                        itemtype.taUpdateCreateItemRcd = item;
                        IVItemMasterType[] itemMaster = { itemtype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the IVItemMasterType schema object
                        eConnect.IVItemMasterType = itemMaster;

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
                        logger.ErrorFormat("Error saving new part: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new part: {0} ", ex.ToString());
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
        /// save new part currency
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public OperationResult SavePartCurrency(IV00105_Part_Currency part)
        {
            var operationResult = new OperationResult();

            var existingPart = _dynamicsContext.IV00105_Part_Currency.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == part.ITEMNMBR.Replace(" ", string.Empty).ToLower());

            if (existingPart == null)
            {
                logger.Debug("Master Part is being created...");

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        // Instantiate a taUpdateCreateItemRcd XML node object
                        taUpdateCreateItemCurrencyRcd_ItemsTaUpdateCreateItemCurrencyRcd item = new taUpdateCreateItemCurrencyRcd_ItemsTaUpdateCreateItemCurrencyRcd();

                        //Populate elements of the taUpdateCreateItemRcd XML node object
                        item.ITEMNMBR = part.ITEMNMBR;
                        //item.CURNCYID = part.CURNCYID;
                        item.UpdateIfExists = 0;

                        // Instantiate a IVItemMasterType schema object
                        IVItemMasterType itemtype = new IVItemMasterType();

                        // Populate the IVItemMasterType schema with the taUpdateCreateItemRcd XML node
                        itemtype.taUpdateCreateItemCurrencyRcd_Items = new taUpdateCreateItemCurrencyRcd_ItemsTaUpdateCreateItemCurrencyRcd[1] { item };
                        IVItemMasterType[] currencyItem = { itemtype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the IVItemMasterType schema object
                        eConnect.IVItemMasterType = currencyItem;

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
                        logger.ErrorFormat("Error saving new part: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new part: {0} ", ex.ToString());
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
        /// update part master
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public OperationResult UpdatePartMaster(IV00101_Part_Master part)
        {
            var operationResult = new OperationResult();

            var existingPart = _dynamicsContext.IV00101_Part_Master.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == part.ITEMNMBR.Replace(" ", string.Empty).ToLower());

            if (existingPart != null)
            {
                logger.Debug("Part Master is being updated.");

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(_dynamicsConnection))
                        {
                            connection.Open();

                            SqlCommand cmd = new SqlCommand("UPDATE dbo.IV00101 SET STNDCOST = @StandardCost, ITEMSHWT = @Weight WHERE ITEMNMBR = @ItemNumber");
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = connection;
                            cmd.Parameters.AddWithValue("@ItemNumber", part.ITEMNMBR);
                            cmd.Parameters.AddWithValue("@StandardCost", part.STNDCOST);
                            cmd.Parameters.AddWithValue("@Weight", part.ITEMSHWT);
                            cmd.ExecuteNonQuery();

                            connection.Close();
                        }

                        operationResult.Success = true;
                        operationResult.Message = "Success";
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error updating part currency: {0} ", ex.ToString());
                    }
                } // end of using statement
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected part currency.";
            }

            return operationResult;
        }

        /// <summary>
        /// update part currency
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public OperationResult UpdatePartCurrency(IV00105_Part_Currency part)
        {
            var operationResult = new OperationResult();

            var existingPart = _dynamicsContext.IV00105_Part_Currency.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == part.ITEMNMBR.Replace(" ", string.Empty).ToLower());

            if (existingPart != null)
            {
                logger.Debug("Part Currency is being updated.");

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(_dynamicsConnection))
                        {
                            connection.Open();

                            SqlCommand cmd = new SqlCommand("UPDATE dbo.IV00105 SET LISTPRCE = @ListPrice WHERE ITEMNMBR = @ItemNumber");
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = connection;
                            cmd.Parameters.AddWithValue("@ItemNumber", part.ITEMNMBR);
                            cmd.Parameters.AddWithValue("@ListPrice", part.LISTPRCE);
                            cmd.ExecuteNonQuery();

                            connection.Close();
                        }

                        operationResult.Success = true;
                        operationResult.Message = "Success";
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error updating part currency: {0} ", ex.ToString());
                    }
                } // end of using statement
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected part currency.";
            }

            return operationResult;
        }

        /// <summary>
        /// update part vendor master
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public OperationResult UpdatePartVendorMaster(IV00105_Part_Currency part)
        {
            var operationResult = new OperationResult();

            var existingPart = _dynamicsContext.IV00105_Part_Currency.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == part.ITEMNMBR.Replace(" ", string.Empty).ToLower());

            if (existingPart != null)
            {
                logger.Debug("Part Currency is being updated.");

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(_dynamicsConnection))
                        {
                            connection.Open();

                            SqlCommand cmd = new SqlCommand("UPDATE dbo.IV00105 SET LISTPRCE = @ListPrice WHERE ITEMNMBR = @ItemNumber");
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = connection;
                            cmd.Parameters.AddWithValue("@ItemNumber", part.ITEMNMBR);
                            cmd.Parameters.AddWithValue("@ListPrice", part.LISTPRCE);
                            cmd.ExecuteNonQuery();

                            connection.Close();
                        }

                        operationResult.Success = true;
                        operationResult.Message = "Success";
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error updating part currency: {0} ", ex.ToString());
                    }
                } // end of using statement
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected part currency.";
            }

            return operationResult;
        }

        /// <summary>
        /// save new part price option
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public OperationResult SavePartPriceOption(IV00107_Part_Price_Option part)
        {
            var operationResult = new OperationResult();

            var existingPart = _dynamicsContext.IV00107_Part_Price_Option.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == part.ITEMNMBR.Replace(" ", string.Empty).ToLower());

            if (existingPart == null)
            {
                logger.Debug("Part Price Option is being created...");

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        // Instantiate a taIVCreateItemPriceListHeader XML node object
                        taIVCreateItemPriceListHeader item = new taIVCreateItemPriceListHeader();

                        //Populate elements of the taIVCreateItemPriceListHeader XML node object
                        item.ITEMNMBR = part.ITEMNMBR;
                        //item.PRICMTHD = part.PRICMTHD;
                        item.PRCLEVEL = part.PRCLEVEL;
                        item.UOFM = part.UOFM;
                        item.CURNCYID = string.Empty;
                        item.UpdateIfExists = 0;

                        // Instantiate a IVItemMasterType schema object
                        IVItemMasterType itemtype = new IVItemMasterType();

                        // Populate the IVItemMasterType schema with the taIVCreateItemPriceListHeader XML node
                        itemtype.taIVCreateItemPriceListHeader = item;
                        IVItemMasterType[] itemMaster = { itemtype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the IVItemMasterType schema object
                        eConnect.IVItemMasterType = itemMaster;

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
                        logger.ErrorFormat("Error saving new part price header: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new part price header: {0} ", ex.ToString());
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
        /// save new part price
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public OperationResult SavePartPrice(IV00108_Part_Price part)
        {
            var operationResult = new OperationResult();

            var existingPart = _dynamicsContext.IV00108_Part_Price.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == part.ITEMNMBR.Replace(" ", string.Empty).ToLower());

            if (existingPart == null)
            {
                logger.Debug("Part Price is being created...");

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        // Instantiate a taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine XML node object
                        taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine item = new taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine();

                        //Populate elements of the taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine XML node object
                        item.ITEMNMBR = part.ITEMNMBR;
                        item.PRCLEVEL = part.PRCLEVEL;
                        item.UOFM = part.UOFM;
                        item.UpdateIfExists = 0;

                        // Instantiate a IVItemMasterType schema object
                        IVItemMasterType itemtype = new IVItemMasterType();

                        // Populate the IVItemMasterType schema with the taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine XML node
                        itemtype.taIVCreateItemPriceListLine_Items = new taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine[1] { item };
                        IVItemMasterType[] itemMaster = { itemtype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the IVItemMasterType schema object
                        eConnect.IVItemMasterType = itemMaster;

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
                        logger.ErrorFormat("Error saving new part price header: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new part price header: {0} ", ex.ToString());
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
        /// save part vendor master
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public OperationResult SavePartVendorMaster(IV00103_Part_Vendor_Master part)
        {
            var operationResult = new OperationResult();

            var existingPart = _dynamicsContext.IV00103_Part_Vendor_Master.FirstOrDefault(x => x.ITEMNMBR.Replace(" ", string.Empty).ToLower() == part.ITEMNMBR.Replace(" ", string.Empty).ToLower());

            if (existingPart == null)
            {
                logger.Debug("Vendor Part is being created...");

                using (eConnectMethods e = new eConnectMethods())
                {
                    try
                    {
                        // Instantiate a taCreateItemVendors_ItemsTaCreateItemVendors XML node object
                        taCreateItemVendors_ItemsTaCreateItemVendors item = new taCreateItemVendors_ItemsTaCreateItemVendors();

                        //Populate elements of the taCreateItemVendors_ItemsTaCreateItemVendors XML node object
                        item.ITEMNMBR = part.ITEMNMBR;
                        item.VENDORID = part.VENDORID;
                        item.VNDITNUM = part.VNDITNUM;
                        item.UpdateIfExists = 0;

                        // Instantiate a IVItemMasterType schema object
                        IVItemMasterType itemtype = new IVItemMasterType();

                        // Populate the IVItemMasterType schema with the taCreateItemVendors_ItemsTaCreateItemVendors XML node
                        itemtype.taCreateItemVendors_Items = new taCreateItemVendors_ItemsTaCreateItemVendors[1] { item };
                        IVItemMasterType[] vendorItem = { itemtype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the IVItemMasterType schema object
                        eConnect.IVItemMasterType = vendorItem;

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
                        logger.ErrorFormat("Error saving new part price header: {0} ", exc.ToString());
                    }
                    // Catch any system error that might occurr.
                    // display the error message on the console
                    catch (System.Exception ex)
                    {
                        Console.Write(ex.ToString());
                        operationResult.Success = false;
                        operationResult.Message = "Error";
                        logger.ErrorFormat("Error saving new part price header: {0} ", ex.ToString());
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
        /// serialize item 
        /// </summary>
        /// <param name="filename"></param>
        public static void SerializeItemObject(string filename)
        {
            try
            {
                // Instantiate an eConnectType schema object
                eConnectType eConnect = new eConnectType();

                // Instantiate a IVItemMasterType  schema object
                IVItemMasterType itemType = new IVItemMasterType();

                // Instantiate a taUpdateCreateItemRcd XML node object
                taUpdateCreateItemRcd item = new taUpdateCreateItemRcd();

                // Create an XML serializer object
                XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                // Populate elements of the taUpdateCreateVendorRcd XML node object
                //item.CUSTNMBR = "Customer001";
                //item.CUSTNAME = "Customer 1";
                //item.ADDRESS1 = "2002 60th St SW";
                //item.ADRSCODE = "Primary";
                //item.CITY = "NewCity";
                //item.ZIPCODE = "52302";

                // Populate the IVItemMasterType schema with the taUpdateCreateItemRcd XML node
                itemType.taUpdateCreateItemRcd = item;
                IVItemMasterType[] itemMaster = { itemType };

                // Populate the eConnectType object with the IVItemMasterType schema object
                eConnect.IVItemMasterType = itemMaster;

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
