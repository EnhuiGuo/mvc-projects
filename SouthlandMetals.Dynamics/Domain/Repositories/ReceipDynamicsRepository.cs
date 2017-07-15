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
using System.Xml;
using System.Xml.Serialization;

namespace SouthlandMetals.Dynamics.Domain.Repositories
{
    public class ReceiptDynamicsRepository : IReceiptDynamicsRepository, IDisposable
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public ReceiptDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// get receipt header by receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public POP30300_Receipt_History GetReceiptHeader(string receiptNumber)
        {
            var receiptHeader = new POP30300_Receipt_History();

            try
            {
                receiptHeader = _dynamicsContext.POP30300_Receipt_History.FirstOrDefault(x => x.POPRCTNM.Replace(" ", string.Empty).ToLower() == receiptNumber.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting receipt header: {0} ", ex.ToString());
            }

            return receiptHeader;
        }

        /// <summary>
        /// get all receipt lines
        /// </summary>
        /// <returns></returns>
        public List<POP30310_ReceiptLine_History> GetReceiptLines()
        {
            var receiptLines = new List<POP30310_ReceiptLine_History>();

            try
            {
                receiptLines = _dynamicsContext.POP30310_ReceiptLine_History.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting receipts lines: {0} ", ex.ToString());
            }

            return receiptLines;
        }

        /// <summary>
        /// save new receipt
        /// </summary>
        /// <param name="receipt"></param>
        /// <param name="receiptLines"></param>
        /// <returns></returns>
        public OperationResult SaveReceipt(POP10300_Receipt_Work receipt, List<POP10310> receiptLines)
        {
            var operationResult = new OperationResult();

            logger.Debug("Receipt is being created...");

            using (eConnectMethods e = new eConnectMethods())
            {
                try
                {
                    if (receiptLines != null && receiptLines.Count > 0)
                    {
                        taPopRcptLotInsert_ItemsTaPopRcptLotInsert[] lotItems = new taPopRcptLotInsert_ItemsTaPopRcptLotInsert[receiptLines.Count];

                        taPopRcptLineInsert_ItemsTaPopRcptLineInsert[] lineItems = new taPopRcptLineInsert_ItemsTaPopRcptLineInsert[receiptLines.Count];

                        var receiptLineNumber = 16384;
                        var lineNumber = 0;

                        foreach (var receiptLine in receiptLines)
                        {
                            // Instantiate a taUpdateCreateItemRcd XML node object
                            taPopRcptLotInsert_ItemsTaPopRcptLotInsert receiptLotLine = new taPopRcptLotInsert_ItemsTaPopRcptLotInsert();

                            receiptLotLine.POPRCTNM = receiptLine.POPRCTNM;
                            receiptLotLine.RCPTLNNM = receiptLineNumber;
                            receiptLotLine.ITEMNMBR = receiptLine.ITEMNMBR;
                            receiptLotLine.SERLTNUM = receiptLine.SERLTNUM;
                            receiptLotLine.SERLTQTY = receiptLine.QTYSHPPD;
                            receiptLotLine.CREATEBIN = 0;

                            lotItems[lineNumber] = receiptLotLine;

                            // Instantiate a taUpdateCreateItemRcd XML node object
                            taPopRcptLineInsert_ItemsTaPopRcptLineInsert receiptLineItem = new taPopRcptLineInsert_ItemsTaPopRcptLineInsert();

                            //Populate elements of the taUpdateCreateItemRcd XML node object
                            receiptLineItem.POPTYPE = receiptLine.POPTYPE;
                            receiptLineItem.POPRCTNM = receiptLine.POPRCTNM;
                            receiptLineItem.RCPTLNNM = receiptLineNumber;
                            receiptLineItem.ITEMNMBR = receiptLine.ITEMNMBR;
                            receiptLineItem.VENDORID = receiptLine.VENDORID;
                            receiptLineItem.PONUMBER = receiptLine.PONUMBER;
                            receiptLineItem.VNDITNUM = receiptLine.VNDITNUM;
                            receiptLineItem.QTYSHPPD = receiptLine.QTYSHPPD;
                            receiptLineItem.AUTOCOST = 1;

                            lineItems[lineNumber] = receiptLineItem;

                            receiptLineNumber = receiptLineNumber * 2;

                            lineNumber++;
                        }

                        // Instantiate a taUpdateCreateItemRcd XML node object
                        taPopRcptHdrInsert receiptHeader = new taPopRcptHdrInsert();

                        //Populate elements of the taUpdateCreateItemRcd XML node object
                        receiptHeader.POPRCTNM = receipt.POPRCTNM;
                        receiptHeader.POPTYPE = receipt.POPTYPE;
                        receiptHeader.receiptdate = receipt.receiptdate;
                        receiptHeader.BACHNUMB = receipt.BACHNUMB;
                        receiptHeader.VENDORID = receipt.VENDORID;

                        // Instantiate a IVItemMasterType schema object
                        POPReceivingsType receipttype = new POPReceivingsType();

                        // Populate the IVItemMasterType schema with the taUpdateCreateItemRcd XML node
                        receipttype.taPopRcptLotInsert_Items = lotItems;
                        receipttype.taPopRcptLineInsert_Items = lineItems;
                        receipttype.taPopRcptHdrInsert = receiptHeader;
                        POPReceivingsType[] receiptEntry = { receipttype };

                        // Instantiate an eConnectType schema object
                        eConnectType eConnect = new eConnectType();

                        // Instantiate a Memory Stream object
                        MemoryStream memoryStream = new MemoryStream();

                        // Create an XML serializer object
                        XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                        // Populate the eConnectType object with the IVItemMasterType schema object
                        eConnect.POPReceivingsType = receiptEntry;

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
        /// serialize recipt
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="receipt"></param>
        /// <param name="receiptLines"></param>
        private static void SerializeReceiptObject(string filename, POP10300_Receipt_Work receipt, List<POP10310> receiptLines)
        {
            try
            {
                // Instantiate an eConnectType schema object
                eConnectType eConnect = new eConnectType();

                taPopRcptLineInsert_ItemsTaPopRcptLineInsert[] lineItems = new taPopRcptLineInsert_ItemsTaPopRcptLineInsert[receiptLines.Count];

                taPopRcptLotInsert_ItemsTaPopRcptLotInsert[] lotItems = new taPopRcptLotInsert_ItemsTaPopRcptLotInsert[receiptLines.Count];

                var receiptLineNumber = 16384;
                var lineNumber = 0;

                foreach (var receiptLine in receiptLines)
                {
                    // Instantiate a taUpdateCreateItemRcd XML node object
                    taPopRcptLotInsert_ItemsTaPopRcptLotInsert receiptLotLine = new taPopRcptLotInsert_ItemsTaPopRcptLotInsert();

                    receiptLotLine.POPRCTNM = receiptLine.POPRCTNM;
                    receiptLotLine.RCPTLNNM = receiptLineNumber;
                    receiptLotLine.ITEMNMBR = receiptLine.ITEMNMBR;
                    receiptLotLine.SERLTNUM = receiptLine.SERLTNUM;
                    receiptLotLine.SERLTQTY = receiptLine.QTYSHPPD;
                    receiptLotLine.CREATEBIN = 0;

                    lotItems[lineNumber] = receiptLotLine;

                    // Instantiate a taUpdateCreateItemRcd XML node object
                    taPopRcptLineInsert_ItemsTaPopRcptLineInsert receiptLineItem = new taPopRcptLineInsert_ItemsTaPopRcptLineInsert();

                    //Populate elements of the taUpdateCreateItemRcd XML node object
                    receiptLineItem.POPTYPE = receiptLine.POPTYPE;
                    receiptLineItem.POPRCTNM = receiptLine.POPRCTNM;
                    receiptLineItem.RCPTLNNM = receiptLineNumber;
                    receiptLineItem.ITEMNMBR = receiptLine.ITEMNMBR;
                    receiptLineItem.VENDORID = receiptLine.VENDORID;
                    receiptLineItem.PONUMBER = receiptLine.PONUMBER;
                    receiptLineItem.VNDITNUM = receiptLine.VNDITNUM;
                    receiptLineItem.QTYSHPPD = receiptLine.QTYSHPPD;
                    receiptLineItem.AUTOCOST = 1;

                    lineItems[lineNumber] = receiptLineItem;

                    receiptLineNumber = receiptLineNumber * 2;

                    lineNumber++;
                }

                // Instantiate a taUpdateCreateItemRcd XML node object
                taPopRcptHdrInsert receiptHeader = new taPopRcptHdrInsert();

                //Populate elements of the taUpdateCreateItemRcd XML node object
                receiptHeader.POPRCTNM = receipt.POPRCTNM;
                receiptHeader.POPTYPE = receipt.POPTYPE;
                receiptHeader.receiptdate = receipt.receiptdate;
                receiptHeader.BACHNUMB = receipt.BACHNUMB;
                receiptHeader.VENDORID = receipt.VENDORID;

                // Instantiate a IVItemMasterType schema object
                POPReceivingsType receipttype = new POPReceivingsType();

                // Populate the IVItemMasterType schema with the taUpdateCreateItemRcd XML node
                receipttype.taPopRcptLotInsert_Items = lotItems;
                receipttype.taPopRcptLineInsert_Items = lineItems;
                receipttype.taPopRcptHdrInsert = receiptHeader;
                POPReceivingsType[] receiptEntry = { receipttype };

                // Instantiate a Memory Stream object
                MemoryStream memoryStream = new MemoryStream();

                // Create an XML serializer object
                XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                // Populate the eConnectType object with the IVItemMasterType schema object
                eConnect.POPReceivingsType = receiptEntry;

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
