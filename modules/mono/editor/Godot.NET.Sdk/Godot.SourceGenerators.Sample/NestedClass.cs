using System;

namespace FADE.SourceGenerators.Sample;

public partial class NestedClass : FADEObject
{
    public partial class NestedClass2 : FADEObject
    {
        public partial class NestedClass3 : FADEObject
        {
            [Signal]
            public delegate void MySignalEventHandler(string str, int num);

            [Export] private String _fieldString = "foo";
            [Export] private String PropertyString { get; set; } = "foo";

            private void Method()
            {
            }
        }
    }
}
