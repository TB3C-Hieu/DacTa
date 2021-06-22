using System;
namespace DacTaHinhThuc
{
    public class FStoCSharp
    {
        public string toCSharp(string path, string text)
        {
            InputHandler iphandler = new InputHandler();
            string[] lines = System.IO.File.ReadAllLines(path);
            string result = null;
            for (int i=0;i<lines.Length;i++)
            {
                if (iphandler.pre_conditions(text) == null && lines[i].Contains("[pre_conditions]"))
                {
                    i+=2;
                    lines[i]=lines[i].Replace("0", "1");
                }  
                lines[i] = lines[i].Replace("[func_name]", iphandler.fnc_name(text));
                lines[i] = lines[i].Replace("[inputvar_wtype]", iphandler.inputvar_wtype(text,"",""));
                lines[i] = lines[i].Replace("[pre_conditions]", iphandler.pre_conditions(text));
                lines[i] = lines[i].Replace("[outputvar_wtype]", iphandler.outputvar_wtype(text,"",""));
                lines[i] = lines[i].Replace("[outputvar]", iphandler.outputvar(text, ""));
                lines[i] = lines[i].Replace("[ref_inputvar_wtype]", iphandler.inputvar_wtype(text, "ref ",""));
                lines[i] = lines[i].Replace("[cp_inputvar_wtype]", iphandler.inputvar_wtype(text,"", "*"));
                lines[i] = lines[i].Replace("[output_constructor]", iphandler.output_constuctor(text));
                lines[i] = lines[i].Replace("[post_conditions]", iphandler.post_conditions(text,";"));
                lines[i] = lines[i].Replace("[input_type_constuctor]", iphandler.input_type_constuctor(text,";"));
                lines[i] = lines[i].Replace("[ref_inputvar]", iphandler.inputvar(text,"ref",""));
                lines[i] = lines[i].Replace("[cp_inputvar]", iphandler.inputvar(text,"","&"));
                lines[i] = lines[i].Replace("[inputvar]", iphandler.inputvar(text,"",""));
                lines[i] = lines[i].Replace("[input_calls]", iphandler.input_calls(text));
                lines[i] = lines[i].Replace("[input_calls2]", iphandler.input_calls2(text));


                result += lines[i];
                result += Environment.NewLine;

            }
            return result;
        }
    }
}
