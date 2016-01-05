using System;
using System.Drawing;
using System.Windows.Forms;

namespace CloudyLauncher
{
  public partial class CustomImageButton : UserControl
  {
    private Image _image_click;
    private Image _image_default, _image_hover;
    private bool _is_pressed;
    private bool _mouse_is_over;

    public CustomImageButton()
    {
      InitializeComponent();
    }

    public Image ImageDefault
    {
      get { return _image_default; }
      set { _image_default = value; }
    }

    public Image ImageHover
    {
      get { return _image_hover; }
      set { _image_hover = value; }
    }

    public Image ImageClick
    {
      get { return _image_click; }
      set { _image_click = value; }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      _is_pressed = true;
      Invalidate();
      base.OnMouseDown(e);
    }

    //OnEnter and OnLeave handle the mouse over image updates
    protected override void OnMouseEnter(EventArgs e)
    {
      _mouse_is_over = true;
      Invalidate();
      base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      _mouse_is_over = false;
      Invalidate();
      base.OnMouseLeave(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      _is_pressed = false;
      Invalidate();
      base.OnMouseUp(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (_is_pressed && _image_click != null)
      {
        //if the button is pressed and we have an image for it
        e.Graphics.DrawImage(_image_click, 0, 0);
      }
      else if (!_is_pressed && _mouse_is_over && _image_hover != null)
      {
        //if the button is not pressed, the mouse is over it and we have an image for it
        e.Graphics.DrawImage(_image_hover, 0, 0);
      }
      else
      {
        //the button is idle
        e.Graphics.DrawImage(_image_default, 0, 0);
      }

      if (Text.Length > 0)
      {
        SizeF size = e.Graphics.MeasureString(Text, Font);
        e.Graphics.DrawString(Text,
                              Font,
                              new SolidBrush(ForeColor),
                              (ClientSize.Width - size.Width) / 2,
                              (ClientSize.Height - size.Height) / 2);
      }

      base.OnPaint(e);
    }
  }
}