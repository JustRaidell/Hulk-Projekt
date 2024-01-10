// See https://aka.ms/new-console-template for more information
namespace Hulk;

class Program
{
    public static Dictionary<string, (List<string>, List<Token>)> context = new Dictionary<string, (List<string>, List<Token>)>();

    public static void Main(string[] args)
    {
        Console.WriteLine("Bienvenido al interprete de H.U.L.K. Introduzca una linea de codigo o presione Ctrl^C para salir");
        while (true)
        {
            string input = Console.ReadLine()!;

            if(input == string.Empty) continue;

            var tokens = Lexer.Tokenizer(input);

            if(Lexer.error_catcher.Count != 0){
                Console.WriteLine("Errores encontrados. Corrijalos y vuelva a intentar");
                Console.WriteLine("");
                Lexer.ErrorList();
                Console.WriteLine("");
            }
            else{
                Parser.FullParser(tokens, new Dictionary<string, Token>());
                if(Parser.error_catcher.Count != 0){
                    Console.WriteLine("");
                    Error.ShowError(Parser.error_catcher[0]);
                    Console.WriteLine("");
                }
            } 
        }
    }

}


