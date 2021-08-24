using System;
using System.Linq.Expressions;

namespace Backend.Patterns.Specification
{
    internal sealed class IdentitySpecification<T> : AbstractSpecification<T>
    {
        public override Expression<Func<T, bool>> ToExpression()
        {
            return x => true;
        }
    }
}