using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace FADE.SourceGenerators
{
    public readonly struct FADEMethodData
    {
        public FADEMethodData(IMethodSymbol method, ImmutableArray<MarshalType> paramTypes,
            ImmutableArray<ITypeSymbol> paramTypeSymbols, (MarshalType MarshalType, ITypeSymbol TypeSymbol)? retType)
        {
            Method = method;
            ParamTypes = paramTypes;
            ParamTypeSymbols = paramTypeSymbols;
            RetType = retType;
        }

        public IMethodSymbol Method { get; }
        public ImmutableArray<MarshalType> ParamTypes { get; }
        public ImmutableArray<ITypeSymbol> ParamTypeSymbols { get; }
        public (MarshalType MarshalType, ITypeSymbol TypeSymbol)? RetType { get; }
    }

    public readonly struct FADESignalDelegateData
    {
        public FADESignalDelegateData(string name, INamedTypeSymbol delegateSymbol, FADEMethodData invokeMethodData)
        {
            Name = name;
            DelegateSymbol = delegateSymbol;
            InvokeMethodData = invokeMethodData;
        }

        public string Name { get; }
        public INamedTypeSymbol DelegateSymbol { get; }
        public FADEMethodData InvokeMethodData { get; }
    }

    public readonly struct FADEPropertyData
    {
        public FADEPropertyData(IPropertySymbol propertySymbol, MarshalType type)
        {
            PropertySymbol = propertySymbol;
            Type = type;
        }

        public IPropertySymbol PropertySymbol { get; }
        public MarshalType Type { get; }

        public bool IsReadOnly => PropertySymbol.IsReadOnly ||
                                  PropertySymbol.SetMethodOrBaseSetMethod() is { IsInitOnly: true };
        public bool IsWriteOnly => PropertySymbol.IsWriteOnly;
    }

    public readonly struct FADEFieldData
    {
        public FADEFieldData(IFieldSymbol fieldSymbol, MarshalType type)
        {
            FieldSymbol = fieldSymbol;
            Type = type;
        }

        public IFieldSymbol FieldSymbol { get; }
        public MarshalType Type { get; }

        public bool IsReadOnly => FieldSymbol.IsReadOnly;
    }

    public struct FADEPropertyOrFieldData
    {
        public FADEPropertyOrFieldData(ISymbol symbol, MarshalType type)
        {
            Symbol = symbol;
            Type = type;
        }

        public FADEPropertyOrFieldData(FADEPropertyData propertyData)
            : this(propertyData.PropertySymbol, propertyData.Type)
        {
        }

        public FADEPropertyOrFieldData(FADEFieldData fieldData)
            : this(fieldData.FieldSymbol, fieldData.Type)
        {
        }

        public ISymbol Symbol { get; }
        public MarshalType Type { get; }
    }
}
