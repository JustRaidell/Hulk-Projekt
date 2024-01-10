namespace Hulk;

class Token
{
    private string[] keywords = { "let", "in", "function", "print", "sin", "cos", "log", "sqrt", "exp", "rand", "if", "else", "true", "false", "PI", "E" };
    private string[] operators = { "+", "-", "*", "/", "^", "=", "<=", ">=", "==", "!=", "!", "<", ">", "&", "|", "@"};
    public string Name { get; private set; }
    public Types Type { get; private set; }
    public enum Types { keyword, text, numerical, boolean, oper, punct, ID, unknown, error }
    public object? Value { get; set; }
    public int Priority {get; private set;}


    public Token(string a, object? b = null, int type = 0)
    {
        Priority = 0;
        if(a == "+" || a == "-") Priority = 1;
        if(a == "*" || a == "/") Priority = 2;
        if(a == "^") Priority = 3;
        if(a == "cos" || a == "sin" || a == "log" || a == "sqrt" || a == "exp") Priority = 4;
        if(a == "(") Priority = 5;

        Name = a;
        Value = b;

        string w = a;
        if(b != null) w = b.ToString()!;

        switch (type)
        {
            case 1:
                Type = Types.error;
                break;
            case 2:
                Type = Types.unknown;
                break;
            default:
                {
                    if (w == "true" || w == "false" || w == "True" || w == "False"){
                        Type = Types.boolean;

                        if(w.ToLower() == "true") Value = true;
                        if(w.ToLower() == "false") Value = false;
                    } 
                    else if (char.IsDigit(w.First()) || w == "PI" || w == "E" || (w.First() == '-' && Value != null)){
                        Type = Types.numerical;

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
                        Type = Types.text;
                        Value = w;
                    } 
                    else if (keywords.Contains(w)) Type = Types.keyword;
                    else if (operators.Contains(w)){
                        Type = Types.oper;
                    } 
                    else if (char.IsPunctuation(w.First())) Type = Types.punct;
                    else Type = Types.ID;
                }
                break;
        }
    }

    public Token(Token main, string? a = null){
        if(a != null) Name = a;
        else Name = main.Name;

        Type = main.Type;
        Value = main.Value;
        Priority = main.Priority;
    }

    public override string ToString()
    {
        return this.Name;
    }

    public static void ConvertToString(Token a){
        a.Type = Types.text;
    }
}