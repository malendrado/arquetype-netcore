using System;
using System.Linq.Expressions;

namespace Backend.Patterns.Specification
{
    public class GenericSpecification<T>
    {
        #region Fields
        private readonly Expression<Func<T, bool>> _expression;
        #endregion

        #region Constructor
        public GenericSpecification(Expression<Func<T, bool>> _expression)
        {
            this._expression = _expression;
        }
        #endregion

        #region Methods
        public bool IsSatisfiedBy(T entity)
        {
            return _expression.Compile().Invoke(entity);
        }
        #endregion
    }
}