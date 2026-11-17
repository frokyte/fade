# Fish completion for the FADE editor
# To use it, install this file in `~/.config/fish/completions` then restart your shell.
# You can also `source` this file directly in your shell startup file.
#
# Copyright (c) 2014-present FADE Engine contributors (see AUTHORS.md).
# Copyright (c) 2007-2014 Juan Linietsky, Ariel Manzur.
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.

function fade_rendering_method_args
    # Use a function instead of a fixed string to customize the argument descriptions.
    echo -e "forward_plus\tHigh-end desktop renderer"
    echo -e "mobile\tHigh-end mobile/desktop renderer"
    echo -e "gl_compatibility\tLow-end desktop, mobile and web renderer"
    echo -e "dummy\tDummy renderer"
end

function fade_rendering_driver_args
    # Use a function instead of a fixed string to customize the argument descriptions.
    echo -e "vulkan\tVulkan renderer"
    echo -e "d3d12\tDirect3D 12 renderer"
    echo -e "metal\tMetal renderer"
    echo -e "opengl3\tOpenGL 3.3 renderer"
    echo -e "opengl3_es\tOpenGL ES 3.0 renderer"
    echo -e "opengl3_angle\tOpenGL ES 3.0 renderer using ANGLE"
    echo -e "dummy\tDummy renderer"
end

# Erase existing completions for FADE.
complete -c fade -e

# General options:
complete -c fade -s h -l help -d "Display the full help message"
complete -c fade -l version -d "Display the version string"
complete -c fade -s v -l verbose -d "Use verbose stdout mode"
complete -c fade -s q -l quiet -d "Quiet mode, silences stdout messages (errors are still displayed)"
complete -c fade -l no-header -d "Do not print engine version and rendering driver/method header on startup"

# Run options:
complete -c fade -s e -l editor -d "Start the editor instead of running the scene"
complete -c fade -s p -l project-manager -d "Start the project manager, even if a project is auto-detected"
complete -c fade -l recovery-mode -d "Start the editor in recovery mode, which disables features that can typically cause startup crashes"
complete -c fade -l debug-server -d "Start the editor debug server (<protocol>://<host/IP>[:<port>] address)" -x
complete -c fade -l dap-port -d "Use the specified port for the GDScript Debug Adapter Protocol" -x
complete -c fade -l lsp-port -d "Use the specified port for the GDScript Language Server Protocol" -x
complete -c fade -l quit -d "Quit after the first iteration"
complete -c fade -l quit-after -d "Quit after the given number of iterations (0 to disable)" -x
complete -c fade -s l -l language -d "Use a specific locale (<locale> being a two-letter code)" -x
complete -c fade -l path -d "Path to a project (<directory> must contain a 'project.fade' file)" -r
complete -c fade -l scene -d "Path or UID of a scene in the project that should be started" -r
complete -c fade -l main-pack -d "Path to a pack (.pck) file to load" -r
complete -c fade -l render-thread -d "Set the render thread mode" -x -a "safe separate"
complete -c fade -l remote-fs -d "Use a remote filesystem (<host/IP>[:<port>] address)" -x
complete -c fade -l remote-fs-password -d "Password for remote filesystem" -x
complete -c fade -l audio-driver -d "Set the audio driver" -x -a "PulseAudio ALSA CoreAudio WASAPI XAudio2 Dummy"
complete -c fade -l display-driver -d "Set the display driver" -x -a "x11 wayland macos windows headless"
complete -c fade -l audio-output-latency -d "Override audio output latency in milliseconds (default is 15 ms)" -x
complete -c fade -l rendering-method -d "Set the renderer" -x -a "(fade_rendering_method_args)"
complete -c fade -l rendering-driver -d "Set the rendering driver" -x -a "(fade_rendering_driver_args)"
complete -c fade -l gpu-index -d "Use a specific GPU (run with --verbose to get available device list)" -x
complete -c fade -l text-driver -d "Set the text driver" -x
complete -c fade -l tablet-driver -d "Set the pen tablet input driver" -x
complete -c fade -l headless -d "Enable headless mode (--display-driver headless --audio-driver Dummy). Useful for servers and with --script"
complete -c fade -l log-file -d "Write output/error log to the specified path instead of the default location defined by the project" -r
complete -c fade -l write-movie -d "Write a video to the specified path (usually with .avi or .png extension). --fixed-fps is forced when enabled" -r

# Display options:
complete -c fade -s f -l fullscreen -d "Request fullscreen mode"
complete -c fade -s m -l maximized -d "Request a maximized window"
complete -c fade -s w -l windowed -d "Request windowed mode"
complete -c fade -s t -l always-on-top -d "Request an always-on-top window"
complete -c fade -l resolution -d "Request window resolution" -x
complete -c fade -l position -d "Request window position" -x
complete -c fade -l screen -d "Request window screen" -x
complete -c fade -l single-window -d "Use a single window (no separate subwindows)"
complete -c fade -l xr-mode -d "Select Extended Reality (XR) mode" -x -a "default off on"
complete -c fade -l wid -d "Request parented to window" -x
complete -c fade -l accessibility -d "Select accessibility mode" -x -a "auto always disabled"
complete -c fade -l accessibility-driver -d "Select accessibility driver" -x -a "accesskit dummy"

# Debug options:
complete -c fade -s d -l debug -d "Debug (local stdout debugger)"
complete -c fade -s b -l breakpoints -d "Specify the breakpoint list as source:line comma-separated pairs, no spaces (use %20 instead)" -x
complete -c fade -l skip-breakpoints -d "If debugger is connected, skips all breakpoints"
complete -c fade -l ignore-error-breaks -d "If debugger is connected, prevents sending error breakpoints"
complete -c fade -l profiling -d "Enable profiling in the script debugger"
complete -c fade -l gpu-profile -d "Show a GPU profile of the tasks that took the most time during frame rendering"
complete -c fade -l gpu-validation -d "Enable graphics API validation layers for debugging"
complete -c fade -l gpu-abort -d "Abort on graphics API usage errors (usually validation layer errors)"
complete -c fade -l generate-spirv-debug-info -d "Generate SPIR-V debug information (Vulkan only)"
complete -c fade -l clear-shader-cache -d "Clear the shader_cache directory at launch, so it's re-generated"
complete -c fade -l extra-gpu-memory-tracking -d "Enable additional memory tracking (Vulkan only)"
complete -c fade -l accurate-breadcrumbs -d "Force barriers between breadcrumbs (Vulkan only)"
complete -c fade -l remote-debug -d "Enable remote debugging (<protocol>://<host/IP>[:<port>] address)" -x
complete -c fade -l debug-collisions -d "Show collision shapes when running the scene"
complete -c fade -l debug-paths -d "Show path lines when running the scene"
complete -c fade -l debug-navigation -d "Show navigation polygons when running the scene"
complete -c fade -l debug-avoidance -d "Show navigation avoidance debug visuals when running the scene"
complete -c fade -l debug-stringnames -d "Print all StringName allocations to stdout when the engine quits"
complete -c fade -l debug-canvas-item-redraw -d "Display a rectangle each time a canvas item requests a redraw"
complete -c fade -l debug-mute-audio -d "Mute all audio output when running the scene"
complete -c fade -l max-fps -d "Set a maximum number of frames per second rendered (can be used to limit power usage), a value of 0 results in unlimited framerate" -x
complete -c fade -l frame-delay -d "Simulate high CPU load (delay each frame by the given number of milliseconds)" -x
complete -c fade -l time-scale -d "Force time scale (higher values are faster, 1.0 is normal speed)" -x
complete -c fade -l disable-vsync -d "Disable vertical synchronization, even if enabled in the project settings"
complete -c fade -l disable-render-loop -d "Disable render loop so rendering only occurs when called explicitly from script"
complete -c fade -l disable-crash-handler -d "Disable crash handler when supported by the platform code"
complete -c fade -l fixed-fps -d "Force a fixed number of frames per second (this setting disables real-time synchronization)" -x
complete -c fade -l delta-smoothing -d "Enable or disable frame delta smoothing" -x -a "enable disable"
complete -c fade -l print-fps -d "Print the frames per second to the stdout"
complete -c fade -l single-threaded-scene -d "Force scene tree to run in single-threaded mode"
complete -c fade -l editor-pseudolocalization -d "Enable pseudolocalization for the editor and the project manager"

# Standalone tools:
complete -c fade -s s -l script -d "Run a script" -r
complete -c fade -l main-loop -d "Run a MainLoop specified by its global class name" -x
complete -c fade -l check-only -d "Only parse for errors and quit (use with --script)"
complete -c fade -l import -d "Start the editor, wait for any resources to be imported, and then quit"
complete -c fade -l export-release -d "Export the project in release mode using the given preset and output path" -x
complete -c fade -l export-debug -d "Export the project in debug mode using the given preset and output path" -x
complete -c fade -l export-pack -d "Export the project data only as a PCK or ZIP file using the given preset and output path" -x
complete -c fade -l export-patch -d "Export pack with changed files only, using the given preset and output path" -x
complete -c fade -l patches -d "List of patches to use with --export-patch (comma-separated)" -x
complete -c fade -l install-android-build-template -d "Install the Android build template (use with --export-release or --export-debug)"
complete -c fade -l convert-3to4 -d "Converts project from FADE 3.x to FADE 4.x"
complete -c fade -l validate-conversion-3to4 -d "Shows what elements will be renamed when converting project from FADE 3.x to FADE 4.x"
complete -c fade -l doctool -d "Dump the engine API reference to the given path (defaults to current directory) in XML format, merging if existing files are found" -r
complete -c fade -l no-docbase -d "Disallow dumping the base types (used with --doctool)"
complete -c fade -l gdextension-docs -d "Generate API reference from all the GDExtensions loaded in the current project (used with --doctool)"
complete -c fade -l gdscript-docs -d "Generate API reference from the inline documentation in the GDScript files found in the given path (used with --doctool)" -r
complete -c fade -l build-solutions -d "Build the scripting solutions (e.g. for C# projects)"
complete -c fade -l dump-gdextension-interface -d "Generate GDExtension header file 'gdextension_interface.h' in the current folder. This file is the base file required to implement a GDExtension"
complete -c fade -l dump-gdextension-interface-json -d "Generate JSON dump of the GDExtension interface named 'gdextension_interface.json' in the current folder"
complete -c fade -l dump-extension-api -d "Generate JSON dump of the FADE API for GDExtension bindings named 'extension_api.json' in the current folder"
complete -c fade -l dump-extension-api-with-docs -d "Generate JSON dump of the FADE API for GDExtension bindings, including documentation"
complete -c fade -l validate-extension-api -d "Validate an extension API file dumped from a previous version of the engine to ensure API compatibility" -r
complete -c fade -l benchmark -d "Benchmark the run time and print it to console"
complete -c fade -l benchmark-file -d "Benchmark the run time and save it to a given file in JSON format" -r
complete -c fade -l test -d "Run all unit tests; run with '--test --help' for more information"
