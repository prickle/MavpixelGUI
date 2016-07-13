/*
    BitBurner AVR Programmer - a frontend for AVRdude
    Copyright (C) 2012 Nick Metcalfe

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Security;
using System.Runtime.ConstrainedExecution;
using System.ComponentModel;

//Scan for comports - returns list of port names and usable descriptions

namespace MavpixelGUI
{
    public class cbxPortEntry : IComparable
    {
        public string Name;
        public string Description;
        public cbxPortEntry(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public override string ToString()
        {
            return Name;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            cbxPortEntry cbxObj = (cbxPortEntry)obj;
            return string.Compare(this.Name, cbxObj.Name);
        }

        #endregion
    }

    internal class Ports
    {

        BackgroundWorker comPortWorker;
        public Ports()
        {
            comPortWorker = new BackgroundWorker();

        }

        RunWorkerCompletedEventHandler oldRunCompleted = null;
        public void ScanDevices(RunWorkerCompletedEventHandler runWorkerCompleted)
        {
            comPortWorker.DoWork += new DoWorkEventHandler(comPortWorker_DoWork);
            if (oldRunCompleted != null) comPortWorker.RunWorkerCompleted -= oldRunCompleted;
            comPortWorker.RunWorkerCompleted += oldRunCompleted = new RunWorkerCompletedEventHandler(runWorkerCompleted);
            comPortWorker.RunWorkerAsync();
        }

        private void comPortWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<cbxPortEntry> result = new List<cbxPortEntry>();
            string[] comPorts = Ports.GetComPorts();
            foreach (string port in comPorts)
            {
                int start = port.LastIndexOf('(');
                int end = port.LastIndexOf(')');
                string designator = port.Substring(start + 1, end - start - 1).Trim();
                if (!designator.EndsWith(":")) designator += ":";
                string description = port.Substring(0, start - 1).Trim();
                result.Add(new cbxPortEntry(designator, description));
            }
            result.Sort(new AlphanumComparator());
            e.Result = result;
        }

        [DllImport("setupapi.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SetupDiGetClassDevs([In()]
            ref Guid classGuid, [MarshalAs(UnmanagedType.LPWStr)]
            string enumerator, IntPtr hwndParent, SetupDiGetClassDevsFlags flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, DeviceInfoData deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr DeviceInfoSet,
            DeviceInfoData DeviceInfoData,
            uint Property,
            out UInt32 PropertyRegDataType,
            byte[] PropertyBuffer,
            uint PropertyBufferSize,
            out UInt32 RequiredSize
            );

        [DllImport("setupapi.dll")]
        internal static extern uint SetupDiDestroyDeviceInfoList(
            IntPtr deviceInfoSet);

        internal enum SetupDiGetClassDevsFlags
        {
            Default = 1,
            Present = 2,
            AllClasses = 4,
            Profile = 8,
            DeviceInterface = (int)0x10
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class DeviceInfoData
        {
            public int Size;
            public Guid ClassGuid;
            public int DevInst;
            public IntPtr Reserved;
        }

        internal const string GUID_DEVCLASS_PORTS = "4d36e978-e325-11ce-bfc1-08002be10318";
        static uint SPDRP_FRIENDLYNAME = 0x0000000C;
        
        public static string[] GetComPorts()
        {
            List<string> Result = new List<string>();
            Guid guid = new Guid(GUID_DEVCLASS_PORTS);
            uint nDevice = 0;
            uint nBytes = 300;
            byte[] retval = new byte[nBytes];
            uint RequiredSize = 0;
            uint PropertyRegDataType = 0;

            DeviceInfoData devInfoData = new DeviceInfoData();
            devInfoData.Size = Marshal.SizeOf(typeof(DeviceInfoData));

            IntPtr hDeviceInfo = SetupDiGetClassDevs(
                ref guid,
                null,
                IntPtr.Zero,
                SetupDiGetClassDevsFlags.Present);

            while (SetupDiEnumDeviceInfo(hDeviceInfo, nDevice++, devInfoData))
            {
                string val = null;
                if (SetupDiGetDeviceRegistryProperty(
                        hDeviceInfo,
                        devInfoData,
                        SPDRP_FRIENDLYNAME,
                        out PropertyRegDataType,
                        retval,
                        nBytes,
                        out RequiredSize))
                {
                    int i;
                    val = System.Text.Encoding.Unicode.GetString(retval);
                    for (i = 0; i < val.Length && val[i] != '\0'; i++) { }
                    Result.Add(val = val.Substring(0, i));
                }
            }
            //int err = Marshal.GetLastWin32Error();
            SetupDiDestroyDeviceInfoList(hDeviceInfo);

            return Result.ToArray();
        }

        /*[DllImport("setupapi.dll")]
        internal static extern Int32 SetupDiClassNameFromGuid(ref Guid ClassGuid, StringBuilder className, Int32 ClassNameSize, ref Int32 RequiredSize);

        static String GetClassNameFromGuid(Guid guid)
        {
            StringBuilder strClassName = new StringBuilder(0);
            Int32 iRequiredSize = 0;
            Int32 iSize = 0;
            Int32 iRet = SetupDiClassNameFromGuid(ref guid, strClassName, iSize, ref iRequiredSize);
            strClassName = new StringBuilder(iRequiredSize);
            iSize = iRequiredSize;
            iRet = SetupDiClassNameFromGuid(ref guid, strClassName, iSize, ref iRequiredSize);
            if (iRet == 1)
            {
                return strClassName.ToString();
            }

            return String.Empty;
        }*/



    }
}

