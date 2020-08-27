using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Data;
using InputHook;

namespace Monitors
{
    public class GestureMonitor
    {
        private LinkedList<Action> callBacks;
        private bool waiting = false;

        private static GestureMonitor instance = null;

        private GestureMonitor()
        {
            callBacks = new LinkedList<Action>();
        }

        public static void WaitForStopRec(Action callback)
        {
            if (instance == null) instance = new GestureMonitor();
            lock(instance)
            {

                instance.callBacks.AddLast(callback);
                if (instance.waiting) return;

                instance.waitForStopRec();
            }
        }

        private void waitForStopRec()
        {
            Hook hook = Hook.I();
            Hook.OnKeyDelegate tempOnU = (k) => { };
            Hook.OnKeyDelegate tempOnD = null;
            tempOnD = (k) =>
            {
                //Console.WriteLine(k);
                // one at a time
                if (k != Key.Escape) return;
                                                          //Console.WriteLine("es");
                hook.RemoveKeyHook(tempOnD, tempOnU);

                Task.Delay(0).ContinueWith((t) =>
                {
                    float lastT = Time.Now(), startT, lastPol = lastT;
                    const float maxDelay = 0.3f;
                    const float minRun = 0.7f;
                    const float maxRun = 1f;
                    const int minTravel = 25;
                    Point lastP = CursorMonitor.Position();
                    LinkedList<Point> points = new LinkedList<Point>();
                    points.AddLast(lastP);

                    while (lastP == CursorMonitor.Position()) { }
                    startT = Time.Now();

                    while (true)
                    {
                        Point p = CursorMonitor.Position();
                        if (Math.Abs(p.X - lastP.X) > minTravel &&
                            Math.Abs(p.Y - lastP.Y) > minTravel)
                        {
                            //Console.WriteLine("pol");
                            points.AddLast(CursorMonitor.Position());
                            lastPol = Time.Now();
                            lastP = CursorMonitor.Position();
                        }

                        float now = Time.Now();
                        if ((now - lastPol > maxDelay && now - startT > minRun) || now - startT > maxRun)
                        {
                            if (evalCirc(points))
                            {
                                foreach(Action c in callBacks)
                                {
                                    c();
                                }
                                callBacks.Clear();
                                //Console.WriteLine("det");
                            }
                            else
                            {
                                hook.AddKeyHook(tempOnD, tempOnU);
                                //Console.WriteLine("fail");
                            }
                            break;
                        }

                        lastT = Time.Now();
                    }
                });

            };
            hook.AddKeyHook(tempOnD, tempOnU);
        }

        private bool evalCirc(LinkedList<Point> points)
        {
            if (points.Count < 3) return false;

            Point f = points.First.Value, l = points.Last.Value;
            float ex = f.X - l.X, ey = f.Y - l.Y;

            points.RemoveLast();
            Point center = new Point();
            foreach (Point p in points)
            {
                center.X += p.X;
                center.Y += p.Y;
                //Console.WriteLine(p);
            }
            float cx = center.X / (float)points.Count;
            float cy = center.Y / (float)points.Count;
            //Console.WriteLine("{0}, {1}", cx, cy);
            float minD = float.MaxValue, maxD = float.MinValue;
            foreach (Point p in points)
            {
                float dx = p.X - cx, dy = p.Y - cy;
                float d = (float)Math.Sqrt((dx * dx) + (dy * dy));
                minD = d < minD ? d : minD;
                maxD = d > maxD ? d : maxD;
            }

            //Console.WriteLine("{0}, {1}, {2}, {3}", maxD, minD, (maxD - minD) / maxD, (float)Math.Sqrt((ex * ex) + (ey * ey)));
            return (maxD - minD) / maxD < 0.5 && (float)Math.Sqrt((ex * ex) + (ey * ey)) < maxD;
        }
    }
}
