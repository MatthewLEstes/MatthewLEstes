﻿' Previous Customer form
' Last Updated: 11/4/2020
' Last modified by Matthew Estes

Public Class frmPreviousCustomer
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click

		' Close form
		Me.Close()

	End Sub

	Private Sub frmPreviousCustomer_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		Try

			UncheckRadios()

			Dim strSelect As String = ""
			Dim cmdSelect As OleDb.OleDbCommand
			Dim drSourceTable As OleDb.OleDbDataReader
			Dim dt As DataTable = New DataTable

			'Delete data from boxes
			For Each cntrl As Control In Controls
				If TypeOf cntrl Is TextBox Then
					cntrl.Text = String.Empty
				End If
			Next

			'Open DB
			If OpenDatabaseConnectionSQLServer() = False Then

				'If DB could not open
				MessageBox.Show(Me, "Database connection error." & vbNewLine &
									"The application will now close.",
									Me.Text + " Error",
									MessageBoxButtons.OK, MessageBoxIcon.Error)
				Me.Close()

			End If

			cboName.BeginUpdate()

			'Create select
			strSelect = "SELECT intCustomerID, ( strLastName + ', ' + strFirstName ) AS FullName FROM TCustomers ORDER BY FullName ASC"

			'Get records
			cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
			drSourceTable = cmdSelect.ExecuteReader

			'Load Table
			dt.Load(drSourceTable)

			' Add items to combo box
			cboName.ValueMember = "intCustomerID"
			cboName.DisplayMember = "FullName"
			cboName.DataSource = dt

			' Select the first item in the list by default
			If cboName.Items.Count > 0 Then cboName.SelectedIndex = 0

			' Show changes
			cboName.EndUpdate()

			' Clean up
			drSourceTable.Close()

			' close the database connection
			CloseDatabaseConnection()

		Catch ex As Exception

			'Unhandled Exception
			MessageBox.Show(ex.Message)

		End Try


	End Sub

	Private Sub cboName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboName.SelectedIndexChanged

		Try

			UncheckRadios()

			ClearLabels()

			LoadCustomerInfo()

			LoadCurrentPaymentType()

			LoadCreditInfo()

			LoadBankInfo()

			LoadJobList()

		Catch ex As Exception

			MessageBox.Show(ex.Message)
		End Try


	End Sub

	Private Sub UncheckRadios()

		'uncheck payment types
		radCash.Checked = False
		radCheck.Checked = False
		radCreditDebit.Checked = False
		radBankTransfer.Checked = False
		'uncheck card types
		radAmericanExpress.Checked = False
		radMastercard.Checked = False
		radVisa.Checked = False

	End Sub

	Private Sub ClearLabels()

		'Credit / Debit label clear
		lblCardholderName.ResetText()
		lblCardNumber.ResetText()
		lblExpiration.ResetText()
		lblCVCNumber.ResetText()

		'Bank Label Clear
		lblRoutingNumber.ResetText()
		lblAccountNumber.ResetText()
		lblAcccountType.ResetText()
		lblNameOnAccount.ResetText()


	End Sub

	Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click

		Try

			ComboBoxSearch()

		Catch ex As Exception

			'Unhandled Exception
			MessageBox.Show(ex.Message)

		End Try

	End Sub

	Private Sub cboName_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cboName.KeyPress

		'If keypress return
		Dim tb As ComboBox = CType(sender, ComboBox)
		If Char.IsControl(e.KeyChar) Then
			If e.KeyChar.Equals(Chr(Keys.Return)) Then

				Try

					'initiante search
					ComboBoxSearch()

				Catch ex As Exception

					'Unhandled Exception
					MessageBox.Show(ex.Message)

				End Try

			End If
		End If

	End Sub

	Private Sub LoadCustomerInfo()

		Dim strAddress As String = ""
		Dim strSelect As String = ""
		Dim strName As String = ""
		Dim cmdSelect As OleDb.OleDbCommand 'Select
		Dim drSourceTable As OleDb.OleDbDataReader 'retrieved data
		Dim dt As DataTable = New DataTable 'reader

		'open the database
		If OpenDatabaseConnectionSQLServer() = False Then


			' No connection error
			MessageBox.Show(Me, "Database connection error." & vbNewLine &
								"The application will now close.",
								Me.Text + " Error",
								MessageBoxButtons.OK, MessageBoxIcon.Error)

			'close the form
			Me.Close()

		End If

		'Select statement
		strSelect = "SELECT * FROM vCustomers WHERE intCustomerID = " & cboName.SelectedValue.ToString


		'Retrieve records 
		cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
		drSourceTable = cmdSelect.ExecuteReader

		'load the data table from the reader
		dt.Load(drSourceTable)


		'strAddress = 

		'populate text boxes
		lblFirstName.Text = dt.Rows(0).Item(1).ToString
		lblLastName.Text = dt.Rows(0).Item(2).ToString
		lblAddress.Text = dt.Rows(0).Item(3).ToString & Environment.NewLine & dt.Rows(0).Item(4).ToString & ", " & dt.Rows(0).Item(5).ToString & ", " & dt.Rows(0).Item(6).ToString
		lblPhone.Text = dt.Rows(0).Item(7).ToString
		lblEmail.Text = dt.Rows(0).Item(8).ToString

		'close connection
		CloseDatabaseConnection()

	End Sub

	Private Sub LoadCurrentPaymentType()

		Dim strSelect As String = ""
		Dim strName As String = ""
		Dim cmdSelect As OleDb.OleDbCommand 'Select
		Dim drSourceTable As OleDb.OleDbDataReader 'retrieved data
		Dim dt As DataTable = New DataTable 'reader

		'open the database
		If OpenDatabaseConnectionSQLServer() = False Then


			' No connection error
			MessageBox.Show(Me, "Database connection error." & vbNewLine &
								"The application will now close.",
								Me.Text + " Error",
								MessageBoxButtons.OK, MessageBoxIcon.Error)

			'close the form
			Me.Close()

		End If

		'Select statement
		strSelect = "SELECT TOP 1 * FROM TCustomerPaymentTypes WHERE intCustomerID = " & cboName.SelectedValue.ToString & " ORDER BY intCustomerPaymentID DESC"

		'Retrieve records 
		cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
		drSourceTable = cmdSelect.ExecuteReader

		'load the data table from the reader
		dt.Load(drSourceTable)

		'Verify any rows were added to dt
		If dt.Rows.Count > 0 Then

			'set radio button for most recent payment type
			If dt.Rows(0).Item(2).ToString = "1" Then
				radCash.Checked = True
			ElseIf dt.Rows(0).Item(2).ToString = "2" Then
				radCreditDebit.Checked = True
			ElseIf dt.Rows(0).Item(2).ToString = "3" Then
				radCheck.Checked = True
			ElseIf dt.Rows(0).Item(2).ToString = "4" Then
				radBankTransfer.Checked = True
			End If

		End If

		'close connection
		CloseDatabaseConnection()

	End Sub

	Private Sub LoadCreditInfo()

		Dim strSelect As String = ""
		Dim strName As String = ""
		Dim cmdSelect As OleDb.OleDbCommand 'Select
		Dim drSourceTable As OleDb.OleDbDataReader 'retrieved data
		Dim dt As DataTable = New DataTable 'reader

		'open the database
		If OpenDatabaseConnectionSQLServer() = False Then


			' No connection error
			MessageBox.Show(Me, "Database connection error." & vbNewLine &
								"The application will now close.",
								Me.Text + " Error",
								MessageBoxButtons.OK, MessageBoxIcon.Error)

			'close the form
			Me.Close()

		End If

		'Select statement
		strSelect = "SELECT TOP 1 * FROM TCreditCards WHERE intCustomerID = " & cboName.SelectedValue.ToString & " ORDER BY intCreditCardID DESC"

		'Retrieve records 
		cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
		drSourceTable = cmdSelect.ExecuteReader

		'load the data table from the reader
		dt.Load(drSourceTable)

		'Verify any rows were added to dt
		If dt.Rows.Count > 0 Then

			'populate text boxes
			lblCardholderName.Text = dt.Rows(0).Item(2).ToString
			lblCardNumber.Text = dt.Rows(0).Item(3).ToString
			lblExpiration.Text = dt.Rows(0).Item(4).ToString
			lblCVCNumber.Text = dt.Rows(0).Item(5).ToString
			'Set radio button
			If dt.Rows(0).Item(6).ToString = "1" Then
				radAmericanExpress.Checked = True
			ElseIf dt.Rows(0).Item(6).ToString = "2" Then
				radMastercard.Checked = True
			ElseIf dt.Rows(0).Item(6).ToString = "3" Then
				radVisa.Checked = True
			End If

		End If

		'close connection
		CloseDatabaseConnection()

	End Sub

	Private Sub LoadBankInfo()

		Dim strSelect As String = ""
		Dim strName As String = ""
		Dim cmdSelect As OleDb.OleDbCommand 'Select
		Dim drSourceTable As OleDb.OleDbDataReader 'retrieved data
		Dim dt As DataTable = New DataTable 'reader

		'open the database
		If OpenDatabaseConnectionSQLServer() = False Then


			' No connection error
			MessageBox.Show(Me, "Database connection error." & vbNewLine &
								"The application will now close.",
								Me.Text + " Error",
								MessageBoxButtons.OK, MessageBoxIcon.Error)

			'close the form
			Me.Close()

		End If

		'Select statement
		strSelect = "SELECT TOP 1 * FROM vBankAccount WHERE intCustomerID = " & cboName.SelectedValue.ToString & " ORDER BY intBankAccountID DESC"

		'Retrieve records 
		cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
		drSourceTable = cmdSelect.ExecuteReader

		'load the data table from the reader
		dt.Load(drSourceTable)

		'Verify any rows were added to dt
		If dt.Rows.Count > 0 Then

			'populate text boxes
			lblRoutingNumber.Text = dt.Rows(0).Item(2).ToString
			lblAccountNumber.Text = dt.Rows(0).Item(3).ToString
			lblAcccountType.Text = dt.Rows(0).Item(4).ToString
			lblNameOnAccount.Text = dt.Rows(0).Item(5).ToString

		End If

		'close connection
		CloseDatabaseConnection()

	End Sub

	Private Sub LoadJobList()

		Dim strSelect As String = ""
		Dim cmdSelect As OleDb.OleDbCommand
		Dim drSourceTable As OleDb.OleDbDataReader
		Dim dt As DataTable = New DataTable


		'Open DB
		If OpenDatabaseConnectionSQLServer() = False Then

			'If DB could not open
			MessageBox.Show(Me, "Database connection error." & vbNewLine &
									"The application will now close.",
									Me.Text + " Error",
									MessageBoxButtons.OK, MessageBoxIcon.Error)
			Me.Close()

		End If

		'Create select
		strSelect = "SELECT JobNumber, strJobDesc AS Description, dtStartDate AS StartDate, dtendDate AS EndDate FROM vJobRecordStatus WHERE intCustomerID = " & cboName.SelectedValue & " ORDER BY intJobRecordID DESC"

		'Get records
		cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
		drSourceTable = cmdSelect.ExecuteReader

		'Load Table
		dt.Load(drSourceTable)

		dgvJobs.DataSource = dt


		' Clean up
		drSourceTable.Close()

		' close the database connection
		CloseDatabaseConnection()




	End Sub

	Private Sub ComboBoxSearch()

		UncheckRadios()

		Dim strSelect As String = ""
		Dim cmdSelect As OleDb.OleDbCommand
		Dim drSourceTable As OleDb.OleDbDataReader
		Dim dt As DataTable = New DataTable

		'Delete data from boxes
		For Each cntrl As Control In Controls
			If TypeOf cntrl Is TextBox Then
				cntrl.Text = String.Empty
			End If
		Next

		'Open DB
		If OpenDatabaseConnectionSQLServer() = False Then

			'If DB could not open
			MessageBox.Show(Me, "Database connection error." & vbNewLine &
								"The application will now close.",
								Me.Text + " Error",
								MessageBoxButtons.OK, MessageBoxIcon.Error)
			Me.Close()

		End If

		cboName.BeginUpdate()

		'Create select
		strSelect = "SELECT intCustomerID, ( strLastName + ', ' + strFirstName ) AS FullName FROM TCustomers WHERE ( strLastName + ', ' + strFirstName ) LIKE '%" & cboName.Text &
			"%' OR (strFirstName + ' ' + strLastName) like '%" & cboName.Text & "%' ORDER BY FullName ASC"

		'Get records
		cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
		drSourceTable = cmdSelect.ExecuteReader

		'Load Table
		dt.Load(drSourceTable)

		' Add items to combo box
		cboName.ValueMember = "intCustomerID"
		cboName.DisplayMember = "FullName"
		cboName.DataSource = dt

		' Select the first item in the list by default
		If cboName.Items.Count > 0 Then cboName.SelectedIndex = 0

		' Show changes
		cboName.EndUpdate()

		' Clean up
		drSourceTable.Close()

		' close the database connection
		CloseDatabaseConnection()

	End Sub

	Private Sub btnEditContact_Click(sender As Object, e As EventArgs) Handles btnEditContact.Click

		Dim UpdateCustomer As New frmUpdateCustomerInfo(cboName.SelectedValue)

		' show the new form so any past data is not still on the form
		UpdateCustomer.ShowDialog()

		'Relead customer info after update
		LoadCustomerInfo()

	End Sub

	Private Sub btnUpdatePayment_Click(sender As Object, e As EventArgs) Handles btnUpdatePayment.Click

		' create a new instance of the customer intake form, passing current intCustomerID
		Dim CustomerPayment As New frmPaymentType(cboName.SelectedValue)

		' show the new form so any past data is not still on the form
		CustomerPayment.ShowDialog()

		'Reload payments
		LoadCurrentPaymentType()

		'Reload cards
		LoadCreditInfo()

		'Reload banks
		LoadBankInfo()

	End Sub


	Private Sub dgvJobs_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvJobs.CellDoubleClick

		'Check to make sure it is not a DBNull value
		If IsDBNull(dgvJobs.CurrentRow.Cells(0).Value) = False Then

			' create a new instance of the job information form, passing current job ID
			Dim JobDetails As New frmJobDetails(dgvJobs.CurrentRow.Cells(0).Value)

			' show the new form so any past data is not still on the form
			JobDetails.ShowDialog()

		End If

	End Sub
End Class