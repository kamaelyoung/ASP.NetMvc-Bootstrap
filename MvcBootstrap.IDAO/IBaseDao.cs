﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcBootstrap.IDAO
{
    public interface IBaseDao<T> where T:class
    {
        T GetEntity(Func<T, bool> whereExp);
        IEnumerable<T> GetEntities(Func<T, bool> whereExp);
        int GetEntitiesCount(Func<T, bool> whereExp);
        IEnumerable<T> GetPagingInfo(Func<T, int> orderby, int pageIndex, int pageSize);
        bool Create(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        bool Delete(List<int> ids);
    }
}