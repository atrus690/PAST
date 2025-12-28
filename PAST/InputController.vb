Imports System.Runtime.InteropServices
Imports System.Threading

Public Class InputController

    Public Shared Sub ExecuteSingleKeyAsync(vKey As Integer, holdTime As Integer)
        Task.Run(Sub() SendKeySync(vKey, holdTime))
    End Sub

    Public Shared Sub ExecuteFullSequence(cmdName As String, spinName As String)
        Task.Run(Sub()
                     Dim cmdKeys As Integer() = Nothing
                     Select Case cmdName
                         Case "Tomahawk" : cmdKeys = {WinApi.VK_UP, WinApi.VK_DOWN}
                         Case "Cobra" : cmdKeys = {WinApi.VK_RIGHT, WinApi.VK_UP}
                         Case "Spike" : cmdKeys = {WinApi.VK_RIGHT, WinApi.VK_DOWN}
                     End Select

                     If cmdKeys IsNot Nothing Then
                         For Each k In cmdKeys
                             SendKeySync(k, 50) : Thread.Sleep(10)
                         Next
                     End If

                     Dim spinKey As Integer = 0
                     Select Case spinName
                         Case "↓" : spinKey = WinApi.VK_DOWN
                         Case "↑" : spinKey = WinApi.VK_UP
                         Case "←" : spinKey = WinApi.VK_LEFT
                         Case "→" : spinKey = WinApi.VK_RIGHT
                     End Select

                     If spinKey <> 0 Then
                         If cmdKeys IsNot Nothing Then Thread.Sleep(10)
                         SendKeySync(spinKey, 150)
                     End If
                 End Sub)
    End Sub

    Private Shared Sub SendKeySync(vKey As Integer, holdTime As Integer)
        Dim scanCode As Short = CShort(WinApi.MapVirtualKey(CUInt(vKey), WinApi.MAPVK_VK_TO_VSC))
        Dim dwFlagsDown As Integer = WinApi.KEYEVENTF_SCANCODE
        Dim dwFlagsUp As Integer = WinApi.KEYEVENTF_SCANCODE Or WinApi.KEYEVENTF_KEYUP

        If vKey >= &H25 AndAlso vKey <= &H28 Then
            dwFlagsDown = dwFlagsDown Or WinApi.KEYEVENTF_EXTENDEDKEY
            dwFlagsUp = dwFlagsUp Or WinApi.KEYEVENTF_EXTENDEDKEY
        End If

        Dim inputs(0) As WinApi.INPUT
        inputs(0).type = WinApi.INPUT_KEYBOARD
        inputs(0).ui.ki = New WinApi.KEYBDINPUT With {.wVk = CShort(vKey), .wScan = scanCode, .dwFlags = dwFlagsDown}
        WinApi.SendInput(1, inputs, Marshal.SizeOf(GetType(WinApi.INPUT)))

        Thread.Sleep(holdTime)

        inputs(0).ui.ki = New WinApi.KEYBDINPUT With {.wVk = CShort(vKey), .wScan = scanCode, .dwFlags = dwFlagsUp}
        WinApi.SendInput(1, inputs, Marshal.SizeOf(GetType(WinApi.INPUT)))
    End Sub
End Class