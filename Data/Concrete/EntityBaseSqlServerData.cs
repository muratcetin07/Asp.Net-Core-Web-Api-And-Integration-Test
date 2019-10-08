using Data.Abstract;
using Data.Core;
using Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Utilities;

namespace Data.Concrete
{
    public class EntityBaseSqlServerData<T> : IData<T> where T : BaseModel
    {
        protected readonly DataContext _context;

        public EntityBaseSqlServerData(DataContext context)
        {
            _context = context;
        }

        #region IData Implementation
        public void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _context.Database.BeginTransaction().Commit();
        }

        public void Rollback()
        {
            _context.Database.BeginTransaction().Rollback();
        }


        /// <summary>
        /// Insert new Entity
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DataResult<T> Insert(T t)
        {
            try
            {
                BeginTransaction();

                _context.Set<T>().Add(t);

                _context.SaveChanges();

                Commit();

                return new DataResult<T>(t) { Code = ResponseCode.OK };
            }
            catch (Exception exc)
            {
                Rollback();
                return new DataResult<T>(null)
                {
                    Code = ResponseCode.BadRequest,
                    Message = exc.Message +
                    exc.InnerException == null ? "" : "(" + exc.InnerException + ")"
                };
            }
        }

        public DataResult InsertBulk(List<T> ts)
        {
            try
            {
                BeginTransaction();
                foreach (var item in ts)
                    _context.Set<T>().Add(item);

                _context.SaveChanges();
                Commit();

                return new DataResult(true, "");
            }
            catch (Exception e)
            {
                Rollback();
                return new DataResult(false, "");
            }

            
        }

        /// <summary>
        /// Update Entity
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DataResult<T> Update(T t)
        {
            try
            {
                long updateId = t.Id;

                T aModel = _context.Set<T>().Where(x => x.Id == updateId).FirstOrDefault();

                if (aModel == null)
                    return new DataResult<T>(null) { Code = ResponseCode.BadRequest, Message = "there is no row for update" };

                BeginTransaction();

                foreach (var propertyInfo in typeof(T).GetProperties())
                    propertyInfo.SetValue(aModel, propertyInfo.GetValue(t, null), null);

                

                _context.SaveChanges();

                Commit();

                return new DataResult<T>(t) { Code = ResponseCode.OK };
            }
            catch (Exception exc)
            {
                Rollback();

                return new DataResult<T>(null)
                {
                    Code = ResponseCode.BadRequest,
                    Message = exc.Message + " " + exc.InnerException == null ? "" : "(" + exc.InnerException + ")"
                };
            }
        }

        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DataResult<bool> Delete(T t)
        {
            return DeleteByKey(t.Id);
        }

        /// <summary>
        /// Delete Entity by Key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataResult<bool> DeleteByKey(long id)
        {
            try
            {
                T aModel = _context.Set<T>().Where(x => x.Id == id).FirstOrDefault();

                if (aModel == null)
                    return new DataResult<bool>(false) { Code = ResponseCode.BadRequest, Message = "there is no row for update" };

                BeginTransaction();
                _context.Set<T>().Remove(aModel);
                _context.SaveChanges();
                Commit();

                return new DataResult<bool>(true) { Code = ResponseCode.OK };
            }
            catch (Exception exc)
            {
                Rollback();

                return new DataResult<bool>(false)
                {
                    Code = ResponseCode.BadRequest,
                    Message = exc.Message +
                    exc.InnerException == null ? "" : "(" + exc.InnerException + ")"
                };
            }
        }

        /// <summary>
        /// Get Entity By Key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetByKey(long id)
        {
            try
            {
                T aModel = _context.Set<T>().Where(x => x.Id == id).FirstOrDefault();
                return aModel;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Get All Entities
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        /// <summary>
        /// Get Count of Entities
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return _context.Set<T>().Count();
        }

        /// <summary>
        /// Find and Get Count of Entities
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int GetCount(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>()
                .Where(predicate)
                .Count();
        }


        /// <summary>
        /// Get All Entities
        /// </summary>
        /// <param name="orderBy">property name to order</param>
        /// <returns></returns>
        public List<T> GetAll(string orderBy, bool isDesc = false)
        {
            return isDesc
                ? _context.Set<T>().OrderByDescending(orderBy).ToList()
                : _context.Set<T>().OrderBy(orderBy).ToList();
        }

        /// <summary>
        /// Find an Entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<T> GetBy(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _context.Set<T>()
                    .Where(predicate)
                    .ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Find an Entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy">order by property name</param>
        /// <returns></returns>
        public List<T> GetBy(Expression<Func<T, bool>> predicate, string orderBy, bool isDesc = false)
        {
            try
            {
                return isDesc
                    ? _context.Set<T>().Where(predicate).OrderByDescending(orderBy).ToList()
                    : _context.Set<T>().Where(predicate).OrderBy(orderBy).ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get Entities by Page
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<T> GetByPage(int pageNumber, int pageCount, string orderBy = "Id", bool isDesc = false)
        {
            try
            {
                return isDesc
                    ? _context.Set<T>().OrderByDescending(orderBy).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList()
                    : _context.Set<T>().OrderBy(orderBy).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Find and Get Entities by Page
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<T> GetByPage(Expression<Func<T, bool>> predicate, int pageNumber, int pageCount, string orderBy = "Id", bool isDesc = false)
        {
            try
            {
                return isDesc
                   ? _context.Set<T>().OrderByDescending(orderBy).Where(predicate).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList()
                   : _context.Set<T>().OrderBy(orderBy).Where(predicate).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get random records 
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<T> GetRandom(int limit)
        {
            if (limit <= 0)
                return new List<T>();

            return _context.Set<T>().OrderBy(x => Guid.NewGuid()).Take(limit).ToList();
        }

        /// <summary>
        /// Get random records in a predicate
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<T> GetRandom(Expression<Func<T, bool>> predicate, int limit)
        {
            if (limit <= 0)
                return new List<T>();

            return _context.Set<T>()
                .Where(predicate)
                .OrderBy(x => Guid.NewGuid())
                .Take(limit).ToList();
        }

        #endregion
    }
}
