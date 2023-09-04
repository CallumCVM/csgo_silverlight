using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace csgo_silverlight_free
{
    public class Mem
    {
        

        public static byte[] Read(int adr, int l)
        {
            byte[] lpBuffer = new byte[(l - 1) + 1];

            try
            {                
                int lpNumberOfBytesRead = 1;
                if (!MainPage.ReadProcessMemory(MainPage.csgoHandle, (IntPtr)adr, lpBuffer, lpBuffer.Length, ref lpNumberOfBytesRead))
                    throw new Exception("Memory read error: " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
            return lpBuffer;
        }
        public static object ReadBool(int adr, int l) =>
            BitConverter.ToBoolean(Read(adr, l), 0);
        public static object ReadInt(int adr, int l) =>
            BitConverter.ToInt32(Read(adr, l), 0);
        public static object ReadLong(int adr, int l) =>
            ToLong(Read(adr, l));
        public static object ReadSingle(int adr, int l) =>
            BitConverter.ToSingle(Read(adr, l), 0);
        private static object ToLong(byte[] byBytes)
        {
            long num = 0;
            int num3 = Information.UBound(byBytes, 1) - Information.LBound(byBytes, 1);
            for (int i = 0; i <= num3; i++)
            {
                if ((i == 0) & ((byBytes[0] & 0x80) > 0))
                {
                    num |= (long)Math.Round(-(byBytes[i] * Math.Pow(2.0, (double)(8 * ((Strings.Len(num.ToString()) - 1) - i)))));
                }
                else
                {
                    num |= (long)Math.Round((double)(byBytes[i] * Math.Pow(2.0, (double)(8 * ((Strings.Len(num.ToString()) - 1) - i)))));
                }
            }
            return num;
        }
        public static void Write(int adr, byte[] Bytes)
        {
            int lpNumberOfBytesRead = 1;
            MainPage.WriteProcessMemory(MainPage.csgoHandle, (IntPtr)adr, Bytes, Bytes.Length, ref lpNumberOfBytesRead);
        }
        public static void WriteBool(int adr, int l, bool val)
        {
            byte[] bytes = new byte[(l - 1) + 1];
            bytes = BitConverter.GetBytes(val);
            Write(adr, bytes);
        }
        public static void WriteInt(int adr, int l, int val)
        {
            byte[] bytes = new byte[(l - 1) + 1];
            bytes = BitConverter.GetBytes(val);
            Write(adr, bytes);
        }
        public static void WriteSingle(int adr, int l, float val)
        {
            byte[] bytes = new byte[(l - 1) + 1];
            bytes = BitConverter.GetBytes(val);
            Write(adr, bytes);
        }
    }
}
