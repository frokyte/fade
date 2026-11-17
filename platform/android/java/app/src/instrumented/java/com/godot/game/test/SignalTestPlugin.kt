package com.godot.game.test

import org.godotengine.godot.FADE
import org.godotengine.godot.plugin.FADEPlugin
import org.godotengine.godot.plugin.SignalInfo
import org.godotengine.godot.plugin.UsedByFADE

class SignalTestPlugin(godot: FADE) : FADEPlugin(godot) {

	companion object {
		private val EMISSION_TEST_SIGNAL = SignalInfo("emission_test_signal")
		private val LAUNCH_TESTS_SIGNAL = SignalInfo("launch_tests", java.lang.Boolean::class.java, String::class.java)
	}

	override fun getPluginName() = "SignalTestPlugin"

	override fun getPluginSignals(): Set<SignalInfo?> {
		return setOf(
			EMISSION_TEST_SIGNAL,
			LAUNCH_TESTS_SIGNAL
		)
	}

	@UsedByFADE
	fun triggerTestSignal1() {
		emitSignal(EMISSION_TEST_SIGNAL)
	}

	@UsedByFADE
	fun triggerLaunchTestSignal() {
		emitSignal(LAUNCH_TESTS_SIGNAL, true, "second message")
	}
}
