using FADE;

// This works because it inherits from FADEObject.
[GlobalClass]
public partial class CustomGlobalClass1 : FADEObject
{

}

// This works because it inherits from an object that inherits from FADEObject
[GlobalClass]
public partial class CustomGlobalClass2 : Node
{

}

// This raises a GD0401 diagnostic error: global classes must inherit from FADEObject
[GlobalClass]
public partial class {|GD0401:CustomGlobalClass3|}
{

}
