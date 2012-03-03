using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
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
        static Process endPointController = new Process();

        static void Main()
        {
            CreateTray();
            Application.Run();
        }

        static private string[,] GetDevices()
        {
            string[,] deviceList; //DeviceNumber,DeviceName
            string tempString;
            string[] tempArray;

            endPointController.StartInfo = new ProcessStartInfo("Externals\\EndPointController.exe")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            endPointController.Start();
            tempString = endPointController.StandardOutput.ReadToEnd().ToString();
            tempArray = new string[tempString.Split(new string[1] { "\n" }, StringSplitOptions.None).Length];
            tempArray = tempString.Split(new string[1] { "\n" }, StringSplitOptions.None);

            deviceList = new string[tempArray.Length - 1, 2]; //length -1 because we have an extra line that we don't need.

            for (int i = 0; i < tempArray.Length - 1; i++)
            {
                deviceList[i, 0] = tempArray[i].Split(new string[2] { " ", ":" }, StringSplitOptions.None)[2];
                deviceList[i, 1] = tempArray[i].Split(new string[1] { ":" }, StringSplitOptions.None)[1].Trim();
            }
            return deviceList;
        }

        static private void SetDevice(string deviceID)
        {
            endPointController.StartInfo = new ProcessStartInfo("Externals\\EndPointController.exe", deviceID)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            endPointController.Start();
            
        }

        static private void CreateTray()
        {
            trayIcon.ContextMenu = trayMenu;
            trayMenu.Popup += new EventHandler(trayMenu_Popup);          
        }

        static void trayMenu_Popup(object sender, EventArgs e)
        {
            string[,] deviceList = GetDevices();
            MMDevice defaultDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

            trayMenu.MenuItems.Clear();

            trayMenu.MenuItems.Add("Current: " + defaultDevice.FriendlyName + " (" + defaultDevice.DeviceFriendlyName + ")");
            trayMenu.MenuItems.Add("-");
            for (int i = 0; i < deviceList.GetLength(0); i++)
            {
                int localI = i; //We create a local int so that we don't have problems with the delegate inside the for loop.
                trayMenu.MenuItems.Add("[" + deviceList[i, 0] + "] " + deviceList[i, 1], (f, g) => SetDevice(deviceList[localI, 0]));
            }
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Exit", (f, g) => Application.Exit());
        }
    }
}
