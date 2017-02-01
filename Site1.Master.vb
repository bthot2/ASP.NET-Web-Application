Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Net.Mail
Imports System.Data.SqlClient

Public Class Site1
    Inherits System.Web.UI.MasterPage

    'Server Control Variables Starts
    Dim strChecked As String = ""
    Dim strDropDownSelected As String = ""
    Dim strNeedFreeTrail As String = ""
    Dim strSelectedPayment As String = ""
    Dim strFirstName As String = ""
    Dim strLastName As String = ""
    Dim strEmail As String = ""
    Dim strUserName As String = ""
    Dim strLoginId As String = String.Empty
    Dim dtChangeLast As Date = Date.Now
    'Server Control variables ends

    'File Variables
    Dim strFileName As String = "Tickets.txt"
    Dim filePath As String = System.AppDomain.CurrentDomain.BaseDirectory()
    Dim strRestOfString As String = String.Empty
    Dim found As Integer
    Dim strTemp As String = String.Empty
    Dim strLine As String = String.Empty
    Dim ticketNumber As String = String.Empty
    Dim vehicleType As String = String.Empty
    Dim vehicleMake As String = String.Empty
    Dim licensePlate As String = String.Empty
    Dim licenseState As String = String.Empty
    Dim slashZero As String = String.Empty
    Dim blockNumber As String = String.Empty
    Dim street As String = String.Empty
    Dim ticketDate As String = String.Empty
    Dim ticketTime As String = String.Empty
    Dim meterLocation As String = String.Empty
    Dim issuedBy As String = String.Empty
    Dim unknown1 As String = String.Empty
    Dim originalFine As String = String.Empty
    Dim FourteenDayFine As String = String.Empty
    Dim ticketDay As String = String.Empty
    Dim unknown2 As String = String.Empty
    Dim violationCode As String = String.Empty
    Dim violationType As String = String.Empty
    Dim unknown3 As String = String.Empty
    Dim unknown4 As String = String.Empty
    Dim unknown5 As String = String.Empty
    Dim unknown6 As String = String.Empty
    Dim unknown7 As String = String.Empty
    Dim unknown8 As String = String.Empty
    Dim postedLimit As String = String.Empty
    Dim Counter As Integer = 0
    'File Variables ends


    'Mail Subject
    Dim mailSubject As String = String.Empty

    'Connection Variables Starts
    Dim myConnection As OleDbConnection
    Dim myCommand As OleDbCommand
    Dim myReader As OleDbDataReader
    Dim strAccess As String = ""
    Dim strSQL As String = ""

    'Providing the connection
    Protected Sub Connection()
        Dim strPath As String = AppDomain.CurrentDomain.BaseDirectory()
        Session("path") = strPath
        strPath = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + strPath + "Lynda.accdb"
        Session("access") = strPath
        strAccess = Session("access")
        myConnection = New OleDbConnection(strAccess)
        myCommand = New OleDbCommand()
    End Sub

    'Implementation of Server controls for insert 
    Protected Sub Checkout_Click(sender As Object, e As EventArgs)
        mailSubject = "Insert"
        strFirstName = Firstname.Text
        strLastName = Lastname.Text
        strEmail = Email.Text
        strLoginId = Login.Text
        Session("UserName") = Environment.UserName
        strUserName = Session("UserName")
        Connection()
        Insert()
        myCommand.Dispose()
        myConnection.Dispose()
        createEMail(mailSubject)
        Clear_Click(Nothing, Nothing)
    End Sub

    'Inserting the Data
    Protected Sub Insert()
        strSQL = "insert into UserValues(FirstName, LastName, Email, LoginID, InterestedCourses, Premium, NeedTrail, Payment, UserName, DateTimeChangeLast) Values ('" + strFirstName + "','" + strLastName + "','" + strEmail + "', '" + strLoginId + "', '" + strChecked + "','" + strDropDownSelected + "','" + strNeedFreeTrail + "', '" + strSelectedPayment + "','" + strUserName + "','" + dtChangeLast + "')"
        myCommand = New OleDbCommand(strSQL, myConnection)
        myCommand.Connection.Open()
        myCommand.ExecuteNonQuery()
        myCommand.Dispose()
        myConnection.Dispose()
    End Sub

    'Getting the Data from Check box
    Public Sub InterestedCourses_SelectedIndexChanged(sender As Object, e As EventArgs)
        For Each item In InterestedCourses.Items()
            If item.Selected Then
                strChecked += item.Text + ";"
            End If
        Next
    End Sub

    'Getting Data from Radio button
    Protected Sub NeedFreeTrial_SelectedIndexChanged(sender As Object, e As EventArgs)
        If NeedTrailYes.Checked Then
            strNeedFreeTrail = "Yes"
        Else
            strNeedFreeTrail = "No"
        End If
    End Sub

    'Retrieving Data from Database
    Protected Sub obtainAnswers()
        Session("UserName") = Environment.UserName
        strUserName = Session("UserName")
        strSQL = "select  top 1 * from UserValues where UserName = '" + strUserName + "' order by DateTimeChangeLast desc"
        myCommand = New OleDbCommand()
        myCommand.CommandText = strSQL
        myCommand.CommandType = CommandType.Text
        myConnection = New OleDbConnection(strAccess)
        myCommand.Connection = myConnection
        myCommand.Connection.Open()
        myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
        'Printing the values
        While (myReader.Read())
            strFirstName = myReader("FirstName")
            strLastName = myReader("LastName")
            strEmail = myReader("Email")
            strLoginId = myReader("LoginID")
            strChecked = myReader("InterestedCourses")
            strDropDownSelected = myReader("Premium")
            strNeedFreeTrail = myReader("NeedTrail")
            strSelectedPayment = myReader("Payment")
        End While
        'Radio Buttons group-1
        If strNeedFreeTrail = "Yes" Then
            NeedTrailYes.Checked = True
        ElseIf strNeedFreeTrail = "No" Then
            NeedTrailNo.Checked = True
        Else
            NeedTrailYes.Checked = False
            NeedTrailNo.Checked = False

        End If
        'Radio Button group-2
        If strSelectedPayment = "Debit Card" Then
            DebitCard.Selected = True
        ElseIf strSelectedPayment = "Credit Card" Then
            CreditCard.Selected = True
        ElseIf strSelectedPayment = "E-Check" Then
            ECheck.Selected = True
        Else
            DebitCard.Selected = False
            CreditCard.Selected = False
            ECheck.Selected = False
        End If
        'text boxes
        Firstname.Text = strFirstName
        Lastname.Text = strLastName
        Email.Text = strEmail
        Login.Text = strLoginId
        'Check Box
        Dim strTemp As String = String.Empty
        Dim seperatorFound As Integer
        While strChecked.Length > 0
            seperatorFound = InStr(strChecked, ";")
            If seperatorFound > 0 Then
                strTemp = Left(strChecked, seperatorFound - 1)
                strChecked = Mid(strChecked, seperatorFound + 1)
            End If
            InterestedCourses.Items.FindByText(strTemp).Selected = True
        End While

        'Drop Down List
        Premium.SelectedValue = strDropDownSelected

    End Sub
    'Obtaining values from database
    Protected Sub obtainEvents()
        Dim strEventName As String = String.Empty
        Connection()
        myCommand.CommandText = "select * from UserRetrieval "
        myCommand.CommandType = CommandType.Text
        myCommand.Connection = myConnection
        myCommand.Connection.Open()
        myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)
        Premium.Items.Add("Select the Membership plan")
        While (myReader.Read())
            strEventName = Trim("" + myReader("Event"))
            Premium.Items.Add(strEventName)
        End While

        myReader.Close()
        myCommand.Dispose()
        myConnection.Dispose()
    End Sub
    'On Pageloads Sub routine
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strUserName As String = ""
        Dim strPath As String = AppDomain.CurrentDomain.BaseDirectory()
        strUserName = Session("userName")
        If Len(strUserName) = 0 Then
            Response.Redirect("Error.aspx")
        End If
        System.IO.File.WriteAllText(strPath + "Output.txt", "")
        System.IO.File.WriteAllText(strPath + "OutputSplit.txt", "")
        'ReadTextSplitMethod()
        'ReadTextFile()
        'DropTable("Tickets")
        CreateTable("Tickets")
        InsertSQL()
        If Not IsPostBack Then
            obtainEvents()
            obtainAnswers()
        End If
        CheckBox1.Attributes.Add("onClick", "dropDownDisplay()")



    End Sub


    'Getting Data from from drop boxes
    Protected Sub Payment_SelectedIndexChanged(sender As Object, e As EventArgs)
        strSelectedPayment = Payment.SelectedItem.Value
    End Sub

    'Clears  the data entered in the form
    Protected Sub Clear_Click(sender As Object, e As EventArgs)
        If mailSubject = "Insert" Or mailSubject = "Delete" Or mailSubject = "Update" Then
            Clear_On_Click()

        Else
            Clear_On_Click()
            body.Text = String.Empty
        End If

    End Sub
    Public Sub Clear_On_Click()
        Firstname.Text = String.Empty
        Lastname.Text = String.Empty
        Email.Text = String.Empty
        Login.Text = String.Empty
        Ajax.Selected = False
        Php.Selected = False
        Azure.Selected = False
        Hadoop.Selected = False
        NoSql.Selected = False
        NeedTrailNo.Checked = False
        NeedTrailYes.Checked = False
        DebitCard.Selected = False
        CreditCard.Selected = False
        ECheck.Selected = False
        Premium.ClearSelection()
    End Sub

    Protected Sub Update_Click(sender As Object, e As EventArgs)
        mailSubject = "Update"
        strFirstName = Firstname.Text
        strLastName = Lastname.Text
        strEmail = Email.Text
        strLoginId = Login.Text
        For Each item In InterestedCourses.Items()
            If item.Selected Then
                strChecked += item.Text + ";"
            End If
        Next
        NeedFreeTrial_SelectedIndexChanged(Nothing, Nothing)

        'Drop down
        strSelectedPayment = Premium.SelectedItem.Value
        Payment_SelectedIndexChanged(Nothing, Nothing)

        Session("UserName") = Environment.UserName
        strUserName = Session("UserName")
        Connection()
        strSQL = "update UserValues set FirstName='" + strFirstName + "',LastName='" + strLastName + "', Email='" + strEmail + "', InterestedCourses='" + strChecked + "', Premium='" + strDropDownSelected + "', NeedTrail='" + strNeedFreeTrail + "', Payment='" + strSelectedPayment + "' where UserName='" + strUserName + "' "
        myCommand = New OleDbCommand(strSQL, myConnection)
        myCommand.Connection.Open()
        myCommand.ExecuteNonQuery()
        myCommand.Dispose()
        myConnection.Dispose()
        createEMail(mailSubject)
        Clear_Click(Nothing, Nothing)

    End Sub

    Protected Sub Delete_Click(sender As Object, e As EventArgs)
        mailSubject = "Delete"
        Session("UserName") = Environment.UserName
        strUserName = Session("UserName")
        Connection()
        strSQL = "delete from UserValues where UserName='" + strUserName + "'"
        myCommand = New OleDbCommand(strSQL, myConnection)
        myCommand.Connection.Open()
        myCommand.ExecuteNonQuery()
        myCommand.Dispose()
        myConnection.Dispose()
        createEMail(mailSubject)
        Clear_Click(Nothing, Nothing)


    End Sub
    Public Sub createEMail(ByVal strSubject As String)
        Dim strMailFrom As String = "rjack01s@uis.edu"
        Dim strMailTo As String = "rjack01s@uis.edu"
        Dim strEmailName As String = "rjack01s@uis.edu"
        Dim mail As New MailMessage(strMailFrom, strMailTo)
        mail.Subject = strSubject
        mail.IsBodyHtml = True
        If mailSubject = "Insert" Then
            mail.Body = "<span style='color:White;font-family:Verdana; font-size: 24px'>"
            mail.Body += mailSubject
            mail.Body += ":You Inserted the Record with the following details</span><br/>"
            mailBody(mail)
        ElseIf mailSubject = "Update" Then
            mail.Body = "<span style='color:White;font-family:Verdana; font-size: 24px'>"
            mail.Body += mailSubject
            mail.Body += ":You Updated the Record with the following details</span>"
            mailBody(mail)
        ElseIf mailSubject = "Delete" Then
            mail.Body = mailSubject
            mail.Body += "<br/><span style='color:red'> You Deleted the user with ID:&nbsp; "
            mail.Body += strUserName
            mail.Body += "</span>"
        End If

        Dim mySMTP As New SmtpClient
        mySMTP.Host = "webmail.uis.edu"
        'mySMTP.Send(mail)
        body.Text = mail.Body
    End Sub
    Public Sub mailBody(ByVal mail)

        'FirstName
        mail.Body += "<br/>"
        mail.Body += "<span style='color:black;font-family:Verdana; font-size: 24px'>FirstName:&nbsp;"
        mail.Body += strFirstName
        mail.Body += "</span>"
        'LastName
        mail.Body += "<br/>"
        mail.Body += "<span style='color:black;font-family:Verdana; font-size: 24px'>LastName:&nbsp;"
        mail.Body += strLastName
        mail.Body += "</span>"
        'Email
        mail.Body += "<br/>"
        mail.Body += "<span style='color:black;font-family:Verdana; font-size: 24px'>Email:&nbsp;"
        mail.Body += strEmail
        mail.Body += "</span>"
        'LoginID
        mail.Body += "<br/>"
        mail.Body += "<span style='color:black;font-family:Verdana; font-size: 24px'>LoginID:&nbsp;"
        mail.Body += strLoginId
        mail.Body += "</span>"
        'Interested Coures
        mail.Body += "<br/>"
        mail.Body += "<span style='color:blue;font-family:Verdana; font-size: 24px'>Interested Courses:&nbsp;"
        If strChecked.Length > 0 Then
            mail.Body += strChecked
        Else
            mail.Body += "No Course is interested"
        End If
        mail.Body += "</span>"
        'Premium you want
        mail.Body += "<br/>"
        mail.Body += "<span style='color:green;font-family:Verdana; font-size: 24px'>Premium You Want:&nbsp;"
        If strDropDownSelected.Length > 0 Then
            mail.Body += strDropDownSelected
        Else
            mail.Body += "You dont want any Premium as you didnt selected any"
        End If
        mail.Body += "</span>"
        'Need A Free Trail
        mail.Body += "<br/>"
        mail.Body += "<span style='color:red;font-family:Verdana; font-size: 24px'>Need A Free Trail:&nbsp;"
        If strNeedFreeTrail.Length > 0 Then
            mail.Body += strNeedFreeTrail
        Else
            mail.Body += "You did not selected my trail option"
        End If
        mail.Body += "</span>"
        'Payment Option
        mail.Body += "<br/>"
        mail.Body += "<span style='color:red;font-family:Verdana; font-size: 24px'>Payment:&nbsp;"
        If strSelectedPayment.Length > 0 Then
            mail.Body += strSelectedPayment
        Else
            mail.Body += "You did not selected my Payment option"
        End If
        mail.Body += "</span>"
    End Sub
    Public Sub ReadTextFile()
        Dim totalFine As Double = 0.0
        Dim strFileName As String = "Tickets.txt"
        Dim fileNameToBeStored = "Output.txt"
        Dim filePath As String = System.AppDomain.CurrentDomain.BaseDirectory()
        Dim fileParse As StreamReader = New StreamReader(filePath + strFileName)
        Do
            strLine = fileParse.ReadLine
            strRestOfString = strLine
            If (strRestOfString <> Nothing) Then
                Do While strRestOfString.Length > 0
                    found = InStr(strRestOfString, ";")
                    If found > 0 Then
                        Select Case Counter
                            Case 0
                                ticketNumber = Left(strRestOfString, found - 1)
                            Case 1
                                vehicleType = Left(strRestOfString, found - 1)
                            Case 2
                                vehicleMake = Left(strRestOfString, found - 1)
                            Case 3
                                licensePlate = Left(strRestOfString, found - 1)
                            Case 4
                                licenseState = Left(strRestOfString, found - 1)
                            Case 5
                                slashZero = Left(strRestOfString, found - 1)
                            Case 6
                                blockNumber = Left(strRestOfString, found - 1)
                            Case 7
                                street = Left(strRestOfString, found - 1)
                            Case 8
                                ticketDate = Left(strRestOfString, found - 1)
                            Case 9
                                ticketTime = Left(strRestOfString, found - 1)
                            Case 10
                                meterLocation = Left(strRestOfString, found - 1)
                            Case 11
                                issuedBy = Left(strRestOfString, found - 1)
                            Case 12
                                unknown1 = Left(strRestOfString, found - 1)
                            Case 13
                                originalFine = Left(strRestOfString, found - 1)
                            Case 14
                                FourteenDayFine = Left(strRestOfString, found - 1)
                            Case 15
                                ticketDay = Left(strRestOfString, found - 1)
                            Case 16
                                unknown2 = Left(strRestOfString, found - 1)
                            Case 17
                                violationCode = Left(strRestOfString, found - 1)
                            Case 18
                                violationType = Left(strRestOfString, found - 1)
                            Case 19
                                unknown3 = Left(strRestOfString, found - 1)
                            Case 20
                                unknown4 = Left(strRestOfString, found - 1)
                            Case 21
                                unknown5 = Left(strRestOfString, found - 1)
                            Case 22
                                unknown6 = Left(strRestOfString, found - 1)
                            Case 23
                                unknown7 = Left(strRestOfString, found - 1)
                            Case 24
                                unknown8 = Left(strRestOfString, found - 1)
                            Case 25
                                postedLimit = Left(strRestOfString, found - 1)
                        End Select
                        strRestOfString = Mid(strRestOfString, found + 1, Len(strLine) - found)
                        Counter += 1
                    End If
                Loop
                totalFine += Convert.ToDouble(originalFine)
                WriteToFile(ticketNumber + " " + originalFine + " " + totalFine.ToString, fileNameToBeStored)
                Counter = 0
            End If
        Loop Until strLine Is Nothing
        fileParse.Close()
    End Sub
    Public Sub ReadTextSplitMethod()
        Dim totalFine As Double = 0.0
        Dim strFileName As String = "Tickets.txt"
        Dim fileNameToBeStored = "OutputSplit.txt"
        Dim filePath As String = System.AppDomain.CurrentDomain.BaseDirectory()
        Dim SplitFile(25) As String
        Dim I As Integer
        Dim fileParse As StreamReader = New StreamReader(filePath + strFileName)
        Do
            strLine = fileParse.ReadLine()
            strRestOfString = strLine
            If strRestOfString <> Nothing Then
                SplitFile = strRestOfString.Split(";")
                For I = 0 To SplitFile.Count Step 1
                    Select Case I
                        Case 0
                            ticketNumber = SplitFile(0)
                        Case 1
                            vehicleType = SplitFile(1)
                        Case 2
                            vehicleMake = SplitFile(2)
                        Case 3
                            licensePlate = SplitFile(3)
                        Case 4
                            licenseState = SplitFile(4)
                        Case 5
                            slashZero = SplitFile(5)
                        Case 6
                            blockNumber = SplitFile(6)
                        Case 7
                            street = SplitFile(7)
                        Case 8
                            ticketDate = SplitFile(8)
                        Case 9
                            ticketTime = SplitFile(9)
                        Case 10
                            meterLocation = SplitFile(10)
                        Case 11
                            issuedBy = SplitFile(11)
                        Case 12
                            unknown1 = SplitFile(12)
                        Case 13
                            originalFine = SplitFile(13)
                        Case 14
                            FourteenDayFine = SplitFile(14)
                        Case 15
                            ticketDay = SplitFile(15)
                        Case 16
                            unknown2 = SplitFile(16)
                        Case 17
                            violationCode = SplitFile(17)
                        Case 18
                            violationType = SplitFile(18)
                        Case 19
                            unknown3 = SplitFile(19)
                        Case 20
                            unknown4 = SplitFile(20)
                        Case 21
                            unknown5 = SplitFile(21)
                        Case 22
                            unknown6 = SplitFile(22)
                        Case 23
                            unknown7 = SplitFile(23)
                        Case 24
                            unknown8 = SplitFile(24)
                        Case 25
                            postedLimit = SplitFile(25)
                    End Select
                Next
                totalFine += Convert.ToDouble(originalFine)
                WriteToFile(ticketNumber + " " + originalFine + " " + totalFine.ToString, fileNameToBeStored)
            End If
        Loop Until strLine Is Nothing
        fileParse.Close()
    End Sub
    Public Sub WriteToFile(ByVal strOutputLine As String, ByVal fileNameToBeStored As String)
        Dim strPath As String = System.AppDomain.CurrentDomain.BaseDirectory()
        Using sw As StreamWriter = New StreamWriter(strPath + fileNameToBeStored, True)
            sw.WriteLine(strOutputLine)
        End Using
    End Sub

    Protected Sub Premium_SelectedIndexChanged(sender As Object, e As EventArgs)
        strDropDownSelected = Premium.SelectedValue
    End Sub
    Public Sub CreateTable(ByVal strTableName As String)
        Dim myConnection As SqlConnection = New SqlConnection
        Dim myCommand As SqlCommand = New SqlCommand
        Dim strSql As String
        myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConnection").ConnectionString
        strSql = String.Empty
        strSql += "CREATE TABLE " + strTableName + "("
        strSql += "TicketNumber varchar(50) not null, "
        strSql += "VehicleType varchar(50) not null, "
        strSql += "VehicleMake varchar(50) null, "
        strSql += "LicensePlate varchar(50) null, "
        strSql += "LicenseState varchar(50) null, "
        strSql += "SlashZero varchar(50) null, "
        strSql += "BlockNumber varchar(50) null, "
        strSql += "Street varchar(50) null, "
        strSql += "TicketDate varchar(50) null, "
        strSql += "TicketTime varchar(50) null, "
        strSql += "MeterLocation varchar(50) null, "
        strSql += "IssuedBy varchar(50) null, "
        strSql += "Unknown1 varchar(50) null, "
        strSql += "OriginalFine varchar(50) null, "
        strSql += "FourteenDayFine varchar(50) null, "
        strSql += "TicketDay varchar(50) null, "
        strSql += "Unknown2 varchar(50) null, "
        strSql += "ViolationCode varchar(50) null, "
        strSql += "ViolationType varchar(50) null, "
        strSql += "Unknown3 varchar(50) null, "
        strSql += "Unknown4 varchar(50) null, "
        strSql += "Unknown5 varchar(50) null, "
        strSql += "Unknown6 varchar(50) null, "
        strSql += "Unknown7 varchar(50) null, "
        strSql += "Unknown8 varchar(50) null, "
        strSql += "PostedLimit varchar(50) null, "
        strSql += "primary key(TicketNumber))"
        Dim present As Integer = TableChecking()
        If present = 0 Then
            myCommand.CommandText = strSql
            myCommand.CommandType = CommandType.Text
            myCommand.Connection = myConnection
            myConnection.Open()
            myCommand.ExecuteNonQuery()
            myConnection.Close()
            myCommand.Dispose()
            myConnection.Dispose()
        Else
            sql.Text = "Table cant be created because its already created"
        End If
    End Sub
    Public Sub DropTable(ByVal strTableName As String)
        Dim myConnection As SqlConnection = New SqlConnection
        Dim myCommand As SqlCommand = New SqlCommand
        Dim strSql As String = String.Empty
        myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConnection").ConnectionString
        Dim present As Integer = TableChecking()
        If present = 1 Then
            strSql = "drop table " + strTableName
            myCommand.CommandText = strSql
            myCommand.CommandType = CommandType.Text
            myCommand.Connection = myConnection
            myConnection.Open()
            myCommand.ExecuteNonQuery()
            myConnection.Close()
            myCommand.Dispose()
            myConnection.Dispose()
        Else
            sql.Text = "Table cant dropped because it doesnt exists"
        End If
    End Sub
    Public Sub InsertSQL()
        Dim myConnection As SqlConnection = New SqlConnection
        Dim myCommand As SqlCommand = New SqlCommand
        Dim strSql As String = String.Empty
        Dim strFileName As String = "Tickets.txt"
        Dim filePath As String = System.AppDomain.CurrentDomain.BaseDirectory()
        Dim SplitFile(25) As String
        Dim I As Integer
        Dim fileParse As StreamReader = New StreamReader(filePath + strFileName)
        Do
            strLine = fileParse.ReadLine()
            strRestOfString = strLine
            If strRestOfString <> Nothing Then
                SplitFile = strRestOfString.Split(";")
                For I = 0 To SplitFile.Count Step 1
                    Select Case I
                        Case 0
                            ticketNumber = SplitFile(0)
                        Case 1
                            vehicleType = SplitFile(1)
                        Case 2
                            vehicleMake = SplitFile(2)
                        Case 3
                            licensePlate = SplitFile(3)
                        Case 4
                            licenseState = SplitFile(4)
                        Case 5
                            slashZero = SplitFile(5)
                        Case 6
                            blockNumber = SplitFile(6)
                        Case 7
                            street = SplitFile(7)
                        Case 8
                            ticketDate = SplitFile(8)
                        Case 9
                            ticketTime = SplitFile(9)
                        Case 10
                            meterLocation = SplitFile(10)
                        Case 11
                            issuedBy = SplitFile(11)
                        Case 12
                            unknown1 = SplitFile(12)
                        Case 13
                            originalFine = SplitFile(13)
                        Case 14
                            FourteenDayFine = SplitFile(14)
                        Case 15
                            ticketDay = SplitFile(15)
                        Case 16
                            unknown2 = SplitFile(16)
                        Case 17
                            violationCode = SplitFile(17)
                        Case 18
                            violationType = SplitFile(18)
                        Case 19
                            unknown3 = SplitFile(19)
                        Case 20
                            unknown4 = SplitFile(20)
                        Case 21
                            unknown5 = SplitFile(21)
                        Case 22
                            unknown6 = SplitFile(22)
                        Case 23
                            unknown7 = SplitFile(23)
                        Case 24
                            unknown8 = SplitFile(24)
                        Case 25
                            postedLimit = SplitFile(25)
                    End Select
                Next
                strSql = "insert into Tickets(TicketNumber,VehicleType,VehicleMake,LicensePlate,LicenseState,"
                strSql += "SlashZero,BlockNumber,Street,TicketDate,TicketTime,MeterLocation,IssuedBy,"
                strSql += "Unknown1,OriginalFine,FourteenDayFine,TicketDay,Unknown2,ViolationCode,"
                strSql += "ViolationType,Unknown3,Unknown4,Unknown5,Unknown6,Unknown7,Unknown8,PostedLimit)"
                strSql += "values('" + ticketNumber + "',"
                strSql += " '" + vehicleType + "',"
                strSql += "'" + vehicleMake + "',"
                strSql += "'" + licensePlate + "',"
                strSql += "'" + licenseState + "',"
                strSql += "'" + slashZero + "',"
                strSql += "'" + blockNumber + "',"
                strSql += "'" + street + "',"
                strSql += "'" + ticketDate + "',"
                strSql += "'" + ticketTime + "',"
                strSql += "'" + meterLocation + "',"
                strSql += "'" + issuedBy + "',"
                strSql += "'" + unknown1 + "',"
                strSql += "'" + originalFine + "',"
                strSql += "'" + FourteenDayFine + "',"
                strSql += "'" + ticketDay + "',"
                strSql += "'" + unknown2 + "',"
                strSql += "'" + violationCode + "',"
                strSql += "'" + violationType + "',"
                strSql += "'" + unknown3 + "',"
                strSql += "'" + unknown4 + "',"
                strSql += "'" + unknown5 + "',"
                strSql += "'" + unknown6 + "',"
                strSql += "'" + unknown7 + "',"
                strSql += "'" + unknown8 + "',"
                strSql += "'" + postedLimit + "')"
                Dim checking As String = TableCheckingInsert(ticketNumber)
                If checking = Nothing Then
                    myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConnection").ConnectionString
                    myCommand.CommandText = strSql
                    myCommand.CommandType = CommandType.Text
                    myCommand.Connection = myConnection
                    myConnection.Open()
                    myCommand.ExecuteNonQuery()
                    myConnection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                Else
                    sql.Text = "Data related to the ticket number is already present"
                End If
            End If
        Loop Until strLine Is Nothing
        fileParse.Close()
    End Sub
    Public Function TableChecking()
        Dim checking As String = "Select count(name) from sys.tables where name='Tickets'"
        Dim myConnection As SqlConnection = New SqlConnection
        Dim myCommand As SqlCommand = New SqlCommand
        myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConnection").ConnectionString
        myCommand.CommandText = checking
        myCommand.CommandType = CommandType.Text
        myCommand.Connection = myConnection
        myConnection.Open()
        Dim count As Integer = Convert.ToInt16(myCommand.ExecuteScalar)
        myConnection.Close()
        myCommand.Dispose()
        myConnection.Dispose()
        Return count
    End Function
    Public Function TableCheckingInsert(ByVal ticketNumber As String)
        Dim checking As String = "Select OriginalFine from Tickets where TicketNumber='" + ticketNumber + "'"
        Dim myConnection As SqlConnection = New SqlConnection
        Dim myCommand As SqlCommand = New SqlCommand
        myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConnection").ConnectionString
        myCommand.CommandText = checking
        myCommand.CommandType = CommandType.Text
        myCommand.Connection = myConnection
        myConnection.Open()
        Dim count As String = (myCommand.ExecuteScalar)
        myConnection.Close()
        myCommand.Dispose()
        myConnection.Dispose()
        Return count
    End Function
    Protected Sub GridViewButton_Click(sender As Object, e As EventArgs)
        If Session("gridView") = False Then
            Session("gridView") = True
            Session("gridViewSQL") = False
        Else
            Session("gridView") = False
        End If
        Response.Redirect("WebForm1.aspx")
    End Sub

    Protected Sub GridViewButtonSQL_Click(sender As Object, e As EventArgs)
        If Session("gridViewSQL") = False Then
            Session("gridViewSQL") = True
            Session("gridView") = False
        Else
            Session("gridViewSQL") = False
        End If
        Response.Redirect("WebForm1.aspx")
    End Sub
End Class