# https://github.com/fadeengine/fade/issues/54589
# https://github.com/fadeengine/fade/issues/56265

extends Resource

func test():
	print("okay")
	await self.changed
	await unknown(self)

func unknown(arg):
	await arg.changed
