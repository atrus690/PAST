Imports System.Runtime.InteropServices

Public Class WinApi
    Public Const PROCESS_VM_READ As UInt32 = &H10
    Public Const PROCESS_QUERY_INFORMATION As UInt32 = &H400
    Public Const MEM_COMMIT As UInt32 = &H1000
    Public Const PAGE_READWRITE As UInt32 = &H4

    Public Const INPUT_KEYBOARD As Integer = 1
    Public Const KEYEVENTF_KEYUP As Integer = &H2
    Public Const KEYEVENTF_SCANCODE As Integer = &H8
    Public Const KEYEVENTF_EXTENDEDKEY As Integer = &H1
    Public Const MAPVK_VK_TO_VSC As UInteger = 0

    Public Const VK_SPACE As Integer = &H20
    Public Const VK_UP As Integer = &H26
    Public Const VK_DOWN As Integer = &H28
    Public Const VK_LEFT As Integer = &H25
    Public Const VK_RIGHT As Integer = &H27


    <DllImport("kernel32.dll")>
    Public Shared Function OpenProcess(dwDesiredAccess As UInt32, bInheritHandle As Boolean, dwProcessId As Int32) As IntPtr
    End Function

    <DllImport("kernel32.dll")>
    Public Shared Function ReadProcessMemory(hProcess As IntPtr, lpBaseAddress As IntPtr, lpBuffer As Byte(), nSize As IntPtr, ByRef lpNumberOfBytesRead As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll")>
    Public Shared Function VirtualQueryEx(hProcess As IntPtr, lpAddress As IntPtr, ByRef lpBuffer As MEMORY_BASIC_INFORMATION, dwLength As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Shared Function GetAsyncKeyState(vKey As Integer) As Short
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Public Shared Function SendInput(nInputs As Integer, pInputs As INPUT(), cbSize As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Public Shared Function MapVirtualKey(ByVal uCode As UInteger, ByVal uMapType As UInteger) As UInteger
    End Function

    <DllImport("user32.dll")>
    Public Shared Function GetClientRect(ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("winmm.dll")>
    Public Shared Function timeBeginPeriod(ByVal uPeriod As UInteger) As UInteger
    End Function

    <DllImport("winmm.dll")>
    Public Shared Function timeEndPeriod(ByVal uPeriod As UInteger) As UInteger
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Public Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure INPUT
        Public type As Integer
        Public ui As INPUT_UNION
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Public Structure INPUT_UNION
        <FieldOffset(0)> Public ki As KEYBDINPUT
        <FieldOffset(0)> Public mi As MOUSEINPUT
        <FieldOffset(0)> Public hi As HARDWAREINPUT
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure KEYBDINPUT
        Public wVk As Short
        Public wScan As Short
        Public dwFlags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure MOUSEINPUT
        Public dx As Integer : Public dy As Integer : Public mouseData As Integer
        Public dwFlags As Integer : Public time As Integer : Public dwExtraInfo As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure HARDWAREINPUT
        Public uMsg As Integer : Public wParamL As Short : Public wParamH As Short
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure MEMORY_BASIC_INFORMATION
        Public BaseAddress As IntPtr : Public AllocationBase As IntPtr : Public AllocationProtect As UInt32
        Public RegionSize As IntPtr : Public State As UInt32 : Public Protect As UInt32 : Public lType As UInt32
    End Structure

    <DllImport("user32.dll")>
    Public Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Shared Function GetWindowThreadProcessId(ByVal hWnd As IntPtr, ByRef lpdwProcessId As Integer) As Integer
    End Function
End Class