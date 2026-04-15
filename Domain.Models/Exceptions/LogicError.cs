namespace Domain.Models.Exceptions
{
    public class LogicError : Exception
    {
        private LogicError(string message = "") : base(message) { }
        public static void Check(Func<bool> cond, string message = "")
        {
            bool all_well = cond();
            if (all_well) return;
            System.Diagnostics.Debug.Assert(false);
            throw new LogicError(message);
        }
    }
}
