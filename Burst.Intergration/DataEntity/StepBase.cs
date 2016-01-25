using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE;

namespace BurstStudio.Burst_Intergration.DataEntity
{
    public class StepBase : UserControl
    {
        internal frmDataEntityWizard Wizard;
        internal DTE DTE;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // StepBase
            // 
            this.Name = "StepBase";
            this.Size = new System.Drawing.Size(587, 397);
            this.ResumeLayout(false);

        }

        public virtual void Enter()
        {
        }

        public virtual void Initialize()
        {
        }
    }
}
