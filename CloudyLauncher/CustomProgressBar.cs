using System;
using System.Drawing;
using System.Windows.Forms;

namespace CloudyLauncher
{
  public partial class CustomProgressBar : UserControl
  {
    //public vars
    //private vars
    private Color _color_background = Color.White;
    private Color _color_foreground = Color.FromArgb(16, 82, 182);
    private float _max = 100.0f;
    private float _val;

    public CustomProgressBar()
    {
      InitializeComponent();
      BackColor = _color_background;
    }

    public float Maximum
    {
      get { return _max; }
      set
      {
        _max = value;

        if (_val > _max)
          _val = _max;

        Invalidate();
      }
    }

    public float Value
    {
      get { return _val; }
      set
      {
        var old_value = _val;

        // Make sure that the value does not stray outside the valid range.
        if (value < 0.0f)
          _val = 0.0f;
        else if (value > _max)
          _val = _max;
        else
          _val = value;

        // Invalidate only the changed area.
        var new_value_rect = ClientRectangle;
        var old_value_rect = ClientRectangle;
        // Use a new value to calculate the rectangle for progress.
        var percent = _val / _max;
        new_value_rect.Width = (int)(new_value_rect.Width * percent);
        // Use an old value to calculate the rectangle for progress.
        percent = old_value / _max;
        old_value_rect.Width = (int)(old_value_rect.Width * percent);
        var update_rect = new Rectangle();

        // Find only the part of the screen that must be updated.
        if (new_value_rect.Width > old_value_rect.Width)
        {
          update_rect.X = old_value_rect.Size.Width;
          update_rect.Width = new_value_rect.Width - old_value_rect.Width;
        }
        else
        {
          update_rect.X = new_value_rect.Size.Width;
          update_rect.Width = old_value_rect.Width - new_value_rect.Width;
        }

        update_rect.Height = Height;
        // Invalidate the intersection region only.
        Invalidate(update_rect);
      }
    }

    public Color ForegroundColor
    {
      get { return _color_foreground; }
      set
      {
        _color_foreground = value;
        Invalidate();
      }
    }

    public Color BackgroundColor
    {
      get { return _color_background; }
      set
      {
        _color_background = value;
        BackColor = _color_background;
        Invalidate();
      }
    }

    protected override void OnResize(EventArgs e)
    {
      // forces a redraw
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      var g = e.Graphics;
      var brush = new SolidBrush(_color_foreground);
      var percent = _val / _max;
      var rect = ClientRectangle;
      rect.Width = (int)(rect.Width * percent);
      g.FillRectangle(brush, rect);
      brush.Dispose();
      g.Dispose();
    }
  }
}