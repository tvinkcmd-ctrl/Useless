using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace UselessChecker
{
    // Палитра Modern Glass 2026 + движок отрисовки. 
    // Богатая винная гамма с глубокими тонами, неоновыми акцентами и премиальным стеклом.
    public static class CyberPalette
    {
        public static readonly Color Background     = Color.FromArgb(12, 8, 14);
        public static readonly Color BackgroundWarm = Color.FromArgb(18, 10, 19);
        public static readonly Color PanelBg        = Color.FromArgb(24, 18, 28);
        public static readonly Color CardBg         = Color.FromArgb(32, 22, 38);
        public static readonly Color CardBgTop      = Color.FromArgb(42, 28, 50);
        public static readonly Color CardHover      = Color.FromArgb(48, 32, 56);

        public static readonly Color AccentNeon     = Color.FromArgb(255, 64, 108);
        public static readonly Color AccentRedDeep  = Color.FromArgb(180, 24, 58);
        public static readonly Color AccentRedMuted = Color.FromArgb(200, 76, 98);
        public static readonly Color AccentGlow     = Color.FromArgb(255, 120, 154);
        public static readonly Color AccentCyan     = Color.FromArgb(86, 180, 230);
        public static readonly Color AccentEmerald  = Color.FromArgb(72, 196, 140);

        public static readonly Color TextPrimary    = Color.FromArgb(250, 248, 252);
        public static readonly Color TextSecondary  = Color.FromArgb(170, 165, 180);
        public static readonly Color TextDark       = Color.FromArgb(120, 115, 130);
        public static readonly Color BorderColor    = Color.FromArgb(58, 48, 68);
        public static readonly Color BorderHover    = Color.FromArgb(140, 90, 118);

        public static Color Alpha(Color c, int a) => Color.FromArgb(Clamp(a), c.R, c.G, c.B);

        public static Color Mix(Color a, Color b, float t)
        {
            t = t < 0 ? 0 : t > 1 ? 1 : t;
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        private static int Clamp(int v) => v < 0 ? 0 : v > 255 ? 255 : v;

        public static GraphicsPath Round(Rectangle r, int rad)
        {
            var p = new GraphicsPath();
            int d = rad * 2;
            if (d > r.Width) d = r.Width;
            if (d > r.Height) d = r.Height;
            if (d <= 0) { p.AddRectangle(r); return p; }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static void DrawBlob(Graphics g, float cx, float cy, float rx, float ry, Color core)
        {
            var rect = new RectangleF(cx - rx, cy - ry, rx * 2, ry * 2);
            using var path = new GraphicsPath();
            path.AddEllipse(rect);
            using var brush = new PathGradientBrush(path)
            {
                CenterColor = core,
                SurroundColors = new[] { Color.Transparent },
                CenterPoint = new PointF(cx, cy)
            };
            g.FillPath(brush, path);
        }

        // Живой фон зоны (главное окно, модалки). Глубокий винный ambient с неоновыми заревами.
        public static void DrawAmbientBackground(Graphics g, Rectangle rect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            float w = rect.Width, h = rect.Height;

            using (var bg = new LinearGradientBrush(rect, Background, BackgroundWarm, 90f))
                g.FillRectangle(bg, rect);

            DrawBlob(g, w * 0.85f, h * 0.12f, w * 0.48f, h * 0.62f, Alpha(AccentNeon, 36));
            DrawBlob(g, w * 0.08f, h * 0.88f, w * 0.44f, h * 0.58f, Alpha(AccentRedDeep, 52));
            DrawBlob(g, w * 0.72f, h * 0.68f, w * 0.38f, h * 0.48f, Alpha(AccentGlow, 28));
            DrawBlob(g, w * 0.25f, h * 0.42f, w * 0.32f, h * 0.40f, Alpha(Color.FromArgb(48, 32, 68), 34));

            using (var vig = new PathGradientBrush(new[] {
                new PointF(0, 0), new PointF(w, 0), new PointF(w, h), new PointF(0, h) }))
            {
                vig.CenterColor = Color.Transparent;
                vig.SurroundColors = new[] { Alpha(Color.Black, 130) };
                g.FillRectangle(vig, rect);
            }
        }

        // Премиальный материал-стекло 2026: глубокая тонировка, объём одним градиентом,
        // мягкие блики и тени без швов, внутреннее свечение при наведении.
        public static void DrawGlassSurface(Graphics g, Rectangle rect, int radius, Color tint, float hover, Color accent)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            using var path = Round(rect, radius);

            // 1) Глубокая тонировка с alpha-вариацией
            int baseAlpha = 215 + (int)(hover * 35);
            using (var fill = new SolidBrush(Alpha(tint, baseAlpha)))
                g.FillPath(fill, path);

            g.SetClip(path);

            // 2) Объём одним плавным градиентом через ColorBlend
            using (var vol = new LinearGradientBrush(rect, Color.White, Color.Black, 90f))
            {
                var blend = new ColorBlend(3);
                blend.Colors = new[] { Alpha(Color.White, 18), Alpha(tint, 0), Alpha(Color.Black, 36) };
                blend.Positions = new[] { 0f, 0.5f, 1f };
                vol.InterpolationColors = blend;
                g.FillRectangle(vol, rect);
            }

            // 3) Тёплый акцентный подъём при hover/active
            if (hover > 0.01f)
                using (var warm = new LinearGradientBrush(rect,
                    Alpha(accent, (int)(hover * 48)), Alpha(accent, (int)(hover * 16)), 90f))
                    g.FillRectangle(warm, rect);

            // 4) Верхний блик — только верхняя треть
            int hiH = Math.Max(2, (int)(rect.Height * 0.38f));
            var hiRect = new Rectangle(rect.X, rect.Y, rect.Width, hiH);
            using (var hl = new LinearGradientBrush(hiRect, Alpha(Color.White, 36), Color.Transparent, 90f))
                g.FillRectangle(hl, hiRect);

            // 5) Нижняя тень — только нижняя треть
            int shH = Math.Max(2, (int)(rect.Height * 0.38f));
            var shRect = new Rectangle(rect.X, rect.Bottom - shH, rect.Width, shH);
            using (var sh = new LinearGradientBrush(shRect, Color.Transparent, Alpha(Color.Black, 48), 90f))
                g.FillRectangle(sh, shRect);

            // 6) Внутреннее акцентное свечение при hover/active
            if (hover > 0.01f)
            {
                int[] al = { 70, 42, 20 };
                for (int i = 0; i < al.Length; i++)
                {
                    var inner = new Rectangle(rect.X + i + 1, rect.Y + i + 1, rect.Width - (i + 1) * 2, rect.Height - (i + 1) * 2);
                    using var ip = Round(inner, Math.Max(1, radius - i - 1));
                    using var pen = new Pen(Alpha(accent, (int)(al[i] * hover)), 1f);
                    g.DrawPath(pen, ip);
                }
            }
            g.ResetClip();

            // 7) Светлый контур + яркий блик по верхней кромке 1px
            using (var border = new Pen(Alpha(Color.White, 36 + (int)(hover * 28)), 1f))
                g.DrawPath(border, path);
            using (var topHi = new Pen(Alpha(Color.White, 72), 1f))
                g.DrawLine(topHi, rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
        }
    }
}