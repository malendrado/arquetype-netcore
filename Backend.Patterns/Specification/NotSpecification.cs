using System;
using System.Linq;
using System.Linq.Expressions;

namespace Backend.Patterns.Specification
{
    internal sealed class NotSpecification<T> : AbstractSpecification<T>
    {
        #region Fields
        private readonly AbstractSpecification<T> _specification;
        #endregion

        #region Constructor
        public NotSpecification(AbstractSpecification<T> _specification)
        {
            this._specification = _specification;
        }
        #endregion

        #region Methods
        public override Expression<Func<T, bool>> ToExpression()
        {
            var _expression = _specification.ToExpression();

            var notExpression = Expression.Not(_expression.Body);
            return Expression.Lambda<Func<T, bool>>(notExpression, _expression.Parameters.Single());
        }
        #endregion
    }
}