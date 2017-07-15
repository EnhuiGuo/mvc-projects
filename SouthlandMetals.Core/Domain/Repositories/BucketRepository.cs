using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class BucketRepository : IBucketRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public BucketRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable buckets
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableBuckets()
        {
            var buckets = new List<SelectListItem>();

            try
            {
                buckets = _db.Bucket.Select(y => new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.BucketId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting buckets: {0} ", ex.ToString());
            }

            return buckets;
        }

        /// <summary>
        /// get all buckets
        /// </summary>
        /// <returns></returns>
        public List<Bucket> GetBuckets()
        {
            var buckets = new List<Bucket>();

            try
            {
                buckets = _db.Bucket.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting buckets: {0} ", ex.ToString());
            }

            return buckets;
        }

        /// <summary>
        /// get bucket by id
        /// </summary>
        /// <param name="bucketId"></param>
        /// <returns></returns>
        public Bucket GetBucket(Guid bucketId)
        {
            var bucket = new Bucket();

            try
            {
                bucket = _db.Bucket.FirstOrDefault(x => x.BucketId == bucketId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting bucket: {0} ", ex.ToString());
            }

            return bucket;
        }

        /// <summary>
        /// get bucket by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bucket GetBucket(string name)
        {
            var bucket = new Bucket();

            try
            {
                bucket = _db.Bucket.FirstOrDefault(x => x.Name.Replace(" ", string.Empty).ToLower() == name.Replace(" ", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting bucket: {0} ", ex.ToString());
            }

            return bucket;
        }

        /// <summary>
        /// save new bucket
        /// </summary>
        /// <param name="newBucket"></param>
        /// <returns></returns>
        public OperationResult SaveBucket(Bucket newBucket)
        {
            var operationResult = new OperationResult();

            try
            {
                var existingBucket = _db.Bucket.FirstOrDefault(x => x.Name.ToLower() == newBucket.Name.ToLower());

                if (existingBucket == null)
                {
                    logger.Debug("Bucket is being created...");

                    _db.Bucket.Add(newBucket);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
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
                logger.ErrorFormat("Error saving new bucket: {0} ", ex.ToString());
            }

            return operationResult;
        }

        /// <summary>
        /// delete bucket
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        public OperationResult DeleteBucket(Bucket bucket)
        {
            var operationResult = new OperationResult();

            var existingBucket = GetBucket(bucket.BucketId);

            if(existingBucket != null)
            {
                try
                {
                    _db.Bucket.Attach(bucket);

                    _db.Bucket.Remove(bucket);

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Delete this bucket success!";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Can not Delete this bucket";
                    logger.ErrorFormat("Error deleting bucket: {0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find bucket to delete.";
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
