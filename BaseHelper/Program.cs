﻿namespace BaseHelper
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using OfficeOpenXml;

    public class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            var ofd = new OpenFileDialog();
            ofd.ShowDialog();
            var package = new ExcelPackage(new FileInfo(ofd.FileName));

            var worksheet = package.Workbook.Worksheets[1];
            var colCount = worksheet.Dimension.End.Column;
            var rowCount = worksheet.Dimension.End.Row;

            var result = string.Empty;

            for (var i = 1; i <= rowCount; i++)
            {
                var rowStr = "<Item ";
                for (var j = 1; j <= colCount; j++)
                {
                    rowStr += $"Prop{j}=\"{worksheet.Cells[i,j].Text.Replace('.', ',')}\" ";
                }

                rowStr += "></Item>";

                result += rowStr + Environment.NewLine;
            }
            
            ShowTextWithNotepad(result);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        private static extern int SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        public static void ShowTextWithNotepad(string message, string title = null)
        {
            var notepad = Process.Start(new ProcessStartInfo("notepad.exe"));
            if (notepad != null)
            {
                notepad.WaitForInputIdle();

                if (!string.IsNullOrEmpty(title))
                    SetWindowText(notepad.MainWindowHandle, title);

                if (!string.IsNullOrEmpty(message))
                {
                    var child = FindWindowEx(notepad.MainWindowHandle, new IntPtr(0), "Edit", null);
                    SendMessage(child, 0x000C, 0, message);
                }
            }
        }
    }
}
