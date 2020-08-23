using System;

public enum MouseAction
{
    WM_LBUTTONDOWN = 0x0201,
    WM_LBUTTONUP = 0x0202,

    WM_MOUSEMOVE = 0x0200,
    WM_MOUSEWHEEL = 0x020A,

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
        return a.StartsWith("WM_L") ? "Left" : a.StartsWith("WM_R") ? "Right" : a.StartsWith("WM_MB") ? "Middle" : "Move";
    }

    public static string GetTypePrefix(MouseAction ma)
    {
        string a = ma.ToString();
        return a.EndsWith("N") ? "down at" : !a.EndsWith("E")? "up at" : "move to";
    }
}