Imports System.Data.SqlClient
Imports System.Configuration

Public Class BaseRepository
    Protected ReadOnly _connectionString As String

    Public Sub New()
        _connectionString = ConfigurationManager.ConnectionStrings("Tactica_DB_CS").ConnectionString
    End Sub

    ' Método para ejecutar consultas y retornar un DataTable
    Protected Function ExecuteQuery(query As String, parameters As List(Of SqlParameter)) As DataTable
        Dim dt As New DataTable()
        Using connection As New SqlConnection(_connectionString)
            Using command As New SqlCommand(query, connection)
                If parameters IsNot Nothing Then
                    command.Parameters.AddRange(parameters.ToArray())
                End If
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    dt.Load(reader)
                End Using
            End Using
        End Using
        Return dt
    End Function

    ' Método para ejecutar INSERT/UPDATE/DELETE
    Protected Function ExecuteNonQuery(query As String, parameters As List(Of SqlParameter)) As Integer
        Using connection As New SqlConnection(_connectionString)
            Using command As New SqlCommand(query, connection)
                If parameters IsNot Nothing Then
                    command.Parameters.AddRange(parameters.ToArray())
                End If
                connection.Open()
                Return command.ExecuteNonQuery()
            End Using
        End Using
    End Function
End Class