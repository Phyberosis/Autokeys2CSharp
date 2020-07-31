using InputHook;
using MouseSimulator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Data;
using System.Windows.Interop;

namespace Autokeys2
{
    internal class InputHandler
    {
        private Hook hook = Hook.I();
        private Keys run = Keys.RMenu;
        private Keys stop = Keys.Escape;

        public delegate void CallBack();

        public void WaitForStopRec(CallBack callback)
        {
            Hook.OnKeyDelegate tempOnU = (k) => { };
            Hook.OnKeyDelegate tempOnD = null;
            tempOnD = (k) =>
            {
                //Console.WriteLine(k);
                // one at a time
                if (k != stop && k!= Keys.None) return; // why fist esc is none???
                //Console.WriteLine("es");
                hook.RemoveKeyHook(tempOnD, tempOnU);

                Task.Delay(0).ContinueWith((t) =>
                {
                    float lastT = Time.Now(), startT, lastPol = lastT;
                    const float maxDelay = 0.3f;
                    const float minRun = 0.7f;
                    const float maxRun = 1f;
                    const int minTravel = 25;
                    Point lastP = Cursor.Position;
                    LinkedList<Point> points = new LinkedList<Point>();
                    points.AddLast(Cursor.Position);

                    while (lastP == Cursor.Position) { }
                    startT = Time.Now();

                    while (true)
                    {
                        if (Math.Abs(Cursor.Position.X - lastP.X) > minTravel &&
                            Math.Abs(Cursor.Position.Y - lastP.Y) > minTravel)
                        {
                            //Console.WriteLine("pol");
                            points.AddLast(Cursor.Position);
                            lastPol = Time.Now();
                            lastP = Cursor.Position;
                        }

                        float now = Time.Now();
                        if ((now - lastPol > maxDelay && now - startT > minRun) || now - startT > maxRun)
                        {
                            if (evalCirc(points))
                            {
                                callback();
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
            foreach(Point p in points)
            {
                float dx = p.X - cx, dy = p.Y - cy;
                float d = (float)Math.Sqrt((dx * dx) + (dy * dy));
                minD = d < minD ? d : minD;
                maxD = d > maxD ? d : maxD;
            }

            //Console.WriteLine("{0}, {1}, {2}, {3}", maxD, minD, (maxD - minD) / maxD, (float)Math.Sqrt((ex * ex) + (ey * ey)));
            return (maxD - minD) / maxD < 0.5 && (float)Math.Sqrt((ex * ex) + (ey * ey)) < maxD;
        }

        public void WaitForRun(CallBack callBack)
        {

        }

        // todo map k to more readable
        public void SetRun(Keys k)
        {
            run = k;
        }

        public Keys GetRun() { return run; }

        public void SetStop(Keys k)
        {
            stop = k;
        }

        public Keys GetStop() { return stop; }
    }
}

//                if (k != stop) return;
//                lock(lck)
//                {
//                    if (startMouse) return;
//                    startMouse = true;
//                    Console.WriteLine("escape");
//                }

//                float lastT = 0;
//const float maxDelay = 0.3f;

//bool bigEnough = false;
//double minDiameter = Math.Pow(Screen.PrimaryScreen.Bounds.Height / 4, 2);
//double maxClose = Math.Pow(Screen.PrimaryScreen.Bounds.Height / 70, 2);
//int minTravel = 5;

//Angle lastAngle = new Angle();
//Angle maxAngle = Math.PI / 4;
//Point start = Cursor.Position;
//Point lastP = new Point(start.X, start.Y);

//Hook.OnMouseDelegate tempOnM = null;
//tempOnM = (ma, x, y) =>
//                {

                    
//                };

//Action<string> stop = (string msg) =>
//{
//    hook.RemoveMouseHook(tempOnM);
//    Console.WriteLine("stopped: {0} | {1}, {2}", msg, x, y);
//    lock (lck) startMouse = false;
//    return;
//};

//                    if (ma != MouseAction.WM_MOUSEMOVE) return;

//                    if (Time.Now() - lastT > maxDelay)
//                        stop((Time.Now() - lastT).ToString()); // no pauses
//                    else
//                        lastT = Time.Now();

//                    if (lastP == start)       // first move
//                    {
//                        lastAngle = Trig.AngleBetween(lastP, new Point(x, y));
//                        Console.WriteLine("circ begin {0} | {1}, {2}", lastAngle, x, y);
//                        lastP = new Point(x, y);
//                        return;
//                    }

//                    int dx = x - lastP.X, dy = y - lastP.Y;
//                    if ((dx* dx) + (dy* dy) > minDiameter) bigEnough = true;  // big enough

//                    if (dx<minTravel && dy<minTravel) return;               // greater dist = better acc
//                    Angle currAngle = Trig.AngleBetween(lastP, new Point(x, y));// smooth circle
//                    if (Math.Abs((double)(lastAngle - currAngle)) > maxAngle) stop(lastAngle + " " + currAngle);

//int tx = x - start.X, ty = y - start.Y;
//bool closed = (tx * tx) + (ty * ty) < maxClose;
//                    if (bigEnough && closed)                                    // circle detected
//                    {
//                        hook.RemoveKeyHook(tempOnD, tempOnU); 
//                        stop("fin");
//callback();
//Console.WriteLine("detected");
//                    }

//                    lastP = new Point(x, y);