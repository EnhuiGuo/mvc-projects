using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Models;
using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SouthlandMetals.Dynamics.Domain.Repositories
{
    public class ReceivablesDynamicsRepository : IReceivablesDynamicsRepository, IDisposable
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicsContext _dynamicsContext;
        private string _dynamicsConnection = ConfigurationManager.AppSettings["dynamicsConnection"];
        private bool disposed = false;

        public ReceivablesDynamicsRepository()
        {
            _dynamicsContext = new DynamicsContext(_dynamicsConnection);
        }

        /// <summary>
        /// save new receivable transaction
        /// </summary>
        /// <param name="receivable"></param>
        /// <returns></returns>
        public OperationResult SaveReceivableTransaction(RM20101 receivable)
        {
            var operationResult = new OperationResult();

            logger.Debug("Receivable is being created...");

            using (eConnectMethods e = new eConnectMethods())
            {
                try
                {
                    // Instantiate a taPMTransactionInsert XML node object
                    taRMTransaction transaction = new taRMTransaction();

                    //Populate elements of the taUpdateCreateItemRcd XML node object
                    transaction.RMDTYPAL = receivable.RMDTYPAL;
                    transaction.DOCNUMBR = receivable.DOCNUMBR;
                    transaction.DOCDATE = receivable.DOCDATE.ToShortDateString();
                    transaction.BACHNUMB = receivable.BACHNUMB;
                    transaction.CUSTNMBR = receivable.CUSTNMBR;
                    transaction.DOCAMNT = receivable.DOCAMNT;
                    transaction.SLSAMNT = receivable.SLSAMNT;

                    // Instantiate a PMTransactionType schema object
                    RMTransactionType transactiontype = new RMTransactionType();

                    // Populate the PMTransactionType schema with the taPMTransactionInsert XML node
                    transactiontype.taRMTransaction = transaction;
                    RMTransactionType[] receivableTransaction = { transactiontype };

                    // Instantiate an eConnectType schema object
                    eConnectType eConnect = new eConnectType();

                    // Instantiate a Memory Stream object
                    MemoryStream memoryStream = new MemoryStream();

                    // Create an XML serializer object
                    XmlSerializer serializer = new XmlSerializer(eConnect.GetType());

                    // Populate the eConnectType object with the PMTransactionType schema object
                    eConnect.RMTransactionType = receivableTransaction;

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
