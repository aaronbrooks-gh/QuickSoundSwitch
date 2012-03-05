using System;
using System.Runtime.InteropServices; //allows our interop code

namespace QuickSoundSwitch
{
    class AudioDevice
    {
        private int deviceNum;
        private UnmanagedType deviceID;
        private string deviceName;

        public int DeviceNum
        {
            get
            {
                return deviceNum;
            }
        }
        public string DeviceName
        {
            get
            {
                return deviceName;
            }
        }

        public AudioDevice(int deviceNum)
        {
            this.deviceNum = deviceNum;
            this.deviceID = GetDeviceID(this.deviceNum);
            this.deviceName = GetDeviceFriendlyName(this.deviceNum);
        }

        public void SetDefault()
        {
            SetDefaultAudioPlaybackDevice(deviceID);
        }

        [DllImport("AudioEndPointController.dll", EntryPoint = "?SetDefaultAudioPlaybackDevice@@YAJPB_W@Z", CallingConvention = CallingConvention.Cdecl)]
        static extern Int32 SetDefaultAudioPlaybackDevice(UnmanagedType deviceID);

        [DllImport("AudioEndPointController.dll", EntryPoint = "?GetDeviceFriendlyName@@YAPA_WH@Z", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        static extern string GetDeviceFriendlyName(int deviceNum);

        [DllImport("AudioEndPointController.dll", EntryPoint = "?GetDeviceID@@YAPB_WH@Z", CallingConvention = CallingConvention.Cdecl)]
        static extern UnmanagedType GetDeviceID(int deviceNum);
    }
}
