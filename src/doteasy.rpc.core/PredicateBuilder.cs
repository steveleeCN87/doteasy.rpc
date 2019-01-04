using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DotEasy.Rpc.Core
{
    /// <summary>
    /// lamba表达式构建
    /// </summary>
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            return (Expression<Func<T, bool>>) (param => true);
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return (Expression<Func<T, bool>>) (param => false);
        }

        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
        {
            return predicate;
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose<Func<T, bool>>(second, new Func<Expression, Expression, Expression>(Expression.AndAlso));
        }

        public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> expression, bool condition, Expression<Func<T, bool>> filterExpression)
        {
            if (condition)
                return expression.And<T>(filterExpression);
            return expression;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose<Func<T, bool>>(second, new Func<Expression, Expression, Expression>(Expression.OrElse));
        }

        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            return Expression.Lambda<Func<T, bool>>((Expression) Expression.Not(expression.Body), (IEnumerable<ParameterExpression>) expression.Parameters);
        }

        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            Expression expression = ParameterRebinder.ReplaceParameters(first.Parameters.Select((f, i) => new
            {
                f = f,
                s = second.Parameters[i]
            }).ToDictionary(p => p.s, p => p.f), second.Body);
            return Expression.Lambda<T>(merge(first.Body, expression), (IEnumerable<ParameterExpression>) first.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> map;

            private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new PredicateBuilder.ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression parameterExpression;
                if (map.TryGetValue(p, out parameterExpression))
                    p = parameterExpression;
                return base.VisitParameter(p);
            }
        }
    }
}