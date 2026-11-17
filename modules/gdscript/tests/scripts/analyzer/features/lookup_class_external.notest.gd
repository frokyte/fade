class A:
	const TARGET: = "wrong"

	class B:
		const TARGET: = "wrong"
		const WAITING: = "fade"

		class D extends C:
			pass

class C:
	const TARGET: = "right"

class E extends A.B.D:
	pass
