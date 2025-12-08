using Android.Webkit;
using Java.Lang;
using System;

namespace HassWebView.Core.Bridges
{
    // This is the Android-specific implementation of the ExternalApp partial class.
    // It inherits from Java.Lang.Object to be compatible with AddJavascriptInterface.
    public partial class ExternalApp : Lang.Object
    {
        // This partial method provides the Android-specific implementation.
        // It is decorated with [JavascriptInterface] to be callable from the WebView.
        [JavascriptInterface]
        public partial void getExternalAuth(string message)
        {
            _authAction?.Invoke("getExternalAuth", message);
            Console.WriteLine($"HassJsBridge.getExternalAuth called on Android with message: {message}");
        }

        // This partial method also provides the Android-specific implementation.
        [JavascriptInterface]
        public partial void revokeExternalAuth(string message)
        {
            _authAction?.Invoke("revokeExternalAuth", message);
            Console.WriteLine($"HassJsBridge.revokeExternalAuth called on Android with message: {message}");
        }
    }
}
