.PHONY: test

test:
	cd Aoc && dotnet build -c Release && ./bin/Release/net10.0/Aoc
