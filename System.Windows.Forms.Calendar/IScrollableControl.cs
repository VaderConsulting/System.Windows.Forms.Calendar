using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Drawing2D;
using System.Drawing;
using System;
namespace System.Windows.Forms.Calendar
{

   public class IScrollableControl : System.Windows.Forms.Control
   {
      public enum ArrowDirection
      {
         Up,
         Down,
         Right,
         Left
      }
      static internal CalendarRenderer _renderer;
      internal int _timeUnitsOffset; //hold the current scroll offset 

      private Rectangle _HorizontalScrollBarBounds;
      private int _HorizontalScrollMaximum;
      private int _HorizontalScrollValue;

      private Rectangle _ButtonLeftBounds;
      private Rectangle _ButtonRightBounds;
      private Rectangle _HorizontalThumbBounds;

      private int _HorizontalLargeChange;
      private int _HorizontalSmallChange;

      private bool _HorizontalScrollBarEnabled;
      private double _HorizontalScrollPercent;
      //the internal horizontal change amount for the thumb

      private bool _ButtonRightPressed;
      private bool _ButtonRightSelected;
      private bool _ButtonRightEnabled;
      private bool _ButtonLeftPressed;
      private bool _ButtonLeftSelected;
      private bool _ButtonLeftEnabled;

      private bool _HorizontalThumbSelected;
      private bool _HorizontalThumbPressed;
      private int _HorizontalThumbOffset;

      private bool _TrackLeftSelected;
      private bool _TrackRightSelected;
      private bool _TrackLeftPressed;
      private bool _TrackRightPressed;

      private Size arrowSize = new Size(5, 3);
      private Size moreSize = new Size(7, 7);

      //Vertical values
      private Rectangle _VerticalScrollBarBounds;
      private int _VerticalScrollMaximum;
      private int _VerticalScrollValue;

      private Rectangle _ButtonUpBounds;
      private Rectangle _ButtonDownBounds;
      private Rectangle _VerticalThumbBounds;

      private int _VerticalLargeChange;
      private int _VerticalSmallChange;

      private bool _VerticalScrollBarEnabled;
      private double _VerticalScrollPercent;
      //the internal Vertical change amount for the thumb

      private bool _ButtonDownPressed;
      private bool _ButtonDownSelected;
      private bool _ButtonDownEnabled;
      private bool _ButtonUpPressed;
      private bool _ButtonUpSelected;
      private bool _ButtonUpEnabled;

      private bool _VerticalThumbSelected;
      private bool _VerticalThumbPressed;
      private int _VerticalThumbOffset;
      private int _VerticalThumbSmallChange;
      private int _VerticalThumbLastPosition;

      private bool _TrackTopSelected;
      private bool _TrackBottomSelected;
      private bool _TrackTopPressed = false;
      private bool _TrackBottomPressed = false;

      private int _ScrollbarSize = 17;
      public event ScrollEventHandler Scroll;
      public delegate void ScrollEventHandler(ScrollOrientation Orientation, int value);

      private Timer progressTimer = new Timer();

      public IScrollableControl()
      {
         this.progressTimer.Tick += new EventHandler(progressTimer_Tick);
      }
      public void InitScrollbars(int FullWidth, int FullHeight)
      {
         //check the vertical dimentions first since we can be certain that the horizontal will exist in most cases
         bool showH = false;
         bool showV = false;
         if (FullHeight > ClientRectangle.Height & ClientRectangle.Height > 0)
         {
            showV = true;
            if (FullWidth > ClientRectangle.Width - _ScrollbarSize & ClientRectangle.Width > 0)
            {
               showH = true;
            }
         }
         else if (FullWidth > ClientRectangle.Width & ClientRectangle.Width > 0)
         {
            showH = true;
            if (FullHeight > ClientRectangle.Height - _ScrollbarSize & ClientRectangle.Height > 0)
            {
               showV = true;
            }
         }

         if (showV)
         {
            Rectangle vRec = new Rectangle(ClientRectangle.Right - _ScrollbarSize, 0, _ScrollbarSize, ClientRectangle.Height - (showH ? _ScrollbarSize : 0));
            _VerticalScrollMaximum = FullHeight;
            SetVerticalBounds(vRec);
            _VerticalScrollBarEnabled = true;
         }
         else
         {
            _VerticalScrollBarEnabled = false;
            _VerticalScrollValue = 0;
            SetVerticalBounds(Rectangle.Empty);
         }

         if (showH)
         {
            Rectangle hRec = new Rectangle(0, ClientRectangle.Bottom - _ScrollbarSize, ClientRectangle.Width - (showV ? _ScrollbarSize : 0), _ScrollbarSize);
            _HorizontalScrollMaximum = FullWidth;
            SetHorizontalBounds(hRec);
            _HorizontalScrollBarEnabled = true;
         }
         else
         {
            _HorizontalScrollBarEnabled = false;
            _HorizontalScrollValue = 0;
            SetHorizontalBounds(Rectangle.Empty);
         }
      }
      internal void ScrollLeft(int value)
      {
         _HorizontalScrollValue -= value;
         if (_HorizontalScrollValue < 0)
         {
            _HorizontalScrollValue = 0;
         }

         UpdateHorizontalThumb();
         RedrawHorizontalScroll();
         if (Scroll != null)
         {
            Scroll(ScrollOrientation.HorizontalScroll, value);
         }
      }
      internal void ScrollRight(int value)
      {
         int overflow = _HorizontalScrollMaximum - _HorizontalScrollBarBounds.Width;
         _HorizontalScrollValue += value;
         if (_HorizontalScrollValue > overflow)
         {
            _HorizontalScrollValue = overflow;
         }

         UpdateHorizontalThumb();
         RedrawHorizontalScroll();
         if (Scroll != null)
         {
            Scroll(ScrollOrientation.HorizontalScroll, value);
         }
      }
      internal void ScrollUp(int value)
      {

         VerticalScrollValue -= value;
         if (VerticalScrollValue < 0)
         {
            VerticalScrollValue = 0;
         }

         UpdateVerticalThumb();
         RedrawVerticalScroll();
         if (Scroll != null)
         {
            Scroll(ScrollOrientation.VerticalScroll, value);
         }
      }
      internal void ScrollDown(int value)
      {
         int overflow = VerticalScrollMaximum - VerticalScrollBarBounds.Height;
         VerticalScrollValue += value;
         if (VerticalScrollValue > overflow)
         {
            VerticalScrollValue = overflow;
         }

         UpdateVerticalThumb();
         RedrawVerticalScroll();
         if (Scroll != null)
         {
            Scroll(ScrollOrientation.VerticalScroll, -value);
         }
      }

      private void SetHorizontalBounds(Rectangle bounds)
      {
         HorizontalScrollBarBounds = bounds;

         if (bounds.Width == 0 | bounds.Height == 0) return;

         int bwidth = bounds.Height;
         int bheight = bounds.Height;

         _ButtonLeftBounds = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Left + bounds.Height, bounds.Bottom);
         _ButtonRightBounds = Rectangle.FromLTRB(bounds.Right - _ButtonLeftBounds.Width, bounds.Top, bounds.Right, bounds.Bottom);

         _HorizontalThumbBounds.Y = _ButtonLeftBounds.Top;
         _HorizontalThumbBounds.Height = _ButtonLeftBounds.Height;

         _ButtonLeftEnabled = HorizontalScrollValue > 0;

         if (!_ButtonLeftEnabled)
         {
            HorizontalScrollValue = 0;
         }
         _ButtonRightEnabled = true;

         double availWidth = _ButtonRightBounds.Left - _ButtonLeftBounds.Right;
         double viewPercent = (float)_HorizontalScrollBarBounds.Width / (float)HorizontalScrollMaximum;
         double thumbWidth = Math.Ceiling(viewPercent * availWidth);

         if (thumbWidth < 30)
         {
            if (availWidth >= 30)
            {
               thumbWidth = 30;
            }
            else
            {
               thumbWidth = availWidth;
            }
         }

         _HorizontalThumbBounds.Width = Convert.ToInt32(thumbWidth);

         UpdateHorizontalThumb();
      }
      private void SetVerticalBounds(Rectangle bounds)
      {
         VerticalScrollBarBounds = bounds;

         if (bounds.Width == 0 | bounds.Height == 0) return;

         int bwidth = bounds.Width;
         int bheight = bounds.Width;

         _ButtonUpBounds = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, bounds.Top + bounds.Width);
         _ButtonDownBounds = Rectangle.FromLTRB(bounds.Left, bounds.Bottom - bounds.Width, bounds.Right, bounds.Bottom);

         _VerticalThumbBounds.X = _ButtonUpBounds.Left;
         _VerticalThumbBounds.Width = _ButtonUpBounds.Width;

         _ButtonUpEnabled = VerticalScrollValue > 0;

         if (!_ButtonUpEnabled)
         {
            VerticalScrollValue = 0;
         }
         _ButtonDownEnabled = true;

         double availHeight = _ButtonDownBounds.Top - _ButtonUpBounds.Bottom;
         double viewPercent = (float)_VerticalScrollBarBounds.Height / (float)VerticalScrollMaximum;
         double thumbHeight = Math.Ceiling(viewPercent * availHeight);

         if (thumbHeight < 10)
         {
            if (availHeight >= 30)
            {
               thumbHeight = 30;
            }
            else
            {
               thumbHeight = availHeight;
            }
         }

         _VerticalThumbBounds.Height = Convert.ToInt32(thumbHeight);

         UpdateVerticalThumb();
      }
      /// <summary>
      /// Updates the position of the scroll thumb depending on the current offset
      /// </summary>
      private void UpdateHorizontalThumb()
      {
         if (HorizontalScrollBarBounds.Width == 0) return;
         int overflow = HorizontalScrollMaximum - HorizontalScrollBarBounds.Width;

         _HorizontalScrollPercent = (float)HorizontalTrackMaximum / (float)overflow;
         int ThumbScrollValue = Convert.ToInt32(Math.Ceiling(HorizontalScrollValue * _HorizontalScrollPercent));

         if (ThumbScrollValue > HorizontalTrackMaximum)
         {
            ThumbScrollValue = HorizontalTrackMaximum;
         }
         else if (ThumbScrollValue < 0)
         {
            ThumbScrollValue = 0;
         }

         _HorizontalThumbBounds.X = _ButtonLeftBounds.Right + ThumbScrollValue;
      }
      /// <summary>
      /// Updates the position of the scroll thumb depending on the current offset
      /// </summary>
      private void UpdateVerticalThumb()
      {
         if (VerticalScrollBarBounds.Height == 0) return;
         int overflow = VerticalScrollMaximum - VerticalScrollBarBounds.Height;

         _VerticalScrollPercent = (float)VerticalTrackMaximum / (float)overflow;
         int ThumbScrollValue = Convert.ToInt32(Math.Ceiling(VerticalScrollValue * _VerticalScrollPercent));
         _VerticalThumbSmallChange = Convert.ToInt32(_VerticalSmallChange * _VerticalScrollPercent);

         if (ThumbScrollValue > VerticalTrackMaximum)
         {
            ThumbScrollValue = VerticalTrackMaximum;
         }
         else if (ThumbScrollValue < 0)
         {
            ThumbScrollValue = 0;
         }

         _VerticalThumbBounds.Y = _ButtonUpBounds.Bottom + ThumbScrollValue;
      }
      /// <summary>
      /// Redraws the scroll part of the list
      /// </summary>
      internal void RedrawHorizontalScroll()
      {
         Invalidate(_HorizontalScrollBarBounds);
      }
      internal void RedrawVerticalScroll()
      {
         Invalidate(_VerticalScrollBarBounds);
      }
      public void DrawHorizontalScrollBar(Graphics g)
      {
         DrawScrollbarFiller(g);
         if ((ScrollBarRenderer.IsSupported))
         {

            ScrollBarRenderer.DrawRightHorizontalTrack(g, _HorizontalScrollBarBounds, ScrollBarState.Normal);

            if ((_HorizontalThumbPressed))
            {
               ScrollBarRenderer.DrawHorizontalThumb(g, _HorizontalThumbBounds, ScrollBarState.Pressed);
               ScrollBarRenderer.DrawHorizontalThumbGrip(g, _HorizontalThumbBounds, ScrollBarState.Pressed);
            }
            else if ((HorizontalThumbSelected))
            {
               ScrollBarRenderer.DrawHorizontalThumb(g, _HorizontalThumbBounds, ScrollBarState.Hot);
               ScrollBarRenderer.DrawHorizontalThumbGrip(g, _HorizontalThumbBounds, ScrollBarState.Hot);
            }
            else
            {
               ScrollBarRenderer.DrawHorizontalThumb(g, _HorizontalThumbBounds, ScrollBarState.Normal);
               ScrollBarRenderer.DrawHorizontalThumbGrip(g, _HorizontalThumbBounds, ScrollBarState.Normal);
            }

            if ((_ButtonLeftPressed))
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonLeftBounds, ScrollBarArrowButtonState.LeftPressed);
            }
            else if ((_ButtonLeftSelected))
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonLeftBounds, ScrollBarArrowButtonState.LeftHot);
            }
            else
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonLeftBounds, ScrollBarArrowButtonState.LeftNormal);
            }

            if ((_ButtonRightPressed))
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonRightBounds, ScrollBarArrowButtonState.RightPressed);
            }
            else if ((_ButtonRightSelected))
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonRightBounds, ScrollBarArrowButtonState.RightHot);
            }
            else
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonRightBounds, ScrollBarArrowButtonState.RightNormal);

            }
         }
         else
         {
            using (SolidBrush b = new SolidBrush(CalendarColorTable.ButtonGlossyNorth))
            {
               g.FillRectangle(b, _HorizontalScrollBarBounds);
            }

            if ((_HorizontalScrollBarEnabled))
            {
               if ((_HorizontalThumbPressed))
               {
                  DrawButtonPressed(g, _HorizontalThumbBounds, CalendarRenderer.Corners.All);
               }
               else if ((_HorizontalThumbSelected))
               {
                  DrawButtonSelected(g, _HorizontalThumbBounds, CalendarRenderer.Corners.All);
               }
               else
               {
                  DrawButton(g, _HorizontalThumbBounds, CalendarRenderer.Corners.All);

               }
            }

            Color dk = CalendarColorTable.Arrow;
            Color lt = CalendarColorTable.ArrowLight;
            Color ds = CalendarColorTable.ArrowDisabled;

            Rectangle arrLeft = CenterOn(_ButtonLeftBounds, new Rectangle(Point.Empty, arrowSize));
            arrLeft.Offset(0, 1);
            Rectangle arrD = CenterOn(_ButtonRightBounds, new Rectangle(Point.Empty, arrowSize));
            arrD.Offset(0, 1);

            DrawArrow(g, arrLeft, (_ButtonLeftEnabled ? lt : Color.Transparent), ArrowDirection.Left);
            arrLeft.Offset(0, -1);
            DrawArrow(g, arrLeft, (_ButtonLeftEnabled ? dk : ds), ArrowDirection.Left);

            DrawArrow(g, arrD, (_ButtonRightEnabled ? lt : Color.Transparent), ArrowDirection.Right);
            arrD.Offset(0, -1);
            DrawArrow(g, arrD, (_ButtonRightEnabled ? dk : ds), ArrowDirection.Right);
         }
      }
      public void DrawVerticalScrollBar(Graphics g)
      {
         DrawScrollbarFiller(g);
         if ((ScrollBarRenderer.IsSupported))
         {

            ScrollBarRenderer.DrawLowerVerticalTrack(g, VerticalScrollBarBounds, ScrollBarState.Normal);

            if ((_VerticalThumbPressed))
            {
               ScrollBarRenderer.DrawVerticalThumb(g, _VerticalThumbBounds, ScrollBarState.Pressed);
               ScrollBarRenderer.DrawVerticalThumbGrip(g, _VerticalThumbBounds, ScrollBarState.Pressed);
            }
            else if ((_VerticalThumbSelected))
            {
               ScrollBarRenderer.DrawVerticalThumb(g, _VerticalThumbBounds, ScrollBarState.Hot);
               ScrollBarRenderer.DrawVerticalThumbGrip(g, _VerticalThumbBounds, ScrollBarState.Hot);
            }
            else
            {
               ScrollBarRenderer.DrawVerticalThumb(g, _VerticalThumbBounds, ScrollBarState.Normal);
               ScrollBarRenderer.DrawVerticalThumbGrip(g, _VerticalThumbBounds, ScrollBarState.Normal);
            }

            if ((_ButtonUpPressed))
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonUpBounds, ScrollBarArrowButtonState.UpPressed);
            }
            else if ((_ButtonUpSelected))
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonUpBounds, ScrollBarArrowButtonState.UpHot);
            }
            else
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonUpBounds, ScrollBarArrowButtonState.UpNormal);
            }

            if ((_ButtonDownPressed))
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonDownBounds, ScrollBarArrowButtonState.DownPressed);
            }
            else if ((_ButtonDownSelected))
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonDownBounds, ScrollBarArrowButtonState.DownHot);
            }
            else
            {
               ScrollBarRenderer.DrawArrowButton(g, _ButtonDownBounds, ScrollBarArrowButtonState.DownNormal);

            }
         }
         else
         {
            using (SolidBrush b = new SolidBrush(CalendarColorTable.ButtonGlossyNorth))
            {
               g.FillRectangle(b, VerticalScrollBarBounds);
            }

            if ((_VerticalScrollBarEnabled))
            {
               if ((_VerticalThumbPressed))
               {
                  DrawButtonPressed(g, _VerticalThumbBounds, CalendarRenderer.Corners.All);
               }
               else if ((_VerticalThumbSelected))
               {
                  DrawButtonSelected(g, _VerticalThumbBounds, CalendarRenderer.Corners.All);
               }
               else
               {
                  DrawButton(g, _VerticalThumbBounds, CalendarRenderer.Corners.All);

               }
            }

            Color dk = CalendarColorTable.Arrow;
            Color lt = CalendarColorTable.ArrowLight;
            Color ds = CalendarColorTable.ArrowDisabled;

            Rectangle arrTop = CenterOn(_ButtonUpBounds, new Rectangle(Point.Empty, arrowSize));
            arrTop.Offset(1, 0);
            Rectangle arrD = CenterOn(_ButtonDownBounds, new Rectangle(Point.Empty, arrowSize));
            arrD.Offset(1, 0);

            DrawArrow(g, arrTop, (_ButtonUpEnabled ? lt : Color.Transparent), ArrowDirection.Up);
            arrTop.Offset(-1, 0);
            DrawArrow(g, arrTop, (_ButtonUpEnabled ? dk : ds), ArrowDirection.Up);

            DrawArrow(g, arrD, (_ButtonDownEnabled ? lt : Color.Transparent), ArrowDirection.Down);
            arrD.Offset(-1, 0);
            DrawArrow(g, arrD, (_ButtonDownEnabled ? dk : ds), ArrowDirection.Down);
         }
      }
      private void DrawScrollbarFiller(Graphics g)
      {
         if (VerticalScrollBarEnabled & HorizontalScrollBarEnabled)
         {
            //we need the filler cell
            Rectangle fillRec = new Rectangle(ClientRectangle.Right - 18, ClientRectangle.Bottom - 18, 18, 18);
            g.FillRectangle(new SolidBrush(BackColor), fillRec);
         }
      }
      protected override void OnMouseMove(MouseEventArgs e)
      {
         base.OnMouseMove(e);
         this.Focus();
         if (_HorizontalScrollBarEnabled)
         {
            bool leftCache = _ButtonLeftSelected;
            bool rightCache = _ButtonRightSelected;
            bool thumbHCache = _HorizontalThumbSelected;

            _ButtonLeftSelected = _ButtonLeftBounds.Contains(e.Location);
            _ButtonRightSelected = _ButtonRightBounds.Contains(e.Location);
            _HorizontalThumbSelected = _HorizontalThumbBounds.Contains(e.Location);
            _TrackLeftSelected = e.Location.X > _ButtonLeftBounds.Right & e.Location.X < _HorizontalThumbBounds.Left & HorizontalScrollBarBounds.Contains(e.Location);
            _TrackRightSelected = e.Location.X < _ButtonRightBounds.Left & e.Location.X > _HorizontalThumbBounds.Right & HorizontalScrollBarBounds.Contains(e.Location);

            if ((HorizontalScrollBarBounds.Width < HorizontalScrollMaximum))
            {
               _ButtonRightEnabled = true;
            }

            if ((leftCache != _ButtonLeftSelected) | (rightCache != _ButtonRightSelected) | (thumbHCache != _HorizontalThumbSelected))
            {
               RedrawHorizontalScroll();
            }

            if (HorizontalThumbPressed)
            {
               int newval = e.X - _HorizontalThumbOffset - _ButtonLeftBounds.Right;
               if (newval < 0)
               {
                  newval = 0;
               }
               else if (newval > HorizontalTrackMaximum)
               {
                  newval = HorizontalTrackMaximum;
               }

               _HorizontalScrollValue = Convert.ToInt32(Math.Ceiling((float)newval / (float)_HorizontalScrollPercent));
               UpdateHorizontalThumb();
               RedrawHorizontalScroll();
               if (Scroll != null)
               {
                  Scroll(ScrollOrientation.HorizontalScroll, e.Delta);
               }
            }
         }
         if (_VerticalScrollBarEnabled)
         {
            bool TopCache = _ButtonUpSelected;
            bool BottomCache = _ButtonDownSelected;
            bool thumbVCache = _VerticalThumbSelected;

            _ButtonUpSelected = _ButtonUpBounds.Contains(e.Location);
            _ButtonDownSelected = _ButtonDownBounds.Contains(e.Location);
            _VerticalThumbSelected = _VerticalThumbBounds.Contains(e.Location);
            _TrackTopSelected = e.Location.Y > _ButtonUpBounds.Bottom & e.Location.Y < _VerticalThumbBounds.Top & VerticalScrollBarBounds.Contains(e.Location);
            _TrackBottomSelected = e.Location.Y < _ButtonDownBounds.Top & e.Location.Y > _VerticalThumbBounds.Bottom & VerticalScrollBarBounds.Contains(e.Location);

            if ((VerticalScrollBarBounds.Height < VerticalScrollMaximum))
            {
               _ButtonDownEnabled = true;
            }

            if ((TopCache != _ButtonUpSelected) | (BottomCache != _ButtonDownSelected) | (thumbVCache != _VerticalThumbSelected))
            {
               RedrawVerticalScroll();
            }

            if (_VerticalThumbPressed)
            {
               int newval = e.Y - _VerticalThumbOffset - _ButtonUpBounds.Bottom;
               if (newval < 0)
               {
                  newval = 0;
               }
               else if (newval > VerticalTrackMaximum)
               {
                  newval = VerticalTrackMaximum;
               }
               if (_VerticalScrollPercent > 0)
               {
                  _VerticalScrollValue = Convert.ToInt32(Math.Ceiling((float)newval / (float)_VerticalScrollPercent));
               }

               UpdateVerticalThumb();
               RedrawVerticalScroll();
               int ThumbMove = newval - _VerticalThumbLastPosition;
               if (Math.Abs(ThumbMove) >= _VerticalThumbSmallChange)
               {
                  if (Scroll != null)
                  {
                     _VerticalThumbDifference = newval;
                     Scroll(ScrollOrientation.VerticalScroll, _VerticalSmallChange);
                  }
               }
            }
         }
      }
      protected override void OnMouseLeave(EventArgs e)
      {
         base.OnMouseLeave(e);
         this.Focus();
         _ButtonLeftSelected = false;
         _ButtonRightSelected = false;
         _HorizontalThumbSelected = false;
         _TrackRightSelected = false;
         _TrackLeftSelected = false;

         RedrawHorizontalScroll();

         _ButtonUpSelected = false;
         _ButtonDownSelected = false;
         _VerticalThumbSelected = false;
         _TrackBottomSelected = false;
         _TrackTopSelected = false;

         RedrawVerticalScroll();
      }
      protected override void OnMouseDown(MouseEventArgs e)
      {
         base.OnMouseDown(e);
         this.Focus();
         if (_HorizontalScrollBarEnabled & _HorizontalScrollBarBounds.Contains(e.Location))
         {
            if (_ButtonRightSelected)
            {
               _ButtonRightPressed = true;
               progressTimer.Start();
            }

            if (_ButtonLeftSelected)
            {
               _ButtonLeftPressed = true;
               progressTimer.Start();
            }

            if (_HorizontalThumbSelected)
            {
               _HorizontalThumbPressed = true;
               _HorizontalThumbOffset = e.X - _HorizontalThumbBounds.X;
            }

            if (_TrackLeftSelected)
            {
               ScrollLeft(_HorizontalLargeChange);
            }
            else if (_TrackRightSelected)
            {
               ScrollRight(_HorizontalLargeChange);
            }
         }
         else if (_ButtonRightSelected | _ButtonLeftSelected | _HorizontalThumbSelected)
         {
            RedrawHorizontalScroll();
         }

         if (_VerticalScrollBarEnabled & _VerticalScrollBarBounds.Contains(e.Location))
         {
            if (_ButtonDownSelected)
            {
               _ButtonDownPressed = true;
               progressTimer.Start();
            }

            if (_ButtonUpSelected)
            {
               _ButtonUpPressed = true;
               progressTimer.Start();
            }

            if (_VerticalThumbSelected)
            {
               _VerticalThumbPressed = true;
               _VerticalThumbOffset = e.Y - _VerticalThumbBounds.Y;
               _VerticalThumbLastPosition = e.Y - _VerticalThumbOffset - _ButtonUpBounds.Bottom;
            }

            if (_TrackTopSelected)
            {
               _TrackTopPressed = true;
               ScrollUp(_VerticalLargeChange);
            }
            else if (_TrackBottomSelected)
            {
               _TrackBottomPressed = true;
               ScrollDown(_VerticalLargeChange);
            }
         }
         else if (_ButtonDownSelected | _ButtonUpSelected | _VerticalThumbSelected)
         {
            RedrawVerticalScroll();
         }
      }
      protected override void OnMouseUp(MouseEventArgs e)
      {
         base.OnMouseUp(e);
         this.Focus();
         progressTimer.Stop();

         _ButtonRightPressed = false;
         _ButtonLeftPressed = false;
         _HorizontalThumbPressed = false;
         _TrackLeftPressed = false;
         _TrackRightPressed = false;

         RedrawHorizontalScroll();

         _ButtonDownPressed = false;
         _ButtonUpPressed = false;
         _VerticalThumbPressed = false;
         _TrackTopPressed = false;
         _TrackBottomPressed = false;

         RedrawVerticalScroll();
      }
      protected override void OnMouseWheel(MouseEventArgs e)
      {
         base.OnMouseWheel(e);
         this.Focus();
         if (_VerticalScrollBarEnabled)
         {
            if (e.Delta > 0)
            {
               ScrollUp(_VerticalSmallChange);
            }
            else
            {
               ScrollDown(_VerticalSmallChange);
            }
         }
      }
      protected override void OnResize(EventArgs e)
      {
         base.OnResize(e);
         //make sure our scroll position doesn't exceed the bounds
         if (_HorizontalScrollValue + _HorizontalScrollBarBounds.Width > _HorizontalScrollMaximum)
         {
            _HorizontalScrollValue = _HorizontalScrollMaximum - _HorizontalScrollBarBounds.Width;
         }
         if (_VerticalScrollValue + _VerticalScrollBarBounds.Height > _VerticalScrollMaximum)
         {
            _VerticalScrollValue = _VerticalScrollMaximum - _VerticalScrollBarBounds.Height;
         }
         InitScrollbars(HorizontalScrollMaximum, VerticalScrollMaximum);
      }
      private void progressTimer_Tick(object sender, EventArgs myEventArgs)
      {
         // Handle the timer tick by updating the thumb position.
         if (!ScrollBarRenderer.IsSupported)
         {
            return;
         }

         // If an arrow is clicked, move the thumb in small increments.
         if (_ButtonRightPressed)
         {
            ScrollRight(_HorizontalSmallChange);
         }
         else if (_ButtonLeftPressed)
         {
            ScrollLeft(_HorizontalSmallChange);
         }
         else if (_TrackRightSelected)
         {
            ScrollRight(_HorizontalLargeChange);
         }
         else if (_TrackLeftSelected)
         {
            ScrollLeft(_HorizontalLargeChange);
         }
         else if (_ButtonDownPressed)
         {
            ScrollDown(_VerticalSmallChange);
         }
         else if (_ButtonUpPressed)
         {
            ScrollUp(_VerticalSmallChange);
         }
         else if (_TrackBottomSelected)
         {
            ScrollDown(_VerticalLargeChange);
         }
         else if (_TrackTopSelected)
         {
            ScrollUp(_VerticalLargeChange);
         }
      }
      #region "Properties"
      public Rectangle HorizontalScrollBarBounds
      {
         get { return _HorizontalScrollBarBounds; }
         set { _HorizontalScrollBarBounds = value; }
      }
      public Rectangle VerticalScrollBarBounds
      {
         get { return _VerticalScrollBarBounds; }
         set { _VerticalScrollBarBounds = value; }
      }
      public int HorizontalScrollMaximum
      {
         get { return _HorizontalScrollMaximum; }
         set { _HorizontalScrollMaximum = value; }
      }
      public int VerticalScrollMaximum
      {
         get { return _VerticalScrollMaximum; }
         set { _VerticalScrollMaximum = value; }
      }
      public int HorizontalScrollValue
      {
         get { return _HorizontalScrollValue; }
         set
         {
            _HorizontalScrollValue = value;
            UpdateHorizontalThumb();
            RedrawHorizontalScroll();
         }
      }
      public int HorizontalLargeChange
      {
         get { return _HorizontalLargeChange; }
         set { _HorizontalLargeChange = value; }
      }
      public int HorizontalSmallChange
      {
         get { return _HorizontalSmallChange; }
         set { _HorizontalSmallChange = value; }
      }
      public int VerticalLargeChange
      {
         get { return _VerticalLargeChange; }
         set { _VerticalLargeChange = value; }
      }
      public int VerticalSmallChange
      {
         get { return _VerticalSmallChange; }
         set { _VerticalSmallChange = value; }
      }
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool HorizontalScrollBarEnabled
      {
         get { return _HorizontalScrollBarEnabled; }
         set { _HorizontalScrollBarEnabled = value; }
      }
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool VerticalScrollBarEnabled
      {
         get { return _VerticalScrollBarEnabled; }
         set { _VerticalScrollBarEnabled = value; }
      }
      /// <summary>
      /// Gets the bounds of the button that scrolls the items left
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public Rectangle ButtonLeftBounds
      {
         get { return _ButtonLeftBounds; }
      }
      /// <summary>
      /// Gets the bounds of the button that scrolls the items right
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public Rectangle ButtonRightBounds
      {
         get { return _ButtonRightBounds; }
      }
      /// <summary>
      /// Gets the bounds of the horizontal scrollbar thumb
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public Rectangle HorizontalThumbBounds
      {
         get { return _HorizontalThumbBounds; }
      }
      /// <summary>
      /// Gets a vaule indicating if the button that scrolls right the content is currently pressed
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool ButtonRightPressed
      {
         get { return _ButtonRightPressed; }
      }
      /// <summary>
      /// Gets a vaule indicating if the button that scrolls right the content is currently selected
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool ButtonRightSelected
      {
         get { return _ButtonRightSelected; }
      }
      /// <summary>
      /// Gets a vaule indicating if the button that scrolls right the content is currently enabled
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool ButtonRightEnabled
      {
         get { return _ButtonRightEnabled; }
      }
      /// <summary>
      /// Gets a vaule indicating if the button that scrolls left the content is currently pressed
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool ButtonLeftPressed
      {
         get { return _ButtonLeftPressed; }
      }
      /// <summary>
      /// Gets a vaule indicating if the button that scrolls left the content is currently selected
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool ButtonLeftSelected
      {
         get { return _ButtonLeftSelected; }
      }
      /// <summary>
      /// Gets a vaule indicating if the button that scrolls left the content is currently enabled
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool ButtonLeftEnabled
      {
         get { return _ButtonLeftEnabled; }
      }
      /// <summary>
      /// Gets if the scrollbar thumb is currently selected
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool HorizontalThumbSelected
      {
         get { return _HorizontalThumbSelected; }
      }
      /// <summary>
      /// Gets if the scrollbar thumb is currently pressed
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool HorizontalThumbPressed
      {
         get { return _HorizontalThumbPressed; }
      }
      /// <summary>
      /// Gets a vaule indicating if the left track is currently selected
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool TrackLeftSelected
      {
         get { return _TrackLeftSelected; }
      }
      /// <summary>
      /// Gets a vaule indicating if the left track is currently pressed
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool TrackLeftPressed
      {
         get { return _TrackLeftPressed; }
      }
      /// <summary>
      /// Gets a vaule indicating if the right track is currently selected
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool TrackRightSelected
      {
         get { return _TrackRightSelected; }
      }
      /// <summary>
      /// Gets a vaule indicating if the right track is currently pressed
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public bool TrackRightPressed
      {
         get { return _TrackRightPressed; }
      }
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public int HorizontalTrackMinimum
      {
         get { return _ButtonLeftBounds.Right; }
      }
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public int HorizontalTrackMaximum
      {
         get { return _ButtonRightBounds.Left - _HorizontalThumbBounds.Width - _ButtonLeftBounds.Right; }
      }
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public int VerticalTrackMinimum
      {
         get { return _ButtonUpBounds.Bottom; }
      }
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public int VerticalTrackMaximum
      {
         get { return _ButtonDownBounds.Top - _VerticalThumbBounds.Height - _ButtonUpBounds.Bottom; }
      }
      public int VerticalScrollValue
      {
         get { return _VerticalScrollValue; }
         set { _VerticalScrollValue = value; }
      }
      public int ScrollbarSize
      {
         get { return _ScrollbarSize; }
         set { _ScrollbarSize = value; }
      }
      #endregion
      #region "Drawing Functions"
      /// <summary>
      /// Draws a regular button in pressed state
      /// </summary>
      /// <param name="g"></param>
      /// <param name="bounds"></param>
      /// <param name="corners"></param>
      public void DrawButtonPressed(Graphics g, Rectangle bounds, CalendarRenderer.Corners corners)
      {
         Rectangle outerR = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);

         Rectangle innerR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Bottom - 2);

         Rectangle glossyR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Top + Convert.ToInt32(bounds.Height * 0.36));

         using (GraphicsPath boundsPath = CalendarRenderer.RoundRectangle(outerR, 3, corners))
         {
            using (SolidBrush brus = new SolidBrush(CalendarColorTable.ButtonPressedBgOut))
            {
               g.FillPath(brus, boundsPath);
            }

            using (GraphicsPath path = new GraphicsPath())
            {
               path.AddEllipse(new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height * 2));
               path.CloseFigure();
               using (PathGradientBrush gradient = new PathGradientBrush(path))
               {
                  gradient.WrapMode = WrapMode.Clamp;
                  gradient.CenterPoint = new PointF(Convert.ToSingle((float)bounds.Left + (float)bounds.Width / (float)2), Convert.ToSingle(bounds.Bottom));
                  gradient.CenterColor = CalendarColorTable.ButtonPressedBgCenter;
                  gradient.SurroundColors = new Color[] { CalendarColorTable.ButtonPressedBgOut };

                  Blend blend = new Blend(3);
                  blend.Factors = new float[] { 0f, 0.8f, 0f };
                  blend.Positions = new float[] { 0f, 0.3f, 1f };

                  Region lastClip = g.Clip;
                  Region newClip = new Region(boundsPath);
                  newClip.Intersect(lastClip);
                  g.SetClip(newClip.GetBounds(g));
                  g.FillPath(gradient, path);
                  g.Clip = lastClip;
               }
            }

            //'Border
            using (Pen p = new Pen(CalendarColorTable.ButtonPressedBorderOut))
            {
               g.DrawPath(p, boundsPath);
            }

            //'Inner border
            using (GraphicsPath path = CalendarRenderer.RoundRectangle(innerR, 3, corners))
            {
               using (Pen p = new Pen(CalendarColorTable.ButtonPressedBorderIn))
               {
                  g.DrawPath(p, path);
               }
            }

            //Glossy effect
            using (GraphicsPath path = CalendarRenderer.RoundRectangle(glossyR, 3, (corners & CalendarRenderer.Corners.NorthWest) | (corners & CalendarRenderer.Corners.NorthEast)))
            {

               using (LinearGradientBrush b = new LinearGradientBrush(glossyR, CalendarColorTable.ButtonPressedGlossyNorth, CalendarColorTable.ButtonPressedGlossySouth, 90))
               {
                  b.WrapMode = WrapMode.TileFlipXY;
                  g.FillPath(b, path);
               }
            }

            DrawPressedShadow(g, outerR);
         }
      }
      /// <summary>
      /// Draws a shadow that indicates that the element is pressed
      /// </summary>
      /// <param name="g"></param>
      /// <param name="r"></param>
      public void DrawPressedShadow(Graphics g, Rectangle r)
      {
         Rectangle shadow = Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Top + 4);

         using (GraphicsPath path = CalendarRenderer.RoundRectangle(shadow, 3, CalendarRenderer.Corners.NorthEast | CalendarRenderer.Corners.NorthWest))
         {
            using (LinearGradientBrush b = new LinearGradientBrush(shadow, Color.FromArgb(50, Color.Black), Color.FromArgb(0, Color.Black), 90))
            {

               b.WrapMode = WrapMode.TileFlipXY;
               g.FillPath(b, path);
            }
         }
      }
      /// <summary>
      /// Draws a regular buttton in selected state
      /// </summary>
      /// <param name="g"></param>
      /// <param name="bounds"></param>
      /// <param name="corners"></param>
      public void DrawButtonSelected(Graphics g, Rectangle bounds, CalendarRenderer.Corners corners)
      {
         Rectangle outerR = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);

         Rectangle innerR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Bottom - 2);

         Rectangle glossyR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Top + Convert.ToInt32(bounds.Height * 0.36));

         using (GraphicsPath boundsPath = CalendarRenderer.RoundRectangle(outerR, 3, corners))
         {
            using (SolidBrush brus = new SolidBrush(CalendarColorTable.ButtonSelectedBgOut))
            {
               g.FillPath(brus, boundsPath);
            }

            using (GraphicsPath path = new GraphicsPath())
            {
               path.AddEllipse(new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height * 2));
               path.CloseFigure();
               using (PathGradientBrush gradient = new PathGradientBrush(path))
               {
                  gradient.WrapMode = WrapMode.Clamp;
                  gradient.CenterPoint = new PointF(Convert.ToSingle((float)bounds.Left + (float)bounds.Width / (float)2), Convert.ToSingle(bounds.Bottom));
                  gradient.CenterColor = CalendarColorTable.ButtonSelectedBgCenter;
                  gradient.SurroundColors = new Color[] { CalendarColorTable.ButtonSelectedBgOut };

                  Blend Blend = new Blend(3);
                  Blend.Factors = new float[] { 0f, 0.8f, 0f };
                  Blend.Positions = new float[] { 0f, 0.3f, 1f };


                  Region lastClip = g.Clip;
                  Region newClip = new Region(boundsPath);
                  newClip.Intersect(lastClip);
                  g.SetClip(newClip.GetBounds(g));
                  g.FillPath(gradient, path);
                  g.Clip = lastClip;
               }
            }

            //Border
            using (Pen p = new Pen(CalendarColorTable.ButtonSelectedBorderOut))
            {
               g.DrawPath(p, boundsPath);
            }

            //Inner border
            using (GraphicsPath path = CalendarRenderer.RoundRectangle(innerR, 3, corners))
            {
               using (Pen p = new Pen(CalendarColorTable.ButtonSelectedBorderIn))
               {
                  g.DrawPath(p, path);
               }
            }

            //Glossy effect
            using (GraphicsPath path = CalendarRenderer.RoundRectangle(glossyR, 3, (corners & CalendarRenderer.Corners.NorthWest) | (corners & CalendarRenderer.Corners.NorthEast)))
            {
               using (LinearGradientBrush b = new LinearGradientBrush(glossyR, CalendarColorTable.ButtonSelectedGlossyNorth, CalendarColorTable.ButtonSelectedGlossySouth, 90))
               {
                  b.WrapMode = WrapMode.TileFlipXY;
                  g.FillPath(b, path);
               }
            }
         }
      }
      /// <summary>
      /// Centers the specified rectangle on the specified container
      /// </summary>
      /// <param name="container"></param>
      /// <param name="r"></param>
      /// <returns></returns>
      public Rectangle CenterOn(Rectangle container, Rectangle r)
      {
         Rectangle result = new Rectangle(Convert.ToInt32(container.Left + ((float)(container.Width - r.Width) / (float)2)), Convert.ToInt32(container.Top + ((float)(container.Height - r.Height) / (float)2)), r.Width, r.Height);

         return result;
      }
      /// <summary>
      /// Draws an arrow on the specified bounds
      /// </summary>
      /// <param name="g"></param>
      /// <param name="b"></param>
      /// <param name="c"></param>
      /// <param name="d"></param>
      public void DrawArrow(Graphics g, Rectangle b, Color c, ArrowDirection d)
      {
         GraphicsPath path = new GraphicsPath();
         Rectangle bounds = b;

         if (b.Width % 2 != 0 & (d == ArrowDirection.Left))
         {
            bounds = new Rectangle(new Point(b.Left - 1, b.Top - 1), new Size(b.Width + 1, b.Height + 1));
         }

         if (d == ArrowDirection.Left)
         {
            path.AddLine(bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
            path.AddLine(bounds.Right, bounds.Bottom, (int)bounds.Left + ((float)bounds.Width / (float)2), bounds.Top);
         }
         else if (d == ArrowDirection.Right)
         {
            path.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            path.AddLine(bounds.Right, bounds.Top, (int)bounds.Left + ((float)bounds.Width / (float)2), bounds.Bottom);
         }
         else if (d == ArrowDirection.Left)
         {
            path.AddLine(bounds.Left, bounds.Top, bounds.Right, (int)bounds.Top + ((float)bounds.Height / (float)2));
            path.AddLine(bounds.Right, (int)bounds.Top + ((float)bounds.Height / (float)2), bounds.Left, bounds.Bottom);
         }
         else
         {
            path.AddLine(bounds.Right, bounds.Top, bounds.Left, (int)bounds.Top + bounds.Height / 2);
            path.AddLine(bounds.Left, (int)bounds.Top + ((float)bounds.Height / (float)2), bounds.Right, bounds.Bottom);
         }

         path.CloseFigure();

         using (SolidBrush bb = new SolidBrush(c))
         {
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;
            g.FillPath(bb, path);
            g.SmoothingMode = sm;
         }

         path.Dispose();
      }
      /// <summary>
      /// Draws a regular button in normal state 
      /// </summary>
      /// <param name="g"></param>
      /// <param name="bounds"></param>
      public void DrawButton(Graphics g, Rectangle bounds, CalendarRenderer.Corners corners)
      {
         if (bounds.Height <= 0 || bounds.Width <= 0)
         {
            return;
         }

         Rectangle outerR = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
         Rectangle innerR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Bottom - 2);
         Rectangle glossyR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Top + Convert.ToInt32((double)bounds.Height * 0.36));

         using (GraphicsPath boundsPath = CalendarProfessionalRenderer.RoundRectangle(outerR, 3, corners))
         {
            using (SolidBrush brus = new SolidBrush(CalendarColorTable.ButtonBgOut))
            {
               g.FillPath(brus, boundsPath);
            }

            using (GraphicsPath path = new GraphicsPath())
            {
               path.AddEllipse(new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height * 2));
               path.CloseFigure();
               using (PathGradientBrush gradient = new PathGradientBrush(path))
               {
                  gradient.WrapMode = WrapMode.Clamp;
                  gradient.CenterPoint = new PointF(Convert.ToSingle(bounds.Left + ((float)bounds.Width / (float)2)), Convert.ToSingle(bounds.Bottom));
                  gradient.CenterColor = CalendarColorTable.ButtonBgCenter;
                  gradient.SurroundColors = new Color[] { CalendarColorTable.ButtonBgOut };

                  Blend blend = new Blend(3);
                  blend.Factors = new float[] { 0f, 0.8f, 0f };
                  blend.Positions = new float[] { 0f, 0.3f, 1f };


                  Region lastClip = g.Clip;
                  Region newClip = new Region(boundsPath);
                  newClip.Intersect(lastClip);
                  g.SetClip(newClip.GetBounds(g));
                  g.FillPath(gradient, path);
                  g.Clip = lastClip;
               }
            }

            //Border
            using (Pen p = new Pen(CalendarColorTable.ButtonBorderOut))
            {
               g.DrawPath(p, boundsPath);
            }

            //Inner border
            using (GraphicsPath path = CalendarRenderer.RoundRectangle(innerR, 3, corners))
            {
               using (Pen p = new Pen(CalendarColorTable.ButtonBorderIn))
               {
                  g.DrawPath(p, path);
               }
            }

            //Glossy effect
            using (GraphicsPath path = CalendarRenderer.RoundRectangle(glossyR, 3, (corners & CalendarRenderer.Corners.NorthWest) | (corners & CalendarRenderer.Corners.NorthEast)))
            {
               if (glossyR.Width > 0 && glossyR.Height > 0)
               {
                  using (LinearGradientBrush b = new LinearGradientBrush(glossyR, CalendarColorTable.ButtonGlossyNorth, CalendarColorTable.ButtonGlossySouth, 90))
                  {
                     b.WrapMode = WrapMode.TileFlipXY;
                     g.FillPath(b, path);
                  }
               }
            }
         }
      }
      #endregion
   }
}
