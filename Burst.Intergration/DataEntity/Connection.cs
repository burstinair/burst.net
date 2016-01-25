using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using EnvDTE;

using Burst.Xml;
using Burst.Data.MSAccess;
using Burst.Data.MSSQL;
using Burst.Data.MySQL;
using Burst.Data.SQLite;
using BurstStudio.Burst_Intergration.DataEntity.AdapterProperties;

namespace BurstStudio.Burst_Intergration.DataEntity
{
    public partial class Connection : StepBase
    {
        private static int selected = -1;
        private static AdapterProperty[] properties = new AdapterProperty[] {
            new MSSQLAdapterProperty(),
            new MSAccessAdapterProperty(),
            new MySQLAdapterProperty(),
            new SQLiteAdapterProperty()
        };

        public Connection()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            string path = Utils.GetSelectedPath(DTE);
            AdapterProperty.SelectedPath = Utils.GetSelectedPath(DTE);

            cbAdapter.Items.AddRange(properties);

            try
            {
                if (selected > -1)
                    cbAdapter.SelectedIndex = selected;
                else
                {
                    Project p = Utils.GetProject(DTE);
                    string configpath = null;
                    try
                    {
                        configpath = p.ProjectItems.Item("App.config").FileNames[1];
                    }
                    catch
                    {
                        configpath = p.ProjectItems.Item("Web.config").FileNames[1];
                    }
                    XElement config = XElement.Load(configpath).Element("appSettings");
                    Type type = Type.GetType(config.XPathQuery("add[key=DbAdapter]").Attribute("value").Value);
                    if (type == typeof(MSSQLAdapter))
                        cbAdapter.SelectedIndex = 0;
                    else if (type == typeof(MSAccessAdapter))
                        cbAdapter.SelectedIndex = 1;
                    else if (type == typeof(MySQLAdapter))
                        cbAdapter.SelectedIndex = 2;
                    else if (type == typeof(SQLiteAdapter))
                        cbAdapter.SelectedIndex = 3;
                    (cbAdapter.SelectedItem as AdapterProperty).ConnectionString = config.XPathQuery("add[key=ConnectionString]").Attribute("value").Value;
                }
            }
            catch
            {
            }
        }

        private void cbAdapter_SelectedIndexChanged(object sender, EventArgs e)
        {
            pgCS.SelectedObject = cbAdapter.SelectedItem;
            selected = cbAdapter.SelectedIndex;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdapterProperty ap = pgCS.SelectedObject as AdapterProperty;
            try
            {
                var provider = Burst.Data.DbProvider.Initialize(ap.Parameters);
                if (provider != null)
                {
                    selected = cbAdapter.SelectedIndex;
                    Wizard.InitializeParameters = ap.Parameters;
                    Wizard.Schema = provider.Schema;
                    Wizard.NextStep();
                }
                else
                    MessageBox.Show("无法使用指定的设置连接到数据库。");
            }
            catch
            {
                MessageBox.Show("无法使用指定的设置连接到数据库。");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Wizard.Cancel();
        }
    }
}
