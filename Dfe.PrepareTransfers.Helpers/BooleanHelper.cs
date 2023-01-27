namespace Dfe.PrepareTransfers.Helpers
{
    public static class BooleanHelper
    {
        public static string ToDisplay(this bool? input) 
        {
            if (input == null) return string.Empty;
            
            return input.Value ? "Yes" : "No";
        }
    }
}
