# Fix-Static-Asset-Handling-ASPNET-Core-eShop

## Objective

Fix static asset handling across three ASP.NET Core eShop projects (eShopLegacyMVC, eShopLegacyWebForms, eShopLegacyNTier) by relocating static files into wwwroot/, resolving case-sensitivity mismatches that cause 404 errors on Linux, implementing missing image URI logic, and updating path references so that ASP.NET Core's static file middleware correctly serves all assets.

## Summary

This transformation addresses static asset serving failures in a multi-project eShop solution that was ported from .NET Framework to ASP.NET Core. Static files such as product images (Pics/), fonts, and stylesheets must reside under wwwroot/ for ASP.NET Core's UseStaticFiles() middleware to serve them. The transformation moves misplaced directories into wwwroot/, fixes case-sensitivity issues between file references in Razor views, CSS, and Bundle.config versus actual filenames on disk, implements the missing AddUriPlaceHolder method so product images render correctly, updates the CatalogDBInitializer path to reflect the new wwwroot location, and performs cross-project validation to ensure consistency across all three applications.

## Entry Criteria

1. The solution contains ASP.NET Core projects that use the static file middleware (UseStaticFiles()).
2. One or more projects have static asset directories (Pics/, fonts/, images/, Content/, Scripts/) located at the project root instead of inside wwwroot/.
3. File or directory references in Razor views (.cshtml), Blazor components (.razor), CSS files, or configuration files (Bundle.config) have case mismatches compared to actual filenames on disk, which will cause failures on case-sensitive file systems (Linux).
4. The CatalogController contains an empty AddUriPlaceHolder method that does not set PictureUri on CatalogItem objects.
5. The CatalogDBInitializer references a Pics path at the content root rather than within wwwroot/.
6. All three solutions (eShopLegacyMVC, eShopLegacyWebForms, eShopLegacyNTier) build successfully before transformation begins.

## Implementation Steps

1. In the eShopLegacyMVC project, move the Pics/ directory from the project root (eShopLegacyMVCSolution/src/eShopLegacyMVC/Pics/) into wwwroot/ (eShopLegacyMVCSolution/src/eShopLegacyMVC/wwwroot/Pics/). This allows ASP.NET Core static file middleware to serve product catalog images.

2. In the eShopLegacyMVC project, move the fonts/ directory from the project root (eShopLegacyMVCSolution/src/eShopLegacyMVC/fonts/) into wwwroot/ (eShopLegacyMVCSolution/src/eShopLegacyMVC/wwwroot/fonts/). The existing base.css relative reference (../fonts/) from the Content/ directory will resolve correctly once fonts/ is inside wwwroot/.

3. In the eShopLegacyMVC project, fix the CSS filename case mismatch. The _Layout.cshtml references ~/Content/site.css (lowercase 's') but the actual file on disk is Site.css (uppercase 'S'). Update the reference in _Layout.cshtml to ~/Content/Site.css to match the file on disk and align with the eShopPorted reference project.

4. In the eShopLegacyMVC project, rename the wwwroot/Images/ directory to wwwroot/images/ (lowercase 'i'). The _Layout.cshtml references ~/images/brand.png, ~/images/brand_dark.png, ~/images/main_footer_text.png with lowercase 'images', and Site.css references ../images/main_banner.png. All references use lowercase, so the directory name must match.

5. In the eShopPorted reference project, also rename wwwroot/Images/ to wwwroot/images/ (lowercase 'i') to fix the same case-sensitivity issue that exists there.

6. In _Layout.cshtml, remove or update any TODO comment on or around line 12 that states "File not found at this path," since the casing fixes resolve the underlying issue.

7. In the eShopLegacyMVC project, implement the AddUriPlaceHolder method in CatalogController.cs. Replace the empty method body with logic that sets the PictureUri property: item.PictureUri = $"/Pics/{item.Id}.png". This enables product images to render in catalog views (CatalogTable.cshtml, Edit.cshtml, Delete.cshtml, Details.cshtml) using the images now served from wwwroot/Pics/.

8. In the eShopLegacyMVC project, update CatalogDBInitializer.cs to reference the Pics directory within wwwroot/ instead of the content root. Change the path construction from Path.Combine(contentRootPath, "Pics") to Path.Combine(contentRootPath, "wwwroot", "Pics"). Alternatively, if IWebHostEnvironment is available, use WebRootPath to construct the path.

9. In the eShopLegacyWebForms project, fix the Bundle.config case mismatch. Bundle.config references ~/Content/Site.css (uppercase 'S') but the actual file on disk is site.css (lowercase 's'). Update the Bundle.config reference to use ~/Content/site.css to match the file on disk and align with the App.razor references that already use the lowercase form.

10. In the eShopLegacyWebForms project, verify that App.razor CSS references use base-relative paths (e.g., Content/site.css without ~/ prefix) which is correct for Blazor Server since the base href is set to "/".

11. Verify that the PicController in eShopLegacyMVC correctly constructs file paths by combining WebRootPath with the picture filename. Confirm that Path.Combine(webRoot, item.PictureFileName) resolves to the correct location under wwwroot/Pics/. If PictureFileName contains only the filename (e.g., "1.png") without the "Pics/" subdirectory prefix, update the path construction to include the Pics/ subdirectory.

12. Verify that UseStaticFiles() middleware is present in the startup/program configuration of all three projects: eShopLegacyMVC Program.cs, eShopPorted Startup.cs, and eShopLegacyWebForms Program.cs.

13. Perform a final cross-project audit to confirm all three projects have a consistent wwwroot/ directory structure containing Content/, Scripts/, Pics/, fonts/, and images/ subdirectories as applicable, with no static assets remaining at the project root that should be in wwwroot/.

## Validation / Exit Criteria

1. All three solutions (eShopLegacyMVC, eShopLegacyWebForms, eShopLegacyNTier) build successfully without errors using dotnet build.
2. The Pics/ and fonts/ directories in eShopLegacyMVC exist only under wwwroot/ and not at the project root.
3. All file and directory name casings match their references in Razor views, Blazor components, CSS files, and configuration files -- specifically site.css/Site.css references are consistent and the images/ directory name matches lowercase references in views and CSS.
4. The AddUriPlaceHolder method in CatalogController sets item.PictureUri to "/Pics/{item.Id}.png" so product images render correctly in all catalog views.
5. The CatalogDBInitializer path for Pics resolves to the wwwroot/Pics/ directory rather than the content root.
6. The Bundle.config in eShopLegacyWebForms references site.css with correct casing matching the file on disk.
7. The PicController path construction correctly resolves to files within wwwroot/Pics/.
8. UseStaticFiles() middleware is confirmed present in all three project startup configurations.
9. No case-sensitivity mismatches remain across any project that would cause 404 errors on Linux file systems.
