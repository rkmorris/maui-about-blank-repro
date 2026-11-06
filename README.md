# .NET MAUI iOS WebView blank after prolonged idle — reproduction

Minimal standalone .NET 10 MAUI app reproducing an intermittent blank WebView on a physical iPhone after the app is left idle. **This is a synthetic reproduction, not production application code.** Its HTML is self-contained, offline and approximately 304 KB. No authentication, third-party SDKs or remote services are involved.

## Environment of captured failure

- Physical iPhone XS, iOS 18.7.11 (22H384)
- .NET 10, Microsoft.Maui.Controls 10.0.110
- MAUI `WebView` backed by `MauiWKWebView`

## Steps to reproduce

1. Open `MauiAboutBlankRepro.sln` in Visual Studio with the .NET 10 MAUI workload and iOS build tools
2. Configure your own Apple development signing in your local environment as required (no signing identity is checked into this repository)
3. Deploy to a physical iPhone and open **About**
4. Confirm the HTML displays correctly, then leave the page open and background the app / lock the phone
5. Return periodically or after several hours (the captured case took about 16 hours)
6. If the WebView is blank, take a screenshot, press **Record Blank Screen**, then **Export Diagnostics**
7. Save the exported log before uninstalling the app; the file is stored in application-private storage

The issue is intermittent, so an initial successful run does not rule out the failure.

## Evidence

- [`evidence/about-repro-diagnostics-sanitized.txt`](evidence/about-repro-diagnostics-sanitized.txt): event log from the real failure; local application-container identifier redacted without changing navigation results or timing
- [`evidence/blank-webview-iphone.png`](evidence/blank-webview-iphone.png): screenshot of the blank HTML area with the native diagnostic controls still visible

The diagnostic log shows an initial successful local HTML navigation, followed around 16 hours later by another navigation that reports `Failure`; the handler remains attached and the page remains visible. The log does not capture a native `NSError`, so the root cause is **not yet established**.

## Implementation

The HTML is opened via `FileSystem.OpenAppPackageFileAsync("about-repro.html")` and assigned once using `HtmlWebViewSource` inside `AboutPage.OnAppearing()`, guarded by `_htmlAssigned`. The app logs relevant MAUI events and exports the log using the system share sheet. There is **no automatic source reassignment on resume**.

## Privacy and public repository hygiene

This example includes synthetic HTML instead of production content. Build outputs, Visual Studio caches, signing information and local account data have been removed. The diagnostic log's container path UUID is redacted; no diagnostic event data was changed.

## Related bug

A separate .NET MAUI GitHub issue will document this WebView failure. This is unrelated to the tab-switching white-flash reproduction.
