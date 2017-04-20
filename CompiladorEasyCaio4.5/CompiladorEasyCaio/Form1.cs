using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FastColoredTextBoxNS;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace CompiladorEasyCaio
{
    public partial class Form1 : Form
    {
        public const string VERSAO = "4.5";
        private string codigoInicial;
        //saida do .exe
        private string PastaSaida;
        //controle de Arquivos
        private bool csOuTxtfoiAberto;
        private string nomeArquivoAtual;
        private string diretorioArquivo;

        public Form1()
        {
            InitializeComponent();

            //Inicializacao de variaveis e propriedades
            PastaSaida = Application.StartupPath;
            nomeArquivoAtual = txtOutput.Text.Replace(".exe", string.Empty);//teste
            codigoInicial = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1 {
    class Program {
        static void Main(string[] args) {
            //Auto Generate
            Console.WriteLine(""Hello World"");
            Console.ReadKey();
        }
    }
}";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboTemas.SelectedIndex = 1;
            this.Text = "Compil v"+VERSAO;
            fctb.Text = codigoInicial;

            int totalCaracters = fctb.Text.Replace(" ", "").Replace("\r\n", "").Replace("\t", "").Length;
            lblQtdCaracteres.Text = "Caracteres: " + totalCaracters;

            csOuTxtfoiAberto = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Text.Contains('*') && fctb.Text != codigoInicial)
            {
                DialogResult respostaFechar = MessageBox.Show("Deseja Salvar antes de Sair?", "O codigo não foi Salvo!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (respostaFechar == DialogResult.Yes)
                    salvarToolStripMenuItem_Click(salvarToolStripMenuItem, EventArgs.Empty);
                else if (respostaFechar == DialogResult.Cancel)
                    e.Cancel = true;
                else
                    e.Cancel = false;//o programa fecha                
            }
        }

        #region MenuStrip

        #region [Arquivo] Eventos e Metodos
        private void novoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Text.Contains('*') && fctb.Text != codigoInicial)            
                if (MessageBox.Show("O codigo atual não foi salvo, realmente deseja substituir por um novo?", "Deseja substituir?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            
            fctb.Text = codigoInicial;
            csOuTxtfoiAberto = false;
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AbrirAquivoMenu())
            {
                csOuTxtfoiAberto = true;
                salvarToolStripMenuItem.Enabled = false;
                this.Text = nomeArquivoAtual + "   -   " + "Compil v" + VERSAO;
            }
        }

        private bool AbrirAquivoMenu()
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Abrir arquivo Linguagem c# (console)",
                Multiselect = false,
                ValidateNames = true,
                Filter = "cs ou texto (*.cs)(*.txt)|*.cs;*.txt|" +
                                     "Arquivos de Texto (*.txt)|*.txt|" +
                                     "Arquivos de cs (*.cs)|*.cs|" +
                                     "Outros |*.*"
            })
                if (ofd.ShowDialog() == (DialogResult.OK))
                {
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        fctb.Text = sr.ReadToEnd();
                    }
                    //Se a extensão for txt
                    if (Path.GetExtension(ofd.SafeFileName) == ".txt")
                        //A textbox saida recebe o nome menos as 3 ultimas letras
                        txtOutput.Text = ofd.SafeFileName.Substring(0, ofd.SafeFileName.Length - 3);
                    else if (Path.GetExtension(ofd.SafeFileName) == ".cs")
                        txtOutput.Text = ofd.SafeFileName.Substring(0, ofd.SafeFileName.Length - 2);
                    //Recebe o nome mais a extensão
                    txtOutput.Text += "exe";
                    nomeArquivoAtual = Path.GetFileName(ofd.FileName);
                    diretorioArquivo = ofd.FileName;
                    return true;
                }
                else
                    return false;

        }

        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //se o arquivo ja foi aberto antes e existe
            if (csOuTxtfoiAberto && File.Exists(diretorioArquivo))
            {   //salvara o arquivo sobrescrevendo e pulara para etapa2
                fctb.SaveToFile(diretorioArquivo, Encoding.ASCII);
                goto etapa2;
            }
            //se nao, pergunto onde quer salvar e avança a etapa1 e etapa2
            else if (SalvarComo())
            {
                goto etapa1;
            }
            //se cancelou o "salvar como" as etapas nao executara
            else
                return;
        etapa1:
            csOuTxtfoiAberto = true;

        etapa2:
            this.Text = nomeArquivoAtual + "   -   " + "Compil v"+VERSAO;
            salvarToolStripMenuItem.Enabled = false;
        }

        private void salvarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SalvarComo())
            {
                this.Text = nomeArquivoAtual + "   -   " + "Compil v" + VERSAO;
                salvarToolStripMenuItem.Enabled = false;
            }
        }

        private void imprimirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.Print(new PrintDialogSettings() { ShowPrintPreviewDialog = true });
        }
        private bool SalvarComo()
        {
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Salvar Arquivo |" + "Compil v" + VERSAO,
                Filter = "C# (*.cs)|*.cs|Text (*.txt)|*.txt",
                FileName = txtOutput.Text.Replace(".exe", string.Empty)
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, fctb.Text, Encoding.ASCII);
                    nomeArquivoAtual = Path.GetFileName(sfd.FileName);
                    diretorioArquivo = sfd.FileName;
                    return true;
                }
                else
                    return false;
            }
        }

        #endregion

        #region [Editar] Eventos
        private void desfazerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fctb.UndoEnabled && fctb.Text != codigoInicial)
                fctb.Undo();            
        }

        private void refazerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fctb.RedoEnabled)
                fctb.Redo();
        }

        private void recortarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fctb.Cut();
        }

        private void copiarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fctb.Copy();
        }

        private void colarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fctb.Paste();
        }

        private void selecionarTudoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fctb.SelectAll();
        }

        private void procurarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.ShowFindDialog();
        }

        private void substituirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.ShowReplaceDialog();
        }
        #endregion

        #region [Configurações] Eventos        
        private void quebraDeLinhasToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            fctb.WordWrap = chkQuebraDeLinhas.Checked;
        }

        private void scrollMapPersonalizadoToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (chkScrollMap.Checked)
            {
                documentMap1.Visible = true;
                //adaptando o tamanho do txt
                fctb.Width -= documentMap1.Width;
            }
            else
            {
                documentMap1.Visible = false;
                fctb.Width += documentMap1.Width;
            }
        }
        
        private void esconderErroListToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = esconderErroListToolStripMenuItem.Checked;
        }

        private void teclasDeAtalhosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new HotkeysEditorForm(fctb.HotkeysMapping);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                fctb.HotkeysMapping = form.GetHotkeys();
        }
        #endregion

        #region [Mudar] Eventos
        private void corDeFundoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog bgColorTela = new ColorDialog())
                if (bgColorTela.ShowDialog() == DialogResult.OK)
                    fctb.BackColor = bgColorTela.Color;

        }

        private void fonteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FontDialog fontLetraTela = new FontDialog()
            {
                ShowColor = true,
                FontMustExist = true,
                MinSize = 9,
                MaxSize = 22
            })
            {
                if (fontLetraTela.ShowDialog() == DialogResult.OK)
                {
                    fctb.ForeColor = fontLetraTela.Color;
                    fctb.Font = fontLetraTela.Font;
                }
            }
        }
        #endregion

        #region [Temas(light,dark...)] Eventos e Metodos
        private void cboTemas_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboTemas.Text)
            {
                case "Light":
                    AlterarHightLight(Color.MediumBlue,                 //cor da seleção
                                      Color.White,                      //cor de fundo     
                                      Color.Black,                      //cor das letras padrao
                                      Color.DarkMagenta,                //Cor dos numeros
                                      Color.Blue,                       //Cor das das palavras chaves
                                      Color.Green,                      //cor dos comentarios
                                      Color.Black,                      //cor das classes
                                      Color.Brown);                     //cor das String
                    fctb.LineNumberColor = Color.Teal;
                    fctb.FoldingIndicatorColor = Color.Green;
                    break;
                case "Dark":
                    AlterarHightLight(Color.LightSkyBlue,               //cor da seleção
                                      Color.FromArgb(30, 30, 30),       //cor de fundo     
                                      Color.White,                      //cor das letras padrao
                                      Color.FromArgb(184, 215, 163),    //Cor dos numeros
                                      Color.FromArgb(86, 156, 214),     //Cor das das palavras chaves
                                      Color.FromArgb(96, 139, 78),      //cor dos comentarios
                                      Color.FromArgb(78, 201, 176),     //cor das classes
                                      Color.FromArgb(214, 157, 133));   //cor das String                    
                    fctb.LineNumberColor = Color.LightSeaGreen;
                    fctb.FoldingIndicatorColor = Color.Lime;
                    break;
                case "Brown":
                    AlterarHightLight(Color.SandyBrown,                 //cor da seleção
                                      Color.FromArgb(26, 15, 11),       //cor de fundo     
                                      Color.FromArgb(195, 190, 152),    //cor das letras padrao
                                      Color.FromArgb(221, 200, 193),    //Cor dos numeros
                                      Color.FromArgb(241, 230, 148),     //Cor das das palavras chaves
                                      Color.FromArgb(124, 165, 99),     //cor dos comentarios
                                      Color.FromArgb(179, 147, 92),      //cor das classes                                      
                                      Color.LightGray);                  //cor das String
                    fctb.LineNumberColor = Color.Goldenrod;
                    fctb.FoldingIndicatorColor = Color.Khaki;
                    break;
                case "PinkW":
                    AlterarHightLight(Color.Orchid,                 //cor da seleção
                                       Color.FromArgb(30, 0, 30),                //cor de fundo     
                                       Color.White,    //cor das letras padrao
                                       Color.Coral,    //Cor dos numeros
                                       Color.Magenta,     //Cor das das palavras chaves
                                       Color.Green,     //cor dos comentarios
                                       Color.MediumSlateBlue,      //cor das classes                                      
                                       Color.Plum);                  //cor das String
                    fctb.LineNumberColor = Color.DarkViolet;
                    fctb.FoldingIndicatorColor = Color.Salmon;
                    break;
                default:
                    MessageBox.Show("Este tema nao está disponivel", "Tema não disponivel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
            //Aplicando no textbox           
            fctb.OnSyntaxHighlight(new TextChangedEventArgs(fctb.Range));
        }

        private void AlterarHightLight(Color selectionColor, Color backColor, Color foreColor, Color numberColor, Color keyWordsColor, Color comentsColor, Color classNamesColor, Color stringsColor)
        {
            //limpo todos estilos
            fctb.ClearStyle(StyleIndex.All);
            fctb.ClearStylesBuffer();
            //cor da seleção
            fctb.SelectionColor = selectionColor;
            //cor de fundo 
            fctb.BackColor = backColor;
            //cor das letras padrao
            fctb.ForeColor = foreColor;
            //Cor dos numeros
            fctb.SyntaxHighlighter.NumberStyle = StyleCor(numberColor);
            //Cor das das palavras chaves
            fctb.SyntaxHighlighter.KeywordStyle = StyleCor(keyWordsColor);
            //cor dos comentarios
            fctb.SyntaxHighlighter.CommentStyle = StyleCor(comentsColor);
            //cor das classes
            fctb.SyntaxHighlighter.ClassNameStyle = StyleCor(classNamesColor);
            //cor das String
            fctb.SyntaxHighlighter.StringStyle = StyleCor(stringsColor);
        }
        //metodo implementar ao AlterarHightLight
        private Style StyleCor(Color minhaCor, FontStyle estiloFonte = FontStyle.Regular)
        {
            SolidBrush corStyle = new SolidBrush(minhaCor);
            Style myStyle = new TextStyle(corStyle, null, estiloFonte);
            return myStyle;
        }
        #endregion

        #endregion

        #region Eventos ToolStrip
        private void txtOutput_Click(object sender, EventArgs e)
        {
            txtOutput.Select(0, txtOutput.Text.Length - 4);
        }

        private void btnPastaSaidaExe_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folder = new FolderBrowserDialog()
            {   //Abrira o local da string
                SelectedPath = PastaSaida
            })
            {
                if (folder.ShowDialog() == DialogResult.OK)
                {   //variavel recebe a pasta selecionada pelo usuário + nome do arquivo
                    PastaSaida = folder.SelectedPath;
                }
            }
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {
            Stopwatch tempoExecucao = new Stopwatch();

            if (!txtOutput.Text.Trim().Contains(".exe"))
                txtOutput.Text = txtOutput.Text + ".exe";

            if (txtOutput.Text == ".exe")
            {
                MessageBox.Show("Informe o nome da saida do programa!", "Por favor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtOutput.Focus();
                return;
            }

            try
            {   //Inicio da contagem
                tempoExecucao.Start();
                //a pasta recebe ela mais o nome do arquivo                
                string enderecoBuild = PastaSaida + "\\" + txtOutput.Text;
                CSharpCodeProvider csc = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });
                CompilerParameters parametros = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, enderecoBuild, true);
                parametros.GenerateExecutable = true;
                CompilerResults resultado = csc.CompileAssemblyFromSource(parametros, fctb.Text);
                if (resultado.Errors.HasErrors)
                {
                    resultado.Errors.Cast<CompilerError>().ToList().ForEach(error => txtStatus.Text = error.ErrorText + "\r\nLinha: "+error.Line+"\tColuna: "+error.Column);
                    txtStatus.BackColor = Color.FromArgb(255, 40, 40);//vermelho                    
                }
                else
                {
                    txtStatus.Text = ("-----Build Succeeded-----\r\n" + enderecoBuild +
                                      "\r\nTempo de Execução: " + tempoExecucao.Elapsed);
                    Process.Start(enderecoBuild);
                    txtStatus.BackColor = Color.FromArgb(0, 170, 10);//verde
                }
                //Fim da contagem
                tempoExecucao.Stop();
                tempoExecucao.Reset();
            }
            //caso o framework da maquina nao estiver correto
            catch (Exception ex)
            {
                txtStatus.Text = "Erro no Framework Reinstale:\r\n" + ex.Message;
                txtStatus.BackColor = Color.Yellow;
                linkPastaArquivos.Visible = false;
            }

            linkPastaArquivos.BackColor = txtStatus.BackColor;
        }
        
        private void chksToolStrip_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDestacarLinhaAtual.Checked)
                fctb.CurrentLineColor = Color.Gray;
            else
                fctb.CurrentLineColor = Color.Transparent;

            fctb.ShowFoldingLines = chkExibirDobrasDeLinhas.Checked;

            fctb.AutoCompleteBrackets = chkAutoFecharChaves.Checked;
        }

        private void btnComentar_Click(object sender, EventArgs e)
        {
            ToolStripButton botoesComentar = (sender as ToolStripButton);

            if (botoesComentar.Tag.ToString() == "comentar")
                fctb.InsertLinePrefix(fctb.CommentPrefix);
            else if (botoesComentar.Tag.ToString() == "descomentar")
                fctb.RemoveLinePrefix(fctb.CommentPrefix);
        }

        private void btnAutoIdentar_Click(object sender, EventArgs e)
        {
            fctb.DoAutoIndent();
        }

        private void btnsBookMarks_Click(object sender, EventArgs e)
        {
            ToolStripButton botaoBookMark = (sender as ToolStripButton);

            switch (botaoBookMark.Tag.ToString())
            {
                case "AddBook":
                    fctb.BookmarkLine(fctb.Selection.Start.iLine);
                    break;
                case "RemoveBook":
                    fctb.UnbookmarkLine(fctb.Selection.Start.iLine);
                    break;
                case "BackBook":
                    fctb.GotoPrevBookmark(fctb.Selection.Start.iLine);
                    break;
                case "NextBook":
                    fctb.GotoNextBookmark(fctb.Selection.Start.iLine);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region eventos do Editor fctb
        //Adição de book marks ao clickar na lateral
        private void fctb_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.X < fctb.LeftIndent)
            {
                var place = fctb.PointToPlace(e.Location);
                if (fctb.Bookmarks.Contains(place.iLine))
                    fctb.Bookmarks.Remove(place.iLine);
                else
                    fctb.Bookmarks.Add(place.iLine);
            }
        }

        private void fctb_SelectionChanged(object sender, EventArgs e)
        {
            lblLinhaAtual.Text = "Linha: " + (fctb.Selection.Start.iLine + 1).ToString();
            lblColunaAtual.Text = "Coluna: " + (fctb.Selection.Start.iChar + 1).ToString();
        }

        MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));
        private void fctb_SelectionChangedDelayed(object sender, EventArgs e)
        {
            fctb.VisibleRange.ClearStyle(SameWordsStyle);
            if (!fctb.Selection.IsEmpty)
                return;//user selected diapason

            //get fragment around caret
            var fragment = fctb.Selection.GetFragment(@"\w");
            string text = fragment.Text;
            if (text.Length == 0)
                return;
            //highlight same words
            var ranges = fctb.VisibleRange.GetRanges("\\b" + text + "\\b").ToArray();
            if (ranges.Length > 1)
                foreach (var r in ranges)
                    r.SetStyle(SameWordsStyle);
        }

        private void fctb_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Text = nomeArquivoAtual + "*" + "   -   " + "Compil v" + VERSAO;
            salvarToolStripMenuItem.Enabled = true;

            lblQtdCaracteres.Text = "Caracteres: " + fctb.Text.Replace(" ", "").Replace("\r\n", "").Replace("\t", "").Length;
        }

        private void fctb_BackColorChanged(object sender, EventArgs e)
        {
            fctb.ChangedLineColor = fctb.BackColor;
            fctb.IndentBackColor = fctb.BackColor;
            txtStatus.BackColor = fctb.BackColor;
            linkPastaArquivos.BackColor = fctb.BackColor;
            documentMap1.BackColor = fctb.BackColor;
        }
        #endregion

        #region Eventos contextMenuStrip
        private void clonarComComentarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //start autoUndo block
            fctb.BeginAutoUndo();
            //expand selection
            fctb.Selection.Expand();
            //get text of selected lines
            string text = Environment.NewLine + fctb.Selection.Text;
            //comment lines
            fctb.InsertLinePrefix("//");
            //move caret to end of selected lines
            fctb.Selection.Start = fctb.Selection.End;
            //insert text
            fctb.InsertText(text);
            //end of autoUndo block
            fctb.EndAutoUndo();
        }

        private void clonarLinhaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //expand selection
            fctb.Selection.Expand();
            //get text of selected lines
            string text = Environment.NewLine + fctb.Selection.Text;
            //move caret to end of selected lines
            fctb.Selection.Start = fctb.Selection.End;
            //insert text
            fctb.InsertText(text);
        }

        #region Eventos e classes para colorir Seleção
        MarkerStyle estiloAzul = new MarkerStyle(new SolidBrush(Color.FromArgb(180, Color.Blue)));
        MarkerStyle estiloVermelho = new MarkerStyle(new SolidBrush(Color.FromArgb(180, Color.Red)));
        MarkerStyle estiloAmarelo = new MarkerStyle(new SolidBrush(Color.FromArgb(180, Color.Yellow)));
        MarkerStyle estiloVerde = new MarkerStyle(new SolidBrush(Color.FromArgb(180, Color.Green)));
        private void subCoresDestacarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrimSelection();
            if (fctb.SelectedText.Equals(string.Empty))
            {
                switch ((string)((sender as ToolStripMenuItem).Tag))
                {
                    case "Blue": fctb[fctb.Selection.Start.iLine].BackgroundBrush = Brushes.Blue; break;
                    case "Red": fctb[fctb.Selection.Start.iLine].BackgroundBrush = Brushes.Red; break;
                    case "Yellow": fctb[fctb.Selection.Start.iLine].BackgroundBrush = Brushes.Yellow; break;
                    case "Green": fctb[fctb.Selection.Start.iLine].BackgroundBrush = Brushes.Green; break;
                }
            }
            else
            {
                switch ((string)((sender as ToolStripMenuItem).Tag))
                {
                    case "Blue": fctb.Selection.SetStyle(estiloAzul); break;
                    case "Red": fctb.Selection.SetStyle(estiloVermelho); break;
                    case "Yellow": fctb.Selection.SetStyle(estiloAmarelo); break;
                    case "Green": fctb.Selection.SetStyle(estiloVerde); break;
                }
            }
            destacarToolStripMenuItem.ForeColor = Color.FromName((sender as ToolStripMenuItem).Tag.ToString());
        }

        private void destacarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrimSelection();
            if (fctb.SelectedText.Equals(string.Empty))
            {
                switch (destacarToolStripMenuItem.ForeColor.Name)
                {
                    case "Blue": fctb[fctb.Selection.Start.iLine].BackgroundBrush = Brushes.Blue; break;
                    case "Red": fctb[fctb.Selection.Start.iLine].BackgroundBrush = Brushes.Red; break;
                    case "Yellow": fctb[fctb.Selection.Start.iLine].BackgroundBrush = Brushes.Yellow; break;
                    case "Green": fctb[fctb.Selection.Start.iLine].BackgroundBrush = Brushes.Green; break;
                }
                fctb.SelectedText = "";
            }
            else
            {
                switch (destacarToolStripMenuItem.ForeColor.Name)
                {
                    case "Blue": fctb.Selection.SetStyle(estiloAzul); break;
                    case "Red": fctb.Selection.SetStyle(estiloVermelho); break;
                    case "Yellow": fctb.Selection.SetStyle(estiloAmarelo); break;
                    case "Green": fctb.Selection.SetStyle(estiloVerde); break;
                    default: fctb.Selection.SetStyle(estiloVermelho); break;
                }
            }
            contextMenuStrip1.Close();
        }

        private void removerDestaqueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.Selection.ClearStyle(estiloAzul, estiloVermelho, estiloAmarelo, estiloVerde);
            fctb[fctb.Selection.Start.iLine].BackgroundBrush = null;
        }

        private void TrimSelection()
        {
            var sel = fctb.Selection;

            //trim left
            sel.Normalize();
            while (char.IsWhiteSpace(sel.CharAfterStart) && sel.Start < sel.End)
                sel.GoRight(true);
            //trim right
            sel.Inverse();
            while (char.IsWhiteSpace(sel.CharBeforeStart) && sel.Start > sel.End)
                sel.GoLeft(true);
        }
        #endregion
        #endregion

        private void linkPastaArquivos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(PastaSaida);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F5:
                    btnCompilar_Click(this.btnCompilar, EventArgs.Empty);
                    return true;
                case Keys.F2:
                    txtOutput.Focus();
                    txtOutput_Click(this.txtOutput, EventArgs.Empty);
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sobre frmSobre = new Sobre();
            frmSobre.ShowDialog();
        }

        private void comandosToolStripMenuItem_Click(object sender, EventArgs e)
        {  
            Comandos cmdAjuda = new Comandos();
            cmdAjuda.Show();
        }
    }
}
