Imports System.Runtime.InteropServices

Public Class MemoryScanner

    Public Shared Function GetPointerAddress(hProcess As IntPtr, startAddress As IntPtr, offsets As Integer()) As IntPtr
        Dim currentAddr As IntPtr = startAddress
        Dim buffer(3) As Byte
        Dim bytesRead As IntPtr

        For i As Integer = 0 To offsets.Length - 2
            If WinApi.ReadProcessMemory(hProcess, currentAddr, buffer, New IntPtr(4), bytesRead) Then
                Dim ptrVal As Integer = BitConverter.ToInt32(buffer, 0)
                If ptrVal = 0 Then Return IntPtr.Zero ' 読み取り失敗またはNullポインタ
                currentAddr = New IntPtr(ptrVal + offsets(i))
            Else
                Return IntPtr.Zero
            End If
        Next

        If WinApi.ReadProcessMemory(hProcess, currentAddr, buffer, New IntPtr(4), bytesRead) Then
            Dim ptrVal As Integer = BitConverter.ToInt32(buffer, 0)
            If ptrVal = 0 Then Return IntPtr.Zero
            Return New IntPtr(ptrVal + offsets(offsets.Length - 1))
        End If

        Return IntPtr.Zero
    End Function

    Public Shared Function ReadInt32(hProcess As IntPtr, address As IntPtr) As Integer
        Dim buffer(3) As Byte
        Dim bytesRead As IntPtr
        If WinApi.ReadProcessMemory(hProcess, address, buffer, New IntPtr(4), bytesRead) Then
            Return BitConverter.ToInt32(buffer, 0)
        End If
        Return 0
    End Function

    Public Shared Function ReadFloat(hProcess As IntPtr, address As IntPtr) As Single
        Dim buffer(3) As Byte
        Dim bytesRead As IntPtr
        If WinApi.ReadProcessMemory(hProcess, address, buffer, New IntPtr(4), bytesRead) Then
            Return BitConverter.ToSingle(buffer, 0)
        End If
        Return 0.0F
    End Function
End Class