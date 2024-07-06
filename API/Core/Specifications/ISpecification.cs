using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    //This interface defines a specification pattern for querying entities of type T.
    //This pattern promotes reusable and maintainable query logic.
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; } // criteria for filtering
        List<Expression<Func<T, object>>> Includes { get; } //a list of includes for eager loading related entities

        Expression<Func<T, Object>> OrderBy { get; }
        Expression<Func<T, Object>> OrderByDescending { get; }

        int Take { get; }
        int Skip { get; }
        bool IsPagingEnable { get; }

    }
}
