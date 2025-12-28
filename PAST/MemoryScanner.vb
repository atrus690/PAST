Imports System.Runtime.InteropServices

Public Class MemoryScanner

    Public Shared Function ScanForValue(hProcess As IntPtr, targetValue As Int32, startAddress As Long, endAddress As Long) As List(Of IntPtr)
        Dim foundList As New List(Of IntPtr)
        Dim mbi As New WinApi.MEMORY_BASIC_INFORMATION()
        Dim address As Long = startAddress

        While WinApi.VirtualQueryEx(hProcess, New IntPtr(address), mbi, New IntPtr(Marshal.SizeOf(mbi))) <> IntPtr.Zero
            If mbi.State = WinApi.MEM_COMMIT AndAlso (mbi.Protect And &H44) <> 0 Then
                Dim buffer(mbi.RegionSize.ToInt64() - 1) As Byte
                Dim bytesRead As IntPtr

                If WinApi.ReadProcessMemory(hProcess, mbi.BaseAddress, buffer, mbi.RegionSize, bytesRead) Then
                    For i As Integer = 0 To buffer.Length - 4 Step 4
                        If BitConverter.ToInt32(buffer, i) = targetValue Then
                            foundList.Add(New IntPtr(mbi.BaseAddress.ToInt64() + i))
                        End If
                    Next
                End If
            End If

            address = mbi.BaseAddress.ToInt64() + mbi.RegionSize.ToInt64()
            If address >= endAddress Then Exit While
        End While

        Return foundList
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