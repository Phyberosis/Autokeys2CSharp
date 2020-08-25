using System;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace OutputSimulator
{
    public class Robot : VirtualMouse
    {
        private IKeyboardSimulator ks = new KeyboardSimulator(new InputSimulator());

        public void DoAction(KeyActions ka, Keys k)
        {
            if (k == Keys.None) return;

            VirtualKeyCode code = (VirtualKeyCode)k;
            switch (ka)
            {
                case KeyActions.DOWN:
                    ks.KeyDown(code);
                    break;
                case KeyActions.UP:
                    ks.KeyUp(code);
                    break;
                case KeyActions.PRESS:
                    ks.KeyDown(code);
                    ks.KeyUp(code);
                    break;
            }
        }

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
