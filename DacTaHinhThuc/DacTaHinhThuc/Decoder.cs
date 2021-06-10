using System;
using System.Collections.Generic;
using System.Linq;
namespace DacTaHinhThuc
{
    public class Decoder
    {
        public string[] subtract_post_conditions(string input_text)
        {
            string[] lines = input_text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            string target_str = null;
            bool post_reached = false;
            foreach (string line in lines)
            {
                if (line.Length >= 4 && line.Substring(0, 4) == "post")
                {
                    post_reached = true;
                }
                if (post_reached && line.Length > 0) target_str += line;
            }
            //ERROR CODE 1
            if (target_str == null) return null;
            target_str = target_str.Remove(0, 4);
            //random bug
            target_str = target_str.Replace("\r", "");
            //trim to make sure there're no spaces
            target_str = target_str.Replace(" ", "");

            string[] result = null;
            result = target_str.Split(new string[] { "||" }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < result.Length; i++)
            {
                //ERROR CODE 2
                if (result[i].Length < 5) return null;
                result[i] = result[i].Trim(' ');
                result[i] = result[i].Substring(1, result[i].Length - 2);
            }
            return result;
        }

        public string[] subtract_variables(string input_text, bool get_root_name)
        {
            string[] lines = input_text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            char[] splitter = { '(', ')', ':', 'R', 'Z', 'B', '\r', ',', '\n', ' ', '$', 'N' };
            string target_string = null;
            bool pre_reached = false;
            string root_name = null;
            string variables = null;
            foreach (string line in lines)
            {
                if (line.Length >= 3 && line.Substring(0, 3) == "pre")
                {
                    pre_reached = true;
                }
                if (!pre_reached && line.Length > 0)
                {
                    target_string += line;
                }
                if (pre_reached) break;
            }
            //ERROR CODE 3
            if (target_string == null) return null;
            target_string = target_string.Replace("char*", "$");
            root_name = target_string.Split('(')[0];
            if (!get_root_name) target_string = target_string.Replace(root_name, "");
            string[] temp = target_string.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);
            variables = String.Join("/", temp);

            return variables.Split('/');

        }

        public string subtract_pre_conditions(string input_text)
        {
            string[] lines = input_text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            string target_string = null;
            bool pre_reached = false;
            foreach (string line in lines)
            {
                if (line.Length >= 4 && line.Substring(0, 4) == "post") break;
                if (line.Length > 3 && line.Substring(0, 3) == "pre") pre_reached = true;
                if (pre_reached && line.Length > 0) target_string += line;
            }
            if (target_string == null) return null;
            target_string = target_string.Remove(0, 3);
            //random bug
            target_string = target_string.Replace("\r", "");
            //make sure there're no spaces
            target_string = target_string.Replace(" ", "");
            return target_string;
        }

        //split all conditions from 1 post_condition
        public string[] split_by_and(string post_condition)
        {
            string[] result = post_condition.Split(new string[] { "&&" }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = result[i].Trim(new char[] { '(', ')', ' ' });
            }
            if (result != null)
                return result;
            else return null;
        }

        public List<string> decode_post(string[] post_conditions, string[] variables)
        {
            List<string> result = new List<string>();
            if (post_conditions == null) return null;//ERROR
            if (variables == null) return null;//ERROR
            foreach (string str in post_conditions)
            {
                //get last variable - aka output variable
                string output_variable = null;
                output_variable = variables[variables.Length - 1];
                //remove output variable
                string[] temp_variables = null;
                temp_variables = variables.Where((source, index) => index != variables.Length - 1).ToArray();
                //conditions
                string[] splitted_conditions = split_by_and(str);
                string[] temp = splitted_conditions;
                splitted_conditions = splitted_conditions.Where(element => !element.Contains(output_variable + "=")).ToArray();
                result.AddRange(splitted_conditions);
                //get func output
                result.Add(temp.Where(element => element.Contains(output_variable + "=")).ToArray()[0]);

            }
            return result;
        }

        public string[] decode_pre(string pre_conditions)
        {
            string[] result = pre_conditions.Split(new string[] { "&&" }, System.StringSplitOptions.RemoveEmptyEntries);

            char[] splitter = { '(', ')' };
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = String.Join("", result[i].Split(splitter, System.StringSplitOptions.RemoveEmptyEntries));
            }
            return result;
        }

        public string var_type(char ch)
        {
            switch (ch)
            {
                case 'R':
                    return "float";
                    
                case 'Z':
                    return "int";
                   
                case 'N':
                    return "int";
                    
                case 'B':
                    return "bool";
                    
                case '*':
                    return "string";
                   
                default:
                    return "int";
                   
            }
        }
        public string[] decode_variables(string input_text)
        {
            string[] lines = input_text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            string target_string = null;
            char[] splitter = { '(', ')', '\r', ',', '\n' };
            string root_name = null;
            foreach (string line in lines)
            {
                if (line.Length >= 3 && line.Substring(0, 3) == "pre")
                {
                    break;
                }
                if (line.Length > 0)
                {
                    target_string += line;
                }
            }
            //ERROR CODE 3
            if (target_string == null) return null;
            root_name = target_string.Split('(')[0];
            target_string = target_string.Replace(root_name, "");
            target_string = target_string.Replace("\r", "");
            target_string = target_string.Replace(" ", "");

            return target_string.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);

        }

    }
}