using SouthlandMetals.Common.Enum;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class DebitMemoRepository : IDebitMemoRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public DebitMemoRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all debit memos
        /// </summary>
        /// <returns></returns>
        public List<DebitMemo> GetDebitMemos()
        {
            var debitMemos = new List<DebitMemo>();

            try
            {
                debitMemos = _db.DebitMemo.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting debit memos: {0} ", ex.ToString());
            }

            return debitMemos;
        }

        /// <summary>
        /// get all debit memo items
        /// </summary>
        /// <returns></returns>
        public List<DebitMemoItem> GetDebitMemoItems()
        {
            var debitMemoItems = new List<DebitMemoItem>();

            try
            {
                debitMemoItems = _db.DebitMemoItem.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting debit memo items: {0} ", ex.ToString());
            }

            return debitMemoItems;
        }

        /// <summary>
        /// get the items of a debit memo
        /// </summary>
        /// <param name="debitMemoId"></param>
        /// <returns></returns>
        public List<DebitMemoItem> GetDebitMemoItems(Guid debitMemoId)
        {
            var debitMemoItems = new List<DebitMemoItem>();

            try
            {
                debitMemoItems = _db.DebitMemoItem.Where(x => x.DebitMemoId == debitMemoId).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting debit memo items: {0} ", ex.ToString());
            }

            return debitMemoItems;
        }

        /// <summary>
        /// get all debit memo attachments
        /// </summary>
        /// <returns></returns>
        public List<DebitMemoAttachment> GetDebitMemoAttachments()
        {
            var debitMemoAttachments = new List<DebitMemoAttachment>();

            try
            {
                debitMemoAttachments = _db.DebitMemoAttachment.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting debit memo attachments: {0} ", ex.ToString());
            }

            return debitMemoAttachments;
        }

        /// <summary>
        /// get debit memo by id
        /// </summary>
        /// <param name="debitMemoId"></param>
        /// <returns></returns>
        public DebitMemo GetDebitMemo(Guid debitMemoId)
        {
            var debitMemo = new DebitMemo();
            try
            {
                debitMemo = _db.DebitMemo.FirstOrDefault(x => x.DebitMemoId == debitMemoId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting debit memo: {0} ", ex.ToString());
            }

            return debitMemo;
        }

        /// <summary>
        /// get debit memo attachment by id
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        public DebitMemoAttachment GetDebitMemoAttachment(Guid attachmentId)
        {
            var debitMemoAttachment = new DebitMemoAttachment();

            try
            {
                debitMemoAttachment = _db.DebitMemoAttachment.FirstOrDefault(x => x.DebitMemoAttachmentId == attachmentId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting debit memo attachment: {0} ", ex.ToString());
            }

            return debitMemoAttachment;
        }

        /// <summary>
        /// get debit memo item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public DebitMemoItem GetDebitMemoItem(Guid itemId)
        {
            var debitMemoItem = new DebitMemoItem();

            try
            {
                debitMemoItem = _db.DebitMemoItem.FirstOrDefault(x => x.DebitMemoItemId == itemId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting debit memo item: {0} ", ex.ToString());
            }

            return debitMemoItem;
        }

        /// <summary>
        /// generate new debit memo number
        /// </summary>
        /// <returns></returns>
        public string DebitMemoNumber()
        {
            Enums.DocumentNumberType type = Enums.DocumentNumberType.DM1;

            var debitMemoNumber = string.Empty;

            try
            {
                var newDebitMemoNumber = new DebitMemoNumber()
                {
                    Type = type.ToString(),
                    Number = null
                };

                var insertedDebitMemoNumber = _db.DebitMemoNumber.Add(newDebitMemoNumber);

                _db.SaveChanges();

                debitMemoNumber = insertedDebitMemoNumber.Type + String.Format("{0:D6}", insertedDebitMemoNumber.Value);

                var recentDebitMemoNumber = _db.DebitMemoNumber.FirstOrDefault(x => x.Value == insertedDebitMemoNumber.Value && x.Type == insertedDebitMemoNumber.Type);

                recentDebitMemoNumber.Number = debitMemoNumber;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error occurred generating new debit memo number: {0} ", ex.ToString());
            }

            return debitMemoNumber;
        }

        /// <summary>
        /// save new debit memo 
        /// </summary>
        /// <param name="newDebitMemo"></param>
        /// <returns></returns>
        public OperationResult SaveDebitMemo(DebitMemo newDebitMemo)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingDebitMemo = _db.DebitMemo.FirstOrDefault(x => x.Number.ToLower() == newDebitMemo.Number.ToLower());

                if (existingDebitMemo == null)
                {
                    logger.Debug("DebitMemo is being created...");

                    var insertedDebitMemo = _db.DebitMemo.Add(newDebitMemo);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedDebitMemo.DebitMemoId;
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
                logger.ErrorFormat("Error saving new DebitMemo: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// save debit memo attachment 
        /// </summary>
        /// <param name="newDebitMemoAttachment"></param>
        /// <returns></returns>
        public OperationResult SaveDebitMemoAttachment(DebitMemoAttachment newDebitMemoAttachment)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingDebitMemoAttachment = _db.DebitMemoAttachment.FirstOrDefault(x => x.Name.ToLower() == newDebitMemoAttachment.Name.ToLower() && x.DebitMemoId == newDebitMemoAttachment.DebitMemoId);

                if (existingDebitMemoAttachment == null)
                {
                    logger.Debug("DebitMemoAttachment is being created...");

                    var insertedDebitMemoAttachment = _db.DebitMemoAttachment.Add(newDebitMemoAttachment);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                    operationResult.ReferenceId = insertedDebitMemoAttachment.DebitMemoAttachmentId;
                    operationResult.Name = insertedDebitMemoAttachment.Name;
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
                logger.ErrorFormat("Error saving new DebitMemoAttachment: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update debit memo
        /// </summary>
        /// <param name="debitMemo"></param>
        /// <returns></returns>
        public OperationResult UpdateDebitMemo(DebitMemo debitMemo)
        {
            var operationResult = new OperationResult();

            var existingDebitMemo = GetDebitMemo(debitMemo.DebitMemoId);

            if (existingDebitMemo != null)
            {
                logger.Debug("Debit memo is being updated.");

                try
                {
                    _db.DebitMemo.Attach(existingDebitMemo);

                    _db.Entry(existingDebitMemo).CurrentValues.SetValues(debitMemo);

                    _db.SaveChanges();

                    var existItems = _db.DebitMemoItem.Where(x => x.DebitMemoId == debitMemo.DebitMemoId).ToList();

                    if (debitMemo.DebitMemoItems != null && debitMemo.DebitMemoItems.Count > 0)
                    {
                        foreach (var debitMemoItem in debitMemo.DebitMemoItems)
                        {
                            var existItem = _db.DebitMemoItem.Find(debitMemoItem.DebitMemoItemId);
                            if (existItem != null)
                            {
                                _db.Entry(existItem).CurrentValues.SetValues(debitMemoItem);
                            }
                            else
                            {
                                _db.DebitMemoItem.Add(debitMemoItem);
                            }
                        }
                    }

                    if (existItems != null && existItems.Count > 0)
                    {
                        foreach (var item in existItems)
                        {
                            var existItem = debitMemo.DebitMemoItems.FirstOrDefault(x => x.DebitMemoItemId == item.DebitMemoItemId);
                            if (existItem == null)
                                _db.DebitMemoItem.Remove(item);
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
                    logger.ErrorFormat("Error while updating debitMemo: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected debitMemo.";
            }

            return operationResult;
        }

        /// <summary>
        /// remove debit memo number 
        /// </summary>
        /// <param name="debitMemoNumber"></param>
        public void RemoveDebitMemoNumber(string debitMemoNumber)
        {
            try
            {
                var temp = _db.DebitMemoNumber.FirstOrDefault(x => x.Number.Replace(" ", string.Empty).ToLower() == debitMemoNumber.Replace(" ", string.Empty).ToLower());

                _db.DebitMemoNumber.Remove(temp);

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error occurred removing debit memo number: {0} ", ex.ToString());
            }
        }

        /// <summary>
        /// delete debit memo 
        /// </summary>
        /// <param name="debitMemoId"></param>
        /// <returns></returns>
        public OperationResult DeleteDebitMemo(Guid debitMemoId)
        {
            var operationResult = new OperationResult();

            var existingDebitMemo = GetDebitMemo(debitMemoId);

            if (existingDebitMemo != null)
            {
                try
                {
                    _db.DebitMemo.Attach(existingDebitMemo);

                    _db.DebitMemo.Remove(existingDebitMemo);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "delete this debitMemo success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Can not update this debitMemo";
                    logger.ErrorFormat("Error while deleting debitMemo: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected debitMemo.";
            }

            return operationResult;
        }

        /// <summary>
        /// delete debit memo attachment
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        public OperationResult DeleteDebitMemoAttachment(Guid attachmentId)
        {
            var operationResult = new OperationResult();

            var existingDebitMemoAttachment = GetDebitMemoAttachment(attachmentId);

            if (existingDebitMemoAttachment != null)
            {
                try
                {
                    _db.DebitMemoAttachment.Attach(existingDebitMemoAttachment);

                    _db.DebitMemoAttachment.Remove(existingDebitMemoAttachment);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "delete this debitMemoAttachment success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Can not update this debitMemoAttachment";
                    logger.ErrorFormat("Error while deleting debitMemo attachment: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find debit memo attachment to delete.";
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
