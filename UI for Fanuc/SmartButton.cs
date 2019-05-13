﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace UI_for_Fanuc
{
    class SmartButton : Button
    {
        
    private DispatcherTimer _timer;


        public int MillisecondsToWait
        {
            get { return (int)GetValue(MillisecondsToWaitProperty); }
            set { SetValue(MillisecondsToWaitProperty, value); }
        }

        public DispatcherTimer Timer
        {
            get { return _timer; }
            set { _timer = value; }
        }



        public ICommand ClickAndHoldCommand
        {
            get { return (ICommand)GetValue(ClickAndHoldCommandProperty); }
            set { SetValue(ClickAndHoldCommandProperty, value); }
        }


        public bool EnableClickHold
        {
            get { return (bool)GetValue(EnableClickHoldProperty); }
            set { SetValue(EnableClickHoldProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableClickHold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableClickHoldProperty =
            DependencyProperty.Register("EnableClickHold", typeof(bool), typeof(SmartButton), new PropertyMetadata(false));



        // Using a DependencyProperty as the backing store for ClickAndHoldCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickAndHoldCommandProperty =
            DependencyProperty.Register("ClickAndHoldCommand", typeof(ICommand), typeof(SmartButton), new UIPropertyMetadata(null));



        // Using a DependencyProperty as the backing store for MillisecondsToWait.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MillisecondsToWaitProperty =
            DependencyProperty.Register("MillisecondsToWait", typeof(int), typeof(SmartButton), new PropertyMetadata(0));


        public SmartButton()
        {
            this.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            this.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;

        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (EnableClickHold)
            {

                bool isMouseReleaseBeforeHoldTimeout = Timer.IsEnabled;
                ResetAndRemoveTimer();
                // Consider it as a mouse click 
                if (isMouseReleaseBeforeHoldTimeout)
                {
                    Command.Execute(CommandParameter);
                }
                e.Handled = true;
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (EnableClickHold)
            {
                Timer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher)
                {
                    Interval = TimeSpan.FromMilliseconds(MillisecondsToWait)
                };
                Timer.Tick += Timer_Tick;
                Timer.IsEnabled = true;
                Timer.Start();
                e.Handled = true;
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            this.ClickAndHoldCommand.Execute(this.CommandParameter);
            ResetAndRemoveTimer();
        }

        private void ResetAndRemoveTimer()
        {
            if (Timer == null) return;
            Timer.Tick -= Timer_Tick;
            Timer.IsEnabled = false;
            Timer.Stop();
            Timer = null;
        }
    }

}
