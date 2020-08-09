using System;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace OutputSimulator
{
    public class Robot : VirtualMouse
    {
        private IKeyboardSimulator ks = new KeyboardSimulator(new InputSimulator());

        public void KeyDown(Keys k)
        {
            //Console.WriteLine(((VirtualKeyCode)k).ToString());
            ks.KeyDown((VirtualKeyCode)k);
        }

        public void KeyUp(Keys k)
        {
            ks.KeyUp((VirtualKeyCode)k);
        }
    }
}
