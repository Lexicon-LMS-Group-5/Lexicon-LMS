using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Exceptions
{
    public class PersistentDataInconsistencyException: Exception
    {
        public enum Diagnostic
        {
            OK,  // Do not use.
            OutOfTimeBounds,
            Overlapping,
            NegativeTimeSpan,
        };

        public PersistentDataInconsistencyException(
            Diagnostic diagnostic,
            string context) 
            : base($"{diagnostic.ToString("F")} at {context}") {}
    }
}
