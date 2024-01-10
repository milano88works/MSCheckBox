using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace milano88.UI.Controls
{
    [DefaultEvent("CheckedChanged")]

    public class MSCheckBox : Control
    {
        private bool _checked = false;

        public EventHandler CheckedChanged;
        private BufferedGraphics _bufGraphics;

        private Color checkedColor = Color.DarkGray;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "DarkGray")]
        public Color CheckedColor
        {
            get { return checkedColor; }
            set { checkedColor = value; this.Invalidate(); }
        }

        private Color unCheckedColor = Color.DarkGray;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "DarkGray")]
        public Color UnCheckedColor
        {
            get { return unCheckedColor; }
            set { unCheckedColor = value; this.Invalidate(); }
        }

        private Color borderColorUnchecked = Color.Gray;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color BorderColorUnchecked
        {
            get { return borderColorUnchecked; }
            set { borderColorUnchecked = value; this.Invalidate(); }
        }

        private Color borderColorChecked = Color.Gray;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color BorderColorChecked
        {
            get { return borderColorChecked; }
            set { borderColorChecked = value; this.Invalidate(); }
        }

        private Color tickColor = Color.White;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "White")]
        public Color TickColor
        {
            get { return tickColor; }
            set { tickColor = value; this.Invalidate(); }
        }

        [DefaultValue(typeof(Font), "Segoe UI, 9pt")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                if (DesignMode)
                    UpdateControlSize();
            }
        }

        public enum Style { Rectangle, Rounded }
        private Style _style;
        [Category("Custom Properties")]
        [DefaultValue(Style.Rectangle)]
        public Style CheckBoxStyle
        {
            get { return _style; }
            set { _style = value; this.Invalidate(); }
        }

        [Category("Custom Properties")]
        [DefaultValue(typeof(bool), "False")]
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                this.Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [Category("Custom Properties")]
        [DefaultValue("MSCheckBox")]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                UpdateControlSize();
            }
        }

        private GraphicsPath GetFigurePath(RectangleF rect, int radius)
        {
            rect = new RectangleF(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        public MSCheckBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.Transparent;
            UpdateGraphicsBuffer();
        }

        private void UpdateGraphicsBuffer()
        {
            if (this.Width > 0 && this.Height > 0)
            {
                BufferedGraphicsContext context = BufferedGraphicsManager.Current;
                context.MaximumBuffer = new Size(Width + 1, Height + 1);
                _bufGraphics = context.Allocate(CreateGraphics(), ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            _bufGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int checkBoxSize = 14;
            Size tickSize = TextRenderer.MeasureText(((char)0xE10B).ToString(), new Font("Segoe UI Symbol", 6F));

            RectangleF rectBorder = new RectangleF()
            {
                X = 0,
                Y = (Height - checkBoxSize) / 2,
                Width = checkBoxSize,
                Height = checkBoxSize
            };

            RectangleF rectCheck = new RectangleF()
            {
                X = rectBorder.X + ((rectBorder.Width - tickSize.Width) / 2),
                Y = rectBorder.Y + ((rectBorder.Height - tickSize.Height) / 2),
                Width = tickSize.Width,
                Height = tickSize.Height
            };

            RectangleF rectBorderRound = new RectangleF()
            {
                X = 0,
                Y = (Height - 15) / 2,
                Width = 15,
                Height = 15
            };

            RectangleF rectCheckRound = new RectangleF()
            {
                X = rectBorderRound.X + ((rectBorderRound.Width - tickSize.Width) / 2) + 1F,
                Y = rectBorderRound.Y + ((rectBorderRound.Height - tickSize.Height) / 2) + 1F,
                Width = tickSize.Width,
                Height = tickSize.Height
            };

            using (SolidBrush brushCheckedColor = new SolidBrush(checkedColor))
            using (SolidBrush brushUnCheckedColor = new SolidBrush(unCheckedColor))
            using (SolidBrush brushTick = new SolidBrush(tickColor))
            {
                if (_style == Style.Rectangle)
                {
                    using (GraphicsPath path = GetFigurePath(rectBorder, 2))
                    using (Pen pen = new Pen(Color.FromArgb(180, unCheckedColor)))
                    {
                        if (_checked)
                        {
                            _bufGraphics.Graphics.FillPath(brushCheckedColor, path);
                            _bufGraphics.Graphics.DrawPath(pen, path);
                            using (Pen penBorder = new Pen(borderColorChecked, 1F))
                                _bufGraphics.Graphics.DrawPath(penBorder, path);
                            TextRenderer.DrawText(_bufGraphics.Graphics, ((char)0xE10B).ToString(), new Font("Segoe UI Symbol", 6F), new Point((int)rectCheck.X, (int)rectCheck.Y), tickColor);
                        }
                        else
                        {
                            _bufGraphics.Graphics.FillPath(brushUnCheckedColor, path);
                            _bufGraphics.Graphics.DrawPath(pen, path);
                            using (Pen penBorder = new Pen(borderColorUnchecked, 1F))
                                _bufGraphics.Graphics.DrawPath(penBorder, path);
                        }
                    }
                }
                else if (_style == Style.Rounded)
                {
                    if (_checked)
                    {
                        _bufGraphics.Graphics.FillEllipse(brushCheckedColor, rectBorderRound);
                        using (Pen penBorder = new Pen(borderColorChecked, 1F))
                            _bufGraphics.Graphics.DrawEllipse(penBorder, rectBorderRound);
                        TextRenderer.DrawText(_bufGraphics.Graphics, ((char)0xE10B).ToString(), new Font("Segoe UI Symbol", 6F),
                            new Point((int)rectCheckRound.X, (int)rectCheckRound.Y), tickColor);
                    }
                    else
                    {
                        _bufGraphics.Graphics.FillEllipse(brushUnCheckedColor, rectBorderRound);
                        using (Pen penBorder = new Pen(borderColorUnchecked, 1F))
                            _bufGraphics.Graphics.DrawEllipse(penBorder, rectBorderRound);
                    }
                }

                TextRenderer.DrawText(_bufGraphics.Graphics, Text, Font, new Point(checkBoxSize + 3, (Height - TextRenderer.MeasureText(Text, Font).Height) / 2), ForeColor);
            }

            _bufGraphics.Render(pevent.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null && BackColor == Color.Transparent)
            {
                Rectangle rect = new Rectangle(Left, Top, Width, Height);
                _bufGraphics.Graphics.TranslateTransform(-rect.X, -rect.Y);
                try
                {
                    using (PaintEventArgs pea = new PaintEventArgs(_bufGraphics.Graphics, rect))
                    {
                        pea.Graphics.SetClip(rect);
                        InvokePaintBackground(Parent, pea);
                        InvokePaint(Parent, pea);
                    }
                }
                finally
                {
                    _bufGraphics.Graphics.TranslateTransform(rect.X, rect.Y);
                }
            }
            else
            {
                using (SolidBrush backColor = new SolidBrush(BackColor))
                    _bufGraphics.Graphics.FillRectangle(backColor, ClientRectangle);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
                _checked = !_checked ? true : false;
            this.Invalidate();
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (DesignMode)
                UpdateControlSize();
            UpdateGraphicsBuffer();
        }

        private void UpdateControlSize()
        {
            int txtWidth = TextRenderer.MeasureText(Text, Font).Width + 15;
            int txtHeight = TextRenderer.MeasureText(Text, Font).Height + 5;
            Size = new Size(txtWidth, txtHeight);
        }

        [Browsable(false)]
        public override Image BackgroundImage { get => base.BackgroundImage; set => base.BackgroundImage = value; }
        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout { get => base.BackgroundImageLayout; set => base.BackgroundImageLayout = value; }
    }
}
