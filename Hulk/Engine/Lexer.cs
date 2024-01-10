namespace Hulk;

class Lexer
{
    public static List<Error> error_catcher = new List<Error>();
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

    public static List<Token> Tokenizer(string input)
    {
        List<char> token_creator = new List<char>();
        List<Token> tokens = new List<Token>();

        if (input.Last() != ';') error_catcher.Add(new Error("Lexical error", "El programa debe terminar con un caracter ;"));

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

            if (i == input.Length - 1 && input[i] != ';' && token_creator.Count != 0) Converter(token_creator, tokens, true);
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