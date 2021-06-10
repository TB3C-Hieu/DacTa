using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace DacTaHinhThuc
{
    public class CSTranslate
    {
        
        Decoder decoder = new Decoder();
        enum state { if_conditions, set_result, aux };
        string sp = '"'.ToString();
        string CP_indent = " ";
        string[] if_statements = { "if (", "else", "else if (" };
        string and_statements = "&&";
        string return_true_statement = "return 1;";
        string return_false_statement = "return 0;";
        string var_Z = "int";
        string var_R = "float";
        string var_char = null;
        string var_bool = null;
        string[] input_func = { ".Parse(Console.ReadLine());" };
        string[] output_func = { "Console.WriteLine( \"Ket qua la: {0} \"", ");\n" };
        //string[] inout_call = { "nhap", "ket qua" };
        string func_void = "void";
        string func_int = "int";
        string[] funcs = { "Nhap", "Xuat", "KiemTra", "" };
        string CPspace = "      ";
     
        string get_var_type(char ch)
        {
            switch (ch)
            {
                case 'R':
                    return var_R;
                case 'Z':
                    return var_Z;
                case '$':
                    return var_char;
                case 'B':
                    return var_bool;
                case 'N':
                    return var_Z;
                default:
                    return null;
            }

        }

        string post_to_CS(List<string> decoded_post, string[] variables, string indent)
        {
            string output_variable = variables[variables.Length - 1];
            string[] equals = { ">=", "<=", "!=" };
            string[] and_or = { "&&", "||" };
            if (decoded_post == null) return null;//error
            state current = state.if_conditions;
            state last_state = state.aux;
            string result = null;
            result += indent;
            for (int i = 0; i < decoded_post.Count; i++)
            {
                if (decoded_post[i].Contains("FALSE")) decoded_post[i] = decoded_post[i].Replace("FALSE", "false");
                if (decoded_post[i].Contains("TRUE")) decoded_post[i] = decoded_post[i].Replace("TRUE", "true");
            
                if (decoded_post[i].Contains(output_variable + "="))
                    current = state.set_result;
                else current = state.if_conditions;
                if (current != state.set_result)
                    if (decoded_post[i].Contains("="))
                    {
                        bool replacable = true;
                        foreach (string str in equals)
                        {
                            if (decoded_post[i].Contains(str)) replacable = false;
                        }
                        if (replacable) decoded_post[i] = decoded_post[i].Replace("=", "==");
                    }
                if (current == state.if_conditions)
                {
                    if (last_state != current)
                    {
                        if (last_state != state.aux)
                            result += Environment.NewLine + CPspace + CPspace + if_statements[2] + "(" + " " + decoded_post[i];
                        else result += CPspace + CPspace + if_statements[0] + "(" + " " + decoded_post[i];
                    }
                    else result += ")" + and_statements + "(" + decoded_post[i];
                }
                if (current == state.set_result)
                {
                    if (last_state != state.aux) result += "))" + Environment.NewLine + CPspace + CPspace + " " + decoded_post[i];
                    else result += decoded_post[i];
                    result += ";";
                }
                last_state = current;
            }
            /*result.Substring(1, result.Length - 1);*/
            result += Environment.NewLine;
            result += CPspace + CPspace + " " + "return " + output_variable + ";"+ Environment.NewLine;
            return result;
        }

        string pre_to_CS(string[] decoded_pre, string indent)
        {
            string result = null;

            if (decoded_pre.Length < 1)
            {
                result += CPspace + CPspace + " " + return_true_statement + Environment.NewLine;
                return result;

            }

            result += CPspace + CPspace;
            for (int i = 0; i < decoded_pre.Length; i++)
            {
                if (i == 0) result += if_statements[0];
                else result += and_statements;
                result += "(" + decoded_pre[i]+")";
            }
            result += ")";
            result += Environment.NewLine + CPspace + CPspace + " " + return_true_statement + Environment.NewLine;
            result += CPspace + CPspace + return_false_statement;
            return result;
        }

        string variables_to_CS(string[] variable, bool for_ouput, string indent)
        {
            string result = null;
            if (!for_ouput)
            {
                for (int i = 0; i < variable.Length - 1; i++)
                {
                    result += CPspace + CPspace + "Console.WriteLine(" + sp + "Nhap ";
                    result += variable[i].Split(':')[0];
                    result += ":" + sp + ");" + Environment.NewLine;
                    result += CPspace + CPspace ;
                    result += variable[i].Split(':')[0];
                    result += "=" + decoder.var_type(variable[i][variable[i].Length-1]);
                    result += input_func[0] ;
                    result += Environment.NewLine;
                }
            }
            else
            {
                for (int i = variable.Length - 1; i < variable.Length; i++)
                {
                    result += CPspace + CPspace ;
                    result += output_func[0] + " "  +  ","
                        + variable[i].Split(':')[0] + output_func[1];
                    
                }
            }
            return result;
        }
      

        public string translateCP(string input_text)
        {
            string result = null;
            string rootname = decoder.subtract_variables(input_text, true)[0];
            string output_var = decoder.subtract_variables(input_text, false)[decoder.subtract_variables(input_text, false).Length - 1];
            string[] variables = decoder.subtract_variables(input_text, false);
            string[] vars = decoder.decode_variables(input_text);
            string va = vars[vars.Length - 1];
            result += "using System;";
            result += Environment.NewLine + "namespace FormalSpecification" + Environment.NewLine;
            result += "{" + Environment.NewLine;
            result += CPspace + "public class Program" + Environment.NewLine + CPspace + "{" + Environment.NewLine;
            //output fnc
            result += CPspace + CPspace + "public " + func_void + " " + funcs[1] + "_" + rootname
                + "(" + decoder.var_type(va[va.Length-1]) + " " + output_var + ")" + Environment.NewLine + CPspace + CPspace + "{" + Environment.NewLine;
            result += CPspace + CPspace + variables_to_CS(decoder.decode_variables(input_text), true, CP_indent);
            result += CPspace + CPspace + "}" + Environment.NewLine;

            //Kiemtra func
            result += CPspace + CPspace + "public " + func_int + " " + funcs[2] + "_" + rootname + "(";
            for (int i = 0; i < variables.Length - 1; i++)
            {
                string var = decoder.decode_variables(input_text)[i];
                result += decoder.var_type(var[var.Length - 1]) + " ";
                result += variables[i] + ",";
            }
            result = result.TrimEnd(',');
            result += ")" + Environment.NewLine;
            result += CPspace + CPspace + "{" + Environment.NewLine;
            string[] decoded_pre = decoder.decode_pre(decoder.subtract_pre_conditions(input_text));
            result += pre_to_CS(decoded_pre, CP_indent);
            result += Environment.NewLine + CPspace + CPspace + "}" + Environment.NewLine;

            //rootname func

            result += CPspace + CPspace + "public " + decoder.var_type(va[va.Length - 1]);
            result += " " + rootname + "(";

            for (int i = 0; i < variables.Length - 1; i++)
            {
                string var = decoder.decode_variables(input_text)[i];
                result += decoder.var_type(var[var.Length - 1]) + " ";
                result += variables[i] + ",";
            }
            result = result.TrimEnd(',');
            result += ")" + Environment.NewLine + CPspace + CPspace +"{" + Environment.NewLine;
            result += CPspace + CPspace + decoder.var_type(va[va.Length - 1]) + " " + output_var + " = ";
            if (decoder.var_type(va[va.Length - 1]) == "string")
            {
                result += "null";
            }
            else if (decoder.var_type(va[va.Length - 1]) == "bool")
            {
                result += "false ";
            }
            else result += "0";
            result += ";" + Environment.NewLine;
            string[] post_conditions = decoder.subtract_post_conditions(input_text);
            List<string> decoded_post = decoder.decode_post(post_conditions, variables);
            result += post_to_CS(decoded_post, variables, CP_indent);
            result += CPspace + CPspace + "}" + Environment.NewLine;

            //input fnc
            //result += func_def + " " + funcs[0] + "_" + rootname + if_statements[1] + Environment.NewLine;
            result += CPspace + CPspace + "public " + func_void + " " + funcs[0] + "_" + rootname + "(";
            for (int i = 0; i < variables.Length - 1; i++)
            {
                string var = decoder.decode_variables(input_text)[i];
                result += "ref " + decoder.var_type(var[var.Length - 1]) + " ";
                result += variables[i] + ",";
            }
            result = result.TrimEnd(',');
            result += ")" + Environment.NewLine + CPspace + CPspace + "{" + Environment.NewLine;
            result += variables_to_CS(decoder.decode_variables(input_text), false, "");
            result += CPspace + CPspace + "}" + Environment.NewLine;

            //project Main
            result += CPspace + CPspace + "public static " + func_void + " " + "Main" + "()" + Environment.NewLine + CPspace + CPspace + "{" + Environment.NewLine;
            for (int i=0; i<vars.Length;i++)
            {
                result += CPspace + CPspace + decoder.var_type(vars[i][vars[i].Length - 1]) + " " + vars[i].Split(':')[0] + "=";
                if (decoder.var_type(vars[i][vars[i].Length - 1]) == "string")
                {
                    result += "null";
                }
                else if (decoder.var_type(vars[i][vars[i].Length - 1]) == "bool")
                {
                    result += "false ";
                }
                else result += "0";
                result += ";" + Environment.NewLine;
            }
            result += CPspace + CPspace + "Program p = new Program();" + Environment.NewLine;
            result += CPspace + CPspace + "p." + funcs[0] + "_" + rootname + "(";
            for (int i = 0; i < variables.Length - 1; i++)
            {
                string var = decoder.decode_variables(input_text)[i];
                result += "ref " + variables[i] + ",";
            }
            result = result.TrimEnd(',');
            result += ");" + Environment.NewLine;
            result += CPspace + CPspace + if_statements[0] + " " + "p." + funcs[2] + "_" + rootname + "(";
            for (int i = 0; i < variables.Length - 1; i++)
            {
                result += variables[i] + ",";
            }
            result = result.TrimEnd(',');
            result += ") == 1)" + Environment.NewLine + CPspace + CPspace + CPspace + "{" + Environment.NewLine;
            result += CPspace + CPspace + CPspace + output_var + "= " + "p." + rootname + "(";
            for (int i = 0; i < variables.Length - 1; i++)
            {
                result += variables[i] + ",";
            }
            result = result.TrimEnd(',');
            result += ");" + Environment.NewLine;
            result += CPspace + CPspace + CPspace + "p." + funcs[1] + "_" + rootname + "(" + output_var + ");" + Environment.NewLine;
            result += CPspace + CPspace + CPspace + "}" + Environment.NewLine + CPspace + CPspace + if_statements[1]  +  Environment.NewLine;
            result += CPspace + CPspace + "Console.WriteLine(" + sp + "khong thoa dieu kien" + sp + output_func[1] + Environment.NewLine;
            result += CPspace + CPspace + "Console.ReadLine();" + Environment.NewLine;
            result += CPspace + CPspace + "}" + Environment.NewLine +  CPspace + "}" + Environment.NewLine + "}" + Environment.NewLine;
            return result;

        }

        public void Run(string appName, string source)
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
        }
    }


}
