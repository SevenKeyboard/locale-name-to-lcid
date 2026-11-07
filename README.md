# localeNameToLCID
Convert a locale name to an LCID on Windows 10+ with .NET 10.

```csharp
if (SevenKeyboard.LocaleNameToLcid.TryGet("en-US", out uint lcid))
    Console.WriteLine($"0x{lcid:X4}"); // 0x0409
```

Version 2.0.0 returns a numeric LCID and success separately; failure returns
`false` and `0`. `null` passes Win32's user-default pointer (a C# adaptation);
`""` remains invariant (`0x007F`), and `"!x-sys-default-locale"` selects the system
default. No trimming or locale-name normalization is performed.

`AllowNeutralNames` (`0x08000000`) replaces AHK's exact
`"LOCALE_ALLOW_NEUTRAL_NAMES"` flag token. AHK ignored numeric flags; C# deliberately
accepts only this numeric value. All other values, including combinations with
extra bits, still select zero native flags.

Build: `dotnet build src/SevenKeyboard.LocaleNameToLcid/SevenKeyboard.LocaleNameToLcid.csproj -c Release`.
Smoke: `dotnet run --project tests/SevenKeyboard.LocaleNameToLcid.Smoke -c Release`.
