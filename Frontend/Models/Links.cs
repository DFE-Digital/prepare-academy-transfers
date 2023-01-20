using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Frontend.Models;

public static class Links
{
   private static readonly IDictionary<string, LinkItem> LinkCache = new ConcurrentDictionary<string, LinkItem>();

   public static readonly LinkItem CookiePreferences = Create(page: "/CookiePreferences");

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
         public static readonly LinkItem Index =
            Create("/Projects/LegalRequirements/IncomingTrustAgreement");
      }

      public static class DiocesanConsent
      {
         public static readonly LinkItem Index = Create("/Projects/LegalRequirements/DiocesanConsent");
      }

      public static class OutgoingTrustConsent
      {
         public static readonly LinkItem Index = Create("/Projects/LegalRequirements/OutgoingTrustConsent");
      }
   }
}

public class LinkItem
{
   public string PageName { get; set; }
   public string BackText { get; set; }
   public string Urn { get; set; }

   public LinkItem For(string urn)
   {
      return new LinkItem { BackText = BackText, PageName = PageName, Urn = urn };
   }

   public LinkItem OverrideFrom(IQueryCollection query)
   {
      return new LinkItem
      {
         BackText = query.ContainsKey("bt") ? query["bt"] : BackText,
         PageName = query.ContainsKey("bl") ? query["bl"] : PageName,
         Urn = query.ContainsKey("u") ? query["u"] : Urn
      };
   }
}
