using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace UselessChecker
{
    // Современная палитра Modern 2026 - эволюция текущей цветовой гаммы
    // Сохраняем винные/красные акценты, но делаем их более благородными и глубокими
    public static class ModernPalette
    {
        // Основные фоны - глубокие, богатые оттенки
        public static readonly Color BackgroundDeep = Color.FromArgb(8, 6, 10);
        public static readonly Color BackgroundSurface = Color.FromArgb(14, 12, 18);
        public static readonly Color BackgroundElevated = Color.FromArgb(22, 18, 26);
        
        // Карточки и панели - многослойные с микро-градиентами
        public static readonly Color CardPrimary = Color.FromArgb(28, 24, 34);
        public static readonly Color CardSecondary = Color.FromArgb(34, 28, 40);
        public static readonly Color CardHover = Color.FromArgb(42, 36, 50);
        public static readonly Color CardActive = Color.FromArgb(48, 40, 58);
        
        // Акцентные цвета - эволюция красной гаммы
        public static readonly Color AccentPrimary = Color.FromArgb(235, 65, 88);      // Основной неон (эволюция AccentNeon)
        public static readonly Color AccentDeep = Color.FromArgb(165, 28, 50);         // Глубокий красный
        public static readonly Color AccentSoft = Color.FromArgb(210, 100, 115);       // Мягкий розово-красный
        public static readonly Color AccentGlow = Color.FromArgb(255, 110, 130);       // Свечение
        public static readonly Color AccentWarm = Color.FromArgb(198, 75, 92);         // Тёплый акцент
        
        // Дополнительные акценты
        public static readonly Color AccentCyan = Color.FromArgb(88, 145, 200);        // Холодный циан
        public static readonly Color AccentEmerald = Color.FromArgb(78, 160, 115);     // Изумруд для успеха
        public static readonly Color AccentAmber = Color.FromArgb(220, 165, 80);       // Янтарь для предупреждений
        
        // Текст
        public static readonly Color TextPrimary = Color.FromArgb(250, 248, 252);      // Основной текст
        public static readonly Color TextSecondary = Color.FromArgb(165, 160, 175);    // Вторичный текст
        public static readonly Color TextMuted = Color.FromArgb(110, 105, 120);        // Приглушенный текст
        public static readonly Color TextDisabled = Color.FromArgb(75, 70, 85);        // Неактивный текст
        
        // Границы и разделители
        public static readonly Color BorderSubtle = Color.FromArgb(45, 40, 55);
        public static readonly Color BorderDefault = Color.FromArgb(60, 52, 70);
        public static readonly Color BorderFocus = Color.FromArgb(95, 75, 95);
        
        // Эффекты
        public static readonly Color ShadowLight = Color.FromArgb(15, 12, 18);
        public static readonly Color ShadowMedium = Color.FromArgb(25, 20, 28);
        public static readonly Color ShadowHeavy = Color.FromArgb(35, 28, 40);
        
        // Утилиты
        public static Color Alpha(Color c, int a) => Color.FromArgb(Math.Max(0, Math.Min(255, a)), c.R, c.G, c.B);
        
        public static Color Mix(Color a, Color b, float t)
        {
            t = Math.Max(0, Math.Min(1, t));
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }
        
        public static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            
            if (diameter > r.Width) diameter = r.Width;
            if (diameter > r.Height) diameter = r.Height;
            
            if (diameter <= 2)
            {
                path.AddRectangle(r);
                return path;
            }
            
            path.AddArc(r.X, r.Y, diameter, diameter, 180, 90);
            path.AddArc(r.Right - diameter, r.Y, diameter, diameter, 270, 90);
            path.AddArc(r.Right - diameter, r.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(r.X, r.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }
        
        // Рисование мягкого градиентного пятна (ambient light)
        public static void DrawAmbientBlob(Graphics g, float cx, float cy, float rx, float ry, Color color)
        {
            using var path = new GraphicsPath();
            path.AddEllipse(cx - rx, cy - ry, rx * 2, ry * 2);
            
            using var brush = new PathGradientBrush(path)
            {
                CenterColor = color,
                SurroundColors = new[] { Color.Transparent },
                CenterPoint = new PointF(cx, cy)
            };
            
            g.FillPath(brush, path);
        }
        
        // Современный живой фон с глубокими градиентами и ambient-светом
        public static void DrawModernBackground(Graphics g, Rectangle rect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            
            float w = rect.Width, h = rect.Height;
            
            // Базовый градиент
            using (var baseGrad = new LinearGradientBrush(
                rect,
                BackgroundDeep,
                Color.FromArgb(12, 10, 15),
                135f))
            {
                g.FillRectangle(baseGrad, rect);
            }
            
            // Ambient свет - несколько слоёв для глубины
            DrawAmbientBlob(g, w * 0.85f, h * 0.12f, w * 0.45f, h * 0.50f, Alpha(AccentPrimary, 35));
            DrawAmbientBlob(g, w * 0.15f, h * 0.88f, w * 0.42f, h * 0.48f, Alpha(AccentDeep, 40));
            DrawAmbientBlob(g, w * 0.50f, h * 0.50f, w * 0.30f, h * 0.35f, Alpha(AccentWarm, 18));
            DrawAmbientBlob(g, w * 0.92f, h * 0.65f, w * 0.25f, h * 0.30f, Alpha(AccentCyan, 12));
            
            // Виньетка по краям
            using (var vignette = new PathGradientBrush(new[]
            {
                new PointF(0, 0), new PointF(w, 0), new PointF(w, h), new PointF(0, h)
            }))
            {
                vignette.CenterColor = Color.Transparent;
                vignette.SurroundColors = new[] { Alpha(Color.Black, 100) };
                g.FillRectangle(vignette, rect);
            }
        }
        
        // Современная карточка с эффектом стекла и объёмом
        public static void DrawModernCard(Graphics g, Rectangle rect, int radius, float elevation, bool hover = false, bool active = false)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            using var path = RoundRect(rect, radius);
            
            // Базовая заливка с микро-градиентом
            Color baseColor = active ? CardActive : (hover ? CardHover : CardPrimary);
            Color topColor = Mix(baseColor, Color.White, 0.03f);
            Color botColor = Mix(baseColor, Color.Black, 0.08f);
            
            using (var fillGrad = new LinearGradientBrush(rect, topColor, botColor, 90f))
            {
                g.FillPath(fillGrad, path);
            }
            
            g.SetClip(path);
            
            // Объём через мягкий свет сверху
            int highlightHeight = Math.Max(2, (int)(rect.Height * 0.35f));
            var highlightRect = new Rectangle(rect.X, rect.Y, rect.Width, highlightHeight);
            using (var highlight = new LinearGradientBrush(
                highlightRect,
                Alpha(Color.White, 18),
                Color.Transparent,
                90f))
            {
                g.FillRectangle(highlight, highlightRect);
            }
            
            // Тень снизу
            int shadowHeight = Math.Max(2, (int)(rect.Height * 0.30f));
            var shadowRect = new Rectangle(rect.X, rect.Bottom - shadowHeight, rect.Width, shadowHeight);
            using (var shadow = new LinearGradientBrush(
                shadowRect,
                Color.Transparent,
                Alpha(Color.Black, 35),
                90f))
            {
                g.FillRectangle(shadow, shadowRect);
            }
            
            // Акцентное свечение при hover/active
            if (hover || active)
            {
                float intensity = active ? 1f : (hover ? 0.6f : 0f);
                
                // Внутренняя обводка
                for (int i = 0; i < 3; i++)
                {
                    var innerRect = new Rectangle(
                        rect.X + i, rect.Y + i,
                        rect.Width - i * 2, rect.Height - i * 2);
                    using var innerPath = RoundRect(innerRect, Math.Max(1, radius - i));
                    using var pen = new Pen(
                        Alpha(active ? AccentPrimary : AccentSoft, (int)(40 * intensity)),
                        1f);
                    g.DrawPath(pen, innerPath);
                }
                
                // Тёплый оверлей
                using (var warmOverlay = new LinearGradientBrush(
                    rect,
                    Alpha(AccentWarm, (int)(25 * intensity)),
                    Alpha(AccentDeep, (int)(10 * intensity)),
                    45f))
                {
                    g.FillRectangle(warmOverlay, rect);
                }
            }
            
            g.ResetClip();
            
            // Граница
            Color borderColor = active ? AccentPrimary : (hover ? BorderFocus : BorderSubtle);
            using (var border = new Pen(Alpha(borderColor, active ? 80 : 50), 1f))
            {
                g.DrawPath(border, path);
            }
            
            // Блик на верхней кромке
            using (var topHighlight = new Pen(Alpha(Color.White, 35), 1f))
            {
                g.DrawLine(topHighlight, rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
            }
        }
        
        // Современная кнопка
        public static void DrawModernButton(Graphics g, Rectangle rect, int radius, 
            Color accentColor, float hover, bool active = false)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            using var path = RoundRect(rect, radius);
            
            // Наружное свечение
            if (hover > 0.01f || active)
            {
                float intensity = active ? 1f : hover;
                for (int i = 0; i < 3; i++)
                {
                    var outerRect = Rectangle.Inflate(rect, i + 2, i + 2);
                    using var outerPath = RoundRect(outerRect, radius + i + 2);
                    using var pen = new Pen(Alpha(accentColor, (int)(new[] { 25, 15, 8 }[i] * intensity)), 1f);
                    g.DrawPath(pen, outerPath);
                }
            }
            
            // Основная заливка
            Color baseColor = active ? CardActive : CardPrimary;
            Color topCol = Mix(baseColor, accentColor, hover * 0.15f);
            Color botCol = Mix(baseColor, Color.Black, 0.12f);
            
            using (var bgGrad = new LinearGradientBrush(rect, topCol, botCol, 90f))
            {
                g.FillPath(bgGrad, path);
            }
            
            g.SetClip(path);
            
            // Свет сверху
            int hlH = Math.Max(2, (int)(rect.Height * 0.35f));
            var hlRect = new Rectangle(rect.X, rect.Y, rect.Width, hlH);
            using (var hl = new LinearGradientBrush(hlRect, Alpha(Color.White, 22), Color.Transparent, 90f))
            {
                g.FillRectangle(hl, hlRect);
            }
            
            // Акцентный оверлей
            if (hover > 0.01f || active)
            {
                float intensity = active ? 1f : hover;
                using (var accentOverlay = new LinearGradientBrush(
                    rect,
                    Alpha(accentColor, (int)(45 * intensity)),
                    Alpha(accentColor, (int)(15 * intensity)),
                    90f))
                {
                    g.FillRectangle(accentOverlay, rect);
                }
            }
            
            g.ResetClip();
            
            // Граница
            Color borderCol = active ? accentColor : Mix(BorderDefault, accentColor, hover * 0.5f);
            using (var border = new Pen(Alpha(borderCol, active ? 90 : 60), 1f + hover * 0.5f))
            {
                g.DrawPath(border, path);
            }
            
            // Верхний блик
            using (var topHi = new Pen(Alpha(Color.White, 50 + (int)(hover * 30)), 1f))
            {
                g.DrawLine(topHi, rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
            }
        }
    }
}
