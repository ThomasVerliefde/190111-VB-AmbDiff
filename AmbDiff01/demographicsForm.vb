
Public Class demographicsForm
	Inherits Form

	Private WithEvents contButton As New continueButton
	Private instrLabel As New Label
	Private WithEvents ageText As New TextBox
	Private ageBox As New labelledBox(Me.ageText, "Bitte geben Sie Ihr Alter an:", setBox:=500)
	Private genderBox As New labelledList({"weiblich", "männlich", "X"}, "Bitte geben Sie Ihr Geschlecht an:", setBox:=500)
	Private languageBox As New labelledList({"Deutsch", "Sonstiges"}, "Bitte geben Sie Ihre Muttersprache an:", setBox:=500)
	Private handBox As New labelledList({"rechts", "links", "beide"}, "Bitte geben Sie Ihre Händigkeit an:", setBox:=500)
	Private WithEvents studyText As New TextBox
	Private studyBox As New labelledBox(Me.studyText, "Bitte geben Sie Ihr Studienfach an:", boxWidth:=500, setBox:=500)

	Private Sub formLoad(sender As Object, e As EventArgs) Handles MyBase.Load

		Me.WindowState = FormWindowState.Maximized
		Me.FormBorderStyle = FormBorderStyle.None
		Me.BackColor = Color.White
		Me.ageText.MaxLength = 2

		Dim i As Double = 0.25
		For Each item In {Me.ageBox, Me.genderBox, Me.languageBox, Me.handBox, Me.studyBox}
			Me.Controls.Add(item)
			objCenter(item, verticalDist:=i, horizontalDist:=0, setLeft:=300)
			i += 0.1
		Next

		Me.Controls.Add(Me.contButton)
		objCenter(Me.contButton)

		Me.instrLabel.Text = "Bitte machen Sie zum Abschluss noch die folgenden Angaben"
		Me.instrLabel.Size = New Size(1000, 40)
		Me.instrLabel.TextAlign = ContentAlignment.MiddleCenter
		Me.instrLabel.Font = mainModule.sansSerif22
		Me.Controls.Add(Me.instrLabel)
		objCenter(Me.instrLabel, verticalDist:=0.13)

		If debugMode Then
			Me.ageText.Text = "23"
			Me.genderBox.optionBox.Text = "weiblich"
			Me.languageBox.optionBox.Text = "Deutsch"
			Me.handBox.optionBox.Text = "rechts"
			Me.studyText.Text = "DEBUG"
			Me.contButton.Select()
		End If

	End Sub

	Private Sub contButton_Click(sender As Object, e As EventArgs) Handles contButton.Click

		If Me.ageText.Text = "" OrElse Val(Me.ageText.Text) < 14 OrElse Val(Me.ageText.Text) > 99 Then
			MsgBox("Bitte geben Sie Ihr Alter korrekt an!", MsgBoxStyle.Critical, Title:="Fehler!")
			Exit Sub
		ElseIf Not Me.genderBox.madeSelection Then
			MsgBox("Bitte geben Sie Ihr Geschlecht an!", MsgBoxStyle.Critical, Title:="Fehler!")
			Exit Sub
		ElseIf Not Me.languageBox.madeSelection Then
			MsgBox("Bitte geben Sie Ihre Muttersprache an!", MsgBoxStyle.Critical, Title:="Fehler!")
			Exit Sub
		ElseIf Not Me.handBox.madeSelection Then
			MsgBox("Bitte geben Sie Ihre Händigkeit an!", MsgBoxStyle.Critical, Title:="Fehler!")
			Exit Sub
		ElseIf Me.studyText.Text = "" Then
			MsgBox("Bitte geben Sie Ihr Studienfach an!", MsgBoxStyle.Critical, Title:="Fehler!")
			Exit Sub
		Else
			dataFrame("Age") = Me.ageText.Text.ToString
			dataFrame("Gender") = Me.genderBox.optionBox.Text.ToString
			dataFrame("Language") = Me.languageBox.optionBox.Text.ToString
			dataFrame("Handedness") = Me.handBox.optionBox.Text.ToString
			dataFrame("Study") = Me.studyText.Text.ToString

			Me.Close()
		End If

	End Sub

	Private Sub suppressNonNumeric(sender As Object, e As KeyPressEventArgs) Handles ageText.KeyPress
		If e.KeyChar <> ControlChars.Back AndAlso Not IsNumeric(e.KeyChar) Then
			e.Handled = True
		End If
	End Sub

	Private Sub pressEnter(sender As Object, e As KeyEventArgs) Handles ageText.KeyDown, studyText.KeyDown
		If e.KeyCode = Keys.Enter Then
			If Not Me.ageText.Text = "" AndAlso Me.ageText.ContainsFocus Then
				Me.genderBox.Select()
			ElseIf Not Me.studyText.Text = "" Then
				Me.contButton.PerformClick()
			End If
		End If
	End Sub

End Class