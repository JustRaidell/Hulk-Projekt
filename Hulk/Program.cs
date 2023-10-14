// See https://aka.ms/new-console-template for more information
namespace Hulk;

class Program
{
    private static void PrintState(List<Token> tokens){
        Console.WriteLine("Tokens:");
        for (int i = 0; i < tokens.Count; i++)
        {
            Console.WriteLine(tokens[i].TokenName + "\t" + tokens[i].TokenType);
        }
        //Console.WriteLine("End tokens");
        Console.WriteLine("");
        Console.WriteLine("");

        Console.WriteLine("Errors:");
        Lexer.ErrorList();
    }

    public static void Main(string[] args)
    {
        // char juan = '<';
        // System.Console.WriteLine(char.IsSymbol(juan));

        string input = Console.ReadLine()!;
        var tokens = Lexer.Tokenizer(input);

        //PrintState(tokens);

        Parser.FullParser(tokens);
    }

}


