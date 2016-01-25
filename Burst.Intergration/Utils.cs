using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;

namespace BurstStudio.Burst_Intergration
{
    public static class Utils
    {
        public static string GetSelectedPath(DTE dte)
        {
            Project _project = Utils.GetProject(dte);
            ProjectItem pi = Utils.GetSelectedDirectory(dte);

            string _path = null;

            if (pi != null)
                _path = pi.Properties.Item("FullPath").Value.ToString();
            else
                _path = _project.Properties.Item("FullPath").Value.ToString();

            return _path;
        }

        public static Project GetProject(DTE dte)
        {
            try
            {
                if (dte.ActiveDocument != null)
                    return dte.ActiveDocument.ProjectItem.ContainingProject;
                else if (dte.ActiveSolutionProjects != null)
                {
                    if ((dte.ActiveSolutionProjects as object[]).Length == 0)
                        return null;
                    return (dte.ActiveSolutionProjects as object[])[0] as Project;
                }
            }
            catch { }
            return null;
        }

        public static ProjectItem GetSelectedDirectory(DTE dte)
        {
            try
            {
                if (dte.SelectedItems == null)
                    return null;
                if (dte.SelectedItems.Count == 0)
                    return null;

                string path = dte.SelectedItems.Item(1).ProjectItem.FileNames[1];
                path = System.IO.Path.GetDirectoryName(path);
                return GetProject(dte).ProjectItems.Item(System.IO.Path.GetFileName(path));
            }
            catch { }
            return null;
        }

        public static string ToPascalName(this string ori)
        {
            if (string.IsNullOrWhiteSpace(ori))
                return ori;
            StringBuilder res = new StringBuilder();
            foreach (var s in ori.Split('_'))
            {
                res.Append(char.ToUpper(s[0]));
                res.Append(s.Substring(1).ToLower());
            }
            return res.ToString();
        }
    }
}
