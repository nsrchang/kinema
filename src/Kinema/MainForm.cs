using System.Globalization;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;

namespace Kinema;

public class MainForm : Form
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool SetDllDirectory(string lpPathName);

    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    static readonly string[] SubExts = { ".srt", ".ass", ".ssa", ".sub", ".idx", ".sup", ".vtt" };

    readonly Settings S = Settings.Load();
    readonly Panel video;
    readonly Panel bar;
    readonly ContextMenuStrip menu = new ContextMenuStrip();
    readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 250 };
    readonly string[] pending;
    SeekBar seek;
    Label timeLbl;
    Button playBtn, interpBtn, vsrBtn, hdrBtn;
    Mpv mpv;
    bool interpOn, vsFailed, fullscreen, closing;
    FormWindowState prevState;
    string lastChain;

    string VsDir => Path.Combine(AppContext.BaseDirectory, "vs");
    bool VsAvailable => !vsFailed && File.Exists(Path.Combine(VsDir, "VSScript.dll"));
    bool UseMvtools => S.Engine == "mvtools" && VsAvailable;

    public MainForm(string[] files)
    {
        pending = files;
        Text = "KINEMA";
        BackColor = Theme.Bg;
        ForeColor = Theme.Text;
        Font = new Font("Microsoft JhengHei UI", 9.5f);
        ClientSize = new Size(1280, 780);
        MinimumSize = new Size(640, 400);
        StartPosition = FormStartPosition.CenterScreen;

        video = new Panel { Dock = DockStyle.Fill, BackColor = Color.Black };
        bar = BuildBar();
        Controls.Add(video);
        Controls.Add(bar);

        interpOn = S.AutoInterp;
        if (Directory.Exists(VsDir))
        {
            SetDllDirectory(VsDir);
            Environment.SetEnvironmentVariable("PATH", VsDir + ";" + Environment.GetEnvironmentVariable("PATH"));
        }
        timer.Tick += (_, _) => Tick();
    }

    Panel BuildBar()
    {
        var p = new Panel { Dock = DockStyle.Bottom, Height = 68, BackColor = Theme.Bg, Padding = new Padding(12, 4, 12, 8) };
        seek = new SeekBar { Dock = DockStyle.Top };
        seek.Seek += v => mpv?.Command("seek", v.ToString("0.###", Inv), "absolute");
        var row = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(0, 6, 0, 0) };
        playBtn = Btn("▶", () => mpv?.Command("cycle", "pause"), 44);
        var openBtn = Btn("開啟", OpenDialog, 60);
        timeLbl = new Label { AutoSize = true, ForeColor = Theme.Text, Margin = new Padding(8, 8, 16, 0), Text = "0:00:00 / 0:00:00" };
        interpBtn = Btn("補幀 關", ToggleInterp, 130);
        vsrBtn = Btn("RTX VSR", () => ToggleNv(true), 84);
        hdrBtn = Btn("RTX HDR", () => ToggleNv(false), 84);
        var subBtn = Btn("字幕", () => ShowSettings(2), 60);
        var setBtn = Btn("設定", () => ShowSettings(0), 60);
        var fsBtn = Btn("全螢幕", ToggleFullscreen, 70);
        row.Controls.AddRange(new Control[] { playBtn, openBtn, timeLbl, interpBtn, vsrBtn, hdrBtn, subBtn, setBtn, fsBtn });
        p.Controls.Add(row);
        p.Controls.Add(seek);
        return p;
    }

    static Button Btn(string text, Action a, int w)
    {
        var b = new Button
        {
            Text = text, Width = w, Height = 32, FlatStyle = FlatStyle.Flat, TabStop = false,
            ForeColor = Theme.Text, BackColor = Theme.Bg, Margin = new Padding(0, 0, 6, 0)
        };
        b.FlatAppearance.BorderColor = Theme.Track;
        b.FlatAppearance.MouseOverBackColor = Theme.Panel;
        b.FlatAppearance.MouseDownBackColor = Theme.Accent;
        b.Click += (_, _) => a();
        return b;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        try { InitMpv(); }
        catch (Exception ex)
        {
            MessageBox.Show(this, "無法載入播放核心 libmpv-2.dll：\n" + ex.Message, "KINEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
        }
        StartPipe();
        if (pending != null && pending.Length > 0) OpenFiles(pending);
    }

    void InitMpv()
    {
        Directory.CreateDirectory(Settings.Dir);
        var inputConf = Path.Combine(Settings.Dir, "kinema-input.conf");
        File.WriteAllText(inputConf, InputConf, new UTF8Encoding(false));

        mpv = new Mpv();
        var opts = new (string, string)[]
        {
            ("wid", video.Handle.ToInt64().ToString(Inv)),
            ("config", "yes"), ("config-dir", Settings.Dir), ("input-conf", inputConf),
            ("input-default-bindings", "yes"), ("input-vo-keyboard", "yes"),
            ("osc", "yes"), ("script-opts", "osc-visibility=never"),
            ("vo", "gpu-next"), ("gpu-api", "d3d11"), ("gpu-context", "d3d11"),
            ("hwdec", "auto-copy"), ("hwdec-codecs", "all"), ("profile", "high-quality"),
            ("video-sync", "display-resample"), ("tscale", "oversample"),
            ("keep-open", "yes"), ("idle", "yes"), ("force-window", "yes"),
            ("save-position-on-quit", "yes"), ("watch-later-dir", Path.Combine(Settings.Dir, "watch_later")),
            ("alang", "jpn,ja,chi,zho,zh,eng,en"), ("slang", "zh-TW,zht,cht,chi,zho,zh,eng,en"),
            ("sub-auto", "fuzzy"), ("osd-font", "Microsoft JhengHei"),
            ("screenshot-directory", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)),
            ("volume", S.Volume.ToString(Inv)),
        };
        foreach (var (k, v) in opts) mpv.SetOption(k, v);
        mpv.Event += OnMpvEvent;
        mpv.Initialize();
        ApplySubStyle();
        ApplyFilters(true);
        UpdateButtons();
        timer.Start();
    }

    void ApplyFilters(bool force = false)
    {
        if (mpv == null) return;
        var parts = new List<string>();
        if (interpOn && UseMvtools)
        {
            var vpy = WriteVpy();
            parts.Add($"@kinema:vapoursynth=file=%{Encoding.UTF8.GetByteCount(vpy)}%{vpy}:buffered-frames=4:concurrent-frames=8");
        }
        mpv.Set("interpolation", interpOn && !UseMvtools ? "yes" : "no");

        if (S.Vsr || S.RtxHdr)
        {
            var o = new List<string> { S.RtxHdr ? "format=x2bgr10" : "format=nv12" };
            double scale = VsrScale();
            if (S.Vsr && scale > 1.05)
            {
                o.Add("scale=" + scale.ToString("0.0", Inv));
                o.Add("scaling-mode=nvidia");
            }
            if (S.RtxHdr) o.Add("nvidia-true-hdr");
            if (o.Count > 1) parts.Add("@kinema-nv:d3d11vpp=" + string.Join(":", o));
        }
        mpv.Set("target-colorspace-hint", S.RtxHdr ? "yes" : "no");

        var chain = string.Join(",", parts);
        if (!force && chain == lastChain) return;
        lastChain = chain;
        mpv.Command("vf", "set", chain);
    }

    double VsrScale()
    {
        double vh = mpv.GetDouble("height"), dh = mpv.GetDouble("display-height");
        if (vh <= 0 || dh <= 0) return 1;
        return Math.Clamp(Math.Floor(dh / vh * 10) / 10, 1, 4);
    }

    string WriteVpy()
    {
        string plugin = Directory.Exists(VsDir)
            ? Directory.GetFiles(VsDir, "*mvtools*.dll", SearchOption.AllDirectories).FirstOrDefault()
            : null;
        var script = VpyTemplate
            .Replace("__PLUGIN__", plugin == null ? "    pass" : $"    core.std.LoadPlugin(r'{plugin}')")
            .Replace("__TARGET__", S.TargetFps.ToString(Inv))
            .Replace("__MAXW__", S.MaxWidth.ToString(Inv));
        var path = Path.Combine(Settings.Dir, "motion.vpy");
        File.WriteAllText(path, script, new UTF8Encoding(false));
        return path;
    }

    void Tick()
    {
        if (mpv == null) return;
        double pos = mpv.GetDouble("time-pos"), dur = mpv.GetDouble("duration");
        if (!seek.Dragging) { seek.Max = dur; seek.Value = pos; seek.Invalidate(); }
        timeLbl.Text = $"{Fmt(pos)} / {Fmt(dur)}";
        playBtn.Text = mpv.Get("pause") == "yes" ? "▶" : "❚❚";
    }

    static string Fmt(double s)
    {
        var t = TimeSpan.FromSeconds(Math.Max(0, s));
        return $"{(int)t.TotalHours}:{t.Minutes:00}:{t.Seconds:00}";
    }

    void OnMpvEvent(int id, IntPtr data)
    {
        switch (id)
        {
            case Mpv.EvShutdown:
                UI(() => { if (!closing) Close(); });
                break;
            case Mpv.EvLog:
                var log = Mpv.ReadLog(data);
                UI(() => HandleError(log.Prefix, log.Text));
                break;
            case Mpv.EvClientMessage:
                var args = Mpv.ReadClientMessage(data);
                if (args.Length > 0) UI(() => OnMessage(args[0]));
                break;
            case Mpv.EvVideoReconfig:
                UI(() => { if (S.Vsr) ApplyFilters(); });
                break;
            case Mpv.EvFileLoaded:
                UI(() => { Text = (mpv?.Get("media-title") ?? "") + " — KINEMA"; });
                break;
        }
    }

    void UI(Action a)
    {
        if (closing || !IsHandleCreated) return;
        try { BeginInvoke(a); } catch (InvalidOperationException) { }
    }

    void HandleError(string prefix, string text)
    {
        var t = text.ToLowerInvariant();
        var p = prefix.ToLowerInvariant();
        if (t.Contains("kinema-nv") || p.Contains("d3d11vpp"))
        {
            if (!S.Vsr && !S.RtxHdr) return;
            S.Vsr = false; S.RtxHdr = false; S.Save();
            ApplyFilters(true); UpdateButtons();
            Osd("NVIDIA 影像增強無法啟用（需要 NVIDIA RTX 顯示卡）");
        }
        else if (t.Contains("kinema") || p.Contains("vapoursynth"))
        {
            if (!interpOn || !UseMvtools) return;
            vsFailed = true;
            ApplyFilters(true); UpdateButtons();
            Osd("動態補償引擎載入失敗，已改用平滑模式");
        }
    }

    void OnMessage(string m)
    {
        switch (m)
        {
            case "kinema-interp": ToggleInterp(); break;
            case "kinema-vsr": ToggleNv(true); break;
            case "kinema-hdr": ToggleNv(false); break;
            case "kinema-fullscreen": ToggleFullscreen(); break;
            case "kinema-exitfs": if (fullscreen) ToggleFullscreen(); break;
            case "kinema-menu": ShowMenu(); break;
            case "kinema-open": OpenDialog(); break;
            case "kinema-subs": ShowSettings(2); break;
        }
    }

    string EngineName => UseMvtools
        ? "MVTools 動態補償 → " + (S.TargetFps == 0 ? "螢幕更新率" : S.TargetFps + " fps")
        : "平滑模式";

    void ToggleInterp()
    {
        interpOn = !interpOn;
        ApplyFilters();
        UpdateButtons();
        Osd(interpOn ? "補幀 開啟 · " + EngineName : "補幀 關閉");
    }

    void ToggleNv(bool vsr)
    {
        if (vsr) S.Vsr = !S.Vsr; else S.RtxHdr = !S.RtxHdr;
        S.Save();
        ApplyFilters();
        UpdateButtons();
        Osd(vsr ? "RTX Video Super Resolution " + (S.Vsr ? "開啟" : "關閉")
                : "RTX Video HDR " + (S.RtxHdr ? "開啟" : "關閉"));
    }

    void Osd(string s) => mpv?.Command("show-text", s, "2000");

    void UpdateButtons()
    {
        interpBtn.Text = !interpOn ? "補幀 關"
            : UseMvtools ? "補幀 開 · " + (S.TargetFps == 0 ? "螢幕" : S.TargetFps.ToString(Inv))
            : "補幀 開 · 平滑";
        SetOn(interpBtn, interpOn);
        SetOn(vsrBtn, S.Vsr);
        SetOn(hdrBtn, S.RtxHdr);
    }

    static void SetOn(Button b, bool on)
    {
        b.BackColor = on ? Theme.Accent : Theme.Bg;
        b.FlatAppearance.BorderColor = on ? Theme.Accent : Theme.Track;
    }

    void ToggleFullscreen()
    {
        fullscreen = !fullscreen;
        if (fullscreen)
        {
            prevState = WindowState;
            bar.Visible = false;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Normal;
            WindowState = FormWindowState.Maximized;
            mpv?.Command("script-message", "osc-visibility", "auto", "no-osd");
        }
        else
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = prevState;
            bar.Visible = true;
            mpv?.Command("script-message", "osc-visibility", "never", "no-osd");
        }
    }

    public void OpenFiles(IEnumerable<string> files)
    {
        if (mpv == null) return;
        bool first = true;
        foreach (var f in files)
        {
            if (SubExts.Contains(Path.GetExtension(f).ToLowerInvariant()))
            {
                mpv.Command("sub-add", f, "select");
                continue;
            }
            mpv.Command("loadfile", f, first ? "replace" : "append");
            first = false;
        }
    }

    void OpenDialog()
    {
        using var d = new OpenFileDialog
        {
            Title = "開啟影片",
            Multiselect = true,
            Filter = "影片檔|*.mkv;*.mp4;*.avi;*.mov;*.wmv;*.flv;*.webm;*.m4v;*.ts;*.m2ts;*.mts;*.mpg;*.mpeg;*.vob;*.3gp;*.rmvb;*.rm;*.asf;*.ogv;*.f4v|所有檔案|*.*"
        };
        if (d.ShowDialog(this) == DialogResult.OK) OpenFiles(d.FileNames);
    }

    void LoadSubDialog()
    {
        using var d = new OpenFileDialog { Title = "載入字幕", Filter = "字幕檔|*.srt;*.ass;*.ssa;*.sub;*.idx;*.sup;*.vtt|所有檔案|*.*" };
        if (d.ShowDialog(this) == DialogResult.OK) mpv?.Command("sub-add", d.FileName, "select");
    }

    void ShowMenu()
    {
        if (mpv == null) return;
        menu.Items.Clear();
        Item("開啟檔案…", "Ctrl+O", OpenDialog);
        Item("載入字幕…", null, LoadSubDialog);
        menu.Items.Add(new ToolStripSeparator());
        Item("補幀", "I", ToggleInterp, interpOn);
        Item("RTX Video Super Resolution", "Ctrl+R", () => ToggleNv(true), S.Vsr);
        Item("RTX Video HDR", "Ctrl+H", () => ToggleNv(false), S.RtxHdr);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(Tracks("音軌", "audio", "aid"));
        menu.Items.Add(Tracks("字幕軌", "sub", "sid"));
        Item("字幕調整…", "Ctrl+S", () => ShowSettings(2));
        Item("設定…", null, () => ShowSettings(0));
        menu.Items.Add(new ToolStripSeparator());
        Item("全螢幕", "Enter", ToggleFullscreen, fullscreen);
        Item("結束", null, Close);
        menu.Show(Cursor.Position);
    }

    void Item(string text, string keys, Action a, bool check = false)
    {
        var it = new ToolStripMenuItem(text) { ShortcutKeyDisplayString = keys, Checked = check };
        it.Click += (_, _) => a();
        menu.Items.Add(it);
    }

    ToolStripMenuItem Tracks(string title, string type, string prop)
    {
        var root = new ToolStripMenuItem(title);
        int n = (int)mpv.GetDouble("track-list/count");
        for (int i = 0; i < n; i++)
        {
            if (mpv.Get($"track-list/{i}/type") != type) continue;
            var id = mpv.Get($"track-list/{i}/id");
            var parts = new[] { mpv.Get($"track-list/{i}/lang"), mpv.Get($"track-list/{i}/title"), mpv.Get($"track-list/{i}/codec") }
                .Where(x => !string.IsNullOrEmpty(x));
            var it = new ToolStripMenuItem($"{id}. {string.Join(" · ", parts)}") { Checked = mpv.Get($"track-list/{i}/selected") == "yes" };
            it.Click += (_, _) => mpv?.Set(prop, id);
            root.DropDownItems.Add(it);
        }
        if (type == "sub")
        {
            var off = new ToolStripMenuItem("關閉字幕");
            off.Click += (_, _) => mpv?.Set("sid", "no");
            root.DropDownItems.Add(off);
        }
        if (root.DropDownItems.Count == 0) root.Enabled = false;
        return root;
    }

    void ShowSettings(int tab)
    {
        using var f = new SettingsForm(S, mpv, VsAvailable, filters =>
        {
            S.Save();
            ApplySubStyle();
            if (filters) ApplyFilters(true);
            UpdateButtons();
        }, tab);
        f.ShowDialog(this);
    }

    void ApplySubStyle()
    {
        if (mpv == null) return;
        mpv.Set("sub-scale", S.SubScale.ToString("0.00", Inv));
        mpv.Set("sub-pos", S.SubPos.ToString(Inv));
        mpv.Set("sub-font", S.SubFont);
        mpv.Set("sub-color", S.SubColor);
        if (mpv.Set("sub-outline-size", S.SubBorder.ToString(Inv)) < 0)
            mpv.Set("sub-border-size", S.SubBorder.ToString(Inv));
        mpv.Set("sub-ass-override", S.SubAssOverride ? "force" : "yes");
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (mpv != null)
        {
            var code = keyData & Keys.KeyCode;
            string k = code switch
            {
                Keys.Space => "SPACE", Keys.Left => "LEFT", Keys.Right => "RIGHT", Keys.Up => "UP", Keys.Down => "DOWN",
                Keys.Enter => "ENTER", Keys.Escape => "ESC", Keys.PageUp => "PGUP", Keys.PageDown => "PGDWN", Keys.Tab => "TAB",
                >= Keys.A and <= Keys.Z => ((char)('a' + (code - Keys.A))).ToString(),
                _ => null
            };
            if (k != null)
            {
                bool letter = k.Length == 1;
                if ((keyData & Keys.Shift) != 0) k = letter ? k.ToUpperInvariant() : "Shift+" + k;
                if ((keyData & Keys.Alt) != 0) k = "Alt+" + k;
                if ((keyData & Keys.Control) != 0) k = "Ctrl+" + k;
                mpv.Command("keypress", k);
                return true;
            }
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    void StartPipe()
    {
        Task.Run(async () =>
        {
            while (!closing)
            {
                try
                {
                    using var s = new NamedPipeServerStream(Program.PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    await s.WaitForConnectionAsync();
                    using var r = new StreamReader(s, Encoding.UTF8);
                    var files = (await r.ReadToEndAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    UI(() =>
                    {
                        if (files.Length > 0) OpenFiles(files);
                        if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;
                        Activate();
                    });
                }
                catch { await Task.Delay(300); }
            }
        });
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        closing = true;
        timer.Stop();
        if (mpv != null)
        {
            var vol = mpv.GetDouble("volume");
            if (vol > 0) S.Volume = (int)vol;
            if (!string.IsNullOrEmpty(mpv.Get("path"))) mpv.Command("write-watch-later-config");
            S.Save();
            mpv.Dispose();
            mpv = null;
        }
        base.OnFormClosing(e);
    }

    const string InputConf = @"# KINEMA 快捷鍵（每次啟動自動產生；自訂請改用同資料夾的 input.conf）
SPACE cycle pause
MBTN_LEFT cycle pause
MBTN_LEFT_DBL script-message kinema-fullscreen
MBTN_RIGHT script-message kinema-menu
ENTER script-message kinema-fullscreen
Alt+ENTER script-message kinema-fullscreen
ESC script-message kinema-exitfs
LEFT seek -5
RIGHT seek 5
Ctrl+LEFT seek -30
Ctrl+RIGHT seek 30
UP add volume 5
DOWN add volume -5
WHEEL_UP add volume 5
WHEEL_DOWN add volume -5
PGUP playlist-prev
PGDWN playlist-next
a cycle audio
s cycle sub
TAB script-binding stats/display-stats-toggle
i script-message kinema-interp
Ctrl+r script-message kinema-vsr
Ctrl+h script-message kinema-hdr
Ctrl+s script-message kinema-subs
Ctrl+o script-message kinema-open
Ctrl+UP add sub-scale 0.05
Ctrl+DOWN add sub-scale -0.05
Shift+UP add sub-pos -1
Shift+DOWN add sub-pos 1
z add sub-delay -0.1
x add sub-delay 0.1
Ctrl+p screenshot
";

    const string VpyTemplate = @"import vapoursynth as vs
core = vs.core
if not hasattr(core, 'mv'):
__PLUGIN__

TARGET_FPS = __TARGET__
MAX_WIDTH = __MAXW__

clip = video_in
src = container_fps if container_fps and container_fps > 0 else 23.976
dst = TARGET_FPS if TARGET_FPS > 0 else (display_fps if display_fps and display_fps > 0 else 60)

if src >= 47 or dst <= src * 1.05 or clip.width > MAX_WIDTH:
    clip.set_output()
else:
    clip = core.std.AssumeFPS(clip, fpsnum=int(round(src * 1000)), fpsden=1000)
    sup = core.mv.Super(clip, pel=2, hpad=16, vpad=16)
    bv = core.mv.Analyse(sup, blksize=16, isb=True, chroma=True, search=3, searchparam=2)
    fv = core.mv.Analyse(sup, blksize=16, isb=False, chroma=True, search=3, searchparam=2)
    clip = core.mv.BlockFPS(clip, sup, bv, fv, num=int(round(dst * 1000)), den=1000, mode=3, thscd1=300, thscd2=100)
    clip.set_output()
";
}
