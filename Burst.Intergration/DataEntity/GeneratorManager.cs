using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using EnvDTE;

using Burst;
using Burst.Data.Entity.CodeGenerate;
using BurstStudio.Burst_Intergration.DataEntity.AdapterProperties;

namespace BurstStudio.Burst_Intergration.DataEntity
{
    public static class GeneratorManager
    {
        public static void Generate(
            IGenerator generator,
            DTE DTE, GeneralProperty[] GeneralProperties,
            bool OverwriteAll,
            bool ToPascalName,
            bool GenerateInitializer,
            NameValueList InitializeParameters,
            string Namespace
        )
        {
            Project _project = Utils.GetProject(DTE);
            ProjectItem pi = Utils.GetSelectedDirectory(DTE);

            ProjectItems _currentDirectory = null;
            string _path = null;
            string _language = null;
            string _fileExtension = null;

            if (pi != null)
            {
                _currentDirectory = pi.ProjectItems;
                _path = pi.Properties.Item("FullPath").Value.ToString();
            }
            else
            {
                _currentDirectory = _project.ProjectItems;
                _path = _project.Properties.Item("FullPath").Value.ToString();
            }

            if (_project.CodeModel.Language == CodeModelLanguageConstants.vsCMLanguageCSharp)
            {
                _language = "CSharp";
                _fileExtension = ".cs";
            }
            else if (_project.CodeModel.Language == CodeModelLanguageConstants.vsCMLanguageVB)
            {
                _language = "VisualBasic";
                _fileExtension = ".vb";
            }

            //Generate
            foreach (GeneralProperty gp in GeneralProperties)
            {
                if (string.IsNullOrWhiteSpace(gp.ClassName))
                {
                    if (ToPascalName)
                        gp.ClassName = gp.TableName.ToPascalName();
                    else
                        gp.ClassName = gp.TableName;
                }
                foreach (GFieldInfo fieldinfo in gp.Fields)
                    if (string.IsNullOrWhiteSpace(fieldinfo.DisplayName))
                    {
                        if (ToPascalName)
                            fieldinfo.DisplayName = fieldinfo.Name.ToPascalName();
                        else
                            fieldinfo.DisplayName = fieldinfo.Name;
                    }
                /*
                foreach (OwnerFieldInfo ownerfieldinfo in gp.OwnerFields)
                    if (string.IsNullOrWhiteSpace(ownerfieldinfo.DisplayName))
                    {
                        if (ToPascalName)
                            ownerfieldinfo.DisplayName = ownerfieldinfo.Name.ToPascalName();
                        else
                            ownerfieldinfo.DisplayName = ownerfieldinfo.Name;
                    }
                */

                ProjectItem _baseItem = null;

                string basefile = Path.Combine(_path, gp.ClassName + _fileExtension);
                if (OverwriteAll)
                    File.WriteAllText(basefile, generator.GenerateBaseFile(gp, Namespace, _language));
                try
                {
                    _baseItem = _currentDirectory.Item(Path.GetFileName(basefile));
                }
                catch
                {
                    if (!OverwriteAll)
                        File.WriteAllText(basefile, generator.GenerateBaseFile(gp, Namespace, _language));
                    _baseItem = _currentDirectory.AddFromFile(basefile);
                }

                string fieldsfile = Path.Combine(_path, gp.ClassName + ".Fields" + _fileExtension);
                File.WriteAllText(fieldsfile, generator.GenerateFieldsFile(gp, Namespace, _language));
                try
                {
                    _baseItem.ProjectItems.Item(Path.GetFileName(fieldsfile));
                }
                catch
                {
                    _baseItem.ProjectItems.AddFromFile(fieldsfile);
                }

                /*
                string ownerfieldsfile = Path.Combine(_path, gp.ClassName + ".OwnerFields" + _fileExtension);
                if (OverwriteAll && File.Exists(ownerfieldsfile) || gp.OwnerFields.Count > 0)
                {
                    File.WriteAllText(ownerfieldsfile, generator.GenerateOwnerFieldsFile(gp, Namespace, _language));
                    try
                    {
                        _baseItem.ProjectItems.Item(Path.GetFileName(ownerfieldsfile));
                    }
                    catch
                    {
                        _baseItem.ProjectItems.AddFromFile(ownerfieldsfile);
                    }
                }
                */
            }

            if (GenerateInitializer)
            {
                var init_file = Path.Combine(_path, "DbInitializer" + _fileExtension);
                File.WriteAllText(
                    init_file,
                    generator.GenerateInitializeFile(InitializeParameters, Namespace, _language)
                );
                try
                {
                    _currentDirectory.Item(Path.GetFileName(init_file));
                }
                catch
                {
                    _currentDirectory.AddFromFile(init_file);
                }
            }
        }
    }
}
