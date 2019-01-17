Imports NodaTime

Public Class _mainForm
	Inherits Form

	Public instructionCount As New Integer 'main counter, to control the flow of the experiment; increases after each continuebutton press in this form

	Friend WithEvents contButton As New continueButton 'main button, to advance the flow of the experiment
	Private ReadOnly instrText As New instructionBox 'main method of displaying the instructions to participants; disabled & readonly

	'All NodaTime.Instant variables, to check starting points of each part
	Private startT As Instant
	'Private otherT As Instant
	Private practiceT As Instant
	Private experimentT As Instant
	'Private explicitT As Instant
	Private demographicsT As Instant
	Private endT As Instant

	'All NodaTime.Duration variables, to check the actual duration (difference in sequential starting points) of each part
	'Private timeOther As Duration
	Private timePractice As Duration
	Private timeExperiment As Duration
	'Private timeExplicit As Duration
	Private timeDemographics As Duration
	Private timeTotal As Duration

	'Variable necessary for grabbing the correct instruction sheet, depending on whether the 'Y' key is used to categorize positive adjectives, or for negative adjectives
	Friend keyAss As String
	'Friend firstOthers As String

	'Friend otherPos As New List(Of String)
	'Friend otherNeg As New List(Of String)

	Public practicePrimes As List(Of List(Of String))
	Public experimentPrimes As List(Of List(Of String))

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

				'Dim otherPractice As New List(Of String)(My.Resources.practiceOthers.Split(" "))
				'otherPractice = compareList(Me.otherNeg, otherPractice)
				'otherPractice = compareList(Me.otherPos, otherPractice)
				'shuffleList(otherPractice)
				Dim practicePrime_Pos As New List(Of String)(My.Resources.practicePrime_Pos.Split(" "))
				shuffleList(practicePrime_Pos)
				Dim practicePrime_Neg As New List(Of String)(My.Resources.practicePrime_Neg.Split(" "))
				shuffleList(practicePrime_Neg)
				Dim practicePrime_Str As New List(Of String)(My.Resources.practicePrime_Str.Split(" "))
				shuffleList(practicePrime_Str)

				'practicePrimes is a List of List of String with 5 lists: PositiveNounPrimes, NegativeNounPrimes, PositiveOthers, NegativeOthers, MatchingLetterStrings
				Me.practicePrimes = New List(Of List(Of String))({
								New List(Of String)({practicePrime_Pos(0)}),
								New List(Of String)({practicePrime_Neg(0)}),
								New List(Of String)({otherPractice(0)}),
								New List(Of String)({otherPractice(1)}),
								New List(Of String)({practicePrime_Str(0)})
								})

				'If debugMode Then
				'	Console.WriteLine("- practicePrimes -")
				'	For Each c In Me.practicePrimes
				'		For Each d In c
				'			Console.Write(" * " + d)
				'		Next
				'		Console.WriteLine("")
				'	Next
				'	Console.WriteLine("")
				'End If

				practiceTrials = createTrials(
										Me.practicePrimes,
										New List(Of List(Of String))({
											 New List(Of String)(My.Resources.practiceTarget_Pos.Split(" ")),
											 New List(Of String)(My.Resources.practiceTarget_Neg.Split(" "))
											 }),
										timesPrimes:=2 'How often each prime is paired with a target from each category
										)
				' Results in 20 Trials (Can shorten to 10 by setting timesPrimes to 1)

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
					Console.WriteLine("Amount of Trials: " & amount)
				End If

				shuffleList(practiceTrials)

				Dim savePrimes As New List(Of String)
				Me.practicePrimes.ForEach(Sub(x) savePrimes.Add(String.Join(" ", x)))
				dataFrame("practicePrimes") = String.Join(" ", savePrimes)

				Dim saveTrials As New List(Of String)
				practiceTrials.ForEach(Sub(x) saveTrials.Add(String.Join(" ", x)))
				dataFrame("practiceTrials") = String.Join("-", saveTrials)

				' Experiment Trials

				Me.experimentPrimes = createPrimes(
					Me.otherPos,
					Me.otherNeg,
					New List(Of String)(My.Resources.experimentPrime_Pos.Split(" ")),
					New List(Of String)(My.Resources.experimentPrime_Neg.Split(" ")),
					New List(Of String)(My.Resources.experimentPrime_Str.Split(" "))
				)

				If debugMode Then
					Console.WriteLine("- experimentPrimes -")
					For Each c In Me.experimentPrimes
						For Each d In c
							Console.Write(" * " + d)
						Next
						Console.WriteLine("")
					Next
					Console.WriteLine("")
				End If

				experimentTrials = createTrials(
					Me.experimentPrimes,
					New List(Of List(Of String))({
						New List(Of String)(My.Resources.experimentTarget_Pos.Split(" ")),
						New List(Of String)(My.Resources.experimentTarget_Neg.Split(" "))
					}),
					timesPrimes:=4 'How often each prime is paired with a target from each category
				)
				' Results in (12 [Primes] x 2 [Targets] x 4) 96 Trials

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
					Console.WriteLine("Amount of Trials: " & amount)
				End If

				shuffleList(experimentTrials)

				savePrimes.Clear()
				Me.experimentPrimes.ForEach(Sub(x) savePrimes.Add(String.Join(" ", x)))
				dataFrame("experimentPrimes") = String.Join(" ", savePrimes)

				saveTrials.Clear()
				experimentTrials.ForEach(Sub(x) saveTrials.Add(String.Join(" ", x)))
				dataFrame("experimentTrials") = String.Join("-", saveTrials)

			Case 2 'Practice Trials
				Me.practiceT = time.GetCurrentInstant()
				practiceForm.ShowDialog()
				Me.instrText.Rtf = My.Resources.ResourceManager.GetString("_3_experiment" & Me.keyAss)
				practiceForm.Dispose()

			Case 3 'Experiment Proper
				Me.experimentT = time.GetCurrentInstant()
				experimentForm.ShowDialog()
				Me.instrText.Rtf = My.Resources.ResourceManager.GetString("_4_explicitInstr")
				experimentForm.Dispose()

			Case 4 'Explicit/Direct Measurements of Ambivalence
				Me.explicitT = time.GetCurrentInstant()
				explicitForm.ShowDialog()
				Me.instrText.Rtf = My.Resources.ResourceManager.GetString("_5_demoInstr")
				explicitForm.Dispose()

			Case 5 'Demographic Information
				Me.demographicsT = time.GetCurrentInstant()
				demographicsForm.ShowDialog()
				Me.instrText.Rtf = My.Resources.ResourceManager.GetString("_6_endInstr")
				demographicsForm.Dispose()

				Me.instrText.Font = New Font("Microsoft Sans Serif", 40)
				Me.contButton.Text = "Abbrechen"
				Me.endT = time.GetCurrentInstant()

				Me.timeOther = Me.practiceT - Me.otherT
				Me.timePractice = Me.experimentT - Me.practiceT
				Me.timeExperiment = Me.explicitT - Me.experimentT
				Me.timeExplicit = Me.demographicsT - Me.explicitT
				Me.timeDemographics = Me.endT - Me.demographicsT
				Me.timeTotal = Me.endT - Me.startT

				dataFrame("timeOther") = Me.timeOther.TotalMinutes.ToString
				dataFrame("timePractice") = Me.timePractice.TotalMinutes.ToString
				dataFrame("timeExperiment") = Me.timeExperiment.TotalMinutes.ToString
				dataFrame("timeExplicit") = Me.timeExplicit.TotalMinutes.ToString
				dataFrame("timeDemographics") = Me.timeDemographics.TotalMinutes.ToString
				dataFrame("timeTotal") = Me.timeTotal.TotalMinutes.ToString
				dataFrame("hostName") = Net.Dns.GetHostName()

				IO.Directory.CreateDirectory("Data")
				saveCSV(dataFrame, "Data\RelAmb_" & dataFrame("Subject") & "_" & Net.Dns.GetHostName & ".csv")

			Case Else
				Me.Close()
		End Select
		Me.instructionCount += 1
	End Sub





End Class
