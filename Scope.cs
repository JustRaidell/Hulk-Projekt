namespace Hulk;

class Scope
{
    public Dictionary<string, Token.Types> scope;


    public Scope(){
        scope = new Dictionary<string, Token.Types>();
    }
    public Scope(Dictionary<string, Token.Types> a){
        scope = new Dictionary<string, Token.Types>(a);
    }

    public void A(){
        
    }


}