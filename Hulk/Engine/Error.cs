namespace Hulk;

class Error{
    private string Type { get; set;}
    private string Description { get; set;}

    public Error(string type, string description){
        Type = type;
        Description = description; 
    }

    public static void ShowError (Error error){
        Console.WriteLine(error.Type + ": " + error.Description);
    }
}

