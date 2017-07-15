using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface IDebitMemoRepository : IDisposable
    {
        List<DebitMemo> GetDebitMemos();

        List<DebitMemoItem> GetDebitMemoItems();

        List<DebitMemoItem> GetDebitMemoItems(Guid debitMemoId);

        List<DebitMemoAttachment> GetDebitMemoAttachments();

        DebitMemo GetDebitMemo(Guid debitMemoId);

        DebitMemoAttachment GetDebitMemoAttachment(Guid attachmentId);

        DebitMemoItem GetDebitMemoItem(Guid itemId);

        string DebitMemoNumber();

        OperationResult SaveDebitMemo(DebitMemo newDebitMemo);

        OperationResult SaveDebitMemoAttachment(DebitMemoAttachment newDebitMemoAttachment);

        OperationResult UpdateDebitMemo(DebitMemo debitMemo);

        void RemoveDebitMemoNumber(string debitMemoNumber);

        OperationResult DeleteDebitMemo(Guid debitMemoId);

        OperationResult DeleteDebitMemoAttachment(Guid attachmentId);
    }
}
