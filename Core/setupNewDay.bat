:: (re)install the current version of the template 
cd /D "%~dp0"
dotnet new install DayTemplate --force


@echo off

:: Prompt the user for DayNumber
set /P DayNumber=Day Number: 

:: Prompt the user for Name
set /P Name=Challenge Name: 

:: Run the dotnet command using the user-supplied values
dotnet new bb.aoc.day --DayNumber %DayNumber% --Name "%Name%"

:: pause at the end so that the user can see the output
pause