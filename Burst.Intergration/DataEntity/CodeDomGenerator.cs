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
    public class CodeDomGenerator : IGenerator
    {
        private System.CodeDom.CodeNamespace _getNamespace(GeneralProperty gp, string Namespace)
        {
            System.CodeDom.CodeNamespace cns = new System.CodeDom.CodeNamespace(Namespace);
            cns.Imports.Add(new CodeNamespaceImport("System"));
            cns.Imports.Add(new CodeNamespaceImport("Burst"));
            cns.Imports.Add(new CodeNamespaceImport("Burst.Data"));
            cns.Imports.Add(new CodeNamespaceImport("Burst.Data.Entity"));
            if (gp != null && gp.IsWebUserModel)
                cns.Imports.Add(new CodeNamespaceImport("Burst.Web"));
            return cns;
        }
        private CodeTypeDeclaration _getType(GeneralProperty gp)
        {
            CodeTypeDeclaration model = new CodeTypeDeclaration(gp.ClassName);
            if (gp.IsWebUserModel)
            {
                model.BaseTypes.Add(
                    new CodeTypeReference("UserBase", new CodeTypeReference[] {
                    new CodeTypeReference(gp.ClassName)
                }));
            }
            else
            {
                model.BaseTypes.Add(
                    new CodeTypeReference("DataEntity", new CodeTypeReference[] {
                    new CodeTypeReference(gp.ClassName)
                }));
            }
            model.TypeAttributes = TypeAttributes.Public;
            model.IsPartial = true;
            return model;
        }
        private string _getString(CodeCompileUnit ccu, string _language)
        {
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = true;
            options.VerbatimOrder = true;

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                CodeDomProvider.CreateProvider(_language).GenerateCodeFromCompileUnit(ccu, sw, options);
                return sb.ToString();
            }
        }

        private List<string> Entities = new List<string>();

        public string GenerateInitializeFile(NameValueList Parameters, string Namespace, string Language)
        {
            CodeCompileUnit ccu = new CodeCompileUnit();
            System.CodeDom.CodeNamespace cns = new System.CodeDom.CodeNamespace(Namespace);
            cns.Imports.Add(new CodeNamespaceImport("System"));
            cns.Imports.Add(new CodeNamespaceImport("Burst"));
            cns.Imports.Add(new CodeNamespaceImport("Burst.Data"));
            CodeTypeDeclaration model = new CodeTypeDeclaration("DbInitializer");
            model.TypeAttributes = TypeAttributes.Public;

            var init = new CodeMemberMethod();
            init.Attributes = MemberAttributes.Static | MemberAttributes.Public;
            init.Name = "InitializeDb";
            init.ReturnType = new CodeTypeReference(typeof(bool));
            init.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "InitializeEntities"));
            init.Statements.Add(new CodeVariableDeclarationStatement(typeof(NameValueList), "__pms", new CodeObjectCreateExpression(typeof(Burst.NameValueList))));
            foreach (var nv in Parameters)
            {
                CodeExpression value;
                var _type = nv.Value.GetType();
                if(_type.IsValueType || _type == typeof(string))
                    value = new CodePrimitiveExpression(nv.Value);
                else
                    value = new CodeObjectCreateExpression(_type);
                init.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeVariableReferenceExpression("__pms"), "Add",
                    new CodePrimitiveExpression(nv.Name),
                    value
                )));
            }
            init.Statements.Add(new CodeVariableDeclarationStatement(
                typeof(Burst.Data.DbProvider),
                "__provider",
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(Burst.Data.DbProvider)),
                    "Initialize",
                    new CodeVariableReferenceExpression("__pms")
                )
            ));
            var init_entities = new List<CodeStatement>();
            foreach (var c in Entities)
                init_entities.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(c), "InitializeDbInfo"
                )));
            init.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression("__provider"),
                    CodeBinaryOperatorType.IdentityInequality,
                    new CodePrimitiveExpression(null)
                ),
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeArgumentReferenceExpression("InitializeEntities"),
                        init_entities.ToArray()
                    ),
                    new CodeMethodReturnStatement(new CodePrimitiveExpression(true))
                },
                new CodeStatement[] {
                    new CodeMethodReturnStatement(new CodePrimitiveExpression(false))
                }
            ));
            model.Members.Add(init);

            cns.Types.Add(model);
            ccu.Namespaces.Add(cns);
            return _getString(ccu, Language);
        }

        public  string GenerateBaseFile(GeneralProperty GeneralProperty, string Namespace, string Language)
        {
            CodeCompileUnit ccu = new CodeCompileUnit();
            System.CodeDom.CodeNamespace cns = _getNamespace(GeneralProperty, Namespace);
            CodeTypeDeclaration model = _getType(GeneralProperty);
            cns.Types.Add(model);
            ccu.Namespaces.Add(cns);
            return _getString(ccu, Language);
        }

        private CodeTypeReference _getEnum(string type, string name, CodeTypeDeclaration model)
        {
            CodeTypeDeclaration member = new CodeTypeDeclaration(name);
            member.TypeAttributes = TypeAttributes.Public;
            member.IsEnum = true;
            foreach (string ei in type.SplitWithQuote("'"))
                member.Members.Add(new CodeMemberField(typeof(int), ei));
            model.Members.Add(member);
            return new CodeTypeReference(string.Format("{0}.{1}", model.Name, name));
        }
        private CodeTypeReference _addField(GFieldInfo fieldinfo, CodeTypeDeclaration model)
        {
            CodeMemberField field = new CodeMemberField();
            field.Attributes = MemberAttributes.Private;
            field.Name = "__" + fieldinfo.Name;

            CodeMemberProperty property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Public;
            property.Name = fieldinfo.DisplayName;

            if (fieldinfo.Type.StartsWith("gen:"))
            {
                if (fieldinfo.Type.Substring(4).StartsWith("enum"))
                    field.Type = property.Type = _getEnum(fieldinfo.Type, "_" + fieldinfo.Name, model);
            }
            else
                field.Type = property.Type = new CodeTypeReference(Type.GetType(fieldinfo.Type));

            property.CustomAttributes.Add(
                new CodeAttributeDeclaration("DataField",
                    new CodeAttributeArgument(
                        new CodePrimitiveExpression(fieldinfo.Name)
                    ),
                    new CodeAttributeArgument(
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression("SerializeType"),
                            fieldinfo.SerializeType.ToString()
                        )
                    )
                )
            );
            if (fieldinfo.IsIndexField)
                property.CustomAttributes.Add(
                    new CodeAttributeDeclaration("IndexField")
                );
            if (fieldinfo.DefaultValue != null)
                property.CustomAttributes.Add(
                    new CodeAttributeDeclaration("DefaultValue",
                        new CodeAttributeArgument(
                            new CodePrimitiveExpression(fieldinfo.DefaultValue)
                        )
                    )
                );

            property.HasGet = true;
            property.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), 
                    field.Name
                )
            ));

            property.HasSet = true;
            property.SetStatements.Add(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                "SetValue",
                new CodePrimitiveExpression(fieldinfo.Name),
                new CodePropertySetValueReferenceExpression()
            ));

            model.Members.Add(field);
            model.Members.Add(property);

            return property.Type;
        }
        public string GenerateFieldsFile(GeneralProperty GeneralProperty, string Namespace, string Language)
        {
            CodeCompileUnit ccu = new CodeCompileUnit();
            System.CodeDom.CodeNamespace cns = _getNamespace(GeneralProperty, Namespace);
            CodeTypeDeclaration model = _getType(GeneralProperty);

            model.CustomAttributes.Add(
                new CodeAttributeDeclaration("DataEntity",
                    new CodeAttributeArgument(new CodePrimitiveExpression(GeneralProperty.TableName)),
                    new CodeAttributeArgument(new CodePrimitiveExpression(GeneralProperty.UseCache))
                )
            );
            CodeMemberMethod setvalue = new CodeMemberMethod();
            setvalue.Attributes = MemberAttributes.Override | MemberAttributes.Family;
            setvalue.Name = "SetValue";
            setvalue.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(string), "Key")
            );
            setvalue.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(object), "Value")
            );
            int _startIndex = 0;
            if (GeneralProperty.IsWebUserModel)
                _startIndex = 4;
            for (int i = _startIndex; i < GeneralProperty.Fields.Count; i++)
            {
                var fieldinfo = GeneralProperty.Fields[i];
                setvalue.Statements.Add(new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodeArgumentReferenceExpression("Key"),
                        CodeBinaryOperatorType.ValueEquality,
                        new CodePrimitiveExpression(fieldinfo.Name)
                    ),
                    new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(),
                            "__" + fieldinfo.Name
                        ),
                        new CodeCastExpression(
                            _addField(fieldinfo, model),
                            new CodeArgumentReferenceExpression("Value")
                        )
                    )
                ));
            }
            setvalue.Statements.Add(
                new CodeExpressionStatement(
                    new CodeMethodInvokeExpression(
                        new CodeBaseReferenceExpression(),
                        "SetValue",
                        new CodeExpression[] {
                            new CodeArgumentReferenceExpression("Key"),
                            new CodeArgumentReferenceExpression("Value")
                        }
                    )
                )
            );
            model.Members.Add(setvalue);

            cns.Types.Add(model);
            ccu.Namespaces.Add(cns);
            Entities.Add(GeneralProperty.ClassName);
            return _getString(ccu, Language);
        }

        /*
        private CodeTypeReference _addOwnerField(OwnerFieldInfo fieldinfo, CodeTypeDeclaration model)
        {
            CodeMemberProperty property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Public;
            property.Name = fieldinfo.Name;

            property.Type = new CodeTypeReference(
                fieldinfo.Type.Substring(4),
                new CodeTypeReference(fieldinfo.OwnedEntityType)
            );

            property.CustomAttributes.Add(
                new CodeAttributeDeclaration("DataOwnerField",
                    new CodeAttributeArgument(
                        new CodeTypeOfExpression(fieldinfo.OwnedEntityType)
                    ),
                    new CodeAttributeArgument(
                        new CodePrimitiveExpression(fieldinfo.AutoLoad)
                    )
                )
            );

            property.HasGet = true;
            property.HasSet = true;

            model.Members.Add(property);

            return property.Type;
        }
        
        public string GenerateOwnerFieldsFile(GeneralProperty GeneralProperty, string Namespace, string Language)
        {
            CodeCompileUnit ccu = new CodeCompileUnit();
            System.CodeDom.CodeNamespace cns = _getNamespace(GeneralProperty, Namespace);
            CodeTypeDeclaration model = _getType(GeneralProperty);

            foreach (OwnerFieldInfo ofi in GeneralProperty.OwnerFields)
                _addOwnerField(ofi, model);

            cns.Types.Add(model);
            ccu.Namespaces.Add(cns);
            return _getString(ccu, Language);
        }
        */
    }
}
