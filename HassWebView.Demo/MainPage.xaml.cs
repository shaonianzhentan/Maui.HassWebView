using HassWebView.Core;
using System.Diagnostics;
using System;

namespace HassWebView.Demo
{
    public partial class MainPage : ContentPage
    {
        private readonly KeyService _keyService;
        CursorControl cursorControl;

        public MainPage(KeyService keyService)
        {
            InitializeComponent();
            _keyService = keyService; // Store the injected service

            wv.Navigating += Wv_Navigating;
            wv.Navigated += Wv_Navigated;
            Loaded += MainPage_Loaded;
            cursorControl = new CursorControl(cursor, root, wv);
        }

        private void Wv_Navigating(object sender, WebNavigatingEventArgs e)
        {
            Debug.WriteLine($">>> WebView Navigating: {e.Url}");
            try
            {
                var host = new Uri(e.Url).Host;
                if (host.EndsWith("youtube.com"))
                {
                    wv.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
                }
                else if (host.EndsWith("bilibili.com"))
                {
                    wv.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Safari/537.36";
                }
            }
            catch (Exception)
            {
                // Invalid URL
            }
        }

        private void Wv_Navigated(object sender, WebNavigatedEventArgs e)
        {
            Debug.WriteLine($">>> WebView Navigated to: {e.Url}");
        }

        private void MainPage_Loaded(object? sender, EventArgs e)
        {

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("MainPage Appearing: Subscribing to KeyService events.");
            _keyService.SingleClick += OnSingleClick;
            _keyService.DoubleClick += OnDoubleClick;
            _keyService.LongClick += OnLongClick;
            _keyService.KeyUp += OnKeyUp; // <-- SUBSCRIBED to KeyUp event
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Debug.WriteLine("MainPage Disappearing: Unsubscribing from KeyService events.");
            _keyService.SingleClick -= OnSingleClick;
            _keyService.DoubleClick -= OnDoubleClick;
            _keyService.LongClick -= OnLongClick;
            _keyService.KeyUp -= OnKeyUp; // <-- UNSUBSCRIBED from KeyUp event
            wv.Navigating -= Wv_Navigating;
            wv.Navigated -= Wv_Navigated;
        }

        private void OnSingleClick(RemoteKeyEventArgs e)
        {

            MainThread.BeginInvokeOnMainThread(async () =>
            {

                switch (e.KeyName)
                {
                    // OK / Enter Key
                    case "Enter":
                    case "DpadCenter":
                        if (wv.IsVideoFullscreen)
                        {
                            cursorControl.VideoPlayPause();
                        }
                        else
                        {
                                cursorControl.Click();
                        }
                        break;

                    // Back / Escape Key
                    case "Escape":
                    case "Back":
                        
                            if (wv.IsVideoFullscreen)
                                wv.ExitFullscreen();
                            else if (wv.CanGoBack)
                                wv.GoBack();
                        break;

                    // Directional Keys (Up, Down, Left, Right)
                    case "Up":
                    case "DpadUp":
                        cursorControl.MoveUpBy();
                        break;

                    case "Down":
                    case "DpadDown":
                        cursorControl.MoveDownBy();
                        break;

                    case "Left":
                    case "DpadLeft":
                        
                            if (wv.IsVideoFullscreen)
                                cursorControl.VideoSeek(-5);
                            else
                                cursorControl.MoveLeftBy();
                        break;

                    case "Right":
                    case "DpadRight":
                        
                            if (wv.IsVideoFullscreen)
                                cursorControl.VideoSeek(5);
                            else
                                cursorControl.MoveRightBy();
                        break;

                }
            });
        }

        private void OnDoubleClick(RemoteKeyEventArgs e)
        {

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                switch (e.KeyName)
                {
                    // OK / Enter Key
                    case "Enter":
                    case "DpadCenter":

                        await cursorControl.DoubleClick();
                        break;

                    // Directional Keys (Up, Down, Left, Right)
                    case "Up":
                    case "DpadUp":
                        cursorControl.SlideUp();
                        break;

                    case "Down":
                    case "DpadDown":
                        cursorControl.SlideDown();
                        break;

                    case "Left":
                    case "DpadLeft":
                        cursorControl.SlideLeft();                        break;

                    case "Right":
                    case "DpadRight":
                        cursorControl.SlideRight();
                        break;
                }
            });
        }

        // --- MODIFIED: OnLongClick now starts a repeating action ---
        private void OnLongClick(RemoteKeyEventArgs e)
        {
            if (e.KeyName == "VolumeUp" || e.KeyName == "VolumeDown")
            {
                e.Handled = false;
                return;
            }

            Debug.WriteLine($"--- OnLongClick: {e.KeyName} ---");

            // The interval for repeating the action, in milliseconds.
            int repeatInterval = 100; 

            switch (e.KeyName)
            {
                case "Up":
                case "DpadUp":
                    _keyService.StartRepeatingAction(() => cursorControl.MoveUpBy(), repeatInterval);
                    break;
                case "Down":
                case "DpadDown":
                    _keyService.StartRepeatingAction(() => cursorControl.MoveDownBy(), repeatInterval);
                    break;
                case "Left":
                case "DpadLeft":
                    if (wv.IsVideoFullscreen) {
                        cursorControl.VideoSeek(-15);
                    } else {
                        _keyService.StartRepeatingAction(() => cursorControl.MoveLeftBy(), repeatInterval);
                    }
                    break;
                case "Right":
                case "DpadRight":
                     if (wv.IsVideoFullscreen) {
                        cursorControl.VideoSeek(15);
                    } else {
                        _keyService.StartRepeatingAction(() => cursorControl.MoveRightBy(), repeatInterval);
                    }
                    break;
            }
        }

        private void OnKeyUp(RemoteKeyEventArgs e)
        {
            Debug.WriteLine($"--- OnKeyUp: {e.KeyName} ---");
            // The KeyService automatically stops the repeating action on KeyUp.
        }
    }
}
