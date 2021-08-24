using System;

namespace Backend.Models.Attributes
{
    public class EntityNameAttribute : Attribute
    {
        #region Fields
        private string _entityName;
        #endregion

        #region Constructor
        public EntityNameAttribute(string Entity)
        {
            _entityName = Entity;
        }
        #endregion

        #region Methods
        public string GetEntityName()
        {
            return _entityName;
        }
        #endregion
    }
}