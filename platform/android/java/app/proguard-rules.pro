# Protect FADE Engine Core
-keep class com.fade.** { *; }
-keep class org.fadeengine.** { *; }
-keep class ** extends org.fadeengine.fade.plugin.FADEPlugin { *; }

# Protect methods exposed to FADE via @UsedByFADE
-keepattributes *Annotation*
-keepclassmembers class * {
    @org.fadeengine.fade.plugin.UsedByFADE *;
}

# Protect JNI bindings
-keepclasseswithmembernames class * {
    native <methods>;
}
-keep public class * extends android.app.Activity
-keep public class * extends android.app.Application
-keep public class * extends android.app.Service
