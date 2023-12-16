// See https://aka.ms/new-console-template for more information
namespace Hulk;

class Program
{
    public static Dictionary<string, (string, List<Token>)> context = new Dictionary<string, (string, List<Token>)>();

    private static void PrintState(List<Token> tokens)
    {
        Console.WriteLine("Tokens:");
        for (int i = 0; i < tokens.Count; i++)
        {
            Console.WriteLine(tokens[i].TokenName + "\t" + tokens[i].TokenType);
        }
        Console.WriteLine("");
        Console.WriteLine("");

        Console.WriteLine("Errors:");
        Lexer.ErrorList();
        Console.WriteLine("");
        Console.WriteLine("*****************************************");
    }

    public static void Main(string[] args)
    {
        System.Console.WriteLine("Bienvenido al interprete de H.U.L.K. Introduzca una linea de codigo o presione Ctrl^C para salir");
        while (true)
        {
            string input = Console.ReadLine()!;
            var tokens = Lexer.Tokenizer(input);

            //PrintState(tokens);
            // if(Lexer.error_catcher.Count != 0){
            //     Console.WriteLine("Errores encontrados. Corrijalos y vuelva a intentar");
            //     Console.WriteLine("");
            //     PrintState(tokens);
            // }
            // else Parser.AbsoluteFullParser(tokens);
            Parser.PrimaryParser(tokens);

        }
    }

}


