using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;


namespace DacTaHinhThuc
{
    public class PythonTranslator
    {
        #region variables
        Decoder decoder = new Decoder();
        enum state { if_conditions, set_result, aux };
        string sp = '"'.ToString();
        string py_indent = "    ";
        string[] if_statements = { "if", ":", "else", "elif" };
        string and_statements = "and";
        string return_true_statement = "return 1";
        string return_false_statement = "return 0";
        string var_Z = "int";
        string var_R = "float";
        string var_char = null;
        string var_bool = null;
        string[] input_func = { "input(", ")" };
        string[] output_func = { "print(", ")" };
        string[] inout_call = { "nhap", "ket qua" };
        string func_def = "def";
        string[] funcs = { "Nhap", "Xuat", "KiemTra", "" };
        #endregion

        #region private funcs
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

        string post_to_py(List<string> decoded_post, string[] variables, string indent)
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
                if (decoded_post[i].Contains("FALSE")) decoded_post[i] = decoded_post[i].Replace("FALSE", "False");
                if (decoded_post[i].Contains("TRUE")) decoded_post[i] = decoded_post[i].Replace("TRUE", "True");
                if (decoded_post[i].Contains("&&")) decoded_post[i] = decoded_post[i].Replace("&&", " and ");
                if (decoded_post[i].Contains("||")) decoded_post[i] = decoded_post[i].Replace("||", " or ");

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
                            result += Environment.NewLine + indent + if_statements[3] + " " + decoded_post[i];
                        else result += if_statements[0] + " " + decoded_post[i];
                    }
                    else result += " " + and_statements + " " + decoded_post[i];
                }
                if (current == state.set_result)
                {
                    if (last_state == state.if_conditions) result += if_statements[1];
                    if (last_state != state.aux) result += Environment.NewLine + indent + indent + decoded_post[i];
                    else result += decoded_post[i];
                }
                last_state = current;
            }
            result.Substring(1, result.Length - 1);
            result += Environment.NewLine;
            result += indent + "return " + output_variable + Environment.NewLine;
            return result;
        }

        string pre_to_py(string[] decoded_pre, string indent)
        {
            string result = null;

            if (decoded_pre.Length < 1)
            {
                result+= indent + return_true_statement + Environment.NewLine;
                return result;

            }

            result += indent;
            for (int i = 0; i < decoded_pre.Length; i++)
            {
                if (i == 0) result += if_statements[0];
                else result += " " + and_statements;
                result += " " + decoded_pre[i];
            }

            result += ":" + Environment.NewLine + indent + indent + return_true_statement + Environment.NewLine;
            result += indent + return_false_statement + Environment.NewLine;
            return result;
        }

        string variables_to_py(string[] variable, bool for_ouput, string indent)
        {
            string result = null;
            if (!for_ouput)
            {
                for (int i = 0; i < variable.Length - 1; i++)
                {
                    result += indent;
                    result += variable[i].Split(':')[0];
                    result += "=" + get_var_type(variable[i][variable[i].Length - 1]);
                    result += "(" + input_func[0] + sp + inout_call[0] + " " + variable[i].Split(':')[0] + " " + sp + input_func[1] + ")";
                    result += Environment.NewLine;
                }
            }
            else
            {
                for (int i = variable.Length - 1; i < variable.Length; i++)
                {
                    result += indent;
                    result += output_func[0] + sp + inout_call[1] + " " + variable[i].Split(':')[0] + ": " + sp + ","
                        + variable[i].Split(':')[0] + output_func[1];
                    result += Environment.NewLine;
                }
            }
            return result;
        }
        #endregion

        public string translate(string input_text)
        {
            string result = null;
            string rootname = decoder.subtract_variables(input_text, true)[0];
            string output_var = decoder.subtract_variables(input_text, false)[decoder.subtract_variables(input_text, false).Length - 1];

            //output fnc
            result += func_def + " " + funcs[1] + "_" + rootname
                + "(" + output_var + ")" + if_statements[1] + Environment.NewLine;
            result += variables_to_py(decoder.decode_variables(input_text), true, py_indent);
            result += Environment.NewLine;

            //Kiemtra func
            result += func_def + " " + funcs[2] + "_" + rootname + "()" + if_statements[1] + Environment.NewLine;
            string[] decoded_pre = decoder.decode_pre(decoder.subtract_pre_conditions(input_text));
            result += pre_to_py(decoded_pre, py_indent);
            result += Environment.NewLine;

            //rootname func
            string[] variables = decoder.subtract_variables(input_text, false);
            result += func_def + " " + rootname + "(";
            for (int i = 0; i < variables.Length - 1; i++)
            {
                result += variables[i] + ",";
            }
            result = result.TrimEnd(',');
            result += ")" + if_statements[1] + Environment.NewLine;
            string[] post_conditions = decoder.subtract_post_conditions(input_text);
            List<string> decoded_post = decoder.decode_post(post_conditions, variables);
            result += post_to_py(decoded_post, variables, py_indent);
            result += Environment.NewLine;

            //input fnc
            //result += func_def + " " + funcs[0] + "_" + rootname + if_statements[1] + Environment.NewLine;
            result += variables_to_py(decoder.decode_variables(input_text), false, "");
            result += Environment.NewLine;

            //project output
            result += if_statements[0] + " " + funcs[2] + "_" + rootname + "() == 1" + if_statements[1] + Environment.NewLine;
            result += py_indent + output_var + "=" + rootname + "(";
            for (int i = 0; i < variables.Length - 1; i++)
            {
                result += variables[i] + ",";
            }
            result = result.TrimEnd(',');
            result += ")" + Environment.NewLine;
            result += py_indent + funcs[1] + "_" + rootname + "(" + output_var + ")" + Environment.NewLine;
            result += if_statements[2] + if_statements[1] + Environment.NewLine;
            result += py_indent + output_func[0] + sp + "Khong thoa dieu kien pre" + sp + output_func[1];
            return result;

        }

        public bool Run(string appName, string source)
        {
            string source_code = source + "\n"  + "wait = input(\"Press any key to continue...\")";
            string[] char_split = { "."};
            string[] str_s = appName.Split(char_split, System.StringSplitOptions.RemoveEmptyEntries);
            appName = str_s[0] + ".py";
            if (System.IO.File.Exists(appName))
            {
                System.IO.File.Delete(appName);
            }


            System.IO.File.WriteAllText(appName, source_code);



            string script = source_code; // TODO - get Iron Python script
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            ScriptSource scriptSource = engine.CreateScriptSourceFromString(source_code);
            CompiledCode compiled = scriptSource.Compile();

            Process.Start(appName);


            return true;

        }
    }


}
