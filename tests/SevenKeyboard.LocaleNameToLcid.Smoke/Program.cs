using System.Runtime.InteropServices;
using SevenKeyboard;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]

int checks = 0;
uint[] ignoredFlags = [1, 0x04000000, 0x08000001, 0x88000000, uint.MaxValue];

// AHK returned these values with zero flags. Numeric C# AllowNeutralNames replaces
// only the original, case-sensitive "LOCALE_ALLOW_NEUTRAL_NAMES" string token.
(string Name, uint Lcid)[] specific =
[
    ("en-US", 0x0409), ("ja-JP", 0x0411), ("ko-KR", 0x0412),
    ("de-DE_phoneb", 0x10407), ("", 0x007F)
];
(string Name, uint Specific, uint Neutral)[] neutral =
[
    ("en", 0x0409, 0x0009), ("zh-Hans", 0x0804, 0x0004), ("zh-Hant", 0x0C04, 0x7C04)
];

foreach ((string name, uint lcid) in specific)
{
    Check(name, 0, lcid);
    Check(name, LocaleNameToLcid.AllowNeutralNames, lcid);
    foreach (uint flags in ignoredFlags)
        Check(name, flags, lcid);
}

foreach ((string name, uint specificLcid, uint neutralLcid) in neutral)
{
    Check(name, 0, specificLcid);
    Check(name, LocaleNameToLcid.AllowNeutralNames, neutralLcid);
    foreach (uint flags in ignoredFlags)
        Check(name, flags, specificLcid);
}

// Independent default queries avoid assuming the machine's current locale.
uint userDefault = Native.GetUserDefaultLCID();
uint systemDefault = Native.GetSystemDefaultLCID();
Require(userDefault != 0 && systemDefault != 0, "Default LCID queries failed.");
foreach (uint flags in new uint[] { 0, LocaleNameToLcid.AllowNeutralNames, 1, 0x08000001, uint.MaxValue })
{
    Check(null, flags, userDefault);
    Check("!x-sys-default-locale", flags, systemDefault);
    Check("not_a_locale", flags, 0);
    Check("LOCALE_NAME_USER_DEFAULT", flags, 0);
    Check("LOCALE_NAME_SYSTEM_DEFAULT", flags, 0);
}

// Preserve native casing, whitespace, embedded-NUL, and long-input interpretation.
foreach (string name in new[] { "EN-us", " en-US", "en-US ", "en_US", "en-US\0ignored", "\0ignored", new string('x', 1000) })
{
    Check(name, 0, Native.LocaleNameToLCID(name, 0));
    Check(name, LocaleNameToLcid.AllowNeutralNames, Native.LocaleNameToLCID(name, 0x08000000));
}

Require(LocaleNameToLcid.TryGet("en", out uint defaultLcid) && defaultLcid == 0x0409,
    "Omitted flags must resolve a neutral name to its specific LCID.");
Console.WriteLine($"PASS: {checks} checks ({RuntimeInformation.ProcessArchitecture}); system=0x{systemDefault:X4}, user=0x{userDefault:X4}.");

void Check(string? name, uint flags, uint expected)
{
    // Explicit expected LCIDs above come from AHK/native observations, not wrapper round trips.
    uint actual = uint.MaxValue;
    bool success = LocaleNameToLcid.TryGet(name, out actual, flags);
    Require(success == (expected != 0) && actual == expected,
        $"Name [{name ?? "<null>"}], flags 0x{flags:X}: success={success}, LCID=0x{actual:X}, expected=0x{expected:X}.");
}

void Require(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
    checks++;
}

internal static class Native
{
    [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern uint LocaleNameToLCID(string? name, uint flags);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    internal static extern uint GetSystemDefaultLCID();

    [DllImport("kernel32.dll", ExactSpelling = true)]
    internal static extern uint GetUserDefaultLCID();
}
