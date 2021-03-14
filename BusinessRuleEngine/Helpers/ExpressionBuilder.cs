using BusinessRuleEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace BusinessRuleEngine.Helpers
{
    public class ExpressionBuilder<T> where T : class
    {
        public Expression<Func<T, bool>> GenerateExpression(RuleSetRule rule)
        {
            if (!string.IsNullOrWhiteSpace(rule.PropertyName) && !string.IsNullOrWhiteSpace(rule.Operation) && !string.IsNullOrWhiteSpace(rule.Value))
            {
                var objectProperty = typeof(T).GetProperty(rule.PropertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                if (objectProperty != null)
                {
                    var parameter = Expression.Parameter(typeof(T), "X");
                    var leftExpression = Expression.Property(parameter, rule.PropertyName);
                    var rightExpression = Expression.Property(parameter, rule.PropertyName);
                    var constantExpression = Expression.Constant(Convert.ToDouble(rule.Value), typeof(double));
                    var constantExpression2 = Expression.Constant(Convert.ToDouble(2000), typeof(double));
                    if (Enum.TryParse<ExpressionType>(rule.Operation, out ExpressionType operation))
                    {

                        var expression = Expression.MakeBinary(operation, leftExpression, constantExpression);
                        var expression2 = Expression.MakeBinary(ExpressionType.GreaterThan, rightExpression, constantExpression2);
                        var x = Expression.And(
                                         expression,
                                        expression2
                                       );
                        return Expression.Lambda<Func<T, bool>>(x, parameter);
                    }
                    return null;
                }
            }
            return null;
        }

        //public Expression<Func<T, bool>> AndComp(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        //{

        //    return first.Compose(second, Expression.AndAlso);
        //}

        public Expression<Func<T, bool>> CombineExpression(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second, string appendoperation)
        {
            Expression<Func<T, bool>> result = null;
            result = (appendoperation == "AND") ? AndAlso(first, second) : OrElse(first, second);
            return result;
        }

        public Expression<Func<T, bool>> AndAlso(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null || right == null)
            {
                return left ?? right;
            }
            var combined = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right));
            return combined;
        }

        public Expression<Func<T, bool>> OrElse(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null || right == null)
            {
                return left ?? right;
            }
            var combined = Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), left.Parameters);
            return combined;
        }
    }
}
