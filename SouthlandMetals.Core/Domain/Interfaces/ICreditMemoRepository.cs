using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface ICreditMemoRepository : IDisposable
    {
        List<CreditMemo> GetCreditMemos();

        List<CreditMemoItem> GetCreditMemoItems();

        List<CreditMemoItem> GetCreditMemoItems(Guid creditMemoId);

        CreditMemo GetCreditMemo(Guid creditMemoId);

        CreditMemo GetCreditMemoByDebitMemo(Guid debitMemoId);

        CreditMemoItem GetCreditMemoItem(Guid itemId);

        OperationResult SaveCreditMemo(CreditMemo newCreditMemo);

        OperationResult UpdateCreditMemo(CreditMemo creditMemo);

        OperationResult DeleteCreditMemo(CreditMemo creditMemo);
    }
}
