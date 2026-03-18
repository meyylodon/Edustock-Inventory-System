Imports MySql.Data.MySqlClient

Public Class Peminjamanvb
    Dim conn As New MySqlConnection("server=localhost;port=3306;username=root;password=;database=db_inventaris;")
    Dim i As Integer
    Dim dr As MySqlDataReader

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Dim f1 As New Dashboard()
        f1.Show()
        Me.Hide()
    End Sub

    Private Sub Peminjaman_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dataload()
        loadNextId()
        tbIdPinjam.ReadOnly = True
    End Sub

    Public Sub simpan()
        Try
            conn.Open()

            Dim cmdCek As New MySqlCommand("
            SELECT stok 
            FROM databarang 
            WHERE idBarang=@id
        ", conn)
            cmdCek.Parameters.AddWithValue("@id", tbIdBarang.Text)

            Dim stokTersedia As Integer = Convert.ToInt32(cmdCek.ExecuteScalar())

            Dim jumlahPinjam As Integer = 1
            If Integer.TryParse(tbJumlahPinjam.Text, jumlahPinjam) = False Then
                jumlahPinjam = 1
            End If

            If stokTersedia <= 0 Then
                MessageBox.Show("Stok barang habis!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If jumlahPinjam > stokTersedia Then
                MessageBox.Show("Jumlah pinjam melebihi stok tersedia!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Dim cmd As New MySqlCommand("
            INSERT INTO peminjaman 
                (status, nisn, nama, idBarang, namaBarang, jumlahPinjam, tanggalPinjam)
            VALUES 
                (@status, @nisn, @nama, @idBarang, @namaBarang, @jumlahPinjam, @tanggalPinjam);
            SELECT LAST_INSERT_ID();
        ", conn)

            cmd.Parameters.AddWithValue("@status", cbStatus.Text)
            cmd.Parameters.AddWithValue("@nisn", tbNisn.Text)
            cmd.Parameters.AddWithValue("@nama", tbNama.Text)
            cmd.Parameters.AddWithValue("@idBarang", tbIdBarang.Text)
            cmd.Parameters.AddWithValue("@namaBarang", tbNamaBarang.Text)
            cmd.Parameters.AddWithValue("@jumlahPinjam", jumlahPinjam)
            cmd.Parameters.AddWithValue("@tanggalPinjam", dtPinjam.Value)

            Dim newId As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            tbIdPinjam.Text = newId.ToString()

            Dim cmdUpdate As New MySqlCommand("
            UPDATE databarang 
            SET stok = stok - @jumlahPinjam
            WHERE idBarang = @idBarang
        ", conn)
            cmdUpdate.Parameters.AddWithValue("@jumlahPinjam", jumlahPinjam)
            cmdUpdate.Parameters.AddWithValue("@idBarang", tbIdBarang.Text)
            cmdUpdate.ExecuteNonQuery()

            MessageBox.Show("Data Berhasil Disimpan",
                        "Tambah", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
            dataload()
        End Try
    End Sub


    Public Sub ubah()
        Try
            conn.Open()
            Dim cmd As New MySqlCommand("UPDATE peminjaman SET status=@status, nisn=@nisn, nama=@nama, idBarang=@idBarang, namaBarang=@namaBarang, jumlahPinjam=@jumlahPinjam, tanggalPinjam=@tanggalPinjam WHERE idPinjam=@idPinjam", conn)
            cmd.Parameters.AddWithValue("@idPinjam", tbIdPinjam.Text)
            cmd.Parameters.AddWithValue("@status", cbStatus.Text)
            cmd.Parameters.AddWithValue("@nisn", tbNisn.Text)
            cmd.Parameters.AddWithValue("@nama", tbNama.Text)
            cmd.Parameters.AddWithValue("@idBarang", tbIdBarang.Text)
            cmd.Parameters.AddWithValue("@namaBarang", tbNamaBarang.Text)
            cmd.Parameters.AddWithValue("@jumlahPinjam", JumlahPinjam)
            cmd.Parameters.AddWithValue("@tanggalPinjam", dtPinjam.Value)
            i = cmd.ExecuteNonQuery()
            If i > 0 Then
                MessageBox.Show("Data Berhasil Diubah", "Ubah Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try

        tbIdPinjam.ReadOnly = False
        Button1.Enabled = True
        dataload()
    End Sub

    Public Sub hapus()
        If MsgBox("Apakah anda yakin akan menghapus data tersebut?", MsgBoxStyle.Question + vbYesNo) = vbYes Then
            Try
                conn.Open()
                Dim cmd As New MySqlCommand("DELETE FROM peminjaman WHERE idPinjam=@idPinjam", conn)
                cmd.Parameters.AddWithValue("@idPinjam", tbIdPinjam.Text)
                i = cmd.ExecuteNonQuery
                If i > 0 Then
                    MessageBox.Show("Data Berhasil Dihapus", "Hapus Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                conn.Close()
            End Try
            tbIdPinjam.ReadOnly = False
            Button1.Enabled = True
            dataload()
        End If
    End Sub
    Public Sub autoIdPinjam()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim cmd As New MySqlCommand("SELECT MAX(idPinjam) FROM peminjaman", conn)
            Dim lastId As Object = cmd.ExecuteScalar()

            If IsDBNull(lastId) Then
                tbIdPinjam.Text = "1"
            Else
                tbIdPinjam.Text = (CInt(lastId) + 1).ToString()
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
            Dim cmd As New MySqlCommand("SHOW TABLE STATUS LIKE 'peminjaman'", conn)
            Dim rd As MySqlDataReader = cmd.ExecuteReader()

            If rd.Read() Then
                Dim nextId As Integer = rd("Auto_increment")
                tbIdPinjam.Text = nextId.ToString()
                tbIdPinjam.ReadOnly = True
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub
    Public Sub reset()
        tbIdPinjam.Text = ""
        cbStatus.Text = ""
        tbNisn.Text = ""
        tbNama.Text = ""
        tbIdBarang.Text = ""
        tbNamaBarang.Text = ""
        tbJumlahPinjam.Text = ""
        dtPinjam.Value = Today
        tbNama.ReadOnly = False
        Button1.Enabled = True
    End Sub
    Public Sub dataload()
        DataGridView1.Rows.Clear()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim cmd As New MySqlCommand("
            SELECT 
                p.idPinjam,
                p.status,
                p.nisn,
                p.nama,
                p.idBarang,
                b.namaBarang AS namaBarang,
                p.jumlahPinjam,
                p.tanggalPinjam
            FROM peminjaman p
            INNER JOIN databarang b ON p.idBarang = b.idBarang
            ORDER BY p.tanggalPinjam ASC
        ", conn)

            dr = cmd.ExecuteReader()

            While dr.Read()
                Dim index As Integer = DataGridView1.Rows.Add(
                dr("idPinjam"),
                dr("status"),
                dr("nisn"),
                dr("nama"),
                dr("idBarang"),
                dr("namaBarang"),
                dr("jumlahPinjam"),
                dr("tanggalPinjam")
            )

                DataGridView1.Rows(index).Cells("icUbah").Value = My.Resources.iconUbah
                DataGridView1.Rows(index).Cells("icHapus").Value = My.Resources.iconHapus
            End While
            dr.Close()

            DataGridView1.RowTemplate.Height = 40
            CType(DataGridView1.Columns("icUbah"), DataGridViewImageColumn).ImageLayout = DataGridViewImageCellLayout.Zoom
            CType(DataGridView1.Columns("icHapus"), DataGridViewImageColumn).ImageLayout = DataGridViewImageCellLayout.Zoom
            DataGridView1.AllowUserToAddRows = False

        Catch ex As Exception
            MsgBox("Gagal memuat data: " & ex.Message)

        Finally
            conn.Close()
        End Try
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        simpan()
        dataload()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        ubah()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        hapus()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        reset()
        tbIdPinjam.ReadOnly = True
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex < 0 Then Exit Sub

        Dim colName As String = DataGridView1.Columns(e.ColumnIndex).Name
        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

        If colName <> "icUbah" AndAlso colName <> "icHapus" Then
            tbIdPinjam.Text = row.Cells("idPinjam").Value.ToString()
            cbStatus.Text = row.Cells("status").Value.ToString()
            tbNisn.Text = row.Cells("nisn").Value.ToString()
            tbNama.Text = row.Cells("nama").Value.ToString()
            tbIdBarang.Text = row.Cells("idBarang").Value.ToString()
            tbNamaBarang.Text = row.Cells("namaBarang").Value.ToString()
            tbJumlahPinjam.Text = row.Cells("jumlahPinjam").Value.ToString()
            dtPinjam.Value = row.Cells("tanggalPinjam").Value

            tbIdPinjam.ReadOnly = True
            Button1.Enabled = False
            Exit Sub
        End If

        If colName = "icUbah" Then
            Try
                conn.Open()

                Dim cmd As New MySqlCommand("
                UPDATE peminjaman 
                SET status=@status, 
                    nisn=@nisn, 
                    nama=@nama, 
                    idBarang=@idBarang, 
                    namaBarang=@namaBarang,
                    jumlahPinjam=@jumlahPinjam,
                    tanggalPinjam=@tanggalPinjam
                WHERE idPinjam=@idPinjam
            ", conn)

                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@idPinjam", tbIdPinjam.Text)
                cmd.Parameters.AddWithValue("@status", cbStatus.Text)
                cmd.Parameters.AddWithValue("@nisn", tbNisn.Text)
                cmd.Parameters.AddWithValue("@nama", tbNama.Text)
                cmd.Parameters.AddWithValue("@idBarang", tbIdBarang.Text)
                cmd.Parameters.AddWithValue("@namaBarang", tbNamaBarang.Text)
                cmd.Parameters.AddWithValue("@jumlahPinjam", Convert.ToInt32(tbJumlahPinjam.Text))
                cmd.Parameters.AddWithValue("@tanggalPinjam", dtPinjam.Value)

                i = cmd.ExecuteNonQuery()
                If i > 0 Then
                    MessageBox.Show("Data Berhasil Diubah", "Ubah Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    dataload()
                End If

            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                conn.Close()
            End Try

            tbIdPinjam.ReadOnly = False
            Button1.Enabled = True
        End If

        If colName = "icHapus" Then
            If MsgBox("Apakah anda yakin akan menghapus data tersebut?", MsgBoxStyle.Question + vbYesNo) = vbYes Then
                Try
                    conn.Open()
                    Dim cmd As New MySqlCommand("DELETE FROM peminjaman WHERE idPinjam=@idPinjam", conn)
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@idPinjam", row.Cells("idPinjam").Value.ToString())
                    i = cmd.ExecuteNonQuery()

                    If i > 0 Then
                        MessageBox.Show("Data Berhasil Dihapus", "Hapus Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        dataload()
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message)
                Finally
                    conn.Close()
                End Try
            End If
        End If
    End Sub
    Private Sub tbCari_TextChanged(sender As Object, e As EventArgs) Handles tbCari.TextChanged
        DataGridView1.Rows.Clear()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim cmd As New MySqlCommand("SELECT * FROM peminjaman WHERE idPinjam LIKE @cari OR status LIKE @cari OR status LIKE @cari OR nisn LIKE @cari OR nama LIKE @cari", conn)
            cmd.Parameters.AddWithValue("@cari", "%" & tbCari.Text & "%")
            dr = cmd.ExecuteReader

            While dr.Read
                Dim index As Integer = DataGridView1.Rows.Add(
                dr.Item("idPinjam"),
                dr.Item("status"),
                dr.Item("nisn"),
                dr.Item("nama"),
                dr.Item("idBarang"),
                dr.Item("namaBarang"),
                dr.Item("jumlahPinjam"),
                dr.Item("tanggalPinjam")
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
        Dim ds As New DataSet1
        Dim dt As DataTable = ds.Tables("Peminjaman")

        For i = 0 To DataGridView1.Rows.Count - 1
            If Not DataGridView1.Rows(i).IsNewRow Then
                dt.Rows.Add(
                    DataGridView1.Rows(i).Cells(0).Value,
                    DataGridView1.Rows(i).Cells(1).Value,
                    DataGridView1.Rows(i).Cells(2).Value,
                    DataGridView1.Rows(i).Cells(3).Value,
                    DataGridView1.Rows(i).Cells(4).Value,
                    DataGridView1.Rows(i).Cells(5).Value,
                    DataGridView1.Rows(i).Cells(6).Value,
                    DataGridView1.Rows(i).Cells(7).Value
                )
            End If
        Next

        Laporan_Denda.ReportViewer1.LocalReport.ReportPath = "D:\Kampus\ProjectInventarisBarangSekolah - Copy\ProjectInventarisBarangSekolah\Report2.rdlc"
        Laporan_Denda.ReportViewer1.LocalReport.DataSources.Clear()
        Laporan_Denda.ReportViewer1.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("Peminjaman", dt))
        Laporan_Denda.ReportViewer1.RefreshReport()
        Laporan_Denda.Show()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim ds As New DataSet1()
        Dim dt As DataTable = ds.Tables("Peminjaman")

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
            .ReportPath = "D:\Kampus\ProjectInventarisBarangSekolah - Copy\ProjectInventarisBarangSekolah\Report5.rdlc"
            .DataSources.Clear()
            .DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("peminjaman", dt))
        End With

        laporan_Denda.ReportViewer1.RefreshReport()
        laporan_Denda.Show()
    End Sub

    Private Sub tbIdBarang_TextChanged(sender As Object, e As EventArgs) Handles tbIdBarang.TextChanged
        Try
            conn.Open()
            Dim cmd As New MySqlCommand("SELECT namaBarang FROM databarang WHERE idBarang=@id", conn)
            cmd.Parameters.AddWithValue("@id", tbIdBarang.Text)
            dr = cmd.ExecuteReader()
            If dr.Read() Then
                tbNamaBarang.Text = dr("namaBarang").ToString()
            Else
                tbNamaBarang.Text = ""
            End If
        Catch
        Finally
            conn.Close()
        End Try
    End Sub
End Class
