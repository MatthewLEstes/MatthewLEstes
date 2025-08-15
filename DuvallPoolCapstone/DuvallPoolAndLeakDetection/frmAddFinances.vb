﻿' Add Finances form
' Last Updated: 11/30/2020
' Last modified by Matthew Estes

Public Class frmAddFinances
	' Close form
	Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click

		' Close form
		Close()

	End Sub



	' Load in the Revenue and parts cost
	Private Sub frmAddFinances_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		'Variables
		Dim strSelect As String
		Dim dblOutstanding As Double = 0
		Dim intYear As Integer = 0
		Dim intYearNumber As Integer = 0
		Dim cmdSelect As OleDb.OleDbCommand
		Dim drSourceTable As OleDb.OleDbDataReader
		Dim cmdSelect2 As OleDb.OleDbCommand
		Dim drSourceTable2 As OleDb.OleDbDataReader
		Dim cmdSelect3 As OleDb.OleDbCommand
		Dim drSourceTable3 As OleDb.OleDbDataReader
		Dim cmdSelect4 As OleDb.OleDbCommand
		Dim drSourceTable4 As OleDb.OleDbDataReader
		Dim intNextHighestRecordID As Integer
		Dim intNextMonth As Integer
		Dim intYearHighest As Integer

		Try


			' Connect to database
			If OpenDatabaseConnectionSQLServer() = False Then

				'Alert if no connection
				MessageBox.Show(Me, "Database connection error." & vbNewLine &
										"The application will now close.",
										Me.Text + " Error",
										MessageBoxButtons.OK, MessageBoxIcon.Error)

				'Close Form
				Me.Close()

			End If

			' Build the select statement
			strSelect = "SELECT MAX(intFinanceID) + 1 AS intNextHighestRecordID " &
						" FROM TFinances"

			'Execute
			cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
			drSourceTable = cmdSelect.ExecuteReader

			'Read
			drSourceTable.Read()

			'Check for empty table
			If drSourceTable.IsDBNull(0) = True Then

				'Start at 1 for empty table
				intNextHighestRecordID = 1

			Else

				'Not empty, add 1 to next line
				intNextHighestRecordID = CInt(drSourceTable.Item(0))

			End If

			' Close the reader
			drSourceTable.Close()

			' Build the select statement
			strSelect = "SELECT intMonthID, Tfinances.intYearID, strYear FROM TFinances, TYears WHERE TFinances.intYearID = TYears.intYearID and intFinanceID = " & intNextHighestRecordID - 1

			'Execute
			cmdSelect2 = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
			drSourceTable2 = cmdSelect2.ExecuteReader

			'load the data table from the reader
			drSourceTable2.Read()

			'Check for empty table
			If drSourceTable2.IsDBNull(0) = True Then

				'Start at 1 for empty table
				intNextMonth = 1
			Else

				'Not empty, add 1 to next line
				intNextMonth = CInt(drSourceTable2.Item(0)) + 1
				intYearHighest = CInt(drSourceTable2.Item(1))
				intYearNumber = CInt(drSourceTable2.Item(2))

			End If

			' Close the reader
			drSourceTable2.Close()

			' If month is now 13, add one to year and set month to 1 for january
			If intNextMonth >= 13 Then

				' Set variables
				intYearHighest += 1
				intNextMonth = 1
				intYearNumber += 1

			End If

			' Build the select statement
			strSelect = "SELECT Total, Paid FROM vMonthlyRevenue WHERE MonthDate = " & intNextMonth & " and YearDate = " & intYearNumber

			'Execute
			cmdSelect3 = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
			drSourceTable3 = cmdSelect3.ExecuteReader

			'load the data table from the reader
			drSourceTable3.Read()

			'Check for empty table
			If drSourceTable3.IsDBNull(0) = False Then
				lblPaidRevenue.Text = "$" & (drSourceTable3.Item(1))
				dblOutstanding = (drSourceTable3.Item(0)) - (drSourceTable3.Item(1))
				lblOutstandingRevenue.Text = "$" & dblOutstanding.ToString
			End If

			' Build the select statement
			strSelect = "SELECT TotalCost FROM vPartsOrderedCost WHERE MonthDate = " & intNextMonth & " and YearDate = " & intYearNumber

			'Execute
			cmdSelect4 = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
			drSourceTable4 = cmdSelect4.ExecuteReader

			'load the data table from the reader
			drSourceTable4.Read()

			'Check for empty table
			If drSourceTable4.IsDBNull(0) = False Then
				lblInventoryCost.Text = "$" & (drSourceTable4.Item(0))
			End If

			' Close database
			CloseDatabaseConnection()

		Catch ex As Exception
			'unhandled exception
			MessageBox.Show(ex.Message)
		End Try
	End Sub

	' Submit the addition of a financial month
	Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

		'Variables
		Dim strSelect As String
		Dim strInsert As String
		Dim strPayroll As String = ""
		Dim strInventory As String = ""
		Dim strInsurance As String = ""
		Dim strVehicle As String = ""
		Dim strFuel As String = ""
		Dim strRent As String = ""
		Dim strUtilities As String = ""
		Dim strOther As String = ""
		Dim intYearNumber As Integer = 0
		Dim cmdSelect As OleDb.OleDbCommand
		Dim cmdInsert As OleDb.OleDbCommand
		Dim drSourceTable As OleDb.OleDbDataReader
		Dim cmdSelect2 As OleDb.OleDbCommand
		Dim cmdInsert2 As OleDb.OleDbCommand
		Dim drSourceTable2 As OleDb.OleDbDataReader
		Dim intNextHighestRecordID As Integer
		Dim intNextMonth As Integer
		Dim intYearHighest As Integer
		Dim intRowsAffected As Integer

		Try
			' Try to validate info
			If Validation() = True Then

				' Set values
				strPayroll = txtPayroll.Text
				strInsurance = txtInsurance.Text
				strVehicle = txtVehicle.Text
				strFuel = txtFuel.Text
				strRent = txtRent.Text
				strUtilities = txtUtilities.Text
				strOther = txtOther.Text

				' Connect to database
				If OpenDatabaseConnectionSQLServer() = False Then

					'Alert if no connection
					MessageBox.Show(Me, "Database connection error." & vbNewLine &
										"The application will now close.",
										Me.Text + " Error",
										MessageBoxButtons.OK, MessageBoxIcon.Error)

					'Close Form
					Me.Close()

				End If

				' Build the select statement
				strSelect = "SELECT MAX(intFinanceID) + 1 AS intNextHighestRecordID " &
							" FROM TFinances"

				'Execute
				cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
				drSourceTable = cmdSelect.ExecuteReader

				'Read
				drSourceTable.Read()

				'Check for empty table
				If drSourceTable.IsDBNull(0) = True Then

					'Start at 1 for empty table
					intNextHighestRecordID = 1

				Else

					'Not empty, add 1 to next line
					intNextHighestRecordID = CInt(drSourceTable.Item(0))

				End If

				' Close the reader
				drSourceTable.Close()

				' Build the select statement
				strSelect = "SELECT intMonthID, Tfinances.intYearID, strYear FROM TFinances, TYears WHERE TFinances.intYearID = TYears.intYearID and intFinanceID = " & intNextHighestRecordID - 1

				'Execute
				cmdSelect2 = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
				drSourceTable2 = cmdSelect2.ExecuteReader

				'load the data table from the reader
				drSourceTable2.Read()

				'Check for empty table
				If drSourceTable2.IsDBNull(0) = True Then

					'Start at 1 for empty table
					intNextMonth = 1
				Else

					'Not empty, add 1 to next line
					intNextMonth = CInt(drSourceTable2.Item(0)) + 1
					intYearHighest = CInt(drSourceTable2.Item(1))
					intYearNumber = CInt(drSourceTable2.Item(2))

				End If

				' Close the reader
				drSourceTable2.Close()

				' If month is now 13, add one to year and set month to 1 for january
				If intNextMonth >= 13 Then

					' Set variables
					intYearHighest += 1
					intNextMonth = 1

					' Insert year into the year table
					intYearNumber += 1
					strInsert = "INSERT INTO TYears (intYearID, strYear) Values (" & intYearHighest & ", '" & intYearNumber & "')"

					cmdInsert2 = New OleDb.OleDbCommand(strInsert, m_conAdministrator)

					intRowsAffected = cmdInsert2.ExecuteNonQuery()

					If intRowsAffected > 0 Then
						MessageBox.Show("New year has been entered.")
						Me.Close()
					End If

					intRowsAffected = 0

				End If

				'Create insert statement
				strInsert = "Insert into TFinances (intFinanceID, intMonthID, intYearID, decPayrollCost, decInsuranceCost, decVehicleCost, decFuelCost, decShopRental, decUtilitiesCost, decOtherCost)" &
					" Values (" & intNextHighestRecordID & ", " & intNextMonth & ", " & intYearHighest & ", " & strPayroll &
					", " & strInsurance & ", " & strVehicle & ", " & strFuel & ", " & strRent & ", " &
					strUtilities & ", " & strOther & ")"

				cmdInsert = New OleDb.OleDbCommand(strInsert, m_conAdministrator)

				intRowsAffected = cmdInsert.ExecuteNonQuery()

				If intRowsAffected > 0 Then
					MessageBox.Show("Finance for the month has been added.")
					Me.Close()
				End If

				CloseDatabaseConnection()

			End If

		Catch ex As Exception
			'unhandled exception
			MessageBox.Show(ex.Message)
		End Try

	End Sub


	' Validates whether all text fiels are filled and are numeric
	Function Validation() As Boolean

		txtPayroll.BackColor = Color.White
		txtInsurance.BackColor = Color.White
		txtVehicle.BackColor = Color.White
		txtFuel.BackColor = Color.White
		txtRent.BackColor = Color.White
		txtUtilities.BackColor = Color.White
		txtOther.BackColor = Color.White

		' check if something is entered in Payroll text box
		If txtPayroll.Text <> String.Empty And IsNumeric(txtPayroll.Text) Then

		Else
			' text box is blank so tell user to enter payroll, change back color to yellow,
			' put focus in text box and return false we don't want to continue
			MessageBox.Show("Please enter payroll expense.")
			txtPayroll.BackColor = Color.Yellow
			txtPayroll.Focus()
			Return False
		End If

		' check if something is entered in Insurance text box
		If txtInsurance.Text <> String.Empty And IsNumeric(txtInsurance.Text) Then

		Else
			' text box is blank so tell user to enter insurance, change back color to yellow,
			' put focus in text box and return false we don't want to continue
			MessageBox.Show("Please enter insurance expense.")
			txtInsurance.BackColor = Color.Yellow
			txtInsurance.Focus()
			Return False
		End If

		' check if something is entered in Vehicle text box
		If txtVehicle.Text <> String.Empty And IsNumeric(txtVehicle.Text) Then

		Else
			' text box is blank so tell user to enter vehicle, change back color to yellow,
			' put focus in text box and return false we don't want to continue
			MessageBox.Show("Please enter vehicle expense.")
			txtVehicle.BackColor = Color.Yellow
			txtVehicle.Focus()
			Return False
		End If

		' check if something is entered in Fuel text box
		If txtFuel.Text <> String.Empty And IsNumeric(txtFuel.Text) Then

		Else
			' text box is blank so tell user to enter fuel, change back color to yellow,
			' put focus in text box and return false we don't want to continue
			MessageBox.Show("Please enter fuel expense.")
			txtFuel.BackColor = Color.Yellow
			txtFuel.Focus()
			Return False
		End If

		' check if something is entered in Rent text box
		If txtRent.Text <> String.Empty And IsNumeric(txtRent.Text) Then

		Else
			' text box is blank so tell user to enter rent, change back color to yellow,
			' put focus in text box and return false we don't want to continue
			MessageBox.Show("Please enter rent expense.")
			txtRent.BackColor = Color.Yellow
			txtRent.Focus()
			Return False
		End If

		' check if something is entered in Utility text box
		If txtUtilities.Text <> String.Empty And IsNumeric(txtUtilities.Text) Then

		Else
			' text box is blank so tell user to enter utility, change back color to yellow,
			' put focus in text box and return false we don't want to continue
			MessageBox.Show("Please enter utilities expense.")
			txtUtilities.BackColor = Color.Yellow
			txtUtilities.Focus()
			Return False
		End If

		' check if something is entered in Other text box
		If txtOther.Text <> String.Empty And IsNumeric(txtOther.Text) Then

		Else
			' text box is blank so tell user to enter other, change back color to yellow,
			' put focus in text box and return false we don't want to continue
			MessageBox.Show("Please enter other expenses.")
			txtOther.BackColor = Color.Yellow
			txtOther.Focus()
			Return False
		End If

		Return True ' all is well in the world

	End Function



	' Loads an edit finances page
	Private Sub btnEditFinances_Click(sender As Object, e As EventArgs) Handles btnEditFinances.Click

		' create a new instance of the edit finances form
		Dim EditFinances As New frmEditFinances

		' show the new form so any past data is not still on the form
		EditFinances.ShowDialog()

	End Sub

End Class