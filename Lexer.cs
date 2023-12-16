namespace Hulk;

class Lexer
{

    public static List<Error> error_catcher = new List<Error>();

    //ESTO ES LO QUE SIRVE (mas o menos)
    //Bug con los numeros de coma flotante(los toma como 3 token distintos)
    //bug con los numeros negativos
    //Arreglar los =>
    //a.b no es un token valido
    //{} y [] no son validos tampoco

    //Puedo al final recorrer la lista de tokens y combinar lo que sea combinable
    //Meter los caracteres de punto al berro
    //Los unicos punctuation validos son , y ; XD

    //Operador de concatenacion @ en strings (no es ahi)
    //No se como meter saltos de linea ni tabulaciones en los strings
    private static void Converter(List<char> char_list, List<Token> string_list, bool valid)
    {
        string c = new string(char_list.ToArray());

        if (valid) ValidConverter(c, string_list);
        else InvalidConverter(c);

        char_list.Clear();
    }

    private static void ValidConverter(string s, List<Token> string_list)
    {
        if (!String.IsNullOrEmpty(s)) string_list.Add(new Token(s));
    }

    private static void InvalidConverter(string s)
    {
        var error = new Error("a", "a");

        if (s.First() == '\"')
            error = new Error("Lexical error", "faltan comillas (\") de cerrado en el string " + s);
        else
            error = new Error("Lexical error", s + " no es un token válido");

        error_catcher.Add(error);

    }

    // private static int String_Tokenizer(List<char> char_list, List<string> string_list, string input, int i)
    // {
    //     bool escaper = false;
    //     bool closed = false;
    //     for (int j = i; j < input.Length; j++, i++)
    //     {
    //         if (input[j] == '\\')
    //         {
    //             escaper = true;
    //             continue;
    //         }

    //         if (input[j] == '\"' && !escaper)
    //         {
    //             closed = true;
    //             char_list.Add(input[j]);
    //             Converter(char_list, string_list, true);
    //             break;
    //         }

    //         char_list.Add(input[j]);
    //         escaper = false;
    //     }
    //     return i;
    // }

    public static List<Token> Tokenizer(string input)
    {
        List<char> token_creator = new List<char>();
        List<Token> tokens = new List<Token>();

        if (input.Last() != ';')
        {
            var error = new Error("Lexical error", "El programa debe terminar con un caracter ;");
            error_catcher.Add(error);
        }

        for (int i = 0; i < input.Length; i++)
        {
            if (!char.IsLetterOrDigit(input[i]) && !char.IsPunctuation(input[i]) && !char.IsSymbol(input[i]) && input[i] != ' ' && input[i] != '\n'){
                var error = new Error("Lexical error", input[i] + " no es un token válido");
                error_catcher.Add(error);
                continue;
            }

            if (token_creator.Count == 0)
            {
                if (char.IsLetterOrDigit(input[i]))
                {
                    token_creator.Add(input[i]);

                    if (i == input.Length - 1) Converter(token_creator, tokens, true);
                }
                if (char.IsPunctuation(input[i]) || char.IsSymbol(input[i]))
                {
                    if (input[i] == '\"')
                    {
                        token_creator.Add(input[i]);
                        i++;
                        bool escaper = false;
                        bool closed = false;
                        for (int j = i; j < input.Length; j++, i++)
                        {
                            if (input[j] == '\\')
                            {
                                escaper = true;
                                continue;
                            }

                            if (input[j] == '\"' && !escaper)
                            {
                                closed = true;
                                token_creator.Add(input[j]);
                                Converter(token_creator, tokens, true);
                                break;
                            }

                            token_creator.Add(input[j]);
                            escaper = false;
                        }

                        if (i == input.Length && !closed)
                        {
                            Converter(token_creator, tokens, false);
                            break;
                        }
                    }
                    else if (input[i] == '!' || input[i] == '<' || input[i] == '>' || input[i] == '=') token_creator.Add(input[i]);
                    else
                    {
                        token_creator.Add(input[i]);
                        Converter(token_creator, tokens, true);
                    }
                }

                continue;
            }

            char a = token_creator.Last();
            char b = input[i];

            if (char.IsLetter(a) && char.IsLetterOrDigit(b)) token_creator.Add(input[i]);
            else if (char.IsDigit(a) && char.IsDigit(b)) token_creator.Add(input[i]);
            //Espero que esta linea no me de bateo
            else if(a == '=' && b == '>'){
                token_creator.Add(input[i]);
                Converter(token_creator, tokens, true);
            }
            else if ((a == '!' || a == '<' || a == '>' || a == '=') && b == '='){
                token_creator.Add(input[i]);
                Converter(token_creator, tokens, true);
            }
            else if ((a == '!' || a == '<' || a == '>' || a == '=') && char.IsLetterOrDigit(b)){
                Converter(token_creator, tokens, true);
                token_creator.Add(input[i]);
            }
            else if (char.IsPunctuation(b) || char.IsSymbol(b))
            {
                Converter(token_creator, tokens, true);
                if (input[i] == '\"')
                {
                    token_creator.Add(input[i]);
                    i++;
                    bool escaper = false;
                    bool closed = false;
                    for (int j = i; j < input.Length; j++, i++)
                    {
                        if (input[j] == '\\')
                        {
                            escaper = true;
                            continue;
                        }

                        if (input[j] == '\"' && !escaper)
                        {
                            closed = true;
                            token_creator.Add(input[j]);
                            Converter(token_creator, tokens, true);
                            break;
                        }

                        token_creator.Add(input[j]);
                        escaper = false;
                    }

                    if (i == input.Length && !closed)
                    {
                        Converter(token_creator, tokens, false);
                        break;
                    }
                }
                else if (input[i] == '!' || input[i] == '<' || input[i] == '>' || input[i] == '=') token_creator.Add(input[i]);
                else
                {
                    token_creator.Add(b);
                    Converter(token_creator, tokens, true);
                }
            }
            else if (b == ' ' || b == '\n')
            {
                if (token_creator.Count == 0) continue;
                Converter(token_creator, tokens, true);
            }
            else
            {
                for (int j = i; j < input.Length && input[j] != ' ' && !char.IsPunctuation(input[j]) && !char.IsSymbol(input[j]); j++, i++)
                {
                    token_creator.Add(input[j]);
                }
                --i;
                Converter(token_creator, tokens, false);
            }

            if (i == input.Length - 1 && input[i] != ';' && token_creator.Count != 0)
            {
                Converter(token_creator, tokens, true);

                //    Como meto esto aqui? ↓
                //    var error = new Error("Lexical error", "El programa debe terminar con un caracter ;");
                //    error_catcher.Add(error);
            }

        }

        return tokens;
    }

    public static void ErrorList()
    {
        for (int i = 0; i < error_catcher.Count; i++)
        {
            Error.ShowError(error_catcher[i]);
        }
    }
}