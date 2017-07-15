using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class CreditMemoRepository : ICreditMemoRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public CreditMemoRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all credit memos
        /// </summary>
        /// <returns></returns>
        public List<CreditMemo> GetCreditMemos()
        {
            var creditMemos = new List<CreditMemo>();

            try
            {
                creditMemos = _db.CreditMemo.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting credit memos: {0} ", ex.ToString());
            }

            return creditMemos;
        }

        /// <summary>
        /// get all credit memo items
        /// </summary>
        /// <returns></returns>
        public List<CreditMemoItem> GetCreditMemoItems()
        {
            var creditMemoItems = new List<CreditMemoItem>();

            try
            {
                creditMemoItems = _db.CreditMemoItem.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting credit memo items: {0} ", ex.ToString());
            }

            return creditMemoItems;
        }

        /// <summary>
        /// get the items of a credit memo
        /// </summary>
        /// <param name="creditMemoId"></param>
        /// <returns></returns>
        public List<CreditMemoItem> GetCreditMemoItems(Guid creditMemoId)
        {
            var creditMemoItems = new List<CreditMemoItem>();

            try
            {
                creditMemoItems = _db.CreditMemoItem.Where(x => x.CreditMemoId == creditMemoId).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting credit memo items: {0} ", ex.ToString());
            }

            return creditMemoItems;
        }

        /// <summary>
        /// get credit memo by id 
        /// </summary>
        /// <param name="creditMemoId"></param>
        /// <returns></returns>
        public CreditMemo GetCreditMemo(Guid creditMemoId)
        {
            var creditMemo = new CreditMemo();
            try
            {
                creditMemo = _db.CreditMemo.FirstOrDefault(x => x.CreditMemoId == creditMemoId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting credit memo: {0} ", ex.ToString());
            }

            return creditMemo;
        }

        /// <summary>
        /// get credit memo by debit memo
        /// </summary>
        /// <param name="debitMemoId"></param>
        /// <returns></returns>
        public CreditMemo GetCreditMemoByDebitMemo(Guid debitMemoId)
        {
            var creditMemo = new CreditMemo();
            try
            {
                creditMemo = _db.CreditMemo.FirstOrDefault(x => x.DebitMemoId == debitMemoId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting credit memo: {0} ", ex.ToString());
            }

            return creditMemo;
        }

        /// <summary>
        /// get credit memo item by id
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public CreditMemoItem GetCreditMemoItem(Guid itemId)
        {
            var creditMemoItem = new CreditMemoItem();

            try
            {
                creditMemoItem = _db.CreditMemoItem.FirstOrDefault(x => x.CreditMemoItemId == itemId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting credit memo item: {0} ", ex.ToString());
            }

            return creditMemoItem;
        }

        /// <summary>
        /// save credit memo
        /// </summary>
        /// <param name="newCreditMemo"></param>
        /// <returns></returns>
        public OperationResult SaveCreditMemo(CreditMemo newCreditMemo)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingCreditMemo = _db.CreditMemo.FirstOrDefault(x => x.Number.ToLower() == newCreditMemo.Number.ToLower());

                if (existingCreditMemo == null)
                {
                    logger.Debug("CreditMemo is being created...");

                    var insertedCreditMemo = _db.CreditMemo.Add(newCreditMemo);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedCreditMemo.CreditMemoId;
                }
                else
                {
                    operationResult.Success = false;
                    operationResult.Message = "Duplicate Entry";
                }
            }
            catch (Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = "Error";
                logger.ErrorFormat("Error saving new creditMemo: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update credit memo
        /// </summary>
        /// <param name="creditMemo"></param>
        /// <returns></returns>
        public OperationResult UpdateCreditMemo(CreditMemo creditMemo)
        {
            var operationResult = new OperationResult();

            var existingCreditMemo = GetCreditMemo(creditMemo.CreditMemoId);

            if (existingCreditMemo != null)
            {
                logger.Debug("Credit memo is being updated.");

                try
                {
                    _db.CreditMemo.Attach(existingCreditMemo);

                    _db.Entry(existingCreditMemo).CurrentValues.SetValues(creditMemo);

                    _db.SaveChanges();

                    var existItems = _db.CreditMemoItem.Where(x => x.CreditMemoId == creditMemo.CreditMemoId).ToList();

                    if (creditMemo.CreditMemoItems != null && creditMemo.CreditMemoItems.Count > 0)
                    {
                        foreach (var creditMemoItem in creditMemo.CreditMemoItems)
                        {
                            var existItem = _db.CreditMemoItem.Find(creditMemoItem.CreditMemoItemId);
                            if (existItem != null)
                            {
                                _db.Entry(existItem).CurrentValues.SetValues(creditMemoItem);
                            }
                            else
                            {
                                creditMemoItem.CreditMemoId = creditMemo.CreditMemoId;
                                _db.CreditMemoItem.Add(creditMemoItem);
                            }
                        }
                    }

                    if (existItems != null && existItems.Count > 0)
                    {
                        foreach (var item in existItems)
                        {
                            var existItem = creditMemo.CreditMemoItems.FirstOrDefault(x => x.CreditMemoItemId == item.CreditMemoItemId);
                            if (existItem == null)
                                _db.CreditMemoItem.Remove(item);
                        }
                    }

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating creditMemo: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected creditMemo.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete credit memo
        /// </summary>
        /// <param name="creditMemo"></param>
        /// <returns></returns>
        public OperationResult DeleteCreditMemo(CreditMemo creditMemo)
        {
            var operationResult = new OperationResult();

            var existingCreditMemo = GetCreditMemo(creditMemo.CreditMemoId);

            if (existingCreditMemo != null)
            {
                try
                {
                    _db.CreditMemo.Attach(creditMemo);

                    _db.CreditMemo.Remove(creditMemo);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "delete this creditMemo success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Unable to update this creditMemo";
                    logger.ErrorFormat("Error while deleting creditMemo: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected creditMemo.";
            }

            return operationResult;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
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
