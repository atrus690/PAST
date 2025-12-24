<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Btn_start = New System.Windows.Forms.Button()
        Me.Btn_stop = New System.Windows.Forms.Button()
        Me.Lb_state = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Txt_p = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer3 = New System.Windows.Forms.Timer(Me.components)
        Me.Cmb_tok = New System.Windows.Forms.ComboBox()
        Me.Cmb_spca = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Txt_pangya = New System.Windows.Forms.TextBox()
        Me.Txt_power = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Btn_start
        '
        Me.Btn_start.Location = New System.Drawing.Point(7, 12)
        Me.Btn_start.Name = "Btn_start"
        Me.Btn_start.Size = New System.Drawing.Size(57, 23)
        Me.Btn_start.TabIndex = 0
        Me.Btn_start.Text = "Start"
        Me.Btn_start.UseVisualStyleBackColor = True
        '
        'Btn_stop
        '
        Me.Btn_stop.Enabled = False
        Me.Btn_stop.Location = New System.Drawing.Point(7, 41)
        Me.Btn_stop.Name = "Btn_stop"
        Me.Btn_stop.Size = New System.Drawing.Size(57, 23)
        Me.Btn_stop.TabIndex = 1
        Me.Btn_stop.Text = "Stop"
        Me.Btn_stop.UseVisualStyleBackColor = True
        '
        'Lb_state
        '
        Me.Lb_state.AutoSize = True
        Me.Lb_state.Font = New System.Drawing.Font("Cambria", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Lb_state.ForeColor = System.Drawing.Color.Red
        Me.Lb_state.Location = New System.Drawing.Point(83, 23)
        Me.Lb_state.Name = "Lb_state"
        Me.Lb_state.Size = New System.Drawing.Size(58, 28)
        Me.Lb_state.TabIndex = 2
        Me.Lb_state.Text = "Stop"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(95, 98)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(12, 12)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "P"
        '
        'Txt_p
        '
        Me.Txt_p.Location = New System.Drawing.Point(113, 95)
        Me.Txt_p.Name = "Txt_p"
        Me.Txt_p.Size = New System.Drawing.Size(35, 19)
        Me.Txt_p.TabIndex = 8
        Me.Txt_p.Text = "100"
        Me.Txt_p.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(5, 73)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 12)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "PangYa"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 98)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(36, 12)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "Power"
        '
        'Timer1
        '
        Me.Timer1.Interval = 900000
        '
        'Timer3
        '
        Me.Timer3.Interval = 1
        '
        'Cmb_tok
        '
        Me.Cmb_tok.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Cmb_tok.FormattingEnabled = True
        Me.Cmb_tok.Items.AddRange(New Object() {"None", "Tomahawk", "Spike", "Cobra"})
        Me.Cmb_tok.Location = New System.Drawing.Point(88, 124)
        Me.Cmb_tok.Name = "Cmb_tok"
        Me.Cmb_tok.Size = New System.Drawing.Size(75, 20)
        Me.Cmb_tok.TabIndex = 12
        '
        'Cmb_spca
        '
        Me.Cmb_spca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Cmb_spca.FormattingEnabled = True
        Me.Cmb_spca.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Cmb_spca.Items.AddRange(New Object() {"None", "↓", "↑", "←", "→"})
        Me.Cmb_spca.Location = New System.Drawing.Point(88, 150)
        Me.Cmb_spca.Name = "Cmb_spca"
        Me.Cmb_spca.Size = New System.Drawing.Size(75, 20)
        Me.Cmb_spca.TabIndex = 13
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(40, 132)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(42, 12)
        Me.Label6.TabIndex = 20
        Me.Label6.Text = "Special"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(20, 153)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(62, 12)
        Me.Label7.TabIndex = 21
        Me.Label7.Text = "Long Press"
        '
        'Txt_pangya
        '
        Me.Txt_pangya.Location = New System.Drawing.Point(49, 70)
        Me.Txt_pangya.Name = "Txt_pangya"
        Me.Txt_pangya.Size = New System.Drawing.Size(32, 19)
        Me.Txt_pangya.TabIndex = 22
        Me.Txt_pangya.Text = "49"
        Me.Txt_pangya.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Txt_power
        '
        Me.Txt_power.Location = New System.Drawing.Point(49, 95)
        Me.Txt_power.Name = "Txt_power"
        Me.Txt_power.Size = New System.Drawing.Size(32, 19)
        Me.Txt_power.TabIndex = 23
        Me.Txt_power.Text = "50"
        Me.Txt_power.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(154, 98)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(11, 12)
        Me.Label8.TabIndex = 24
        Me.Label8.Text = "%"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(175, 182)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Txt_power)
        Me.Controls.Add(Me.Txt_pangya)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Cmb_spca)
        Me.Controls.Add(Me.Cmb_tok)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Txt_p)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Lb_state)
        Me.Controls.Add(Me.Btn_stop)
        Me.Controls.Add(Me.Btn_start)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.ShowIcon = False
        Me.Text = "PAST"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Btn_start As System.Windows.Forms.Button
    Friend WithEvents Btn_stop As System.Windows.Forms.Button
    Friend WithEvents Lb_state As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Txt_p As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents Timer2 As System.Windows.Forms.Timer
    Friend WithEvents Timer3 As System.Windows.Forms.Timer
    Friend WithEvents Cmb_tok As System.Windows.Forms.ComboBox
    Friend WithEvents Cmb_spca As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Txt_pangya As System.Windows.Forms.TextBox
    Friend WithEvents Txt_power As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
End Class
