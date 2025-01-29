@echo off

:: sets this folder as the current directory
cd /D "%~dp0"
:: (re)install the current version of the template (DayTemplate refers to the folder name, which contains the template)
dotnet new install DayTemplate --force

:: Prompt the user for variables to use in the template
set /P DayNumber=Day Number: 
set /P Name=Challenge Name: 

:: Execute the template
dotnet new bb.aoc.day --DayNumber %DayNumber% --Name "%Name%"

:: add the new files to git
git add .

:: pause at the end so that the user can see the output
pause