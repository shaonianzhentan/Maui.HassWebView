using HassWebView.Core.Events;

namespace HassWebView.Core.Services;

/// <summary>
/// Defines a contract for an object that can handle remote control key events.
/// </summary>
public interface IKeyHandler
{
    /// <summary>
    /// Called when a key is pressed down.
    /// </summary>
    /// <param name="sender">The KeyService instance that fired the event.</param>
    /// <param name="args">The event arguments, containing the key name.</param>
    /// <returns>Return true to indicate the event was handled. Return false to let the system handle it.</returns>
    bool OnKeyDown(KeyService sender, RemoteKeyEventArgs args);

    /// <summary>
    /// Called when a key is released.
    /// </summary>
    /// <param name="sender">The KeyService instance that fired the event.</param>
    /// <param name="args">The event arguments, containing the key name.</param>
    void OnKeyUp(KeyService sender, RemoteKeyEventArgs args);

    /// <summary>
    /// Called when a single click gesture is detected.
    /// </summary>
    /// <param name="sender">The KeyService instance that fired the event.</param>
    /// <param name="args">The event arguments, containing the key name.</param>
    void OnSingleClick(KeyService sender, RemoteKeyEventArgs args);

    /// <summary>
    /// Called when a double click gesture is detected.
    /// </summary>
    /// <param name="sender">The KeyService instance that fired the event.</param>
    /// <param name="args">The event arguments, containing the key name.</param>
    void OnDoubleClick(KeyService sender, RemoteKeyEventArgs args);

    /// <summary>
    /// Called when a long press gesture is detected.
    /// </summary>
    /// <param name="sender">The KeyService instance that fired the event.</param>
    /// <param name="args">The event arguments, containing the key name.</param>
    void OnLongClick(KeyService sender, RemoteKeyEventArgs args);
}
