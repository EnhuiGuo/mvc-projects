using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface IQuoteRepository : IDisposable
    {
        List<Quote> GetQuotes();

        Quote GetQuote(Guid quoteId);

        Quote GetQuoteByProject(Guid projectId);

        OperationResult SaveQuote(Quote quote);

        OperationResult DeleteQuote(Guid quoteId);

        OperationResult UpdateQuote(Quote quote);

        string QuoteNumber();

        void RemoveQuoteNumber(string quoteNumber);
    }
}
