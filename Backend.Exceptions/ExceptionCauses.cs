using System.Collections.Generic;

namespace Backend.Exceptions
{
    public class ExceptionCauses
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string ProspectoId { get; set; }
        public IDictionary<string, string> Data { get; set; } = new Dictionary<string,string>();
    }
}