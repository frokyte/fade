using System;

namespace FADE.SourceGenerators.Sample
{
    public partial class AllWriteOnly : FADEObject
    {
        private bool _writeOnlyBackingField = false;
        public bool WriteOnlyProperty { set => _writeOnlyBackingField = value; }
    }
}
