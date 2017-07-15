using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Models;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SouthlandMetals.Dynamics.Domain.Repositories
{
    public class PayablesDynamicsRepository : IPayablesDynamicsRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public PayablesDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// save new payable transaction 
        /// </summary>
        /// <param name="payable"></param>
        /// <returns></returns>
        public OperationResult SavePayableTransaction(PM10000_Payables_Work payable)
        {
            var operationResult = new OperationResult();

            logger.Debug("Payable is being created...");

            using (eConnectMethods e = new eConnectMethods())
            {
                try
                {
                    // Instantiate a taPMTransactionInsert XML node object
                    taPMTransactionInsert transaction = new taPMTransactionInsert();

                    //Populate elements of the taUpdateCreateItemRcd XML node object
                    transaction.BACHNUMB = payable.BACHNUMB;
                    transaction.VCHNUMWK = payable.VCHNUMWK;
                    transaction.VENDORID = payable.VENDORID;
                    transaction.DOCNUMBR = payable.DOCNUMBR;
                    transaction.DOCTYPE = payable.DOCTYPE;
                    transaction.DOCAMNT = payable.DOCAMNT;
                    transaction.DOCDATE = payable.DOCDATE.ToShortDateString();
                    transaction.MSCCHAMT = payable.MSCCHAMT;
                    transaction.PRCHAMNT = payable.PRCHAMNT;
                    transaction.CHRGAMNT = payable.CHRGAMNT;
                    transaction.TAXAMNT = payable.TAXAMNT;
                    transaction.FRTAMNT = payable.FRTAMNT;
                    transaction.TRDISAMT = payable.TRDISAMT;
                    transaction.CASHAMNT = payable.CASHAMNT;
                    transaction.CHEKAMNT = payable.CHEKAMNT;
                    transaction.CRCRDAMT = payable.CRCRDAMT;
                    transaction.DISTKNAM = payable.DISTKNAM;

                    // Instantiate a PMTransactionType schema object
                    PMTransactionType transactiontype = new PMTransactionType();

                    // Populate the PMTransactionType schema with the taPMTransactionInsert XML node
                    transactiontype.taPMTransactionInsert = transaction;
                    PMTransactionType[] payableTransaction = { transactiontype };

                    // Instantiate an eConnectType schema object
                    eConnectType eConnect = new eConnectType();

                    // Instantiate a Memory Stream object
                    MemoryStream memoryStream = new MemoryStream();

                    // Create an XML serializer object
                    XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                    // Populate the eConnectType object with the PMTransactionType schema object
                    eConnect.PMTransactionType = payableTransaction;

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
                    logger.ErrorFormat("Error saving new payables transaction: {0} ", exc.ToString());
                }
                // Catch any system error that might occurr.
                // display the error message on the console
                catch (System.Exception ex)
                {
                    Console.Write(ex.ToString());
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error saving new payables transaction: {0} ", ex.ToString());
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
        /// serialize payables object
        /// </summary>
        /// <param name="filename"></param>
        public static void SerializePayablesObject(string filename)
        {
            try
            {
                // Instantiate an eConnectType schema object
                eConnectType eConnect = new eConnectType();

                // Instantiate a PMTransactionType  schema object
                PMTransactionType transactionType = new PMTransactionType();

                // Instantiate a taPMTransactionInsert XML node object
                taPMTransactionInsert transaction = new taPMTransactionInsert();

                // Create an XML serializer object
                XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                // Populate elements of the taPMTransactionInsert XML node object
                //transaction.CUSTNMBR = "Customer001";
                //transaction.CUSTNAME = "Customer 1";
                //transaction.ADDRESS1 = "2002 60th St SW";
                //transaction.ADRSCODE = "Primary";
                //transaction.CITY = "NewCity";
                //transaction.ZIPCODE = "52302";

                // Populate the PMTransactionType schema with the taPMTransactionInsert XML node
                transactionType.taPMTransactionInsert = transaction;
                PMTransactionType[] payablesTransaction = { transactionType };

                // Populate the eConnectType object with the PMTransactionType schema object
                eConnect.PMTransactionType = payablesTransaction;

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
