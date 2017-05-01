REM Name: Release.Appleseed.Search.Index.Service
REM Author : Rahul Singh
REM Date : 2/21/2016
REM Purpose : Creates a zip file and moves it to the Framework Directory. Doesn't yet version it. 

cd ..\Appleseed.Services.Search.Console\bin\
7za a IndexService.zip Release  
move IndexService.zip ..\..\..\..\Appleseed.Framework\Binaries\Appleseed.Search\