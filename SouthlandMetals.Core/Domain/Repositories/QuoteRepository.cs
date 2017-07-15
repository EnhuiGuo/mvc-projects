using SouthlandMetals.Common.Enum;
using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class QuoteRepository : IQuoteRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public QuoteRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all quotes
        /// </summary>
        /// <returns></returns>
        public List<Quote> GetQuotes()
        {
            var quotes = new List<Quote>();

            try
            {
                quotes = _db.Quote.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting quotes: {0} ", ex.ToString());
            }

            return quotes;
        }

        /// <summary>
        /// get quote by id
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public Quote GetQuote(Guid quoteId)
        {
            var quote = new Quote();

            try
            {
                quote = _db.Quote.FirstOrDefault(x => x.QuoteId == quoteId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting quote: {0} ", ex.ToString());
            }

            return quote;
        }

        /// <summary>
        /// get quote by project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Quote GetQuoteByProject(Guid projectId)
        {
            var quote = new Quote();

            try
            {
                quote = _db.Quote.FirstOrDefault(x => x.ProjectId == projectId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting quote by project: {0} ", ex.ToString());
            }

            return quote;
        }

        /// <summary>
        /// generate quote number 
        /// </summary>
        /// <returns></returns>
        public string QuoteNumber()
        {
            Enums.DocumentNumberType type = Enums.DocumentNumberType.Q;

            var quoteNumber = string.Empty;

            try
            {
                var newQuoteNumber = new QuoteNumber()
                {
                    Type = type.ToString(),
                    Number = null
                };

                var insertedQuoteNumber = _db.QuoteNumber.Add(newQuoteNumber);

                _db.SaveChanges();

                quoteNumber = insertedQuoteNumber.Type + String.Format("{0:D6}", insertedQuoteNumber.Value);

                var recentQuoteNumber = _db.QuoteNumber.FirstOrDefault(x => x.Value == insertedQuoteNumber.Value && x.Type == insertedQuoteNumber.Type);

                recentQuoteNumber.Number = quoteNumber;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error occurred generating quote number: {0} ", ex.ToString());
            }

            return quoteNumber;
        }

        /// <summary>
        /// save quote 
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public OperationResult SaveQuote(Quote quote)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingQuote = _db.Quote.FirstOrDefault(x => x.Number.ToLower() == quote.Number.ToLower());

                if (existingQuote == null)
                {
                    quote.QuoteDate = DateTime.Now;

                    var insertedQuote = _db.Quote.Add(quote);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Create quote success!";
                    operationResult.ReferenceId = insertedQuote.QuoteId;
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
                operationResult.Message = "can not create this quote!";
                logger.ErrorFormat("Error occurred saving new quote: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// update quote 
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public OperationResult UpdateQuote(Quote quote)
        {
            var operationResult = new OperationResult();

            var existingQuote = _db.Quote.Find(quote.QuoteId);

            if (existingQuote != null)
            {
                try
                {
                    _db.Entry(existingQuote).CurrentValues.SetValues(quote);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Update Success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "can not update this quote!";
                    logger.ErrorFormat("Error occurred updating quote: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find quote to update";
            }

            return operationResult;
        }

        /// <summary>
        /// remove quote number
        /// </summary>
        /// <param name="quoteNumber"></param>
        public void RemoveQuoteNumber(string quoteNumber)
        {
            try
            {
                var temp = _db.QuoteNumber.FirstOrDefault(x => x.Number.ToLower() == quoteNumber.ToLower());

                _db.QuoteNumber.Remove(temp);

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error occurred removing quote number: {0} ", ex.ToString());
            }
        }

        /// <summary>
        /// delete quote 
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public OperationResult DeleteQuote(Guid quoteId)
        {
            var operationResult = new OperationResult();

            var existingQuote = GetQuote(quoteId);

            if (existingQuote != null)
            {
                try
                {
                    _db.Quote.Attach(existingQuote);

                    _db.Quote.Remove(existingQuote);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Delete Success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "can not delete this quotation!";
                    logger.ErrorFormat("Error occurred deleting quote: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find quote to delete";
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
