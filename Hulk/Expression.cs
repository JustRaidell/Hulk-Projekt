namespace Hulk;

abstract class Expression
{
    public enum ExpressionType {numerical, boolean, text, error, unknown};
    public ExpressionType Type;

    //public virtual int Value {get; private set;}

    public abstract ExpressionType Parse();
}

abstract class BinaryExpression : Expression
{
    public Expression Left {get; private set;}
    public Expression Right {get; private set;}
    public override abstract ExpressionType Parse();

    
}

class BoolExpresion : BinaryExpression{
    public override ExpressionType Parse()
    {
        if(Left.Parse() != ExpressionType.boolean || Right.Parse() != ExpressionType.boolean){
            return ExpressionType.error;
        }
        return ExpressionType.boolean;
    }
}

class AtomExpression : Expression
{
    public override ExpressionType Parse(){
        return this.Type;
    }
}