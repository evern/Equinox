using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;

namespace AutoUpdate
{
    public partial class ProgressForm : WaitForm
    {
        public ProgressForm()
        {
            InitializeComponent();
            this.progressPanel1.AutoHeight = true;
            this.progressBarControl1.Properties.Step = 1;
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
            if(command == WaitFormCommand.PerformStep)
            {
                progressBarControl1.PerformStep();
            }
            else if(command == WaitFormCommand.SetMax)
            {
                int max = (int)arg;
                progressBarControl1.Properties.Maximum = max;
            }
        }

        #endregion

        public enum WaitFormCommand
        {
            SetMax,
            PerformStep
        }
    }
}