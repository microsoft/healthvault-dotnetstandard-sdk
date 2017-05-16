# Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## C# Coding Style

Our code style is largely borrowed from the [CoreFX C# Coding Style](https://github.com/dotnet/corefx/blob/eb883d78defbc7d4cae3b8ebd0fa68852eb583e3/Documentation/coding-guidelines/coding-style.md), which should hopefully privide a well-known base familiar to the community at large.  Like CoreFX, we have also included a Visual Studio settings file in our repo (/OneSDK.vssettings) that you can use to import many of the relevant settings into Visual Studio to help you follow the following coding style.  It is based on the one provided by the CoreFX team, though ours has been modified slightly to match the newline and indentation conventions in our codebase.

The general rule we follow is "use Visual Studio defaults".
1.	We use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces, where each brace begins on a new line. A single line statement block can go without braces but the block must be properly indented on its own line and it must not be nested in other statement blocks that use braces.
2.	We use four spaces of indentation (no tabs).
3.	We use `_camelCase` for internal and private fields and use `readonly` where possible. Prefix instance fields with `_`, static fields with `s_` and thread static fields with `t_`.  When used on static fields, `readonly` should come after `static` (i.e. `static readonly` not `readonly static`).
4.	We avoid `this.` unless absolutely necessary.
5.	We always specify the visibility, even if it's the default (i.e. `private string _foo` not `string _foo`). Visibility should be the first modifier (i.e. `public abstract` not `abstract public`).
6.	Namespace imports should be specified at the top of the file, outside of `namespace` declarations and should be sorted alphabetically, with System namespaces sorted before others.
7.	Avoid more than one empty line at any time. For example, do not have two blank lines between members of a type.  Comments should be preceded by an empty line.  Closing braces should be followed by an empty line unless followed by another closing brace or a paired statement (e.g., `if`/`else` or `try`/`catch`/`finally`).
8.	Avoid spurious free spaces. For example avoid `if (someVar == 0)...`, where the dots mark the spurious free spaces. Consider enabling "View White Space (Ctrl+E, S)" if using Visual Studio, to aid detection.
9.	If a file happens to differ in style from these guidelines (e.g. private members are named `m_member` rather than `_member`), the existing style in that file takes precedence.
10.	We only use var when it's obvious what the variable type is (i.e. `var stream = new FileStream(...)` not `var stream = OpenStandardInput()`).
11.	We use language keywords instead of BCL types (i.e. `int`, `string`, `float` instead of `Int32`, `String`, `Single`, etc) for both type references as well as method calls (i.e. `int.Parse` instead of `Int32.Parse`).
12.	We use PascalCasing to name all our constant local variables and fields. The only exception is for interop code where the constant value should exactly match the name and value of the code you are calling via interop.
13.	We use `nameof(...)` instead of `"..."` whenever possible and relevant.
