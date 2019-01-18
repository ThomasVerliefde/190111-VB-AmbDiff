Imports NodaTime

Public Class _mainForm
	Inherits Form

	Public instructionCount As New Integer 'main counter, to control the flow of the experiment; increases after each continuebutton press in this form

	Friend WithEvents contButton As New continueButton 'main button, to advance the flow of the experiment
	Private ReadOnly instrText As New instructionBox 'main method of displaying the instructions to participants; disabled & readonly

	'All NodaTime.Instant variables, to check starting points of each part
	Private startT As Instant
	Private practiceT As Instant
	Private experimentT0 As Instant
	Private experimentT1 As Instant
	Private demographicsT As Instant
	Private endT As Instant

	'All NodaTime.Duration variables, to check the actual duration (difference in sequential starting points) of each part
	Private timePractice As Duration
	Private timeExperimentB0 As Duration
	Private timeExperimentB1 As Duration
	Private timeDemographics As Duration
	Private timeTotal As Duration

	'Variable necessary for grabbing the correct instruction sheet, depending on whether the 'Y' key is used to categorize positive adjectives, or for negative adjectives
	Friend keyAss As String

	Private savePrimes As New List(Of String)
	Private saveTrials As New List(Of String)

	Private Sub formLoad(sender As Object, e As EventArgs) Handles MyBase.Load

		Me.WindowState = FormWindowState.Maximized
		Me.FormBorderStyle = FormBorderStyle.None
		Me.BackColor = Color.White

		subjectForm.ShowDialog()

		Me.Controls.Add(Me.instrText)
		objCenter(Me.instrText, 0.42)
		Me.instrText.Rtf = My.Resources.ResourceManager.GetString("_0_mainInstr")

		Me.Controls.Add(Me.contButton)
		objCenter(Me.contButton)

	End Sub

	Public Sub loadNext(sender As Object, e As EventArgs) Handles contButton.Click
		Select Case Me.instructionCount
			Case 0 'Start of Experiment & Building All Trials
				Me.instrText.Rtf = My.Resources.ResourceManager.GetString("_1_practice" & Me.keyAss)

				subjectForm.Dispose()
				Me.startT = time.GetCurrentInstant()

				' Practice Trials

				Dim practicePrime_Amb As New List(Of String)(My.Resources.practicePrime_Amb.Split(" "))
				shuffleList(practicePrime_Amb)
				Dim practicePrime_Pos As New List(Of String)(My.Resources.practicePrime_Pos.Split(" "))
				shuffleList(practicePrime_Pos)
				Dim practicePrime_Neg As New List(Of String)(My.Resources.practicePrime_Neg.Split(" "))
				shuffleList(practicePrime_Neg)
				Dim practicePrime_Str As New List(Of String)(My.Resources.practicePrime_Str.Split(" "))
				shuffleList(practicePrime_Str)

				'practicePrimes is a List of List of String with 5 lists: PositiveNounPrimes, NegativeNounPrimes, PositiveOthers, NegativeOthers, MatchingLetterStrings
				practicePrimes = New List(Of List(Of String))({
									New List(Of String)({practicePrime_Amb(0), practicePrime_Amb(1)}),
									New List(Of String)({practicePrime_Pos(0), practicePrime_Pos(1)}),
									New List(Of String)({practicePrime_Neg(0), practicePrime_Neg(1)}),
									New List(Of String)({practicePrime_Str(0), practicePrime_Str(1)})
								})

				If debugMode Then
					Console.WriteLine("- practicePrimes -")
					For Each c In practicePrimes
						For Each d In c
							Console.Write(" * " + d)
						Next
						Console.WriteLine("")
					Next
					Console.WriteLine("")
				End If

				practiceTrials = createTrials(
					practicePrimes,
					New List(Of List(Of String))({
						 New List(Of String)(My.Resources.practiceTarget_Pos.Split(" ")),
						 New List(Of String)(My.Resources.practiceTarget_Neg.Split(" "))
					 }),
					timesPrimes:=1 'How often each prime is paired with a target from each category
				) ' Results in 12 Trials (6 Primes x 2 Targets x 1 Pairings)


				If debugMode Then
					Console.WriteLine("- practiceTrials -")
					Dim amount As Integer
					For Each c In practiceTrials
						For Each d In c
							Console.Write(" * " + d)
						Next
						amount += c.Count
						Console.WriteLine("")
					Next
					Console.WriteLine("Amount of Practice Trials: " & (amount / 4))
					Console.WriteLine("")
				End If

				practicePrimes.ForEach(Sub(x) Me.savePrimes.Add(String.Join(" ", x)))
				dataFrame("practicePrimes") = String.Join(" ", Me.savePrimes)

				practiceTrials.ForEach(Sub(x) Me.saveTrials.Add(String.Join(" ", x)))
				dataFrame("practiceTrials") = String.Join("-", Me.saveTrials)

				shuffleList(practiceTrials)

				' Experiment Trials

				experimentPrimes =
					New List(Of List(Of String))({
						New List(Of String)(My.Resources.experimentPrime_Amb.Split(" ")),
						New List(Of String)(My.Resources.experimentPrime_Pos.Split(" ")),
						New List(Of String)(My.Resources.experimentPrime_Neg.Split(" ")),
						New List(Of String)(My.Resources.experimentPrime_Str.Split(" "))
					})

				If debugMode Then
					Console.WriteLine("- experimentPrimes -")
					For Each c In experimentPrimes
						For Each d In c
							Console.Write(" * " + d)
						Next
						Console.WriteLine("")
					Next
					Console.WriteLine("")
				End If

				experimentTrials = createTrials(
					experimentPrimes,
					New List(Of List(Of String))({
						New List(Of String)(My.Resources.experimentTarget_Pos.Split(" ")),
						New List(Of String)(My.Resources.experimentTarget_Neg.Split(" "))
					}),
					timesPrimes:=2 'How often each prime is paired with a target from each category
				)
				' Results in (24 [Primes] x 2 [Targets] x 2 [Pairings]) 96 Trials

				If debugMode Then
					Console.WriteLine("- experimentTrials -")
					Dim amount As Integer
					For Each c In experimentTrials
						For Each d In c
							Console.Write(" * " + d)
						Next
						amount += c.Count
						Console.WriteLine("")
					Next
					Console.WriteLine("Amount of Trials: " & (amount / 4))
				End If

				Me.savePrimes.Clear()
				experimentPrimes.ForEach(Sub(x) Me.savePrimes.Add(String.Join(" ", x)))
				dataFrame("experimentPrimes") = String.Join(" ", Me.savePrimes)

				Me.saveTrials.Clear()
				experimentTrials.ForEach(Sub(x) Me.saveTrials.Add(String.Join(" ", x)))
				dataFrame("experimentTrials") = String.Join("-", Me.saveTrials)

				shuffleList(experimentTrials)

			Case 1 'Practice Trials
				Me.practiceT = time.GetCurrentInstant()
				practiceForm.ShowDialog()
				Me.instrText.Rtf = My.Resources.ResourceManager.GetString("_3_experiment" & Me.keyAss)
				practiceForm.Dispose()

			Case 2 'Experiment Proper Block 1
				Me.experimentT0 = time.GetCurrentInstant()
				experimentForm.ShowDialog()
				Me.instrText.Rtf = My.Resources.ResourceManager.GetString("_4_block" & Me.keyAss)
				experimentForm.Dispose()

			Case 3 'Experiment Proper Block 2		!There should be an adaptable way to implement blocks!
				Me.experimentT1 = time.GetCurrentInstant()
				experimentForm.ShowDialog()
				Me.instrText.Rtf = My.Resources.ResourceManager.GetString("_5_demoInstr")
				experimentForm.Dispose()

			Case 4 'Demographic Information
				Me.demographicsT = time.GetCurrentInstant()
				demographicsForm.ShowDialog()
				Me.instrText.Rtf = My.Resources.ResourceManager.GetString("_6_endInstr")
				demographicsForm.Dispose()

				Me.instrText.Font = sansSerif40
				Me.contButton.Text = "Abbrechen"
				Me.endT = time.GetCurrentInstant()

				Me.timePractice = Me.experimentT0 - Me.practiceT
				Me.timeExperimentB0 = Me.experimentT1 - Me.experimentT0
				Me.timeExperimentB1 = Me.demographicsT - Me.experimentT1
				Me.timeDemographics = Me.endT - Me.demographicsT
				Me.timeTotal = Me.endT - Me.startT

				dataFrame("timePractice") = Me.timePractice.TotalMinutes.ToString
				dataFrame("timeExperimentB0") = Me.timeExperimentB0.TotalMinutes.ToString
				dataFrame("timeExperimentB1") = Me.timeExperimentB1.TotalMinutes.ToString
				dataFrame("timeDemographics") = Me.timeDemographics.TotalMinutes.ToString
				dataFrame("timeTotal") = Me.timeTotal.TotalMinutes.ToString
				dataFrame("hostName") = Net.Dns.GetHostName()

				IO.Directory.CreateDirectory("Data")
				saveCSV(dataFrame, "Data\AmbDiff01_" & dataFrame("Subject") & "_" & Net.Dns.GetHostName & ".csv")

			Case Else
				Me.Close()
		End Select
		Me.instructionCount += 1
	End Sub

End Class
