Imports System.Runtime.InteropServices

Public Class MemoryScanner

    Public Shared Function ReadFloat(hProcess As IntPtr, address As IntPtr) As Single
        Dim result As Single = 0
        Dim bytesRead As IntPtr

        If WinApi.ReadProcessMemory(hProcess, address, result, New IntPtr(4), bytesRead) Then
            Return result
        End If
        Return 0.0F
    End Function

    Public Shared Function ReadInt32(hProcess As IntPtr, address As IntPtr) As Integer
        Dim result As Integer = 0
        Dim bytesRead As IntPtr

        If WinApi.ReadProcessMemory(hProcess, address, result, New IntPtr(4), bytesRead) Then
            Return result
        End If
        Return 0
    End Function

    Public Shared Function GetPointerAddress(hProcess As IntPtr, startAddress As IntPtr, offsets As Integer()) As IntPtr
        Dim currentAddr As IntPtr = startAddress
        Dim ptrVal As Integer = 0
        Dim bytesRead As IntPtr

        For i As Integer = 0 To offsets.Length - 2
            If WinApi.ReadProcessMemory(hProcess, currentAddr, ptrVal, New IntPtr(4), bytesRead) Then
                If ptrVal = 0 Then Return IntPtr.Zero
                currentAddr = New IntPtr(ptrVal + offsets(i))
            Else
                Return IntPtr.Zero
            End If
        Next

        If WinApi.ReadProcessMemory(hProcess, currentAddr, ptrVal, New IntPtr(4), bytesRead) Then
            If ptrVal = 0 Then Return IntPtr.Zero
            Return New IntPtr(ptrVal + offsets(offsets.Length - 1))
        End If

        Return IntPtr.Zero
    End Function

End Class