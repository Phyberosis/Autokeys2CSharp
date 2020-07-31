using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using InputHook;

namespace Autokeys2
{
    internal class Recording
    {
        private Hook hook = Hook.I();

        public void Begin()
        {
            setHooks();
        }

        private void setHooks()
        {
            hook.AddKeyHook(onKeyDown, onKeyUp);
            hook.AddMouseHook(onMouse);
        }

        public void End()
        {
            unhook();
        }

        private void unhook()
        {
            hook.RemoveKeyHook(onKeyDown, onKeyUp);
            hook.RemoveMouseHook(onMouse);
        }

        public void Play()
        {

        }

        private void onKeyDown(Keys k)
        {
        }

        private void onKeyUp(Keys k)
        {

        }

        private void onMouse(MouseAction a, int x, int y)
        {

        }
    }
}
