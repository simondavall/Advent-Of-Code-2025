INPUTS=sample.txt input.txt

.PHONY: test

test: Program.cs $(INPUTS)
	dotnet build -c Debug && ./bin/Debug/net10.0/Day10 $(INPUTS)
