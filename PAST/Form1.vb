Imports System.Threading

Public Class Form1
    Private Const GAUGE_0_VAL As Single = 140.0F
    Private Const GAUGE_100_VAL As Single = 500.0F
    Private Const GAUGE_RANGE As Single = GAUGE_100_VAL - GAUGE_0_VAL

    Private Const BASE_OFFSET As Integer = &HAFB0A0
    Private ReadOnly POINTER_OFFSETS As Integer() = {&H128, &HE8}

    Private _targetAddress As IntPtr = IntPtr.Zero
    Private _hProcess As IntPtr = IntPtr.Zero
    Private _isMonitoring As Boolean = False
    Private _cts As CancellationTokenSource
    Private _monitoringTask As Task
    Private _targetPid As Integer = 0

    Private Function IsGameActive() As Boolean
        If _targetPid = 0 Then Return False

        Dim foregroundWnd As IntPtr = WinApi.GetForegroundWindow()
        Dim foregroundProcId As Integer = 0

        WinApi.GetWindowThreadProcessId(foregroundWnd, foregroundProcId)

        Return (foregroundProcId = _targetPid)
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Txt_pangya.Text = My.Settings.PangyaKey
        Txt_power.Text = My.Settings.PowerKey

        If My.Settings.FormLocation.IsEmpty = False AndAlso
           (My.Settings.FormLocation.X <> 0 OrElse My.Settings.FormLocation.Y <> 0) Then

            Me.StartPosition = FormStartPosition.Manual
            Me.Location = My.Settings.FormLocation
        End If

        Textset()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        StopMonitoring()
        If Me.WindowState = FormWindowState.Normal Then
            My.Settings.FormLocation = Me.Location
        Else
            My.Settings.FormLocation = Me.RestoreBounds.Location
        End If

        My.Settings.PangyaKey = Txt_pangya.Text
        My.Settings.PowerKey = Txt_power.Text

        My.Settings.Save()
        WinApi.timeEndPeriod(1)
        Environment.Exit(0)
    End Sub

    Private Sub Textset()
        Cmb_spca.Text = "None"
        Cmb_tok.Text = "None"
    End Sub

    Private Async Sub Btn_start_Click(sender As Object, e As EventArgs) Handles Btn_start.Click
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime

        Dim procs As Process() = Process.GetProcessesByName("ProjectG")
        If procs.Length = 0 Then
            MsgBox("ProjectG not found.")
            Return
        End If

        Dim targetProc As Process = procs(0)
        _targetPid = targetProc.Id
        _hProcess = WinApi.OpenProcess(WinApi.PROCESS_VM_READ Or WinApi.PROCESS_QUERY_INFORMATION, False, targetProc.Id)

        Dim moduleBase As IntPtr = targetProc.MainModule.BaseAddress

        Dim staticAddress As IntPtr = IntPtr.Add(moduleBase, BASE_OFFSET)

        _targetAddress = MemoryScanner.GetPointerAddress(_hProcess, staticAddress, POINTER_OFFSETS)

        If _targetAddress <> IntPtr.Zero Then
            Lb_state.Text = "Start"
            Lb_state.ForeColor = Color.Blue
            Btn_start.Enabled = False
            Btn_stop.Enabled = True
            Txt_pangya.Enabled = False
            Txt_power.Enabled = False

            WinApi.timeBeginPeriod(1)
            _isMonitoring = True
            _cts = New CancellationTokenSource()

            _monitoringTask = Task.Run(Sub() MonitoringLoop(_cts.Token))
            Await _monitoringTask

            _isMonitoring = False
            If Not Me.IsDisposed Then
                Lb_state.Text = "Stop"
                Lb_state.ForeColor = Color.Red
                Btn_start.Enabled = True
                Btn_stop.Enabled = False
                Txt_pangya.Enabled = True
                Txt_power.Enabled = True
            End If
        Else
            MsgBox("Error: Failed to supplement memory. (Please check administrator privileges or game version.)")
        End If
    End Sub

    Private Sub Btn_stop_Click(sender As Object, e As EventArgs) Handles Btn_stop.Click
        StopMonitoring()
    End Sub

    Private Sub StopMonitoring()
        If _isMonitoring Then
            _cts?.Cancel()
            _isMonitoring = False
            WinApi.timeEndPeriod(1)
            Lb_state.Text = "Stop"
            Lb_state.ForeColor = Color.Red
            Btn_start.Enabled = True
            Btn_stop.Enabled = False
            Txt_pangya.Enabled = True
            Txt_power.Enabled = True
        End If
    End Sub

    Private Sub SafeInvoke(action As Action)
        Try
            If Not Me.IsDisposed AndAlso Me.IsHandleCreated Then Me.Invoke(action)
        Catch
        End Try
    End Sub

    Private Sub MonitoringLoop(ByVal token As CancellationToken)
        Dim hasTargetFired As Boolean = False
        Dim hasReturnFired As Boolean = False
        Dim hasMidFired As Boolean = False

        Dim targetPercent As Double = 100.0
        Dim keyPangya As Integer = 0
        Dim keyPower As Integer = 0

        SafeInvoke(Sub()
                       keyPangya = CInt(Txt_pangya.Text)
                       keyPower = CInt(Txt_power.Text)
                   End Sub)

        Do
            If token.IsCancellationRequested Then Exit Do
            If Me.IsDisposed Then Exit Do
            If Not IsGameActive() Then
                Thread.Sleep(100)
                Continue Do
            End If

            Dim currentFloat As Single = MemoryScanner.ReadFloat(_hProcess, _targetAddress)

            Dim currentPercent As Double = (currentFloat - GAUGE_0_VAL) / GAUGE_RANGE * 100.0

            SafeInvoke(Sub() Double.TryParse(Txt_p.Text, targetPercent))

            Dim isPowerPressed As Boolean = (WinApi.GetAsyncKeyState(keyPower) And &H8000) <> 0
            Dim isPangyaPressed As Boolean = (WinApi.GetAsyncKeyState(keyPangya) And &H8000) <> 0

            If isPowerPressed Then
                If currentPercent >= targetPercent AndAlso Not hasTargetFired Then
                    InputController.ExecuteSingleKeyAsync(WinApi.VK_SPACE, 130)
                    hasTargetFired = True
                End If
            Else
                hasTargetFired = False
            End If

            If isPangyaPressed Then
                If Not hasMidFired AndAlso currentPercent < 50.0 AndAlso currentPercent > 10.0 Then
                    Dim cmd As String = "", spn As String = ""
                    SafeInvoke(Sub()
                                   cmd = Cmb_tok.Text
                                   spn = Cmb_spca.Text
                               End Sub)
                    InputController.ExecuteFullSequence(cmd, spn)
                    hasMidFired = True
                End If

                If currentPercent <= 0.2 AndAlso Not hasReturnFired Then
                    InputController.ExecuteSingleKeyAsync(WinApi.VK_SPACE, 130)
                    hasReturnFired = True
                    SafeInvoke(Sub() Textset())
                    Thread.Sleep(1000)
                End If
            Else
                hasReturnFired = False
                hasMidFired = False
            End If

            Thread.Sleep(0)
        Loop
    End Sub

End Class