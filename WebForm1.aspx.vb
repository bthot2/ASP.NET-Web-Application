Imports System.Data.OleDb
Imports System.Data.SqlClient

Public Class WebForm1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Response.Write("Hello")
        Dim strUserName As String = Environment.UserName
        Session("userName") = strUserName
        LoadGridView()
        If Session("gridView") = False Then
            gridViewTickets.Visible = False
        Else
            gridViewTickets.Visible = True
        End If
        If Session("gridViewSQL") = False Then
            GridViewTicketsSQL.Visible = False
        Else
            GridViewTicketsSQL.Visible = True
        End If

    End Sub

    Private Sub LoadGridView()
        Dim myConnection As SqlConnection
        Dim myCommand As SqlCommand
        Dim sqlds As DataSet = New DataSet

        myConnection = New SqlConnection()
        myCommand = New SqlCommand()
        myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConnection").ConnectionString

        Dim strSQL As String = "select * from Tickets"

        myCommand.CommandText = strSQL
        myCommand.CommandType = CommandType.Text
        myCommand.Connection = myConnection
        myConnection.Open()
        Dim da As SqlDataAdapter = New SqlDataAdapter(myCommand)
        da.Fill(sqlds)
        gridViewTickets.DataSource = sqlds.Tables(0)
        Session("sort") = sqlds.Tables(0)
        gridViewTickets.DataBind()

        myConnection.Close()
        myCommand.Dispose()
        myConnection.Dispose()
    End Sub

    Private Sub gridViewTickets_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gridViewTickets.SelectedIndexChanged
        Dim row As GridViewRow = gridViewTickets.SelectedRow()
        Dim strSelected As String = String.Empty
        strSelected = "Ticket Number: " + row.Cells(1).Text
        strSelected += " | License Plate: " + row.Cells(2).Text
        strSelected += " | License Slate: " + row.Cells(3).Text
        strSelected += " | Meter Location: " + row.Cells(4).Text
        SelectedRow.Text = strSelected
        gridViewTickets.Visible = False
        Session("gridView") = False
    End Sub

    Protected Sub LinkButton1_Click(sender As Object, e As EventArgs)
        SqlDataSource1.InsertParameters("VehicleType").DefaultValue = CType(GridViewTicketsSQL.FooterRow.FindControl("vehicleType"), TextBox).Text
        SqlDataSource1.InsertParameters("TicketNumber").DefaultValue = CType(GridViewTicketsSQL.FooterRow.FindControl("ticketNumber"), TextBox).Text
    End Sub

    Protected Sub gridViewTickets_Sorting(sender As Object, e As GridViewSortEventArgs)
        'gridViewTickets.Sort(e.SortExpression, SortDirection.Descending)
        Dim dt = TryCast(Session("sort"), DataTable)
        dt.DefaultView.Sort = e.SortExpression & " " & GetDirection(e.SortExpression)
        gridViewTickets.DataSource = Session("sort")
        gridViewTickets.DataBind()
    End Sub


    Private Function GetDirection(ByVal column As String) As String
        ' By default, set the sort direction to ascending.
        Dim sortDirection = "ASC"
        ' Retrieve the last column that was sorted.
        Dim sortExpression = TryCast(ViewState("SortExpression"), String)
        If sortExpression Is Nothing Then
            sortExpression = "ticketNumber"
        End If
        If sortExpression IsNot Nothing Then
            ' Check if the same column is being sorted.
            ' Otherwise, the default value can be returned.
            If sortExpression = column Then
                'Dim lastDirection = ViewState("SortDirection")
                Dim lastDirection = TryCast(ViewState("SortDirection"), String)
                If lastDirection Is Nothing _
          OrElse lastDirection = "ASC" Then
                    sortDirection = "DESC"
                ElseIf lastDirection = "DESC" Then
                    sortDirection = "ASC"
                End If
            End If
        End If

        ' Save new values in ViewState.
        ViewState("SortDirection") = sortDirection
        ViewState("SortExpression") = column

        Return sortDirection

    End Function

    Protected Sub gridViewTickets_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gridViewTickets.PageIndex = e.NewPageIndex
        Me.LoadGridView()
    End Sub

    Protected Sub gridViewTickets_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Dim myConnection As SqlConnection
        Dim myCommand As SqlCommand
        myConnection = New SqlConnection()
        myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConnection").ConnectionString
        myCommand = New SqlCommand()
        myCommand.CommandText = "DELETE FROM Tickets WHERE TicketNumber = @TicketNumber"
        myCommand.Connection = myConnection
        myCommand.CommandType = CommandType.Text
        Dim strTicketNumber As String = gridViewTickets.Rows(e.RowIndex).Cells(1).Text
        myCommand.Parameters.Add(“@TicketNumber”, SqlDbType.VarChar).Value = strTicketNumber
        myConnection.Open()
        myCommand.ExecuteNonQuery()
        LoadGridView()
        myConnection.Close()
        myCommand.Dispose()
        myConnection.Dispose()
    End Sub

    Public Sub gridViewTickets_RowEditing(sender As Object, e As GridViewEditEventArgs)
        gridViewTickets.EditIndex = e.NewEditIndex
        LoadGridView()
    End Sub

    Protected Sub gridViewTickets_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
        gridViewTickets.EditIndex = -1
        LoadGridView()
    End Sub

    Protected Sub gridViewTickets_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)
        Dim myconnection As SqlConnection
        Dim mycommand As SqlCommand
        myconnection = New SqlConnection()
        myconnection.ConnectionString = ConfigurationManager.ConnectionStrings("mysqlconnection").ConnectionString
        mycommand = New SqlCommand()
        mycommand.CommandText = "update tickets set violationtype=@violationtype where ticketnumber = @ticketnumber"
        mycommand.Connection = myconnection
        mycommand.CommandType = CommandType.Text
        Dim strticketnumber As String = gridViewTickets.Rows(e.RowIndex).Cells(1).Text
        Dim strviolationtype As String = CType(gridViewTickets.Rows(e.RowIndex).FindControl("textbox1"), TextBox).Text
        mycommand.Parameters.Add("@ticketnumber", SqlDbType.VarChar).Value = "4147299"
        mycommand.Parameters.Add(“@violationtype”, SqlDbType.VarChar).Value = "Consecutive"
        myconnection.Open()
        mycommand.ExecuteNonQuery()
        LoadGridView()
        myconnection.Close()
        mycommand.Dispose()
        myconnection.Dispose()
    End Sub
End Class