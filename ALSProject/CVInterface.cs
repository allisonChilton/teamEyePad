using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;
using EyeXFramework;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
namespace ALSProject
{

    //this translates the Tobii SDK to calls for our system

    //Tobii SDK: http://developer.tobii.com/documentation/
    class CVInterface
    {
        public static System.Timers.Timer timer;

        protected static bool cantStopDontStop = true;
        protected static bool AutoAlarm = false;

        #region Constructors
        public CVInterface()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += Timer_Tick;
        }
        #endregion

        #region Public Methods
        public void StartEyeTracking()
        {

            using (var eyeXHost = new EyeXHost())
            {
                // Create a data stream: lightly filtered gaze point data.
                // Other choices of data streams include EyePositionDataStream and FixationDataStream.
                using (var fixationGazeDataStream = eyeXHost.CreateFixationDataStream(FixationDataMode.Slow))
                {
                    // Start the EyeX host.
                    eyeXHost.Start();
                    eyeXHost.EyeTrackingDeviceStatusChanged += EyeXHost_EyeTrackingDeviceStatusChanged;
                    eyeXHost.UserPresenceChanged += EyeXHost_UserPresenceChanged;
                    //eyeXHost.CreateEyePositionDataStream
                    double smoothX = 0;
                    double smoothY = 0;
                    double box = 35;

                    // Write the data to the console.
                    fixationGazeDataStream.Next += (s, e) =>
                    {
                        if (e.X > smoothX + box || e.X < smoothX - box || e.Y > smoothY + box || e.Y < smoothY - box)
                        {
                            Cursor.Position = new Point(Convert.ToInt32(e.X), Convert.ToInt32(e.Y));
                            smoothX = e.X;
                            smoothY = e.Y;
                        }
                        else
                        {
                            Cursor.Position = new Point(Convert.ToInt32(smoothX), Convert.ToInt32(smoothY));
                        }
                    };
                    while (cantStopDontStop) { Thread.Sleep(1000); }
                }
            }
        }

        public static void SetAutoAlarm(bool isAutoAlarm)
        {
            AutoAlarm = isAutoAlarm;
        }

        public static bool GetAutoAlarm()
        {
            return AutoAlarm;
        }

        public static void PleaseStop()
        {
            cantStopDontStop = false;
        }
        #endregion

        #region Events
        private void Timer_Tick(object sender, EventArgs e)
        {
            ALSAlarm alarm = new ALSAlarm();
            if (!alarm.isAlarmOn())
            {
                alarm.PerformClick();
            }
            timer.Stop();
        }

        private void EyeXHost_UserPresenceChanged(object sender, EngineStateValue<UserPresence> e)
        {
            try
            {
                if (AutoAlarm && e.Value.Equals(UserPresence.NotPresent))
                {
                    StartTimer();
                }
                else
                {
                    CVInterface.timer.Stop();
                }
            }
            catch (Exception) { }

        }

        private void EyeXHost_EyeTrackingDeviceStatusChanged(object sender, EngineStateValue<EyeTrackingDeviceStatus> e)
        {
            if (AutoAlarm && e.Value.Equals(EyeTrackingDeviceStatus.DeviceNotConnected))
            {
                ALSAlarm alarm = new ALSAlarm();
                if (!alarm.isAlarmOn())
                {
                    alarm.PerformClick();
                }
            }
        }
        #endregion

        #region Private Methods
        private void StartTimer()
        {
            timer.Start();
        }
        #endregion
    }
}

