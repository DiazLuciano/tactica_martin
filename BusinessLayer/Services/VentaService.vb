Public Class VentaService
    Private ReadOnly _ventaRepository As New VentaRepository()
    Private ReadOnly _clienteRepository As New ClienteRepository()
    Private ReadOnly _productoRepository As New ProductoRepository()

    ' Método existente - Obtener ventas por rango de fechas
    Public Function ObtenerVentasPorFecha(fechaInicio As DateTime, fechaFin As DateTime) As List(Of Venta)
        If fechaInicio > fechaFin Then
            Throw New ArgumentException("La fecha de inicio no puede ser mayor que la fecha final")
        End If
        Return _ventaRepository.ObtenerPorRangoFechas(fechaInicio, fechaFin)
    End Function

    ' Método existente - Obtener detalle de venta
    Public Function ObtenerDetallePorVentaId(idVenta As Integer) As List(Of VentaItem)
        If idVenta <= 0 Then
            Throw New ArgumentException("ID de venta no válido")
        End If
        Return _ventaRepository.ObtenerDetallePorVentaId(idVenta)
    End Function

    ' Método existente - Procesar nueva venta
    Public Sub ProcesarVenta(venta As Venta, items As List(Of VentaItem))
        ValidarVenta(venta, items)
        _ventaRepository.GuardarVenta(venta, items)
    End Sub

    ' Método nuevo - Obtener venta por ID
    Public Function ObtenerVentaPorId(idVenta As Integer) As Venta
        If idVenta <= 0 Then
            Throw New ArgumentException("ID de venta no válido")
        End If
        Return _ventaRepository.ObtenerPorId(idVenta)
    End Function

    ' Método nuevo - Crear venta (similar a ProcesarVenta pero con interfaz más clara)
    Public Sub CrearVenta(venta As Venta, items As List(Of VentaItem))
        venta.Fecha = DateTime.Now
        venta.Estado = 1
        ProcesarVenta(venta, items)
    End Sub

    ' Método nuevo - Actualizar venta existente
    Public Sub ActualizarVenta(venta As Venta, items As List(Of VentaItem))
        If venta.ID <= 0 Then
            Throw New ArgumentException("Venta no válida para actualización")
        End If

        ' Verificar que la venta existe
        If _ventaRepository.ObtenerPorId(venta.ID) Is Nothing Then
            Throw New ArgumentException("La venta no existe")
        End If

        ValidarVenta(venta, items)
        _ventaRepository.ActualizarVenta(venta, items)
    End Sub

    ' Método nuevo - Anular venta (baja lógica)
    Public Sub AnularVenta(idVenta As Integer)
        Dim venta = _ventaRepository.ObtenerPorId(idVenta)
        If venta Is Nothing Then
            Throw New ArgumentException("Venta no encontrada")
        End If

        If venta.Estado = 0 Then
            Throw New InvalidOperationException("La venta ya está anulada")
        End If

        venta.Estado = 0 ' Inactivo
        _ventaRepository.ActualizarVenta(venta, Nothing)
    End Sub

    ' Método existente - Obtener cliente por ID
    Public Function ObtenerClientePorId(idCliente As Integer) As Cliente
        Return _clienteRepository.ObtenerPorId(idCliente)
    End Function

    ' Método nuevo - Validaciones comunes para ventas
    Private Sub ValidarVenta(venta As Venta, items As List(Of VentaItem))
        If venta Is Nothing Then
            Throw New ArgumentException("La venta no puede ser nula")
        End If

        If items Is Nothing OrElse items.Count = 0 Then
            Throw New ArgumentException("La venta debe contener al menos un producto")
        End If

        ' Validar cliente
        If _clienteRepository.ObtenerPorId(venta.IDCliente) Is Nothing Then
            Throw New ArgumentException("Cliente no encontrado")
        End If

        ' Validar items
        For Each item In items
            Dim producto = _productoRepository.ObtenerPorId(item.IDProducto)
            If producto Is Nothing Then
                Throw New ArgumentException($"Producto con ID {item.IDProducto} no encontrado")
            End If

            If item.Cantidad <= 0 Then
                Throw New ArgumentException($"La cantidad para el producto {producto.Nombre} debe ser mayor que cero")
            End If

            item.PrecioUnitario = producto.Precio
            item.PrecioTotal = item.PrecioUnitario * item.Cantidad
        Next

        ' Calcular total
        venta.Total = items.Sum(Function(i) i.PrecioTotal)
    End Sub
End Class