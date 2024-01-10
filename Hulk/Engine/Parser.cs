namespace Hulk;

class Parser
{
    public static List<Error> error_catcher = new List<Error>();
    private static Token unknown = new Token("", null, 2);
    private static Token error = new Token("", null, 1);
    private static bool[] TokenMask = new bool[1];


    private static Token ParseString(Token node)
    {
        string temp = node.Name.Remove(0, 1);
        node.Value = temp.Remove(temp.Length - 1);
        return node;
    }

    private static Token ParseNumber(Token node)
    {
        if (node.Name != "PI" && node.Name != "E") node.Value = double.Parse(node.Name);
        return node;
    }

    private static Token ParseBool(Token node)
    {
        if (node.Name == "true") node.Value = true;
        else if (node.Name == "false") node.Value = false;
        return node;
    }

    private static Token ParseID(Token node, Dictionary<string, Token> scope)
    {
        if (!scope.ContainsKey(node.Name)) return new Token(node.Name);
        else return scope[node.Name];
    }

    //En este metodo evaluo cada uno de los operadores
    private static Token ParseOperator(Token node, List<Token> node_list, Dictionary<string, Token> scope, List<string> context, int plus, Token? prev_token = null, Token? next_token = null)
    {
        //Miembro derecho del operador en cuestion. Si no lo conozco, evaluo para ver que tengo
        Token right;
        if (next_token == null) right = Parse(node_list[node_list.IndexOf(node) + 1], node_list, scope, context, plus);
        else right = next_token;

        //Evaluacion del operador !
        if (node.Name == "!")
        {
            if (right.Type != Token.Types.boolean)
            {
                error_catcher.Add(new Error("Semantic error", $"Operator ! cannot be used with {right.Type} expressions"));
                return error;
            }
            else return new Token(node.Name, !((bool)right.Value!));
        }

        if (node == node_list.First())
        {
            error_catcher.Add(new Error("Syntax error", $"Missing left part of the operator {node.Name}"));
            return error;
        }

        //Miembro izquierdo del operador en cuestion. Si no lo conozco, evaluo para ver que tengo
        Token left;
        if (prev_token == null) left = Parse(node_list[node_list.IndexOf(node) - 1], node_list, scope, context, plus);
        else left = prev_token;

        //Evaluacion del operador = 
        if (node.Name == "=")
        {
            if (left.Type != Token.Types.ID && !scope.ContainsKey(left.Name))
            {
                error_catcher.Add(new Error("Semantic error", $"Variable expected on left member"));
                return error;
            }

            if (right.Type != Token.Types.text && right.Type != Token.Types.numerical && right.Type != Token.Types.boolean)
            {
                error_catcher.Add(new Error("Semantic error", $"Cannot assign {right.Type} expression to {left.Name}"));
                return error;
            }
            else
            {
                if (scope.ContainsKey(left.Name)) left = new Token(right, left.Name);
                else left = right;
            }
            return left;
        }
        //Evaluacion de los operadores | y & 
        if (node.Name == "|" || node.Name == "&")
        {
            if (right.Type != Token.Types.boolean || left.Type != Token.Types.boolean)
            {
                error_catcher.Add(new Error("Semantic error", $"Operator {node.Name} cannot be used with {left.Type} and {right.Type} expressions"));
                return error;
            }
            else
            {
                if (node.Name == "|") return new Token(node.Name, (bool)left.Value! || (bool)right.Value!);
                if (node.Name == "&") return new Token(node.Name, (bool)left.Value! && (bool)right.Value!);
            }
        }
        //Evaluacion de los operadores <, <=, > y >= 
        if (node.Name == "<" || node.Name == "<=" || node.Name == ">" || node.Name == ">=")
        {
            if (right.Type != Token.Types.numerical || left.Type != Token.Types.numerical)
            {
                error_catcher.Add(new Error("Semantic error", $"Operator {node.Name} cannot be used with {left.Type} and {right.Type} expressions"));
                return error;
            }
            else
            {
                if (node.Name == "<") return new Token(node.Name, (double)left.Value! < (double)right.Value!);
                if (node.Name == ">") return new Token(node.Name, (double)left.Value! > (double)right.Value!); ;
                if (node.Name == "<=") return new Token(node.Name, (double)left.Value! <= (double)right.Value!); ;
                if (node.Name == ">=") return new Token(node.Name, (double)left.Value! >= (double)right.Value!); ;
            }
        }
        //Evaluacion de los operadores != y == 
        if (node.Name == "!=" || node.Name == "==")
        {
            if (right.Type != left.Type)
            {
                error_catcher.Add(new Error("Semantic error", $"Operator {node.Name} cannot be used with {left.Type} and {right.Type} expressions"));
                return error;
            }
            else
            {
                if (node.Name == "!=") return new Token(node.Name, !left.Value!.Equals(right.Value));
                if (node.Name == "==") return new Token(node.Name, left.Value!.Equals(right.Value));
            }
        }
        //Evaluacion de los operadores +, -, *, / y ^ 
        if (node.Name == "+" || node.Name == "-" || node.Name == "*" || node.Name == "/" || node.Name == "^")
        {
            if (right.Type != Token.Types.numerical || left.Type != Token.Types.numerical)
            {
                error_catcher.Add(new Error("Semantic error", $"Operator {node.Name} cannot be used with {left.Type} and {right.Type} expressions"));
                return error;
            }
            else
            {
                if (node.Name == "+") return new Token(node.Name, (double)left.Value! + (double)right.Value!);
                if (node.Name == "-") return new Token(node.Name, (double)left.Value! - (double)right.Value!);
                if (node.Name == "*") return new Token(node.Name, (double)left.Value! * (double)right.Value!);
                if (node.Name == "/")
                {
                    if ((double)right.Value! == 0)
                    {
                        error_catcher.Add(new Error("Invalid Operation", $"Cannot divide by zero"));
                        return error;
                    }
                    return new Token(node.Name, (double)left.Value! / (double)right.Value!);
                }
                if (node.Name == "^") return new Token(node.Name, Math.Pow((double)left.Value!, (double)right.Value!));
            }
        }
        //Evaluacion del operador @
        if (node.Name == "@")
        {
            if ((right.Type != Token.Types.text && right.Type != Token.Types.numerical) || (left.Type != Token.Types.text && left.Type != Token.Types.numerical))
            {
                error_catcher.Add(new Error("Semantic error", $"Operator {node.Name} cannot be used with {left.Type} and {right.Type} expressions"));
                return error;
            }

            var res = new Token(node.Name, left.Value!.ToString() + right.Value!.ToString());
            Token.ConvertToString(res);
            return res;
        }

        return unknown;
    }

    //En este metodo evaluo los nodos de tipo keyword
    private static Token ParseKeyword(Token node, List<Token> node_list, Dictionary<string, Token> scope, List<string> context, int plus)
    {
        if (node.Name == "cos" || node.Name == "sin" || node.Name == "sqrt" || node.Name == "exp" || node.Name == "log" || node.Name == "print")
        {

            //Argumentos sin evaluar de cada funcion
            var arguments = GetArgument(NewExpression(node_list.IndexOf(node) + 1, node_list, new string[] { ")", ";" }, new string[] { "(" }, new string[] { ")" }), plus + 2);

            //Validador de cantidad de argumentos
            if (Validate(node.Name, arguments.Count)) return error;

            //Primer (y en la mayoria de casos unico) argumento de la funcion despues de evaluado
            Token func_arg = SemiParser(arguments[0], scope, context, plus + 2);


            if (node.Name == "print")
            {
                if (Validate(node.Name, func_arg.Type, new Token.Types[] { Token.Types.numerical, Token.Types.boolean, Token.Types.text })) return error;

                Console.WriteLine(func_arg.Value);
                return new Token(func_arg, "print");
            }
            if (node.Name == "log")
            {
                var func_arg2 = SemiParser(arguments[1], scope, context, plus + 3 + arguments[0].Count);

                if (func_arg.Type != Token.Types.numerical || func_arg2.Type != Token.Types.numerical)
                {
                    error_catcher.Add(new Error("Semantic error", $"Both numerical arguments required for function {node.Name}"));
                    return error;
                }
                else return new Token("log", Math.Log((double)func_arg2.Value!, (double)func_arg.Value!));
            }
            if (node.Name == "cos" || node.Name == "sin" || node.Name == "sqrt" || node.Name == "exp")
            {
                if (Validate(node.Name, func_arg.Type, new Token.Types[] { Token.Types.numerical })) return error;

                if (node.Name == "cos") return new Token("cos", Math.Cos((double)func_arg.Value!));
                if (node.Name == "sin") return new Token("sin", Math.Sin((double)func_arg.Value!));
                if (node.Name == "sqrt") return new Token("sqrt", Math.Sqrt((double)func_arg.Value!));
                if (node.Name == "exp") return new Token("exp", Math.Pow(double.E, (double)func_arg.Value!));
            }
        }


        //Valor del nodo o expresion inmediatamente despues de la actual
        var next_token = Parse(node_list[node_list.IndexOf(node) + 1], node_list, scope, context, plus);

        if (node.Name == "if")
        {
            if (Validate(node.Name, next_token.Type, new Token.Types[] { Token.Types.boolean })) return error;

            List<Token> cuerpo = new List<Token>();
            for (int i = node_list.IndexOf(node); i < node_list.Count; i++)
            {
                if (!TokenMask[i + plus])
                {
                    cuerpo = NewExpression(i, node_list, new string[] { "" }, new string[] { "(", "if" }, new string[] { ")", "else" });
                    break;
                }
            }

            //cuerpo del if
            var if_body = NewExpression(0, cuerpo, new string[] { "else" }, new string[] { "(", "if" }, new string[] { ")", "else" });

            if (if_body.Count == cuerpo.Count)
            {
                error_catcher.Add(new Error("Syntax error", $"Missing else in if-else expression"));
                return error;
            }

            //cuerpo del else
            var else_body = NewExpression(if_body.Count + 1, cuerpo, new string[] { ";" }, new string[] { "(", "if" }, new string[] { ")", "else" });

            //Esto tiene el defecto de que solo evalua el bloque que va a devolver, o sea que si
            //en el otro bloque hay un error, el programa no lo ve;

            if ((bool)next_token.Value! == true)
            {
                //evaluo el bloque if
                Token parsed_if = SemiParser(if_body, scope, context, node_list.Count - cuerpo.Count + 1 + plus);

                //y marco el bloque else
                for (int i = 0; i < cuerpo.Count; i++)
                {
                    TokenMask[node_list.Count - cuerpo.Count + i + plus] = true;
                }

                return parsed_if;
            }
            else //O viceversa
            {
                TokenMask[node_list.Count - cuerpo.Count + if_body.Count + plus] = true;
                Token parsed_else = SemiParser(else_body, scope, context, (node_list.Count - cuerpo.Count) + if_body.Count + 1 + plus);

                for (int i = 0; i < cuerpo.Count; i++)
                {
                    TokenMask[node_list.Count - cuerpo.Count + i + plus] = true;
                }

                return parsed_else;
            }

        }
        if (node.Name == "let")
        {
            if (node_list[node_list.IndexOf(node) + 1].Type != Token.Types.ID)
            {
                error_catcher.Add(new Error("Semantic error", $"Expected variable name after let keyword"));
                return error;
            }
            if (Validate(node.Name, next_token.Type, new Token.Types[] { Token.Types.numerical, Token.Types.boolean, Token.Types.text })) return error;

            //cuerpo completo de la expresion let-in
            var cuerpo = NewExpression(node_list.IndexOf(node) + 1, node_list, new string[] { ";" }, new string[] { "(" }, new string[] { ")" });

            //cuerpo del bloque in
            List<Token> e = new List<Token>(cuerpo);

            //resultado
            Token result = new Token("let");

            //ultima variable creada y su valor
            Token last_id = new Token(next_token, cuerpo[0].Name);

            //scope que se utilizara dentro del bloque in
            var new_scope = new Dictionary<string, Token>();
            if (scope.Count != 0) new_scope = new Dictionary<string, Token>(scope);


            for (int i = 0; i < cuerpo.Count; i++)
            {
                //declaracion multiple a traves de comas
                if (cuerpo[i].Name == ",")
                {
                    //agrego al scope lo ultimo que tengo
                    if (new_scope.ContainsKey(last_id.Name)) new_scope[last_id.Name] = last_id;
                    else new_scope.Add(last_id.Name, last_id);

                    TokenMask[i + plus + 1] = true;

                    if (cuerpo[i + 1].Type != Token.Types.ID)
                    {
                        error_catcher.Add(new Error("Semantic error", $"Expected variable name after comma"));
                        return error;
                    }

                    //Declaracion y evaluacion de la nueva variable
                    var declaration = NewExpression(i + 1, cuerpo, new string[] { ",", "in", ";" }, new string[] { "(" }, new string[] { ")" });
                    Token eval = SemiParser(declaration, new_scope, context, plus + i + 2);

                    if (eval.Type == Token.Types.error)
                    {
                        error_catcher.Add(new Error("Invalid Operation", $"Cannot use expression as assignation"));
                        return error;
                    }

                    //Si todo sale bien mi nueva ultima variable sera la evaluacion que acabo de hacer, y mi ultimo resultado, su valor
                    last_id = new Token(eval, cuerpo[i + 1].Name);
                    result.Value = last_id.Value;
                }
                if (cuerpo[i].Name == "in")
                {
                    //agrego al scope
                    if (new_scope.ContainsKey(last_id.Name)) new_scope[last_id.Name] = last_id;
                    else new_scope.Add(last_id.Name, last_id);

                    TokenMask[i + plus + 1] = true;

                    e.RemoveRange(0, i + 1);

                    //Y evaluo el cuerpo del in
                    result = SemiParser(e, new_scope, context, i + plus + 2);
                    break;
                }
            }

            if (result.Type == Token.Types.unknown) return next_token;
            return result;
        }
        if (node.Name == "function")
        {
            if (next_token.Type != Token.Types.ID)
            {
                error_catcher.Add(new Error("Semantic error", $"Expected function name after function keyword"));
                return error;
            }

            string func_name = next_token.Name;
            var argumentos = GetArgument(NewExpression(node_list.IndexOf(node) + 2, node_list, new string[] { "=>", ";" }, new string[] { "(" }, new string[] { ")" }), plus + 3);
            List<string> args_name = new List<string>();

            foreach (var item in argumentos)
            {
                if (item.Count != 1)
                {
                    error_catcher.Add(new Error("Semantic error", $"There can be only one element per argument declaration"));
                    return error;
                }
                foreach (var item2 in item)
                {
                    if (item2.Type != Token.Types.ID)
                    {
                        error_catcher.Add(new Error("Semantic error", $"Only variables can be used as arguments in function declaration"));
                        return error;
                    }
                    args_name.Add(item2.Name);
                }
            }

            //marcador de donde terminan los argumentos de la funcion y donde comienza su cuerpo
            int k = 2 * (argumentos.Count + 2) - 1;

            if (node_list[k].Name != "=>")
            {
                error_catcher.Add(new Error("Syntax error", $"Expected operator => after arguments"));
                return error;
            }

            for (int i = 0; i < k; i++) { TokenMask[i + plus] = true; }

            List<Token> cuerpo = NewExpression(k + 1, node_list, new string[] { }, new string[] { "(" }, new string[] { ")" });

            foreach (var item in cuerpo)
            {
                if (item.Type == Token.Types.ID && !args_name.Contains(item.Name))
                {
                    error_catcher.Add(new Error("Semantic error", $"Function {func_name} does not receive {item.Name} as argument"));
                    return error;
                }
            }

            Program.context.Add(func_name, (args_name, cuerpo));
        }

        return unknown;
    }
    //Metodo para crear nuevas expresiones
    private static List<Token> NewExpression(int i, List<Token> tokens, string[] EndCharacter, string[] BalancePlus, string[] BalanceMinus)
    {
        //Debuggear estas dos cosas: Balances negativos y balances != 0 al final del ciclo
        //Ojo el if basico rompe esto
        List<Token> exp = new List<Token>();
        int balance = 0;


        for (int j = i; j < tokens.Count; j++)
        {

            if (EndCharacter.Contains(tokens[j].Name) && balance == 0)
            {
                break;
            }


            if (BalancePlus.Contains(tokens[j].Name)) balance++;
            if (BalanceMinus.Contains(tokens[j].Name)) balance--;

            exp.Add(tokens[j]);
        }

        return exp;
    }
    //Metodo que devuelve los argumentos sin evaluar de una funcion
    private static List<List<Token>> GetArgument(List<Token> arguments, int plus)
    {
        int balance = 0;

        for (int i = 0; i < arguments.Count; i++)
        {
            TokenMask[i + plus] = false;
        }

        var w = new List<List<Token>>();
        var aux = new List<Token>();

        for (int i = 1; i < arguments.Count - 1; i++)
        {
            if (arguments[i].Name == "(") balance++;
            if (arguments[i].Name == ")") balance--;

            if ((arguments[i].Name == "," || i == arguments.Count - 1) && balance == 0)
            {
                var temp = aux.ToList();
                w.Add(temp);
                aux.Clear();
                continue;
            }
            aux.Add(arguments[i]);
        }
        w.Add(aux);

        return w;
    }
    //Metodo que dice si la cantidad de argumentos es valido 
    private static bool Validate(string name, int arguments, int expected = 1)
    {
        if (name == "log") expected = 2;
        if (arguments != expected)
        {
            error_catcher.Add(new Error("Semantic error", $"Function {name} takes {expected} argument(s), but {arguments} were given"));
            return true;
        }
        return false;
    }
    //Metodo que dice si el tipo de argumentos es valido
    private static bool Validate(string name, Token.Types type, Token.Types[] expected, Token.Types? type2 = null)
    {
        if (!expected.Contains(type))
        {
            error_catcher.Add(new Error("Semantic error", $"Function {name} cannot take {type} elements as arguments"));
            return true;
        }
        return false;
    }
    //Metodo que evalua si los parentesis y los if-else estan balanceados
    private static bool CheckBalance(List<Token> tokens)
    {
        int balance1 = 0;
        int balance2 = 0;

        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].Name == "(") balance1++;
            if (tokens[i].Name == ")") balance1--;

            if (tokens[i].Name == "if") balance2++;
            if (tokens[i].Name == "else") balance2--;

            if (balance1 < 0)
            {
                error_catcher.Add(new Error("Syntax error", $"Missing open parentheses before closed parentheses"));
                return true;
            }
            if (balance2 < 0)
            {
                error_catcher.Add(new Error("Syntax error", $"Missing if expression before else expression"));
                return true;
            }
        }

        if (balance1 != 0)
        {
            error_catcher.Add(new Error("Syntax error", $"Detected open parentheses without its correspondant closed pair"));
            return true;
        }
        if (balance2 != 0)
        {
            error_catcher.Add(new Error("Syntax error", $"Detected if expression without its correspondant else expression"));
            return true;
        }

        return false;
    }

    private static Token Parse(Token node, List<Token> node_list, Dictionary<string, Token> scope, List<string> context, int plus = 0, Token? prev_token = null)
    {
        int i = node_list.IndexOf(node);

        //Si lo siguiente que tengo es un operador, me muevo hacia el
        if (node.Name != ";" && i != node_list.Count - 1)
        {
            Token temp = node_list[i + 1];
            if (temp.Type == Token.Types.oper && TokenMask[node_list.IndexOf(temp) + plus] != true) { node = temp; i++; }
        }

        TokenMask[i + plus] = true;

        //Si lo que tengo que evaluar es un (, creo y evaluo la expresion completa entre parentesis
        if (node.Name == "(")
        {
            var temp = NewExpression(i + 1, node_list, new string[] { ")" }, new string[] { "(" }, new string[] { ")" });
            TokenMask[i + temp.Count + 1 + plus] = true;

            return SemiParser(temp, scope, context, i + 1 + plus);
        }

        //Si tengo que evaluar una funcion definida propia, creo un nuevo scope local con el vaor de cada uno de los argumentos
        //Y evaluo el cuerpo de la funcion en base a ese nuevo scope.  
        if (context.Contains(node.Name))
        {
            //Valor de los argumentos sin evaluar
            var arguments = GetArgument(NewExpression(node_list.IndexOf(node) + 1, node_list, new string[] { "=>", ";" }, new string[] { "(" }, new string[] { ")" }), plus + 2);
            //Valor ya evaluado de los argumentos
            List<Token> true_args = new List<Token>();
            //Scope local
            var local_scope = new Dictionary<string, Token>(scope);

            //Evaluo y asigno cada uno de los argumentos
            for (int j = 0, acumulate = 1; j < arguments.Count; j++)
            {
                TokenMask[plus + acumulate] = true;
                true_args.Add(SemiParser(arguments[j], scope, context, plus + acumulate + 1));
                acumulate = acumulate + arguments[j].Count + 1;
            }

            if (Validate(node.Name, true_args.Count, Program.context[node.Name].Item1.Count)) return error;

            //Aqui lo que hago es ver si alguno de mis argumentos ya tiene valor en mi anterior scope
            for (int j = 0; j < true_args.Count; j++)
            {
                if (!local_scope.ContainsKey(Program.context[node.Name].Item1[j]))
                    local_scope.Add(Program.context[node.Name].Item1[j], true_args[j]);
            }

            //Pila que guarda el estado de mis mascaras de nodo
            Stack<bool[]> mask_stat = new Stack<bool[]>();

            mask_stat.Push(TokenMask);
            var eval = FullParser(Program.context[node.Name].Item2, local_scope);
            TokenMask = mask_stat.Pop();

            return eval;
        }

        switch (node.Type)
        {
            case Token.Types.text:
                return ParseString(node);
            case Token.Types.numerical:
                return ParseNumber(node);
            case Token.Types.boolean:
                return ParseBool(node);
            case Token.Types.ID:
                return ParseID(node, scope);
            case Token.Types.oper:
                return ParseOperator(node, node_list, scope, context, plus, prev_token);
            case Token.Types.keyword:
                return ParseKeyword(node, node_list, scope, context, plus);
            default:
                return unknown;
        }
    }

    public static Token FullParser(List<Token> tokens, Dictionary<string, Token> scope)
    {
        //Array que indica que nodos he parseado ya y cuales no
        TokenMask = new bool[tokens.Count];

        if (CheckBalance(tokens)) return error;

        return SemiParser(tokens, scope, Program.context.Keys.ToList());
    }

    private static Token SemiParser(List<Token> tokens, Dictionary<string, Token> scope, List<string> context, int plus = 0)
    {
        //Evaluador de expresiones aritmeticas
        if (tokens.Count > 1)
        {
            if (tokens[0].Name != "let" && tokens[0].Name != "print" && tokens[0].Name != "function" && tokens[0].Name != "if")
            {
                int max_priority = int.MaxValue;
                //Indice del oprador de maxima prioridad
                int index = 0;
                int balance = 0;
                for (int i = 0; i < tokens.Count; i++)
                {
                    if (tokens[i].Name == "(") balance++;
                    if (tokens[i].Name == ")") balance--;
                    if (balance > 0) continue;
                    if (balance < 0)
                    {
                        error_catcher.Add(new Error("Syntax error", $"Missing open parentheses before closed parentheses"));
                        return error;
                    }
                    if (tokens[i].Priority <= max_priority && tokens[i].Priority != 0) { max_priority = tokens[i].Priority; index = i; }
                }

                Token[] a1 = new Token[index];
                tokens.CopyTo(0, a1, 0, index);

                //Parte derecha del operador
                var left_exp = a1.ToList();

                //Parte izquierda del operador
                List<Token> right_exp;
                if (tokens[index].Name == "(") right_exp = NewExpression(index, tokens, new string[] { ";" }, new string[] { }, new string[] { });
                right_exp = NewExpression(index + 1, tokens, new string[] { ";" }, new string[] { }, new string[] { });

                //Evaluacion
                if (tokens[index].Type == Token.Types.oper)
                {
                    Token left = SemiParser(left_exp, scope, context, plus);
                    Token right = SemiParser(right_exp, scope, context, left_exp.Count + plus + 1);

                    TokenMask[index + plus] = true;

                    return ParseOperator(tokens[index], tokens, scope, context, plus, left, right);
                }
                else return Parse(tokens[index], tokens, scope, context, plus);
            }
        }


        Token actual_token;
        List<Token> result = new List<Token>();
        Token? prev_token = null;


        for (int i = 0; i < tokens.Count; i++)
        {
            actual_token = tokens[i];
            if (TokenMask[i + plus]) continue;

            if (i != tokens.Count - 1 && actual_token.Name != ";" && tokens[i + 1].Type == Token.Types.oper)
            {
                actual_token = tokens[i + 1];
            }



            if (result.Count != 0)
            {
                prev_token = result.Last();
                if (actual_token.Type == Token.Types.oper) result.Remove(result.Last());
            }


            Token r = Parse(actual_token, tokens, scope, context, plus, prev_token);

            if (r.Type == Token.Types.unknown) continue;

            result.Add(r);
        }

        if (result.Count == 0) return unknown;
        if (result.Count > 1)
        {
            error_catcher.Add(new Error("Syntax error", $"Given expression parses in more than one way"));
            return error;
        }
        return result[0];
    }
}
