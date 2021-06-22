using System;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace DacTaHinhThuc
{
    public class InputHandler
    {
        string indentation = "    ";
        string long_indentation = "                ";
        char quotation = '"';
        // max2soduong
        public string fnc_name(string input)
        {
            //remove spaces
            input = input.Replace(" ", "");
            //Split the inputs
            string[] lines = input.Split(new string[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
            //get the first element, func's name
            string[] target = lines[0].Split(new string[] { "(" }, System.StringSplitOptions.None);
            string result = target[0];
            return result;
        }

        // [a,b,c]
        public string[] get_variables(string input, bool get_type)
        {
            //remove spaces
            input = input.Replace(" ", "");
            string[] lines = input.Split(new string[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
            //get the variables
            string target = lines[0].Split(new string[] { "(" }, System.StringSplitOptions.RemoveEmptyEntries)[1];
            //2 splitters
            char[] splitter = { '(', ')', ':', 'R', 'Z', 'B', ',', '$', 'N' };
            char[] type_splitter = { '(', ')', ',' };
            target = target.Replace("char*", "$");
            string[] result = null;
            if (get_type)
                result = target.Split(type_splitter, System.StringSplitOptions.RemoveEmptyEntries);
            else
                result = target.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);
            return result;
        }

        // float a, float b
        public string inputvar_wtype(string input, string pointer_str, string cp_pointer)
        {
            string[] vars = get_variables(input, true);
            string result = null;
            for (int i = 0; i < vars.Length - 1; i++)
            {
                char type = vars[i][vars[i].Length - 1];
                result += pointer_str;
                switch (type)
                {
                    case 'R':
                        result += "float";
                        break;
                    case '$':
                        result += "string";
                        break;
                    case 'B':
                        result += "bool";
                        break;
                    default:
                        result += "int";
                        break;
                }
                result += " " + cp_pointer + vars[i].Split(':')[0] + ",";
            }
            return result.TrimEnd(',');

        }

        //float x
        public string outputvar_wtype(string input, string pointer_str, string cp_pointer)
        {
            string[] vars = get_variables(input, true);
            string result = null;
            for (int i = vars.Length - 1; i < vars.Length; i++)
            {
                char type = vars[i][vars[i].Length - 1];
                result += pointer_str;
                switch (type)
                {
                    case 'R':
                        result += "float";
                        break;
                    case '$':
                        result += "string";
                        break;
                    case 'B':
                        result += "bool";
                        break;
                    default:
                        result += "int";
                        break;
                }
                result += " " + cp_pointer + vars[i].Split(':')[0] + ",";
            }
            return result.TrimEnd(',');
        }

        // a,b
        public string inputvar(string input, string pointer_str, string cp_pointer)
        {
            string[] vars = get_variables(input, true);

            string result = null;
            for (int i = 0; i < vars.Length - 1; i++)
            {
                result += pointer_str + cp_pointer + vars[i].Split(':')[0] + ",";
            }
            return result.TrimEnd(',');
        }

        // x
        public string outputvar(string input, string pointer_str)
        {
            return pointer_str + get_variables(input, false)[get_variables(input, false).Length - 1];
        }
        // ((a>0)&&(b>0)
        public string pre_conditions(string input)
        {
            input = input.Replace(" ", "");
            string[] lines = input.Split(new string[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
            string target = null;
            bool get_line = false;
            foreach (string line in lines)
            {
                if (line.Substring(0, 3) == "pre")
                    get_line = true;
                if (line.Length > 3 && line.Substring(0, 4) == "post")
                    break;
                if (get_line)
                    target += line;
            }
            char[] splitter = { '(', ')', ',', '&' };
            target = target.Replace("pre", "");
            string[] conditions = target.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);
            string result = "(";
            foreach (string str in conditions)
            {
                // (a>0)
                result += "(" + str + ")";
                // (a>0)&&
                result += "&&";
            }

            result = result.TrimEnd('&');
            if (result == "(")
                return null;
            // ((a>0)&&(b>0))
            result += ")";
            return result;

        }

        public string output_constuctor(string input)
        {
            if (outputvar_wtype(input, "", "").Contains("string"))
                return "null";
            else if (outputvar_wtype(input, "", "").Contains("bool"))
                return "false";
            else return "0";
        }
        // (a=b) -> (a==b)
        string fix_equalsign(string setter)
        {
            string str = setter;
            if (setter.Contains("="))
            {
                if (!(setter.Contains("!=") || setter.Contains(">=") || setter.Contains("<=")))
                    str = setter.Replace("=", "==");
            }
            return str;
        }

        // if (a>b)
        //      x=a;
        public string post_conditions(string input, string semicolon)
        {
            input = input.Replace(" ", "");
            string[] lines = input.Split(new string[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
            string target = null;
            bool get_line = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length >= 4 && lines[i].Substring(0, 4) == "post")
                    get_line = true;
                if (get_line)
                {
                    lines[i] = lines[i].Replace("TRUE", "true");
                    lines[i] = lines[i].Replace("FALSE", "false");
                    target += lines[i];
                }
            }
            target = target.Replace("post", "");
            string[] vars = get_variables(input, false);
            string x_factor = vars[vars.Length - 1] + "=";
            string[] conditions = target.Split(new string[] { "||" }, System.StringSplitOptions.RemoveEmptyEntries);
            string result = null;

            foreach (string condition in conditions)
            {
                if (conditions.Length > 1)
                    if (result == null)
                        result += "if ";
                    else
                        result += long_indentation + "else if";

                string[] setters = condition.Split(new string[] { "&&" }, System.StringSplitOptions.RemoveEmptyEntries);
                string temp = null;
                if (setters.Length > 2)
                    result += "(";
                foreach (string str in setters)
                {
                    if (str.Contains(x_factor))
                        temp = str.Trim(new char[] { '(', ')' });
                    else
                    {
                        result += "(" + fix_equalsign(str).Trim(new char[] { '(', ')' }) + ")" + "&&";
                    }
                }
                result += "&";
                result = result.TrimEnd('&');
                if (setters.Length > 2)
                    result += ")";
                result += Environment.NewLine + long_indentation + indentation + temp + semicolon + Environment.NewLine;
            }
            return result;

        }

        public string input_type_constuctor(string input, string semicolon)
        {
            string[] vars = inputvar_wtype(input, "", "").Split(',');
            string result = null;
            for (int i = 0; i < vars.Length; i++)
            {
                if (i != 0) result += long_indentation;
                result += vars[i];
                string val = "0";
                if (vars[i].Contains("string"))
                    val = "null";
                else if (vars[i].Contains("bool"))
                    val = "false";
                result += " = " + val + semicolon + Environment.NewLine;
            }
            return result;
        }

        public string input_calls(string input)
        {
            string[] vars = inputvar_wtype(input, "", "").Split(',');
            string result = null;
            for (int i = 0; i < vars.Length; i++)
            {
                string var_type = vars[i].Split(' ')[0];
                string var_name = vars[i].Split(' ')[1];
                if (i != 0)
                    result += long_indentation;
                result += "Console.WriteLine(" + quotation + "Nhap " + var_name + ": " + quotation + ");";
                result += Environment.NewLine;
                result += long_indentation;
                result += var_name + "=" + var_type + ".Parse(Console.ReadLine());";
                result += Environment.NewLine;
            }
            return result;
        }
        public string input_calls2(string input)
        {
            string[] vars = inputvar_wtype(input, "", "").Split(',');
            string result = null;
            for (int i = 0; i < vars.Length; i++)
            {
                string var_type = vars[i].Split(' ')[0];
                string var_name = vars[i].Split(' ')[1];
                if (i != 0)
                    result += long_indentation;
                result += "cout<<" + quotation + "Nhap " + var_name + ": " + quotation + ";";
                result += Environment.NewLine;
                result += long_indentation;
                result += "cin>>" + var_name + ";";
                result += Environment.NewLine;
            }
            return result;
        }
       /* public void Run(string appName, string source)
        {
            string[] char_split = { "." };
            string[] str_s = appName.Split(char_split, System.StringSplitOptions.RemoveEmptyEntries);
            appName = str_s[0] + ".exe";

            if (System.IO.File.Exists(appName))
            {
                System.IO.File.Delete(appName);
            }


            System.IO.File.WriteAllText(appName, source);

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeCompiler icc = codeProvider.CreateCompiler();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = appName;

            CompilerResults results = icc.CompileAssemblyFromSource(parameters, source);

            Process.Start(appName);
        }*/
    }

}
