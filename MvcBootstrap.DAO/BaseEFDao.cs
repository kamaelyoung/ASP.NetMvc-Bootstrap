﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcBootstrap.IDAO;
using MvcBootstrap.EFModel;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;

namespace MvcBootstrap.DAO
{
    public abstract class BaseEFDao<T> : IBaseDao<T> where T : class
    {
        protected virtual string tableName
        {
            get
            {
                return this.GetType().Name.ToString().Replace("Dao", "");
            }
        }

        #region IBaseDao<T> Members

        public virtual T GetEntity(Expression<Func<T, bool>> whereExp)
        {
            using (DBEntity db = new DBEntity())
            {
                return db.CreateObjectSet<T>().Where(whereExp).SingleOrDefault();
            }
        }

        public virtual IEnumerable<T> GetAll()
        {
            using (DBEntity db = new DBEntity())
            {
                return db.CreateObjectSet<T>().ToList();
            }
        }

        public virtual IEnumerable<T> GetEntities(Expression<Func<T, bool>> whereExp)
        {
            using (DBEntity db = new DBEntity())
            {
                return db.CreateObjectSet<T>().Where(whereExp).ToList();
            }
        }

        public virtual int GetEntitiesCount(Expression<Func<T, bool>> whereExp)
        {
            using (DBEntity db = new DBEntity())
            {
                return db.CreateObjectSet<T>().Where(whereExp).Count();
            }
        }


        public virtual IEnumerable<T> GetPagingInfo(Expression<Func<T, int>> orderby, int pageIndex, int pageSize)
        {
            using (DBEntity db = new DBEntity())
            {
                return db.CreateObjectSet<T>()
                         .OrderBy(orderby)
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
            }
        }

        public virtual IEnumerable<T> GetPagingInfo(int pageIndex, int pageSize)
        {
            using (DBEntity db = new DBEntity())
            {
                return db.CreateObjectSet<T>()
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
            }
        }

        public virtual IEnumerable<T> GetPagingInfo(int pageSize)
        {
            using (DBEntity db = new DBEntity())
            {
                return db.CreateObjectSet<T>()
                         .Take(pageSize)
                         .ToList();
            }
        }

        public virtual bool Create(T entity)
        {
            using (DBEntity db = new DBEntity())
            {
                db.CreateObjectSet<T>().AddObject(entity);
                return db.SaveChanges() > 0;
            }
        }

        public virtual bool Update(T entity)
        {
            using (DBEntity db = new DBEntity())
            {
                try
                {
                    var obj = db.CreateObjectSet<T>();
                    obj.Attach(entity);
                    db.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
                    return db.SaveChanges() > 0;
                }
                catch (OptimisticConcurrencyException)
                {
                    db.Refresh(RefreshMode.StoreWins, entity);
                    return false;
                }
            }
        }

        public virtual bool Delete(T entity)
        {
            using (DBEntity db = new DBEntity())
            {
                try
                {
                    var obj = db.CreateObjectSet<T>();
                    obj.Attach(entity);
                    db.ObjectStateManager.ChangeObjectState(entity, EntityState.Deleted);
                    obj.DeleteObject(entity);
                    return db.SaveChanges() > 0;
                }
                catch (OptimisticConcurrencyException)
                {
                    db.Refresh(RefreshMode.StoreWins, entity);
                    return false;
                }
            }
        }

        public virtual bool Delete(List<int> idList)
        {
            using (DBEntity db = new DBEntity())
            {
                try
                {
                    string ids = string.Join(",", idList.ToArray());
                    db.DeleteObjects(ids, tableName);
                    return true;
                }
                catch (OptimisticConcurrencyException)
                {
                    return false;
                }
            }
        }

        #endregion
    }
}
