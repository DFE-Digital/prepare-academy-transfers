using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Web.Models;

public static class Links
{
   private static readonly IDictionary<string, LinkItem> LinkCache = new ConcurrentDictionary<string, LinkItem>();

   private static LinkItem Create(string page, string text = "Back")
   {
      var item = new LinkItem { PageName = page, BackText = text };
      LinkCache.Add(page, item);
      return item;
   }

   public static LinkItem FindByPageName(string page)
   {
      if (string.IsNullOrWhiteSpace(page)) return default;

      return LinkCache.ContainsKey(page) ? LinkCache[page] : default;
   }

   public static class Global
   {
      public static readonly LinkItem CookiePreferences = Create(page: "/CookiePreferences");
   }

   public static class HeadteacherBoard
   {
      public static readonly LinkItem Preview =
         Create("/TaskList/HtbDocument/Preview", "Back to preview");
   }

   public static class ProjectType
   {
      public static readonly LinkItem Index =
         Create("/ProjectType/Index", "Back to project type");
   }

   public static class Project
   {
      public static readonly LinkItem Index = Create("/Projects/Index", "Back to project");
   }

   public static class ProjectList
   {
      public static readonly LinkItem Index = Create("/Home/Index");
   }

   public static class ProjectAssignment
   {
      public static readonly LinkItem Index = Create("/Projects/ProjectAssignment/Index");
   }

   public static class LegalRequirements
   {
      public static readonly LinkItem Index = Create("/Projects/LegalRequirements/Index");

      public static class IncomingTrustAgreement
      {
         public static readonly LinkItem Home =
            Create("/Projects/LegalRequirements/IncomingTrustAgreement");
      }

      public static class DiocesanConsent
      {
         public static readonly LinkItem Home = Create("/Projects/LegalRequirements/DiocesanConsent");
      }

      public static class OutgoingTrustConsent
      {
         public static readonly LinkItem Home = Create("/Projects/LegalRequirements/OutgoingTrustConsent");
      }
   }
}