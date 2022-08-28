// real world example: https://github.com/dotnet/aspnetcore/blob/main/src/Http/Http/src/HttpContextAccessor.cs
// best to avoid AsyncLocal as much as possible: https://twitter.com/davidfowl/status/1563935285754593281 

using Spectre.Console;

var asyncLocal = new AsyncLocal<string>();
asyncLocal.Value = "1";

AnsiConsole.MarkupLine($"[purple]In root, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
await OuterAsync();

AnsiConsole.MarkupLine($"[purple]In root, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
await OuterAsync();

AnsiConsole.MarkupLine($"[purple]In root, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
Outer();
AnsiConsole.MarkupLine($"[purple]In root, AsyncLocal value is \"{asyncLocal.Value}\"[/]");

async Task OuterAsync()
{
    AnsiConsole.MarkupLine($"[teal]In {nameof(OuterAsync)}, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
    asyncLocal.Value = "2";
    await InnerAsync();
    AnsiConsole.MarkupLine($"[teal]In {nameof(OuterAsync)}, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
}

async Task InnerAsync()
{
    AnsiConsole.MarkupLine($"[yellow]In {nameof(InnerAsync)}, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
    asyncLocal.Value = "3";
    await Task.Yield();
    AnsiConsole.MarkupLine($"[yellow]In {nameof(InnerAsync)}, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
}

void Outer()
{
    AnsiConsole.MarkupLine($"[teal]In {nameof(Outer)}, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
    asyncLocal.Value = "2";
    Inner();
    AnsiConsole.MarkupLine($"[teal]In {nameof(Outer)}, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
}

void Inner()
{
    AnsiConsole.MarkupLine($"[yellow]In {nameof(Inner)}, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
    asyncLocal.Value = "3";
    AnsiConsole.MarkupLine($"[yellow]In {nameof(Inner)}, AsyncLocal value is \"{asyncLocal.Value}\"[/]");
}
