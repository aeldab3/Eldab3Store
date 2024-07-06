using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public BaseSpecification() { } //empty constructor

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>(); //Initializes an empty list to store lambda expressions for eager loading related entities

        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnable { get; private set; }

        //A protected method that allows adding a lambda expression to the Includes list.
        //Facilitates specifying related entities to include in the query results.
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = ConvertOrderByExpression(orderByExpression);
        }
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = ConvertOrderByExpression(orderByDescExpression);
        }

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnable = true;
        }

        private Expression<Func<T, object>> ConvertOrderByExpression(Expression<Func<T, object>> orderByExpression)
        {
            // Check if the body of the expression is a MemberExpression of type decimal
            if (orderByExpression.Body is MemberExpression memberExpression && memberExpression.Type == typeof(decimal))
            {
                // Convert decimal to double within a new lambda expression
                var converted = Expression.Lambda<Func<T, double>>(
                    Expression.Convert(memberExpression, typeof(double)), orderByExpression.Parameters);

                // Wrap the converted expression back into the original type
                return Expression.Lambda<Func<T, object>>(converted.Body, converted.Parameters);
            }
            // Return the original expression if no conversion is needed
            return orderByExpression;
        }

    }
}
