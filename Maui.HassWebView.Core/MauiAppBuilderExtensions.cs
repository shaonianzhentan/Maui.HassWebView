using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.LifecycleEvents;
using System.Diagnostics;

#if ANDROID
using Com.Tencent.Smtt.Export.External;
using Com.Tencent.Smtt.Sdk;
using Maui.HassWebView.Core.Platforms.Android.TencentX5;
using AndroidX.Core.View;
using Android.Runtime;
using Android.Views;
#endif

#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.Maui.Handlers;
using Windows.System;
#endif

namespace Maui.HassWebView.Core;

public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// 使用 HassWebView 控件
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MauiAppBuilder UseHassWebView(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            // 使用非泛型重载，避免在编译期对 handler 类型做 IElementHandler 约束检查
            // handlers.AddHandler(typeof(HassWebView), typeof(HassWebViewHandler));
#if ANDROID
            handlers.AddHandler(typeof(HassWebView), typeof(Platforms.Android.HassWebViewHandler));
#elif WINDOWS
            handlers.AddHandler(typeof(HassWebView), typeof(Platforms.Windows.HassWebViewHandler));
#endif
        });

        builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID

                
                events.AddAndroid(android =>
                {
                    android.OnApplicationCreate(app =>
                    {
                        // 在调用TBS初始化、创建WebView之前进行如下配置
                        QbSdk.InitTbsSettings(new Dictionary<string, Java.Lang.Object>
                        {
                            { TbsCoreSettings.TbsSettingsUseSpeedyClassloader, true },
                            { TbsCoreSettings.TbsSettingsUseDexloaderService, true }
                        });
                    });

                    // Hook into the main activity's creation
                    android.OnCreate((activity, bundle) =>
                    {

                        QbSdk.DownloadWithoutWifi = true;

                        var tbsListener = new TencentTbsListener();
                        tbsListener.DownloadProgress += (s, e) =>
                        {
                            Console.WriteLine($"DownloadProgress {e}");
                        };
                        tbsListener.DownloadFinished += (s, e) =>
                        {
                            Console.WriteLine($"DownloadFinished {e}");
                        };
                        tbsListener.InstallFinished += (s, e) =>
                        {
                            Console.WriteLine($"InstallFinished {e}");
                        };

                        var preInitCallback = new PreInitCallback();
                        preInitCallback.CoreInitFinished += (s, e) =>
                        {
                            Console.WriteLine($"CoreInitFinished {e}");
                        };
                        preInitCallback.ViewInitFinished += (s, e) =>
                        {
                            Console.WriteLine($"ViewInitFinished {e}");
                        };
                        QbSdk.SetTbsListener(tbsListener);

                        Console.WriteLine("InitX5Environment");
                        QbSdk.InitX5Environment(activity, preInitCallback);
                    });
                });
#endif
            });

        return builder;
    }

    /// <summary>
    /// 开启沉浸式模式（隐藏系统栏）
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MauiAppBuilder UseImmersiveMode(this MauiAppBuilder builder)
    {

#if ANDROID
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddAndroid(android =>
            {
                android.OnCreate((activity, bundle) =>
                {
                    var window = activity.Window;

                    // 内容延伸到状态栏和导航栏
                    WindowCompat.SetDecorFitsSystemWindows(window, false);

                    var controller = WindowCompat.GetInsetsController(window, window.DecorView);
                    if (controller != null)
                    {
                        // 隐藏状态栏与导航栏
                        controller.Hide(WindowInsetsCompat.Type.StatusBars());
                        controller.Hide(WindowInsetsCompat.Type.NavigationBars());

                        // 相当于 ImmersiveSticky
                        controller.SystemBarsBehavior =
                            WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
                    }
                });
            });
        });
#endif
        return builder;
    }

    /// <summary>
    /// 监听遥控器按键
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="longPressTimeout"></param>
    /// <param name="doubleClickTimeout"></param>
    /// <returns></returns>
    public static MauiAppBuilder UseRemoteControl(
        this MauiAppBuilder builder,
        int longPressTimeout = 750,
        int doubleClickTimeout = 100)
    {
        // 1. Register KeyService as a singleton with the provided timings
        builder.Services.AddSingleton(new KeyService(longPressTimeout, doubleClickTimeout));

        builder.ConfigureLifecycleEvents(events =>
        {
#if ANDROID
            events.AddAndroid(android =>
            {
                android.OnCreate((activity, bundle) =>
                {
                    var keyService = (activity.Application as MauiApplication)?.Services.GetService<KeyService>();
                    if (keyService == null)
                    {
                        Debug.WriteLine("KeyService not found. Make sure it's registered.");
                        return;
                    }

                    var window = activity.Window;
                    var originalCallback = window.Callback;
                    window.Callback = new Platforms.Android.KeyCallback(originalCallback, keyService);
                });
            });
#endif
        });

#if WINDOWS
            WindowHandler.Mapper.AppendToMapping("RemoteControl", (handler, view) =>
            {
                var keyService = handler.MauiContext?.Services.GetService<KeyService>();
                if (keyService == null) return;
                if (handler.PlatformView.Content is not UIElement ui) return;
                
                ui.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler((s, e) =>
                {
                    // --- THE NEW, CORRECT LOGIC ---
                    // 1. Call OnPressed and get its boolean result.
                    bool shouldBeHandled = keyService.OnPressed(e.Key.ToString());
                    
                    // 2. Assign the result directly to e.Handled.
                    e.Handled = shouldBeHandled;

                }), true);

                ui.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler((s, e) =>
                {
                    // KeyUp doesn't need to be conditional, as it only signals release.
                    // But we should still prevent it from bubbling up if the keydown was handled.
                    keyService.OnReleased();

                    // This part is tricky. We don't know if the original KeyDown was handled here.
                    // For simplicity and to avoid unintended side effects, we'll mark it as handled
                    // if it's not a key we'd typically let the system handle (like Volume keys).
                    
                }), true);
            });
#endif

        return builder;
    }




}