#!/bin/bash
cat << 'CS_EOF' > TestProject.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="MockTest.cs" />
  </ItemGroup>
</Project>
CS_EOF
# Let's see if we can use dotnet test instead of running the whole unity tests if it's missing pwsh.
# Actually Unity test runner needs Unity.
