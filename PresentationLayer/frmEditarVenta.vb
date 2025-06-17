Public Class frmEditarVenta
    Private ReadOnly _ventaService As New VentaService()
    Private ReadOnly _clienteService As New ClienteService()
    Private ReadOnly _productoService As New ProductoService()

    Private _venta As Venta
    Private _items As New List(Of VentaItem)()
    Private _esNueva As Boolean = True

    ' Controles
    Private WithEvents cboClientes As ComboBox
    Private WithEvents cboProductos As ComboBox
    Private WithEvents txtCantidad As TextBox
    Private WithEvents btnAgregarItem As Button
    Private WithEvents btnQuitarItem As Button
    Private WithEvents dgvItems As DataGridView
    Private WithEvents btnGuardar As Button
    Private WithEvents btnCancelar As Button
    Private WithEvents lblTotal As Label
    Private WithEvents dtpFecha As DateTimePicker

    ' Constructor para nueva venta
    Public Sub New()
        InitializeComponent()
        _venta = New Venta() With {
            .Fecha = DateTime.Now,
            .Estado = 1
        }
        ConfigurarFormulario()
    End Sub

    ' Constructor para edición
    Public Sub New(idVenta As Integer)
        InitializeComponent()
        _venta = _ventaService.ObtenerVentaPorId(idVenta)
        _items = _ventaService.ObtenerDetallePorVentaId(idVenta)
        _esNueva = False
        ConfigurarFormulario()
    End Sub

    Private Sub ConfigurarFormulario()
        Me.Text = If(_esNueva, "Nueva Venta", "Modificar Venta")
        Me.Size = New Size(800, 600)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        ' Configurar controles
        ConfigurarControles()
        CargarCombos()
        ActualizarTotal()
    End Sub

    Private Sub ConfigurarControles()
        ' Etiqueta y combo de clientes
        Dim lblCliente As New Label With {
            .Text = "Cliente:",
            .Location = New Point(20, 20),
            .Size = New Size(60, 20)
        }
        Me.Controls.Add(lblCliente)

        cboClientes = New ComboBox With {
            .Location = New Point(80, 20),
            .Width = 300,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }
        Me.Controls.Add(cboClientes)

        ' Etiqueta y fecha
        Dim lblFecha As New Label With {
            .Text = "Fecha:",
            .Location = New Point(400, 20),
            .Size = New Size(60, 20)
        }
        Me.Controls.Add(lblFecha)

        dtpFecha = New DateTimePicker With {
            .Location = New Point(460, 20),
            .Width = 120,
            .Format = DateTimePickerFormat.Short,
            .Value = _venta.Fecha
        }
        Me.Controls.Add(dtpFecha)

        ' Sección para agregar items
        Dim lblAgregarProducto As New Label With {
            .Text = "Agregar Producto:",
            .Location = New Point(20, 60),
            .Size = New Size(120, 20)
        }
        Me.Controls.Add(lblAgregarProducto)

        cboProductos = New ComboBox With {
            .Location = New Point(140, 60),
            .Width = 250,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }
        Me.Controls.Add(cboProductos)

        Dim lblCantidad As New Label With {
            .Text = "Cantidad:",
            .Location = New Point(400, 60),
            .Size = New Size(60, 20)
        }
        Me.Controls.Add(lblCantidad)

        txtCantidad = New TextBox With {
            .Location = New Point(460, 60),
            .Width = 80,
            .Text = "1"
        }
        Me.Controls.Add(txtCantidad)

        btnAgregarItem = New Button With {
            .Text = "Agregar",
            .Location = New Point(550, 60),
            .Size = New Size(80, 23)
        }
        Me.Controls.Add(btnAgregarItem)

        btnQuitarItem = New Button With {
            .Text = "Quitar",
            .Location = New Point(640, 60),
            .Size = New Size(80, 23),
            .Enabled = False
        }
        Me.Controls.Add(btnQuitarItem)

        ' Grid de items
        dgvItems = New DataGridView With {
            .Location = New Point(20, 100),
            .Size = New Size(740, 300),
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .MultiSelect = False
        }
        dgvItems.Columns.Add("Producto", "Producto")
        dgvItems.Columns.Add("Cantidad", "Cantidad")
        dgvItems.Columns.Add("PrecioUnitario", "Precio Unitario")
        dgvItems.Columns.Add("Total", "Total")
        Me.Controls.Add(dgvItems)

        ' Total
        lblTotal = New Label With {
            .Text = "Total: $0.00",
            .Location = New Point(600, 420),
            .Size = New Size(160, 25),
            .Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleRight
        }
        Me.Controls.Add(lblTotal)

        ' Botones de acción
        btnGuardar = New Button With {
            .Text = "Guardar",
            .Location = New Point(520, 460),
            .Size = New Size(120, 35),
            .DialogResult = DialogResult.OK
        }
        Me.Controls.Add(btnGuardar)

        btnCancelar = New Button With {
            .Text = "Cancelar",
            .Location = New Point(650, 460),
            .Size = New Size(120, 35),
            .DialogResult = DialogResult.Cancel
        }
        Me.Controls.Add(btnCancelar)
    End Sub

    Private Sub CargarCombos()
        ' Cargar clientes
        cboClientes.DataSource = _clienteService.ObtenerTodosClientes()
        cboClientes.DisplayMember = "Nombre"
        cboClientes.ValueMember = "ID"

        If Not _esNueva Then
            cboClientes.SelectedValue = _venta.IDCliente
        End If

        ' Cargar productos
        cboProductos.DataSource = _productoService.ObtenerTodos()
        cboProductos.DisplayMember = "Nombre"
        cboProductos.ValueMember = "ID"

        ' Cargar items existentes si es edición
        If Not _esNueva Then
            ActualizarGridItems()
        End If
    End Sub

    Private Sub ActualizarGridItems()
        dgvItems.Rows.Clear()
        For Each item In _items
            dgvItems.Rows.Add(
                item.Producto.Nombre,
                item.Cantidad.ToString("N2"),
                item.PrecioUnitario.ToString("C2"),
                item.PrecioTotal.ToString("C2")
            )
        Next
        ActualizarTotal()
    End Sub

    Private Sub ActualizarTotal()
        Dim total = _items.Sum(Function(i) i.PrecioTotal)
        lblTotal.Text = $"Total: {total.ToString("C2")}"
        _venta.Total = total
    End Sub

    Private Sub btnAgregarItem_Click(sender As Object, e As EventArgs) Handles btnAgregarItem.Click
        Try
            Dim producto = DirectCast(cboProductos.SelectedItem, Producto)
            Dim cantidad = Decimal.Parse(txtCantidad.Text)

            If cantidad <= 0 Then
                MessageBox.Show("La cantidad debe ser mayor que cero", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Verificar si el producto ya está en la lista
            Dim itemExistente = _items.FirstOrDefault(Function(i) i.IDProducto = producto.ID)
            If itemExistente IsNot Nothing Then
                itemExistente.Cantidad += cantidad
                itemExistente.PrecioTotal = itemExistente.PrecioUnitario * itemExistente.Cantidad
            Else
                _items.Add(New VentaItem With {
                    .IDProducto = producto.ID,
                    .Producto = producto,
                    .Cantidad = cantidad,
                    .PrecioUnitario = producto.Precio,
                    .PrecioTotal = producto.Precio * cantidad
                })
            End If

            ActualizarGridItems()
            txtCantidad.Text = "1"
        Catch ex As Exception
            MessageBox.Show("Error al agregar producto: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnQuitarItem_Click(sender As Object, e As EventArgs) Handles btnQuitarItem.Click
        If dgvItems.SelectedRows.Count > 0 Then
            Dim productoNombre = dgvItems.SelectedRows(0).Cells("Producto").Value.ToString()
            Dim item = _items.FirstOrDefault(Function(i) i.Producto.Nombre = productoNombre)

            If item IsNot Nothing Then
                _items.Remove(item)
                ActualizarGridItems()
            End If
        End If
    End Sub

    Private Sub dgvItems_SelectionChanged(sender As Object, e As EventArgs) Handles dgvItems.SelectionChanged
        btnQuitarItem.Enabled = dgvItems.SelectedRows.Count > 0
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Try
            ' Validaciones básicas
            If cboClientes.SelectedItem Is Nothing Then
                MessageBox.Show("Debe seleccionar un cliente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If _items.Count = 0 Then
                MessageBox.Show("Debe agregar al menos un producto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Asignar valores a la venta
            _venta.IDCliente = DirectCast(cboClientes.SelectedItem, Cliente).ID
            _venta.Fecha = dtpFecha.Value
            _venta.Total = _items.Sum(Function(i) i.PrecioTotal)

            ' Guardar según corresponda
            If _esNueva Then
                _ventaService.CrearVenta(_venta, _items)
            Else
                _ventaService.ActualizarVenta(_venta, _items)
            End If

            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class