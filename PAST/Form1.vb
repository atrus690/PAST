Imports System.Threading

Public Class Form1
    Private _zeroPos As Single
    Private _fullPos As Single
    Private _scanTargetInt As Integer
    Private _targetAddress As IntPtr = IntPtr.Zero
    Private _hProcess As IntPtr = IntPtr.Zero
    Private _isMonitoring As Boolean = False
    Private _cts As CancellationTokenSource
    Private _monitoringTask As Task
    Private _candidateAddresses As List(Of IntPtr)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Txt_pangya.Text = ConfigManager.GetValue("PangYakey", "49")
        Txt_power.Text = ConfigManager.GetValue("Powerkey", "50")
        Dim x As Integer = CInt(ConfigManager.GetValue("Form.X", "0"))
        Dim y As Integer = CInt(ConfigManager.GetValue("Form.Y", "0"))
        If x <> 0 And y <> 0 Then SetDesktopLocation(x, y)

        Textset()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        StopMonitoring()
        ConfigManager.SetValue("Form.X", Location.X.ToString())
        ConfigManager.SetValue("Form.Y", Location.Y.ToString())
        ConfigManager.SetValue("PangYakey", Txt_pangya.Text)
        ConfigManager.SetValue("Powerkey", Txt_power.Text)
        WinApi.timeEndPeriod(1)
        Environment.Exit(0)
    End Sub

    Private Sub Textset()
        Cmb_spca.Text = "None"
        Cmb_tok.Text = "None"
    End Sub

    Private Sub CalculateGaugeParams(w As Integer, h As Integer)
        Const BASE_HEIGHT As Single = 600.0F
        Const BASE_LEN As Single = 450.0F
        Const BASE_OFFSET_INIT As Single = -268.75F

        Dim scale As Single = h / BASE_HEIGHT
        Dim centerX As Single = w / 2.0F
        Dim currentLen As Single = BASE_LEN * scale

        _zeroPos = centerX - (currentLen / 2.0F)
        _fullPos = centerX + (currentLen / 2.0F)

        Dim targetFloat As Single = centerX + (BASE_OFFSET_INIT * scale)
        _scanTargetInt = BitConverter.ToInt32(BitConverter.GetBytes(targetFloat), 0)
    End Sub

    Private Async Sub Btn_start_Click(sender As Object, e As EventArgs) Handles Btn_start.Click
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime

        Dim procs As Process() = Process.GetProcessesByName("ProjectG")
        If procs.Length = 0 Then
            MsgBox("ProjectG not found.")
            Return
        End If

        _hProcess = WinApi.OpenProcess(WinApi.PROCESS_VM_READ Or WinApi.PROCESS_QUERY_INFORMATION, False, procs(0).Id)

        Dim height As Integer = MemoryScanner.ReadInt32(_hProcess, New IntPtr(&HF02330))
        Dim width As Integer = MemoryScanner.ReadInt32(_hProcess, New IntPtr(&HF02334))

        If width = 0 Or height = 0 Then
            Dim rect As New WinApi.RECT()
            WinApi.GetClientRect(procs(0).MainWindowHandle, rect)
            width = rect.Right : height = rect.Bottom
        End If

        CalculateGaugeParams(width, height)

        Dim foundAddresses As List(Of IntPtr) = MemoryScanner.ScanForValue(_hProcess, _scanTargetInt, 0, &H7FFFFFFF)

        If foundAddresses IsNot Nothing AndAlso foundAddresses.Count > 0 Then
            _candidateAddresses = foundAddresses
            _targetAddress = IntPtr.Zero

            Lb_state.Text = "Start"
            Lb_state.ForeColor = Color.Blue
            Btn_start.Enabled = False
            Btn_stop.Enabled = True

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
            End If
        Else
            MsgBox("Error: Setup failed.")
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

        Dim totalRange As Single = _fullPos - _zeroPos
        If totalRange = 0 Then totalRange = 450.0F
        Dim initFloatVal As Single = BitConverter.ToSingle(BitConverter.GetBytes(_scanTargetInt), 0)

        SafeInvoke(Sub()
                       keyPangya = CInt(Txt_pangya.Text)
                       keyPower = CInt(Txt_power.Text)
                   End Sub)

        Do
            If token.IsCancellationRequested Then Exit Do
            If Me.IsDisposed Then Exit Do

            If _targetAddress = IntPtr.Zero Then
                For Each addr As IntPtr In _candidateAddresses
                    Dim val As Single = MemoryScanner.ReadFloat(_hProcess, addr)
                    If Math.Abs(val - initFloatVal) > 1.0F Then
                        _targetAddress = addr
                        Exit For
                    End If
                Next
                If _targetAddress = IntPtr.Zero Then
                    Thread.Sleep(10)
                    Continue Do
                End If
            End If

            Dim isPowerPressed As Boolean = (WinApi.GetAsyncKeyState(keyPower) And &H8000) <> 0
            Dim isPangyaPressed As Boolean = (WinApi.GetAsyncKeyState(keyPangya) And &H8000) <> 0

            SafeInvoke(Sub() Double.TryParse(Txt_p.Text, targetPercent))

            Dim currentFloat As Single = MemoryScanner.ReadFloat(_hProcess, _targetAddress)
            Dim currentPercent As Double = (currentFloat - _zeroPos) / totalRange * 100.0

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
                End If
            Else
                hasReturnFired = False
                hasMidFired = False
            End If

            Thread.Sleep(0)
        Loop
    End Sub

End Class