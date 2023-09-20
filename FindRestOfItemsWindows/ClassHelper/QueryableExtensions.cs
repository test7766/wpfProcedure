using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Optimapharm.PSC.PurchasingManager.Windows.FindRestOfItemsWindows.ClassHelper
{
    public static class QueryableExtensions
    {
        public static Expression<Func<T, bool>> FilterKey<T>(T castData)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            List<Expression> propertyExpressions = new List<Expression>();

            // Перебираем свойства в castData и добавляем их в запрос
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                object propertyValue = propertyInfo.GetValue(castData);
                if (propertyValue != null)
                {
                    Expression propertyExpression = Expression.Property(parameter, propertyInfo);

                    // Если свойство является строкой, то выполняем фильтрацию строковым образом
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        Expression propertyToLower = Expression.Call(propertyExpression, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                        Expression propertyContains = Expression.Call(propertyToLower, typeof(string).GetMethod("Contains"), Expression.Constant(propertyValue.ToString().ToLower()));
                        propertyExpressions.Add(propertyContains);
                    }
                    else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // Если свойство является nullable типом, то выполняем фильтрацию на равенство, учитывая null
                        var propertyValueExpression = Expression.Constant(propertyValue, propertyInfo.PropertyType);
                        propertyExpressions.Add(Expression.Equal(propertyExpression, propertyValueExpression));
                    }
                    else
                    {
                        // Если свойство не является строкой и не nullable, то выполняем фильтрацию на равенство
                        propertyExpressions.Add(Expression.Equal(propertyExpression, Expression.Constant(propertyValue)));
                    }
                }
            }
            // Составляем выражение для фильтрации
            Expression filterExpression = propertyExpressions.Any()
                ? propertyExpressions.Aggregate((expr1, expr2) => Expression.AndAlso(expr1, expr2))
                : Expression.Constant(true);

            return Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
        }


    }
}

