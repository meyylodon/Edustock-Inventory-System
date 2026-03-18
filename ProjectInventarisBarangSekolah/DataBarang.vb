Imports System.Reflection
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Data_Barang
    Dim conn As New MySqlConnection("server=localhost;port=3306;username=root;password=;database=db_inventaris;")
    Dim i As Integer
    Dim dr As MySqlDataReader

    Private Sub Data_Barang_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dataload()
        loadNextId()
        tbIdBarang.ReadOnly = True
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Dim f1 As New Dashboard()
        f1.Show()
        Me.Hide()
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex < 0 Then Exit Sub
        Dim colName As String = DataGridView1.Columns(e.ColumnIndex).Name
        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
        If colName <> "icUbah" AndAlso colName <> "icHapus" Then
            tbIdBarang.Text = row.Cells(0).Value.ToString()
            tbNamaBarang.Text = row.Cells(1).Value.ToString()
            cbKategori.Text = row.Cells(2).Value.ToString()
            tbStok.Text = row.Cells(3).Value.ToString()

            Dim kondisi As String = row.Cells(4).Value.ToString()
            If kondisi = "Baik" Then
                rbBaik.Checked = True
            ElseIf kondisi = "Rusak" Then
                rbRusak.Checked = True
            Else
                rbBaik.Checked = False
                rbRusak.Checked = False
            End If

            tbIdBarang.ReadOnly = True
            Button1.Enabled = False
            Exit Sub
        End If

        If colName = "icUbah" Then
            Try
                Dim kondisi As String = ""
                If rbBaik.Checked Then
                    kondisi = rbBaik.Text
                ElseIf rbRusak.Checked Then
                    kondisi = rbRusak.Text
                End If

                conn.Open()
                Dim cmd As New MySqlCommand("UPDATE databarang SET namaBarang=@namaBarang, kategori=@kategori, stok=@stok, kondisi=@kondisi WHERE idBarang=@idBarang", conn)
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@idBarang", tbIdBarang.Text)
                cmd.Parameters.AddWithValue("@namaBarang", tbNamaBarang.Text)
                cmd.Parameters.AddWithValue("@kategori", cbKategori.Text)
                cmd.Parameters.AddWithValue("@stok", tbStok.Text)
                cmd.Parameters.AddWithValue("@kondisi", kondisi)
                i = cmd.ExecuteNonQuery

                If i > 0 Then
                    MessageBox.Show("Data Berhasil Diubah", "Ubah Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    dataload()
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                conn.Close()
            End Try

            tbIdBarang.ReadOnly = False
            Button1.Enabled = True
        End If

        If colName = "icHapus" Then
            If MsgBox("Apakah anda yakin akan menghapus data tersebut?", MsgBoxStyle.Question + vbYesNo) = vbYes Then
                Try
                    conn.Open()
                    Dim cmd As New MySqlCommand("DELETE FROM databarang WHERE idBarang=@idBarang", conn)
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@idBarang", row.Cells(0).Value.ToString())
                    i = cmd.ExecuteNonQuery

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

    Public Sub simpan()
        Dim kondisi As String = ""
        If rbBaik.Checked Then
            kondisi = rbBaik.Text
        ElseIf rbRusak.Checked Then
            kondisi = rbRusak.Text
        End If

        Try
            conn.Open()

            Dim query As String =
        "INSERT INTO databarang (namaBarang, kategori, stok, kondisi)
         VALUES (@namaBarang, @kategori, @stok, @kondisi);
         SELECT LAST_INSERT_ID();"

            Dim cmd As New MySqlCommand(query, conn)

            cmd.Parameters.AddWithValue("@namaBarang", tbNamaBarang.Text)
            cmd.Parameters.AddWithValue("@kategori", cbKategori.Text)
            cmd.Parameters.AddWithValue("@stok", tbStok.Text)
            cmd.Parameters.AddWithValue("@kondisi", kondisi)

            Dim newID As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            tbIdBarang.Text = newID.ToString()

            MessageBox.Show("Data Berhasil Disimpan", "Tambah Data", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error : " & ex.Message, "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
            dataload()
        End Try
    End Sub

    Public Sub ubah()
        Dim kondisi As String = ""
        If rbBaik.Checked Then
            kondisi = rbBaik.Text
        ElseIf rbRusak.Checked Then
            kondisi = rbRusak.Text
        End If

        Try
            conn.Open()
            Dim cmd As New MySqlCommand("UPDATE databarang SET namaBarang=@namaBarang, kategori=@kategori, stok=@stok, kondisi=@kondisi WHERE idBarang=@idBarang", conn)
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@idBarang", tbIdBarang.Text)
            cmd.Parameters.AddWithValue("@namaBarang", tbNamaBarang.Text)
            cmd.Parameters.AddWithValue("@kategori", cbKategori.Text)
            cmd.Parameters.AddWithValue("@stok", tbStok.Text)
            cmd.Parameters.AddWithValue("@kondisi", kondisi)
            i = cmd.ExecuteNonQuery
            If i > 0 Then
                MessageBox.Show("Data Berhasil Diubah", "Ubah Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
        tbIdBarang.ReadOnly = False
        Button1.Enabled = True
    End Sub

    Public Sub hapus()
        If MsgBox("Apakah anda yakin akan menghapus data tersebut?", MsgBoxStyle.Question + vbYesNo) = vbYes Then
            Try
                conn.Open()
                Dim cmd As New MySqlCommand("DELETE FROM databarang WHERE idBarang=@idBarang", conn)
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@idBarang", tbIdBarang.Text)
                i = cmd.ExecuteNonQuery
                If i > 0 Then
                    MessageBox.Show("Data Berhasil Dihapus", "Hapus Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                conn.Close()
            End Try
            tbIdBarang.ReadOnly = False
            Button1.Enabled = True
        End If
    End Sub
    Public Sub autoIdBarang()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim cmd As New MySqlCommand("SELECT MAX(idBarang) FROM databarang", conn)
            Dim lastId As Object = cmd.ExecuteScalar()

            If IsDBNull(lastId) Then
                tbIdBarang.Text = "1"
            Else
                tbIdBarang.Text = (CInt(lastId) + 1).ToString()
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
            Dim cmd As New MySqlCommand("SHOW TABLE STATUS LIKE 'databarang'", conn)
            Dim rd As MySqlDataReader = cmd.ExecuteReader()

            If rd.Read() Then
                Dim nextId As Integer = rd("Auto_increment")
                tbIdBarang.Text = nextId.ToString()
                tbIdBarang.ReadOnly = True
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub
    Public Sub reset()
        tbIdBarang.Text = ""
        tbNamaBarang.Text = ""
        cbKategori.Text = ""
        tbStok.Text = ""
        rbBaik.Checked = False
        rbRusak.Checked = False
        tbIdBarang.ReadOnly = False
        Button1.Enabled = True
    End Sub

    Public Sub dataload()
        DataGridView1.Rows.Clear()
        Try
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If

            conn.Open()
            Dim cmd As New MySqlCommand("SELECT * FROM databarang", conn)
            dr = cmd.ExecuteReader
            While dr.Read
                Dim index As Integer = DataGridView1.Rows.Add(
                dr.Item("idBarang"),
                dr.Item("namaBarang"),
                dr.Item("kategori"),
                dr.Item("stok"),
                dr.Item("kondisi")
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

    Private Sub tbCari_TextChanged(sender As Object, e As EventArgs) Handles tbCari.TextChanged
        DataGridView1.Rows.Clear()
        Try
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If

            conn.Open()
            Dim cmd As New MySqlCommand("SELECT * FROM databarang WHERE namaBarang LIKE @cari OR kategori LIKE @cari", conn)
            cmd.Parameters.AddWithValue("@cari", "%" & tbCari.Text & "%")
            dr = cmd.ExecuteReader

            While dr.Read
                Dim index As Integer = DataGridView1.Rows.Add(
                dr.Item("idBarang"),
                dr.Item("namaBarang"),
                dr.Item("kategori"),
                dr.Item("stok"),
                dr.Item("kondisi")
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

    Private Sub Label7_Click(sender As Object, e As EventArgs) Handles Label7.Click

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        ubah()
        dataload()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        hapus()
        dataload()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        reset()
        tbIdBarang.ReadOnly = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        simpan()
        dataload()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim ds As New DataSet1
        Dim dt As New DataTable

        dt = ds.Tables("DataBarang")
        For i = 0 To DataGridView1.Rows.Count - 1
            If Not DataGridView1.Rows(i).IsNewRow Then
                dt.Rows.Add(
            DataGridView1.Rows(i).Cells(0).Value,
            DataGridView1.Rows(i).Cells(1).Value,
            DataGridView1.Rows(i).Cells(2).Value,
            DataGridView1.Rows(i).Cells(3).Value,
            DataGridView1.Rows(i).Cells(4).Value
        )
            End If
        Next
        Laporan_Denda.ReportViewer1.LocalReport.ReportPath = "D:\Kampus\ProjectInventarisBarangSekolah - Copy\ProjectInventarisBarangSekolah\Report1.rdlc"
        Laporan_Denda.ReportViewer1.LocalReport.DataSources.Clear()
        Laporan_Denda.ReportViewer1.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("DataBarang", dt))
        Laporan_Denda.Show()
        Laporan_Denda.ReportViewer1.RefreshReport()

    End Sub
End Class
