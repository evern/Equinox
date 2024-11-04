using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;

namespace CheckmateDX
{
    public partial class ProgressForm : WaitForm
    {
        public ProgressForm()
        {
            InitializeComponent();
            this.progressPanel1.AutoHeight = true;
            progressBarControl1.Properties.Step = 1;
            progressBarControl1.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Broken;
            progressBarControl1.Properties.PercentView = true;
            progressBarControl1.Properties.Minimum = 0;
        }

        #region Overrides

        public override void SetCaption(string caption)
        {
            base.SetCaption(caption);
            this.progressPanel1.Caption = caption;
        }

        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            this.progressPanel1.Description = description;
        }

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
            WaitFormCommand command = (WaitFormCommand)cmd;
            if (command == WaitFormCommand.SetProgressMax)
            {
                int max = (int)arg;
                progressBarControl1.Properties.Step = 1;
                progressBarControl1.Properties.Maximum = max;
                progressBarControl1.Reset();
            }
            if (command == WaitFormCommand.SetStep)
            {
                int step = (int)arg;
                progressBarControl1.Properties.Step = step;
            }
            else if(command == WaitFormCommand.PerformStep)
            {
                progressBarControl1.PerformStep();
            }
            else if(command == WaitFormCommand.SetLargeFormat)
            {
                this.Width = (int)arg;
                this.CenterToScreen();
            }
        }

        #endregion

        public enum WaitFormCommand
        {
            SetProgressMax,
            SetStep,
            PerformStep,
            SetLargeFormat
        }
    }
}