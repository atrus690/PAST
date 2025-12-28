Imports System.Text

Public Class ConfigManager
    Private Declare Function WritePrivateProfileString Lib "KERNEL32.DLL" Alias "WritePrivateProfileStringA" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer
    Private Declare Function GetPrivateProfileString Lib "KERNEL32.DLL" Alias "GetPrivateProfileStringA" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer

    Private Const INI_PATH As String = ".\Settings.ini"
    Private Const APP_NAME As String = "PAST"

    Public Shared Function GetValue(KeyName As String, Defaults As String) As String
        Dim strResult As String = Space(255)
        Call GetPrivateProfileString(APP_NAME, KeyName, Defaults, strResult, Len(strResult), INI_PATH)
        Return strResult.Substring(0, InStr(strResult, Chr(0)) - 1)
    End Function

    Public Shared Sub SetValue(KeyName As String, Param As String)
        Call WritePrivateProfileString(APP_NAME, KeyName, Param, INI_PATH)
    End Sub
End Class