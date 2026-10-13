using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Kinema;

public sealed class Mpv : IDisposable
{
    const string Lib = "libmpv-2.dll";
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern IntPtr mpv_create();
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern int mpv_initialize(IntPtr ctx);
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern void mpv_terminate_destroy(IntPtr ctx);
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern int mpv_set_option_string(IntPtr ctx, byte[] name, byte[] data);
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern int mpv_set_property_string(IntPtr ctx, byte[] name, byte[] data);
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern IntPtr mpv_get_property_string(IntPtr ctx, byte[] name);
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern int mpv_command(IntPtr ctx, IntPtr[] args);
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern void mpv_free(IntPtr data);
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern IntPtr mpv_wait_event(IntPtr ctx, double timeout);
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern void mpv_wakeup(IntPtr ctx);
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] static extern int mpv_request_log_messages(IntPtr ctx, byte[] minLevel);

    public const int EvShutdown = 1, EvLog = 2, EvEndFile = 7, EvFileLoaded = 8, EvClientMessage = 16, EvVideoReconfig = 17;

    IntPtr handle;
    Thread thread;
    volatile bool running;
    public event Action<int, IntPtr> Event;

    static byte[] U8(string s) => Encoding.UTF8.GetBytes((s ?? "") + "\0");

    public Mpv()
    {
        handle = mpv_create();
        if (handle == IntPtr.Zero) throw new InvalidOperationException("mpv_create failed");
    }

    public int SetOption(string name, string value) => mpv_set_option_string(handle, U8(name), U8(value));

    public void Initialize()
    {
        int r = mpv_initialize(handle);
        if (r < 0) throw new InvalidOperationException("mpv_initialize failed: " + r);
        mpv_request_log_messages(handle, U8("error"));
        running = true;
        thread = new Thread(Loop) { IsBackground = true, Name = "mpv-events" };
        thread.Start();
    }

    public int Set(string name, string value) =>
        handle == IntPtr.Zero ? -1 : mpv_set_property_string(handle, U8(name), U8(value));

    public string Get(string name)
    {
        if (handle == IntPtr.Zero) return null;
        var p = mpv_get_property_string(handle, U8(name));
        if (p == IntPtr.Zero) return null;
        var s = Marshal.PtrToStringUTF8(p);
        mpv_free(p);
        return s;
    }

    public double GetDouble(string name) =>
        double.TryParse(Get(name), NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0;

    public int Command(params string[] args)
    {
        if (handle == IntPtr.Zero) return -1;
        var ptrs = new IntPtr[args.Length + 1];
        try
        {
            for (int i = 0; i < args.Length; i++) ptrs[i] = Marshal.StringToCoTaskMemUTF8(args[i] ?? "");
            return mpv_command(handle, ptrs);
        }
        finally
        {
            foreach (var p in ptrs) if (p != IntPtr.Zero) Marshal.FreeCoTaskMem(p);
        }
    }

    void Loop()
    {
        while (running)
        {
            var ev = mpv_wait_event(handle, -1);
            if (ev == IntPtr.Zero) continue;
            int id = Marshal.ReadInt32(ev);
            if (id == 0) continue;
            var data = Marshal.ReadIntPtr(ev, 16);
            try { Event?.Invoke(id, data); } catch { }
            if (id == EvShutdown) break;
        }
    }

    public static string[] ReadClientMessage(IntPtr data)
    {
        if (data == IntPtr.Zero) return Array.Empty<string>();
        int n = Marshal.ReadInt32(data);
        var arr = Marshal.ReadIntPtr(data, 8);
        var r = new string[n];
        for (int i = 0; i < n; i++) r[i] = Marshal.PtrToStringUTF8(Marshal.ReadIntPtr(arr, i * IntPtr.Size)) ?? "";
        return r;
    }

    public static (string Prefix, string Text) ReadLog(IntPtr data)
    {
        if (data == IntPtr.Zero) return ("", "");
        return (Marshal.PtrToStringUTF8(Marshal.ReadIntPtr(data, 0)) ?? "",
                Marshal.PtrToStringUTF8(Marshal.ReadIntPtr(data, 16)) ?? "");
    }

    public void Dispose()
    {
        if (handle == IntPtr.Zero) return;
        running = false;
        mpv_wakeup(handle);
        thread?.Join(1000);
        mpv_terminate_destroy(handle);
        handle = IntPtr.Zero;
    }
}
