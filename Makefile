export PORT ?= 5126

test:
	curl --location 'http://localhost:${PORT}/getRawDatabaseImage' --form "file=@"../savegame/HL-01-00.sav"" --output HL-01-00.rdi.sqlite

test_loop:
	bash -c '[ ! -d output ] && mkdir output; \
		while true; do \
			r=$$(( RANDOM % 15 )); \
			echo $$r ; \
			curl --location "http://localhost:${PORT}/getRawDatabaseImage" \
				-s \
				-o output/HL-01-$$(printf "%02d" $$r).rdi.sqlite \
				-F file=@../savegame/HL-01-$$(printf "%02d" $$r).sav ;\
			sleep 1s; \
		done \
		'
