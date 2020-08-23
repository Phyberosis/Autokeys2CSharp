using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Events
{
    public class EventHandle<T>
    {
        private LinkedList<Action<T>> listeners = new LinkedList<Action<T>>();

        public void Notify()
        {
            Notify(default);
        }

        public void Notify(T o)
        {
            foreach(var a in listeners)
            {
                Task.Delay(0).ContinueWith((t) => { a(o); });
            }
        }

        public void RegisterListener(Action<T> l)
        {
            listeners.AddLast(l);
            //Console.WriteLine("l "+listeners.Count);
        }

        public void RemoveListener(Action<T> l)
        {
            listeners.Remove(l);
            //Console.WriteLine("l "+listeners.Count);
        }
    }

    public enum EventID
    {
        REC, TEST, TEST2
    }

    public static class EventsBuiltin
    {
        private struct GenericHandle
        {
            private object handle;
            public EventHandle<T> GetHandle<T>()
            {
                return (EventHandle<T>) handle;
            }
            public GenericHandle(object handle)
            {
                this.handle = handle;
            }
        }


        private static Dictionary<EventID, GenericHandle> builtin = new Dictionary<EventID, GenericHandle>();

        public static EventHandle<object> RegisterEvent(EventID id)
        {
            return  RegisterEvent<object>(id);
        }

        public static EventHandle<T> RegisterEvent<T>(EventID id)
        {
            lock(builtin)
            {
                EventHandle<T> handle;
                if (builtin.ContainsKey(id))
                {
                    handle = builtin[id].GetHandle<T>();
                }
                else
                {
                    handle = new EventHandle<T>();
                    builtin.Add(id, new GenericHandle(handle));
                }

                //Console.WriteLine("b " + builtin.Count);
                return handle;
            }
        }

        public static void RegisterListener(EventID id, Action<object> callback)
        {
            RegisterListener<object>(id, callback);
        }

        public static void RegisterListener<T>(EventID id, Action<T> callback)
        {
            lock(builtin)
            {
                EventHandle<T> handle;
                if (!builtin.ContainsKey(id))
                {
                    handle = new EventHandle<T>();
                    builtin.Add(id, new GenericHandle(handle));
                }
                else
                {
                    handle = builtin[id].GetHandle<T>();
                }

                //Console.WriteLine("b " + builtin.Count);
                handle.RegisterListener(callback);
            }
        }

        public static void RemoveListener(EventID id, Action<object> callback)
        {
            RemoveListener<object>(id, callback);
        }

        public static void RemoveListener<T>(EventID id, Action<T> callback)
        {
            lock(builtin)
            {
                builtin[id].GetHandle<T>().RemoveListener(callback);
            }
        }
    }
}
