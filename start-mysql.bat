@echo off
REM ============================================================
REM  Starts MySQL for CPMMS.
REM
REM  This runs the server as YOUR user account, not as a Windows
REM  service, so it needs no administrator rights. Keep this
REM  window open while you use the app; closing it stops MySQL.
REM
REM  The data lives in %LOCALAPPDATA%\CPMMS-MySQL\data and is
REM  completely separate from any older MySQL install on this PC.
REM ============================================================

set "BASEDIR=C:\Program Files\MySQL\MySQL Server 8.0"
set "DATADIR=%LOCALAPPDATA%\CPMMS-MySQL\data"

if not exist "%BASEDIR%\bin\mysqld.exe" (
    echo MySQL Server was not found at:
    echo   %BASEDIR%
    echo Install MySQL Server 8.0 first, or edit BASEDIR in this file.
    pause
    exit /b 1
)

if not exist "%DATADIR%" (
    echo First run - creating the database directory...
    "%BASEDIR%\bin\mysqld.exe" --initialize-insecure --datadir="%DATADIR%" --basedir="%BASEDIR%"
    echo Done. Remember to import database\01_schema.sql and database\02_seed.sql.
)

echo.
echo Starting MySQL on port 3306. Leave this window open.
echo Press Ctrl+C to stop the server.
echo.
"%BASEDIR%\bin\mysqld.exe" --datadir="%DATADIR%" --basedir="%BASEDIR%" --port=3306 --console
pause
