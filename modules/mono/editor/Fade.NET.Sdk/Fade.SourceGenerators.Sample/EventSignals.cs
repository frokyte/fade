namespace FADE.SourceGenerators.Sample;

public partial class EventSignals : FADEObject
{
    [Signal]
    public delegate void MySignalEventHandler(string str, int num);
}
