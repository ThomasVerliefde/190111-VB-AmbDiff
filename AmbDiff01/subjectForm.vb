Public Class subjectForm
	Inherits Form

	Private WithEvents contButton As New continueButton(Txt:="Bestätigen")
	Private WithEvents textBox As New TextBox
	Private ReadOnly subjPanel As New labelledBox(Me.textBox, "VPNr:")
	Private subjN As Integer

	Private Sub formLoad(sender As Object, e As EventArgs) Handles MyBase.Load

		Dim screenWidth As Integer = Screen.PrimaryScreen.Bounds.Width
		Dim screenHeight As Integer = Screen.PrimaryScreen.Bounds.Height

		Me.WindowState = FormWindowState.Normal
		Me.FormBorderStyle = FormBorderStyle.None

		Me.Size = New Point(250, 200)
		Me.BackColor = Color.Gray
		Me.Top = (3 * screenHeight / 4) - (Me.Height / 2)
		Me.Left = (screenWidth / 2) - (Me.Width / 2)

		Me.Controls.Add(Me.contButton)
		objCenter(Me.contButton)

		Me.Controls.Add(Me.subjPanel)
		objCenter(Me.subjPanel, 0.4)

		Me.textBox.MaxLength = 3
		Me.textBox.Select()

	End Sub

	Private Sub contButton_Click(sender As Object, e As EventArgs) Handles contButton.Click

		If Me.textBox.Text = 999 Then
			debugMode = True
			Me.textBox.Text = 499
		End If

		If Me.textBox.Text = "" OrElse Val(Me.textBox.Text) < 1 OrElse Val(Me.textBox.Text) > 500 Then
			'Interestingly, Val("these are letters") returns 0, and results in error
			MsgBox("Bitte geben Sie eine korrekte VPNr ein!", MsgBoxStyle.Critical, Title:="Fehler!")
			Exit Sub
		Else
			Me.subjN = CInt(Me.textBox.Text)
			Select Case getCond(Me.subjN)
				Case 0
					_mainForm.keyAss = "Ypos"
				Case 1
					_mainForm.keyAss = "Yneg"
			End Select

			dataFrame("Subject") = Me.subjN.ToString
			dataFrame("Key") = _mainForm.keyAss

			If debugMode Then
				dataFrame("Subject") = "DEBUG"
			End If

			Me.Close()
		End If
	End Sub

	Private Sub confirmEnter(sender As Object, e As KeyEventArgs) Handles textBox.KeyDown
		If e.KeyCode = Keys.Enter Then
			Me.contButton.PerformClick()
		End If
	End Sub

	Private Sub suppressNonNumeric(sender As Object, e As KeyPressEventArgs) Handles textBox.KeyPress
		If e.KeyChar <> ControlChars.Back AndAlso Not IsNumeric(e.KeyChar) Then
			e.Handled = True
		End If
	End Sub

End Class