using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controls
{
    public class MyDateEdit : DateEdit
    {
        public MyDateEdit()
        {
        }
        protected override DevExpress.XtraEditors.Popup.PopupBaseForm CreatePopupForm()
        {
            return new MyVistaPopupDateEditForm(this);
        }
    }

    public class MyVistaPopupDateEditForm : VistaPopupDateEditForm
    {
        public MyVistaPopupDateEditForm(DateEdit ownerEdit)
            : base(ownerEdit)
        {
        }
        protected override CalendarControl CreateCalendar()
        {
            MyVistaDateEditCalendar c = new MyVistaDateEditCalendar();
            return c;
        }
    }

    public class MyVistaDateEditCalendar : CalendarControl
    {
        public MyVistaDateEditCalendar()
            : base()
        {
        }
        public override Size CalcBestSize()
        {
            Size size = base.CalcBestSize();
            return new Size(ViewInfo.RightAreaSize.Width + 40, size.Height - 40);

        }
        protected override DevExpress.XtraEditors.Drawing.BaseControlPainter CreatePainter()
        {
            var p = new MyVistaDateEditCalendarObjectPainter();
            return p;
        }

        protected override BaseStyleControlViewInfo CreateViewInfo()
        {
            var v = new MyVistaCalendarViewInfo(this);
            return v;
        }

        protected override bool IsPopupCalendar { get { return true; } }
        public override DefaultBoolean ShowOkButton
        {
            get { return DefaultBoolean.True; }
            set { }
        }
    }

    public class MyVistaCalendarViewInfo : VistaCalendarViewInfo
    {
        public MyVistaCalendarViewInfo(CalendarControlBase owner)
            : base(owner)
        {

        }
        protected override CalendarAreaViewInfoBase CreateRightAreaInfo()
        {
            return base.CreateRightAreaInfo();
        }

        protected override Size CalcFooterSize()
        {
            Size s = base.CalcFooterSize();
            s.Height += 50;
            return s;
        }


        protected override Rectangle CalcCalendarsClientBounds(Rectangle bounds)
        {
            Rectangle rect = base.CalcCalendarsClientBounds(bounds);
            rect.Y = 2;
            return rect;
        }

        protected override CalendarFooterViewInfoBase CreateFooterInfo()
        {
            MyVistaCalendarFooterViewInfo info = new MyVistaCalendarFooterViewInfo(this);
            return info;
        }
        protected override Rectangle CalcRightAreaBounds()
        {
            Rectangle rect = base.CalcRightAreaBounds();
            int x = (this.OwnerControl.CalcBestSize().Width / 2) - rect.Width / 2;
            rect.X = x;
            rect.Y = 10;
            return rect;
        }

    }
    public class MyVistaCalendarFooterViewInfo : VistaCalendarFooterViewInfo
    {
        public MyVistaCalendarFooterViewInfo(CalendarViewInfoBase viewInfo)
            : base(viewInfo)
        {
        }
        protected override Rectangle CalcOkButtonBounds()
        {
            Rectangle rect = base.CalcOkButtonBounds();
            rect.Y += 30;
            rect.X += 28;
            return rect;
        }

        protected override Rectangle CalcClearButtonBounds()
        {
            Rectangle rect = base.CalcClearButtonBounds();
            rect.Y += 30;
            rect.X = 5;
            return rect;
        }

        protected override Rectangle CalcCancelButtonBounds()
        {
            Rectangle rect = base.CalcCancelButtonBounds();
            rect.Y += 30;
            rect.X -= 40;
            return rect;
        }

        protected override Rectangle CalcButtonPanelBounds()
        {
            Rectangle rect = base.CalcButtonPanelBounds();
            return rect;
        }
    }

    public class MyVistaDateEditCalendarObjectPainter : VistaDateEditCalendarObjectPainter
    {
        public MyVistaDateEditCalendarObjectPainter()
            : base()
        {
        }
        protected override void DrawBackground(CalendarControlInfoArgs info)
        {
            base.DrawBackground(info);
        }
        protected override void DrawHeader(CalendarControlInfoArgs info)
        {
        }
        protected override void DrawAdornments(ControlGraphicsInfoArgs info)
        {
        }

        protected override void DrawCalendars(CalendarControlInfoArgs info)
        {
        }
    }
}
