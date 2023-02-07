using System.Linq.Expressions;
using System.Reflection;

namespace ReactiveFramework.Extensions;

public static class ExpressionExtensions
{
    public static string GetPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> expression)
    {
        var propInfo = expression.GetMember();
        return propInfo.Name;
    }

    public static PropertyInfo GetPropertyInfo<T, TProperty>(this Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is not MemberExpression member)
        {
            throw new ArgumentException($"Expression refers to a method");
        }

        var propInfo = member.Member as PropertyInfo;

        return propInfo ?? throw new ArgumentException("Expression refers to a field");
    }

    public static MemberInfo GetMember<T, TProperty>(this Expression<Func<T, TProperty>> expression)
    {
        if (RemoveUnary(expression.Body) is not MemberExpression memberExp)
        {
            throw new ArgumentException("Expressions refers to a methode");
        }

        var currentExpr = memberExp.Expression;

        if (currentExpr is null)
        {
            throw new InvalidOperationException("Expression was null");
        }

        while (true)
        {
            currentExpr = RemoveUnary(currentExpr!);

            if (currentExpr != null && currentExpr.NodeType == ExpressionType.MemberAccess)
            {
                currentExpr = ((MemberExpression)currentExpr).Expression;
            }
            else
            {
                break;
            }
        }

        if (currentExpr == null || currentExpr.NodeType != ExpressionType.Parameter)
        {
            throw new Exception();
        }

        return memberExp.Member;
    }

    private static Expression RemoveUnary(Expression toUnwrap)
    {
        return toUnwrap is UnaryExpression expression ? expression.Operand : toUnwrap;
    }
}

