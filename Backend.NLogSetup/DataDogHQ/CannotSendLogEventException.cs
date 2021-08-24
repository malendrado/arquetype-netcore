using System;

namespace Backend.NLogSetup.DataDogHQ
{
    public class CannotSendLogEventException : Exception
    {
        public CannotSendLogEventException() : base($"Could not send payload to Datadog") {  }
    }
}