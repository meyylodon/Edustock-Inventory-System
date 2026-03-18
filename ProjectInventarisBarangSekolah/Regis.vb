Imports MySql.Data.MySqlClient

Public Class Regis
    Dim conn As MySqlConnection
    Dim cmd As MySqlCommand

    '=== Membuat koneksi ke database ===
    Sub koneksi()
        conn = New MySqlConnection("server=localhost;port=3306;user id=root;password=;database=db_inventaris;")
        If conn.State = ConnectionState.Closed Then
            conn.Open()
        End If
    End Sub

    '=== Tombol "Daftar" ===
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            '=== Validasi input kosong ===
            If TextBox3.Text = "" Or TextBox1.Text = "" Or TextBox2.Text = "" Then
                MsgBox("Semua data harus diisi dulu sebelum daftar!", MsgBoxStyle.Exclamation, "Peringatan")
                Exit Sub
            End If

            koneksi()

            'Cek apakah username sudah digunakan
            Dim checkCmd As New MySqlCommand("SELECT COUNT(*) FROM login WHERE username=@username", conn)
            checkCmd.Parameters.AddWithValue("@username", TextBox1.Text)
            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

            If count > 0 Then
                MsgBox("Username sudah terdaftar, silakan gunakan username lain!", MsgBoxStyle.Exclamation)
                conn.Close()
                Exit Sub
            End If

            '=== Simpan data ===
            cmd = New MySqlCommand("INSERT INTO login (nama, username, password) VALUES (@nama, @username, @password)", conn)
            cmd.Parameters.AddWithValue("@nama", TextBox3.Text)
            cmd.Parameters.AddWithValue("@username", TextBox1.Text)
            cmd.Parameters.AddWithValue("@password", TextBox2.Text)
            cmd.ExecuteNonQuery()

            MsgBox("Registrasi berhasil! Silakan login menggunakan akun Anda.", MsgBoxStyle.Information)

            conn.Close()

            'Auto-fill ke form login
            Form1.TextBox1.Text = TextBox1.Text
            Form1.TextBox2.Text = TextBox2.Text

            Form1.Show()
            Me.Hide()

        Catch ex As MySqlException
            MsgBox("Kesalahan database: " & ex.Message, MsgBoxStyle.Critical)
        Catch ex As Exception
            MsgBox("Terjadi kesalahan: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    '=== Tombol kembali (jika pakai PictureBox) ===
    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Form1.Show()
        Me.Hide()
    End Sub
End Class
