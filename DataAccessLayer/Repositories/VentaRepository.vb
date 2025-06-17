Imports System.Data.SqlClient

Public Class VentaRepository
    Inherits BaseRepository

    ' Obtener ventas por rango de fechas
    Public Function ObtenerPorRangoFechas(fechaInicio As DateTime, fechaFin As DateTime) As List(Of Venta)
        Dim query = "SELECT ID, IDCliente, Fecha, Total, Estado FROM ventas WHERE Estado = 1 AND Fecha BETWEEN  @FechaInicio AND @FechaFin ORDER BY Fecha"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@FechaInicio", fechaInicio),
            New SqlParameter("@FechaFin", fechaFin)
        }

        Dim dt = ExecuteQuery(query, parameters)
        Dim ventas As New List(Of Venta)()

        For Each row As DataRow In dt.Rows
            ventas.Add(New Venta() With {
                .ID = Convert.ToInt32(row("ID")),
                .IDCliente = Convert.ToInt32(row("IDCliente")),
                .Fecha = Convert.ToDateTime(row("Fecha")),
                .Total = Convert.ToDecimal(row("Total")),
                .Estado = row("Estado").ToString()
            })
        Next

        Return ventas
    End Function

    ' Obtener todas las ventas
    Public Function ObtenerTodas() As List(Of Venta)
        Dim query = "SELECT ID, IDCliente, Fecha, Total, Estado FROM ventas ORDER BY Fecha"
        Dim dt = ExecuteQuery(query, Nothing)

        Return (From row As DataRow In dt.Rows
                Select New Venta() With {
                    .ID = Convert.ToInt32(row("ID")),
                    .IDCliente = Convert.ToInt32(row("IDCliente")),
                    .Fecha = Convert.ToDateTime(row("Fecha")),
                    .Total = Convert.ToDecimal(row("Total")),
                    .Estado = row("Estado").ToString()
                }).ToList()
    End Function

    ' Obtener una venta por su ID
    Public Function ObtenerPorId(idVenta As Integer) As Venta
        Dim query = "SELECT ID, IDCliente, Fecha, Total, Estado FROM ventas WHERE ID = @ID"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@ID", idVenta)
        }

        Dim dt = ExecuteQuery(query, parameters)
        If dt.Rows.Count = 0 Then Return Nothing

        Dim row = dt.Rows(0)
        Return New Venta() With {
            .ID = Convert.ToInt32(row("ID")),
            .IDCliente = Convert.ToInt32(row("IDCliente")),
            .Fecha = Convert.ToDateTime(row("Fecha")),
            .Total = Convert.ToDecimal(row("Total")),
            .Estado = row("Estado").ToString()
        }
    End Function

    ' Obtener items de una venta (básico)
    Public Function ObtenerItemsVenta(idVenta As Integer) As List(Of VentaItem)
        Dim query = "SELECT ID, IDProducto, PrecioUnitario, Cantidad, PrecioTotal FROM ventasitems WHERE IDVenta = @IDVenta"
        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@IDVenta", idVenta)
        }
        Dim dt = ExecuteQuery(query, parameters)

        Return (From row As DataRow In dt.Rows
                Select New VentaItem() With {
                    .ID = Convert.ToInt32(row("ID")),
                    .IDProducto = Convert.ToInt32(row("IDProducto")),
                    .PrecioUnitario = Convert.ToDecimal(row("PrecioUnitario")),
                    .Cantidad = Convert.ToInt32(row("Cantidad")),
                    .PrecioTotal = Convert.ToDecimal(row("PrecioTotal"))
                }).ToList()
    End Function

    ' Obtener detalle completo de una venta (con info de producto)
    Public Function ObtenerDetallePorVentaId(idVenta As Integer) As List(Of VentaItem)
        Dim query As String = "
        SELECT vi.ID, vi.IDVenta, vi.IDProducto, vi.PrecioUnitario, vi.Cantidad, vi.PrecioTotal,
               p.Nombre, p.Precio, p.Categoria
        FROM ventasitems vi
        INNER JOIN productos p ON vi.IDProducto = p.ID
        WHERE vi.IDVenta = @idVenta"

        Dim parameters = New List(Of SqlParameter) From {
            New SqlParameter("@IDVenta", idVenta)
        }

        Dim tabla = ExecuteQuery(query, parameters)
        Dim items = New List(Of VentaItem)()

        For Each row As DataRow In tabla.Rows
            items.Add(New VentaItem With {
                .ID = Convert.ToInt32(row("ID")),
                .IDVenta = Convert.ToInt32(row("IDVenta")),
                .IDProducto = Convert.ToInt32(row("IDProducto")),
                .PrecioUnitario = Convert.ToDouble(row("PrecioUnitario")),
                .Cantidad = Convert.ToDouble(row("Cantidad")),
                .PrecioTotal = Convert.ToDouble(row("PrecioTotal")),
                .Producto = New Producto With {
                    .ID = Convert.ToInt32(row("IDProducto")),
                    .Nombre = row("Nombre").ToString(),
                    .Precio = Convert.ToDouble(row("Precio")),
                    .Categoria = row("Categoria").ToString()
                }
            })
        Next

        Return items
    End Function

    ' Guardar nueva venta con items (transacción)
    Public Sub GuardarVenta(venta As Venta, items As List(Of VentaItem))
        Using connection As New SqlConnection(_connectionString)
            connection.Open()
            Dim transaction = connection.BeginTransaction()

            Try
                Dim ventaId = InsertarVenta(connection, transaction, venta)
                InsertarItemsVenta(connection, transaction, ventaId, items)
                ActualizarTotalVenta(connection, transaction, ventaId, venta.Total)

                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw New Exception("Error al guardar la venta: " & ex.Message, ex)
            End Try
        End Using
    End Sub

    ' Actualizar venta existente
    Public Sub ActualizarVenta(venta As Venta, items As List(Of VentaItem))
        Using connection As New SqlConnection(_connectionString)
            connection.Open()
            Dim transaction = connection.BeginTransaction()

            Try
                ' Actualizar cabecera de venta
                ActualizarCabeceraVenta(connection, transaction, venta)

                ' Si hay items, actualizarlos
                If items IsNot Nothing AndAlso items.Count > 0 Then
                    ' Eliminar items antiguos
                    EliminarItemsVenta(connection, transaction, venta.ID)
                    ' Insertar nuevos items
                    InsertarItemsVenta(connection, transaction, venta.ID, items)
                End If

                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw New Exception("Error al actualizar venta: " & ex.Message, ex)
            End Try
        End Using
    End Sub

    ' Métodos privados de ayuda
    Private Function InsertarVenta(connection As SqlConnection, transaction As SqlTransaction, venta As Venta) As Integer
        Dim query = "INSERT INTO Ventas (IDCliente, Fecha, Total, Estado) " &
                    "VALUES (@IDCliente, @Fecha, @Total, @Estado); " &
                    "SELECT CAST(scope_identity() AS int);"

        Using cmd As New SqlCommand(query, connection, transaction)
            cmd.Parameters.AddWithValue("@IDCliente", venta.IDCliente)
            cmd.Parameters.AddWithValue("@Fecha", venta.Fecha)
            cmd.Parameters.AddWithValue("@Total", venta.Total)
            cmd.Parameters.AddWithValue("@Estado", venta.Estado)

            Return Convert.ToInt32(cmd.ExecuteScalar())
        End Using
    End Function

    Private Sub ActualizarCabeceraVenta(connection As SqlConnection, transaction As SqlTransaction, venta As Venta)
        Dim query = "UPDATE Ventas SET IDCliente = @IDCliente, Fecha = @Fecha, Total = @Total, Estado = @Estado WHERE ID = @ID"

        Using cmd As New SqlCommand(query, connection, transaction)
            cmd.Parameters.AddWithValue("@ID", venta.ID)
            cmd.Parameters.AddWithValue("@IDCliente", venta.IDCliente)
            cmd.Parameters.AddWithValue("@Fecha", venta.Fecha)
            cmd.Parameters.AddWithValue("@Total", venta.Total)
            cmd.Parameters.AddWithValue("@Estado", venta.Estado)

            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub EliminarItemsVenta(connection As SqlConnection, transaction As SqlTransaction, ventaId As Integer)
        Dim query = "DELETE FROM VentasItems WHERE IDVenta = @IDVenta"

        Using cmd As New SqlCommand(query, connection, transaction)
            cmd.Parameters.AddWithValue("@IDVenta", ventaId)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub InsertarItemsVenta(connection As SqlConnection, transaction As SqlTransaction, ventaId As Integer, items As List(Of VentaItem))
        Dim query = "INSERT INTO VentasItems (IDVenta, IDProducto, Cantidad, PrecioUnitario, PrecioTotal) " &
                    "VALUES (@IDVenta, @IDProducto, @Cantidad, @PrecioUnitario, @PrecioTotal)"

        For Each item In items
            Using cmd As New SqlCommand(query, connection, transaction)
                cmd.Parameters.AddWithValue("@IDVenta", ventaId)
                cmd.Parameters.AddWithValue("@IDProducto", item.IDProducto)
                cmd.Parameters.AddWithValue("@Cantidad", item.Cantidad)
                cmd.Parameters.AddWithValue("@PrecioUnitario", item.PrecioUnitario)
                cmd.Parameters.AddWithValue("@PrecioTotal", item.PrecioTotal)

                cmd.ExecuteNonQuery()
            End Using
        Next
    End Sub

    Private Sub ActualizarTotalVenta(connection As SqlConnection, transaction As SqlTransaction, ventaId As Integer, total As Decimal)
        Dim query = "UPDATE Ventas SET Total = @Total WHERE ID = @ID"

        Using cmd As New SqlCommand(query, connection, transaction)
            cmd.Parameters.AddWithValue("@ID", ventaId)
            cmd.Parameters.AddWithValue("@Total", total)

            cmd.ExecuteNonQuery()
        End Using
    End Sub
End Class