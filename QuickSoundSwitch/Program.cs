using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices; //allows our interop code
using NAudio;
using NAudio.CoreAudioApi;

namespace QuickSoundSwitch
{
    class Program
    {
        static NotifyIcon trayIcon = new NotifyIcon
        {
            Icon = Icon.FromHandle(Properties.Resources.QuickSoundSwitch.Handle),
            Visible = true,
            Text = "QuickSoundSwitch"
        };
        static ContextMenu trayMenu = new ContextMenu();

        static void Main()
        {
            CreateTray();
            Application.Run();
        }

        static AudioDevice[] GetDevices()
        {
            AudioDevice[] deviceList = new AudioDevice[GetDeviceCount()];

            for (int i = 0; i < GetDeviceCount(); i++)
            {
                deviceList[i] = new AudioDevice(i);
            }

            return deviceList;
        }

        static void CreateTray()
        {
            trayIcon.ContextMenu = trayMenu;
            trayMenu.Popup += new EventHandler(trayMenu_Popup);      
        }

        static void trayMenu_Popup(object sender, EventArgs e)
        {
            AudioDevice[] deviceList = GetDevices();
            MMDevice defaultDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

            trayMenu.MenuItems.Clear();

            trayMenu.MenuItems.Add("Current: " + defaultDevice.FriendlyName + " (" + defaultDevice.DeviceFriendlyName + ")");
            trayMenu.MenuItems.Add("-");

            for (int i = 0; i < deviceList.Length; i++)
            {
                int localI = i; //We create a local int so that we don't have problems with the delegate inside the for loop.
                trayMenu.MenuItems.Add(/*"[" + (deviceList[i].DeviceNum+1) + "] " + */deviceList[i].DeviceName, (f, g) => deviceList[localI].SetDefault());
            }

            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Exit", (f, g) => Application.Exit());
        }

        [DllImport("AudioEndPointController.dll", EntryPoint = "?GetDeviceCount@@YAHXZ", CallingConvention = CallingConvention.Cdecl)] //Get the entrypoint using dumpbin.exe /exports.
        static extern int GetDeviceCount();
    }
}
