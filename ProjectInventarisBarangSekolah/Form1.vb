Imports MySql.Data.MySqlClient

Public Class Form1
    Dim conn As New MySqlConnection("server=localhost;port=3306;username=root;password=;database=db_inventaris;")
    Dim i As Integer
    Dim dr As MySqlDataReader

    Sub koneksi()
        Try
            If conn Is Nothing Then
                conn = New MySqlConnection("server=localhost;port=3306;username=root;password=;database=db_inventaris;")
            End If
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
        Catch ex As Exception
            MsgBox("Koneksi ke database gagal: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            '=== Validasi input kosong ===
            If TextBox1.Text.Trim() = "" Then
                MsgBox("Username tidak boleh kosong!", MsgBoxStyle.Exclamation)
                TextBox1.Focus()
                Exit Sub
            End If

            If TextBox2.Text.Trim() = "" Then
                MsgBox("Password tidak boleh kosong!", MsgBoxStyle.Exclamation)
                TextBox2.Focus()
                Exit Sub
            End If

            koneksi()

            '=== CHECK USERNAME ADA ATAU TIDAK ===
            Dim checkUserCmd As New MySqlCommand("SELECT COUNT(*) FROM login WHERE username=@username", conn)
            checkUserCmd.Parameters.AddWithValue("@username", TextBox1.Text.Trim())
            Dim userCount As Integer = Convert.ToInt32(checkUserCmd.ExecuteScalar())

            If userCount = 0 Then
                MsgBox("Username tidak ditemukan!", MsgBoxStyle.Exclamation)
                conn.Close()
                Exit Sub
            End If

            '=== CHECK PASSWORD BENAR ATAU TIDAK ===
            Dim loginCmd As New MySqlCommand("SELECT COUNT(*) FROM login 
                                          WHERE username=@username AND password=@password", conn)
            loginCmd.Parameters.AddWithValue("@username", TextBox1.Text.Trim())
            loginCmd.Parameters.AddWithValue("@password", TextBox2.Text.Trim())

            Dim loginCount As Integer = Convert.ToInt32(loginCmd.ExecuteScalar())

            If loginCount = 0 Then
                MsgBox("Password salah!", MsgBoxStyle.Exclamation)
                conn.Close()
                Exit Sub
            End If

            '=== LOGIN BERHASIL ===
            MsgBox("Login berhasil, selamat datang " & TextBox1.Text & "!", MsgBoxStyle.Information)

            conn.Close()
            Me.Hide()
            Dashboard.Show()

        Catch ex As MySqlException
            MsgBox("Kesalahan database: " & ex.Message, MsgBoxStyle.Critical)
        Catch ex As Exception
            MsgBox("Terjadi kesalahan: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub



    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim f2 As New Regis()
            f2.Show()
            Me.Hide()
        Catch ex As Exception
            MsgBox("Gagal membuka form registrasi: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub
End Class
