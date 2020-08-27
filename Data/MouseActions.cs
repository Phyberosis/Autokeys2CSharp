using System;

public enum MouseAction
{
    NONE = 0,

    WM_LBUTTONDOWN = 0x0201,
    WM_LBUTTONUP = 0x0202,

    WM_MOUSEMOVE = 0x0200,
    //WM_MOUSEWHEEL = 0x020A,

    WM_RBUTTONDOWN = 0x0204,
    WM_RBUTTONUP = 0x0205,

    WM_MBUTTONDOWN = 0x207,
    WM_MBUTTONUP = 0x208
}

public static class MouseActionVerbalizer
{
    public static string Convert(MouseAction ma)
    {
        string a = ma.ToString();
        string r =  a.StartsWith("WM_L") ? "Left" : a.StartsWith("WM_R") ? "Right" : a.StartsWith("WM_MB") ? "Middle" : "Move";
        return r.StartsWith("Mo")? r : r + ", ";
    }

    public static string GetType(MouseAction ma)
    {
        string a = ma.ToString();
        return a.EndsWith("N") ? " down" : !a.EndsWith("E")? " up" : "";
    }
}