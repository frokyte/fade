partial class ExportedFields
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
        var values = new global::System.Collections.Generic.Dictionary<global::FADE.StringName, global::FADE.Variant>(62);
        bool ___fieldBoolean_default_value = true;
        values.Add(PropertyName.@_fieldBoolean, global::FADE.Variant.From<bool>(___fieldBoolean_default_value));
        char ___fieldChar_default_value = 'f';
        values.Add(PropertyName.@_fieldChar, global::FADE.Variant.From<char>(___fieldChar_default_value));
        sbyte ___fieldSByte_default_value = 10;
        values.Add(PropertyName.@_fieldSByte, global::FADE.Variant.From<sbyte>(___fieldSByte_default_value));
        short ___fieldInt16_default_value = 10;
        values.Add(PropertyName.@_fieldInt16, global::FADE.Variant.From<short>(___fieldInt16_default_value));
        int ___fieldInt32_default_value = 10;
        values.Add(PropertyName.@_fieldInt32, global::FADE.Variant.From<int>(___fieldInt32_default_value));
        long ___fieldInt64_default_value = -10_000;
        values.Add(PropertyName.@_fieldInt64, global::FADE.Variant.From<long>(___fieldInt64_default_value));
        byte ___fieldByte_default_value = 10;
        values.Add(PropertyName.@_fieldByte, global::FADE.Variant.From<byte>(___fieldByte_default_value));
        ushort ___fieldUInt16_default_value = 10;
        values.Add(PropertyName.@_fieldUInt16, global::FADE.Variant.From<ushort>(___fieldUInt16_default_value));
        uint ___fieldUInt32_default_value = 10;
        values.Add(PropertyName.@_fieldUInt32, global::FADE.Variant.From<uint>(___fieldUInt32_default_value));
        ulong ___fieldUInt64_default_value = 10;
        values.Add(PropertyName.@_fieldUInt64, global::FADE.Variant.From<ulong>(___fieldUInt64_default_value));
        float ___fieldSingle_default_value = 10;
        values.Add(PropertyName.@_fieldSingle, global::FADE.Variant.From<float>(___fieldSingle_default_value));
        double ___fieldDouble_default_value = 10;
        values.Add(PropertyName.@_fieldDouble, global::FADE.Variant.From<double>(___fieldDouble_default_value));
        string ___fieldString_default_value = "foo";
        values.Add(PropertyName.@_fieldString, global::FADE.Variant.From<string>(___fieldString_default_value));
        float ___fieldStaticImport_default_value = global::FADE.Mathf.RadToDeg(2  * global::FADE.Mathf.Pi);
        values.Add(PropertyName.@_fieldStaticImport, global::FADE.Variant.From<float>(___fieldStaticImport_default_value));
        global::FADE.Vector2 ___fieldVector2_default_value = new(10f, 10f);
        values.Add(PropertyName.@_fieldVector2, global::FADE.Variant.From<global::FADE.Vector2>(___fieldVector2_default_value));
        global::FADE.Vector2I ___fieldVector2I_default_value = global::FADE.Vector2I.Up;
        values.Add(PropertyName.@_fieldVector2I, global::FADE.Variant.From<global::FADE.Vector2I>(___fieldVector2I_default_value));
        global::FADE.Rect2 ___fieldRect2_default_value = new(new global::FADE.Vector2(10f, 10f), new global::FADE.Vector2(10f, 10f));
        values.Add(PropertyName.@_fieldRect2, global::FADE.Variant.From<global::FADE.Rect2>(___fieldRect2_default_value));
        global::FADE.Rect2I ___fieldRect2I_default_value = new(new global::FADE.Vector2I(10, 10), new global::FADE.Vector2I(10, 10));
        values.Add(PropertyName.@_fieldRect2I, global::FADE.Variant.From<global::FADE.Rect2I>(___fieldRect2I_default_value));
        global::FADE.Transform2D ___fieldTransform2D_default_value = global::FADE.Transform2D.Identity;
        values.Add(PropertyName.@_fieldTransform2D, global::FADE.Variant.From<global::FADE.Transform2D>(___fieldTransform2D_default_value));
        global::FADE.Vector3 ___fieldVector3_default_value = new(10f, 10f, 10f);
        values.Add(PropertyName.@_fieldVector3, global::FADE.Variant.From<global::FADE.Vector3>(___fieldVector3_default_value));
        global::FADE.Vector3I ___fieldVector3I_default_value = global::FADE.Vector3I.Back;
        values.Add(PropertyName.@_fieldVector3I, global::FADE.Variant.From<global::FADE.Vector3I>(___fieldVector3I_default_value));
        global::FADE.Basis ___fieldBasis_default_value = new global::FADE.Basis(global::FADE.Quaternion.Identity);
        values.Add(PropertyName.@_fieldBasis, global::FADE.Variant.From<global::FADE.Basis>(___fieldBasis_default_value));
        global::FADE.Quaternion ___fieldQuaternion_default_value = new global::FADE.Quaternion(global::FADE.Basis.Identity);
        values.Add(PropertyName.@_fieldQuaternion, global::FADE.Variant.From<global::FADE.Quaternion>(___fieldQuaternion_default_value));
        global::FADE.Transform3D ___fieldTransform3D_default_value = global::FADE.Transform3D.Identity;
        values.Add(PropertyName.@_fieldTransform3D, global::FADE.Variant.From<global::FADE.Transform3D>(___fieldTransform3D_default_value));
        global::FADE.Vector4 ___fieldVector4_default_value = new(10f, 10f, 10f, 10f);
        values.Add(PropertyName.@_fieldVector4, global::FADE.Variant.From<global::FADE.Vector4>(___fieldVector4_default_value));
        global::FADE.Vector4I ___fieldVector4I_default_value = global::FADE.Vector4I.One;
        values.Add(PropertyName.@_fieldVector4I, global::FADE.Variant.From<global::FADE.Vector4I>(___fieldVector4I_default_value));
        global::FADE.Projection ___fieldProjection_default_value = global::FADE.Projection.Identity;
        values.Add(PropertyName.@_fieldProjection, global::FADE.Variant.From<global::FADE.Projection>(___fieldProjection_default_value));
        global::FADE.Aabb ___fieldAabb_default_value = new global::FADE.Aabb(10f, 10f, 10f, new global::FADE.Vector3(1f, 1f, 1f));
        values.Add(PropertyName.@_fieldAabb, global::FADE.Variant.From<global::FADE.Aabb>(___fieldAabb_default_value));
        global::FADE.Color ___fieldColor_default_value = global::FADE.Colors.Aquamarine;
        values.Add(PropertyName.@_fieldColor, global::FADE.Variant.From<global::FADE.Color>(___fieldColor_default_value));
        global::FADE.Plane ___fieldPlane_default_value = global::FADE.Plane.PlaneXZ;
        values.Add(PropertyName.@_fieldPlane, global::FADE.Variant.From<global::FADE.Plane>(___fieldPlane_default_value));
        global::FADE.Callable ___fieldCallable_default_value = new global::FADE.Callable(global::FADE.Engine.GetMainLoop(), "_process");
        values.Add(PropertyName.@_fieldCallable, global::FADE.Variant.From<global::FADE.Callable>(___fieldCallable_default_value));
        global::FADE.Signal ___fieldSignal_default_value = new global::FADE.Signal(global::FADE.Engine.GetMainLoop(), "property_list_changed");
        values.Add(PropertyName.@_fieldSignal, global::FADE.Variant.From<global::FADE.Signal>(___fieldSignal_default_value));
        global::ExportedFields.MyEnum ___fieldEnum_default_value = global::ExportedFields.MyEnum.C;
        values.Add(PropertyName.@_fieldEnum, global::FADE.Variant.From<global::ExportedFields.MyEnum>(___fieldEnum_default_value));
        global::ExportedFields.MyFlagsEnum ___fieldFlagsEnum_default_value = global::ExportedFields.MyFlagsEnum.C;
        values.Add(PropertyName.@_fieldFlagsEnum, global::FADE.Variant.From<global::ExportedFields.MyFlagsEnum>(___fieldFlagsEnum_default_value));
        byte[] ___fieldByteArray_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@_fieldByteArray, global::FADE.Variant.From<byte[]>(___fieldByteArray_default_value));
        int[] ___fieldInt32Array_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@_fieldInt32Array, global::FADE.Variant.From<int[]>(___fieldInt32Array_default_value));
        long[] ___fieldInt64Array_default_value = { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@_fieldInt64Array, global::FADE.Variant.From<long[]>(___fieldInt64Array_default_value));
        float[] ___fieldSingleArray_default_value = { 0f, 1f, 2f, 3f, 4f, 5f, 6f  };
        values.Add(PropertyName.@_fieldSingleArray, global::FADE.Variant.From<float[]>(___fieldSingleArray_default_value));
        double[] ___fieldDoubleArray_default_value = { 0d, 1d, 2d, 3d, 4d, 5d, 6d  };
        values.Add(PropertyName.@_fieldDoubleArray, global::FADE.Variant.From<double[]>(___fieldDoubleArray_default_value));
        string[] ___fieldStringArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@_fieldStringArray, global::FADE.Variant.From<string[]>(___fieldStringArray_default_value));
        string[] ___fieldStringArrayEnum_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@_fieldStringArrayEnum, global::FADE.Variant.From<string[]>(___fieldStringArrayEnum_default_value));
        global::FADE.Vector2[] ___fieldVector2Array_default_value = { global::FADE.Vector2.Up, global::FADE.Vector2.Down, global::FADE.Vector2.Left, global::FADE.Vector2.Right   };
        values.Add(PropertyName.@_fieldVector2Array, global::FADE.Variant.From<global::FADE.Vector2[]>(___fieldVector2Array_default_value));
        global::FADE.Vector3[] ___fieldVector3Array_default_value = { global::FADE.Vector3.Up, global::FADE.Vector3.Down, global::FADE.Vector3.Left, global::FADE.Vector3.Right   };
        values.Add(PropertyName.@_fieldVector3Array, global::FADE.Variant.From<global::FADE.Vector3[]>(___fieldVector3Array_default_value));
        global::FADE.Color[] ___fieldColorArray_default_value = { global::FADE.Colors.Aqua, global::FADE.Colors.Aquamarine, global::FADE.Colors.Azure, global::FADE.Colors.Beige   };
        values.Add(PropertyName.@_fieldColorArray, global::FADE.Variant.From<global::FADE.Color[]>(___fieldColorArray_default_value));
        global::FADE.FADEObject[] ___fieldFADEObjectOrDerivedArray_default_value = { null  };
        values.Add(PropertyName.@_fieldFADEObjectOrDerivedArray, global::FADE.Variant.CreateFrom(___fieldFADEObjectOrDerivedArray_default_value));
        global::FADE.StringName[] ___fieldStringNameArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@_fieldStringNameArray, global::FADE.Variant.From<global::FADE.StringName[]>(___fieldStringNameArray_default_value));
        global::FADE.NodePath[] ___fieldNodePathArray_default_value = { "foo", "bar"  };
        values.Add(PropertyName.@_fieldNodePathArray, global::FADE.Variant.From<global::FADE.NodePath[]>(___fieldNodePathArray_default_value));
        global::FADE.Rid[] ___fieldRidArray_default_value = { default, default, default  };
        values.Add(PropertyName.@_fieldRidArray, global::FADE.Variant.From<global::FADE.Rid[]>(___fieldRidArray_default_value));
        int[] ___fieldEmptyInt32Array_default_value = global::System.Array.Empty<int>();
        values.Add(PropertyName.@_fieldEmptyInt32Array, global::FADE.Variant.From<int[]>(___fieldEmptyInt32Array_default_value));
        int[] ___fieldArrayFromList_default_value = new global::System.Collections.Generic.List<int>(global::System.Array.Empty<int>()).ToArray();
        values.Add(PropertyName.@_fieldArrayFromList, global::FADE.Variant.From<int[]>(___fieldArrayFromList_default_value));
        global::FADE.Variant ___fieldVariant_default_value = "foo";
        values.Add(PropertyName.@_fieldVariant, global::FADE.Variant.From<global::FADE.Variant>(___fieldVariant_default_value));
        global::FADE.FADEObject ___fieldFADEObjectOrDerived_default_value = default;
        values.Add(PropertyName.@_fieldFADEObjectOrDerived, global::FADE.Variant.From<global::FADE.FADEObject>(___fieldFADEObjectOrDerived_default_value));
        global::FADE.Texture ___fieldFADEResourceTexture_default_value = default;
        values.Add(PropertyName.@_fieldFADEResourceTexture, global::FADE.Variant.From<global::FADE.Texture>(___fieldFADEResourceTexture_default_value));
        global::FADE.Texture ___fieldFADEResourceTextureWithInitializer_default_value = new()  { ResourceName  = ""   };
        values.Add(PropertyName.@_fieldFADEResourceTextureWithInitializer, global::FADE.Variant.From<global::FADE.Texture>(___fieldFADEResourceTextureWithInitializer_default_value));
        global::FADE.StringName ___fieldStringName_default_value = new global::FADE.StringName("foo");
        values.Add(PropertyName.@_fieldStringName, global::FADE.Variant.From<global::FADE.StringName>(___fieldStringName_default_value));
        global::FADE.NodePath ___fieldNodePath_default_value = new global::FADE.NodePath("foo");
        values.Add(PropertyName.@_fieldNodePath, global::FADE.Variant.From<global::FADE.NodePath>(___fieldNodePath_default_value));
        global::FADE.Rid ___fieldRid_default_value = default;
        values.Add(PropertyName.@_fieldRid, global::FADE.Variant.From<global::FADE.Rid>(___fieldRid_default_value));
        global::FADE.Collections.Dictionary ___fieldFADEDictionary_default_value = new()  { { "foo", 10  }, { global::FADE.Vector2.Up, global::FADE.Colors.Chocolate   }  };
        values.Add(PropertyName.@_fieldFADEDictionary, global::FADE.Variant.From<global::FADE.Collections.Dictionary>(___fieldFADEDictionary_default_value));
        global::FADE.Collections.Array ___fieldFADEArray_default_value = new()  { "foo", 10, global::FADE.Vector2.Up, global::FADE.Colors.Chocolate   };
        values.Add(PropertyName.@_fieldFADEArray, global::FADE.Variant.From<global::FADE.Collections.Array>(___fieldFADEArray_default_value));
        global::FADE.Collections.Dictionary<string, bool> ___fieldFADEGenericDictionary_default_value = new()  { { "foo", true  }, { "bar", false  }  };
        values.Add(PropertyName.@_fieldFADEGenericDictionary, global::FADE.Variant.CreateFrom(___fieldFADEGenericDictionary_default_value));
        global::FADE.Collections.Array<int> ___fieldFADEGenericArray_default_value = new()  { 0, 1, 2, 3, 4, 5, 6  };
        values.Add(PropertyName.@_fieldFADEGenericArray, global::FADE.Variant.CreateFrom(___fieldFADEGenericArray_default_value));
        long[] ___fieldEmptyInt64Array_default_value = global::System.Array.Empty<long>();
        values.Add(PropertyName.@_fieldEmptyInt64Array, global::FADE.Variant.From<long[]>(___fieldEmptyInt64Array_default_value));
        return values;
    }
#endif // TOOLS
#pragma warning restore CS0109
}
