using ScintillaNET;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using ScintillaNET.WPF;
using System.Drawing;
using ScintillaNET_FindReplaceDialog;
using System.Windows.Media;

namespace DacTaHinhThuc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Decoder decoder = new Decoder();
        /*FunctionC functionC = new FunctionC();*/
        PythonTranslator py_translator = new PythonTranslator();
        CSTranslate cp_translator = new CSTranslate();
        enum state { python, cpp };
        state current = state.cpp;
        File file_handler = new File();
        //ScintillaNET.Scintilla OutputArea;
        //ScintillaNET.Scintilla InputArea;
        public MainWindow()
        {
            InitializeComponent();
            this.FilePanel.Visibility = Visibility.Hidden;
           
            //panel.Controls.Add(OutputArea);
            //panel2.Controls.Add(InputArea);

            
            // BASIC CONFIG
            //OutputArea.Dock = InputArea.Dock = System.Windows.Forms.DockStyle.Fill;
           // OutputArea.TextChanged += TextArea_TextChanged;
           // InputArea.TextChanged += InputArea_TextChanged; 
            // INITIAL VIEW CONFIG
            OutputArea.WrapMode = InputArea.WrapMode = WrapMode.None;
            OutputArea.IndentationGuides = InputArea.IndentationGuides = IndentView.LookBoth;

            // STYLING
            InitColors(OutputArea);
            InitColors(InputArea);
            InitSyntaxColoring(OutputArea);
            InitSyntaxColoring(InputArea);
            InputArea.Styles[ScintillaNET.Style.Cpp.Word2].ForeColor = System.Drawing.Color.OrangeRed;
            InputArea.SetKeywords(1, "R Z N def void Null ArgumentError arguments Array Boolean Class Date DefinitionError Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path System File Windows Forms ScintillaNET");

        }

        private void InputArea_TextChanged(object sender, System.EventArgs e)
        {
        }

        private void TextArea_TextChanged(object sender, System.EventArgs e)
        {
        }
        private void InitColors(ScintillaNET.WPF.ScintillaWPF TextArea)
        {
            TextArea.CaretForeColor = Colors.White;
            TextArea.SetSelectionBackColor(true, Colors.Green);

        }
        public static System.Windows.Media.Color IntToMediaColor(int rgb)
        {
            return System.Windows.Media.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }
        public static System.Drawing.Color IntToColor(int rgb)
        {
            return System.Drawing.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }
       
        private void InitSyntaxColoring(ScintillaNET.WPF.ScintillaWPF TextArea)
        {

            // Configure the default style
            TextArea.StyleResetDefault();
            TextArea.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            TextArea.Styles[ScintillaNET.Style.Default].Size = 10;
            TextArea.Styles[ScintillaNET.Style.Default].BackColor = IntToColor(0x212121);
            TextArea.Styles[ScintillaNET.Style.Default].ForeColor = IntToColor(0xFFFFFF);
            TextArea.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            TextArea.Styles[ScintillaNET.Style.Cpp.Identifier].ForeColor = IntToColor(0xD0DAE2);
            TextArea.Styles[ScintillaNET.Style.Cpp.Comment].ForeColor = IntToColor(0xBD758B);
            TextArea.Styles[ScintillaNET.Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
            TextArea.Styles[ScintillaNET.Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
            TextArea.Styles[ScintillaNET.Style.Cpp.Number].ForeColor = IntToColor(0xFFFF00);
            TextArea.Styles[ScintillaNET.Style.Cpp.String].ForeColor = IntToColor(0xFFFF00);
            TextArea.Styles[ScintillaNET.Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
            TextArea.Styles[ScintillaNET.Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            TextArea.Styles[ScintillaNET.Style.Cpp.Operator].ForeColor = IntToColor(0xE0E0E0);
            TextArea.Styles[ScintillaNET.Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
            TextArea.Styles[ScintillaNET.Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            TextArea.Styles[ScintillaNET.Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
            TextArea.Styles[ScintillaNET.Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
            TextArea.Styles[ScintillaNET.Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
            TextArea.Styles[ScintillaNET.Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
            TextArea.Styles[ScintillaNET.Style.Cpp.GlobalClass].ForeColor = IntToColor(0x48A8EE);

            TextArea.Lexer = Lexer.Cpp;
            
            TextArea.SetKeywords(0, "pre post class extends implements import interface new case do while else if for in switch throw get set function var try catch finally while with default break continue delete return each const namespace package include use is as instanceof typeof author copy default deprecated eventType example exampleText exception haxe inheritDoc internal link mtasc mxmlc param private return see serial serialData serialField since throws usage version langversion playerversion productversion dynamic private public partial static intrinsic internal native override protected AS3 final super this arguments null Infinity NaN undefined true false abstract as base bool break by byte case catch char checked class const continue decimal default delegate do double descending explicit event extern else enum false finally fixed float for foreach from goto group if implicit in int interface internal into is lock long new null namespace object operator out override orderby params private protected public readonly ref return switch struct sbyte sealed short sizeof stackalloc static string select this throw true try typeof uint ulong unchecked unsafe ushort using var virtual volatile void while where yield");
            TextArea.SetKeywords(1, "def void Null ArgumentError arguments Array Boolean Class Date DefinitionError Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path System File Windows Forms ScintillaNET");

        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            InputArea.Text = "";
            InputArea.Text = file_handler.ReadFile(file_handler.OpenFile());
        }

        private void Button_Click_FileButon(object sender, RoutedEventArgs e)
        {
            if (this.FilePanel.Visibility == Visibility.Hidden)
                this.FilePanel.Visibility = Visibility.Visible;
            else
                this.FilePanel.Visibility = Visibility.Hidden;
        }

        private void Button_Click_Menu(object sender, RoutedEventArgs e)
        {

        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            file_handler.SaveFile(OutputArea.Text);
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            InputArea.Text = OutputArea.Text = "";
        }
        private void PyClick(object sender, RoutedEventArgs e)
        {
            current = state.python;
            OutputArea.Text = "";
            string text = InputArea.Text;
            if (decoder.subtract_post_conditions(text) == null)
            {
                System.Windows.MessageBox.Show("Incorrect post conditions");
            }
            else if (decoder.subtract_pre_conditions(text) == null)
            {
                System.Windows.MessageBox.Show("Incorrect pre conditions");
            }
            else if (decoder.subtract_variables(text, false) == null)
            {
                System.Windows.MessageBox.Show("Incorrect variables");
            }
            else
            {
                string text_out = py_translator.translate(text);
                OutputArea.Text = text_out;
            }
        }
        private void CSClick(object sender, RoutedEventArgs e)
        {
            current = state.cpp;
            OutputArea.Text = "";
            string text = InputArea.Text;
            if (decoder.subtract_post_conditions(text) == null)
            {
                System.Windows.MessageBox.Show("Incorrect post conditions");
            }
            else if (decoder.subtract_pre_conditions(text) == null)
            {
                System.Windows.MessageBox.Show("Incorrect pre conditions");
            }
            else if (decoder.subtract_variables(text, false) == null)
            {
                System.Windows.MessageBox.Show("Incorrect variables");
            }
            else
            {

                string text_out = cp_translator.translateCP(text);
                OutputArea.Text = text_out;
                //cp_translator.Run(ExitFileNameTextBox.Text, text_out);

            }

        }
        private void BuildBtn_Click(object sender, RoutedEventArgs e)
        {

            string text = InputArea.Text;

            if (current == state.cpp)
            {
                if (decoder.subtract_post_conditions(text) == null)
                {
                    System.Windows.MessageBox.Show("Incorrect post conditions");
                }
                else if (decoder.subtract_pre_conditions(text) == null)
                {
                    System.Windows.MessageBox.Show("Incorrect pre conditions");
                }
                else if (decoder.subtract_variables(text, false) == null)
                {
                    System.Windows.MessageBox.Show("Incorrect variables");
                }
                else
                {

                    string text_out = cp_translator.translateCP(text);
                    OutputArea.Text = text_out;
                    cp_translator.Run(ExitFileNameTextBox.Text, text_out);
                }

               
            }
            else if (current == state.python)
            {
                if (decoder.subtract_post_conditions(text) == null)
                {
                    System.Windows.MessageBox.Show("Incorrect post conditions");
                }
                else if (decoder.subtract_pre_conditions(text) == null)
                {
                    System.Windows.MessageBox.Show("Incorrect pre conditions");
                }
                else if (decoder.subtract_variables(text, false) == null)
                {
                    System.Windows.MessageBox.Show("Incorrect variables");
                }
                else
                {
                    string text_out = py_translator.translate(text);
                    OutputArea.Text = text_out;
                    py_translator.Run(ExitFileNameTextBox.Text, text_out);
                }
            }
        }

       
      
    }
}
