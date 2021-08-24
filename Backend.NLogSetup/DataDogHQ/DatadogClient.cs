using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.NLogSetup.DataDogHQ
{
    public interface IDatadogClient
    {
        /// <summary>Send payload to Datadog logs-backend synchronously.</summary>
        void Write(IReadOnlyCollection<string> events);

        /// <summary>Send payload to Datadog logs-backend asynchronously.</summary>
        Task WriteAsync(IReadOnlyCollection<string> events);

        /// <summary>
        ///     Cleanup existing resources.
        /// </summary>
        void Close();
    }
}