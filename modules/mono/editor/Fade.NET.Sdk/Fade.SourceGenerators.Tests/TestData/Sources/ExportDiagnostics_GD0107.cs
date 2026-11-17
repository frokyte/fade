using FADE;
using FADE.Collections;

public partial class ExportDiagnostics_GD0107_OK : Node
{
    [Export]
    public Node NodeField;

    [Export]
    public Node[] SystemArrayOfNodesField;

    [Export]
    public Array<Node> FADEArrayOfNodesField;

    [Export]
    public Dictionary<Node, string> FADEDictionaryWithNodeAsKeyField;

    [Export]
    public Dictionary<string, Node> FADEDictionaryWithNodeAsValueField;

    [Export]
    public Node NodeProperty { get; set; }

    [Export]
    public Node[] SystemArrayOfNodesProperty { get; set; }

    [Export]
    public Array<Node> FADEArrayOfNodesProperty { get; set; }

    [Export]
    public Dictionary<Node, string> FADEDictionaryWithNodeAsKeyProperty { get; set; }

    [Export]
    public Dictionary<string, Node> FADEDictionaryWithNodeAsValueProperty { get; set; }
}

public partial class ExportDiagnostics_GD0107_KO : Resource
{
    [Export]
    public Node {|GD0107:NodeField|};

    [Export]
    public Node[] {|GD0107:SystemArrayOfNodesField|};

    [Export]
    public Array<Node> {|GD0107:FADEArrayOfNodesField|};

    [Export]
    public Dictionary<Node, string> {|GD0107:FADEDictionaryWithNodeAsKeyField|};

    [Export]
    public Dictionary<string, Node> {|GD0107:FADEDictionaryWithNodeAsValueField|};

    [Export]
    public Node {|GD0107:NodeProperty|} { get; set; }

    [Export]
    public Node[] {|GD0107:SystemArrayOfNodesProperty|} { get; set; }

    [Export]
    public Array<Node> {|GD0107:FADEArrayOfNodesProperty|} { get; set; }

    [Export]
    public Dictionary<Node, string> {|GD0107:FADEDictionaryWithNodeAsKeyProperty|} { get; set; }

    [Export]
    public Dictionary<string, Node> {|GD0107:FADEDictionaryWithNodeAsValueProperty|} { get; set; }
}
