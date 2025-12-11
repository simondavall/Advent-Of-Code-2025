INPUTS=sample.txt sample2.txt input.txt

.PHONY: test

test: Program.cs $(INPUTS)
	dotnet build -c Debug && ./bin/Debug/net10.0/Day11 $(INPUTS)
