Public Class frmVentas
    Private ReadOnly _ventaService As New VentaService()
    Private ReadOnly _clienteService As New ClienteService() ' Asumo que tienes este servicio
    Private _ventasActuales As List(Of Venta)
    Private _ventaSeleccionada As Venta = Nothing

    ' Controles
    Private WithEvents dgvVentas As DataGridView
    Private WithEvents dgvDetalleVenta As DataGridView
    Private WithEvents txtBuscarCliente As TextBox
    Private WithEvents dtpFechaDesde As DateTimePicker
    Private WithEvents dtpFechaHasta As DateTimePicker
    Private WithEvents btnBuscar As Button
    Private WithEvents lblTotalGeneral As Label

    Private WithEvents btnNuevaVenta As Button
    Private WithEvents btnModificarVenta As Button
    Private WithEvents btnAnularVenta As Button

    Private Sub frmVentas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarControles()
        ConfigurarGrids()
        CargarVentasIniciales()
    End Sub

    Private Sub ConfigurarControles()
        Me.Text = "Buscador de Ventas"
        Me.ClientSize = New Size(850, 600)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Font = New Font("Segoe UI", 9)

        ' Filtros
        Me.Controls.Add(New Label With {.Text = "Cliente:", .Location = New Point(20, 20), .Size = New Size(50, 20)})
        txtBuscarCliente = New TextBox() With {.Location = New Point(80, 20), .Size = New Size(200, 20)}
        Me.Controls.Add(txtBuscarCliente)

        Me.Controls.Add(New Label With {.Text = "Desde:", .Location = New Point(300, 20), .Size = New Size(50, 20)})
        dtpFechaDesde = New DateTimePicker() With {.Location = New Point(360, 20), .Size = New Size(120, 20), .Format = DateTimePickerFormat.Short}
        Me.Controls.Add(dtpFechaDesde)

        Me.Controls.Add(New Label With {.Text = "Hasta:", .Location = New Point(500, 20), .Size = New Size(50, 20)})
        dtpFechaHasta = New DateTimePicker() With {.Location = New Point(560, 20), .Size = New Size(120, 20), .Format = DateTimePickerFormat.Short}
        Me.Controls.Add(dtpFechaHasta)

        btnBuscar = New Button() With {.Text = "Buscar", .Location = New Point(700, 20), .Size = New Size(100, 23)}
        Me.Controls.Add(btnBuscar)

        ' Grillas
        dgvVentas = New DataGridView() With {
            .Location = New Point(20, 60),
            .Size = New Size(810, 200),
            .Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right
        }
        Me.Controls.Add(dgvVentas)

        dgvDetalleVenta = New DataGridView() With {
            .Location = New Point(20, 280),
            .Size = New Size(810, 200),
            .Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right
        }
        Me.Controls.Add(dgvDetalleVenta)

        ' Total
        lblTotalGeneral = New Label() With {
            .Text = "Total General: $0.00",
            .Location = New Point(20, 500),
            .Size = New Size(300, 20),
            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
        }
        Me.Controls.Add(lblTotalGeneral)

        ' Configurar fechas por defecto
        dtpFechaDesde.Value = Date.Today.AddDays(-30)
        dtpFechaHasta.Value = Date.Today

        ' Agregamos estos nuevos botones debajo del lblTotalGeneral
        btnNuevaVenta = New Button() With {
            .Text = "Nueva Venta",
            .Location = New Point(350, 500),
            .Size = New Size(120, 30)
        }
        Me.Controls.Add(btnNuevaVenta)

        btnModificarVenta = New Button() With {
            .Text = "Modificar",
            .Location = New Point(480, 500),
            .Size = New Size(120, 30),
            .Enabled = False
        }
        Me.Controls.Add(btnModificarVenta)

        btnAnularVenta = New Button() With {
            .Text = "Anular",
            .Location = New Point(610, 500),
            .Size = New Size(120, 30),
            .Enabled = False
        }
        Me.Controls.Add(btnAnularVenta)
    End Sub

    Private Sub ConfigurarGrids()
        ' Configurar dgvVentas
        dgvVentas.AutoGenerateColumns = False
        dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvVentas.Columns.Add("ID", "ID")
        dgvVentas.Columns.Add("Fecha", "Fecha")
        dgvVentas.Columns.Add("Cliente", "Cliente")
        dgvVentas.Columns.Add("Total", "Total")

        ' Configurar dgvDetalleVenta
        dgvDetalleVenta.AutoGenerateColumns = False
        dgvDetalleVenta.Columns.Add("Producto", "Producto")
        dgvDetalleVenta.Columns.Add("Cantidad", "Cantidad")
        dgvDetalleVenta.Columns.Add("PrecioUnitario", "Precio Unitario")
        dgvDetalleVenta.Columns.Add("PrecioTotal", "Total")
    End Sub

    Private Sub CargarVentasIniciales()
        Try
            Dim fechaInicio = dtpFechaDesde.Value.Date
            Dim fechaFin = dtpFechaHasta.Value.Date.AddDays(1).AddTicks(-1)

            _ventasActuales = _ventaService.ObtenerVentasPorFecha(fechaInicio, fechaFin)
            ActualizarGridVentas()
        Catch ex As Exception
            MostrarError("Error al cargar ventas: " & ex.Message)
        End Try
    End Sub

    Private Sub ActualizarGridVentas()
        dgvVentas.Rows.Clear()

        For Each venta In _ventasActuales
            Dim cliente = _clienteService.ObtenerPorId(venta.IDCliente)
            Dim nombreCliente = If(cliente IsNot Nothing, cliente.Nombre, "Cliente no encontrado")

            dgvVentas.Rows.Add(venta.ID, venta.Fecha.ToShortDateString(), nombreCliente, venta.Total.ToString("C2"))
        Next
    End Sub

    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        Try
            Dim fechaInicio = dtpFechaDesde.Value.Date
            Dim fechaFin = dtpFechaHasta.Value.Date.AddDays(1).AddTicks(-1)

            ' Primero filtramos por fecha (usando el método existente)
            _ventasActuales = _ventaService.ObtenerVentasPorFecha(fechaInicio, fechaFin)

            ' Luego filtramos por cliente localmente
            If Not String.IsNullOrWhiteSpace(txtBuscarCliente.Text) Then
                Dim filtro = txtBuscarCliente.Text.ToLower()
                _ventasActuales = _ventasActuales.Where(Function(v)
                                                            Dim cliente = _clienteService.ObtenerPorId(v.IDCliente)
                                                            Return cliente IsNot Nothing AndAlso cliente.Nombre.ToLower().Contains(filtro)
                                                        End Function).ToList()
            End If

            ActualizarGridVentas()

            If _ventasActuales.Count = 0 Then
                dgvDetalleVenta.Rows.Clear()
                lblTotalGeneral.Text = "Total General: $0.00"
                MessageBox.Show("No se encontraron ventas con los criterios especificados", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MostrarError("Error al buscar ventas: " & ex.Message)
        End Try
    End Sub

    Private Sub dgvVentas_SelectionChanged(sender As Object, e As EventArgs) Handles dgvVentas.SelectionChanged
        If dgvVentas.SelectedRows.Count > 0 Then
            Dim idVenta = Convert.ToInt32(dgvVentas.SelectedRows(0).Cells("ID").Value)
            _ventaSeleccionada = _ventasActuales.FirstOrDefault(Function(v) v.ID = idVenta)

            If _ventaSeleccionada IsNot Nothing Then
                MostrarDetalleVenta(_ventaSeleccionada)
                ' Habilitamos botones de modificar y anular cuando hay selección
                btnModificarVenta.Enabled = True
                btnAnularVenta.Enabled = True
            End If
        Else
            ' Deshabilitamos botones cuando no hay selección
            btnModificarVenta.Enabled = False
            btnAnularVenta.Enabled = False
        End If
    End Sub

    Private Sub btnNuevaVenta_Click(sender As Object, e As EventArgs) Handles btnNuevaVenta.Click
        Dim frmNuevaVenta As New frmEditarVenta()
        If frmNuevaVenta.ShowDialog() = DialogResult.OK Then
            ' Refrescar la lista de ventas después de crear una nueva
            CargarVentasIniciales()
        End If
    End Sub

    Private Sub btnModificarVenta_Click(sender As Object, e As EventArgs) Handles btnModificarVenta.Click
        If _ventaSeleccionada IsNot Nothing Then
            Dim frmEditarVenta As New frmEditarVenta(_ventaSeleccionada.ID)
            If frmEditarVenta.ShowDialog() = DialogResult.OK Then
                ' Refrescar la lista de ventas después de modificar
                CargarVentasIniciales()
            End If
        End If
    End Sub

    Private Sub btnAnularVenta_Click(sender As Object, e As EventArgs) Handles btnAnularVenta.Click
        If _ventaSeleccionada IsNot Nothing Then
            If MessageBox.Show($"¿Está seguro que desea anular la venta #{_ventaSeleccionada.ID}?",
                             "Confirmar Anulación",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) = DialogResult.Yes Then

                Try
                    _ventaService.AnularVenta(_ventaSeleccionada.ID)
                    MessageBox.Show("Venta anulada correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    CargarVentasIniciales()
                Catch ex As Exception
                    MostrarError("Error al anular venta: " & ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Sub MostrarDetalleVenta(venta As Venta)
        dgvDetalleVenta.Rows.Clear()
        Dim totalGeneral As Decimal = 0

        Try
            ' Usamos el método existente de tu repositorio
            Dim items = _ventaService.ObtenerDetallePorVentaId(venta.ID)

            For Each item In items
                dgvDetalleVenta.Rows.Add(
                    item.Producto.Nombre,
                    item.Cantidad.ToString(),
                    item.PrecioUnitario.ToString("C2"),
                    item.PrecioTotal.ToString("C2")
                )
                totalGeneral += item.PrecioTotal
            Next
        Catch ex As Exception
            MostrarError("Error al cargar detalles: " & ex.Message)
        End Try

        lblTotalGeneral.Text = $"Total General: {totalGeneral.ToString("C2")}"
    End Sub

    Private Sub MostrarError(mensaje As String)
        MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub
End Class