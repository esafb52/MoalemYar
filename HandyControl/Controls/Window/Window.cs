﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using HandyControl.Data;
using HandyControl.Tools;
#if netle40
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
#endif

namespace HandyControl.Controls
{
    public class Window : System.Windows.Window
    {
        private Thickness _tempThickness;

        public static readonly DependencyProperty NonClientAreaContentProperty = DependencyProperty.Register(
            "NonClientAreaContent", typeof(object), typeof(Window), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty CloseButtonHoverBackgroundProperty = DependencyProperty.Register(
            "CloseButtonHoverBackground", typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty CloseButtonHoverForegroundProperty =
            DependencyProperty.Register(
                "CloseButtonHoverForeground", typeof(Brush), typeof(Window),
                new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty CloseButtonBackgroundProperty = DependencyProperty.Register(
            "CloseButtonBackground", typeof(Brush), typeof(Window), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty CloseButtonForegroundProperty = DependencyProperty.Register(
            "CloseButtonForeground", typeof(Brush), typeof(Window),
            new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty OtherButtonBackgroundProperty = DependencyProperty.Register(
            "OtherButtonBackground", typeof(Brush), typeof(Window), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty OtherButtonForegroundProperty = DependencyProperty.Register(
            "OtherButtonForeground", typeof(Brush), typeof(Window),
            new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty OtherButtonHoverBackgroundProperty = DependencyProperty.Register(
            "OtherButtonHoverBackground", typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty OtherButtonHoverForegroundProperty =
            DependencyProperty.Register(
                "OtherButtonHoverForeground", typeof(Brush), typeof(Window),
                new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty NonClientAreaBackgroundProperty = DependencyProperty.Register(
            "NonClientAreaBackground", typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty NonClientAreaForegroundProperty = DependencyProperty.Register(
            "NonClientAreaForeground", typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty NonClientAreaHeightProperty = DependencyProperty.Register(
            "NonClientAreaHeight", typeof(double), typeof(Window),
            new PropertyMetadata(28.0));

        public static readonly DependencyProperty ShowNonClientAreaProperty = DependencyProperty.Register(
            "ShowNonClientArea", typeof(bool), typeof(Window), new PropertyMetadata(ValueBoxes.TrueBox));

        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(
            "ShowTitle", typeof(bool), typeof(Window), new PropertyMetadata(ValueBoxes.FalseBox));

        public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register(
            "IsFullScreen", typeof(bool), typeof(Window), new PropertyMetadata(ValueBoxes.FalseBox,
                (o, args) =>
                {
                    var ctl = (Window)o;
                    var v = (bool)args.NewValue;
                    if (v)
                    {
                        ctl.OriginState = ctl.WindowState;
                        ctl.OriginStyle = ctl.WindowStyle;
                        ctl.OriginResizeMode = ctl.ResizeMode;
                        ctl.WindowStyle = WindowStyle.None;
                        //下面三行不能改变，就是故意的
                        ctl.WindowState = WindowState.Maximized;
                        ctl.WindowState = WindowState.Minimized;
                        ctl.WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        ctl.WindowState = ctl.OriginState;
                        ctl.WindowStyle = ctl.OriginStyle;
                        ctl.ResizeMode = ctl.OriginResizeMode;
                    }
                }));

        public Window()
        {
            var chrome = new WindowChrome
            {
                CornerRadius = new CornerRadius(),
                GlassFrameThickness = new Thickness(1)
            };
            BindingOperations.SetBinding(chrome, WindowChrome.CaptionHeightProperty,
                new Binding(NonClientAreaHeightProperty.Name) {Source = this});
            WindowChrome.SetWindowChrome(this, chrome);

            Loaded += delegate
            {
                _tempThickness = BorderThickness;
                if (WindowState == WindowState.Maximized)
                {
                    BorderThickness = new Thickness();
                }

                CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand,
                    (s, e) => WindowState = WindowState.Minimized));
                CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand,
                    (s, e) => WindowState = WindowState.Maximized));
                CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand,
                    (s, e) => WindowState = WindowState.Normal));
                CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (s, e) => Close()));
                CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));
            };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Maximized)
            {
                BorderThickness = new Thickness();
            }
            else if (WindowState == WindowState.Normal)
            {
                BorderThickness = _tempThickness;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (SizeToContent != SizeToContent.WidthAndHeight)
                return;

            SizeToContent = SizeToContent.Height;
            Dispatcher.BeginInvoke(new Action(() => { SizeToContent = SizeToContent.WidthAndHeight; }));
        }

        public Brush CloseButtonBackground
        {
            get => (Brush)GetValue(CloseButtonBackgroundProperty);
            set => SetValue(CloseButtonBackgroundProperty, value);
        }

        public Brush CloseButtonForeground
        {
            get => (Brush)GetValue(CloseButtonForegroundProperty);
            set => SetValue(CloseButtonForegroundProperty, value);
        }

        public Brush OtherButtonBackground
        {
            get => (Brush)GetValue(OtherButtonBackgroundProperty);
            set => SetValue(OtherButtonBackgroundProperty, value);
        }

        public Brush OtherButtonForeground
        {
            get => (Brush)GetValue(OtherButtonForegroundProperty);
            set => SetValue(OtherButtonForegroundProperty, value);
        }

        /// <summary>
        ///     原始状态
        /// </summary>
        private WindowState OriginState { get; set; }

        /// <summary>
        ///     原始样式
        /// </summary>
        private WindowStyle OriginStyle { get; set; }

        /// <summary>
        ///     原始尺寸调节模式
        /// </summary>
        private ResizeMode OriginResizeMode { get; set; }

        public double NonClientAreaHeight
        {
            get => (double)GetValue(NonClientAreaHeightProperty);
            set => SetValue(NonClientAreaHeightProperty, value);
        }

        public bool IsFullScreen
        {
            get => (bool)GetValue(IsFullScreenProperty);
            set => SetValue(IsFullScreenProperty, value);
        }

        public object NonClientAreaContent
        {
            get => GetValue(NonClientAreaContentProperty);
            set => SetValue(NonClientAreaContentProperty, value);
        }

        public Brush CloseButtonHoverBackground
        {
            get => (Brush)GetValue(CloseButtonHoverBackgroundProperty);
            set => SetValue(CloseButtonHoverBackgroundProperty, value);
        }

        public Brush CloseButtonHoverForeground
        {
            get => (Brush)GetValue(CloseButtonHoverForegroundProperty);
            set => SetValue(CloseButtonHoverForegroundProperty, value);
        }

        public Brush OtherButtonHoverBackground
        {
            get => (Brush)GetValue(OtherButtonHoverBackgroundProperty);
            set => SetValue(OtherButtonHoverBackgroundProperty, value);
        }

        public Brush OtherButtonHoverForeground
        {
            get => (Brush)GetValue(OtherButtonHoverForegroundProperty);
            set => SetValue(OtherButtonHoverForegroundProperty, value);
        }

        public Brush NonClientAreaBackground
        {
            get => (Brush)GetValue(NonClientAreaBackgroundProperty);
            set => SetValue(NonClientAreaBackgroundProperty, value);
        }

        public Brush NonClientAreaForeground
        {
            get => (Brush)GetValue(NonClientAreaForegroundProperty);
            set => SetValue(NonClientAreaForegroundProperty, value);
        }

        public bool ShowNonClientArea
        {
            get => (bool)GetValue(ShowNonClientAreaProperty);
            set => SetValue(ShowNonClientAreaProperty, value);
        }

        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            var point = WindowState == WindowState.Maximized
                ? new Point(0, 28)
                : new Point(Left, Top + 28);
            SystemCommands.ShowSystemMenu(this, point);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
                InvalidateMeasure();
        }

        /// <summary>
        ///     获取自定义窗口
        /// </summary>
        /// <returns></returns>
        public static Window GetCustomWindow(FrameworkElement content)
        {
            var window = new Window
            {
                Style = ResourceHelper.GetResource<Style>(ResourceToken.WindowWin10),
                Content = content
            };
            window.Loaded += (s, e) =>
            {
                window.Width = window.BorderThickness.Left + window.BorderThickness.Right + content.Width;
                if (!(window.Template.FindName("GridMenu", window) is Grid nemuArea))
                    throw new NullReferenceException("can not find GridMenu in template");
                window.Height = window.BorderThickness.Top + window.BorderThickness.Bottom + content.Height +
                                nemuArea.ActualHeight;
            };

            return window;
        }
    }
}