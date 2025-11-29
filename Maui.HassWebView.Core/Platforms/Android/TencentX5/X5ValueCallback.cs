using Com.Tencent.Smtt.Sdk;

namespace Maui.HassWebView.Core.Platforms.Android.TencentX5;

public class X5ValueCallback : Java.Lang.Object, IValueCallback
{
    private readonly Action<string> _callback;
    public X5ValueCallback(Action<string> callback) => _callback = callback;
    public void OnReceiveValue(Java.Lang.Object value)
    {
        _callback?.Invoke(value?.ToString());
    }
}