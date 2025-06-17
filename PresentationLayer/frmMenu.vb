Public Class frmMenu
    Private Sub frmMenu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Menú Principal"
        Me.Size = New Size(400, 300)

        Dim btnClientes As New Button With {
            .Text = "Clientes",
            .Size = New Size(200, 40),
            .Location = New Point(100, 40)
        }
        AddHandler btnClientes.Click, AddressOf AbrirClientes
        Me.Controls.Add(btnClientes)

        Dim btnProductos As New Button With {
            .Text = "Productos",
            .Size = New Size(200, 40),
            .Location = New Point(100, 90)
        }
        AddHandler btnProductos.Click, AddressOf AbrirProductos
        Me.Controls.Add(btnProductos)

        Dim btnVentas As New Button With {
            .Text = "Ventas",
            .Size = New Size(200, 40),
            .Location = New Point(100, 140)
        }
        AddHandler btnVentas.Click, AddressOf AbrirVentas
        Me.Controls.Add(btnVentas)

        Dim btnSalir As New Button With {
            .Text = "Salir",
            .Size = New Size(200, 40),
            .Location = New Point(100, 190)
        }
        AddHandler btnSalir.Click, AddressOf SubSalir
        Me.Controls.Add(btnSalir)
    End Sub

    Private Sub AbrirClientes(sender As Object, e As EventArgs)
        Dim frm As New frmClientes()
        frm.ShowDialog()
    End Sub

    Private Sub AbrirProductos(sender As Object, e As EventArgs)
        Dim frm As New frmProductos()
        frm.ShowDialog()
    End Sub

    Private Sub AbrirVentas(sender As Object, e As EventArgs)
        Dim frm As New frmVentas()
        frm.ShowDialog()
    End Sub

    Private Sub SubSalir(sender As Object, e As EventArgs)
        Application.Exit()
    End Sub
End Class
