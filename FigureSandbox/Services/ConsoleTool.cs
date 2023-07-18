using System.Runtime.InteropServices;

namespace FigureSandbox.Services;

public static class ConsoleTool
{
    [DllImport("Kernel32")]
    private static extern void AllocConsole();

    [DllImport("Kernel32")]
    private static extern void FreeConsole();

    public static void ShowConsole() => AllocConsole();
    public static void CloseConsole() => FreeConsole();
}
