using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CompiladorEasyCaio
{
    //========CLASSE RESPONSAVEL NA CRIAÇÃO DE PASTA E ARQUIVOS DE TEXTO DEFAULT=================
    class cl_comandos
    {
        private string nomePastaRaiz;
        public static string localPastaRaiz;

        //Construtor
        public cl_comandos()
        {
            nomePastaRaiz = "ComandosDeApoio";
            localPastaRaiz = System.Windows.Forms.Application.StartupPath + "\\" + nomePastaRaiz;
        }

        public void CriarPastaRaiz()
        {
            DirectoryInfo pastaComandosDeApoio = new DirectoryInfo(localPastaRaiz);
            //só irá criar novos arquivos se deletar a pasta Raiz
            if (!pastaComandosDeApoio.Exists)
            {
                pastaComandosDeApoio.Create();
                criarSubPastaseArquivos();
            }
        }

        private void criarSubPastaseArquivos()
        {
            string extensaoArquivo = ".txt";
            //informo todas arquivos e as subPastas a qual pertencerão ao ser criado
            string[] pastaArquivos = new string[]
            {
                "\\Classes\\System.Math",
                "\\Classes\\System.Random",

                "\\Arrays e Matrizes\\Vetores",
                "\\Arrays e Matrizes\\Matrizes",

                "\\Syntax\\Condicionais",
                "\\Syntax\\Repeticoes",
                "\\Syntax\\try-catch",
            };

            foreach (string item in pastaArquivos)
            {
                //removo as ultimas palavras que não são pastas
                string pasta = item.Remove(item.LastIndexOf(@"\"));
                Directory.CreateDirectory(localPastaRaiz + pasta);
                //cria cada arquivo de texto eu seu endereço 
                StreamWriter sw = File.CreateText(localPastaRaiz + item + extensaoArquivo);
                sw.Close();
            }
            //envio os endereço por parametros
            inserirConteudoNosArquivosDeTexto(pastaArquivos);
        }

        private void inserirConteudoNosArquivosDeTexto(params string[] pastaArquivos)
        {
            //inseri o conteudo na ordem crescente que defini as pastas/arquivos 
            string[] conteudoTexto = new string[]
            {
                #region System.Math
@"double valordePi  = Math.PI;        //3.1415...
double raizQuadrada = Math.Sqrt(9);   //3 (3x3 = 9)
double elevacao     = Math.Pow(2, 3); //8 (2x2x2 = 8)
                
double  valorMaior = Math.Max(3.2, 4.5);  //4.5
int     valorMenor = Math.Min(-3, -2);    //-3;
                
double qtdCasasDecimais = Math.Round(3.22341, 3); //3.223
                
int arrendondaCima = (int)Math.Ceiling(8.4); //9
int arredondaBaixo = (int)Math.Floor(8.8);   //8
                
//arrendondara o inteiro mais proximo,
int arredonda = (int)Math.Round(0.4);       //0
//===========================================================
//   se o número a ser arredondado está na metade do caminho 
//   entre um inteiro e outro (ex.: 0.5) ele sempre          
//   arredondará para o número par:                          
//   Math.Round(0.5); = 0                                    
//   Math.Round(1.5); = 2                                    
//   Math.Round(2.5); = 2                                    
//===========================================================",
                #endregion      
                #region System.Random
@"//o método Next mais simples, retorna um inteiro maior ou igual ao valor min e menor que o valor max
int numSortedo = aleatorio.Next(2, 5); //+1 as end is excluded.
--------------------------------------------------------------------------------
//prencher o array com numeros randomicos maior ou igual de 0 a 255
Random rnd = new Random();           
byte[] b = new byte[5]; 
rnd.NextBytes(b); 
/*
* output example:
* b[0] = 0
* b[1] = 16
* b[2] = 189
* b[3] = 255
* b[4] = 36            
*/
-------------------------------------------------------------------------------
//O método NextDouble gera valores entre 0 e 1
Random random = new Random();
//gera numeros com até 16 casas decimais ex: 0.2105142654154274
Console.WriteLine(random.NextDouble()); 
            
//é possivel definir a quantidade de casas decimais ultilizando o metodo Round da classe Math
Console.WriteLine(Math.Round(random.NextDouble(), 2)); 
//Outputs: 0, 0.1, 0.23, 0.6, 0.67, 1
//Detalhe: utilizando esse método as casas decimais que terminam com 0 são excluidas",
                                                
                #endregion
                #region Vetores
@"//declaração de vetores
Array vetor1 = new int[10];
int[] vetor2 = new int[10];

//declaração junto com inicialização 
Array vetor3 = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
int[] vetor4 = { 19, 28, 37, 46, 55, 64, 73, 82, 91, 0 };

//atribuiçao de valores 
int variavel = 22;
vetor2[0] = 11;
vetor2[1] = variavel;
vetor2[2] = Int32.Parse(Console.ReadLine());
--------------------------------------------
//visualização com foreach
foreach (int v in vetor3)
    Console.WriteLine(v);

//visualização com for
for (byte i = 0; i < vetor4.Length; i++)
{
    Console.WriteLine(vetor4[i]);
}",
            #endregion
                #region Matrizes
@"//declaração de matrizes
char[,] matriz1 = new char[3, 2]; //Linhas e colunas

//atribuiçao de valores
matriz1[0, 0] = 'a'; //linha 0 e coluna 0 
matriz1[0, 1] = 'b'; //linha 0 e coluna 1
matriz1[1, 0] = 'c';
matriz1[1, 1] = 'd';
matriz1[2, 0] = 'e';
matriz1[2, 1] = 'f';

//declaração de matrizes com inicalizaçao de valores
string[,] matriz2 = new string[,] { 
    {""linha0-Coluna0"",""linha0-Coluna 1"", ""linha0-Coluna 2"" },//[0,0] , [0,1], [0,2]
    {""linha1-Coluna0"",""linha1-Coluna 1"", ""linha1-Coluna 2"" },//[1,0] , [1,1], [1,2]
    //Mesmo que new string[2,3]
};


//visualizaçao com foreach
foreach (char c in matriz1) 
    Console.WriteLine(c);  
              
//visualização com for
for (int linha = 0; linha < matriz2.GetLength(0); linha++)        //matriz2.GetLength(0) = 2;     
    for (int coluna = 0; coluna < matriz2.GetLength(1); coluna++) //matriz2.GetLength(1) = 3               
        Console.WriteLine(""Dimensoes: ["" + linha.ToString() + "","" + coluna.ToString() + ""] ="" + 
                            matriz2[linha, coluna]);",
                #endregion
                #region Condicionais
@"Console.WriteLine(""Digite seu Nome (1/4)"");
string nome = Console.ReadLine();
Console.WriteLine(""Digite sua Idade (2/4)"");
int idade = int.Parse(Console.ReadLine());
Console.WriteLine(""Digite sua Altura (4/4)"");
float altura = float.Parse(Console.ReadLine());
            
//If-Else
if(idade > 100 && (altura > 2.5 || altura < 1))
    Console.WriteLine(""{0} você Mentiu! não tem {1} anos, nem {2} de altura"", nome, idade, altura);
else if(idade >= 18 && idade <= 100)
    Console.WriteLine(""{0} você é maior de Idade"", nome);
else if(idade < 18 && idade > 0)
    Console.WriteLine(""{0} você é menor de Idade"", nome);
else
    Console.WriteLine(""{0} nada"", nome);

            
Console.WriteLine(""Digite seu Sexo M ou F"");
char sexo = char.Parse(Console.ReadLine());
//Switch-Case
switch(sexo) //string, int, char, bool
{
    case 'F':
    case 'f':
        Console.WriteLine(""{0} você é Mulher"", nome);
        break;
    case 'm':
    case 'M':
        Console.WriteLine(""{0} você é Homem"", nome);
        break;
    default:
        Console.WriteLine(""{0} você é {1}"", nome, sexo);
        break;
}
Console.ReadKey();",
                #endregion
                #region Repeticoes
@"//Do-While
string resp;
do
{
    Console.WriteLine(""Só paro se digitar 'sair'"");
    resp = Console.ReadLine();
} while(resp != ""sair"");

//While       
int n = 20;
while(n != 0)
{
    Console.Write(""{0} "", n);
    n--;
}
    Console.WriteLine("""");

//For
n = 20;
for(int i = 1; i <= n; i++)
{
    Console.Write(""{0} "", i); //1, 2, 3, 4...
}

//Foreach
char[] vetor = { 'a', 'b', 'c', 'd', 'e' };
foreach(char posicao in vetor)
{
    Console.WriteLine(posicao);
}

Console.ReadKey();",
            #endregion
                #region Try/Catch

@"Try-Catch
try
{
    int x = int.Parse(Console.ReadLine());
}
catch(Exception)
{
    Console.WriteLine(""Nao foi Possivel Converter o numero para inteiro"");   
}
Console.ReadKey();"
            #endregion
            };

            for (int i = 0; i <= (conteudoTexto.Length - 1); i++)
            {
                StreamWriter ficheiro = new StreamWriter(localPastaRaiz + pastaArquivos[i] + ".txt", false, Encoding.UTF8);
                ficheiro.WriteLine(conteudoTexto[i]);
                ficheiro.Close();
            }
        }
    }
}

