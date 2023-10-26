using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MinecraftSimulation
{
    public class SlotManager
    {
        // La variable qui stockera le numéro de slot actuel
        private int currentSlot = 1;

        // La classe qui gère le hook de clavier
        private KeyboardHook keyboardHook = new KeyboardHook();

        // La méthode qui gère les entrées clavier
        private void HandleKeyboardInput(KeyEventArgs e)
        {
            // Si l'utilisateur appuie sur une touche de 1 à 9
            if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9)
            {
                // On met à jour le numéro de slot en fonction de la touche appuyée
                currentSlot = e.KeyCode - Keys.D0;
            }
        }

        // La méthode qui démarre le hook de clavier
        public void StartListening()
        {
            keyboardHook.KeyboardHooked += KeyboardHook_KeyboardHooked;
            keyboardHook.Hook();
        }

        // La méthode qui arrête le hook de clavier
        public void StopListening()
        {
            keyboardHook.Unhook();
        }

        // Le gestionnaire d'événement pour les touches du clavier capturées par le hook
        private void KeyboardHook_KeyboardHooked(object sender, KeyEventArgs e)
        {
            HandleKeyboardInput(e);
        }

        // La méthode qui retourne le numéro de slot actuel
        public int GetCurrentSlot()
        {
            return currentSlot;
        }
    }

    // La classe qui gère le hook de clavier
    public class KeyboardHook
    {
        // L'événement qui sera déclenché lorsqu'une touche du clavier est capturée par le hook
        public event EventHandler<KeyEventArgs> KeyboardHooked;

        // La délégation pour le gestionnaire d'événement de hook de clavier
        public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);

        // Les constantes pour les valeurs de wParam
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        // La structure pour les données de hook de clavier
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        // Les imports de fonction de hook de clavier
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        // WH_KEYBOARD_LL comme étant égale à 13.
        private const int WH_KEYBOARD_LL = 13;

        // Le handle du hook actuel
        int hHook = 0;

        // La méthode de callback du hook
        KeyboardHookProc hookProc;

        // Le constructeur de la classe
public KeyboardHook()
        {
            // On crée la méthode de callback
            hookProc = new KeyboardHookProc(HookProc);
        }


// La méthode qui installe le hook
public void Hook()
        {
            // On récupère le handle de l'instance de notre application
            IntPtr hInstance = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);

            // On installe le hook avec le handle de notre application et la méthode de callback
            hHook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
        }

        // La méthode qui désinstalle le hook
        public void Unhook()
        {
            // On désinstalle le hook
            UnhookWindowsHookEx(hHook);
        }

        // La méthode de callback
        int HookProc(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            // Si le hook doit être appelé
            if (code >= 0)
            {
                // On crée un objet KeyEventArgs avec les données de la touche capturée
                KeyEventArgs e = new KeyEventArgs((Keys)lParam.vkCode);

                // On déclenche l'événement KeyboardHooked
                KeyboardHooked(this, e);

                // Si l'utilisateur a traité l'événement (c'est-à-dire qu'il a appelé e.Handled = true), on bloque la touche
                if (e.Handled)
                    return 1;
            }

            // On appelle le prochain hook dans la chaîne
            return CallNextHookEx(hHook, code, wParam, ref lParam);
        }
    }
}