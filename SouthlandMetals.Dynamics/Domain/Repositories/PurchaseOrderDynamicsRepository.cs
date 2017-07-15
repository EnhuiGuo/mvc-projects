using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SouthlandMetals.Dynamics.Domain.Repositories
{
    public class PurchaseOrderDynamicsRepository : IPurchaseOrderDynamicsRepository, IDisposable
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public PurchaseOrderDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// save new vendor order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderLines"></param>
        /// <returns></returns>
        public OperationResult SaveVendorOrder(POP10100_PurchaseOrder_Work order, List<POP10110_PurchaseOrderLine_Work> orderLines)
        {
            var operationResult = new OperationResult();

            logger.Debug("Vendor Order is being created...");

            using (eConnectMethods e = new eConnectMethods())
            {
                try
                {
                    if (orderLines != null && orderLines.Count > 0)
                    {
                        taPoLine_ItemsTaPoLine[] lineItems = new taPoLine_ItemsTaPoLine[orderLines.Count];

                        var orderLineNumber = 16384;
                        var lineNumber = 0;

                        foreach (var orderLine in orderLines)
                        {
                            // Instantiate a taUpdateCreateItemRcd XML node object
                            taPoLine_ItemsTaPoLine orderLineItem = new taPoLine_ItemsTaPoLine();

                            //Populate elements of the taUpdateCreateItemRcd XML node object
                            orderLineItem.PONUMBER = orderLine.PONUMBER;
                            orderLineItem.VENDORID = orderLine.VENDORID;
                            orderLineItem.DOCDATE = orderLine.DOCDATE;
                            orderLineItem.LOCNCODE = orderLine.LOCNCODE;
                            orderLineItem.VNDITNUM = orderLine.VNDITNUM;
                            orderLineItem.ITEMNMBR = orderLine.ITEMNMBR;
                            orderLineItem.QUANTITY = orderLine.QUANTITY;
                            //DUE DATE
                            orderLineItem.PRMDATE = orderLine.PRMDATE;
                            //SHIP DATE
                            orderLineItem.PRMSHPDTE = orderLine.PRMSHPDTE;
                            orderLineItem.UpdateIfExists = 0;

                            lineItems[lineNumber] = orderLineItem;

                            orderLineNumber = orderLineNumber * 2;

                            lineNumber++;
                        }

                        // Instantiate a taUpdateCreateItemRcd XML node object
                        taPoHdr orderHeader = new taPoHdr();

                        //Populate elements of the taUpdateCreateItemRcd XML node object
                        orderHeader.POTYPE = order.POTYPE;
                        orderHeader.PONUMBER = order.PONUMBER;
                        orderHeader.VENDORID = order.VENDORID;
                        orderHeader.SUBTOTAL = order.SUBTOTAL;
                        orderHeader.UpdateIfExists = 0;

                        // Instantiate a IVItemMasterType schema object
                        POPTransactionType ordertype = new POPTransactionType();

                        // Populate the IVItemMasterType schema with the taUpdateCreateItemRcd XML node
                        ordertype.taPoLine_Items = lineItems;
                        ordertype.taPoHdr = orderHeader;
                        POPTransactionType[] orderEntry = { ordertype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the IVItemMasterType schema object
                        eConnect.POPTransactionType = orderEntry;

                        ///////////////////////////////////////////////////////////////////////////////

                        // Serialize the eConnectType.
                        serializer.Serialize(memoryStream, eConnect);

                        // Reset the position of the memory stream to the start.              
                        memoryStream.Position = 0;

                        // Create an XmlDocument from the serialized eConnectType in memory.
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(memoryStream);
                        memoryStream.Close();

                        /////////////////////////////////////////////////////////////////////////////////

                        //string xmldocument;

                        //SerializeReceiptHeaderObject("C:\\receipt.xml", receipt, receiptLines);

                        ////Use an XML document to create a string representation of the customer
                        //XmlDocument xmldoc = new XmlDocument();
                        //xmldoc.Load("C:\\receipt.xml");
                        //xmldocument = xmldoc.OuterXml;

                        ////Call eConnect to process the xmldocument.
                        //e.CreateEntity(_dynamicsConnection, xmldocument);

                        //////////////////////////////////////////////////////////////////////////////////

                        // Call eConnect to process the XmlDocument.
                        e.CreateEntity(_dynamicsConnection, xmlDocument.OuterXml);

                        operationResult.Success = true;
                        operationResult.Message = "Success";
                    }
                    else
                    {
                        operationResult.Success = false;
                        operationResult.Message = "No items are attached to receive.";
                    }
                }
                // The eConnectException class will catch eConnect business logic errors.
                // display the error message on the console
                catch (eConnectException exc)
                {
                    Console.Write(exc.ToString());
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error saving new receipt: {0} ", exc.ToString());
                }
                // Catch any system error that might occurr.
                // display the error message on the console
                catch (System.Exception ex)
                {
                    Console.Write(ex.ToString());
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error saving new receipt: {0} ", ex.ToString());
                }
                finally
                {
                    // Call the Dispose method to release the resources
                    // of the eConnectMethds object
                    e.Dispose();
                }
            } // end of using statement

            return operationResult;
        }

        /// <summary>
        /// update vendor order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderLines"></param>
        /// <returns></returns>
        public OperationResult UpdateVendorOrder(POP10100_PurchaseOrder_Work order, List<POP10110_PurchaseOrderLine_Work> orderLines)
        {
            var operationResult = new OperationResult();

            logger.Debug("Vendor Order is being created...");

            using (eConnectMethods e = new eConnectMethods())
            {
                try
                {
                    if (orderLines != null && orderLines.Count > 0)
                    {
                        taPoLine_ItemsTaPoLine[] lineItems = new taPoLine_ItemsTaPoLine[orderLines.Count];

                        var orderLineNumber = 16384;
                        var lineNumber = 0;

                        foreach (var orderLine in orderLines)
                        {
                            // Instantiate a taUpdateCreateItemRcd XML node object
                            taPoLine_ItemsTaPoLine orderLineItem = new taPoLine_ItemsTaPoLine();

                            //Populate elements of the taUpdateCreateItemRcd XML node object
                            orderLineItem.PONUMBER = orderLine.PONUMBER;
                            orderLineItem.VENDORID = orderLine.VENDORID;
                            orderLineItem.DOCDATE = orderLine.DOCDATE;
                            orderLineItem.LOCNCODE = orderLine.LOCNCODE;
                            orderLineItem.VNDITNUM = orderLine.VNDITNUM;
                            orderLineItem.ITEMNMBR = orderLine.ITEMNMBR;
                            orderLineItem.QUANTITY = orderLine.QUANTITY;
                            //DUE DATE
                            orderLineItem.PRMDATE = orderLine.PRMDATE;
                            //SHIP DATE
                            orderLineItem.PRMSHPDTE = orderLine.PRMSHPDTE;
                            orderLineItem.UpdateIfExists = 1;

                            lineItems[lineNumber] = orderLineItem;

                            orderLineNumber = orderLineNumber * 2;

                            lineNumber++;
                        }

                        // Instantiate a taUpdateCreateItemRcd XML node object
                        taPoHdr orderHeader = new taPoHdr();

                        //Populate elements of the taUpdateCreateItemRcd XML node object
                        orderHeader.POTYPE = order.POTYPE;
                        orderHeader.PONUMBER = order.PONUMBER;
                        orderHeader.VENDORID = order.VENDORID;
                        orderHeader.SUBTOTAL = order.SUBTOTAL;
                        orderHeader.UpdateIfExists = 1;

                        // Instantiate a IVItemMasterType schema object
                        POPTransactionType ordertype = new POPTransactionType();

                        // Populate the IVItemMasterType schema with the taUpdateCreateItemRcd XML node
                        ordertype.taPoLine_Items = lineItems;
                        ordertype.taPoHdr = orderHeader;
                        POPTransactionType[] orderEntry = { ordertype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the IVItemMasterType schema object
                        eConnect.POPTransactionType = orderEntry;

                        ///////////////////////////////////////////////////////////////////////////////

                        // Serialize the eConnectType.
                        serializer.Serialize(memoryStream, eConnect);

                        // Reset the position of the memory stream to the start.              
                        memoryStream.Position = 0;

                        // Create an XmlDocument from the serialized eConnectType in memory.
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(memoryStream);
                        memoryStream.Close();

                        /////////////////////////////////////////////////////////////////////////////////

                        //string xmldocument;

                        //SerializeReceiptHeaderObject("C:\\receipt.xml", receipt, receiptLines);

                        ////Use an XML document to create a string representation of the customer
                        //XmlDocument xmldoc = new XmlDocument();
                        //xmldoc.Load("C:\\receipt.xml");
                        //xmldocument = xmldoc.OuterXml;

                        ////Call eConnect to process the xmldocument.
                        //e.CreateEntity(_dynamicsConnection, xmldocument);

                        //////////////////////////////////////////////////////////////////////////////////

                        // Call eConnect to process the XmlDocument.
                        e.CreateEntity(_dynamicsConnection, xmlDocument.OuterXml);

                        operationResult.Success = true;
                        operationResult.Message = "Success";
                    }
                    else
                    {
                        operationResult.Success = false;
                        operationResult.Message = "No items are attached to receive.";
                    }
                }
                // The eConnectException class will catch eConnect business logic errors.
                // display the error message on the console
                catch (eConnectException exc)
                {
                    Console.Write(exc.ToString());
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error saving new receipt: {0} ", exc.ToString());
                }
                // Catch any system error that might occurr.
                // display the error message on the console
                catch (System.Exception ex)
                {
                    Console.Write(ex.ToString());
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error saving new receipt: {0} ", ex.ToString());
                }
                finally
                {
                    // Call the Dispose method to release the resources
                    // of the eConnectMethds object
                    e.Dispose();
                }
            } // end of using statement

            return operationResult;
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
