using System;

namespace MongoIdentity.Models
{
    public class ConfirmationOccurrence : Occurrence
    {
        public ConfirmationOccurrence()
        {
        }

        public ConfirmationOccurrence(DateTime confirmedOn) : base(confirmedOn)
        {
        }
    }
}
