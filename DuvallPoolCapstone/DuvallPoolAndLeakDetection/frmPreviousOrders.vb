' Previous Orders form
' Last Updated: 12/12/2020
' Last modified by Matthew Estes

Public Class frmPreviousOrders

	' Close form
	Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click

		' Close form
		Me.Close()

	End Sub



	' Runs when program is loaded
	Private Sub frmPreviousOrders_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		Try
			' Load parts
			LoadParts()
		Catch ex As Exception

			'Unhandled Exception
			MessageBox.Show(ex.Message)

		End Try

	End Sub



	' Load part list
	Private Sub LoadParts()

		Try
			' Declare variables
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

			cboPartName.BeginUpdate()

			'Create select
			strSelect = "SELECT intPartID, strPartName FROM TParts ORDER BY strPartName ASC"

			'Get records
			cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
			drSourceTable = cmdSelect.ExecuteReader

			'Load Table
			dt.Load(drSourceTable)

			' Add items to combo box
			cboPartName.ValueMember = "intPartID"
			cboPartName.DisplayMember = "strPartName"
			cboPartName.DataSource = dt

			' Select the first item in the list by default
			If cboPartName.Items.Count > 0 Then cboPartName.SelectedIndex = 0

			' Show changes
			cboPartName.EndUpdate()

			' Clean up
			drSourceTable.Close()

			' close the database connection
			CloseDatabaseConnection()

		Catch ex As Exception

			'Unhandled Exception
			MessageBox.Show(ex.Message)

		End Try
	End Sub


	' Load order list
	Private Sub LoadOrders()

		Try
			' Declare variables
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

			cboOrderNumber.BeginUpdate()

			'Create select
			strSelect = "SELECT intPartOrderedID, PONumber FROM TPartsOrders WHERE intPartID = " & cboPartName.SelectedValue & " ORDER BY PONumber DESC"

			'Get records
			cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
			drSourceTable = cmdSelect.ExecuteReader

			'Load Table
			dt.Load(drSourceTable)

			' Add items to combo box
			cboOrderNumber.ValueMember = "intPartOrderedID"
			cboOrderNumber.DisplayMember = "PONumber"
			cboOrderNumber.DataSource = dt

			' Select the first item in the list by default
			If cboOrderNumber.Items.Count > 0 Then cboOrderNumber.SelectedIndex = 0

			' Show changes
			cboOrderNumber.EndUpdate()

			' Clean up
			drSourceTable.Close()

			' close the database connection
			CloseDatabaseConnection()

		Catch ex As Exception

			'Unhandled Exception
			MessageBox.Show(ex.Message)

		End Try
	End Sub



	' Runs when PartName selected is changed
	Private Sub cboPartName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPartName.SelectedIndexChanged
		' Load orders related to that part
		LoadOrders()
	End Sub



	' Runs when OrderNumber selected is changed
	Private Sub cboOrderNumber_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboOrderNumber.SelectedIndexChanged

		Try
			' Declare variables
			Dim strSelect As String = ""
			Dim strPartName As String = ""
			Dim intArrived As Integer
			Dim cmdSelect As OleDb.OleDbCommand
			Dim drSourceTable As OleDb.OleDbDataReader
			Dim dt As DataTable = New DataTable

			'Open the DB
			If OpenDatabaseConnectionSQLServer() = False Then

				' No connection error
				MessageBox.Show(Me, "Database connection error." & vbNewLine &
									"The application will now close.",
									Me.Text + " Error",
									MessageBoxButtons.OK, MessageBoxIcon.Error)

				'close the form
				Me.Close()

			End If

			'SELECT
			strSelect = "SELECT * FROM vPartOrders WHERE intPartOrderedID = " & cboOrderNumber.SelectedValue.ToString

			'Retrieve
			cmdSelect = New OleDb.OleDbCommand(strSelect, m_conAdministrator)
			drSourceTable = cmdSelect.ExecuteReader

			'Load from table
			dt.Load(drSourceTable)

			'Populate labels and CBO
			lblSerialNumber.Text = dt.Rows(0).Item(1).ToString
			lblPartDescription.Text = dt.Rows(0).Item(2).ToString
			lblQuantity.Text = dt.Rows(0).Item(3).ToString
			lblTotalPurchaseCost.Text = "$" + dt.Rows(0).Item(4).ToString
			lblVendorName.Text = dt.Rows(0).Item(5).ToString
			lblContactName.Text = dt.Rows(0).Item(6).ToString
			lblAddress.Text = dt.Rows(0).Item(7).ToString
			lblCity.Text = dt.Rows(0).Item(8).ToString
			lblState.Text = dt.Rows(0).Item(9).ToString
			lblZip.Text = dt.Rows(0).Item(10).ToString
			lblEmail.Text = dt.Rows(0).Item(11).ToString
			lblPhone.Text = dt.Rows(0).Item(12).ToString
			lblDateOrdered.Text = dt.Rows(0).Item(13).ToString
			lblDateArrived.Text = dt.Rows(0).Item(14).ToString
			intArrived = CInt(dt.Rows(0).Item(15))

			' Check if it is arrived
			If intArrived = 1 Then

				' Set delivered to true
				radDelivered.Checked = True
			Else

				' Set delivered to false
				radTransit.Checked = True
			End If
			'Close DB connection
			CloseDatabaseConnection()

		Catch ex As Exception

		End Try
	End Sub



	' Update part orders
	Private Sub btnEditPartOrders_Click(sender As Object, e As EventArgs) Handles btnEditPartOrders.Click

		Dim EditPartOrders As New frmEditPartOrders()

		'Show the New form so any past data Is Not still on the form
		EditPartOrders.ShowDialog()

		'Relead customer info after update
		LoadOrders()

	End Sub
End Class