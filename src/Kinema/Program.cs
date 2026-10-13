using System.IO.Pipes;
using System.Text;

namespace Kinema;

static class Program
{
    public const string PipeName = "KINEMA.Player.Pipe";

    [STAThread]
    static void Main(string[] args)
    {
        var files = args.Where(a => !a.StartsWith("--"))
            .Select(a => a.Contains("://") ? a : SafeFull(a)).ToArray();

        using var mutex = new Mutex(true, "KINEMA.Player.SingleInstance", out bool first);
        if (!first)
        {
            try
            {
                using var c = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
                c.Connect(2000);
                using var w = new StreamWriter(c, new UTF8Encoding(false));
                w.Write(string.Join("\n", files));
                return;
            }
            catch { }
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm(files));
    }

    static string SafeFull(string p)
    {
        try { return Path.GetFullPath(p); } catch { return p; }
    }
}
