using Events;
using InputHook;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Monitors
{
    public class ExternalKeyMonitor
    {
        private static object initSync = new object();
        private static Dictionary<Key, ExternalKeyMonitor> monitors;

        private LinkedList<Action> callbacks;
        private readonly Key key;

        private int seenVSent = 0;

        private ExternalKeyMonitor(Key k, Action callback)
        {
            callbacks = new LinkedList<Action>();
            callbacks.AddLast(callback);
            key = k;
            monitors.Add(k, this);
        }
        
        private  static void init()
        {
            monitors = new Dictionary<Key, ExternalKeyMonitor>();
            var hook = Hook.I();

            hook.AddKeyHook((k) =>
            {
                lock(monitors)
                {
                    if (monitors.ContainsKey(k))
                    {
                        var m = monitors[k];
                        m.seenVSent++;
                        if(m.seenVSent >= 1)
                        {
                            Task.Delay(16).ContinueWith((t) =>
                            {
                                bool call = false;
                                lock (monitors)
                                {
                                    if(m.seenVSent >= 1) { call = true; m.seenVSent--; }
                                }

                                if (!call) return;

                                foreach (Action c in m.callbacks)
                                {
                                    c();
                                }
                            });
                        }
                    }
                }
            }, (ku) => { });

            EventsBuiltin.RegisterListener<Key>(EventID.KEY_SENT, (k) =>
            {
                lock(monitors)
                {
                    if (monitors.ContainsKey(k))
                    {
                        monitors[k].seenVSent--;
                    }
                }
            });
        }

        public static void Monitor(Key k, Action callback)
        {
            lock(initSync)
            {
                if (monitors == null) init();
            }

            lock(monitors)
            {
                if (monitors.ContainsKey(k))
                {
                    monitors[k].callbacks.AddLast(callback);
                }
                else
                {
                    new ExternalKeyMonitor(k, callback);
                }
            }
        }

        public static void Remove(Key k)
        {
            lock(monitors)
            {
                if (monitors.ContainsKey(k)) monitors[k].dispose();
            }
        }

        private void dispose()
        {
            callbacks = null;
            monitors.Remove(key);
        }
    }
}
