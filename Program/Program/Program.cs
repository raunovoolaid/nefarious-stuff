using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace VAM_Bhop
{
    class Program
    {
        public static int aLocalPlayer = 0x00A31504;
        public static int oFlags = 0x100;
        public static int aJump = 0x04EE1EF0;

        public static string process = "csgo";
        public static int bClient;

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetAsyncKeyState(int vKey);

        static void Main(string[] args)
        {
            VAMemory vam = new VAMemory(process);

            if (GetModuleAddy())
            {
                int fJump = bClient + aJump;

                aLocalPlayer = bClient + aLocalPlayer;
                int LocalPlayer = vam.ReadInt32((IntPtr)aLocalPlayer);

                int aFlags = LocalPlayer + oFlags;

                while (true)
                {
                    while (GetAsyncKeyState(32) < 0)
                    {
                        int Flags = vam.ReadInt32((IntPtr)aFlags);

                        if (Flags == 257)
                        {
                            vam.WriteInt32((IntPtr)fJump, 5);
                            Thread.Sleep(10);
                            vam.WriteInt32((IntPtr)fJump, 4);

                            Console.Clear();
                            Console.WriteLine("Jumping", Console.ForegroundColor = ConsoleColor.Green);
                            Console.WriteLine(GetAsyncKeyState(32));
                        }
                    }
                    Console.Clear();
                    Console.WriteLine("Standing", Console.ForegroundColor = ConsoleColor.Yellow);
                    Thread.Sleep(10);
                }
               
            }
        }

        static bool GetModuleAddy()
        {
            try
            {
                Process[] p = Process.GetProcessesByName(process);

                if (p.Length > 0)
                {
                    foreach (ProcessModule m in p[0].Modules)
                    {
                        if (m.ModuleName == "client.dll")
                        {
                            bClient = (int)m.BaseAddress;
                            return true;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
