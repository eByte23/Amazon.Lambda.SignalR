cd .\src\ServerlessTest\
dotnet restore
dotnet lambda package --configuration release --framework netcoreapp2.1 --output-package ..\..\build\hello.zip

