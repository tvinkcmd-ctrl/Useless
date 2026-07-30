using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace UselessChecker
{
    // Modern Button 2026 - эволюция CyberButton с новым дизайном
    // Плавные анимации, глубокое свечение, эффект стекла
    public class ModernButton : Control, IButtonControl
    {
        private float _hover;
        private bool _isHovered;
        private bool _active;
        private DialogResult _dialogResult;
        private float _rippleR, _rippleA;
        private Point _rippleC;
        private readonly System.Windows.Forms.Timer _animTimer, _rippleTimer;
        
        // Свойства
        public Color AccentColor { get; set; } = ModernPalette.AccentPrimary;
        public Color BaseColor { get; set; } = ModernPalette.CardPrimary;
        public int CornerRadius { get; set; } = 14;
        public bool ShowGlow { get; set; } = true;
        public bool ShowRipple { get; set; } = true;
        
        public bool Active
        {
            get => _active;
            set { if (_active != value) { _active = value; Invalidate(); } }
        }
        
        public ModernButton()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI Variable", 10, FontStyle.Medium);
            ForeColor = ModernPalette.TextPrimary;
            
            // Анимация hover
            _animTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animTimer.Tick += (s, e) =>
            {
                float target = _isHovered ? 1f : 0f;
                float diff = target - _hover;
                
                if (Math.Abs(diff) < 0.01f)
                {
                    _hover = target;
                    _animTimer.Stop();
                }
                else
                {
                    _hover += diff * 0.18f; // Плавная интерполяция
                }
                
                Invalidate();
            };
            
            // Ripple эффект
            _rippleTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _rippleTimer.Tick += (s, e) =>
            {
                _rippleR += 4f;
                _rippleA -= 12f;
                
                if (_rippleA <= 0)
                {
                    _rippleA = 0;
                    _rippleTimer.Stop();
                }
                
                Invalidate();
            };
        }
        
        public DialogResult DialogResult 
        { 
            get => _dialogResult; 
            set => _dialogResult = value; 
        }
        
        public void NotifyDefault(bool value) { }
        public void PerformClick() { if (Enabled) OnClick(EventArgs.Empty); }
        
        protected override void OnMouseEnter(EventArgs e) 
        { 
            _isHovered = true; 
            _animTimer.Start(); 
            base.OnMouseEnter(e); 
        }
        
        protected override void OnMouseLeave(EventArgs e) 
        { 
            _isHovered = false; 
            _animTimer.Start(); 
            base.OnMouseLeave(e); 
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && ShowRipple)
            {
                _rippleC = e.Location;
                _rippleR = 2f;
                _rippleA = 180f;
                _rippleTimer.Start();
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            float eff = _active ? 1f : _hover;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int radius = CornerRadius;
            
            using var path = ModernPalette.RoundRect(rect, radius);
            
            // 1) Наружное свечение
            if (ShowGlow && eff > 0.01f)
            {
                int[] alphas = { 30, 18, 9 };
                for (int i = 0; i < alphas.Length; i++)
                {
                    var outer = Rectangle.Inflate(rect, i + 3, i + 3);
                    using var outerPath = ModernPalette.RoundRect(outer, radius + i + 3);
                    using var pen = new Pen(ModernPalette.Alpha(AccentColor, (int)(alphas[i] * eff)), 1f);
                    g.DrawPath(pen, outerPath);
                }
            }
            
            // 2) Основная заливка с градиентом
            Color topCol = ModernPalette.Mix(BaseColor, ModernPalette.CardSecondary, 0.7f);
            Color botCol = ModernPalette.Mix(BaseColor, Color.Black, 0.15f);
            
            if (_active)
            {
                topCol = ModernPalette.Mix(topCol, AccentColor, 0.12f);
            }
            
            using (var bgGrad = new LinearGradientBrush(rect, topCol, botCol, 90f))
            {
                g.FillPath(bgGrad, path);
            }
            
            g.SetClip(path);
            
            // 3) Объём через мягкий свет сверху
            int hlHeight = Math.Max(2, (int)(rect.Height * 0.35f));
            var hlRect = new Rectangle(rect.X, rect.Y, rect.Width, hlHeight);
            using (var highlight = new LinearGradientBrush(
                hlRect,
                ModernPalette.Alpha(Color.White, 20),
                Color.Transparent,
                90f))
            {
                g.FillRectangle(highlight, hlRect);
            }
            
            // 4) Тень снизу
            int shHeight = Math.Max(2, (int)(rect.Height * 0.30f));
            var shRect = new Rectangle(rect.X, rect.Bottom - shHeight, rect.Width, shHeight);
            using (var shadow = new LinearGradientBrush(
                shRect,
                Color.Transparent,
                ModernPalette.Alpha(Color.Black, 35),
                90f))
            {
                g.FillRectangle(shadow, shRect);
            }
            
            // 5) Акцентный оверлей при hover/active
            if (eff > 0.01f)
            {
                using (var accentOverlay = new LinearGradientBrush(
                    rect,
                    ModernPalette.Alpha(AccentColor, (int)(50 * eff)),
                    ModernPalette.Alpha(AccentColor, (int)(18 * eff)),
                    90f))
                {
                    g.FillRectangle(accentOverlay, rect);
                }
            }
            
            // 6) Ripple эффект
            if (_rippleA > 0 && ShowRipple)
            {
                using (var rippleBrush = new SolidBrush(
                    ModernPalette.Alpha(AccentColor, (int)_rippleA)))
                {
                    g.FillEllipse(rippleBrush, 
                        _rippleC.X - _rippleR, 
                        _rippleC.Y - _rippleR, 
                        _rippleR * 2, 
                        _rippleR * 2);
                }
            }
            
            // 7) Индикатор активной вкладки (левая полоса)
            if (_active)
            {
                var barWidth = 4;
                var barHeight = rect.Height - 16;
                var barX = rect.X + 3;
                var barY = rect.Y + 8;
                
                // Свечение полосы
                using (var barGlow = new LinearGradientBrush(
                    new Rectangle(barX - 2, barY, barWidth + 16, barHeight),
                    ModernPalette.Alpha(AccentColor, 140),
                    Color.Transparent,
                    0f))
                {
                    g.FillRectangle(barGlow, barX - 2, barY, barWidth + 16, barHeight);
                }
                
                // Сама полоса
                using (var barGrad = new LinearGradientBrush(
                    new Rectangle(barX, barY, barWidth, barHeight),
                    ModernPalette.Alpha(AccentColor, 200),
                    AccentColor,
                    90f))
                {
                    g.FillRectangle(barGrad, barX, barY, barWidth, barHeight);
                }
            }
            
            g.ResetClip();
            
            // 8) Граница
            Color borderCol = _active 
                ? AccentColor 
                : ModernPalette.Mix(ModernPalette.BorderDefault, AccentColor, eff * 0.5f);
            
            using (var border = new Pen(
                ModernPalette.Alpha(borderCol, _active ? 100 : 65), 
                1f + eff * 0.6f))
            {
                g.DrawPath(border, path);
            }
            
            // 9) Блик на верхней кромке
            using (var topHi = new Pen(
                ModernPalette.Alpha(Color.White, 45 + (int)(eff * 35)), 
                1f))
            {
                g.DrawLine(topHi, rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
            }
            
            // 10) Текст
            Color textColor = eff > 0.6f 
                ? Color.White 
                : ModernPalette.Mix(ForeColor, Color.White, _hover * 0.35f);
            
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | 
                TextFormatFlags.EndEllipsis | TextFormatFlags.GlyphOverhangPadding);
        }
        
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }
        
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Invalidate();
        }
    }
}
