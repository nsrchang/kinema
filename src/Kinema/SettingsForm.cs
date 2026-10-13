using System.Globalization;

namespace Kinema;

public class SettingsForm : Form
{
    readonly Settings s;
    readonly Mpv mpv;
    readonly Action<bool> changed;

    public SettingsForm(Settings settings, Mpv mpv, bool vsAvailable, Action<bool> changed, int tab)
    {
        s = settings;
        this.mpv = mpv;
        this.changed = changed;
        Text = "KINEMA 設定";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(580, 480);
        Font = new Font("Microsoft JhengHei UI", 9.5f);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(InterpPage(vsAvailable));
        tabs.TabPages.Add(NvidiaPage());
        tabs.TabPages.Add(SubPage());
        tabs.SelectedIndex = Math.Clamp(tab, 0, 2);

        var close = new Button { Text = "關閉", DialogResult = DialogResult.OK, Width = 90, Height = 30 };
        var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        bottom.Controls.Add(close);
        Controls.Add(tabs);
        Controls.Add(bottom);
        AcceptButton = close;
    }

    TabPage InterpPage(bool vsAvailable)
    {
        int[] fps = { 60, 120, 0 };
        int[] widths = { 1280, 1920, 2560, 3840 };
        return Page("補幀",
            ("引擎", Combo(new[] { "MVTools 動態補償（推薦）", "平滑模式（無動態補償，任何電腦可用）" },
                s.Engine == "mvtools" ? 0 : 1, i => { s.Engine = i == 0 ? "mvtools" : "smooth"; changed(true); })),
            ("目標幀率", Combo(new[] { "60 fps", "120 fps", "跟隨螢幕更新率" },
                Array.IndexOf(fps, s.TargetFps), i => { s.TargetFps = fps[i]; changed(true); })),
            ("解析度上限", Combo(new[] { "720p", "1080p（推薦）", "1440p", "4K（需要高階 CPU）" },
                Array.IndexOf(widths, s.MaxWidth), i => { s.MaxWidth = widths[i]; changed(true); })),
            ("", Check("開啟影片時自動啟用補幀", s.AutoInterp, v => { s.AutoInterp = v; changed(false); })),
            ("狀態", Note(vsAvailable
                ? "動態補償引擎已內建。超過解析度上限、或來源已 ≥ 47 fps 的影片會自動略過。按 Tab 可在統計資訊確認輸出幀率。"
                : "找不到動態補償引擎，補幀會使用平滑模式。")));
    }

    TabPage NvidiaPage() => Page("NVIDIA",
        ("", Check("RTX Video Super Resolution（AI 超解析）", s.Vsr, v => { s.Vsr = v; changed(true); })),
        ("", Check("RTX Video HDR（SDR 轉 HDR）", s.RtxHdr, v => { s.RtxHdr = v; changed(true); })),
        ("說明", Note("需要 NVIDIA RTX 顯示卡與新版驅動，並在 NVIDIA App 或控制面板啟用「RTX 視訊增強」。RTX HDR 需先開啟 Windows HDR。超解析只在影片解析度低於螢幕時作用。")));

    TabPage SubPage()
    {
        var font = new Button { Text = s.SubFont, AutoSize = true };
        font.Click += (_, _) =>
        {
            using var d = new FontDialog { Font = new Font(s.SubFont, 12f), ShowEffects = false };
            if (d.ShowDialog(this) == DialogResult.OK) { s.SubFont = d.Font.Name; font.Text = s.SubFont; changed(false); }
        };

        var color = new Button { Text = s.SubColor, AutoSize = true, BackColor = SafeColor(s.SubColor) };
        color.Click += (_, _) =>
        {
            using var d = new ColorDialog { Color = SafeColor(s.SubColor), FullOpen = true };
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                var c = d.Color;
                s.SubColor = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
                color.BackColor = c;
                color.Text = s.SubColor;
                changed(false);
            }
        };

        var border = new NumericUpDown { Minimum = 0, Maximum = 10, Value = Math.Clamp(s.SubBorder, 0, 10), Width = 80 };
        border.ValueChanged += (_, _) => { s.SubBorder = (int)border.Value; changed(false); };

        double cur = mpv?.GetDouble("sub-delay") ?? 0;
        var delay = new NumericUpDown { Minimum = -600, Maximum = 600, DecimalPlaces = 1, Increment = 0.1M, Width = 100, Value = (decimal)Math.Clamp(cur, -600, 600) };
        delay.ValueChanged += (_, _) => mpv?.Set("sub-delay", ((double)delay.Value).ToString(CultureInfo.InvariantCulture));

        var load = new Button { Text = "載入外部字幕…", AutoSize = true };
        load.Click += (_, _) =>
        {
            using var d = new OpenFileDialog { Filter = "字幕檔|*.srt;*.ass;*.ssa;*.sub;*.idx;*.sup;*.vtt|所有檔案|*.*" };
            if (d.ShowDialog(this) == DialogResult.OK) mpv?.Command("sub-add", d.FileName, "select");
        };

        return Page("字幕",
            ("字型", font),
            ("顏色", color),
            ("大小", Track(50, 250, (int)Math.Round(s.SubScale * 100), v => { s.SubScale = v / 100.0; changed(false); })),
            ("垂直位置", Track(50, 100, s.SubPos, v => { s.SubPos = v; changed(false); })),
            ("外框粗細", border),
            ("時間偏移（秒）", delay),
            ("", Check("強制套用樣式到 ASS 字幕", s.SubAssOverride, v => { s.SubAssOverride = v; changed(false); })),
            ("", load),
            ("快捷鍵", Note("Ctrl+↑/↓ 大小 · Shift+↑/↓ 位置 · Z / X 時間偏移 ±0.1 秒")));
    }

    static Color SafeColor(string hex)
    {
        try { return ColorTranslator.FromHtml(hex); } catch { return Color.White; }
    }

    static TabPage Page(string title, params (string Label, Control Ctl)[] rows)
    {
        var page = new TabPage(title) { Padding = new Padding(12) };
        var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoScroll = true };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        foreach (var (label, ctl) in rows)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var l = new Label { Text = label ?? "", AutoSize = true, Margin = new Padding(0, 8, 8, 8) };
            ctl.Margin = new Padding(0, 4, 0, 8);
            if (ctl is ComboBox || ctl is TrackBar) ctl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            grid.Controls.Add(l);
            grid.Controls.Add(ctl);
        }
        page.Controls.Add(grid);
        return page;
    }

    static ComboBox Combo(string[] items, int sel, Action<int> onChange)
    {
        var c = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        c.Items.AddRange(items);
        c.SelectedIndex = Math.Max(0, sel);
        c.SelectedIndexChanged += (_, _) => onChange(c.SelectedIndex);
        return c;
    }

    static CheckBox Check(string text, bool v, Action<bool> on)
    {
        var c = new CheckBox { Text = text, Checked = v, AutoSize = true };
        c.CheckedChanged += (_, _) => on(c.Checked);
        return c;
    }

    static TrackBar Track(int min, int max, int val, Action<int> on)
    {
        var t = new TrackBar { Minimum = min, Maximum = max, Value = Math.Clamp(val, min, max), TickStyle = TickStyle.None };
        t.ValueChanged += (_, _) => on(t.Value);
        return t;
    }

    static Label Note(string t) => new Label { Text = t, AutoSize = true, MaximumSize = new Size(390, 0), ForeColor = SystemColors.GrayText };
}
