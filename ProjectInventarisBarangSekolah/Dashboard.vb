Public Class Dashboard

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim f4 As New Data_Barang()
        f4.Show()
        Me.Hide()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim f3 As New Peminjamanvb()
        f3.Show()
        Me.Hide()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim f4 As New Pengembalian()
        f4.Show()
        Me.Hide()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim f5 As New Denda()
        f5.Show()
        Me.Hide()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs)
        Dim f6 As New Pengguna()
        f6.Show()
        Me.Hide()
    End Sub

    Private Sub PictureBox5_Click(sender As Object, e As EventArgs) Handles PictureBox5.Click
        Dim f1 As New Form1()
        f1.Show()
        Me.Hide()
    End Sub
End Class