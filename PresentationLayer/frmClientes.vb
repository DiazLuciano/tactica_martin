Imports System.Data.SqlClient

Public Class frmClientes
    ' Conexión usando la cadena desde App.config
    Private ReadOnly _clienteService As New ClienteService()
    Private _clientesActuales As List(Of Cliente)
    Private _clienteSeleccionado As Cliente = Nothing

    ' Declaración de controles
    ' Controles del formulario (deben coincidir con el diseñador)
    Private WithEvents dgvClientes As DataGridView
    Private WithEvents txtNombre As TextBox
    Private WithEvents txtTelefono As TextBox
    Private WithEvents txtCorreo As TextBox
    Private WithEvents txtBuscar As TextBox
    Private WithEvents btnAgregar As Button
    Private WithEvents btnModificar As Button
    Private WithEvents btnEliminar As Button
    Private WithEvents btnLimpiar As Button
    Private WithEvents btnGuardar As Button
    Private WithEvents btnCancelar As Button


    ' Método para inicializar los controles manualmente
    Private Sub frmClientes_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarControles()
        CargarClientes()
        ConfigurarDataGridView()
    End Sub

    Private Sub ConfigurarControles()
        ' Configuración básica del formulario
        Me.Text = "Gestión de Clientes"
        Me.ClientSize = New Size(650, 450)

        ' Crear y configurar DataGridView
        dgvClientes = New DataGridView()
        With dgvClientes
            .Name = "dgvClientes"
            .Location = New Point(20, 20)
            .Size = New Size(610, 200)
            .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            .TabIndex = 0
        End With
        Me.Controls.Add(dgvClientes)

        ' Crear etiquetas y campos de texto
        Dim lblCliente As New Label With {.Text = "Cliente:", .Location = New Point(20, 240), .Size = New Size(80, 20)}
        Me.Controls.Add(lblCliente)

        txtNombre = New TextBox()
        With txtNombre
            .Name = "txtCliente"
            .Location = New Point(110, 240)
            .Size = New Size(300, 20)
            .TabIndex = 1
        End With
        Me.Controls.Add(txtNombre)

        Dim lblTelefono As New Label With {.Text = "Teléfono:", .Location = New Point(20, 280), .Size = New Size(80, 20)}
        Me.Controls.Add(lblTelefono)

        txtTelefono = New TextBox()
        With txtTelefono
            .Name = "txtTelefono"
            .Location = New Point(110, 280)
            .Size = New Size(300, 20)
            .TabIndex = 2
        End With
        Me.Controls.Add(txtTelefono)

        Dim lblCorreo As New Label With {.Text = "Correo:", .Location = New Point(20, 320), .Size = New Size(80, 20)}
        Me.Controls.Add(lblCorreo)

        txtCorreo = New TextBox()
        With txtCorreo
            .Name = "txtCorreo"
            .Location = New Point(110, 320)
            .Size = New Size(300, 20)
            .TabIndex = 3
        End With
        Me.Controls.Add(txtCorreo)

        txtBuscar = New TextBox()
        With txtCorreo
            .Name = "txtBuscar"
            .Location = New Point(110, 320)
            .Size = New Size(300, 20)
            .TabIndex = 3
        End With
        Me.Controls.Add(txtBuscar)

        ' Crear botones
        btnGuardar = New Button()
        With btnGuardar
            .Name = "btnGuardar"
            .Text = "Guardar"
            .Location = New Point(110, 360)
            .Size = New Size(150, 30)
            .TabIndex = 8
            .Enabled = False
        End With
        Me.Controls.Add(btnGuardar)

        btnCancelar = New Button()
        With btnCancelar
            .Name = "btnCancelar"
            .Text = "Cancelar"
            .Location = New Point(270, 360)
            .Size = New Size(150, 30)
            .TabIndex = 9
            .Enabled = False
        End With
        Me.Controls.Add(btnCancelar)

        btnAgregar = New Button()
        With btnAgregar
            .Name = "btnAgregar"
            .Text = "Agregar"
            .Location = New Point(430, 240)
            .Size = New Size(150, 30)
            .TabIndex = 4
        End With
        Me.Controls.Add(btnAgregar)

        btnModificar = New Button()
        With btnModificar
            .Name = "btnModificar"
            .Text = "Modificar"
            .Location = New Point(430, 280)
            .Size = New Size(150, 30)
            .TabIndex = 5
        End With
        Me.Controls.Add(btnModificar)

        btnEliminar = New Button()
        With btnEliminar
            .Name = "btnEliminar"
            .Text = "Eliminar"
            .Location = New Point(430, 320)
            .Size = New Size(150, 30)
            .TabIndex = 6
        End With
        Me.Controls.Add(btnEliminar)

        btnLimpiar = New Button()
        With btnLimpiar
            .Name = "btnLimpiar"
            .Text = "Limpiar"
            .Location = New Point(430, 360)
            .Size = New Size(150, 30)
            .TabIndex = 7
        End With
        Me.Controls.Add(btnLimpiar)
    End Sub

    Private Sub ConfigurarDataGridView()
        dgvClientes.AutoGenerateColumns = False
        dgvClientes.AllowUserToAddRows = False
        dgvClientes.AllowUserToDeleteRows = False
        dgvClientes.ReadOnly = True
        dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvClientes.MultiSelect = False

        ' Configurar columnas
        dgvClientes.Columns.Clear()
        dgvClientes.Columns.Add("ID", "ID")
        dgvClientes.Columns.Add("Nombre", "Nombre")
        dgvClientes.Columns.Add("Telefono", "Teléfono")
        dgvClientes.Columns.Add("Correo", "Correo")

        ' Configurar propiedades de columnas
        dgvClientes.Columns("ID").DataPropertyName = "ID"
        dgvClientes.Columns("Nombre").DataPropertyName = "Nombre"
        dgvClientes.Columns("Telefono").DataPropertyName = "Telefono"
        dgvClientes.Columns("Correo").DataPropertyName = "Correo"
    End Sub

    Private Sub CargarClientes()
        Try
            _clientesActuales = _clienteService.ObtenerTodosClientes()
            ActualizarDataGridView()
        Catch ex As Exception
            MostrarError($"Error al cargar clientes: {ex.Message}")
        End Try
    End Sub

    Private Sub ActualizarDataGridView()
        dgvClientes.DataSource = Nothing
        dgvClientes.DataSource = _clientesActuales
        dgvClientes.ClearSelection()
        _clienteSeleccionado = Nothing
    End Sub

    Private Sub dgvClientes_SelectionChanged(sender As Object, e As EventArgs) Handles dgvClientes.SelectionChanged
        If dgvClientes.SelectedRows.Count > 0 Then
            _clienteSeleccionado = DirectCast(dgvClientes.SelectedRows(0).DataBoundItem, Cliente)
            MostrarDatosCliente(_clienteSeleccionado)
        Else
            _clienteSeleccionado = Nothing
            LimpiarCampos()
        End If
    End Sub

    Private Sub MostrarDatosCliente(cliente As Cliente)
        txtNombre.Text = cliente.Nombre
        txtTelefono.Text = cliente.Telefono
        txtCorreo.Text = cliente.Correo
    End Sub

    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles txtBuscar.TextChanged
        Try
            If String.IsNullOrWhiteSpace(txtBuscar.Text) Then
                CargarClientes()
            Else
                _clientesActuales = _clienteService.BuscarClientes(txtBuscar.Text.Trim())
                ActualizarDataGridView()
            End If
        Catch ex As Exception
            MostrarError($"Error al buscar clientes: {ex.Message}")
        End Try
    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        EstablecerModoEdicion(esNuevo:=True)
    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        If _clienteSeleccionado Is Nothing Then
            MostrarAdvertencia("Seleccione un cliente para modificar")
            Return
        End If
        EstablecerModoEdicion(esNuevo:=False)
    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        If _clienteSeleccionado Is Nothing Then
            MostrarAdvertencia("Seleccione un cliente para eliminar")
            Return
        End If

        If MessageBox.Show("¿Está seguro que desea eliminar este cliente?", "Confirmar",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            Return ' ← Solo salimos si el usuario dice que NO
        End If

        Try
            _clienteService.EliminarCliente(_clienteSeleccionado.ID)
            MostrarExito("Cliente eliminado correctamente")
            CargarClientes()
        Catch ex As Exception
            MostrarError($"Error al eliminar cliente: {ex.Message}")
        End Try
    End Sub


    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If Not ValidarCampos() Then Return

        Dim cliente As Cliente
        If _clienteSeleccionado Is Nothing Then
            ' Nuevo cliente
            cliente = New Cliente With {
                .Nombre = txtNombre.Text.Trim(),
                .Telefono = txtTelefono.Text.Trim(),
                .Correo = txtCorreo.Text.Trim()
            }
        Else
            ' Cliente existente
            cliente = _clienteSeleccionado
            cliente.Nombre = txtNombre.Text.Trim()
            cliente.Telefono = txtTelefono.Text.Trim()
            cliente.Correo = txtCorreo.Text.Trim()
        End If

        Try
            If _clienteSeleccionado Is Nothing Then
                _clienteService.RegistrarCliente(cliente)
                MostrarExito("Cliente registrado correctamente")
            Else
                _clienteService.ActualizarCliente(cliente)
                MostrarExito("Cliente actualizado correctamente")
            End If

            CargarClientes()
            EstablecerModoInicial()
        Catch ex As Exception
            MostrarError(ex.Message)
        End Try
    End Sub

    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        EstablecerModoInicial()
        If _clienteSeleccionado IsNot Nothing Then
            MostrarDatosCliente(_clienteSeleccionado)
        Else
            LimpiarCampos()
        End If
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        LimpiarCampos()
        dgvClientes.ClearSelection()
        _clienteSeleccionado = Nothing
    End Sub

    Private Function ValidarCampos() As Boolean
        If String.IsNullOrWhiteSpace(txtNombre.Text) Then
            MostrarAdvertencia("El nombre del cliente es obligatorio")
            txtNombre.Focus()
            Return False
        End If

        If Not String.IsNullOrEmpty(txtCorreo.Text) AndAlso Not txtCorreo.Text.Contains("@") Then
            MostrarAdvertencia("El correo electrónico no es válido")
            txtCorreo.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub EstablecerModoEdicion(esNuevo As Boolean)
        txtNombre.ReadOnly = False
        txtTelefono.ReadOnly = False
        txtCorreo.ReadOnly = False

        btnAgregar.Enabled = False
        btnModificar.Enabled = False
        btnEliminar.Enabled = False
        btnLimpiar.Enabled = False
        dgvClientes.Enabled = False
        txtBuscar.Enabled = False

        btnGuardar.Enabled = True
        btnCancelar.Enabled = True

        If esNuevo Then
            LimpiarCampos()
            txtNombre.Focus()
        End If
    End Sub

    Private Sub EstablecerModoInicial()
        txtNombre.ReadOnly = True
        txtTelefono.ReadOnly = True
        txtCorreo.ReadOnly = True

        btnAgregar.Enabled = True
        btnModificar.Enabled = True
        btnEliminar.Enabled = True
        btnLimpiar.Enabled = True
        dgvClientes.Enabled = True
        txtBuscar.Enabled = True

        btnGuardar.Enabled = False
        btnCancelar.Enabled = False
    End Sub

    Private Sub LimpiarCampos()
        txtNombre.Clear()
        txtTelefono.Clear()
        txtCorreo.Clear()
    End Sub

    Private Sub MostrarError(mensaje As String)
        MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub MostrarAdvertencia(mensaje As String)
        MessageBox.Show(mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

    Private Sub MostrarExito(mensaje As String)
        MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class