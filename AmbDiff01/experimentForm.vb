
Public Class experimentForm
	Inherits Form

	Private stopwatchTarget As New Stopwatch
	Private answeringTime As Long

	Private WithEvents timerITI As New Timer
	Private WithEvents timerFix As New Timer
	Private WithEvents timerPrime As New Timer

	Private leftLab As New Label
	Private rightLab As New Label
	Private slowLab As New Label
	Private fixLab As New Label
	Private primeLab As New Label
	Private targetLab As New Label

	Private ignoreKeys As Boolean
	Private trialCounter As Integer

	Private Sub formLoad(sender As Object, e As EventArgs) Handles MyBase.Load

		Me.WindowState = FormWindowState.Maximized
		Me.FormBorderStyle = FormBorderStyle.None
		Me.BackColor = Color.White

		Cursor.Hide()
		Me.timerITI.Interval = 300
		Me.timerFix.Interval = 500
		Me.timerPrime.Interval = 150

		If debugMode Then
			Me.timerITI.Interval = 50
			Me.timerFix.Interval = 50
			Me.timerPrime.Interval = 50
		End If

		Me.Controls.AddRange({Me.leftLab, Me.rightLab, Me.slowLab, Me.fixLab, Me.primeLab, Me.targetLab})

		Select Case _mainForm.keyAss
			Case "Ypos"
				Me.leftLab.Text = "Y = positiv"
				Me.rightLab.Text = "- = negativ"
			Case "Yneg"
				Me.leftLab.Text = "Y = negativ"
				Me.rightLab.Text = "- = positiv"
		End Select

		For Each label In New List(Of Label)({Me.leftLab, Me.rightLab})
			With label
				.Left = 100
				.Top = 100
				.Font = sansSerif20
				.TextAlign = ContentAlignment.TopLeft
				.AutoSize = True
			End With
		Next

		Me.rightLab.TextAlign = ContentAlignment.TopRight
		Me.rightLab.Left = Me.Width - TextRenderer.MeasureText(Me.rightLab.Text, sansSerif20).Width - 100

		For Each label In New List(Of Label)({Me.slowLab, Me.fixLab, Me.primeLab, Me.targetLab})
			With label
				.Visible = False
				.Text = ""
				.Font = sansSerif60
				.TextAlign = ContentAlignment.MiddleCenter
				.Width = Screen.PrimaryScreen.Bounds.Width / 1.5
				.Height = 100
			End With
			objCenter(label, 0.5)
		Next

		With Me.slowLab
			.Text = "Schneller!"
			.Font = sansSerif25B
			.ForeColor = Color.Red
		End With

		With Me.fixLab
			.Text = "+"
			.Font = sansSerif72
		End With

		Me.timerITI.Enabled = True

	End Sub

	Private Sub timerITI_Tick(sender As Object, e As EventArgs) Handles timerITI.Tick
		Me.timerITI.Stop()
		Me.timerFix.Start()
		Me.slowLab.Visible = False
		Me.fixLab.Visible = True
	End Sub

	Private Sub timerFix_Tick(sender As Object, e As EventArgs) Handles timerFix.Tick
		Me.timerFix.Stop()
		Me.timerPrime.Start()
		If Me.trialCounter < experimentTrials.Count Then
			Me.primeLab.Text = experimentTrials(Me.trialCounter)(0)
		End If
		Me.fixLab.Visible = False
		Me.fixLab.Visible = False
		Me.primeLab.Visible = True
	End Sub

	Private Sub timerPrime_Tick(sender As Object, e As EventArgs) Handles timerPrime.Tick
		Me.timerPrime.Stop()
		Me.stopwatchTarget.Start()
		If Me.trialCounter < experimentTrials.Count Then
			Me.targetLab.Text = experimentTrials(Me.trialCounter)(1)
		End If
		Me.primeLab.Visible = False
		Me.targetLab.Visible = True
		Me.ignoreKeys = False

		If debugMode Then
			SendKeys.SendWait("Y")
		End If

	End Sub

	Private Sub responseAL(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

		e.Handled = Me.ignoreKeys 'Stops the event when it is not the categorisation-phase

		If e.KeyCode = Keys.Y Or e.KeyCode = Keys.OemMinus Then 'Non-short-circuited "Or" (instead of OrElse) to prevent systematic differences between A and L (although these would be minimal)
			Me.answeringTime = Me.stopwatchTarget.ElapsedMilliseconds
			Me.stopwatchTarget.Reset()
			Me.timerITI.Start()
			Me.targetLab.Visible = False
			Me.slowLab.Visible = Me.answeringTime > 700
			Me.ignoreKeys = True

			dataFrame("experiment_" & blockCounter & "_" & Me.trialCounter & "_answer") = e.KeyCode.ToString
			dataFrame("experiment_" & blockCounter & "_" & Me.trialCounter & "_time") = Me.answeringTime.ToString
			dataFrame("experiment_" & blockCounter & "_" & Me.trialCounter & "_prime") = experimentTrials(Me.trialCounter)(0)
			dataFrame("experiment_" & blockCounter & "_" & Me.trialCounter & "_target") = experimentTrials(Me.trialCounter)(1)
			dataFrame("experiment_" & blockCounter & "_" & Me.trialCounter & "_primeCat") = experimentTrials(Me.trialCounter)(2)
			dataFrame("experiment_" & blockCounter & "_" & Me.trialCounter & "_targetCat") = experimentTrials(Me.trialCounter)(3)

			Me.trialCounter += 1

			If Me.trialCounter = experimentTrials.Count Then
				blockCounter += 1
				shuffleList(experimentTrials)
				Me.timerITI.Stop()
				Cursor.Show()
				Me.Close()
			End If

		End If

	End Sub

End Class