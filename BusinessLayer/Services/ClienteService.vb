Public Class ClienteService
    Private _repository As New ClienteRepository()

    ' Método nuevo para obtener un cliente por su ID
    Public Function ObtenerPorId(idCliente As Integer) As Cliente
        ' Validación básica
        If idCliente <= 0 Then
            Throw New ArgumentException("ID de cliente no válido")
        End If

        ' Usamos el repositorio para obtener el cliente
        Dim cliente = _repository.ObtenerPorId(idCliente)

        If cliente Is Nothing Then
            Throw New KeyNotFoundException($"No se encontró el cliente con ID {idCliente}")
        End If

        Return cliente
    End Function

    ' Registrar un nuevo cliente con validaciones (método existente)
    Public Sub RegistrarCliente(cliente As Cliente)
        ' Validaciones de negocio
        If String.IsNullOrWhiteSpace(cliente.Nombre) Then
            Throw New ArgumentException("El nombre del cliente es obligatorio")
        End If

        If cliente.Nombre.Length > 255 Then
            Throw New ArgumentException("El nombre no puede exceder los 255 caracteres")
        End If

        If Not String.IsNullOrEmpty(cliente.Correo) AndAlso Not cliente.Correo.Contains("@") Then
            Throw New ArgumentException("El correo electrónico no es válido")
        End If

        ' Verificar si el cliente ya existe
        Dim clientesExistentes = _repository.ObtenerPorNombre(cliente.Nombre)
        If clientesExistentes.Any() Then
            Throw New InvalidOperationException("Ya existe un cliente con ese nombre")
        End If

        ' Si pasa todas las validaciones, guardar
        _repository.Insertar(cliente)
    End Sub

    ' Actualizar cliente (método existente)
    Public Sub ActualizarCliente(cliente As Cliente)
        ' Validaciones similares a RegistrarCliente
        ' ...
        _repository.Actualizar(cliente)
    End Sub

    ' Eliminar cliente (método existente)
    Public Sub EliminarCliente(id As Integer)
        ' Validar si el cliente tiene ventas asociadas
        ' (Podrías necesitar un método en VentaRepository para verificar esto)
        _repository.Eliminar(id)
    End Sub

    ' Buscar clientes (método existente)
    Public Function BuscarClientes(filtro As String) As List(Of Cliente)
        Return _repository.ObtenerPorNombre(filtro)
    End Function

    ' Obtener todos los clientes (método existente)
    Public Function ObtenerTodosClientes() As List(Of Cliente)
        Return _repository.ObtenerTodos()
    End Function
End Class