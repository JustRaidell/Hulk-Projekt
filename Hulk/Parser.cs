namespace Hulk;

class Parser
{
    private static List<Error> error_catcher = new List<Error>();

    //Que retornan ustedes, y que reciben?
    private static Token.Types ParseString(Token a)
    {
        return Token.Types.str;
    }

    private static Token.Types ParseNumber(Token a)
    {
        return Token.Types.number;
    }

    private static Token.Types ParseBool(Token a)
    {
        return Token.Types.boolean;
    }

    private static Token.Types ParseID(Token a)
    {
        return Token.Types.ID;
    }

    //Estos 2 son los problematicos
    //Los operadores toman como arg expresiones. O sea, tengo q crear la clase Expression
    private static bool ParseOperator(Token left, Token right)
    {
        return true;
    }

    private static bool ParseKeyword(Token a, Token b)
    {
        if (a.TokenName == "print")
        {
            //Valido si next_token es numero, string o booleano
            var temp = Parse(b, b);
            if (temp == Token.Types.number || temp == Token.Types.str || temp == Token.Types.boolean) return true;
            else return false;

            //O valido si es '(' + expresion numerica, string, booleana + ')'
            //Cualquier otro caso error

        }
        return false;
    }

    private static Token.Types Parse(Token a, Token b)
    {
        switch (a.TokenType)
        {
            case Token.Types.str:
                return ParseString(a);
            case Token.Types.number:
                return ParseNumber(a);
            case Token.Types.boolean:
                return ParseBool(a);
            case Token.Types.ID:
                return ParseID(a);
            case Token.Types.oper:
            //return ParseOperator(a);
            case Token.Types.keyword:
                ParseKeyword(a, b);
                return Token.Types.keyword;
            default:
                return Token.Types.unknown;
                //Faltan las keywords, es complicado
        }
    }

    public static void FullParser(List<Token> tokens)
    {
        Token actual_token;
        Token next_token;
        for (int i = 0; i < tokens.Count; i++)
        {
            actual_token = tokens[i];
            if (i != tokens.Count - 1)
            {
                next_token = tokens[i + 1];
                Parse(actual_token, next_token);
            }
            else Parse(actual_token, actual_token);

        }
    }
}