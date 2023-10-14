namespace Hulk;

class Token{
    private string[] keywords = {"let", "in", "function", "print", "sin", "cos", "log", "sqrt", "exp", "rand", "if", "else", "true", "false", "PI", "E"};
    private string[] operators = {"+", "-", "*", "/", "^", "=", "<=", ">=", "==", "!", "<", ">", "&", "|"};
    public string TokenName {get; private set;}
    public Types TokenType {get; private set;}
    public enum Types {keyword, str, number, boolean, oper, punct, ID, unknown}//{get; private set;}
    //public string TokenType {get; private set;}

    public Token(string a){
        TokenName = a;

        if(TokenName == "true" || TokenName == "false") TokenType = Types.boolean;
        else if(char.IsDigit(TokenName.First()) || TokenName == "PI" || TokenName == "E") TokenType = Types.number;
        else if(keywords.Contains(TokenName)) TokenType = Types.keyword;
        else if(TokenName.First() == '\"') TokenType = Types.str;
        else if(char.IsPunctuation(TokenName.First())) TokenType = Types.punct;
        else if(char.IsSymbol(TokenName.First())) TokenType = Types.oper;
        else TokenType = Types.ID;
    }

    public static string ToString(Token a){
        return a.TokenName;
    }
}

// class Keyword : Token
// {
    
// }