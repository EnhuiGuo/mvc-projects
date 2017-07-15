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
    public class InventoryDynamicsRepository : IIventoryDynamicsRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public InventoryDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// save new inventory transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transactionLines"></param>
        /// <returns></returns>
        public OperationResult SaveInventoryTransaction(IV10000_IventoryTransaction_Work transaction, List<IV10000_IventoryTransaction_Work> transactionLines)
        {
            var operationResult = new OperationResult();

            logger.Debug("Receipt is being created...");

            using (eConnectMethods e = new eConnectMethods())
            {
                try
                {
                    if (transactionLines != null && transactionLines.Count > 0)
                    {
                        taIVTransactionLotInsert_ItemsTaIVTransactionLotInsert[] lotItems = new taIVTransactionLotInsert_ItemsTaIVTransactionLotInsert[transactionLines.Count];

                        taIVTransactionLineInsert_ItemsTaIVTransactionLineInsert[] lineItems = new taIVTransactionLineInsert_ItemsTaIVTransactionLineInsert[transactionLines.Count];

                        var transactionLineNumber = 16384;
                        var lineNumber = 0;

                        foreach (var transactionLine in transactionLines)
                        {
                            //// Instantiate a taUpdateCreateItemRcd XML node object
                            taIVTransactionLotInsert_ItemsTaIVTransactionLotInsert transactionLotItem = new taIVTransactionLotInsert_ItemsTaIVTransactionLotInsert();

                            //Populate elements of the taUpdateCreateItemRcd XML node object
                            transactionLotItem.IVDOCNBR = transactionLine.IVDOCNBR;
                            transactionLotItem.IVDOCTYP = transactionLine.IVDOCTYP;
                            transactionLotItem.LOTNUMBR = transactionLine.LOTNUMBR;
                            transactionLotItem.ITEMNMBR = transactionLine.ITEMNMBR;
                            transactionLotItem.SERLTQTY = transactionLine.TRXQTY;
                            transactionLotItem.LOCNCODE = transactionLine.TRXLOCTN;
                            //transactionLotItem.EXPNDATE = transactionLine.EXPNDATE;
                            //transactionLotItem.DATERECD = transactionLine.DATERECD;

                            lotItems[lineNumber] = transactionLotItem;

                            // Instantiate a taUpdateCreateItemRcd XML node object
                            taIVTransactionLineInsert_ItemsTaIVTransactionLineInsert transactionLineItem = new taIVTransactionLineInsert_ItemsTaIVTransactionLineInsert();

                            //Populate elements of the taUpdateCreateItemRcd XML node object
                            transactionLineItem.IVDOCNBR = transactionLine.IVDOCNBR;
                            transactionLineItem.IVDOCTYP = transactionLine.IVDOCTYP;
                            //transactionLineItem.LNSEQNBR = transactionLine.LNSEQNBR;
                            transactionLineItem.ITEMNMBR = transactionLine.ITEMNMBR;
                            transactionLineItem.TRXLOCTN = transactionLine.TRXLOCTN;
                            transactionLineItem.TRXQTY = transactionLine.TRXQTY;
                            //transactionLineItem.Reason_Code = transactionLine.Reason_Code;

                            lineItems[lineNumber] = transactionLineItem;

                            transactionLineNumber = transactionLineNumber * 2;

                            lineNumber++;
                        }

                        // Instantiate a taUpdateCreateItemRcd XML node object
                        taIVTransactionHeaderInsert transactionHeader = new taIVTransactionHeaderInsert();

                        //Populate elements of the taUpdateCreateItemRcd XML node object
                        transactionHeader.BACHNUMB = transaction.BACHNUMB;
                        transactionHeader.IVDOCNBR = transaction.IVDOCNBR;
                        transactionHeader.IVDOCTYP = transaction.IVDOCTYP;
                        transactionHeader.DOCDATE = transaction.DOCDATE;

                        // Instantiate a IVItemMasterType schema object
                        IVInventoryTransactionType transactiontype = new IVInventoryTransactionType();

                        // Populate the IVItemMasterType schema with the taUpdateCreateItemRcd XML node
                        transactiontype.taIVTransactionLotInsert_Items = lotItems;
                        transactiontype.taIVTransactionLineInsert_Items = lineItems;
                        transactiontype.taIVTransactionHeaderInsert = transactionHeader;
                        IVInventoryTransactionType[] transactionEntry = { transactiontype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the IVItemMasterType schema object
                        eConnect.IVInventoryTransactionType = transactionEntry;

                        ///////////////////////////////////////////////////////////////////////////////

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

                        /////////////////////////////////////////////////////////////////////////////////

                        //string xmldocument;

                        //SerializeTransactionObject("C:\\inventoryTransaction.xml", transaction, transactionLines);

                        ////Use an XML document to create a string representation of the customer
                        //XmlDocument xmldoc = new XmlDocument();
                        //xmldoc.Load("C:\\inventoryTransaction.xml");
                        //xmldocument = xmldoc.OuterXml;

                        ////Call eConnect to process the xmldocument.
                        //e.CreateEntity(_dynamicsConnection, xmldocument);

                        //////////////////////////////////////////////////////////////////////////////////

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
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error saving new receipt: {0} ", exc.ToString());
                }
                // Catch any system error that might occurr.
                // display the error message on the console
                catch (System.Exception ex)
                {
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
