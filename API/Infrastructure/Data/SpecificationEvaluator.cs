using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    //This class is responsible for applying a given specification to an IQueryable<TEntity>.
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria); //Applies the filtering criteria to the query if it exists.
            }
            //Uses the Aggregate method to apply all the includes to the query.
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include)); //For each include expression in spec.Includes, it applies the Include method to the current query.  //current is the initial value


            return query; //Returns the final query with all specifications (criteria and includes) applied.
        }
    }
}
