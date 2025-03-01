using Core_Layer.Entities;
using Core_Layer.Inetrfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Specifications
{
    public class SpecificationEvaluator <TEntity , TKey> where TEntity : BaseEntity<TKey>
    {
        public static IQueryable<TEntity> BuildQuery(IQueryable<TEntity> inputQuery , ISpecification<TEntity> specs)
        {
            var query = inputQuery;
            if(specs.Criteria is not null)
                query = query.Where(specs.Criteria);

            foreach(var item in specs.Includes)
            {
                query = query.Include(item);
            }

            return query;
        }
    }
}
