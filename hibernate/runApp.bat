@echo off
setlocal
set BASEDIR=%CD%
set DEP=%BASEDIR%\target\dependency\*
set CLPATH=%CLASSPATH%;%DEP%
cd target\classes
java -cp "%CLPATH%" com/actian/example/App
cd ..\..
endlocal
