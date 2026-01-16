#Requires AutoHotkey v1.1.17+
;==============================================================
; localeNameToLCID — Converts a locale name to an LCID via LocaleNameToLCID
;
; GitHub: https://github.com/SevenKeyboard/locale-name-to-lcid
; Author: SevenKeyboard Ltd. (2026)
; License: The Unlicense
;
; Documentation / References:
;   LocaleNameToLCID function (winnls.h)
;     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-localenametolcid
;   GetSystemDefaultLCID function (winnls.h)
;     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-getsystemdefaultlcid
;   GetUserDefaultLCID function (winnls.h)
;     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-getuserdefaultlcid
;==============================================================
class VersionManager_localeNameToLCID
{
    static _ := VersionManager_localeNameToLCID._init()
    _init()    {
        global
        LOCALENAMETOLCID_VERSION := "1.0.0"
    }
}
localeNameToLCID(lpName, dwFlags:=0)    {
    static LOCALE_ALLOW_NEUTRAL_NAMES:=0x08000000
    return format("0x{:04X}", dllCall("Kernel32.dll\LocaleNameToLCID", "Str",lpName, "UInt",(dwFlags=="LOCALE_ALLOW_NEUTRAL_NAMES"?%dwFlags%:0), "UInt"))
}