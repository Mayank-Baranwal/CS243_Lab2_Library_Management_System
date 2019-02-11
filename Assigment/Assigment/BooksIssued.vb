﻿Public Class BooksIssued

    Dim cnt As Integer = 0

    Private Access As New LMS

    Public Sub RefreshPage()
        'Dim cnt As Integer = 0
        lblBklt.Text = "You can issue " & Log.CurBkLimit & " more books!"

        lblRet1.Show()
        btnRet1.Show()

        Access.ExecQuery("SELECT * from Users WHERE [ID] =" & Log.CurID)
        If Not String.IsNullOrEmpty(Access.Exception) Then MsgBox(Access.Exception) : Exit Sub

        Dim checkString As String = ""
        For Each R As DataRow In Access.DBDT.Rows
            checkString &= R.Item(9)
        Next
        Dim array As String() = Split(checkString, "-")
        Dim bkno As Integer = 0
        For Each temp As String In array
            If IsNumeric(temp) = False Then
                Continue For
            End If

            Access.ExecQuery("SELECT * from Book WHERE [ID]=" & CInt(temp))
            If Not String.IsNullOrEmpty(Access.Exception) Then MsgBox(Access.Exception) : Exit Sub
            If Access.DBDT.Rows.Count <> 0 Then
                bkno = 1
            End If
            For Each R As DataRow In Access.DBDT.Rows
                cnt += 1
                Dim checktemp As String = ""
                'checktemp &= R.Item(0)
                checktemp &= "Book Name: " & R.Item(1) & Environment.NewLine
                checktemp &= "Authors: " & R.Item(3) & Environment.NewLine
                checktemp &= "Genre: " & R.Item(6) & Environment.NewLine
                checktemp &= "Publisher: " & R.Item(7) & Environment.NewLine
                checktemp &= "ISBN: " & R.Item(2) & Environment.NewLine
                checktemp &= "Copies Available: " & R.Item(4) & Environment.NewLine

                If cnt = 1 Then
                    lblRet1.Font = New Font("Century Gothic", 9)
                    btnRet1.Font = New Font("Century Gothic", 9)
                    'btnRet1.DialogResult = R.Item(0)
                    lblRet1.Text = checktemp
                    AddHandler btnRet1.Click, AddressOf Me.Button_Click
                    tempRet1.Text = R.Item(0)
                Else
                    DynamicBooks(cnt, checktemp, R)
                End If
            Next
        Next

        'MessageBox.Show(bkno)

        If bkno = 0 Then
            lblBklt.Text &= " You currently have no books issued!"
            lblRet1.Hide()
            btnRet1.Hide()
        End If
    End Sub


    Private Sub DynamicBooks(i As Integer, txt As String, R As DataRow)
        'MessageBox.Show("Hello")
        Dim lblName As String
        lblName = "lblRet" & CStr(i)
        Dim yt As Integer = lblRet1.Location.Y + 120 * (i - 1)
        Dim lbl1 As New Label
        lbl1.Font = New Font("Century Gothic", 9)
        lbl1.Name = lblName
        lbl1.Text = txt
        lbl1.AutoSize = True
        'lbl1.Height = lblissue1.Hseight
        'lbl1.Width = lblissue1.Width
        lbl1.BorderStyle = Windows.Forms.BorderStyle.Fixed3D
        lbl1.BackColor = Color.Snow
        lbl1.Margin = New Padding(10, 10, 10, 10)
        Me.Controls.Add(lbl1)
        lbl1.Location = New Point(lblRet1.Location.X, yt)

        Dim btnName As String
        btnName = "btnRet" & CStr(i)
        Dim yb As Integer = btnRet1.Location.Y + 120 * (i - 1)
        Dim btn1 As New Button
        btn1.Font = New Font("Century Gothic", 9)
        btn1.Name = btnName
        btn1.Text = "Return book!"
        btn1.Height = btnRet1.Height
        btn1.Width = btnRet1.Width
        btn1.BackColor = System.Drawing.Color.FromArgb(171, 8, 55)
        btn1.ForeColor = Color.Snow
        btn1.FlatStyle = FlatStyle.Flat
        'btn1.Margin = New Padding(10, 10, 10, 10)
        Me.Controls.Add(btn1)
        btn1.Location = New Point(btnRet1.Location.X, yb)
        'btn1.DialogResult = R.Item(0)

        AddHandler btn1.Click, AddressOf Me.Button_Click

        Dim tempName As String
        Dim temp1 As New Label
        tempName = "tempRet" & CStr(i)
        temp1.Name = tempName
        temp1.Text = R.Item(0)
        'MessageBox.Show(temp1.Text)
        Me.Controls.Add(temp1)
        temp1.Hide()

    End Sub

    Public Sub Clr()
        lblRet1.Text = ""
        tempRet1.Text = ""
        'MessageBox.Show(cnt)
        For i As Integer = 2 To cnt
            'MessageBox.Show("Removing Button " & "lblRet" & CStr(i))
            RemoveHandler Me.Controls.Find("btnRet" & CStr(i), True)(0).Click, AddressOf Me.Button_Click
            Me.Controls.Remove(Me.Controls.Find("lblRet" & CStr(i), True)(0))
            Me.Controls.Remove(Me.Controls.Find("btnRet" & CStr(i), True)(0))
            Me.Controls.Remove(Me.Controls.Find("tempRet" & CStr(i), True)(0))
        Next
        RemoveHandler btnRet1.Click, AddressOf Me.Button_Click
        cnt = 0
        lblRet1.Hide()
        btnRet1.Hide()
    End Sub

    Private Sub Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim selectedBtn As Button = sender
        'MessageBox.Show("you have clicked button " & selectedBtn.DialogResult)
        If Log.CurUser = "" Then
            MessageBox.Show("You are currently not logged in! Please log in to return books!", "Error")
            Console.WriteLine("Error in Returning: Not logged in" & Environment.NewLine)
            Exit Sub
        End If

        'Dim IDFind As Integer = CInt(selectedBtn.DialogResult)
        Dim tmp As Label = Me.Controls.Find("tempRet" & selectedBtn.Name.Substring(6), True)(0)
        'MessageBox.Show(tmp.Text)
        Dim IDFind As Integer = CInt(tmp.Text)

        Access.ExecQuery("SELECT * FROM Book where [ID] = " & IDFind)
        If Not String.IsNullOrEmpty(Access.Exception) Then MsgBox(Access.Exception) : Exit Sub
        If Access.DBDT.Rows.Count = 0 Or Access.DBDT.Rows.Count > 1 Then
            MessageBox.Show("Book to be returned not found", "Error")
            Console.WriteLine("Book to be returned not found" & Environment.NewLine)
            Exit Sub
        End If

        Dim BkID As Integer = Access.DBDT.Rows(0).Item(0)
        Dim BkTit As String = Access.DBDT.Rows(0).Item(1)
        Dim BkCnt As Integer = Access.DBDT.Rows(0).Item(4) + 1
        Dim BkIss As String = Access.DBDT.Rows(0).Item(5)
        Dim BkIssNew As String = "0"
        Dim array As String() = Split(BkIss, "-")
        Dim done As Boolean = 0
        For Each temp As String In array
            If temp.Trim() = "0" Then
                Continue For
            End If
            If (temp.Trim() = CStr(Log.CurID) And done = 0) Then
                done = 1
                Continue For
            End If
            BkIssNew &= "-" & temp
        Next

        Access.ExecQuery("Update Book set Copies_Available=" & BkCnt & ", Issued_to='" & BkIssNew & "' where [ID]=" & BkID)
        If Not String.IsNullOrEmpty(Access.Exception) Then MsgBox(Access.Exception) : Exit Sub

        Log.CurBkLimit += 1
        Dim array2 As String() = Split(Log.CurBooks, "-")
        Dim done2 As Boolean = 0
        Dim CurBooksNew As String = "0"
        For Each temp As String In array2
            'MessageBox.Show(temp)
            If temp.Trim() = "0" Then
                Continue For
            End If
            If CInt(temp) = BkID And done2 = 0 Then
                done2 = 1
                Continue For
            End If
            CurBooksNew &= "-" & temp
        Next
        Log.CurBooks = CurBooksNew
        Try
            Access.ExecQuery("Update Users set Book_Limit=" & Log.CurBkLimit & ", Books_Issued='" & Log.CurBooks & "' where [ID]=" & Log.CurID)
        Catch ex As Exception
            MessageBox.Show("Unknown Error")
        End Try
        MessageBox.Show("Book " & BkTit & " successfully returned!", "Success")
        Console.Write("Book " & BkTit & " successfully returned!" & Environment.NewLine)
        'MessageBox.Show(cnt)
        Clr()
        RefreshPage()
    End Sub

    Private Sub lblRet1_Click(sender As Object, e As EventArgs) Handles lblRet1.Click

    End Sub

    Private Sub BooksIssued_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub btnRet1_Click(sender As Object, e As EventArgs) Handles btnRet1.Click

    End Sub
End Class
