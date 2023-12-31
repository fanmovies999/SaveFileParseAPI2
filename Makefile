export SERVER ?= localhost
export PORT ?= 5000

test:
	mkdir -p output
	curl --location 'http://${SERVER}:${PORT}/getRawDatabaseImage'  --form "file=@"../savegame/HL-01-00.sav"" --output output/HL-01-00.rdi.sqlite
	curl --location 'http://${SERVER}:${PORT}/getRawExclusiveImage' --form "file=@"../savegame/HL-01-00.sav"" --output output/HL-01-00.rei.sqlite

test_loop:
	bash -c '[ ! -d output ] && mkdir output; \
		while true; do \
			r=$$(( RANDOM % 15 )); \
			echo $$r ; \
			curl --location "http://${SERVER}:${PORT}/getRawDatabaseImage" \
				-s \
				-o output/HL-01-$$(printf "%02d" $$r).rdi.sqlite \
				-F file=@../savegame/HL-01-$$(printf "%02d" $$r).sav ;\
			[ $$? -ne 0 ] && break ; \
			sleep 1s; \
		done \
		'
