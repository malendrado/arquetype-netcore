using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Backend.Infrastructure.Helpers
{
    public static class MappingHelper
{
    public static Expression<Func<TTo, bool>> ConvertExpression<TFrom, TTo>(this Expression<Func<TFrom, bool>> expr)
    {
        Dictionary<Expression, Expression> substitutues = new Dictionary<Expression, Expression>();
        var oldParam = expr.Parameters[0];
        var newParam = Expression.Parameter(typeof(TTo), oldParam.Name);
        substitutues.Add(oldParam, newParam);
        Expression body = ConvertNode(expr.Body, substitutues);
        return Expression.Lambda<Func<TTo, bool>>(body, newParam);
    }

    static Expression ConvertNode(Expression node, IDictionary<Expression, Expression> subst)
    {
        if (node == null) return null;
        if (subst.ContainsKey(node)) return subst[node];

        switch (node.NodeType)
        {
            case ExpressionType.Constant:
                return node;
            case ExpressionType.MemberAccess:
                {
                    var me = (MemberExpression)node;
                    var newNode = ConvertNode(me.Expression, subst);

                    MemberInfo info = null;
                    foreach (MemberInfo mi in newNode.Type.GetMembers())
                    {
                        if (mi.MemberType == MemberTypes.Property)
                        {
                            if (mi.Name.ToLower().Contains(me.Member.Name.ToLower()))
                            {
                                info = mi;
                                break;
                            }
                        }
                    }
                    return Expression.MakeMemberAccess(newNode, info);
                }
            case ExpressionType.AndAlso:
            case ExpressionType.OrElse:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.Equal:
            case ExpressionType.Call: /* will probably work for a range of common binary-expressions */
                {
                    var be = (BinaryExpression)node;
                    return Expression.MakeBinary(be.NodeType, ConvertNode(be.Left, subst), ConvertNode(be.Right, subst), be.IsLiftedToNull, be.Method);
                }

            default:
                throw new NotSupportedException(node.NodeType.ToString());
        }
    }
}
}