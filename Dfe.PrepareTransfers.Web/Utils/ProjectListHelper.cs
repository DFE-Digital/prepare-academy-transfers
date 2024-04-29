namespace Dfe.PrepareTransfers.Web.Utils
{
    public static class ProjectListHelper
   {
      // Convert from "LASTNAME, Firstname" to "Firstname Lastname"
      public static string ConvertToFirstLast(string name)
      {
         if (string.IsNullOrEmpty(name)) return string.Empty;

         var parts = name.Split(',');
         if (parts.Length == 2)
         {
            return $"{parts[1].Trim()} {parts[0].Trim()}";
         }

         return name;
      }
   }
}
