# https://github.com/fadeengine/fade/issues/66675

func example(thing):
	print(thing.has_method("asdf"))

func test():
	example(Node2D)
