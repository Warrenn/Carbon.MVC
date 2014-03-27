using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.Code
{
    public class DataTableResultModelBuilder<T>
    {
        private  IQueryable<T> queryable;
        private  Func<T, object[]> dataExpression;

        private readonly IList<Func<IQueryable<T>, IQueryable<T>>> sortAscendingExpressions =
            new List<Func<IQueryable<T>, IQueryable<T>>>();

        private readonly IList<Func<IQueryable<T>, IQueryable<T>>> sortDescendingExpressions =
            new List<Func<IQueryable<T>, IQueryable<T>>>();


        public void AddQueryable(IQueryable<T> query)
        {
            queryable = query;
            
        }
        public void AddDataExpression(Func<T,object[]> dataConversion)
        {
            dataExpression = dataConversion;
        }

        public void AddSortExpression<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            sortAscendingExpressions.Add(queryable1 => queryable1.OrderBy(expression));
            sortDescendingExpressions.Add(queryable1 => queryable1.OrderByDescending(expression));
        }

        public void AddSearchFilter(Expression<Func<T, bool>> expression)
        {
            if (queryable == null) return;
            queryable = queryable.Where(expression);
        }

        public DataTableResultModel BuildResult(DataTableParamModel param)
        {
            var query = queryable;
            foreach (var sortModel in param.SortCol)
            {
                var columnIndex = sortModel.ColumnId;
                if (columnIndex >= sortAscendingExpressions.Count()) continue;

                query = sortModel.Ascending
                            ? sortAscendingExpressions[columnIndex](query)
                            : sortDescendingExpressions[columnIndex](query);
            }
            var total = query.Count();
            var range = query.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var data = range.ToArray().Select(dataExpression);
            return new DataTableResultModel
                {
                    aaData = data,
                    iTotalDisplayRecords = total,
                    iTotalRecords = total,
                    sEcho = param.sEcho
                };
        }
    }
}