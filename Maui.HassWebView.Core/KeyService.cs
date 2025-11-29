using System.Diagnostics;

namespace Maui.HassWebView.Core;

public class KeyService
{
    // Events now use the new RemoteKeyEventArgs
    public event Action<RemoteKeyEventArgs> SingleClick;
    public event Action<RemoteKeyEventArgs> DoubleClick;
    public event Action<RemoteKeyEventArgs> LongClick;

    private readonly int _longPressTimeout;
    private readonly int _doubleClickTimeout;

    private System.Threading.Timer _longPressTimer;
    private System.Threading.Timer _doubleClickTimer;
    private string _lastKey;
    private int _pressCount = 0;

    public KeyService(int longPressTimeout = 750, int doubleClickTimeout = 300)
    {
        _longPressTimeout = longPressTimeout;
        _doubleClickTimeout = doubleClickTimeout;
    }

    public void OnPressed(string keyName)
    {
        if (_lastKey != keyName)
        {
            ResetDoubleClickState();
            _pressCount = 0;
        }
        
        _lastKey = keyName;
        _pressCount++;

        _doubleClickTimer?.Change(Timeout.Infinite, Timeout.Infinite);

        if (_pressCount == 1)
        {
            _longPressTimer = new System.Threading.Timer(LongPressTimerCallback, keyName, _longPressTimeout, Timeout.Infinite);
        }
    }

    public void OnReleased()
    {
        _longPressTimer?.Change(Timeout.Infinite, Timeout.Infinite);

        if (_pressCount == 1)
        {
            _doubleClickTimer = new System.Threading.Timer(DoubleClickTimerCallback, _lastKey, _doubleClickTimeout, Timeout.Infinite);
        }
        else if (_pressCount >= 2)
        {
            Debug.WriteLine("DoubleClick detected");
            // Use the new RemoteKeyEventArgs
            DoubleClick?.Invoke(new RemoteKeyEventArgs(_lastKey));
            ResetDoubleClickState();
        }
    }

    private void LongPressTimerCallback(object state)
    { 
        Debug.WriteLine("LongClick detected");
        // Use the new RemoteKeyEventArgs
        LongClick?.Invoke(new RemoteKeyEventArgs((string)state));
        ResetDoubleClickState();
    }

    private void DoubleClickTimerCallback(object state)
    {
        Debug.WriteLine("SingleClick detected");
        // Use the new RemoteKeyEventArgs
        SingleClick?.Invoke(new RemoteKeyEventArgs((string)state));
        ResetDoubleClickState();
    }

    private void ResetDoubleClickState()
    {
        _pressCount = 0;
        _lastKey = null;
        _doubleClickTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        _longPressTimer?.Change(Timeout.Infinite, Timeout.Infinite);
    }
}
