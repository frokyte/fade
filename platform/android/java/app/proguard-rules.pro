# Protect FADE Engine Core
-keep class com.godot.** { *; }
-keep class org.godotengine.** { *; }
-keep class ** extends org.godotengine.godot.plugin.FADEPlugin { *; }

# Protect methods exposed to FADE via @UsedByFADE
-keepattributes *Annotation*
-keepclassmembers class * {
    @org.godotengine.godot.plugin.UsedByFADE *;
}

# Protect JNI bindings
-keepclasseswithmembernames class * {
    native <methods>;
}
-keep public class * extends android.app.Activity
-keep public class * extends android.app.Application
-keep public class * extends android.app.Service
