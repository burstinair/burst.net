using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.CodeDom;
using EnvDTE;

using Burst;
using Burst.Data.Schema;
using Burst.Data.Entity.CodeGenerate;
using BurstStudio.Burst_Intergration.DataEntity.AdapterProperties;

namespace BurstStudio.Burst_Intergration.DataEntity
{
    public partial class frmDataEntityWizard : Form
    {
        private DTE _dte;
        private StepBase[] _steps;
        private int curStep;

        public NameValueList InitializeParameters;
        public DbSchema Schema;
        public GeneralProperty[] Tables;

        public frmDataEntityWizard(DTE dte)
        {
            InitializeComponent();
            _dte = dte;

            List<StepBase> steps = new List<StepBase>();

            StepBase sb = new Connection();
            sb.Wizard = this;
            sb.DTE = _dte;
            steps.Add(sb);
            sb.Initialize();

            sb = new Tables();
            sb.Wizard = this;
            sb.DTE = _dte;
            steps.Add(sb);
            sb.Initialize();

            sb = new Confirm();
            sb.Wizard = this;
            sb.DTE = _dte;
            steps.Add(sb);
            sb.Initialize();

            _steps = steps.ToArray();

            pMain.Controls.Add(steps[0]);
            curStep = 0;
        }

        public void NextStep()
        {
            pMain.Controls.Remove(_steps[curStep]);
            curStep++;
            pMain.Controls.Add(_steps[curStep]);
            _steps[curStep].Enter();
        }
        public void PrevStep()
        {
            pMain.Controls.Remove(_steps[curStep]);
            curStep--;
            pMain.Controls.Add(_steps[curStep]);
            _steps[curStep].Enter();
        }
        public void Cancel()
        {
            Close();
        }

        private void frmDataEntityWizard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
