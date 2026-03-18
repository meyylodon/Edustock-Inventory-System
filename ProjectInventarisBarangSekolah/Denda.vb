Imports MySql.Data.MySqlClient
Imports ProjectInventarisBarangSekolah.Denda

Public Class Denda
    Dim conn As New MySqlConnection("server=localhost;port=3306;username=root;password=;database=db_inventaris;")
    Dim i As Integer
    Dim dr As MySqlDataReader

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Dim f1 As New Dashboard()
        f1.Show()
        Me.Hide()
    End Sub
    Public Sub simpan()
        If tbIdDenda.Text = "" Or tbIdKembali.Text = "" Or cbJenis.Text = "" Or tbJumlahDenda.Text = "" Or tbStatus.Text = "" Then
            MsgBox("Semua field wajib diisi!", MsgBoxStyle.Exclamation, "Peringatan")
            Exit Sub
        End If

        Try
            conn.Open()
            Dim cmd As New MySqlCommand("
            INSERT INTO denda 
                (idDenda, idPengembalian, nama, jenisPelanggaran, jumlahDenda, tanggalDenda, statusPembayaran) 
            VALUES 
                (@idDenda, @idPengembalian, @nama, @jenisPelanggaran, @jumlahDenda, @tanggalDenda, @statusPembayaran)
        ", conn)

            cmd.Parameters.AddWithValue("@idDenda", tbIdDenda.Text)
            cmd.Parameters.AddWithValue("@idPengembalian", tbIdKembali.Text)
            cmd.Parameters.AddWithValue("@nama", tbNama.Text)
            cmd.Parameters.AddWithValue("@jenisPelanggaran", cbJenis.Text)
            cmd.Parameters.AddWithValue("@jumlahDenda", tbJumlahDenda.Text)
            cmd.Parameters.AddWithValue("@tanggalDenda", dtDenda.Value)
            cmd.Parameters.AddWithValue("@statusPembayaran", tbStatus.Text)

            cmd.ExecuteNonQuery()

            MessageBox.Show("Data Berhasil Disimpan", "Tambah Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
            dataload()
            reset()

        Catch ex As MySqlException
            MsgBox("Error MySQL: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Public Sub ubah()
        If tbIdDenda.Text = "" Then
            MsgBox("Pilih data terlebih dahulu untuk diubah!", MsgBoxStyle.Information, "Peringatan")
            Exit Sub
        End If

        Try
            conn.Open()
            Dim cmd As New MySqlCommand("
                UPDATE denda SET 
                    idPengembalian=@idPengembalian,
                    nama=@nama,
                    jenisPelanggaran=@jenisPelanggaran, 
                    jumlahDenda=@jumlahDenda, 
                    tanggalDenda=@tanggalDenda, 
                    statusPembayaran=@statusPembayaran 
                WHERE idDenda=@idDenda
            ", conn)

            cmd.Parameters.AddWithValue("@idDenda", tbIdDenda.Text)
            cmd.Parameters.AddWithValue("@idPengembalian", tbIdKembali.Text)
            cmd.Parameters.AddWithValue("@nama", tbNama.Text)
            cmd.Parameters.AddWithValue("@jenisPelanggaran", cbJenis.Text)
            cmd.Parameters.AddWithValue("@jumlahDenda", tbJumlahDenda.Text)
            cmd.Parameters.AddWithValue("@tanggalDenda", dtDenda.Value)
            cmd.Parameters.AddWithValue("@statusPembayaran", tbStatus.Text)

            i = cmd.ExecuteNonQuery()
            If i > 0 Then
                MessageBox.Show("Data Berhasil Diubah", "Ubah Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                dataload()
                reset()
            End If
        Catch ex As MySqlException
            MsgBox("Error MySQL: " & ex.Message)
        Catch ex As Exception
            MsgBox("Error VB: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub
    Public Sub hapus()
        If tbIdDenda.Text = "" Then
            MsgBox("Pilih data yang ingin dihapus!", MsgBoxStyle.Information, "Peringatan")
            Exit Sub
        End If

        If MsgBox("Apakah anda yakin akan menghapus data tersebut?", MsgBoxStyle.Question + vbYesNo) = vbYes Then
            Try
                conn.Open()
                Dim cmd As New MySqlCommand("DELETE FROM denda WHERE idDenda=@idDenda", conn)
                cmd.Parameters.AddWithValue("@idDenda", tbIdDenda.Text)
                i = cmd.ExecuteNonQuery()

                If i > 0 Then
                    MessageBox.Show("Data Berhasil Dihapus", "Hapus Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    dataload()
                    reset()
                End If
            Catch ex As MySqlException
                MsgBox("Error MySQL: " & ex.Message)
            Catch ex As Exception
                MsgBox("Error VB: " & ex.Message)
            Finally
                conn.Close()
            End Try
        End If
    End Sub
    Public Sub autoIdDenda()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim cmd As New MySqlCommand("SELECT MAX(idDenda) FROM denda", conn)
            Dim lastId As Object = cmd.ExecuteScalar()

            If IsDBNull(lastId) Then
                tbIdDenda.Text = "1"
            Else
                tbIdDenda.Text = (CInt(lastId) + 1).ToString()
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub
    Public Sub loadNextId()
        Try
            conn.Open()
            Dim cmd As New MySqlCommand("SHOW TABLE STATUS LIKE 'denda'", conn)
            Dim rd As MySqlDataReader = cmd.ExecuteReader()

            If rd.Read() Then
                Dim nextId As Integer = rd("Auto_increment")
                tbIdDenda.Text = nextId.ToString()
                tbIdDenda.ReadOnly = True
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub
    Public Sub reset()
        tbIdDenda.Clear()
        tbIdKembali.Clear()
        tbNama.Clear()
        cbJenis.SelectedIndex = -1
        tbJumlahDenda.Clear()
        dtDenda.Value = Today
        tbStatus.Clear()
        tbIdDenda.ReadOnly = False
        Button1.Enabled = True
    End Sub
    Public Sub dataload()
        DataGridView1.Rows.Clear()
        Try
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If

            conn.Open()
            Dim cmd As New MySqlCommand("SELECT * FROM denda", conn)
            dr = cmd.ExecuteReader
            While dr.Read
                Dim index As Integer = DataGridView1.Rows.Add(
                dr.Item("idDenda"),
                dr.Item("idPengembalian"),
                dr.Item("nama"),
                dr.Item("jenisPelanggaran"),
                dr.Item("jumlahDenda"),
                dr.Item("tanggalDenda"),
                dr.Item("statusPembayaran")
            )

                DataGridView1.Rows(index).Cells("icUbah").Value = My.Resources.iconUbah
                DataGridView1.Rows(index).Cells("icHapus").Value = My.Resources.iconHapus
            End While
            dr.Close()

            DataGridView1.RowTemplate.Height = 40
            If DataGridView1.Columns.Contains("icUbah") Then
                CType(DataGridView1.Columns("icUbah"), DataGridViewImageColumn).ImageLayout = DataGridViewImageCellLayout.Zoom
            End If
            If DataGridView1.Columns.Contains("icHapus") Then
                CType(DataGridView1.Columns("icHapus"), DataGridViewImageColumn).ImageLayout = DataGridViewImageCellLayout.Zoom
            End If

            DataGridView1.AllowUserToAddRows = False

        Catch ex As Exception
            MsgBox("Gagal memuat data: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        simpan()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        ubah()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        hapus()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        reset()
        tbIdDenda.ReadOnly = True
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex < 0 Then Exit Sub
        Dim colName As String = DataGridView1.Columns(e.ColumnIndex).Name
        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
        If colName <> "icUbah" AndAlso colName <> "icHapus" Then
            tbIdDenda.Text = row.Cells(0).Value.ToString()
            tbIdKembali.Text = row.Cells(1).Value.ToString()
            tbNama.Text = row.Cells(2).Value.ToString()
            cbJenis.Text = row.Cells(3).Value.ToString()
            tbJumlahDenda.Text = row.Cells(4).Value.ToString()
            Try
                dtDenda.Value = Convert.ToDateTime(row.Cells(5).Value)
            Catch
                dtDenda.Value = Today
            End Try
            tbStatus.Text = row.Cells(6).Value.ToString()

            tbIdDenda.ReadOnly = True
            Button1.Enabled = False
            Exit Sub
        End If

        If colName = "icUbah" Then
            Try
                conn.Open()
                Dim cmd As New MySqlCommand("
                UPDATE denda SET 
                    idPengembalian=@idPengembalian,
                    nama=@nama,
                    jenisPelanggaran=@jenisPelanggaran,
                    jumlahDenda=@jumlahDenda,
                    tanggalDenda=@tanggalDenda,
                    statusPembayaran=@statusPembayaran
                WHERE idDenda=@idDenda
            ", conn)

                cmd.Parameters.AddWithValue("@idDenda", tbIdDenda.Text)
                cmd.Parameters.AddWithValue("@idPengembalian", tbIdKembali.Text)
                cmd.Parameters.AddWithValue("@nama", tbNama.Text)
                cmd.Parameters.AddWithValue("@jenisPelanggaran", cbJenis.Text)
                cmd.Parameters.AddWithValue("@jumlahDenda", tbJumlahDenda.Text)
                cmd.Parameters.AddWithValue("@tanggalDenda", dtDenda.Value)
                cmd.Parameters.AddWithValue("@statusPembayaran", tbStatus.Text)

                i = cmd.ExecuteNonQuery()
                If i > 0 Then
                    MessageBox.Show("Data Berhasil Diubah", "Ubah Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    dataload()
                End If
            Catch ex As Exception
                MsgBox("Error: " & ex.Message)
            Finally
                conn.Close()
            End Try

            tbIdDenda.ReadOnly = False
            Button1.Enabled = True
        End If

        If colName = "icHapus" Then
            If MsgBox("Apakah anda yakin akan menghapus data ini?", MsgBoxStyle.Question + vbYesNo) = vbYes Then
                Try
                    conn.Open()
                    Dim cmd As New MySqlCommand("DELETE FROM denda WHERE idDenda=@idDenda", conn)
                    cmd.Parameters.AddWithValue("@idDenda", row.Cells(0).Value.ToString())
                    i = cmd.ExecuteNonQuery()

                    If i > 0 Then
                        MessageBox.Show("Data Berhasil Dihapus", "Hapus Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        dataload()
                    End If
                Catch ex As Exception
                    MsgBox("Error: " & ex.Message)
                Finally
                    conn.Close()
                End Try
            End If
        End If
    End Sub
    Private Sub Denda_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dataload()
        loadNextId()
        tbIdDenda.ReadOnly = True
    End Sub

    Private Sub tbCari_TextChanged(sender As Object, e As EventArgs) Handles tbCari.TextChanged
        DataGridView1.Rows.Clear()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim cmd As New MySqlCommand("
            SELECT * FROM denda 
            WHERE idDenda LIKE @cari 
               OR idPengembalian LIKE @cari 
               OR jenisPelanggaran LIKE @cari 
               OR nama LIKE @cari
               OR statusPembayaran LIKE @cari
        ", conn)
            cmd.Parameters.AddWithValue("@cari", "%" & tbCari.Text & "%")

            dr = cmd.ExecuteReader()
            While dr.Read()
                Dim index As Integer = DataGridView1.Rows.Add(
                dr.Item("idDenda"),
                dr.Item("idPengembalian"),
                dr.Item("jenisPelanggaran"),
                dr.Item("jumlahDenda"),
                If(IsDBNull(dr.Item("tanggalDenda")), "", Convert.ToDateTime(dr.Item("tanggalDenda")).ToString("yyyy-MM-dd")),
                dr.Item("statusPembayaran")
            )

                DataGridView1.Rows(index).Cells("icUbah").Value = My.Resources.iconUbah
                DataGridView1.Rows(index).Cells("icHapus").Value = My.Resources.iconHapus
            End While
            dr.Close()

            DataGridView1.RowTemplate.Height = 40
            DataGridView1.AllowUserToAddRows = False
            CType(DataGridView1.Columns("icUbah"), DataGridViewImageColumn).ImageLayout = DataGridViewImageCellLayout.Zoom
            CType(DataGridView1.Columns("icHapus"), DataGridViewImageColumn).ImageLayout = DataGridViewImageCellLayout.Zoom

        Catch ex As Exception
            MsgBox("Gagal mencari data: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim ds As New DataSet2
        Dim dt As New DataTable

        dt = ds.Tables("Denda")
        For i = 0 To DataGridView1.Rows.Count - 1
            If Not DataGridView1.Rows(i).IsNewRow Then
                dt.Rows.Add(
                DataGridView1.Rows(i).Cells(0).Value,
                DataGridView1.Rows(i).Cells(1).Value,
                DataGridView1.Rows(i).Cells(2).Value,
                DataGridView1.Rows(i).Cells(3).Value,
                DataGridView1.Rows(i).Cells(4).Value,
                DataGridView1.Rows(i).Cells(5).Value,
                DataGridView1.Rows(i).Cells(6).Value
            )
            End If
        Next
        Laporan_Denda.ReportViewer1.LocalReport.ReportPath = "D:\Kampus\ProjectInventarisBarangSekolah - Copy\ProjectInventarisBarangSekolah\Report4.rdlc"
        Laporan_Denda.ReportViewer1.LocalReport.DataSources.Clear()
        Laporan_Denda.ReportViewer1.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("Denda", dt))
        Laporan_Denda.Show()
        Laporan_Denda.ReportViewer1.RefreshReport()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim ds As New DataSet2()
        Dim dt As DataTable = ds.Tables("Denda")

        For Each row As DataGridViewRow In DataGridView1.Rows
            If Not row.IsNewRow Then
                dt.Rows.Add(
                    row.Cells(0).Value,
                    row.Cells(1).Value,
                    row.Cells(2).Value,
                    row.Cells(3).Value,
                    row.Cells(4).Value,
                    row.Cells(5).Value,
                    row.Cells(6).Value
                )
            End If
        Next
        Dim laporan_Denda As New Laporan_Denda()

        With laporan_Denda.ReportViewer1.LocalReport
            .ReportPath = "D:\Kampus\ProjectInventarisBarangSekolah - Copy\ProjectInventarisBarangSekolah\Report7.rdlc"
            .DataSources.Clear()
            .DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("Denda", dt))
        End With

        laporan_Denda.ReportViewer1.RefreshReport()
        laporan_Denda.Show()

    End Sub
    Private Sub tbIdKembali_TextChanged(sender As Object, e As EventArgs) Handles tbIdKembali.TextChanged
        If tbIdKembali.Text = "" Then
            tbNama.Text = ""
            Exit Sub
        End If

        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim cmd As New MySqlCommand("SELECT nama FROM pengembalian WHERE idPengembalian=@idPengembalian", conn)
            cmd.Parameters.AddWithValue("@idPengembalian", tbIdKembali.Text)

            Dim rd As MySqlDataReader = cmd.ExecuteReader()
            If rd.Read() Then
                tbNama.Text = rd("nama").ToString()
            Else
                tbNama.Text = ""
            End If

            rd.Close()
        Catch ex As Exception
            MsgBox("Error ambil nama: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

End Class