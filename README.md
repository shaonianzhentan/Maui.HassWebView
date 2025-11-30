# HassWebView
适配 Home Assistant 的 MAUI WebView 控件


MauiProgram.cs
```cs
using HassWebView.Core;

builder
.UseHassWebView()
.UseImmersiveMode() // 可选
.UseRemoteControl();  // 可选
```
