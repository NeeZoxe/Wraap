using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.Distributions;
using WindowsInput;
using System.Security;
using System.Security.Cryptography;

namespace NeeZoxeClicker
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect,
        int nTopRect,
        int nRightRect,
        int nBottomRect,
        int nWidthEllipse,
        int nHeightEllipse
        );

        // Var auto
        bool autoclick = false;

        // déclaration structure detection de block
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentProcessId();

        const uint PROCESS_ALL_ACCESS = 0x001F0FFF;

        struct Position
        {
            public float x;
            public float y;
            public float z;
        }

        struct Direction
        {
            public float x;
            public float y;
            public float z;
        }

        // Calcul la distance entre deux points dans l'espace 3D dans une direction donnée
        float distanceInDirection(Position p1, Position p2, Direction d)
        {
            return (p2.x - p1.x) * d.x + (p2.y - p1.y) * d.y + (p2.z - p1.z) * d.z;
        }

        static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return theStructure;
        }
        // import jitter

        // Déclarer la constante MOUSEEVENTF_MOVE
        public const int MOUSEEVENTF_MOVE = 0x0001;
        private Point previousPosition;

        [DllImport(@"C:\Path\To\MathNet.Numerics.dll", EntryPoint = "NextGaussian")]
        public static extern double NextGaussian(double mean, double stddev);

        [DllImport(@"C:\Path\To\MathNet.Numerics.dll", EntryPoint = "NextVonMises")]
        public static extern double NextVonMises(double kappa);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        // Son Toggle
        private SoundPlayer _soundPlayer;

        // Déclaration des constantes pour les hook de clavier et souris
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int VK_1 = 0x31;
        private const int VK_2 = 0x32;
        private const int VK_3 = 0x33;
        private const int VK_4 = 0x34;
        private const int VK_5 = 0x35;
        private const int VK_6 = 0x36;
        private const int VK_7 = 0x37;
        private const int VK_8 = 0x38;
        private const int VK_9 = 0x39;

        // Déclaration de la variable qui contient le numéro de slot actuel
        private static int currentSlot = 1;

        // Déclaration des variables pour les hook de clavier et souris
        private static LowLevelKeyboardProc _keyboardProc;
        private static LowLevelMouseProc _mouseProc;
        private static IntPtr _keyboardHookID = IntPtr.Zero;
        private static IntPtr _mouseHookID = IntPtr.Zero;

        // Déclaration de la fonction de hook de clavier
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) // si une touche est enfoncée
            {
                KBDLLHOOKSTRUCT kbd = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (IsMinecraftActive()) // si Minecraft est le processus actif
                {
                    if (kbd.key == Keys.D1 || kbd.key == Keys.NumPad1) // si la touche 1 est pressée
                    {
                        currentSlot = 1; // met à jour la valeur de currentSlot
                    }
                    if (kbd.key == Keys.D2 || kbd.key == Keys.NumPad2) // si la touche 2 est pressée
                    {
                        currentSlot = 2; // met à jour la valeur de currentSlot
                    }
                    if (kbd.key == Keys.D3 || kbd.key == Keys.NumPad3) // si la touche 3 est pressée
                    {
                        currentSlot = 3; // met à jour la valeur de currentSlot
                    }
                    if (kbd.key == Keys.D4 || kbd.key == Keys.NumPad4) // si la touche 4 est pressée
                    {
                        currentSlot = 4; // met à jour la valeur de currentSlot
                    }
                    if (kbd.key == Keys.D5 || kbd.key == Keys.NumPad5) // si la touche 5 est pressée
                    {
                        currentSlot = 5; // met à jour la valeur de currentSlot
                    }
                    if (kbd.key == Keys.D6 || kbd.key == Keys.NumPad6) // si la touche 6 est pressée
                    {
                        currentSlot = 6; // met à jour la valeur de currentSlot
                    }
                    if (kbd.key == Keys.D7 || kbd.key == Keys.NumPad7) // si la touche 7 est pressée
                    {
                        currentSlot = 7; // met à jour la valeur de currentSlot
                    }
                    if (kbd.key == Keys.D8 || kbd.key == Keys.NumPad8) // si la touche 8 est pressée
                    {
                        currentSlot = 8; // met à jour la valeur de currentSlot
                    }
                    if (kbd.key == Keys.D9 || kbd.key == Keys.NumPad9) // si la touche 9 est pressée
                    {
                        currentSlot = 9; // met à jour la valeur de currentSlot
                    }
                }
                Console.WriteLine(currentSlot);
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }
        // Déclaration de la fonction de hook de souris
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEWHEEL) // si la molette de la souris est utilisée
            {
                MSLLHOOKSTRUCT mouse = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                if (IsMinecraftActive()) // si Minecraft est le processus actif
                {
                    if (mouse.mouseData > 0) // si la molette est tournée vers le haut
                    {
                        currentSlot--; // augmente la valeur de currentSlot
                        if (currentSlot < 1) // si currentSlot dépasse 9
                        {
                            currentSlot = 9; // met currentSlot à 9
                        }
                    }
                    if (mouse.mouseData < 0) // si la molette est tournée vers le bas
                    {
                        currentSlot++; // diminue la valeur de currentSlot
                        if (currentSlot > 9) // si currentSlot est inférieur à 1
                        {
                            currentSlot = 1; // met currentSlot à 1
                        }
                    }
                }
                Console.WriteLine(currentSlot);
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        // Déclaration de la fonction qui désinstalle les hook de clavier et souris
        private static void UninstallHook()
        {
            UnhookWindowsHookEx(_keyboardHookID);
            UnhookWindowsHookEx(_mouseHookID);
        }

        // Déclaration de la fonction qui retourne le nom du processus actif
        private static string GetActiveProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);
            return p.ProcessName;
        }

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        // Déclaration de la fonction qui retourne vrai si le processus actif est Minecraft
        private static bool IsMinecraftActive()
        {
            return GetActiveWindowTitle().Contains("Minecraft");
        }

        // Déclaration de la fonction qui installe les hook de clavier et souris
        private static void InstallHook()
        {
            _keyboardProc = KeyboardHookProc;
            _mouseProc = MouseHookProc;
            _keyboardHookID = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc, GetModuleHandle(IntPtr.Zero), 0);
            _mouseHookID = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, GetModuleHandle(IntPtr.Zero), 0);
        }

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20,20));
            USER.Text = "USER : " + Environment.UserName;
            MenuTabControl.Padding = new Point(15, 10);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ChoixMode.SelectedIndex = 0;
            ChoixMode2.SelectedIndex = 0;
            ToggleSoundList.SelectedIndex = 0;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UninstallHook(); // désinstalle les hook de clavier et souris
        }

        // Déclaration de la structure pour le hook de clavier
        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        // Déclaration de la structure pour le hook de souris
        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public Point pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        // Déclaration des fonctions importées depuis user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        // var slot
        int Slot1 = 1;
        int Slot2 = 2;
        int Slot3 = 3;
        int Slot4 = 4;
        int Slot5 = 5;
        int Slot6 = 6;
        int Slot7 = 7;
        int Slot8 = 8;
        int Slot9 = 9;

        // Initialisez SlotActiveAuto à false
        bool SlotActiveAuto = false;
        private void CompareSlots()
        {
            // Initialisez SlotActiveAuto à false
            SlotActiveAuto = false;

            // Vérifiez si SlotBox est coché et si sa valeur correspond à currentSlot
            if (SlotBox1.Checked && Slot1 == currentSlot)
            {
                SlotActiveAuto = true;
            }
            else if (SlotBox2.Checked && Slot2 == currentSlot)
            {
                SlotActiveAuto = true;
            }
            else if (SlotBox3.Checked && Slot3 == currentSlot)
            {
                SlotActiveAuto = true;
            }
            else if (SlotBox4.Checked && Slot4 == currentSlot)
            {
                SlotActiveAuto= true;
            }
            else if (SlotBox5.Checked && Slot5 == currentSlot)
            {
                SlotActiveAuto = true;
            }
            else if (SlotBox6.Checked && Slot6 == currentSlot)
            {
                SlotActiveAuto = true;
            }
            else if (SlotBox7.Checked && Slot7 == currentSlot)
            {
                SlotActiveAuto = true;
            }
            else if (SlotBox8.Checked && Slot8 == currentSlot)
            {
                SlotActiveAuto = true;
            }
            else if (SlotBox9.Checked && Slot9 == currentSlot)
            {
                SlotActiveAuto = true;
            }
            // Si aucun des SlotBox n'est coché ou aucun ne correspond à currentSlot, SlotActiveAuto restera false
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void CPSTrackabr_Scroll(object sender, ScrollEventArgs e)
        {
            CPSValue.Text = (CPSTrackabr.Value.ToString() + " CPS");
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (btnToggle.Text.Contains("enable"))
            {
                btnToggle.Text = "disable";
                if (CheckBoxBeepToggle.Checked == true)
                {
                    _soundPlayer.Play();
                }
                if (SlotCheckBox.Checked == false)
                {
                    SlotActiveAuto = true;
                }
            }
            else if (btnToggle.Text.Contains("disable"))
            {
                btnToggle.Text = "enable";
            }
        }

        private void btnToggle_TextChanged(object sender, EventArgs e)
        {
            if (btnToggle.Text.Contains("disable"))
            {
                Autoclicker.Start();
            }
            else
            {
                Autoclicker.Stop();
            }
        }


        int min;
        int max;

        public string getActiveWindowName()
        {
            try
            {
                var activateHandle = GetForegroundWindow();

                Process[] processes = Process.GetProcesses();
                foreach (Process clsProcess in processes)
                {
                    if (activateHandle == clsProcess.MainWindowHandle)
                    {
                        string processName = clsProcess.ProcessName;
                        return processName;
                    }
                }
            }
            catch { }
            return null;
        }

        private void Random_Tick(object sender, EventArgs e)
        {
            if (btnToggle.Text.Contains("disable") && ChoixMode.Text == ChoixMode.SelectedIndex.ToString("Legit (medium deviation)"))
            {
                min = CPSTrackabr.Value - 2;
                max = CPSTrackabr.Value + 2;
                Random rand = new Random();
                randomTB.Value = (rand.Next(min, max));

            }
            else if (btnToggle.Text.Contains("disable") && ChoixMode.Text == ChoixMode.SelectedIndex.ToString("Extra Legit (medium deviation)"))
            {
                min = CPSTrackabr.Value - 3;
                max = CPSTrackabr.Value + 4;
                Random rand = new Random();
                randomTB.Value = (rand.Next(min, max));
                
            }
        }

        private void siticoneTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        IntPtr hWndMinecraft = IntPtr.Zero;

        private void FindMinecraftWindow()
        {
            Process[] processes = Process.GetProcessesByName("javaw");
            foreach (Process process in processes)
            {
                process.Refresh();
                if (process.MainWindowTitle.Contains("Minecraft") || process.MainWindowTitle.Contains("Lunar"))
                {
                    process.WaitForInputIdle();
                    hWndMinecraft = process.MainWindowHandle;
                    break;
                }
            }
        }


        private async void Autoclicker_Tick(object sender, EventArgs e)
        {
            try
            {
                Autoclicker.Interval = 1000 / randomTB.Value;
            }
            catch { }
            if (btnToggle.Text.Contains("disable"))
            {
                FindMinecraftWindow();
                if (hWndMinecraft != IntPtr.Zero)
                {
                    Console.WriteLine("minecraft found");
                    if (!SlotCheckBox.Checked)
                    {
                        if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                        {
                            autoclick = true;
                            PostMessage(hWndMinecraft, 0x0201, 0, 0);
                            await Task.Delay(30);
                            PostMessage(hWndMinecraft, 0x0202, 0, 0);
                        }
                        else
                        {
                            autoclick = false;
                        }
                        Console.WriteLine("autoclick state : " + autoclick);
                    }
                    else if (SlotCheckBox.Checked)
                    {
                        if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left && SlotActiveAuto == true)
                        {
                            autoclick = true;
                            PostMessage(hWndMinecraft, 0x0201, 0, 0);
                            await Task.Delay(30);
                            PostMessage(hWndMinecraft, 0x0202, 0, 0);
                        }
                        else
                        {
                            autoclick = false;
                        }
                        Console.WriteLine("autoclick state : " + autoclick);
                    }
                } else
                {
                    Console.WriteLine("Minecraft not found");
                }

            }
        }
        bool keyIsSetL = false;

        private void bindBtn_Click(object sender, EventArgs e)
        {
            bindBtn.Text = "...";
            keyIsSetL = false;
        }

        KeysConverter Key = new KeysConverter();

        private void Binding_Tick(object sender, EventArgs e)
        {
            if (bindBtn.Text != "none" && bindBtn.Text != "...")
            {
                Keys Binding = (Keys)Key.ConvertFromString(bindBtn.Text.Replace("...", ""));
                if (GetAsyncKeyState(Binding) < 0)
                {

                    while (GetAsyncKeyState(Binding) < 0)
                    {
                        Thread.Sleep(20);
                    }
                    if (btnToggle.Text.Contains("enable"))
                    {
                        btnToggle.Text = "disable";
                        if (CheckBoxBeepToggle.Checked == true)
                        {
                            _soundPlayer.Play();
                        }
                    }
                    else if (btnToggle.Text.Contains("disable"))
                    {
                        btnToggle.Text = "enable";
                    }

                    return;
                }
            }
        }

        private void bindBtn_KeyDown(object sender, KeyEventArgs e)
        {
            {
                string keydata = e.KeyData.ToString();
                if (!keydata.Contains("Alt"))
                {
                    if (GetAsyncKeyState(Keys.Escape) < 0)
                    {
                        bindBtn.Text = "none";
                    }
                    else if (keyIsSetL == false)
                    {
                        bindBtn.Text = keydata;
                        keyIsSetL = true;
                    }
                }
            }
            KeysConverter Key = new KeysConverter();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {

        }

        private void SelfDestruct_Click(object sender, EventArgs e)
        {
            string exename = AppDomain.CurrentDomain.FriendlyName;
            DirectoryInfo d = new DirectoryInfo(@"C:\Windows\Prefetch");
            FileInfo[] Files = d.GetFiles(exename + "*");
            foreach (FileInfo file in Files)
            {
                File.Delete(file.FullName);
            }
            Application.Exit();
        }

        private void ChoixMode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void siticoneComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void siticoneButton1_Click_1(object sender, EventArgs e)
        {
            if (HideTaskBar.Text.Contains("Hide Taskbar off"))
            {
                HideTaskBar.Text = "Hide Taskbar on";
                this.ShowInTaskbar = false;
            }
            else if (HideTaskBar.Text.Contains("Hide Taskbar on"))
            {
                HideTaskBar.Text = "Hide Taskbar off";
                this.ShowInTaskbar = true;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }


        private bool breakBlockState = false;
        private void siticoneCustomCheckBox1_Click(object sender, EventArgs e)
        {
            if (CheckBreakBlock.Checked)
            {

                Position playerPosition;
                Direction playerDirection;
                Position blockPosition;

                // Ouvrez le processus de Minecraft
                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, GetCurrentProcessId());
                if (processHandle == IntPtr.Zero)
                {
                    Console.WriteLine("Erreur lors de l'ouverture du processus de Minecraft !\n");
                    return;
                }
                else
                {
                    Console.WriteLine("Le code est exécuté !");
                }

                // Répétez la boucle infiniment
                while (true)
                {
                    // Récupérez les coordonnées de position du joueur dans Minecraft
                    byte[] playerPositionBuffer = new byte[Marshal.SizeOf(typeof(Position))];
                    ReadProcessMemory(processHandle, (IntPtr)0x11F1B50, playerPositionBuffer, Marshal.SizeOf(typeof(Position)), out int bytesRead);
                    playerPosition = ByteArrayToStructure<Position>(playerPositionBuffer);

                    // Récupérez les coordonnées de position du block que le joueur regarde dans Minecraft
                    byte[] blockPositionBuffer = new byte[Marshal.SizeOf(typeof(Position))];
                    ReadProcessMemory(processHandle, (IntPtr)0x11F1B60, blockPositionBuffer, Marshal.SizeOf(typeof(Position)), out bytesRead);
                    blockPosition = ByteArrayToStructure<Position>(blockPositionBuffer);

                    // Récupérez la direction dans laquelle le joueur regarde dans Minecraft
                    byte[] playerDirectionBuffer = new byte[Marshal.SizeOf(typeof(Direction))];
                    ReadProcessMemory(processHandle, (IntPtr)0x11ED9E8, playerDirectionBuffer, Marshal.SizeOf(typeof(Direction)), out bytesRead);
                    playerDirection = ByteArrayToStructure<Direction>(playerDirectionBuffer);

                    float distance = distanceInDirection(playerPosition, blockPosition, playerDirection);

                    if (distance <= 3)
                    {
                        Console.WriteLine("Le bloc peut être cassé !\n");
                        breakBlockState = true;
                    }
                    else
                    {
                        breakBlockState = false;
                        Console.WriteLine("Le bloc ne peut pas être cassé !\n");
                    }

                    break;
                }

                // Fermez le processus de Minecraft
                CloseHandle(processHandle);
            }
        
    }

        private void CPSTrackabr2_Scroll(object sender, ScrollEventArgs e)
        {
            CPSValue2.Text = (CPSTrackabr2.Value.ToString() + " CPS");
        }

        private void btnToggle2_Click(object sender, EventArgs e)
        {
            if (btnToggle2.Text.Contains("enable"))
            {
                btnToggle2.Text = "disable";
            }
            else if (btnToggle2.Text.Contains("disable"))
            {
                btnToggle2.Text = "enable";
            }
        }

        private void btnToggle2_TextChanged(object sender, EventArgs e)
        {
            if (btnToggle2.Text.Contains("disable"))
            {
                Autoclicker2.Start();
            }
            else
            {
                Autoclicker2.Stop();
            }
        }

        int min2;
        int max2;
        private void Random2_Tick(object sender, EventArgs e)
        {
            if (btnToggle2.Text.Contains("disable") && ChoixMode2.Text == ChoixMode2.SelectedIndex.ToString("Legit (medium deviation)"))
            {
                min2 = CPSTrackabr2.Value - 2;
                max2 = CPSTrackabr2.Value + 2;
                Random rand = new Random();
                randomTB2.Value = (rand.Next(min2, max2));

            }
            else if (btnToggle2.Text.Contains("disable") && ChoixMode2.Text == ChoixMode2.SelectedIndex.ToString("Extra Legit (medium deviation)"))
            {
                min2 = CPSTrackabr2.Value - 3;
                max2 = CPSTrackabr2.Value + 4;
                Random rand = new Random();
                randomTB2.Value = (rand.Next(min2, max2));

            }
        }

        private void ChoixMode2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void Autoclicker2_Tick(object sender, EventArgs e)
        {
            try
            {
                Autoclicker2.Interval = 1000 / randomTB2.Value;
            }
            catch { }
            if (btnToggle2.Text.Contains("disable"))
            {
                FindMinecraftWindow();
                if (hWndMinecraft != IntPtr.Zero)
                {
                    if ((MouseButtons & MouseButtons.Right) == MouseButtons.Right)
                    {
                        PostMessage(hWndMinecraft, 0x0204, 0, 0);
                        await Task.Delay(30);
                        PostMessage(hWndMinecraft, 0x0205, 0, 0);
                    }
                }
            }
        }

        bool keyIsSetR = false;
        private void bindBtn2_Click(object sender, EventArgs e)
        {
            bindBtn2.Text = "...";
            keyIsSetR = false;
        }

        private void bindBtn2_KeyDown(object sender, KeyEventArgs e)
        {
            {
                string keydata = e.KeyData.ToString();
                if (!keydata.Contains("Alt"))
                {
                    if (GetAsyncKeyState(Keys.Escape) < 0)
                    {
                        bindBtn2.Text = "none";
                    }
                    else if (keyIsSetR == false)
                    {
                        bindBtn2.Text = keydata;
                        keyIsSetR = true;
                    }
                }
            }
            KeysConverter Key = new KeysConverter();
        }

        private void Binding2_Tick(object sender, EventArgs e)
        {
            if (bindBtn2.Text != "none" && bindBtn2.Text != "...")
            {
                Keys Binding2 = (Keys)Key.ConvertFromString(bindBtn2.Text.Replace("...", ""));
                if (GetAsyncKeyState(Binding2) < 0)
                {

                    while (GetAsyncKeyState(Binding2) < 0)
                    {
                        Thread.Sleep(20);
                    }
                    if (btnToggle2.Text.Contains("enable"))
                    {
                        btnToggle2.Text = "disable";
                    }
                    else if (btnToggle2.Text.Contains("disable"))
                    {
                        btnToggle2.Text = "enable";
                    }

                    return;
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void siticoneVTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        int couleurR = 148;
        private void siticoneTrackBar1_Scroll_1(object sender, ScrollEventArgs e)
        {
            couleurR = ColorR.Value;
            ColorR.ThumbColor = Color.FromArgb(couleurR, couleurG, couleurB);
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        int couleurB = 211;
        private void siticoneTrackBar3_Scroll(object sender, ScrollEventArgs e)
        {
            couleurB = ColorB.Value;
            ColorB.ThumbColor = Color.FromArgb(couleurR, couleurG, couleurB);
        }

        int couleurG = 0;
        private void ColorG_Scroll(object sender, ScrollEventArgs e)
        {
            couleurG = ColorG.Value;
            ColorG.ThumbColor = Color.FromArgb(couleurR, couleurG, couleurB);
        }


        // bouton RGB
        bool startRGB = false;
        private void CheckRGB_Click(object sender, EventArgs e)
        {
            if (CheckRGB.Checked == true)
            {
                startRGB = true;
            }
            else if (CheckRGB.Checked == false)
            {
                startRGB = false;
            }
        }

        // RGB
        int r = 255, g = 0, b = 0;

        // Haut de page MouseMove
        private void panelWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (mDown)
            {
                Point currentPos = PointToScreen(e.Location);
                Location = new Point(currentPos.X - Pos.X, currentPos.Y - Pos.Y);
            }
        }

        // Haut de page MouseUp
        private void panelWindow_MouseUp(object sender, MouseEventArgs e)
        {
            mDown = false;
        }

        // Haut de Fenetre MouseDown
        Point Pos;
        bool mDown = false;

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void TextWrap_Click(object sender, EventArgs e)
        {

        }

        private void CloseBox_Click(object sender, EventArgs e)
        {
            Application.Exit();
            UninstallHook(); // désinstalle les hook de clavier et souris
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void pbDestruct_Click(object sender, EventArgs e)
        {
            string exename = AppDomain.CurrentDomain.FriendlyName;
            DirectoryInfo d = new DirectoryInfo(@"C:\Windows\Prefetch");
            FileInfo[] Files = d.GetFiles(exename + "*");
            foreach (FileInfo file in Files)
            {
                File.Delete(file.FullName);
            }
            Application.Exit();
        }

        private void randomTB2_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click_2(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }

        private void CPSValue_Click(object sender, EventArgs e)
        {

        }
     
        public void Jitter()
        {
            Random rng = new Random();

            // Génère une amplitude aléatoire en utilisant une distribution uniforme entre un quart de la valeur de la barre de défilement et la moitié de la valeur de la barre de défilement
            int Xaxis = rng.Next(TrackBarAxisX.Value / 8, TrackBarAxisX.Value / 4);

            // Génère une direction aléatoire entre 0 et 360 degrés
            double direction = rng.NextDouble() * 360;

            // Calcule les coordonnées x et y du nouveau point en utilisant une fonction sinusoïdale
            double frequency = 2 * Math.PI / 360; // fréquence en radians par degré
            int x = (int)(Xaxis * Math.Sin(direction * frequency));
            int y = (int)(Xaxis * Math.Cos(direction * frequency));

            // Ajouter du bruit aléatoire aux coordonnées générées en utilisant une distribution uniforme
            int noiseX = rng.Next(-TrackBarAxisX.Value / 2, TrackBarAxisX.Value / 2);
            int noiseY = rng.Next(-TrackBarAxisX.Value / 2, TrackBarAxisX.Value / 2);
            x += noiseX;
            y += noiseY;

            // Récupérer les coordonnées actuelles de la souris
            Point currentPosition;
            GetCursorPos(out currentPosition);

            // Si c'est la première fois que la fonction est appelée, enregistrer les coordonnées actuelles comme précédentes
            if (previousPosition == null)
            {
                previousPosition = currentPosition;
            }

            // Déplacer la souris vers les coordonnées cibles en utilisant mouse_event
            mouse_event(MOUSEEVENTF_MOVE, x, y, 0, 0);

            // enregistrer les coordonnées actuelles comme précédentes pour la prochaine fois que la fonction est appelée
            previousPosition = currentPosition;

        }

        private void siticoneTrackBar4_Scroll(object sender, ScrollEventArgs e)
        {
            SHIFTX.Text = (TrackBarAxisX.Value.ToString() + " SHIFT");
        }

        private void SHIFTX_Click(object sender, EventArgs e)
        {

        }

        private void TrackBarAxisY_Scroll(object sender, ScrollEventArgs e)
        {
            SHIFTY.Text = (TrackBarAxisY.Value.ToString() + " HERTZ");
        }

        private void CheckBoxJitter_CheckedChanged(object sender, EventArgs e)
        {   
            if (CheckBoxJitter.Checked == true)
            {
                TimerJitter.Start();
            } else
            {
                TimerJitter.Stop();
            }
        }

        int minJitter;
        int maxJitter;
        int timerRandomJitter;

        private void randomJitter_Tick(object sender, EventArgs e)
        {
            if (CheckBoxJitter.Checked == true)
            {
                minJitter = TrackBarAxisY.Value - 2;
                maxJitter = TrackBarAxisY.Value + 2;
                Random rand = new Random();
                timerRandomJitter = (rand.Next(minJitter, maxJitter));
            }
        }

        private void TimerJitter_Tick(object sender, EventArgs e)
        {
            try
            {
                TimerJitter.Interval = 1000 / timerRandomJitter;
            }
            catch { }
            if (CheckBoxJitter.Checked == true)
            {
                FindMinecraftWindow();
                if (hWndMinecraft != IntPtr.Zero)
                {
                    // Vérification si le clic gauche est enfoncé && si le jitter est activée
                    if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                    {
                        Jitter();
                    }
                }
            }
        }

        private void SHIFTY_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void siticoneTrackBarCPSDELIMITER_Scroll(object sender, ScrollEventArgs e)
        {
            TextCpsDelimiter.Text = (siticoneTrackBarCPSDELIMITER.Value.ToString() + " CPS MAX");
            CPSTrackabr.Maximum = siticoneTrackBarCPSDELIMITER.Value;
            CPSTrackabr2.Maximum = siticoneTrackBarCPSDELIMITER.Value;
            randomTB.Maximum = siticoneTrackBarCPSDELIMITER.Value;
            randomTB2.Maximum = siticoneTrackBarCPSDELIMITER.Value;
        }

        private void ToggleSoundList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ToggleSoundList.Text == ToggleSoundList.SelectedIndex.ToString("Pew"))
            {
                _soundPlayer = new SoundPlayer(Properties.Resources.Pew);
            }
            else if (ToggleSoundList.Text == ToggleSoundList.SelectedIndex.ToString("Retro"))
            {
                _soundPlayer = new SoundPlayer(Properties.Resources.Retro);
            }
        }

        private void MinecraftMonitorTimer_Tick(object sender, EventArgs e)
        {
            CompareSlots();
            Console.WriteLine("frequence");
        }

        private void SlotCheckBox_Click(object sender, EventArgs e)
        {
            if (SlotCheckBox.Checked) 
            {
                InstallHook(); // installe les hook de clavier et souris
                MinecraftMonitorTimer.Start();
            }
            else
            {
                UninstallHook();
                MinecraftMonitorTimer.Stop();
            }
        }

        [DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CredRead(string target, CRED_TYPE type, int reservedFlag, out IntPtr CredentialPtr);

        [DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CredFree([In] IntPtr cred);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CREDENTIAL
        {
            public UInt32 Flags;
            public UInt32 Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public IntPtr CredentialBlob;
            public UInt32 Persist;
            public UInt32 AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }

        private enum CRED_TYPE : uint
        {
            CRED_TYPE_GENERIC = 1,
            CRED_TYPE_DOMAIN_PASSWORD = 2,
            CRED_TYPE_DOMAIN_CERTIFICATE = 3,
        }

        static void ListCredentials(string[] args)
        {
            string target = "targetName";
            CRED_TYPE type = CRED_TYPE.CRED_TYPE_GENERIC;
            int reservedFlag = 0;
            IntPtr CredentialPtr;
            if (CredRead(target, type, reservedFlag, out CredentialPtr))
            {
                var cred = (CREDENTIAL)Marshal.PtrToStructure(CredentialPtr, typeof(CREDENTIAL));

                // Utilisation de GCHandle pour gérer les données sensibles
                GCHandle handle = GCHandle.Alloc(cred.CredentialBlob, GCHandleType.Pinned);
                try
                {
                    var passwordBytes = new byte[cred.CredentialBlobSize];
                    Marshal.Copy(handle.AddrOfPinnedObject(), passwordBytes, 0, (int)cred.CredentialBlobSize);
                    var password = new SecureString();
                    for (var i = 0; i < passwordBytes.Length; i++)
                        password.AppendChar((char)passwordBytes[i]);

                    Console.WriteLine("Type : " + cred.Type);
                    Console.WriteLine("TargetName : " + Marshal.PtrToStringUni(cred.TargetName));
                    Console.WriteLine("UserName : " + Marshal.PtrToStringUni(cred.UserName));
                    Console.WriteLine("Password : " + password.ToString());
                    Console.WriteLine("-----------------");
                }
                finally
                {
                    handle.Free();
                    CredFree(CredentialPtr);
                }
            }
        }
    

    private void IgnoreMenuCheckBox_Click(object sender, EventArgs e)
        {
            if (IgnoreMenuCheckBox.Checked)
            {
                ListCredentials(null);
            }
        }

        private void panelWindow_MouseDown(object sender, MouseEventArgs e)
        {
            Pos.X = e.X;
            Pos.Y = e.Y;
            mDown = true;
        }

        private void ColorAll_Tick(object sender, EventArgs e)
        {

            if (startRGB == true)
            {
                this.ColorR.ThumbColor = Color.FromArgb(r, g, b);
                this.ColorG.ThumbColor = Color.FromArgb(r, g, b);
                this.ColorB.ThumbColor = Color.FromArgb(r, g, b);
                this.TextR.ForeColor = Color.FromArgb(r, g, b);
                this.TextG.ForeColor = Color.FromArgb(r, g, b);
                this.TextB.ForeColor = Color.FromArgb(r, g, b);
                this.TextBreakBlocks.ForeColor = Color.FromArgb(r, g, b);
                this.CheckBreakBlock.CheckedState.FillColor = Color.FromArgb(r, g, b);
                this.CheckBreakBlock.CheckedState.BorderColor = Color.FromArgb(r, g, b);
                this.SelfDestruct.ForeColor = Color.FromArgb(r, g, b);
                this.HideTaskBar.ForeColor = Color.FromArgb(r, g, b);
                this.ColorTitle.ForeColor = Color.FromArgb(r, g, b);
                this.ChoixMode.ForeColor = Color.FromArgb(r, g, b);
                this.ChoixMode2.ForeColor = Color.FromArgb(r, g, b);
                this.bindBtn.ForeColor = Color.FromArgb(r, g, b);
                this.bindBtn2.ForeColor = Color.FromArgb(r, g, b);
                this.btnToggle.ForeColor = Color.FromArgb(r, g, b);
                this.btnToggle2.ForeColor = Color.FromArgb(r, g, b);
                this.CPSValue.ForeColor = Color.FromArgb(r, g, b);
                this.CPSValue2.ForeColor = Color.FromArgb(r, g, b);
                this.CPSTrackabr.ThumbColor = Color.FromArgb(r, g, b);
                this.CPSTrackabr2.ThumbColor = Color.FromArgb(r, g, b);
                this.TextCPSLeft.ForeColor = Color.FromArgb(r, g, b);
                this.TextCPSRight.ForeColor = Color.FromArgb(r, g, b);
                this.TextRGBMod.ForeColor = Color.FromArgb(r, g, b);
                this.CheckRGB.CheckedState.FillColor = Color.FromArgb(r, g, b);
                this.CheckRGB.CheckedState.BorderColor = Color.FromArgb(r, g, b);
                this.TextWrap.ForeColor = Color.FromArgb(r, g, b);
                this.USER.ForeColor = Color.FromArgb(r, g, b);
                this.MenuTabControl.TabButtonSelectedState.InnerColor = Color.FromArgb(r, g, b);

                if (r > 0 && b == 0)
                {
                    r--;
                    g++;
                }
                if (g > 0 && r == 0)
                {
                    g--;
                    b++;
                }
                if (b > 0 && g == 0)
                {
                    b--;
                    r++;
                }

                // bouton click Droit changement couleur
                if (btnToggle2.Text.Contains("disable"))
                {
                    btnToggle2.ForeColor = Color.FromArgb(50, 50, 50);
                    btnToggle2.FillColor = Color.FromArgb(r, g, b);
                }
                else if (btnToggle2.Text.Contains("enable"))
                {
                    btnToggle2.ForeColor = Color.FromArgb(r, g, b);
                    btnToggle2.FillColor = Color.FromArgb(50, 50, 50);
                }

                // bouton click gauche changement couleur
                if (btnToggle.Text.Contains("disable"))
                {
                    btnToggle.ForeColor = Color.FromArgb(50, 50, 50);
                    btnToggle.FillColor = Color.FromArgb(r, g, b);
                }
                else if (btnToggle.Text.Contains("enable"))
                {
                    btnToggle.ForeColor = Color.FromArgb(r, g, b);
                    btnToggle.FillColor = Color.FromArgb(50, 50, 50);
                }
            }
            
            else if (startRGB == false)
            {
                ColorR.ThumbColor = Color.FromArgb(couleurR, couleurG, couleurB);
                ColorG.ThumbColor = Color.FromArgb(couleurR, couleurG, couleurB);
                ColorB.ThumbColor = Color.FromArgb(couleurR, couleurG, couleurB);
                TextR.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                TextG.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                TextB.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                TextBreakBlocks.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                CheckBreakBlock.CheckedState.FillColor = Color.FromArgb(couleurR, couleurG, couleurB);
                CheckBreakBlock.CheckedState.BorderColor = Color.FromArgb(couleurR, couleurG, couleurB);
                SelfDestruct.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                HideTaskBar.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                ColorTitle.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                ChoixMode.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                ChoixMode2.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                bindBtn.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                bindBtn2.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                CPSValue.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                CPSValue2.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                CPSTrackabr.ThumbColor = Color.FromArgb(couleurR, couleurG, couleurB);
                CPSTrackabr2.ThumbColor = Color.FromArgb(couleurR, couleurG, couleurB);
                TextCPSLeft.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                TextCPSRight.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                TextRGBMod.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                CheckRGB.CheckedState.FillColor = Color.FromArgb(couleurR, couleurG, couleurB);
                CheckRGB.CheckedState.BorderColor = Color.FromArgb(couleurR, couleurG, couleurB);
                TextWrap.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                USER.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                MenuTabControl.TabButtonSelectedState.InnerColor = Color.FromArgb(couleurR, couleurG, couleurB);

                // bouton autoclick right changement couleur de base
                if (btnToggle2.Text.Contains("disable"))
                {
                    btnToggle2.ForeColor = Color.FromArgb(50, 50, 50);
                    btnToggle2.FillColor = Color.FromArgb(couleurR, couleurG, couleurB);
                }
                else if (btnToggle2.Text.Contains("enable"))
                {
                    btnToggle2.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                    btnToggle2.FillColor = Color.FromArgb(50, 50, 50);
                }

                // bouton autoclick left changement couleur de base
                if (btnToggle.Text.Contains("disable"))
                {
                    btnToggle.ForeColor = Color.FromArgb(50, 50, 50);
                    btnToggle.FillColor = Color.FromArgb(couleurR, couleurG, couleurB);
                }
                else if (btnToggle.Text.Contains("enable"))
                {
                    btnToggle.ForeColor = Color.FromArgb(couleurR, couleurG, couleurB);
                    btnToggle.FillColor = Color.FromArgb(50, 50, 50);
                }
            }
        }
    }
}
