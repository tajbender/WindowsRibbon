//#define OldCode
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Xml;

namespace UIRibbonTools
{
    public class CSharpCodeBuilder : AbstractCodeBuilder
    {
        protected override void SetStreamWriter(string path)
        {
            sw = File.CreateText(Path.Combine(path, ribbonItemsClass + ".Designer.cs"));
        }

        protected override void WriteHeader()
        {
            sw.WriteLine("//------------------------------------------------------------------------------");
            sw.WriteLine("// <auto-generated>");
            sw.WriteLine("//     This code was generated by a tool.");
            sw.WriteLine("//     Runtime Version:");
            sw.WriteLine("//");
            sw.WriteLine("//     Changes to this file may cause incorrect behavior and will be lost if");
            sw.WriteLine("//     the code is regenerated.");
            sw.WriteLine("// </auto-generated>");
            sw.WriteLine("//------------------------------------------------------------------------------");
            sw.WriteLine();
            sw.WriteLine("using System;");
            sw.WriteLine("using RibbonLib;");
            sw.WriteLine("using RibbonLib.Controls;");
            sw.WriteLine("using RibbonLib.Interop;");
            sw.WriteLine();
            sw.WriteLine("namespace RibbonLib.Controls");
            sw.WriteLine("{");
            sw.WriteLine(Indent(1) + "partial class " + ribbonItemsClass);
            sw.WriteLine(Indent(1) + "{");
        }

        protected override void WriteConst()
        {
            if (!hasHFile)
            {
                sw.WriteLine("// Warning: *.h file does not exist. The commands maybe incomplete !!!");
                sw.WriteLine();
            }
            sw.WriteLine(Indent(2) + "private static class Cmd");
            sw.WriteLine(Indent(2) + "{");
#if OldCode
            for (int i = 0; i < pair3List.Count; i++)
            {
                sw.WriteLine(Ident(3) + "public const uint " + pair3List[i].Key + " = " + pair3List[i].Value + ";");
            }
            for (int i = 0; i < pair1List.Count; i++)
            {
                sw.WriteLine(Ident(3) + "public const uint " + pair1List[i].Key + " = " + pair1List[i].Value + ";");
            }
#else
            for (int i = 0; i < ribbonItems.Count; i++)
            {
                RibbonItem ribbonItem = ribbonItems[i];
                if (ribbonItem.IsContextPopup)
                    popupRibbonItems.Add(ribbonItem);
                sw.WriteLine(Indent(3) + "public const uint " + ribbonItem.CommandName + " = " + ribbonItem.CommandId.ToString(CultureInfo.InvariantCulture) + ";");
            }
#endif
            if (_qatCustomizeCommand != null)
                sw.WriteLine(Indent(3) + "public const uint " + _qatCustomizeCommand.Value.Key + " = " + _qatCustomizeCommand.Value.Value.ToString(CultureInfo.InvariantCulture) + ";");
            sw.WriteLine(Indent(2) + "}");
            sw.WriteLine();
        }

        protected override void WritePopupConst()
        {
            sw.WriteLine(Indent(2) + "// ContextPopup CommandName");
            for (int i = 0; i < popupRibbonItems.Count; i++)
            {
                RibbonItem ribbonItem = popupRibbonItems[i];
                string name = ribbonItem.CommandName;
                if (!Char.IsNumber(name[0]))
                {
                    if (!string.IsNullOrEmpty(ribbonItem.Comment))
                        IntelliSenseComment(ribbonItem.Comment);
                    sw.WriteLine(Indent(2) + "public const uint " + name + " = Cmd." + name + ";");
                }
                else
                    sw.WriteLine(Indent(2) + "// CommandId = " + name);
            }
            sw.WriteLine();
        }

        protected override void WriteProperties()
        {
            sw.WriteLine(Indent(2) + "private static bool initialized;");
            sw.WriteLine();
            sw.WriteLine(Indent(2) + "public " + "Ribbon" + " " + "Ribbon" + " { get; private set; }");
#if OldCode
            for (int i = 0; i < pair2List.Count; i++)
            {
                string name = GetPropertyName(pair2List[i].Key);
                if (!Char.IsNumber(name[0]))
                    sw.WriteLine(Ident(2) + "public " + pair2List[i].Value + " " + name + " { get; private set; }");
                else
                {
                    string identifier = pair2List[i].Value.Substring("Ribbon".Length) + name;
                    sw.WriteLine(Ident(2) + "public " + pair2List[i].Value + " " + identifier + " { get; private set; }");
                }
            }
#else
            for (int i = 0; i < ribbonItems.Count; i++)
            {
                RibbonItem ribbonItem = ribbonItems[i];
                if (!(ribbonItem.IsContextPopup))
                {
                    string name = GetPropertyName(ribbonItem.CommandName);
                    if (!string.IsNullOrEmpty(ribbonItem.Comment))
                        IntelliSenseComment(ribbonItem.Comment);
                    sw.WriteLine(Indent(2) + "public " + ribbonItem.RibbonClassName + " " + name + " { get; private set; }");
                }
            }
#endif
            sw.WriteLine();
        }

        protected override void WriteConstructor()
        {
            sw.WriteLine(Indent(2) + "public " + ribbonItemsClass + "(Ribbon ribbon)");
            sw.WriteLine(Indent(2) + "{");
            sw.WriteLine(Indent(3) + "if (ribbon == null)");
            sw.WriteLine(Indent(4) + "throw new ArgumentNullException(nameof(ribbon), \"Parameter is null\");");
            sw.WriteLine(Indent(3) + "if (initialized)");
            sw.WriteLine(Indent(4) + "return;");
            sw.WriteLine(Indent(3) + "this.Ribbon = ribbon;");
#if OldCode
            for (int i = 0; i < pair2List.Count; i++)
            {
                string name = GetPropertyName(pair2List[i].Key);
                if (!Char.IsNumber(name[0]))
                    sw.WriteLine(Ident(3) + name + " = new " + pair2List[i].Value + "(ribbon, " + "Cmd." + pair2List[i].Key + ");");
                else
                {
                    string identifier = pair2List[i].Value.Substring("Ribbon".Length) + name;
                    sw.WriteLine(Ident(3) + identifier + " = new " + pair2List[i].Value + "(ribbon, " + pair2List[i].Key + ");");
                }
            }
#else
            for (int i = 0; i < ribbonItems.Count; i++)
            {
                RibbonItem ribbonItem = ribbonItems[i];
                if (!(ribbonItem.IsContextPopup))
                {
                    string name = GetPropertyName(ribbonItem.CommandName);
                    if (ribbonItem.RibbonClassName.Equals("RibbonQuickAccessToolbar") && _qatCustomizeCommand != null)
                        sw.WriteLine(Indent(3) + name + " = new " + ribbonItem.RibbonClassName + "(ribbon, " + "Cmd." + ribbonItem.CommandName + ", " + "Cmd." + _qatCustomizeCommand.Value.Key + ");");
                    else
                        sw.WriteLine(Indent(3) + name + " = new " + ribbonItem.RibbonClassName + "(ribbon, " + "Cmd." + ribbonItem.CommandName + ");");
                }
            }
#endif
            sw.WriteLine(Indent(3) + "initialized = true;");
            sw.WriteLine(Indent(2) + "}");
            sw.WriteLine();
        }

        protected override void CloseCodeFile()
        {
            sw.WriteLine(Indent(1) + "}");
            sw.WriteLine("}");
            sw.Close();
        }

        private void IntelliSenseComment(string comment)
        {
            sw.WriteLine(Indent(2) + "/// <summary>");
            sw.WriteLine(Indent(2) + "/// " + comment);
            sw.WriteLine(Indent(2) + "/// </summary>");
        }
    }
}
