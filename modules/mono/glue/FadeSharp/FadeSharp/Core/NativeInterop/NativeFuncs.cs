#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable IDE1006 // Naming rule violation
// ReSharper disable InconsistentNaming

using System;
using System.Runtime.CompilerServices;
using FADE.SourceGenerators.Internal;


namespace FADE.NativeInterop
{
    /*
     * IMPORTANT:
     * The order of the methods defined in NativeFuncs must match the order
     * in the array defined at the bottom of 'glue/runtime_interop.cpp'.
     */

    [GenerateUnmanagedCallbacks(typeof(UnmanagedCallbacks))]
    public static unsafe partial class NativeFuncs
    {
        private static bool initialized;

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
        public static void Initialize(IntPtr unmanagedCallbacks, int unmanagedCallbacksSize)
        {
            if (initialized)
                throw new InvalidOperationException("Already initialized.");
            initialized = true;

            if (unmanagedCallbacksSize != sizeof(UnmanagedCallbacks))
                throw new ArgumentException("Unmanaged callbacks size mismatch.", nameof(unmanagedCallbacksSize));

            _unmanagedCallbacks = Unsafe.AsRef<UnmanagedCallbacks>((void*)unmanagedCallbacks);
        }

        private partial struct UnmanagedCallbacks
        {
        }

        // Custom functions

        internal static partial fade_bool fadesharp_dotnet_module_is_initialized();

        internal static partial void fadesharp_initialize_marshaling_information(out ulong r_ref_count_offset, out ulong r_capacity_offset, out ulong r_size_offset, out ulong r_data_offset);

        public static partial IntPtr fadesharp_method_bind_get_method(in fade_string_name p_classname,
            in fade_string_name p_methodname);

        public static partial IntPtr fadesharp_method_bind_get_method_with_compatibility(
            in fade_string_name p_classname, in fade_string_name p_methodname, ulong p_hash);

        public static partial delegate* unmanaged<fade_bool, IntPtr> fadesharp_get_class_constructor(
            in fade_string_name p_classname);

        public static partial IntPtr fadesharp_engine_get_singleton(in fade_string p_name);


        internal static partial Error fadesharp_stack_info_vector_resize(
            ref DebuggingUtils.fade_stack_info_vector p_stack_info_vector, int p_size);

        internal static partial void fadesharp_stack_info_vector_destroy(
            ref DebuggingUtils.fade_stack_info_vector p_stack_info_vector);

        internal static partial void fadesharp_internal_editor_file_system_update_files(in fade_packed_string_array p_script_paths);

        internal static partial void fadesharp_internal_script_debugger_send_error(in fade_string p_func,
            in fade_string p_file, int p_line, in fade_string p_err, in fade_string p_descr,
            fade_error_handler_type p_type, in DebuggingUtils.fade_stack_info_vector p_stack_info_vector);

        internal static partial fade_bool fadesharp_internal_script_debugger_is_active();

        internal static partial IntPtr fadesharp_internal_object_get_associated_gchandle(IntPtr ptr);

        internal static partial void fadesharp_internal_object_disposed(IntPtr ptr, IntPtr gcHandleToFree);

        internal static partial void fadesharp_internal_refcounted_disposed(IntPtr ptr, IntPtr gcHandleToFree,
            fade_bool isFinalizer);

        internal static partial Error fadesharp_internal_signal_awaiter_connect(IntPtr source,
            in fade_string_name signal,
            IntPtr target, IntPtr awaiterHandlePtr);

        internal static partial void fadesharp_internal_tie_native_managed_to_unmanaged(IntPtr gcHandleIntPtr,
            IntPtr unmanaged, in fade_string_name nativeName, fade_bool refCounted);

        internal static partial void fadesharp_internal_tie_user_managed_to_unmanaged(IntPtr gcHandleIntPtr,
            IntPtr unmanaged, fade_ref* scriptPtr, fade_bool refCounted);

        internal static partial void fadesharp_internal_tie_managed_to_unmanaged_with_pre_setup(
            IntPtr gcHandleIntPtr, IntPtr unmanaged);

        internal static partial IntPtr fadesharp_internal_unmanaged_get_script_instance_managed(IntPtr p_unmanaged,
            out fade_bool r_has_cs_script_instance);

        internal static partial IntPtr fadesharp_internal_unmanaged_get_instance_binding_managed(IntPtr p_unmanaged);

        internal static partial IntPtr fadesharp_internal_unmanaged_instance_binding_create_managed(IntPtr p_unmanaged,
            IntPtr oldGCHandlePtr);

        internal static partial void fadesharp_internal_new_csharp_script(fade_ref* r_dest);

        internal static partial fade_bool fadesharp_internal_script_load(in fade_string p_path, fade_ref* r_dest);

        internal static partial void fadesharp_internal_reload_registered_script(IntPtr scriptPtr);

        internal static partial void fadesharp_array_filter_fade_objects_by_native(scoped in fade_string_name p_native_name,
            scoped in fade_array p_input, out fade_array r_output);

        internal static partial void fadesharp_array_filter_fade_objects_by_non_native(scoped in fade_array p_input,
            out fade_array r_output);

        public static partial void fadesharp_ref_new_from_ref_counted_ptr(out fade_ref r_dest,
            IntPtr p_ref_counted_ptr);

        public static partial void fadesharp_ref_destroy(ref fade_ref p_instance);

        public static partial void fadesharp_string_name_new_from_string(out fade_string_name r_dest,
            scoped in fade_string p_name);

        public static partial void fadesharp_node_path_new_from_string(out fade_node_path r_dest,
            scoped in fade_string p_name);

        public static partial void
            fadesharp_string_name_as_string(out fade_string r_dest, scoped in fade_string_name p_name);

        public static partial void fadesharp_node_path_as_string(out fade_string r_dest, scoped in fade_node_path p_np);

        public static partial fade_packed_byte_array fadesharp_packed_byte_array_new_mem_copy(byte* p_src,
            int p_length);

        public static partial fade_packed_int32_array fadesharp_packed_int32_array_new_mem_copy(int* p_src,
            int p_length);

        public static partial fade_packed_int64_array fadesharp_packed_int64_array_new_mem_copy(long* p_src,
            int p_length);

        public static partial fade_packed_float32_array fadesharp_packed_float32_array_new_mem_copy(float* p_src,
            int p_length);

        public static partial fade_packed_float64_array fadesharp_packed_float64_array_new_mem_copy(double* p_src,
            int p_length);

        public static partial fade_packed_vector2_array fadesharp_packed_vector2_array_new_mem_copy(Vector2* p_src,
            int p_length);

        public static partial fade_packed_vector3_array fadesharp_packed_vector3_array_new_mem_copy(Vector3* p_src,
            int p_length);

        public static partial fade_packed_vector4_array fadesharp_packed_vector4_array_new_mem_copy(Vector4* p_src,
            int p_length);

        public static partial fade_packed_color_array fadesharp_packed_color_array_new_mem_copy(Color* p_src,
            int p_length);

        public static partial void fadesharp_packed_string_array_add(ref fade_packed_string_array r_dest,
            in fade_string p_element);

        public static partial void fadesharp_callable_new_with_delegate(IntPtr p_delegate_handle, IntPtr p_trampoline,
            IntPtr p_object, out fade_callable r_callable);

        internal static partial fade_bool fadesharp_callable_get_data_for_marshalling(scoped in fade_callable p_callable,
            out IntPtr r_delegate_handle, out IntPtr r_trampoline, out IntPtr r_object, out fade_string_name r_name);

        internal static partial fade_variant fadesharp_callable_call(scoped in fade_callable p_callable,
            fade_variant** p_args, int p_arg_count, out fade_variant_call_error p_call_error);

        internal static partial void fadesharp_callable_call_deferred(in fade_callable p_callable,
            fade_variant** p_args, int p_arg_count);

        internal static partial Color fadesharp_color_from_ok_hsl(float p_h, float p_s, float p_l, float p_alpha);

        internal static partial float fadesharp_color_get_ok_hsl_h(in Color p_self);

        internal static partial float fadesharp_color_get_ok_hsl_s(in Color p_self);

        internal static partial float fadesharp_color_get_ok_hsl_l(in Color p_self);

        // GDNative functions

        // gdnative.h

        public static partial void fadesharp_method_bind_ptrcall(IntPtr p_method_bind, IntPtr p_instance, void** p_args,
            void* p_ret);

        public static partial fade_variant fadesharp_method_bind_call(IntPtr p_method_bind, IntPtr p_instance,
            fade_variant** p_args, int p_arg_count, out fade_variant_call_error p_call_error);

        // variant.h

        public static partial void
            fadesharp_variant_new_string_name(out fade_variant r_dest, scoped in fade_string_name p_s);

        public static partial void fadesharp_variant_new_copy(out fade_variant r_dest, scoped in fade_variant p_src);

        public static partial void fadesharp_variant_new_node_path(out fade_variant r_dest, scoped in fade_node_path p_np);

        public static partial void fadesharp_variant_new_object(out fade_variant r_dest, IntPtr p_obj);

        public static partial void fadesharp_variant_new_transform2d(out fade_variant r_dest, scoped in Transform2D p_t2d);

        public static partial void fadesharp_variant_new_basis(out fade_variant r_dest, scoped in Basis p_basis);

        public static partial void fadesharp_variant_new_transform3d(out fade_variant r_dest, scoped in Transform3D p_trans);

        public static partial void fadesharp_variant_new_projection(out fade_variant r_dest, scoped in Projection p_proj);

        public static partial void fadesharp_variant_new_aabb(out fade_variant r_dest, scoped in Aabb p_aabb);

        public static partial void fadesharp_variant_new_dictionary(out fade_variant r_dest,
            scoped in fade_dictionary p_dict);

        public static partial void fadesharp_variant_new_array(out fade_variant r_dest, scoped in fade_array p_arr);

        public static partial void fadesharp_variant_new_packed_byte_array(out fade_variant r_dest,
            scoped in fade_packed_byte_array p_pba);

        public static partial void fadesharp_variant_new_packed_int32_array(out fade_variant r_dest,
            scoped in fade_packed_int32_array p_pia);

        public static partial void fadesharp_variant_new_packed_int64_array(out fade_variant r_dest,
            scoped in fade_packed_int64_array p_pia);

        public static partial void fadesharp_variant_new_packed_float32_array(out fade_variant r_dest,
            scoped in fade_packed_float32_array p_pra);

        public static partial void fadesharp_variant_new_packed_float64_array(out fade_variant r_dest,
            scoped in fade_packed_float64_array p_pra);

        public static partial void fadesharp_variant_new_packed_string_array(out fade_variant r_dest,
            scoped in fade_packed_string_array p_psa);

        public static partial void fadesharp_variant_new_packed_vector2_array(out fade_variant r_dest,
            scoped in fade_packed_vector2_array p_pv2a);

        public static partial void fadesharp_variant_new_packed_vector3_array(out fade_variant r_dest,
            scoped in fade_packed_vector3_array p_pv3a);

        public static partial void fadesharp_variant_new_packed_vector4_array(out fade_variant r_dest,
            scoped in fade_packed_vector4_array p_pv4a);

        public static partial void fadesharp_variant_new_packed_color_array(out fade_variant r_dest,
            scoped in fade_packed_color_array p_pca);

        public static partial fade_bool fadesharp_variant_as_bool(scoped in fade_variant p_self);

        public static partial Int64 fadesharp_variant_as_int(scoped in fade_variant p_self);

        public static partial double fadesharp_variant_as_float(scoped in fade_variant p_self);

        public static partial fade_string fadesharp_variant_as_string(scoped in fade_variant p_self);

        public static partial Vector2 fadesharp_variant_as_vector2(scoped in fade_variant p_self);

        public static partial Vector2I fadesharp_variant_as_vector2i(scoped in fade_variant p_self);

        public static partial Rect2 fadesharp_variant_as_rect2(scoped in fade_variant p_self);

        public static partial Rect2I fadesharp_variant_as_rect2i(scoped in fade_variant p_self);

        public static partial Vector3 fadesharp_variant_as_vector3(scoped in fade_variant p_self);

        public static partial Vector3I fadesharp_variant_as_vector3i(scoped in fade_variant p_self);

        public static partial Transform2D fadesharp_variant_as_transform2d(scoped in fade_variant p_self);

        public static partial Vector4 fadesharp_variant_as_vector4(scoped in fade_variant p_self);

        public static partial Vector4I fadesharp_variant_as_vector4i(scoped in fade_variant p_self);

        public static partial Plane fadesharp_variant_as_plane(scoped in fade_variant p_self);

        public static partial Quaternion fadesharp_variant_as_quaternion(scoped in fade_variant p_self);

        public static partial Aabb fadesharp_variant_as_aabb(scoped in fade_variant p_self);

        public static partial Basis fadesharp_variant_as_basis(scoped in fade_variant p_self);

        public static partial Transform3D fadesharp_variant_as_transform3d(scoped in fade_variant p_self);

        public static partial Projection fadesharp_variant_as_projection(scoped in fade_variant p_self);

        public static partial Color fadesharp_variant_as_color(scoped in fade_variant p_self);

        public static partial fade_string_name fadesharp_variant_as_string_name(scoped in fade_variant p_self);

        public static partial fade_node_path fadesharp_variant_as_node_path(scoped in fade_variant p_self);

        public static partial Rid fadesharp_variant_as_rid(scoped in fade_variant p_self);

        public static partial fade_callable fadesharp_variant_as_callable(scoped in fade_variant p_self);

        public static partial fade_signal fadesharp_variant_as_signal(scoped in fade_variant p_self);

        public static partial fade_dictionary fadesharp_variant_as_dictionary(scoped in fade_variant p_self);

        public static partial fade_array fadesharp_variant_as_array(scoped in fade_variant p_self);

        public static partial fade_packed_byte_array fadesharp_variant_as_packed_byte_array(scoped in fade_variant p_self);

        public static partial fade_packed_int32_array fadesharp_variant_as_packed_int32_array(scoped in fade_variant p_self);

        public static partial fade_packed_int64_array fadesharp_variant_as_packed_int64_array(scoped in fade_variant p_self);

        public static partial fade_packed_float32_array fadesharp_variant_as_packed_float32_array(scoped in fade_variant p_self);

        public static partial fade_packed_float64_array fadesharp_variant_as_packed_float64_array(scoped in fade_variant p_self);

        public static partial fade_packed_string_array fadesharp_variant_as_packed_string_array(scoped in fade_variant p_self);

        public static partial fade_packed_vector2_array fadesharp_variant_as_packed_vector2_array(scoped in fade_variant p_self);

        public static partial fade_packed_vector3_array fadesharp_variant_as_packed_vector3_array(scoped in fade_variant p_self);

        public static partial fade_packed_vector4_array fadesharp_variant_as_packed_vector4_array(
            in fade_variant p_self);

        public static partial fade_packed_color_array fadesharp_variant_as_packed_color_array(scoped in fade_variant p_self);

        public static partial fade_bool fadesharp_variant_equals(scoped in fade_variant p_a, scoped in fade_variant p_b);

        // string.h

        public static partial void fadesharp_string_new_with_utf16_chars(out fade_string r_dest, char* p_contents);

        // string_name.h

        public static partial void fadesharp_string_name_new_copy(out fade_string_name r_dest,
            scoped in fade_string_name p_src);

        // node_path.h

        public static partial void fadesharp_node_path_new_copy(out fade_node_path r_dest, scoped in fade_node_path p_src);

        // array.h

        public static partial void fadesharp_array_new(out fade_array r_dest);

        public static partial void fadesharp_array_new_copy(out fade_array r_dest, scoped in fade_array p_src);

        public static partial fade_variant* fadesharp_array_ptrw(ref fade_array p_self);

        // dictionary.h

        public static partial void fadesharp_dictionary_new(out fade_dictionary r_dest);

        public static partial void fadesharp_dictionary_new_copy(out fade_dictionary r_dest,
            scoped in fade_dictionary p_src);

        // destroy functions

        public static partial void fadesharp_packed_byte_array_destroy(ref fade_packed_byte_array p_self);

        public static partial void fadesharp_packed_int32_array_destroy(ref fade_packed_int32_array p_self);

        public static partial void fadesharp_packed_int64_array_destroy(ref fade_packed_int64_array p_self);

        public static partial void fadesharp_packed_float32_array_destroy(ref fade_packed_float32_array p_self);

        public static partial void fadesharp_packed_float64_array_destroy(ref fade_packed_float64_array p_self);

        public static partial void fadesharp_packed_string_array_destroy(ref fade_packed_string_array p_self);

        public static partial void fadesharp_packed_vector2_array_destroy(ref fade_packed_vector2_array p_self);

        public static partial void fadesharp_packed_vector3_array_destroy(ref fade_packed_vector3_array p_self);

        public static partial void fadesharp_packed_vector4_array_destroy(ref fade_packed_vector4_array p_self);

        public static partial void fadesharp_packed_color_array_destroy(ref fade_packed_color_array p_self);

        public static partial void fadesharp_variant_destroy(ref fade_variant p_self);

        public static partial void fadesharp_string_destroy(ref fade_string p_self);

        public static partial void fadesharp_string_name_destroy(ref fade_string_name p_self);

        public static partial void fadesharp_node_path_destroy(ref fade_node_path p_self);

        public static partial void fadesharp_signal_destroy(ref fade_signal p_self);

        public static partial void fadesharp_callable_destroy(ref fade_callable p_self);

        public static partial void fadesharp_array_destroy(ref fade_array p_self);

        public static partial void fadesharp_dictionary_destroy(ref fade_dictionary p_self);

        // Array

        public static partial int fadesharp_array_add(ref fade_array p_self, in fade_variant p_item);

        public static partial int fadesharp_array_add_range(ref fade_array p_self, in fade_array p_collection);

        public static partial int fadesharp_array_binary_search(ref fade_array p_self, int p_index, int p_count, in fade_variant p_value);

        public static partial void fadesharp_array_duplicate(scoped ref fade_array p_self, fade_bool p_deep, out fade_array r_dest);

        public static partial void fadesharp_array_fill(ref fade_array p_self, in fade_variant p_value);

        public static partial int fadesharp_array_index_of(ref fade_array p_self, in fade_variant p_item, int p_index = 0);

        public static partial void fadesharp_array_insert(ref fade_array p_self, int p_index, in fade_variant p_item);

        public static partial int fadesharp_array_last_index_of(ref fade_array p_self, in fade_variant p_item, int p_index);

        public static partial void fadesharp_array_make_read_only(ref fade_array p_self);

        public static partial void fadesharp_array_set_typed(
            ref fade_array p_self,
            uint p_elem_type,
            in fade_string_name p_elem_class_name,
            in fade_ref p_elem_script);

        public static partial fade_bool fadesharp_array_is_typed(ref fade_array p_self);

        public static partial void fadesharp_array_max(scoped ref fade_array p_self, out fade_variant r_value);

        public static partial void fadesharp_array_min(scoped ref fade_array p_self, out fade_variant r_value);

        public static partial void fadesharp_array_pick_random(scoped ref fade_array p_self, out fade_variant r_value);

        public static partial fade_bool fadesharp_array_recursive_equal(ref fade_array p_self, in fade_array p_other);

        public static partial void fadesharp_array_remove_at(ref fade_array p_self, int p_index);

        public static partial Error fadesharp_array_resize(ref fade_array p_self, int p_new_size);

        public static partial void fadesharp_array_reverse(ref fade_array p_self);

        public static partial void fadesharp_array_shuffle(ref fade_array p_self);

        public static partial void fadesharp_array_slice(scoped ref fade_array p_self, int p_start, int p_end,
            int p_step, fade_bool p_deep, out fade_array r_dest);

        public static partial void fadesharp_array_sort(ref fade_array p_self);

        public static partial void fadesharp_array_to_string(ref fade_array p_self, out fade_string r_str);

        public static partial void fadesharp_packed_byte_array_compress(scoped in fade_packed_byte_array p_src, int p_mode, out fade_packed_byte_array r_dst);

        public static partial void fadesharp_packed_byte_array_decompress(scoped in fade_packed_byte_array p_src, long p_buffer_size, int p_mode, out fade_packed_byte_array r_dst);

        public static partial void fadesharp_packed_byte_array_decompress_dynamic(scoped in fade_packed_byte_array p_src, long p_buffer_size, int p_mode, out fade_packed_byte_array r_dst);

        // Dictionary

        public static partial fade_bool fadesharp_dictionary_try_get_value(scoped ref fade_dictionary p_self,
            scoped in fade_variant p_key,
            out fade_variant r_value);

        public static partial void fadesharp_dictionary_set_value(ref fade_dictionary p_self, in fade_variant p_key,
            in fade_variant p_value);

        public static partial void fadesharp_dictionary_keys(scoped ref fade_dictionary p_self, out fade_array r_dest);

        public static partial void fadesharp_dictionary_values(scoped ref fade_dictionary p_self, out fade_array r_dest);

        public static partial int fadesharp_dictionary_count(ref fade_dictionary p_self);

        public static partial void fadesharp_dictionary_key_value_pair_at(scoped ref fade_dictionary p_self, int p_index,
            out fade_variant r_key, out fade_variant r_value);

        public static partial void fadesharp_dictionary_add(ref fade_dictionary p_self, in fade_variant p_key,
            in fade_variant p_value);

        public static partial void fadesharp_dictionary_clear(ref fade_dictionary p_self);

        public static partial fade_bool fadesharp_dictionary_contains_key(ref fade_dictionary p_self,
            in fade_variant p_key);

        public static partial void fadesharp_dictionary_duplicate(scoped ref fade_dictionary p_self, fade_bool p_deep,
            out fade_dictionary r_dest);

        public static partial void fadesharp_dictionary_merge(ref fade_dictionary p_self, in fade_dictionary p_dictionary, fade_bool p_overwrite);

        public static partial fade_bool fadesharp_dictionary_recursive_equal(ref fade_dictionary p_self, in fade_dictionary p_other);

        public static partial fade_bool fadesharp_dictionary_remove_key(ref fade_dictionary p_self,
            in fade_variant p_key);

        public static partial void fadesharp_dictionary_make_read_only(ref fade_dictionary p_self);

        public static partial void fadesharp_dictionary_set_typed(
            ref fade_dictionary p_self,
            uint p_key_type,
            in fade_string_name p_key_class_name,
            in fade_ref p_key_script,
            uint p_value_type,
            in fade_string_name p_value_class_name,
            in fade_ref p_value_script);

        public static partial fade_bool fadesharp_dictionary_is_typed_key(ref fade_dictionary p_self);

        public static partial fade_bool fadesharp_dictionary_is_typed_value(ref fade_dictionary p_self);

        public static partial uint fadesharp_dictionary_get_typed_key_builtin(ref fade_dictionary p_self);

        public static partial uint fadesharp_dictionary_get_typed_value_builtin(ref fade_dictionary p_self);

        public static partial void fadesharp_dictionary_get_typed_key_class_name(ref fade_dictionary p_self, out fade_string_name r_dest);

        public static partial void fadesharp_dictionary_get_typed_value_class_name(ref fade_dictionary p_self, out fade_string_name r_dest);

        public static partial void fadesharp_dictionary_get_typed_key_script(ref fade_dictionary p_self, out fade_variant r_dest);

        public static partial void fadesharp_dictionary_get_typed_value_script(ref fade_dictionary p_self, out fade_variant r_dest);

        public static partial void fadesharp_dictionary_to_string(scoped ref fade_dictionary p_self, out fade_string r_str);

        // StringExtensions

        public static partial void fadesharp_string_simplify_path(scoped in fade_string p_self,
            out fade_string r_simplified_path);

        public static partial void fadesharp_string_capitalize(scoped in fade_string p_self,
            out fade_string r_capitalized);

        public static partial void fadesharp_string_to_camel_case(scoped in fade_string p_self,
            out fade_string r_camel_case);

        public static partial void fadesharp_string_to_pascal_case(scoped in fade_string p_self,
            out fade_string r_pascal_case);

        public static partial void fadesharp_string_to_snake_case(scoped in fade_string p_self,
            out fade_string r_snake_case);

        public static partial void fadesharp_string_to_kebab_case(scoped in fade_string p_self,
            out fade_string r_kebab_case);

        // NodePath

        public static partial void fadesharp_node_path_get_as_property_path(in fade_node_path p_self,
            ref fade_node_path r_dest);

        public static partial void fadesharp_node_path_get_concatenated_names(scoped in fade_node_path p_self,
            out fade_string r_names);

        public static partial void fadesharp_node_path_get_concatenated_subnames(scoped in fade_node_path p_self,
            out fade_string r_subnames);

        public static partial void fadesharp_node_path_get_name(scoped in fade_node_path p_self, int p_idx,
            out fade_string r_name);

        public static partial int fadesharp_node_path_get_name_count(in fade_node_path p_self);

        public static partial void fadesharp_node_path_get_subname(scoped in fade_node_path p_self, int p_idx,
            out fade_string r_subname);

        public static partial int fadesharp_node_path_get_subname_count(in fade_node_path p_self);

        public static partial fade_bool fadesharp_node_path_is_absolute(in fade_node_path p_self);

        public static partial fade_bool fadesharp_node_path_equals(in fade_node_path p_self, in fade_node_path p_other);

        public static partial int fadesharp_node_path_hash(in fade_node_path p_self);

        // GD, etc

        internal static partial void fadesharp_bytes_to_var(scoped in fade_packed_byte_array p_bytes,
            fade_bool p_allow_objects,
            out fade_variant r_ret);

        internal static partial void fadesharp_convert(scoped in fade_variant p_what, int p_type,
            out fade_variant r_ret);

        internal static partial int fadesharp_hash(in fade_variant p_var);

        internal static partial IntPtr fadesharp_instance_from_id(ulong p_instance_id);

        internal static partial void fadesharp_print(in fade_string p_what);

        public static partial void fadesharp_print_rich(in fade_string p_what);

        internal static partial void fadesharp_printerr(in fade_string p_what);

        internal static partial void fadesharp_printraw(in fade_string p_what);

        internal static partial void fadesharp_prints(in fade_string p_what);

        internal static partial void fadesharp_printt(in fade_string p_what);

        internal static partial float fadesharp_randf();

        internal static partial uint fadesharp_randi();

        internal static partial void fadesharp_randomize();

        internal static partial double fadesharp_randf_range(double from, double to);

        internal static partial double fadesharp_randfn(double mean, double deviation);

        internal static partial int fadesharp_randi_range(int from, int to);

        internal static partial uint fadesharp_rand_from_seed(ulong seed, out ulong newSeed);

        internal static partial void fadesharp_seed(ulong seed);

        internal static partial void fadesharp_weakref(IntPtr p_obj, out fade_ref r_weak_ref);

        internal static partial void fadesharp_str_to_var(scoped in fade_string p_str, out fade_variant r_ret);

        internal static partial void fadesharp_var_to_bytes(scoped in fade_variant p_what, fade_bool p_full_objects,
            out fade_packed_byte_array r_bytes);

        internal static partial void fadesharp_var_to_str(scoped in fade_variant p_var, out fade_string r_ret);

        internal static partial void fadesharp_err_print_error(in fade_string p_function, in fade_string p_file, int p_line, in fade_string p_error, in fade_string p_message = default, fade_bool p_editor_notify = fade_bool.False, fade_error_handler_type p_type = fade_error_handler_type.ERR_HANDLER_ERROR);

        // Object

        public static partial void fadesharp_object_to_string(IntPtr ptr, out fade_string r_str);
    }
}
