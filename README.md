# CSharp8Demo

Simple console app used to test new C# 8 features.

## Prerequisites (while still in preview)

1. Install Visual Studio 2019 and upgrade to latest version
2. Install latest version of [.NET Core 3](https://dotnet.microsoft.com/download/dotnet-core/3.0)
3. Activate preview versions of .NET Core in VS:
    - Options > Project and Solutions > .NET Core > ☑️ Use previews of the .NET Core SDK
4. In project properties make sure
    - Application > Target framework is .NET Core 3
    - Build > Advanced... > Language version is `unsupported preview of next C# version (preview)` or `C# 8.0 (beta)`
    
## Resources

- [What's new in C# 8.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8) - Microsoft's guide.
- [Nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references)
- [Pattern matching tutorial with switch expressions](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/pattern-matching)
- [dotnet/csharplang](https://github.com/dotnet/csharplang) - Official repo for the design of the C# programming language
- [dotnet/roslyn](https://github.com/dotnet/roslyn) - C# compiler repo
