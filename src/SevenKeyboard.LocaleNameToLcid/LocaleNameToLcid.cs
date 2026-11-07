//==============================================================
// LocaleNameToLcid — Converts a locale name to an LCID via LocaleNameToLCID
//
// GitHub: https://github.com/SevenKeyboard/locale-name-to-lcid
// Author: SevenKeyboard Ltd. (2026)
// License: The Unlicense
//
// Documentation / References:
//   LocaleNameToLCID function (winnls.h)
//     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-localenametolcid
//   GetSystemDefaultLCID function (winnls.h)
//     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-getsystemdefaultlcid
//   GetUserDefaultLCID function (winnls.h)
//     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-getuserdefaultlcid
//==============================================================
using System.Runtime.InteropServices;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]

namespace SevenKeyboard;

public static partial class LocaleNameToLcid
{
    public const uint AllowNeutralNames = 0x08000000;

    /// <summary>Converts a locale name to an LCID; returns false and zero on native failure.</summary>
    /// <remarks>Null selects the user default; an empty string selects the invariant locale.</remarks>
    public static bool TryGet(string? localeName, out uint lcid, uint flags = 0)
    {
        // The numeric constant replaces AHK's exact "LOCALE_ALLOW_NEUTRAL_NAMES" token.
        // Other values, including combinations with extra bits, remain ignored.
        uint nativeFlags = flags == AllowNeutralNames ? AllowNeutralNames : 0;
        lcid = NativeLocaleNameToLCID(localeName, nativeFlags);
        return lcid != 0;
    }

    [LibraryImport("kernel32.dll", EntryPoint = "LocaleNameToLCID", StringMarshalling = StringMarshalling.Utf16)]
    private static partial uint NativeLocaleNameToLCID(string? localeName, uint flags);
}
