using FADE;

public partial class Generic<T> : FADEObject
{
    private int _field;
}

// Generic again but different generic parameters
public partial class {|GD0003:Generic|}<T, R> : FADEObject
{
    private int _field;
}

// Generic again but without generic parameters
public partial class {|GD0003:Generic|} : FADEObject
{
    private int _field;
}
