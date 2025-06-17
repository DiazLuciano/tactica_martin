Public Class frmProductos
    Private ReadOnly _productoService As New ProductoService()
    Private _productosActuales As List(Of Producto)
    Private _productoSeleccionado As Producto = Nothing

    ' Controles
    Private WithEvents dgvProductos As DataGridView
    Private WithEvents txtNombre As TextBox
    Private WithEvents txtPrecio As TextBox
    Private WithEvents txtCategoria As TextBox
    Private WithEvents txtBuscar As TextBox
    Private WithEvents btnAgregar As Button
    Private WithEvents btnModificar As Button
    Private WithEvents btnEliminar As Button
    Private WithEvents btnLimpiar As Button
    Private WithEvents btnGuardar As Button
    Private WithEvents btnCancelar As Button

    Private Sub frmProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarControles()
        CargarProductos()
        ConfigurarDataGridView()
    End Sub

    Private Sub ConfigurarControles()
        Me.Text = "Gestión de Productos"
        Me.ClientSize = New Size(650, 450)

        ' Configurar DataGridView (mismo tamaño y posición que en frmClientes)
        dgvProductos = New DataGridView()
        With dgvProductos
            .Location = New Point(20, 20)
            .Size = New Size(610, 200)
            .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        End With
        Me.Controls.Add(dgvProductos)

        ' Configurar etiquetas y campos de texto (misma disposición que en frmClientes)
        Me.Controls.Add(New Label With {.Text = "Nombre:", .Location = New Point(20, 240), .Size = New Size(80, 20)})
        txtNombre = New TextBox() With {.Location = New Point(110, 240), .Size = New Size(300, 20)}
        Me.Controls.Add(txtNombre)

        Me.Controls.Add(New Label With {.Text = "Precio:", .Location = New Point(20, 280), .Size = New Size(80, 20)})
        txtPrecio = New TextBox() With {.Location = New Point(110, 280), .Size = New Size(300, 20)}
        Me.Controls.Add(txtPrecio)

        Me.Controls.Add(New Label With {.Text = "Categoría:", .Location = New Point(20, 320), .Size = New Size(80, 20)})
        txtCategoria = New TextBox() With {.Location = New Point(110, 320), .Size = New Size(300, 20)}
        Me.Controls.Add(txtCategoria)

        ' Campo de búsqueda (misma posición que en frmClientes)
        Me.Controls.Add(New Label With {.Text = "Buscar:", .Location = New Point(20, 360), .Size = New Size(80, 20)})
        txtBuscar = New TextBox() With {.Location = New Point(110, 360), .Size = New Size(300, 20)}
        Me.Controls.Add(txtBuscar)

        ' Botones inferiores (mismo tamaño y posición que en frmClientes)
        btnGuardar = New Button() With {
        .Text = "Guardar",
        .Location = New Point(110, 400),
        .Size = New Size(150, 30),
        .Enabled = False
    }
        Me.Controls.Add(btnGuardar)

        btnCancelar = New Button() With {
        .Text = "Cancelar",
        .Location = New Point(270, 400),
        .Size = New Size(150, 30),
        .Enabled = False
    }
        Me.Controls.Add(btnCancelar)

        ' Botones laterales (mismo tamaño y posición que en frmClientes)
        btnAgregar = New Button() With {
        .Text = "Agregar",
        .Location = New Point(430, 240),
        .Size = New Size(150, 30)
    }
        Me.Controls.Add(btnAgregar)

        btnModificar = New Button() With {
        .Text = "Modificar",
        .Location = New Point(430, 280),
        .Size = New Size(150, 30)
    }
        Me.Controls.Add(btnModificar)

        btnEliminar = New Button() With {
        .Text = "Eliminar",
        .Location = New Point(430, 320),
        .Size = New Size(150, 30)
    }
        Me.Controls.Add(btnEliminar)

        btnLimpiar = New Button() With {
        .Text = "Limpiar",
        .Location = New Point(430, 360),
        .Size = New Size(150, 30)
    }
        Me.Controls.Add(btnLimpiar)
    End Sub
    Private Sub ConfigurarDataGridView()
        dgvProductos.AutoGenerateColumns = False
        dgvProductos.AllowUserToAddRows = False
        dgvProductos.AllowUserToDeleteRows = False
        dgvProductos.ReadOnly = True
        dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvProductos.MultiSelect = False

        dgvProductos.Columns.Clear()
        dgvProductos.Columns.Add("ID", "ID")
        dgvProductos.Columns.Add("Nombre", "Nombre")
        dgvProductos.Columns.Add("Precio", "Precio")
        dgvProductos.Columns.Add("Categoria", "Categoria")

        dgvProductos.Columns("ID").DataPropertyName = "ID"
        dgvProductos.Columns("Nombre").DataPropertyName = "Nombre"
        dgvProductos.Columns("Precio").DataPropertyName = "Precio"
        dgvProductos.Columns("Categoria").DataPropertyName = "Categoria"
    End Sub
    Private Sub CargarProductos()
        Try
            _productosActuales = _productoService.ObtenerTodos()
            ActualizarDataGridView()
        Catch ex As Exception
            MostrarError("Error al cargar productos: " & ex.Message)
        End Try
    End Sub

    Private Sub ActualizarDataGridView()
        dgvProductos.DataSource = Nothing
        dgvProductos.DataSource = _productosActuales
        dgvProductos.ClearSelection()
        _productoSeleccionado = Nothing
    End Sub

    Private Sub dgvProductos_SelectionChanged(sender As Object, e As EventArgs) Handles dgvProductos.SelectionChanged
        If dgvProductos.SelectedRows.Count > 0 Then
            _productoSeleccionado = DirectCast(dgvProductos.SelectedRows(0).DataBoundItem, Producto)
            MostrarDatosProducto(_productoSeleccionado)
        Else
            _productoSeleccionado = Nothing
            LimpiarCampos()
        End If
    End Sub

    Private Sub MostrarDatosProducto(producto As Producto)
        txtNombre.Text = producto.Nombre
        txtPrecio.Text = producto.Precio.ToString()
        txtCategoria.Text = producto.Categoria
    End Sub

    Private Sub txtBuscar_TextChanged(sender As Object, e As EventArgs) Handles txtBuscar.TextChanged
        Try
            If String.IsNullOrWhiteSpace(txtBuscar.Text) Then
                CargarProductos()
            Else
                _productosActuales = _productoService.BuscarProductos(txtBuscar.Text.Trim())
                ActualizarDataGridView()
            End If
        Catch ex As Exception
            MostrarError("Error al buscar productos: " & ex.Message)
        End Try
    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        EstablecerModoEdicion(True)
    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        If _productoSeleccionado Is Nothing Then
            MostrarAdvertencia("Seleccione un producto para modificar")
            Return
        End If
        EstablecerModoEdicion(False)
    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        If _productoSeleccionado Is Nothing Then
            MostrarAdvertencia("Seleccione un producto para eliminar")
            Return
        End If
        If MessageBox.Show("¿Está seguro que desea eliminar este producto?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then Return

        Try
            _productoService.EliminarProducto(_productoSeleccionado.ID)
            MostrarExito("Producto eliminado correctamente")
            CargarProductos()
        Catch ex As Exception
            MostrarError("Error al eliminar producto: " & ex.Message)
        End Try
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If Not ValidarCampos() Then Return

        Dim producto As Producto
        If _productoSeleccionado Is Nothing Then
            producto = New Producto With {
                .Nombre = txtNombre.Text.Trim(),
                .Precio = Convert.ToDecimal(txtPrecio.Text),
                .Categoria = txtCategoria.Text.Trim()
            }
        Else
            producto = _productoSeleccionado
            producto.Nombre = txtNombre.Text.Trim()
            producto.Precio = Convert.ToDecimal(txtPrecio.Text)
            producto.Categoria = txtCategoria.Text.Trim()
        End If

        Try
            If _productoSeleccionado Is Nothing Then
                _productoService.RegistrarProducto(producto)
                MostrarExito("Producto registrado correctamente")
            Else
                _productoService.ActualizarProducto(producto)
                MostrarExito("Producto actualizado correctamente")
            End If
            CargarProductos()
            EstablecerModoInicial()
        Catch ex As Exception
            MostrarError(ex.Message)
        End Try
    End Sub

    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        EstablecerModoInicial()
        If _productoSeleccionado IsNot Nothing Then
            MostrarDatosProducto(_productoSeleccionado)
        Else
            LimpiarCampos()
        End If
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        LimpiarCampos()
        dgvProductos.ClearSelection()
        _productoSeleccionado = Nothing
    End Sub

    Private Function ValidarCampos() As Boolean
        If String.IsNullOrWhiteSpace(txtNombre.Text) Then
            MostrarAdvertencia("El nombre del producto es obligatorio")
            txtNombre.Focus()
            Return False
        End If
        If Not Decimal.TryParse(txtPrecio.Text, Nothing) Then
            MostrarAdvertencia("Ingrese un precio válido")
            txtPrecio.Focus()
            Return False
        End If
        Return True
    End Function

    Private Sub EstablecerModoEdicion(esNuevo As Boolean)
        txtNombre.ReadOnly = False
        txtPrecio.ReadOnly = False
        txtCategoria.ReadOnly = False

        btnAgregar.Enabled = False
        btnModificar.Enabled = False
        btnEliminar.Enabled = False
        btnLimpiar.Enabled = False
        dgvProductos.Enabled = False
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
        txtPrecio.ReadOnly = True
        txtCategoria.ReadOnly = True

        btnAgregar.Enabled = True
        btnModificar.Enabled = True
        btnEliminar.Enabled = True
        btnLimpiar.Enabled = True
        dgvProductos.Enabled = True
        txtBuscar.Enabled = True

        btnGuardar.Enabled = False
        btnCancelar.Enabled = False
    End Sub

    Private Sub LimpiarCampos()
        txtNombre.Clear()
        txtPrecio.Clear()
        txtCategoria.Clear()
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
