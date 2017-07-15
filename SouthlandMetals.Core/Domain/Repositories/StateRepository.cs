using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class StateRepository : IStateRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public StateRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get selectable states
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableStates()
        {
            var states = new List<SelectListItem>();

            try
            {
                states = _db.State.Select(y => new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.StateId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting states: {0} ", ex.ToString());
            }

            return states;
        }

        /// <summary>
        /// get state by id 
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public State GetState(Guid? stateId)
        {
            var state = new State();

            try
            {
                state = _db.State.FirstOrDefault(x => x.StateId == stateId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting state: {0} ", ex.ToString());
            }

            return state;
        }

        /// <summary>
        /// get state by name
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public State GetState(string stateName)
        {
            var state = new State();

            try
            {
                if (stateName == "mg")
                {
                    state = _db.State.FirstOrDefault(x => x.Abbreviation == "MG");
                }
                else
                {
                    state = _db.State.FirstOrDefault(x => x.Abbreviation.Replace(" ", string.Empty).ToLower() == stateName.Replace(" ", string.Empty).ToLower());
                }  
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting state: {0} ", ex.ToString());
            }

            return state;
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
