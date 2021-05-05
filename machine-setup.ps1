# To run this script directly, open PowerShell and run this command:
#     PS> Invoke-WebRequest -UseBasicParsing https://raw.githubusercontent.com/JayBazuzi/sudoku-kata/master/machine-setup.ps1 | Invoke-Expression

#Requires -RunAsAdministrator

iwr -useb https://raw.githubusercontent.com/JayBazuzi/machine-setup/main/windows.ps1 | iex
iwr -useb https://raw.githubusercontent.com/JayBazuzi/machine-setup/main/visual-studio.ps1 | iex

mkdir C:\Source\
pushd C:\Source\
  & "C:\Program Files\Git\cmd\git.exe" clone https://github.com/JayBazuzi/sudoku-kata.git
  pushd sudoku-kata
    github .
    & "C:\Program Files\dotnet\dotnet.exe" test SudokuKata/SudokuKata.sln
    & "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\devenv.exe" SudokuKata/SudokuKata.sln
    # force prompt for credentials
    & 'C:\Program Files\Git\cmd\git.exe' push
  popd
popd
