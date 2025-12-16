using System.Diagnostics;
using Microsoft.Maui.ApplicationModel;
using HassWebView.Core.Events;
using System;
using System.Threading;

namespace HassWebView.Core.Services;

public class KeyService
{
    // Events for key actions
    public event Func<object, RemoteKeyEventArgs, bool> KeyDown;
    public event Action<object, RemoteKeyEventArgs> KeyUp;
    public event Action<object, RemoteKeyEventArgs> SingleClick;
    public event Action<object, RemoteKeyEventArgs> DoubleClick;
    public event Action<object, RemoteKeyEventArgs> LongClick;

    private readonly int _longPressTimeout;
    private readonly int _doubleClickTimeout;

    private Timer _longPressTimer;
    private Timer _doubleClickTimer;
    private string _lastKey;
    private int _pressCount = 0;
    private bool _longPressHasFired = false;
    private Timer _repeatingActionTimer;
    private Action _repeatingAction;

    public KeyService(int longPressTimeout = 750, int doubleClickTimeout = 300)
    {
        _longPressTimeout = longPressTimeout;
        _doubleClickTimeout = doubleClickTimeout;
    }

    public void StartRepeatingAction(Action action, int interval = 100)
    {
        StopRepeatingAction();
        _repeatingAction = action;
        _repeatingActionTimer = new Timer(RepeatingActionCallback, null, 0, interval);
    }

    private void RepeatingActionCallback(object state)
    {
        MainThread.BeginInvokeOnMainThread(() => _repeatingAction?.Invoke());
    }

    public void StopRepeatingAction()
    {
        _repeatingActionTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        _repeatingActionTimer?.Dispose();
        _repeatingActionTimer = null;
        _repeatingAction = null;
    }

    public bool OnPressed(string keyName)
    {
        var args = new RemoteKeyEventArgs(keyName);
        bool handled = false;
        if (KeyDown != null)
        {
            foreach (Func<object, RemoteKeyEventArgs, bool> handler in KeyDown.GetInvocationList())
            {
                if (handler(this, args))
                {
                    handled = true;
                    break;
                }
            }
        }

        if (!handled)
        {
            ResetDoubleClickState();
            return false;
        }

        if (_longPressHasFired) return true;

        if (_lastKey != keyName)
        {
            StopRepeatingAction();
            ResetDoubleClickState();
            _pressCount = 0;
        }

        _lastKey = keyName;
        _pressCount++;

        _doubleClickTimer?.Change(Timeout.Infinite, Timeout.Infinite);

        if (_pressCount == 1)
        {
            _longPressTimer = new Timer(LongPressTimerCallback, keyName, _longPressTimeout, Timeout.Infinite);
        }

        return true;
    }

    public bool OnReleased()
    {
        StopRepeatingAction();

        if (_lastKey == null && !_longPressHasFired)
        {
            return false;
        }

        if (_lastKey != null)
        {
            KeyUp?.Invoke(this, new RemoteKeyEventArgs(_lastKey));
        }

        if (_longPressHasFired)
        {
            _longPressHasFired = false;
            ResetDoubleClickState();
            return true;
        }

        _longPressTimer?.Change(Timeout.Infinite, Timeout.Infinite);

        if (_pressCount == 1)
        {
            _doubleClickTimer = new Timer(DoubleClickTimerCallback, _lastKey, _doubleClickTimeout, Timeout.Infinite);
        }
        else if (_pressCount >= 2)
        {
            DoubleClick?.Invoke(this, new RemoteKeyEventArgs(_lastKey));
            ResetDoubleClickState();
        }

        return true;
    }

    private void LongPressTimerCallback(object state)
    {
        if (_longPressHasFired) return;
        _longPressHasFired = true;
        LongClick?.Invoke(this, new RemoteKeyEventArgs((string)state));
    }

    private void DoubleClickTimerCallback(object state)
    {
        SingleClick?.Invoke(this, new RemoteKeyEventArgs((string)state));
        ResetDoubleClickState();
    }

    private void ResetDoubleClickState()
    {
        _pressCount = 0;
        _lastKey = null;
        _doubleClickTimer?.Dispose();
        _doubleClickTimer = null;
        _longPressTimer?.Dispose();
        _longPressTimer = null;
    }
}