namespace Maui.HassWebView.Core;

/// <summary>
/// Provides data for the remote control key events.
/// </summary>
public class RemoteKeyEventArgs : EventArgs
{
    /// <summary>
    /// Gets the name of the key that was pressed.
    /// </summary>
    public string KeyName { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the event was handled.
    /// Setting this to false will pass the event to the underlying system for processing.
    /// </summary>
    public bool Handled { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteKeyEventArgs"/> class.
    /// </summary>
    /// <param name="keyName">The name of the key.</param>
    public RemoteKeyEventArgs(string keyName)
    {
        KeyName = keyName;
    }
}
