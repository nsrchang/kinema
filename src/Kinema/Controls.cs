namespace Kinema;

static class Theme
{
    public static readonly Color Bg = ColorTranslator.FromHtml("#1d2d3d");
    public static readonly Color Panel = ColorTranslator.FromHtml("#2c455d");
    public static readonly Color Accent = ColorTranslator.FromHtml("#5980a6");
    public static readonly Color Text = ColorTranslator.FromHtml("#f2f2f3");
    public static readonly Color Track = ColorTranslator.FromHtml("#416180");
}

class SeekBar : Control
{
    public double Value, Max;
    public bool Dragging { get; private set; }
    public event Action<double> Seek;

    public SeekBar()
    {
        DoubleBuffered = true;
        Height = 20;
        Cursor = Cursors.Hand;
        BackColor = Theme.Bg;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        int y = Height / 2 - 2;
        using var bg = new SolidBrush(Theme.Track);
        using var fg = new SolidBrush(Theme.Accent);
        e.Graphics.FillRectangle(bg, 0, y, Width, 4);
        if (Max > 0) e.Graphics.FillRectangle(fg, 0, y, (int)(Width * Math.Clamp(Value / Max, 0, 1)), 4);
    }

    protected override void OnMouseDown(MouseEventArgs e) { Dragging = true; Fire(e.X); }
    protected override void OnMouseMove(MouseEventArgs e) { if (Dragging) Fire(e.X); }
    protected override void OnMouseUp(MouseEventArgs e) { if (Dragging) Fire(e.X); Dragging = false; }

    void Fire(int x)
    {
        if (Max <= 0 || Width <= 0) return;
        Value = Math.Clamp((double)x / Width, 0, 1) * Max;
        Invalidate();
        Seek?.Invoke(Value);
    }
}
