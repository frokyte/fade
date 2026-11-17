partial class ExportedProperties
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
#if TOOLS
    /// <summary>
    /// Get the default values for all properties declared in this class.
    /// This method is used by FADE to determine the value that will be
    /// used by the inspector when resetting properties.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.Dictionary<global::FADE.StringName, global::FADE.Variant> GetFADEPropertyDefaultValues()
    {
        var values = new global::System.Collections.Generic.Dictionary<global::FADE.StringName, global::FADE.Variant>(71);
        string __NotGenerateComplexLamdaProperty_default_value = default;
        values.Add(PropertyName.@NotGenerateComplexLamdaProperty, global::FADE.Variant.From<string>(__NotGenerateComplexLamdaProperty_default_value));
        string __NotGenerateLamdaNoFieldProperty_default_value = default;
        values.Add(PropertyName.@NotGenerateLamdaNoFieldProperty, global::FADE.Variant.From<string>(__NotGenerateLamdaNoFieldProperty_default_value));
        string __NotGenerateComplexReturnProperty_default_value = default;
        values.Add(PropertyName.@NotGenerateComplexReturnProperty, global::FADE.Variant.From<string>(__NotGenerateComplexReturnProperty_default_value));
        string __NotGenerateReturnsProperty_default_value = default;
        values.Add(PropertyName.@NotGenerateReturnsProperty, global::FADE.Variant.From<string>(__NotGenerateReturnsProperty_default_value));
        string __FullPropertyString_default_value = "FullPropertyString";
        values.Add(PropertyName.@FullPropertyString, global::FADE.Variant.From<string>(__FullPropertyString_default_value));
        string __FullPropertyString_Complex_default_value = new string("FullPropertyString_Complex")   + global::System.Convert.ToInt32("1");
        values.Add(PropertyName.@FullPropertyString_Complex, global::FADE.Variant.From<string>(__FullPropertyString_Complex_default_value));
        float __FullPropertyStaticImport_default_value = global::FADE.Mathf.Pi;
        values.Add(PropertyName.@FullPropertyStaticImport, global::FADE.Variant.From<float>(__FullPropertyStaticImport_default_value));
        string __LamdaPropertyString_default_value = "LamdaPropertyString";
        values.Add(PropertyName.@LamdaPropertyString, global::FADE.Variant.From<string>(__LamdaPropertyString_default_value));
        float __LambdaPropertyStaticImport_default_value = global::FADE.Mathf.Tau;
        values.Add(PropertyName.@LambdaPropertyStaticImport, global::FADE.Variant.From<float>(__LambdaPropertyStaticImport_default_value));
        string __PrimaryCtorParameter_default_value = default;
        values.Add(PropertyName.@PrimaryCtorParameter, global::FADE.Variant.From<string>(__PrimaryCtorParameter_default_value));
        float __ConstantMath_default_value = 2  * global::FADE.Mathf.Pi;
        values.Add(PropertyName.@ConstantMath, global::FADE.Variant.From<float>(__ConstantMath_default_value));
        float __ConstantMathStaticImport_default_value = global::FADE.Mathf.RadToDeg(2  * global::FADE.Mathf.Pi);
        values.Add(PropertyName.@ConstantMathStaticImport, global::FADE.Variant.From<float>(__ConstantMathStaticImport_default_value));
        string __StaticStringAddition_default_value = string.Empty   + string.Empty;
        values.Add(PropertyName.@StaticStringAddition, global::FADE.Variant.From<string>(__StaticStringAddition_default_value));
        bool __PropertyBoolean_default_value = true;
        values.Add(PropertyName.@PropertyBoolean, global::FADE.Variant.From<bool>(__PropertyBoolean_default_value));
        char __PropertyChar_default_value = 'f';
        values.Add(PropertyName.@PropertyChar, global::FADE.Variant.From<char>(__PropertyChar_default_value));
        sbyte __PropertySByte_default_value = 10;
        values.Add(PropertyName.@PropertySByte, global::FADE.Variant.From<sbyte>(__PropertySByte_default_value));
        short __PropertyInt16_default_value = 10;
        values.Add(PropertyName.@PropertyInt16, global::FADE.Variant.From<short>(__PropertyInt16_default_value));
        int __PropertyInt32_default_value = 10;
        values.Add(PropertyName.@PropertyInt32, global::FADE.Variant.From<int>(__PropertyInt32_default_value));
        long __PropertyInt64_default_value = -10_000;
        values.Add(PropertyName.@PropertyInt64, global::FADE.Variant.From<long>(__PropertyInt64_default_value));
        byte __PropertyByte_default_value = 10;
        values.Add(PropertyName.@PropertyByte, global::FADE.Variant.From<byte>(__PropertyByte_default_value));
        ushort __PropertyUInt16_default_value = 10;
        values.Add(PropertyName.@PropertyUInt16, global::FADE.Variant.From<ushort>(__PropertyUInt16_default_value));
        uint __PropertyUInt32_default_value = 10;
        values.Add(PropertyName.@PropertyUInt32, global::FADE.Variant.From<uint>(__PropertyUInt32_default_value));
        ulong __PropertyUInt64_default_value = 10;
        values.Add(PropertyName.@PropertyUInt64, global::FADE.Variant.From<ulong>(__PropertyUInt64_default_value));
        float __PropertySingle_default_value = 10;
        values.Add(PropertyName.@PropertySingle, global::FADE.Variant.From<float>(__PropertySingle_default_value));
        double __PropertyDouble_default_value = 10;
        values.Add(PropertyName.@PropertyDouble, global::FADE.Variant.From<double>(__PropertyDouble_default_value));
        string __PropertyString_default_value = "foo";
        values.Add(PropertyName.@PropertyString, global::FADE.Variant.From<string>(__PropertyString_default_value));
        global::FADE.Vector2 __PropertyVector2_default_value = new(10f, 10f);
        values.Add(PropertyName.@PropertyVector2, global::FADE.Variant.From<global::FADE.Vector2>(__PropertyVector2_default_value));
        global::FADE.Vector2I __PropertyVector2I_default_value = global::FADE.Vector2I.Up;
        values.Add(PropertyName.@PropertyVector2I, global::FADE.Variant.From<global::FADE.Vector2I>(__PropertyVector2I_default_value));
        global::FADE.Rect2 __PropertyRect2_default_value = new(new global::FADE.Vector2(10f, 10f), new global::FADE.Vector2(10f, 10f));
        values.Add(PropertyName.@PropertyRect2, global::FADE.Variant.From<global::FADE.Rect2>(__PropertyRect2_default_value));
        global::FADE.Rect2I __PropertyRect2I_default_value = new(new global::FADE.Vector2I(10, 10), new global::FADE.Vector2I(10, 10));
        values.Add(PropertyName.@PropertyRect2I, global::FADE.Variant.From<global::FADE.Rect2I>(__PropertyRect2I_default_value));
        global::FADE.Transform2D __PropertyTransform2D_default_value = global::FADE.Transform2D.Identity;
        values.Add(PropertyName.@PropertyTransform2D, global::FADE.Variant.From<global::FADE.Transform2D>(__PropertyTransform2D_default_value));
        global::FADE.Vector3 __PropertyVector3_default_value = new(10f, 10f, 10f);
        values.Add(PropertyName.@PropertyVector3, global::FADE.Variant.From<global::FADE.Vector3>(__PropertyVector3_default_value));
        global::FADE.Vector3I __PropertyVector3I_default_value = global::FADE.Vector3I.Back;
        values.Add(PropertyName.@PropertyVector3I, global::FADE.Variant.From<global::FADE.Vector3I>(__PropertyVector3I_default_value));
        global::FADE.Basis __PropertyBasis_default_value = new global::FADE.Basis(global::FADE.Quaternion.Identity);
        values.Add(PropertyName.@PropertyBasis, global::FADE.Variant.From<global::FADE.Basis>(__PropertyBasis_default_value));
        global::FADE.Quaternion __PropertyQuaternion_default_value = new global::FADE.Quaternion(global::FADE.Basis.Identity);
        values.Add(PropertyName.@PropertyQuaternion, global::FADE.Variant.From<global::FADE.Quaternion>(__PropertyQuaternion_default_value));
        global::FADE.Transform3D __PropertyTransform3D_default_value = global::FADE.Transform3D.Identity;
        values.Add(PropertyName.@PropertyTransform3D, global::FADE.Variant.From<global::FADE.Transform3D>(__PropertyTransform3D_default_value));
        global::FADE.Vector4 __PropertyVector4_default_value = new(10f, 10f, 10f, 10f);
        values.Add(PropertyName.@PropertyVector4, global::FADE.Variant.From<global::FADE.Vector4>(__PropertyVector4_default_value));
        global::FADE.Vector4I __PropertyVector4I_default_value = global::FADE.Vector4I.One;
        values.Add(PropertyName.@PropertyVector4I, global::FADE.Variant.From<global::FADE.Vector4I>(__PropertyVector4I_default_value));
        global::FADE.Projection __PropertyProjection_default_value = global::FADE.Projection.Identity;
        values.Add(PropertyName.@PropertyProjection, global::FADE.Variant.From<global::FADE.Projection>(__PropertyProjection_default_value));
        global::FADE.Aabb __PropertyAabb_default_value = new global::FADE.Aabb(10f, 10f, 10f, new global::FADE.Vector3(1f, 1f, 1f));
        values.Add(PropertyName.@PropertyAabb, global::FADE.Variant.From<global::FADE.Aabb>(__PropertyAabb_default_value));
        global::FADE.Color __PropertyColor_default_value = global::FADE.Colors.Aquamarine;
        values.Add(PropertyName.@PropertyColor, global::FADE.Variant.From<global::FADE.Color>(__PropertyColor_default_value));
        global::FADE.Plane __PropertyPlane_default_value = global::FADE.Plane.PlaneXZ;
        values.Add(PropertyName.@PropertyPlane, global::FADE.Variant.From<global::FADE.Plane>(__PropertyPlane_default_value));
        global::FADE.Callable __PropertyCallable_default_value = new global::FADE.Callable(global::FADE.Engine.GetMainLoop(), "_process");
        values.Add(PropertyName.@PropertyCallable, global::FADE.Variant.From<global::FADE.Callable>(__PropertyCallable_default_value));
        global::FADE.Signal __PropertySignal_default_value = new global::FADE.Signal(global::FADE.Engine.GetMainLoop(), "Propertylist_changed");
        values.Add(PropertyName.@PropertySignal, global::FADE.Variant.From<global::FADE.Signal>(__PropertySignal_default_value));
        global::ExportedProperties.MyEnum __PropertyEnum_default_value = global::ExportedProperties.MyEnum.C;
        values.Add(PropertyName.@PropertyEnum, global::FADE.Variant.From<global::ExportedProperties.MyEnum>(__PropertyEnum_default_value));
        global::ExportedProperties.MyFlagsEnum __PropertyFlagsEnum_default_value = global::ExportedProperties.MyFlagsEnum.C;
        values.Add(PropertyName.@PropertyFlagsEnum, global::FADE.Variant.From<global::ExportedProperties.MyFlagsEnum>(__PropertyFlagsEnum_default_value));
        byte[] __PropertyByteArray_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@PropertyByteArray, global::FADE.Variant.From<byte[]>(__PropertyByteArray_default_value));
        int[] __PropertyInt32Array_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@PropertyInt32Array, global::FADE.Variant.From<int[]>(__PropertyInt32Array_default_value));
        long[] __PropertyInt64Array_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@PropertyInt64Array, global::FADE.Variant.From<long[]>(__PropertyInt64Array_default_value));
        float[] __PropertySingleArray_default_value = { 0f, 1f, 2f, 3f, 4f, 5f, 6f  };
        values.Add(PropertyName.@PropertySingleArray, global::FADE.Variant.From<float[]>(__PropertySingleArray_default_value));
        double[] __PropertyDoubleArray_default_value = { 0d, 1d, 2d, 3d, 4d, 5d, 6d  };
        values.Add(PropertyName.@PropertyDoubleArray, global::FADE.Variant.From<double[]>(__PropertyDoubleArray_default_value));
        string[] __PropertyStringArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@PropertyStringArray, global::FADE.Variant.From<string[]>(__PropertyStringArray_default_value));
        string[] __PropertyStringArrayEnum_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@PropertyStringArrayEnum, global::FADE.Variant.From<string[]>(__PropertyStringArrayEnum_default_value));
        global::FADE.Vector2[] __PropertyVector2Array_default_value = { global::FADE.Vector2.Up, global::FADE.Vector2.Down, global::FADE.Vector2.Left, global::FADE.Vector2.Right   };
        values.Add(PropertyName.@PropertyVector2Array, global::FADE.Variant.From<global::FADE.Vector2[]>(__PropertyVector2Array_default_value));
        global::FADE.Vector3[] __PropertyVector3Array_default_value = { global::FADE.Vector3.Up, global::FADE.Vector3.Down, global::FADE.Vector3.Left, global::FADE.Vector3.Right   };
        values.Add(PropertyName.@PropertyVector3Array, global::FADE.Variant.From<global::FADE.Vector3[]>(__PropertyVector3Array_default_value));
        global::FADE.Color[] __PropertyColorArray_default_value = { global::FADE.Colors.Aqua, global::FADE.Colors.Aquamarine, global::FADE.Colors.Azure, global::FADE.Colors.Beige   };
        values.Add(PropertyName.@PropertyColorArray, global::FADE.Variant.From<global::FADE.Color[]>(__PropertyColorArray_default_value));
        global::FADE.FADEObject[] __PropertyFADEObjectOrDerivedArray_default_value = { null  };
        values.Add(PropertyName.@PropertyFADEObjectOrDerivedArray, global::FADE.Variant.CreateFrom(__PropertyFADEObjectOrDerivedArray_default_value));
        global::FADE.StringName[] __field_StringNameArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@field_StringNameArray, global::FADE.Variant.From<global::FADE.StringName[]>(__field_StringNameArray_default_value));
        global::FADE.NodePath[] __field_NodePathArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@field_NodePathArray, global::FADE.Variant.From<global::FADE.NodePath[]>(__field_NodePathArray_default_value));
        global::FADE.Rid[] __field_RidArray_default_value = { default, default, default  };
        values.Add(PropertyName.@field_RidArray, global::FADE.Variant.From<global::FADE.Rid[]>(__field_RidArray_default_value));
        global::FADE.Variant __PropertyVariant_default_value = "foo";
        values.Add(PropertyName.@PropertyVariant, global::FADE.Variant.From<global::FADE.Variant>(__PropertyVariant_default_value));
        global::FADE.FADEObject __PropertyFADEObjectOrDerived_default_value = default;
        values.Add(PropertyName.@PropertyFADEObjectOrDerived, global::FADE.Variant.From<global::FADE.FADEObject>(__PropertyFADEObjectOrDerived_default_value));
        global::FADE.Texture __PropertyFADEResourceTexture_default_value = default;
        values.Add(PropertyName.@PropertyFADEResourceTexture, global::FADE.Variant.From<global::FADE.Texture>(__PropertyFADEResourceTexture_default_value));
        global::FADE.Texture __PropertyFADEResourceTextureWithInitializer_default_value = new()  { ResourceName  = ""   };
        values.Add(PropertyName.@PropertyFADEResourceTextureWithInitializer, global::FADE.Variant.From<global::FADE.Texture>(__PropertyFADEResourceTextureWithInitializer_default_value));
        global::FADE.StringName __PropertyStringName_default_value = new global::FADE.StringName("foo");
        values.Add(PropertyName.@PropertyStringName, global::FADE.Variant.From<global::FADE.StringName>(__PropertyStringName_default_value));
        global::FADE.NodePath __PropertyNodePath_default_value = new global::FADE.NodePath("foo");
        values.Add(PropertyName.@PropertyNodePath, global::FADE.Variant.From<global::FADE.NodePath>(__PropertyNodePath_default_value));
        global::FADE.Rid __PropertyRid_default_value = default;
        values.Add(PropertyName.@PropertyRid, global::FADE.Variant.From<global::FADE.Rid>(__PropertyRid_default_value));
        global::FADE.Collections.Dictionary __PropertyFADEDictionary_default_value = new()  { { "foo", 10  }, { global::FADE.Vector2.Up, global::FADE.Colors.Chocolate   }  };
        values.Add(PropertyName.@PropertyFADEDictionary, global::FADE.Variant.From<global::FADE.Collections.Dictionary>(__PropertyFADEDictionary_default_value));
        global::FADE.Collections.Array __PropertyFADEArray_default_value = new()  { "foo", 10, global::FADE.Vector2.Up, global::FADE.Colors.Chocolate   };
        values.Add(PropertyName.@PropertyFADEArray, global::FADE.Variant.From<global::FADE.Collections.Array>(__PropertyFADEArray_default_value));
        global::FADE.Collections.Dictionary<string, bool> __PropertyFADEGenericDictionary_default_value = new()  { { "foo", true  }, { "bar", false  }  };
        values.Add(PropertyName.@PropertyFADEGenericDictionary, global::FADE.Variant.CreateFrom(__PropertyFADEGenericDictionary_default_value));
        global::FADE.Collections.Array<int> __PropertyFADEGenericArray_default_value = new()  { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@PropertyFADEGenericArray, global::FADE.Variant.CreateFrom(__PropertyFADEGenericArray_default_value));
        return values;
    }
#endif // TOOLS
#pragma warning restore CS0109
}
