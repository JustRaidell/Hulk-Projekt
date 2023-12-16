namespace Hulk;

class Error{
    public string e_type { get; private set;}
    public string e_description { get; private set;}

    public Error(string error_type, string error_description){
        e_type = error_type;
        e_description = error_description; 
    }

    public static void ShowError (Error e){
        Console.WriteLine(e.e_type + ": " + e.e_description);
    }
}

