using System;
using System.Diagnostics;
using System.Threading;

namespace TriggerBotVAM
{
    class Program
    {
        public static int oLocalPlayer = 0x00A31504;
        public static int oEntityList = 0x04A4CCC4;
        public static int oCrossHairID = 0x0000AA44;
        public static int oTeam = 0x000000F0;
        public static int oHealth = 0x000000FC;
        public static int oAttack = 0x02E8CCD0;
        public static int oEntityLoopDistance = 0x00000010;

        public static string process = "csgo";
        public static int bClient;

        static void Main(string[] args)
        {
            VAMemory vam = new VAMemory(process);

            if (GetModuleAddy())
            {
                int fAttack = bClient + oAttack;

                while(true)
                {
                    Console.Clear();
                    Console.Write("Nothing...", Console.ForegroundColor = ConsoleColor.Red);

                    int address = bClient + oLocalPlayer;
                    int LocalPlayer = vam.ReadInt32((IntPtr)address);

                    address = LocalPlayer + oTeam;
                    int MyTeam = vam.ReadInt32((IntPtr)address);

                    address = LocalPlayer + oCrossHairID;
                    int PlayerInCross = vam.ReadInt32((IntPtr)address);

                    if (PlayerInCross > 0 && PlayerInCross < 65)
                    {
                        address = bClient + oEntityList + (PlayerInCross - 1) * oEntityLoopDistance;
                        int PtrToPIC = vam.ReadInt32((IntPtr)address);

                        address = PtrToPIC + oHealth;
                        int PICHealth = vam.ReadInt32((IntPtr)address);

                        address = PtrToPIC + oTeam;
                        int PICTeam = vam.ReadInt32((IntPtr)address);
                        Console.Write(PICTeam);
                        if ((PICTeam != MyTeam) && (PICTeam > 1) && (PICHealth > 0))
                        {
                            Console.Clear();
                            Console.Write("Shooting!", Console.ForegroundColor = ConsoleColor.Green);
                            vam.WriteInt32((IntPtr)fAttack, 1);
                            Thread.Sleep(1);
                            vam.WriteInt32((IntPtr)fAttack, 4);
                        }
                    }
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
