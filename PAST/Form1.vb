Imports System.Runtime.InteropServices
Imports System.Threading

Public Class Form1

#Region "API and Structure Definitions"
    <DllImport("kernel32.dll")>
    Private Shared Function OpenProcess(dwDesiredAccess As UInt32, bInheritHandle As Boolean, dwProcessId As Int32) As IntPtr
    End Function
    <DllImport("kernel32.dll")>
    Private Shared Function ReadProcessMemory(hProcess As IntPtr, lpBaseAddress As IntPtr, lpBuffer As Byte(), nSize As IntPtr, ByRef lpNumberOfBytesRead As IntPtr) As Boolean
    End Function
    <DllImport("kernel32.dll")>
    Private Shared Function VirtualQueryEx(hProcess As IntPtr, lpAddress As IntPtr, ByRef lpBuffer As MEMORY_BASIC_INFORMATION, dwLength As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetAsyncKeyState(vKey As Integer) As Short
    End Function
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SendInput(nInputs As Integer, pInputs As INPUT(), cbSize As Integer) As Integer
    End Function
    <DllImport("user32.dll", SetLastError:=True)>
    Public Shared Function MapVirtualKey(ByVal uCode As UInteger, ByVal uMapType As UInteger) As UInteger
    End Function

    <DllImport("winmm.dll")>
    Private Shared Function timeBeginPeriod(ByVal uPeriod As UInteger) As UInteger
    End Function
    <DllImport("winmm.dll")>
    Private Shared Function timeEndPeriod(ByVal uPeriod As UInteger) As UInteger
    End Function

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

    Private Const INPUT_KEYBOARD As Integer = 1
    Private Const KEYEVENTF_KEYUP As Integer = &H2
    Private Const KEYEVENTF_SCANCODE As Integer = &H8
    Private Const KEYEVENTF_EXTENDEDKEY As Integer = &H1
    Private Const MAPVK_VK_TO_VSC As UInteger = 0

    Private Const VK_SPACE As Integer = &H20
    Private Const VK_UP As Integer = &H26
    Private Const VK_DOWN As Integer = &H28
    Private Const VK_LEFT As Integer = &H25
    Private Const VK_RIGHT As Integer = &H27
#End Region

#Region "Variable Declaration"
    Private Const PROCESS_VM_READ As UInt32 = &H10
    Private Const PROCESS_QUERY_INFORMATION As UInt32 = &H400
    Private Const MEM_COMMIT As UInt32 = &H1000
    Private Const PAGE_READWRITE As UInt32 = &H4

    Private _targetAddress As IntPtr = IntPtr.Zero
    Private _hProcess As IntPtr = IntPtr.Zero
    Private _isMonitoring As Boolean = False
    Private _cts As CancellationTokenSource
    Private _monitoringTask As Task

    Private Const MEM_0_PERCENT As Single = 175.0F
    Private Const MEM_100_PERCENT As Single = 625.0F
    Private Const VALUE_RANGE As Single = 450.0F
#End Region

#Region "Initialization Process"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckAuthenticationAndLoadSettings()
        Textset()
    End Sub

    ' Force termination of processes at shutdown
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            ' 1. Just in case
            If _cts IsNot Nothing Then _cts.Cancel()

            ' 2. Save Settings
            SaveSettings()
            timeEndPeriod(1)
        Catch
        Finally
            ' Force termination of each process
            Environment.Exit(0)
        End Try
    End Sub

    Private Sub CheckAuthenticationAndLoadSettings()

        Txt_pangya.Text = GetIni("AutoShot", "PangYakey", "49", ".\Settings.ini")
        Txt_power.Text = GetIni("AutoShot", "Powerkey", "50", ".\Settings.ini")
        SetDesktopLocation(CInt(GetIni("AutoShot", "Form.X", "0", ".\Settings.ini")),
                           CInt(GetIni("AutoShot", "Form.Y", "0", ".\Settings.ini")))
    End Sub

    Private Sub SaveSettings()
        PutIni("AutoShot", "Form.X", Location.X.ToString(), ".\Settings.ini")
        PutIni("AutoShot", "Form.Y", Location.Y.ToString(), ".\Settings.ini")
        PutIni("AutoShot", "PangYakey", Txt_pangya.Text, ".\Settings.ini")
        PutIni("AutoShot", "Powerkey", Txt_power.Text, ".\Settings.ini")
    End Sub

    ' INI file manipulation
    Declare Function WritePrivateProfileString Lib "KERNEL32.DLL" Alias "WritePrivateProfileStringA" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer
    Declare Function GetPrivateProfileString Lib "KERNEL32.DLL" Alias "GetPrivateProfileStringA" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer

    Public Function GetIni(ByVal ApName As String, ByVal KeyName As String, ByVal Defaults As String, ByVal Filename As String) As String
        Dim strResult As String = Space(255)
        Call GetPrivateProfileString(ApName, KeyName, Defaults, strResult, Len(strResult), Filename)
        Return strResult.Substring(0, InStr(strResult, Chr(0)) - 1)
    End Function

    Public Sub PutIni(ByVal ApName As String, ByVal KeyName As String, ByVal Param As String, ByVal Filename As String)
        Call WritePrivateProfileString(ApName, KeyName, Param, Filename)
    End Sub
#End Region

#Region "Monitoring"
    Private Async Sub Btn_start_Click(sender As Object, e As EventArgs) Handles Btn_start.Click
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime

        Dim procs As Process() = Process.GetProcessesByName("ProjectG")
        If procs.Length = 0 Then
            MsgBox("ProjectG not found.")
            Return
        End If

        _hProcess = OpenProcess(PROCESS_VM_READ Or PROCESS_QUERY_INFORMATION, False, procs(0).Id)
        _targetAddress = FastScan(_hProcess, 1124286464)

        If _targetAddress <> IntPtr.Zero Then
            Lb_state.Text = "Start"
            Lb_state.ForeColor = Color.Blue
            Btn_start.Enabled = False
            Btn_stop.Enabled = True
            timeBeginPeriod(1)
            _isMonitoring = True
            _cts = New CancellationTokenSource()

            ' Monitoring Start
            _monitoringTask = Task.Run(Sub() MonitoringLoop(_cts.Token))
            Await _monitoringTask

            ' Post-stop processing
            _isMonitoring = False
            ' If the form is still active, refresh the display
            If Not Me.IsDisposed Then
                Lb_state.Text = "Stop"
                Lb_state.ForeColor = Color.Red
                Btn_start.Enabled = True
                Btn_stop.Enabled = False
            End If
        Else
            MsgBox("Error
1. Launch with administrator privileges.
2. Enter the course.
3. Select a club other than PT.")
        End If
    End Sub

    Private Sub Btn_stop_Click(sender As Object, e As EventArgs) Handles Btn_stop.Click
        StopMonitoring()
    End Sub

    Private Sub StopMonitoring()
        If _isMonitoring Then
            _cts?.Cancel()
            _isMonitoring = False
            timeEndPeriod(1)
            Lb_state.Text = "Stop"
            Lb_state.ForeColor = Color.Red
            Btn_start.Enabled = True
            Btn_stop.Enabled = False
        End If
    End Sub

    Private Sub SafeInvoke(action As Action)
        Try
            If Not Me.IsDisposed AndAlso Me.IsHandleCreated Then
                Me.Invoke(action)
            End If
        Catch
            ' All errors are ignored
        End Try
    End Sub

    Private Sub MonitoringLoop(ByVal token As CancellationToken)
        Dim hasTargetFired As Boolean = False
        Dim hasReturnFired As Boolean = False
        Dim hasMidFired As Boolean = False
        Dim targetPercent As Double = 100.0
        Dim buffer(3) As Byte
        Dim bytesRead As IntPtr
        Dim keyPangya As Integer = 0
        Dim keyPower As Integer = 0

        SafeInvoke(Sub()
                       keyPangya = CInt(Txt_pangya.Text)
                       keyPower = CInt(Txt_power.Text)
                   End Sub)

        Do
            If token.IsCancellationRequested Then Exit Do
            If Me.IsDisposed Then Exit Do

            Dim isPowerPressed As Boolean = (GetAsyncKeyState(keyPower) And &H8000) <> 0
            Dim isPangyaPressed As Boolean = (GetAsyncKeyState(keyPangya) And &H8000) <> 0

            SafeInvoke(Sub() Double.TryParse(Txt_p.Text, targetPercent))

            If ReadProcessMemory(_hProcess, _targetAddress, buffer, New IntPtr(4), bytesRead) Then
                Dim currentFloat As Single = BitConverter.ToSingle(buffer, 0)
                Dim currentPercent As Double = (currentFloat - MEM_0_PERCENT) / VALUE_RANGE * 100.0

                ' Outbound
                If isPowerPressed Then
                    If currentPercent >= targetPercent AndAlso Not hasTargetFired Then
                        ExecuteSingleKeyAsync(VK_SPACE, 130)
                        hasTargetFired = True
                    End If
                Else
                    hasTargetFired = False
                End If

                ' Return
                If isPangyaPressed Then
                    If Not hasMidFired AndAlso currentPercent < 50.0 AndAlso currentPercent > 10.0 Then
                        Dim cmd As String = "", spn As String = ""
                        SafeInvoke(Sub()
                                       cmd = Cmb_tok.Text
                                       spn = Cmb_spca.Text
                                   End Sub)
                        ExecuteFullSequence(cmd, spn)
                        hasMidFired = True
                    End If

                    ' 0% Impact
                    If currentPercent <= 0.0 AndAlso Not hasReturnFired Then
                        ExecuteSingleKeyAsync(VK_SPACE, 130)
                        hasReturnFired = True

                        SafeInvoke(Sub() Textset())
                    End If
                Else
                    hasReturnFired = False
                    hasMidFired = False
                End If

            End If
            Thread.Sleep(0)
        Loop
    End Sub
#End Region

#Region "Key Inputs and Sequences"
    Private Sub ExecuteFullSequence(cmdName As String, spinName As String)
        Task.Run(Sub()
                     ' Command
                     Dim cmdKeys As Integer() = Nothing
                     Select Case cmdName
                         Case "Tomahawk" : cmdKeys = {VK_UP, VK_DOWN}
                         Case "Cobra" : cmdKeys = {VK_RIGHT, VK_UP}
                         Case "Spike" : cmdKeys = {VK_RIGHT, VK_DOWN}
                     End Select
                     If cmdKeys IsNot Nothing Then
                         For Each k In cmdKeys
                             SendKeySync(k, 50) : Thread.Sleep(10)
                         Next
                     End If

                     ' Spin
                     Dim spinKey As Integer = 0
                     Select Case spinName
                         Case "↓" : spinKey = VK_DOWN
                         Case "↑" : spinKey = VK_UP
                         Case "←" : spinKey = VK_LEFT
                         Case "→" : spinKey = VK_RIGHT
                     End Select
                     If spinKey <> 0 Then
                         If cmdKeys IsNot Nothing Then Thread.Sleep(10)
                         SendKeySync(spinKey, 150)
                     End If
                 End Sub)
    End Sub

    Private Sub SendKeySync(vKey As Integer, holdTime As Integer)
        Dim scanCode As Short = CShort(MapVirtualKey(CUInt(vKey), MAPVK_VK_TO_VSC))
        Dim dwFlagsDown As Integer = KEYEVENTF_SCANCODE
        Dim dwFlagsUp As Integer = KEYEVENTF_SCANCODE Or KEYEVENTF_KEYUP

        If vKey >= &H25 AndAlso vKey <= &H28 Then
            dwFlagsDown = dwFlagsDown Or KEYEVENTF_EXTENDEDKEY
            dwFlagsUp = dwFlagsUp Or KEYEVENTF_EXTENDEDKEY
        End If

        Dim inputs(0) As INPUT
        inputs(0).type = INPUT_KEYBOARD
        inputs(0).ui.ki = New KEYBDINPUT With {.wVk = CShort(vKey), .wScan = scanCode, .dwFlags = dwFlagsDown}
        SendInput(1, inputs, Marshal.SizeOf(GetType(INPUT)))

        Thread.Sleep(holdTime)

        inputs(0).ui.ki = New KEYBDINPUT With {.wVk = CShort(vKey), .wScan = scanCode, .dwFlags = dwFlagsUp}
        SendInput(1, inputs, Marshal.SizeOf(GetType(INPUT)))
    End Sub

    Private Sub ExecuteSingleKeyAsync(vKey As Integer, holdTime As Integer)
        Task.Run(Sub() SendKeySync(vKey, holdTime))
    End Sub
#End Region

#Region "Scan Utility"
    Private Function FastScan(hProcess As IntPtr, targetValue As Int32) As IntPtr
        Dim mbi As New MEMORY_BASIC_INFORMATION()
        Dim address As Long = 0
        While VirtualQueryEx(hProcess, New IntPtr(address), mbi, New IntPtr(Marshal.SizeOf(mbi))) <> IntPtr.Zero
            If mbi.State = MEM_COMMIT AndAlso (mbi.Protect And &H44) <> 0 Then
                Dim buffer(mbi.RegionSize.ToInt64() - 1) As Byte
                Dim bytesRead As IntPtr
                If ReadProcessMemory(hProcess, mbi.BaseAddress, buffer, mbi.RegionSize, bytesRead) Then
                    For i As Integer = 0 To buffer.Length - 4 Step 4
                        If BitConverter.ToInt32(buffer, i) = targetValue Then
                            Return New IntPtr(mbi.BaseAddress.ToInt64() + i)
                        End If
                    Next
                End If
            End If
            address = mbi.BaseAddress.ToInt64() + mbi.RegionSize.ToInt64()
            If address >= &H7FFFFFFF Then Exit While
        End While
        Return IntPtr.Zero
    End Function

    Private Sub Textset()
        Cmb_spca.Text = "None"
        Cmb_tok.Text = "None"
    End Sub

#End Region

End Class