namespace Hulk;

class Token
{
    private string[] keywords = { "let", "in", "function", "print", "sin", "cos", "log", "sqrt", "exp", "rand", "if", "else", "true", "false", "PI", "E" };
    private string[] operators = { "+", "-", "*", "/", "^", "=", "<=", ">=", "==", "!=", "!", "<", ">", "&", "|", "@", ""};
    public string TokenName { get; private set; }
    public Types TokenType { get; private set; }
    public enum Types { keyword, str, number, boolean, oper, punct, ID, unknown, error }
    public object? Value { get; set; }


    public Token(string a, object? b = null, int type = 0)
    {
        TokenName = a;
        Value = b;

        string w = a;
        if(b != null) w = b.ToString()!;

        switch (type)
        {
            case 1:
                TokenType = Types.error;
                break;
            case 2:
                TokenType = Types.unknown;
                break;
            default:
                {
                    if (w == "true" || w == "false" || w == "True" || w == "False"){
                        TokenType = Types.boolean;

                        if(w.ToLower() == "true") Value = true;
                        if(w.ToLower() == "false") Value = false;
                    } 
                    else if (char.IsDigit(w.First()) || w == "PI" || w == "E" || (w.First() == '-' && Value != null)){
                        TokenType = Types.number;

                        if(w == "PI"){
                            Value = double.Pi;
                            break;
                        } 
                        if(w == "E"){
                            Value = double.E;
                            break;
                        } 
                        Value = double.Parse(w);
                    } 
                    else if (w.First() == '\"'){
                        TokenType = Types.str;
                        Value = w;
                    } 
                    else if (keywords.Contains(w)) TokenType = Types.keyword;
                    else if (operators.Contains(w)) TokenType = Types.oper;
                    else if (char.IsPunctuation(w.First())) TokenType = Types.punct;
                    else TokenType = Types.ID;
                }
                break;
        }
    }

    public override string ToString()
    {
        return this.TokenName;
    }

    public static void Status(Token a){
        System.Console.WriteLine("");
        System.Console.WriteLine(a.TokenName);
        System.Console.WriteLine(a.TokenType);
        System.Console.WriteLine(a.Value);
        if(a.Value != null) System.Console.WriteLine(a.Value.GetType());
        System.Console.WriteLine("");
    }
}

// class Keyword : Token
// {
//     public Keyword(string a) : base(a)
//     {
//         if (w == "sin" || w == "cos" || w == "log" ||w == "sqrt")
//         {
//             TokenType = Keyword.Types.numerical;
//         }

//     }

//     new public Types TokenType;
//     new public enum Types { numerical, boolean, ID, unknown };
// }