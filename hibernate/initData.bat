@echo off
setlocal
set BASEDIR=%CD%
set DEP=%BASEDIR%\target\dependency\*
set HIBDATA=%BASEDIR%\data\airline.dat
set CLPATH=%CLASSPATH%;%DEP%
REM echo CLPATH=%CLPATH%
cd target\classes
java -cp "%CLPATH%" com/actian/example/InitData
cd ..\..
endlocal
