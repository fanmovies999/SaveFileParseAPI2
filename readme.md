# Hogwarts Legacy - Save File Parse API

## Description
The program is using dotnet. It is an asp.net code webserver (with minimalist implementation of webserver).

It exposed 2 endpoints:
- /getRawDatabaseImage : To get the field RawDatabaseImage from the save file, returning it uncompressed.
- /getRawExclusiveImage : To get the field RawExclusiveImage from the save file, returning it uncompressed.

It takes the save file as form, with name file. (with curl : --form "file=@"filename.sav"")

It send as file the content for the field. (with curl --ouput filename.sqlite)

Here is a call sample
```
curl --location 'http://localhost:5000/getRawDatabaseImage'  --form "file=@"HL-01-00.sav"" --output HL-01-00.rdi.sqlite
```

Rem: If the savegame is an old file with RawDatabaseImage and RawExclusiveImage not compressed, the endpoints will just send the content of the field.


## Compile the program and run it

### Install dotnet framework version 7 or upper.

### From VSCODE
Open the folder in VSCODE.
Add dotnet extension.
Add Solution explorer extention.
In the solution explorer, build the solution (to ensure that oodle libraries are copied in the right folder)
Then you could start or debug the program.

### From command line, VSCODE and DOTNET shoudl be installed
From main folder
```
dotnet build
dotnet run --project SaveFileParseAPI2/SaveFileParseAPI2.csproj 
```

### From container
From main folder
```
docker compose build
docker compose up -d
```

## Make some tests.

### curl
```
curl --location 'http://localhost:5000/getRawDatabaseImage'  --form "file=@"HL-01-00.sav"" --output HL-01-00.rdi.sqlite
```

### Makefile (please use Linux to run this script)
Create a folder "savegame" at the same level than this project. Copy save game files in this folder.

In the folder, I put 15 files named  HL-01-XX.sav.

In the make file, I created 2 targets:
- test : will call the 2 endpoints on the file ../savegame/HL-01-00.sav and save the answers in folder output/ , the file name will contain rdi for RawDatabaseImage and rei for RawExclusiveImage)
- test_loop : will call getRawDatabaseImage randomly on the 15 save files. And will save the sqlite file in output/ with file named .rdi.sqlite

Example:
```
make test

make test_loop
```

If you need to change the name of the server or the port, define the varibles before calling make...
```
SERVER=localhost PORT=5000 make test_loop
```
By default SERVER = localhost and PORT = 5000

## Memory management
I hope I did let a memory leak.
In order to avoid consuming to much memory, I included the possibility to stop the web server after STOPCOUNT calls on endpoints.
STOPCOUNT is an environment variable.

If you want server to be up, I woulld advice to use docker (and docker compose)
In docker, set the environment variable STOPCOUNT to 100 and set restart option to "unless-stopped".
It means the 100 call will fail, but the memory will be kept under 100 MB (in my tests).

## Oodle compression librairies
Oodle librairies comes from Unreal SDK.
You could also find them in https://github.com/WorkingRobot/OodleUE.git

## Code inspiration
Code is copied/inspired from:
- [klukule](https://github.com/klukule/SaveFileParseAPI)
- [UnrealEngine.Gvas](https://github.com/SparkyTD/UnrealEngine.Gvas)
- [CUE4Parse](https://github.com/FabianFG/CUE4Parse)

