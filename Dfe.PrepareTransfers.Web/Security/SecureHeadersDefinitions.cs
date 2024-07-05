using Dfe.PrepareTransfers.Web.Options;
using Microsoft.AspNetCore.Builder;

namespace Dfe.PrepareTransfers.Web.Security;

public static class SecurityHeadersDefinitions
{
   public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDev, AllowedExternalSourcesOptions externalSources)
   {
      HeaderPolicyCollection policy = new HeaderPolicyCollection()
         .AddFrameOptionsDeny()
         .AddXssProtectionDisabled()
         .AddContentTypeOptionsNoSniff()
         .AddReferrerPolicyStrictOriginWhenCrossOrigin()
         .RemoveServerHeader()
         .AddCrossOriginOpenerPolicy(builder => { builder.SameOrigin(); })
         .AddCrossOriginEmbedderPolicy(builder => { builder.RequireCorp(); })
         .AddCrossOriginResourcePolicy(builder => { builder.SameOrigin(); })
         .AddContentSecurityPolicy(builder =>
         {
            builder.AddObjectSrc().None();
            builder.AddBlockAllMixedContent();
            builder.AddImgSrc()
               .Self()
               .From("data:")
               .From(externalSources.Images);
            builder.AddFormAction().Self();
            builder.AddFormAction().OverHttps();
            builder.AddFontSrc().Self();
            builder.AddStyleSrc().Self();
            builder.AddBaseUri().Self();
            builder.AddScriptSrc()
               .From(externalSources.Scripts)
               .UnsafeInline()
               .WithNonce();
            builder.AddFrameAncestors().None();
         })
         .AddPermissionsPolicy(builder =>
         {
            builder.AddAccelerometer().None();
            builder.AddAutoplay().None();
            builder.AddCamera().None();
            builder.AddEncryptedMedia().None();
            builder.AddFullscreen().All();
            builder.AddGeolocation().None();
            builder.AddGyroscope().None();
            builder.AddMagnetometer().None();
            builder.AddMicrophone().None();
            builder.AddMidi().None();
            builder.AddPayment().None();
            builder.AddPictureInPicture().None();
            builder.AddSyncXHR().None();
            builder.AddUsb().None();
         });

      return policy;
   }
}
