using Events;
using System;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;

namespace OutputSimulator
{
    public class Robot : VirtualMouse
    {
        private IKeyboardSimulator ks = new KeyboardSimulator(new InputSimulator());
        private EventHandle<Key> handle;

        public Robot() : base()
        {
            handle = EventsBuiltin.RegisterEvent<Key>(EventID.KEY_SENT);
        }

        public void DoAction(KeyActions ka, Key k)
        {
            if (k == Key.None) return;
            //Console.WriteLine(k);
            VirtualKeyCode code = (VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k);
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

        public void KeyDown(Key k)
        {
            //Console.WriteLine(((VirtualKeyCode)k).ToString());
            ks.KeyDown((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k));
            handle.Notify(k);
        }

        public void KeyUp(Key k)
        {
            ks.KeyUp((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(k));
        }
    }
}
