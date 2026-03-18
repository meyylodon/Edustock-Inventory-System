Imports MySql.Data.MySqlClient

Public Class Pengembalian
    Dim conn As New MySqlConnection("server=localhost;port=3306;username=root;password=;database=db_inventaris;")
    Dim dr As MySqlDataReader

    Public Sub simpan()
        If String.IsNullOrWhiteSpace(tbIdPinjam.Text) Then
            MsgBox("ID Pinjam wajib diisi.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Dim jumlah As Integer = 0
        Integer.TryParse(tbJumlahPinjam.Text, jumlah)

        Try
            conn.Open()

            Dim sql As String = "
            INSERT INTO pengembalian
                (idPinjam, status, nisn, nama, idBarang, namaBarang, jumlahPinjam, tanggalPengembalian)
            VALUES
                (@idPinjam, @status, @nisn, @nama, @idBarang, @namaBarang, @jumlahPinjam, @tanggalPengembalian)
            "

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@idPinjam", tbIdPinjam.Text)
                cmd.Parameters.AddWithValue("@status", cbStatus.Text)
                cmd.Parameters.AddWithValue("@nisn", If(String.IsNullOrWhiteSpace(tbNisn.Text), DBNull.Value, CType(tbNisn.Text, Object)))
                cmd.Parameters.AddWithValue("@nama", tbNama.Text)
                cmd.Parameters.AddWithValue("@idBarang", tbIdBarang.Text)
                cmd.Parameters.AddWithValue("@namaBarang", tbNamaBarang.Text)
                cmd.Parameters.AddWithValue("@jumlahPinjam", jumlah)
                cmd.Parameters.AddWithValue("@tanggalPengembalian", dtKembali.Value)

                If cmd.ExecuteNonQuery() > 0 Then
                    ' Update stok barang (tambahkan jumlah pengembalian)
                    Dim updateStok As String = "
                        UPDATE databarang
                        SET stok = stok + @jumlah
                        WHERE idBarang = @idBarang
                    "
                    Using cmd2 As New MySqlCommand(updateStok, conn)
                        cmd2.Parameters.AddWithValue("@jumlah", jumlah)
                        cmd2.Parameters.AddWithValue("@idBarang", tbIdBarang.Text)
                        cmd2.ExecuteNonQuery()
                    End Using

                    MessageBox.Show("Pengembalian disimpan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("Gagal menyimpan pengembalian.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End Using

        Catch ex As Exception
            MsgBox("Error saat menyimpan: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Public Sub ubah()
        If String.IsNullOrWhiteSpace(tbIdPengembalian.Text) Then
            MsgBox("Pilih data terlebih dahulu untuk diubah.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Dim jumlah As Integer = 0
        Integer.TryParse(tbJumlahPinjam.Text, jumlah)

        Try
            conn.Open()
            Dim sql As String = "
                UPDATE pengembalian SET
                    idPinjam = @idPinjam,
                    status = @status,
                    nisn = @nisn,
                    nama = @nama,
                    idBarang = @idBarang,
                    namaBarang = @namaBarang,
                    jumlahPinjam = @jumlahPinjam,
                    tanggalPengembalian = @tanggalPengembalian
                WHERE idPengembalian = @idPengembalian
            "

            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@idPengembalian", tbIdPengembalian.Text)
                cmd.Parameters.AddWithValue("@idPinjam", tbIdPinjam.Text)
                cmd.Parameters.AddWithValue("@status", cbStatus.Text)
                cmd.Parameters.AddWithValue("@nisn", If(String.IsNullOrWhiteSpace(tbNisn.Text), DBNull.Value, CType(tbNisn.Text, Object)))
                cmd.Parameters.AddWithValue("@nama", tbNama.Text)
                cmd.Parameters.AddWithValue("@idBarang", tbIdBarang.Text)
                cmd.Parameters.AddWithValue("@namaBarang", tbNamaBarang.Text)
                cmd.Parameters.AddWithValue("@jumlahPinjam", jumlah)
                cmd.Parameters.AddWithValue("@tanggalPengembalian", dtKembali.Value)

                If cmd.ExecuteNonQuery() > 0 Then
                    MessageBox.Show("Data berhasil diubah.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("Tidak ada perubahan yang dilakukan.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using

        Catch ex As Exception
            MsgBox("Gagal mengubah: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub
    Public Sub hapus()
        If String.IsNullOrWhiteSpace(tbIdPengembalian.Text) Then
            MsgBox("Pilih data untuk dihapus.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        If MsgBox("Yakin ingin menghapus data?", vbYesNo + vbQuestion) = vbYes Then
            Try
                conn.Open()
                Dim sql As String = "DELETE FROM pengembalian WHERE idPengembalian = @id"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id", CInt(tbIdPengembalian.Text))
                    If cmd.ExecuteNonQuery() > 0 Then
                        MessageBox.Show("Data berhasil dihapus.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show("Gagal menghapus data.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using
            Catch ex As Exception
                MsgBox("Gagal menghapus: " & ex.Message)
            Finally
                If conn.State = ConnectionState.Open Then conn.Close()
            End Try
        End If
    End Sub

    Public Sub loadNextId()
        Try
            conn.Open()
            Dim cmd As New MySqlCommand("SHOW TABLE STATUS LIKE 'pengembalian'", conn)
            Using rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    tbIdPengembalian.Text = rd("Auto_increment").ToString()
                    tbIdPengembalian.ReadOnly = True
                End If
            End Using
        Catch ex As Exception
            MsgBox("Error loadNextId: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Public Sub tampilDataPeminjaman()
        Try
            Dim da As New MySqlDataAdapter("SELECT * FROM peminjaman", conn)
            Dim dt As New DataTable
            da.Fill(dt)
            DataGridView1.DataSource = dt
        Catch ex As Exception
            MsgBox("Gagal menampilkan data peminjaman: " & ex.Message)
        End Try
    End Sub

    ' ======= DATALOAD (perbaikan: sertakan idPinjam & jumlahPinjam) =======
    Public Sub dataload()
        DataGridView1.Rows.Clear()

        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim cmd As New MySqlCommand("
            SELECT 
                idPengembalian,
                idPinjam,
                status,
                nisn,
                nama,
                idBarang,
                namaBarang,
                jumlahPinjam,
                tanggalPengembalian
            FROM pengembalian
            ORDER BY idPengembalian DESC
        ", conn)

            dr = cmd.ExecuteReader()

            While dr.Read()
                Dim idx As Integer = DataGridView1.Rows.Add(
                dr("idPengembalian"),
                dr("idPinjam"),
                dr("status"),
                dr("nisn"),
                dr("nama"),
                dr("idBarang"),
                dr("namaBarang"),
                dr("jumlahPinjam"),
                dr("tanggalPengembalian")
            )

                If DataGridView1.Columns.Contains("icUbah") Then
                    DataGridView1.Rows(idx).Cells("icUbah").Value = My.Resources.iconUbah
                End If
                If DataGridView1.Columns.Contains("icHapus") Then
                    DataGridView1.Rows(idx).Cells("icHapus").Value = My.Resources.iconHapus
                End If
            End While

            dr.Close()

            DataGridView1.RowTemplate.Height = 40
            DataGridView1.AllowUserToAddRows = False

            If DataGridView1.Columns.Contains("icUbah") Then
                CType(DataGridView1.Columns("icUbah"), DataGridViewImageColumn).ImageLayout =
                DataGridViewImageCellLayout.Zoom
            End If

            If DataGridView1.Columns.Contains("icHapus") Then
                CType(DataGridView1.Columns("icHapus"), DataGridViewImageColumn).ImageLayout =
                DataGridViewImageCellLayout.Zoom
            End If

        Catch ex As Exception
            MsgBox("Gagal memuat data: " & ex.Message)

        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub



    ' === Fungsi kecil untuk menghindari error DBNull ===
    Private Function SafeVal(rdr As MySqlDataReader, kolom As String) As String
        If rdr(kolom) Is DBNull.Value Then Return ""
        Return rdr(kolom).ToString()
    End Function


    ' ======= DataGridView CellClick (mapping sesuai urutan kolom di atas) =======
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex < 0 Then Exit Sub
        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
        Dim colName As String = DataGridView1.Columns(e.ColumnIndex).Name

        ' jika klik isi baris (bukan icon)
        If colName <> "icUbah" AndAlso colName <> "icHapus" Then
            tbIdPengembalian.Text = row.Cells(0).Value.ToString()
            tbIdPinjam.Text = row.Cells(1).Value.ToString()
            cbStatus.Text = row.Cells(2).Value.ToString()
            tbNisn.Text = row.Cells(3).Value.ToString()
            tbNama.Text = row.Cells(4).Value.ToString()
            tbIdBarang.Text = row.Cells(5).Value.ToString()
            tbNamaBarang.Text = row.Cells(6).Value.ToString()
            tbJumlahPinjam.Text = row.Cells(7).Value.ToString()

            Dim tgl As Date
            If Date.TryParse(row.Cells(8).Value.ToString(), tgl) Then
                dtKembali.Value = tgl
            Else
                dtKembali.Value = Today
            End If

            tbIdPengembalian.ReadOnly = True
            Button1.Enabled = False
            Exit Sub
        End If

        ' jika klik icon ubah
        If colName = "icUbah" Then
            ' bawaan: isi control dulu (agar user bisa edit), lalu panggil ubah()
            tbIdPengembalian.Text = row.Cells(0).Value.ToString()
            tbIdPinjam.Text = row.Cells(1).Value.ToString()
            cbStatus.Text = row.Cells(2).Value.ToString()
            tbNisn.Text = row.Cells(3).Value.ToString()
            tbNama.Text = row.Cells(4).Value.ToString()
            tbIdBarang.Text = row.Cells(5).Value.ToString()
            tbNamaBarang.Text = row.Cells(6).Value.ToString()
            tbJumlahPinjam.Text = row.Cells(7).Value.ToString()

            Dim tgl As Date
            If Date.TryParse(row.Cells(8).Value.ToString(), tgl) Then
                dtKembali.Value = tgl
            Else
                dtKembali.Value = Today
            End If

            Exit Sub
        End If

        If colName = "icHapus" Then
            If MsgBox("Yakin ingin menghapus data ini?", vbYesNo + vbQuestion) = vbYes Then
                Try
                    conn.Open()
                    Using cmd As New MySqlCommand("DELETE FROM pengembalian WHERE idPengembalian = @id", conn)
                        cmd.Parameters.AddWithValue("@id", CInt(row.Cells(0).Value))
                        If cmd.ExecuteNonQuery() > 0 Then
                            MessageBox.Show("Data berhasil dihapus.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            dataload()
                        End If
                    End Using
                Catch ex As Exception
                    MsgBox("Gagal menghapus: " & ex.Message)
                Finally
                    If conn.State = ConnectionState.Open Then conn.Close()
                End Try
            End If
        End If
    End Sub

    ' ======= Pencarian sederhana =======
    Private Sub tbCari_TextChanged(sender As Object, e As EventArgs) Handles tbCari.TextChanged
        DataGridView1.Rows.Clear()
        Try
            conn.Open()
            Dim sql As String = "
                SELECT 
                    idPengembalian,
                    idPinjam,
                    status,
                    nisn,
                    nama,
                    idBarang,
                    namaBarang,
                    jumlahPinjam,
                    tanggalPengembalian
                FROM pengembalian
                WHERE idPengembalian LIKE @cari
                   OR idPinjam LIKE @cari
                   OR status LIKE @cari
                   OR nisn LIKE @cari
                   OR nama LIKE @cari
                   OR namaBarang LIKE @cari
            "
            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@cari", "%" & tbCari.Text & "%")
                Using rdr As MySqlDataReader = cmd.ExecuteReader()
                    While rdr.Read()
                        Dim idx As Integer = DataGridView1.Rows.Add(
                            If(rdr("idPengembalian") Is DBNull.Value, "", rdr("idPengembalian")),
                            If(rdr("idPinjam") Is DBNull.Value, "", rdr("idPinjam")),
                            If(rdr("status") Is DBNull.Value, "", rdr("status")),
                            If(rdr("nisn") Is DBNull.Value, "", rdr("nisn")),
                            If(rdr("nama") Is DBNull.Value, "", rdr("nama")),
                            If(rdr("idBarang") Is DBNull.Value, "", rdr("idBarang")),
                            If(rdr("namaBarang") Is DBNull.Value, "", rdr("namaBarang")),
                            If(rdr("jumlahPinjam") Is DBNull.Value, 0, rdr("jumlahPinjam")),
                            If(rdr("tanggalPengembalian") Is DBNull.Value, "", rdr("tanggalPengembalian"))
                        )

                        If DataGridView1.Columns.Contains("icUbah") Then DataGridView1.Rows(idx).Cells("icUbah").Value = My.Resources.iconUbah
                        If DataGridView1.Columns.Contains("icHapus") Then DataGridView1.Rows(idx).Cells("icHapus").Value = My.Resources.iconHapus
                    End While
                End Using
            End Using
        Catch ex As Exception
            MsgBox("Gagal mencari data: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim ds As New DataSet2
        Dim dt As New DataTable

        dt = ds.Tables("Pengembalian")
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
        DataGridView1.Rows(i).Cells(7).Value,
        DataGridView1.Rows(i).Cells(8).Value
    )
            End If
        Next
        Laporan_Denda.ReportViewer1.LocalReport.ReportPath = "D:\Kampus\ProjectInventarisBarangSekolah - Copy\ProjectInventarisBarangSekolah\Report3.rdlc"
        Laporan_Denda.ReportViewer1.LocalReport.DataSources.Clear()
        Laporan_Denda.ReportViewer1.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("Pengembalian", dt))
        Laporan_Denda.Show()
        Laporan_Denda.ReportViewer1.RefreshReport()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim ds As New DataSet2()
        Dim dt As DataTable = ds.Tables("Pengembalian")

        For Each row As DataGridViewRow In DataGridView1.Rows
            If Not row.IsNewRow Then
                dt.Rows.Add(
                row.Cells(0).Value,
                row.Cells(1).Value,
                row.Cells(2).Value,
                row.Cells(3).Value,
                row.Cells(4).Value,
                row.Cells(5).Value,
                row.Cells(6).Value,
                row.Cells(7).Value,
                row.Cells(8).Value
            )
            End If
        Next
        Dim laporan_Denda As New Laporan_Denda()

        With laporan_Denda.ReportViewer1.LocalReport
            .ReportPath = "D:\Kampus\ProjectInventarisBarangSekolah - Copy\ProjectInventarisBarangSekolah\Report6.rdlc"
            .DataSources.Clear()
            .DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("Pengembalian", dt))
        End With

        laporan_Denda.ReportViewer1.RefreshReport()
        laporan_Denda.Show()

    End Sub
    Private Sub Pengembalian_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dataload()
        tbIdPengembalian.ReadOnly = True
        loadNextId()
    End Sub

    Private Sub tbIdPinjam_TextChanged(sender As Object, e As EventArgs) Handles tbIdPinjam.TextChanged
        If String.IsNullOrWhiteSpace(tbIdPinjam.Text) Then
            tbNisn.Clear() : cbStatus.SelectedIndex = -1 : tbNama.Clear()
            tbIdBarang.Clear() : tbNamaBarang.Clear() : tbJumlahPinjam.Clear()
            Exit Sub
        End If

        Try
            conn.Open()
            Dim query As String = "
                SELECT idPinjam, status, nisn, nama, idBarang, namaBarang, jumlahPinjam
                FROM peminjaman
                WHERE idPinjam = @idPinjam
            "
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@idPinjam", tbIdPinjam.Text)
                Using rdr As MySqlDataReader = cmd.ExecuteReader()
                    If rdr.Read() Then
                        tbNisn.Text = If(rdr("nisn") Is DBNull.Value, "", rdr("nisn").ToString())
                        cbStatus.Text = If(rdr("status") Is DBNull.Value, "", rdr("status").ToString())
                        tbNama.Text = If(rdr("nama") Is DBNull.Value, "", rdr("nama").ToString())
                        tbIdBarang.Text = If(rdr("idBarang") Is DBNull.Value, "", rdr("idBarang").ToString())
                        tbNamaBarang.Text = If(rdr("namaBarang") Is DBNull.Value, "", rdr("namaBarang").ToString())
                        tbJumlahPinjam.Text = If(rdr("jumlahPinjam") Is DBNull.Value, "0", rdr("jumlahPinjam").ToString())
                    Else
                        tbNisn.Clear() : cbStatus.SelectedIndex = -1 : tbNama.Clear()
                        tbIdBarang.Clear() : tbNamaBarang.Clear() : tbJumlahPinjam.Clear()
                    End If
                End Using
            End Using
        Catch ex As Exception
            MsgBox("Error: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Public Sub reset()
        tbIdPengembalian.Text = ""
        tbIdPinjam.Text = ""
        cbStatus.Text = ""
        tbNisn.Text = ""
        tbNama.Text = ""
        tbIdBarang.Text = ""
        tbNamaBarang.Text = ""
        tbJumlahPinjam.Text = ""
        dtKembali.Value = Today
        tbNama.ReadOnly = False
        Button1.Enabled = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        simpan()
        dataload()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        reset()
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Dim f1 As New Dashboard()
        f1.Show()
        Me.Hide()
    End Sub
End Class
