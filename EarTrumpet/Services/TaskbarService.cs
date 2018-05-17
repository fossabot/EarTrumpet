﻿using EarTrumpet.Interop;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EarTrumpet.Services
{
    public sealed class TaskbarService
    {
        private const string _className = "Shell_TrayWnd";

        public static TaskbarState GetWinTaskbarState()
        {
            var appbar = new Shell32.APPBARDATA();

            var hwnd = User32.FindWindowW(_className, null);

            appbar.cbSize = Marshal.SizeOf(appbar);
            appbar.uEdge = 0;
            appbar.hWnd = hwnd;
            appbar.lParam = 1;

            User32.GetWindowRect(hwnd, out Win32.RECT scaledTaskbarRect);

            var taskbarNonDPIAwareSize = Shell32.SHAppBarMessage((int)ABMsg.ABM_GETTASKBARPOS, ref appbar);
            var scalingAmount = (double)(scaledTaskbarRect.Bottom - scaledTaskbarRect.Top) / (appbar.rc.Bottom - appbar.rc.Top);

            TaskbarState retState = new TaskbarState();
            retState.TaskbarSize.Top = (int)(appbar.rc.Top * scalingAmount);
            retState.TaskbarSize.Bottom = (int)(appbar.rc.Bottom * scalingAmount);
            retState.TaskbarSize.Left = (int)(appbar.rc.Left * scalingAmount);
            retState.TaskbarSize.Right = (int)(appbar.rc.Right * scalingAmount);

            var screen = Screen.AllScreens.FirstOrDefault(x => x.Bounds.Contains(
                new Rectangle(
                    retState.TaskbarSize.Left,
                    retState.TaskbarSize.Top,
                    retState.TaskbarSize.Right - retState.TaskbarSize.Left,
                    retState.TaskbarSize.Bottom - retState.TaskbarSize.Top)
            ));

            retState.TaskbarScreen = screen;
            retState.TaskbarPosition = TaskbarPosition.Bottom;

            if (screen != null)
            {
                if (retState.TaskbarSize.Bottom == screen.Bounds.Bottom && retState.TaskbarSize.Top == screen.Bounds.Top)
                {
                    retState.TaskbarPosition = (retState.TaskbarSize.Left == screen.Bounds.Left) ? TaskbarPosition.Left : TaskbarPosition.Right;
                }
                if (retState.TaskbarSize.Right == screen.Bounds.Right && retState.TaskbarSize.Left == screen.Bounds.Left)
                {
                    retState.TaskbarPosition = (retState.TaskbarSize.Top == screen.Bounds.Top) ? TaskbarPosition.Top : TaskbarPosition.Bottom;
                }
            }

            return retState;
        }
    }

    public enum ABMsg
    {
        ABM_NEW = 0,
        ABM_REMOVE,
        ABM_QUERYPOS,
        ABM_SETPOS,
        ABM_GETSTATE,
        ABM_GETTASKBARPOS,
        ABM_ACTIVATE,
        ABM_GETAUTOHIDEBAR,
        ABM_SETAUTOHIDEBAR,
        ABM_WINDOWPOSCHANGED,
        ABM_SETSTATE
    }

    public struct TaskbarState
    {
        public TaskbarPosition TaskbarPosition;
        public Win32.RECT TaskbarSize;
        public Screen TaskbarScreen;
    }

    public enum TaskbarPosition
    {
        Top,
        Left,
        Right,
        Bottom
    }
}