.PHONY: test
PAIRS := sample.txt:10 input.txt:1000

test:
	dotnet build -c Debug
	$(foreach p,$(PAIRS), \
		$(eval INPUT=$(word 1,$(subst :, ,$p))) \
		$(eval ARG=$(word 2,$(subst :, ,$p))) \
		./bin/Debug/net10.0/Day08 $(INPUT) $(ARG); \
	)
