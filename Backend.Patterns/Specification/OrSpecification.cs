using System;
using System.Linq;
using System.Linq.Expressions;

namespace Backend.Patterns.Specification
{
    internal sealed class OrSpecification<T> : AbstractSpecification<T>
    {
        #region Fields
        private readonly AbstractSpecification<T> _left;
        private readonly AbstractSpecification<T> _right;
        #endregion

        #region Constructor
        public OrSpecification(AbstractSpecification<T> _left, AbstractSpecification<T> _right)
        {
            this._left = _left;
            this._right = _right;
        }
        #endregion

        #region Methods
        public override Expression<Func<T, bool>> ToExpression()
        {
            var _leftExpression = _left.ToExpression();
            var _rightExpression = _right.ToExpression();

            var orElseExpression = Expression.OrElse(_leftExpression, _rightExpression);
            return Expression.Lambda<Func<T, bool>>(orElseExpression, _leftExpression.Parameters.Single());
        }
        #endregion
    }
}