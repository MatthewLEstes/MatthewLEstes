﻿Public Class frmPaymentType

	'variable for intCustomerID, restricted to this form
	Dim receiveCustomerID As Integer

	Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
		Me.Close()
	End Sub

	Public Sub New(ByVal passedCustomerID As Integer)

		'Receive current intCustomerID from whichever form called this one
		InitializeComponent()
		receiveCustomerID = passedCustomerID

	End Sub

	Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

		Dim intRadioValue As Integer
		Dim strSelect As String
		Dim strInsert As String
		Dim cmdSelect As OleDb.OleDbCommand
		Dim cmdInsert As OleDb.OleDbCommand
		Dim drSourceTable As OleDb.OleDbDataReader
		Dim intNextHighestRecordID As Integer
		Dim intRowsAffected As Integer

		If radCash.Checked = True Then

			intRadioValue = 1

		ElseIf radCreditDebit.Checked = True Then

			intRadioValue = 2

		ElseIf radCheck.Checked = True Then

			intRadioValue = 3

		ElseIf radBankTransfer.Checked = True Then

			intRadioValue = 4

		End If


		Try

			If OpenDatabaseConnectionSQLServer() = False Then


				'Alert if no connection
				MessageBox.Show(Me, "Database connection error." & vbNewLine &
										"The application will now close.",
										Me.Text + " Error",
										MessageBoxButtons.OK, MessageBoxIcon.Error)

				'Close Form
				Me.Close()

			End If


			strSelect = "SELECT MAX(intCustomerPaymentID) + 1 AS intNextHighestRecordID FROM TCustomerPaymentTypes"

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

				'Not empty, add 
				intNextHighestRecordID = CInt(drSourceTable.Item(0))

			End If


			'Update statement
			strInsert = "Insert into TCustomerPaymentTypes Values (" & intNextHighestRecordID & ", " & receiveCustomerID & ", " & intRadioValue & ")"


			' execute the statement
			cmdInsert = New OleDb.OleDbCommand(strInsert, m_conAdministrator)

			' Insert the row
			intRowsAffected = cmdInsert.ExecuteNonQuery()

			' have to let the user know what happened 
			If intRowsAffected >= 1 Then

				'MessageBox.Show("Update successful")

				' close the database connection
				CloseDatabaseConnection()

				'Determine if another form is required to open
				If intRadioValue = 2 Then

					' create a new instance of the add credit card form, passing current intCustomerID
					Dim CreditCard As New frmAddCreditCard(receiveCustomerID)

					'Make Payment Type invisible
					Me.Visible = False

					' show the new form so any past data is not still on the form
					CreditCard.ShowDialog()


				ElseIf intRadioValue >= 3 Then

					' create a new instance of the add bank account form, passing current intCustomerID
					Dim BankAccount As New frmAddBankAccount(receiveCustomerID)

					'Make Payment Type invisible
					Me.Visible = False

					' show the new form so any past data is not still on the form
					BankAccount.ShowDialog()

				End If


			Else
				MessageBox.Show("Update failed")

				' close the database connection
				CloseDatabaseConnection()
			End If

			Me.Close()


		Catch ex As Exception
			'unhandled exception
			MessageBox.Show(ex.Message)
		End Try

	End Sub

	Private Sub frmPaymentType_Load(sender As Object, e As EventArgs) Handles MyBase.Load



	End Sub
End Class