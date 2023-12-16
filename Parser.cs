namespace Hulk;

class Parser
{
    private static List<Error> error_catcher = new List<Error>();
    private static Token unknown = new Token("", null, 2);
    private static Token error = new Token("", null, 1);
    private static bool[] TokenMask = new bool[1];


    private static Token ParseString(Token a)
    {
        string temp = a.TokenName.Remove(0,1);
        a.Value = temp.Remove(temp.Length - 1);
        return a;
    }

    private static Token ParseNumber(Token a)
    {
        if (a.TokenName != "PI" && a.TokenName != "E") a.Value = double.Parse(a.TokenName);
        return a;
    }

    private static Token ParseBool(Token a)
    {
        if (a.TokenName == "true") a.Value = true;
        else if (a.TokenName == "false") a.Value = false;
        return a;
    }

    private static Token ParseID(Token a, Dictionary<string, Token> scope)
    {
        if (!scope.ContainsKey(a.TokenName)) return new Token(a.TokenName);
        else return scope[a.TokenName];
    }

    private static Token ParseOperator(Token a, List<Token> b, Dictionary<string, Token> scope, List<string> context, int plus, Token prev_token)
    {
        var right = Parse(b[b.IndexOf(a) + 1], b, scope, context, unknown, plus);

        if (a.TokenName == "!")
        {
            if (right.TokenType != Token.Types.boolean){
                error_catcher.Add (new Error("Semantic error", "Expected boolean expression after !"));
                return error;
            } 
            else return new Token(a.TokenName, !((bool)right.Value!));
        }

        if (a == b.First()){
            error_catcher.Add (new Error("Syntax error", "Missing left part of the operator"));
            return error;
        } 

        Token left;
        if (prev_token.TokenType == Token.Types.unknown) left = Parse(b[b.IndexOf(a) - 1], b, scope, context, unknown, plus);
        else left = prev_token;

        if (a.TokenName == "=")
        {
            if (left.TokenType != Token.Types.ID && !scope.ContainsKey(left.TokenName)){
                error_catcher.Add (new Error("Semantic error", "Assignment requires variable on left part"));
                return error;
            } 
            else if (right.TokenType != Token.Types.str && right.TokenType != Token.Types.number && right.TokenType != Token.Types.boolean){
                error_catcher.Add (new Error("Semantic error", "You cannot assign this type"));
                return error;
            } 
            else{
                if(scope.ContainsKey(left.TokenName)){
                    var temp = new Token(left.TokenName, right.Value);
                    left = temp;
                }
                else left = right;
            }
            return left;
        }
        if (a.TokenName == "|" || a.TokenName == "&")
        {
            if (right.TokenType != Token.Types.boolean || left.TokenType != Token.Types.boolean) return error;
            else
            {
                if (a.TokenName == "|") return new Token(a.TokenName, (bool)left.Value! || (bool)right.Value!);
                if (a.TokenName == "&") return new Token(a.TokenName, (bool)left.Value! && (bool)right.Value!);
            }
        }
        if (a.TokenName == "<" || a.TokenName == "<=" || a.TokenName == ">" || a.TokenName == ">=")
        {
            if (right.TokenType != Token.Types.number || left.TokenType != Token.Types.number) return error;
            else
            {
                if (a.TokenName == "<") return new Token(a.TokenName, (double)left.Value! < (double)right.Value!);
                if (a.TokenName == ">") return new Token(a.TokenName, (double)left.Value! > (double)right.Value!); ;
                if (a.TokenName == "<=") return new Token(a.TokenName, (double)left.Value! <= (double)right.Value!); ;
                if (a.TokenName == ">=") return new Token(a.TokenName, (double)left.Value! >= (double)right.Value!); ;
            }
        }
        if (a.TokenName == "!=" || a.TokenName == "==")
        {
            if (right.TokenType != left.TokenType) return error;
            else
            {
                if (a.TokenName == "!=") return new Token(a.TokenName, !left.Value!.Equals(right.Value));
                if (a.TokenName == "==") return new Token(a.TokenName, left.Value!.Equals(right.Value));
            }
        }
        if (a.TokenName == "+" || a.TokenName == "-" || a.TokenName == "*" || a.TokenName == "/" || a.TokenName == "^")
        {
            if (right.TokenType != Token.Types.number || left.TokenType != Token.Types.number) return error;
            else
            {
                if (a.TokenName == "+") return new Token(a.TokenName, (double)left.Value! + (double)right.Value!);
                if (a.TokenName == "-") return new Token(a.TokenName, (double)left.Value! - (double)right.Value!);
                if (a.TokenName == "*") return new Token(a.TokenName, (double)left.Value! * (double)right.Value!);
                if (a.TokenName == "/")
                {
                    if ((double)right.Value! == 0) return error;
                    return new Token(a.TokenName, (double)left.Value! / (double)right.Value!);
                }
                if (a.TokenName == "^") return new Token(a.TokenName, Math.Pow((double)left.Value!, (double)right.Value!));
            }
        }
        if (a.TokenName == "@")
        {
            if ((right.TokenType != Token.Types.str && right.TokenType != Token.Types.number) || (left.TokenType != Token.Types.str && left.TokenType != Token.Types.number)) return error;
            else return new Token(a.TokenName, (string)left.Value! + (string)right.Value!);
        }

        return unknown;
    }

    private static Token ParseKeyword(Token a, List<Token> b, Dictionary<string, Token> scope, List<string> context, int plus)
    {
        var next_token = Parse(b[b.IndexOf(a) + 1], b, scope, context, unknown, plus);

        if (a.TokenName == "print")
        {
            if (next_token.TokenType != Token.Types.number && next_token.TokenType != Token.Types.str && next_token.TokenType != Token.Types.boolean) return error;
            else
            {
                Console.WriteLine(next_token.Value);
                return new Token("print", next_token.Value);
            }
        }
        if (a.TokenName == "cos" || a.TokenName == "sin" || a.TokenName == "log" || a.TokenName == "sqrt" || a.TokenName == "exp")
        {
            if (a.TokenName == "log")
            {
                var log_base = Parse(b[b.IndexOf(a) + 2], b, scope, context, unknown, plus);
                var log_argument = Parse(b[b.IndexOf(a) + 4], b, scope, context, unknown, plus);
                if (log_base.TokenType != Token.Types.number && log_argument.TokenType != Token.Types.number) return error;
                else return new Token("log", Math.Log((double)log_argument.Value!, (double)log_base.Value!));
            }
            if (next_token.TokenType != Token.Types.number) return error;
            else
            {
                if (a.TokenName == "cos") return new Token("cos", Math.Cos((double)next_token.Value!));
                if (a.TokenName == "sin") return new Token("sin", Math.Sin((double)next_token.Value!));
                if (a.TokenName == "sqrt") return new Token("sqrt", Math.Sqrt((double)next_token.Value!));
                if (a.TokenName == "exp") return new Token("exp", Math.Pow(double.E, (double)next_token.Value!));
            }
        }
        if (a.TokenName == "if")
        {
            if (next_token.TokenType != Token.Types.boolean) return error;
            else
            {
                List<Token> cuerpo = new List<Token>();
                for (int i = b.IndexOf(a); i < b.Count; i++)
                {
                    if (!TokenMask[i + plus])
                    {
                        cuerpo = NewExpression(i, b, new string[] { "" });
                        break;
                    }
                }
                var if_body = NewExpression(0, cuerpo, new string[] { "else" });

                if (if_body.Count == cuerpo.Count) return error;

                var else_body = NewExpression(if_body.Count + 1, cuerpo, new string[] { ";" });

                if ((bool)next_token.Value! == true)
                {
                    var parsed_if = SemiParser(if_body, scope, context, b.Count - cuerpo.Count + 1 + plus);

                    for (int i = 0; i < cuerpo.Count; i++)
                    {
                        TokenMask[b.Count - cuerpo.Count + i +plus] = true;
                    }

                    if (parsed_if.Count == 0) return unknown;
                    else if (parsed_if.Count == 1) return parsed_if[0];
                    else return error;
                }
                else if ((bool)next_token.Value! == false)
                {
                    TokenMask[b.Count - cuerpo.Count + if_body.Count + plus] = true;
                    var parsed_else = SemiParser(else_body, scope, context, (b.Count - cuerpo.Count) + if_body.Count + 1 + plus);

                    for (int i = 0; i < cuerpo.Count; i++)
                    {
                        TokenMask[b.Count - cuerpo.Count + i + plus] = true;
                    }

                    if (parsed_else.Count == 0) return unknown;
                    else if (parsed_else.Count == 1) return parsed_else[0];
                    else return error;
                }
            }
        }
        if (a.TokenName == "let")
        {
            if (b[b.IndexOf(a) + 1].TokenType != Token.Types.ID) return error;
            if (next_token.TokenType != Token.Types.number && next_token.TokenType != Token.Types.str && next_token.TokenType != Token.Types.boolean) return error;

            var cuerpo = NewExpression(b.IndexOf(a) + 1, b, new string[] { ";" });
            List<Token> e = new List<Token>(cuerpo);
            Token l = new Token("let");
            List<Token> result = new List<Token>();
            result.Add(l);

            var new_scope = new Dictionary<string, Token>();
            if (scope.Count != 0) new_scope = new Dictionary<string, Token>(scope);
            var last_id = new Token(cuerpo[0].TokenName, next_token.Value);

            for (int i = 0; i < cuerpo.Count; i++)
            {
                if (cuerpo[i].TokenName == ",")
                {
                    TokenMask[i + plus + 1] = true;

                    if (cuerpo[i + 1].TokenType != Token.Types.ID) return error;

                    var declaration = NewExpression(i+1, cuerpo, new string[] {",", "in", ";"});


                    if(new_scope.ContainsKey(last_id.TokenName)) new_scope[last_id.TokenName] = last_id;
                    else new_scope.Add(last_id.TokenName, last_id);

                    var a2 = SemiParser(declaration, new_scope, context, plus + i + 2);
                    if(a2.Count > 1) return error;
                    last_id = new Token (cuerpo[i+1].TokenName, a2[0].Value);
                    result[0].Value = last_id.Value;

                }
                if (cuerpo[i].TokenName == "in")
                {

                    if(new_scope.ContainsKey(last_id.TokenName)) new_scope[last_id.TokenName] = last_id;
                    else new_scope.Add(last_id.TokenName, last_id);

                    TokenMask[i + plus + 1] = true;
                    e.RemoveRange(0, i + 1);

                    result = SemiParser(e, new_scope, context, i + plus + 2);
                    break;
                }
            }
            
            if (result.Count == 0) return next_token;
            if (result.Count == 1) return result[0];
            return error;
        }
        if (a.TokenName == "function")
        {
            Token[] expected = { new Token("ID"), new Token("("), new Token("ID"), new Token(")"), new Token("=>") };
            List<Token> cuerpo = NewExpression(b.IndexOf(a) + 1, b, new string[] { ";" });

            for (int i = 0; i < 5; i++)
            {
                var temp = new Token(cuerpo[i].TokenName);
                if(temp.TokenType != expected[i].TokenType) return error;
                if(temp.TokenType != Token.Types.ID && temp.TokenName != expected[i].TokenName) return error;
                TokenMask[plus + i + 1] = true;
            }
            
            string func_name = cuerpo[0].TokenName;
            string arg_name = cuerpo[2].TokenName;
            cuerpo.RemoveRange(0,5);

            foreach (var item in cuerpo)
            {
                if(item.TokenType == Token.Types.ID && item.TokenName != arg_name) return error;
            }

            Program.context.Add(func_name, (arg_name, cuerpo));
             
        }

        return unknown;
    }

    private static List<Token> NewExpression(int i, List<Token> tokens, string[] EndCharacter)
    {
        List<Token> exp = new List<Token>();
        int balance = 0;

        for (int j = i; j < tokens.Count; j++)
        {

            if (EndCharacter.Contains(tokens[j].TokenName) && balance == 0)
            {
                break;
            }

            if (tokens[j].TokenName == "(") balance++;
            if (tokens[j].TokenName == ")") balance--;

            exp.Add(tokens[j]);
        }

        return exp;
    }
    private static Token Parse(Token a, List<Token> b, Dictionary<string, Token> scope, List<string> context, Token prev_token, int plus = 0)
    {
        int i = b.IndexOf(a);

        if (a.TokenName != ";" && i != b.Count - 1)
        {
            Token temp = b[i + 1];
            if (temp.TokenType == Token.Types.oper && TokenMask[b.IndexOf(temp) + plus] != true)
            {
                a = temp; i++;
            }
        }

        TokenMask[i + plus] = true;

        if (a.TokenName == "(")
        {
            var temp = NewExpression(i + 1, b, new string[] { ")" });
            TokenMask[i + temp.Count + 1 + plus] = true;

            var result = SemiParser(temp, scope, context, i + 1 + plus);


            if (result.Count != 1) return error;
            return result[0];
        }

        if(context.Contains(a.TokenName)){

            var argumento = new Token("argumento", Parse(b[i + 1], b, scope, context, prev_token, plus).Value);

            var local_scope = new Dictionary<string, Token>(scope);
            
            if(!local_scope.ContainsKey(Program.context[a.TokenName].Item1)) local_scope.Add(Program.context[a.TokenName].Item1, argumento);
            else local_scope[Program.context[a.TokenName].Item1] = argumento;

            return FullParser(Program.context[a.TokenName].Item2, local_scope);
        }
        
        switch (a.TokenType)
        {
            case Token.Types.str:
                return ParseString(a);
            case Token.Types.number:
                return ParseNumber(a);
            case Token.Types.boolean:
                return ParseBool(a);
            case Token.Types.ID:
                return ParseID(a, scope);
            case Token.Types.oper:
                return ParseOperator(a, b, scope, context, plus, prev_token);
            case Token.Types.keyword:
                return ParseKeyword(a, b, scope, context, plus);
            default:
                return unknown;
        }
    }

    public static void PrimaryParser(List<Token> tokens){
        FullParser(tokens, new Dictionary<string, Token>());
    }
    private static Token FullParser(List<Token> tokens, Dictionary<string, Token> scope)
    {
        TokenMask = new bool[tokens.Count];

        var result = SemiParser(tokens, scope, Program.context.Keys.ToList());

        if(result.Count != 1) return error;
        return result[0];
    }

    private static List<Token> SemiParser(List<Token> tokens, Dictionary<string, Token> scope, List<string> context, int plus = 0)
    {

        Token actual_token;
        List<Token> result = new List<Token>();
        Token prev_token = unknown;


        for (int i = 0; i < tokens.Count; i++)
        {
            actual_token = tokens[i];
            if (TokenMask[i + plus]) continue;

            if (i != tokens.Count - 1 && actual_token.TokenName != ";" && tokens[i + 1].TokenType == Token.Types.oper)
            {
                actual_token = tokens[i + 1];
            }

            if (result.Count != 0)
            {
                prev_token = result.Last();
                if (actual_token.TokenType == Token.Types.oper) result.Remove(result.Last());
            }

            //Meter la condicional de ignorar los unknown;
            Token p = Parse(actual_token, tokens, scope, context, prev_token, plus);

            result.Add(p);
        }

        return result;
    }
}
