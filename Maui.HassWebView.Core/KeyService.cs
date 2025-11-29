using System.Timers;

namespace Maui.HassWebView.Core
{
    public class KeyService
    {
        public event Action<string> SingleClick;
        public event Action<string> DoubleClick;
        public event Action<string> LongClick;
        public event Action<string> Down;

        private readonly System.Timers.Timer _longPressTimer;
        private readonly System.Timers.Timer _clickTimer;
        private readonly System.Timers.Timer _downTimer;

        private readonly object _lock = new object();

        private string _currentKeyName;
        private bool _isLongPress = false;
        private int _pressCount = 0;

        public KeyService(int longPressTimeout = 750, int doubleClickTimeout = 300, int downInterval = 100)
        {
            _longPressTimer = new System.Timers.Timer(longPressTimeout);
            _longPressTimer.Elapsed += OnLongPressTimerElapsed;
            _longPressTimer.AutoReset = false;

            _clickTimer = new System.Timers.Timer(doubleClickTimeout);
            _clickTimer.Elapsed += OnClickTimerElapsed;
            _clickTimer.AutoReset = false;

            _downTimer = new System.Timers.Timer(downInterval);
            _downTimer.Elapsed += OnDownTimerElapsed;
            _downTimer.AutoReset = true;
        }

        private void OnDownTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_pressCount > 0)
                {
                    Down?.Invoke(_currentKeyName);
                }
            }
        }

        internal void OnPressed(string keyName)
        {
            lock (_lock)
            {
                // 忽略长按过程中的按键重复事件
                if (_pressCount > 0 && !_clickTimer.Enabled)
                {
                    return;
                }

                _currentKeyName = keyName;
                _pressCount++;

                _clickTimer.Stop();

                if (_pressCount == 1) // 第一次按下
                {
                    _isLongPress = false;
                    _longPressTimer.Start();
                    _downTimer.Start();
                }
                else if (_pressCount == 2) // 第二次按下 (用于双击)
                {
                    _longPressTimer.Stop();
                    _downTimer.Stop();
                    DoubleClick?.Invoke(_currentKeyName);
                    _pressCount = 0;
                }
            }
        }

        internal void OnReleased()
        {
            lock (_lock)
            {
                _longPressTimer.Stop();
                _downTimer.Stop();

                // 如果已经触发了长按，则重置所有状态并立即返回
                if (_isLongPress)
                {
                    _isLongPress = false;
                    _pressCount = 0;
                    return;
                }

                if (_pressCount == 1)
                {
                    // 启动计时器以区分单击和双击
                    _clickTimer.Start();
                }
            }
        }

        private void OnLongPressTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_pressCount == 1)
                {
                    _isLongPress = true; // 标记已发生长按
                    LongClick?.Invoke(_currentKeyName);
                    // 注意：此处不再重置 _pressCount，交由 OnReleased 处理
                }
            }
        }

        private void OnClickTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_pressCount == 1)
                {
                    SingleClick?.Invoke(_currentKeyName);
                }
                _pressCount = 0; // 重置状态
            }
        }
    }
}