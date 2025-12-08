using System;

namespace HassWebView.Core.Bridges
{
    // This is the shared part of the class, defining the public API and shared logic.
    // The class and its platform-specific methods are marked as partial.
    public partial class ExternalApp
    {
        private readonly Action<string, string> _authAction;

        // The constructor remains in the shared part as it's common to all platforms.
        public ExternalApp(Action<string, string> authAction)
        {
            _authAction = authAction;
        }

        // These are the method signatures. The implementations are provided
        // in the platform-specific partial class files.
        // Note: They have no body and are marked as partial.
        public partial void getExternalAuth(string message);

        public partial void revokeExternalAuth(string message);
    }
}
