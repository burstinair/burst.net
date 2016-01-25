using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using Burst;
using BurstStudio.Burst_Intergration.DataEntity.AdapterProperties;

namespace BurstStudio.Burst_Intergration.DataEntity
{
    public interface IGenerator
    {
        string GenerateInitializeFile(NameValueList Parameters, string Namespace, string Language);
        string GenerateBaseFile(GeneralProperty GeneralProperty, string Namespace, string Language);
        string GenerateFieldsFile(GeneralProperty GeneralProperty, string Namespace, string Language);
        //string GenerateOwnerFieldsFile(GeneralProperty GeneralProperty, string Namespace, string Language);
    }
}
