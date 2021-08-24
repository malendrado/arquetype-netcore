using System.Collections.Generic;
using Newtonsoft.Json;

namespace Backend.Exceptions
{
    public class ExceptionDetails
    {
        #region Fields
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Causes { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        #endregion
    }
}