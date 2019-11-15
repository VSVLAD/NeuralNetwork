Option Strict On

Imports System.Data.SQLite
Imports System.Runtime.CompilerServices

<DebuggerNonUserCode>
Public Module SQLiteExtensions

    <Extension>
    Public Iterator Function SelectRows(ConnectionObject As IDbConnection, SQL As String) As IEnumerable(Of IDictionary(Of String, Object))
        Using cmd As IDbCommand = ConnectionObject.CreateCommand()
            cmd.CommandType = CommandType.Text
            cmd.CommandText = SQL

            Using reader As IDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim line As New Dictionary(Of String, Object)

                    For I = 0 To reader.FieldCount - 1
                        If reader.GetValue(I) IsNot DBNull.Value Then
                            line.Add(reader.GetName(I), reader.GetValue(I))
                        Else
                            line.Add(reader.GetName(I), Nothing)
                        End If
                    Next

                    Yield line
                End While
            End Using
        End Using
    End Function

    <Extension>
    Public Function SelectRow(ConnectionObject As IDbConnection, SQL As String) As IDictionary(Of String, Object)
        Dim line As New Dictionary(Of String, Object)

        Using cmd As IDbCommand = ConnectionObject.CreateCommand()
            cmd.CommandType = CommandType.Text
            cmd.CommandText = SQL

            Using reader As IDataReader = cmd.ExecuteReader()
                While reader.Read()
                    For I = 0 To reader.FieldCount - 1
                        If reader.GetValue(I) IsNot DBNull.Value Then
                            line.Add(reader.GetName(I), reader.GetValue(I))
                        Else
                            line.Add(reader.GetName(I), Nothing)
                        End If
                    Next

                    Return line
                End While
            End Using
        End Using

        Return line
    End Function

    <Extension>
    Public Function SelectCell(ConnectionObject As IDbConnection, SQL As String) As Object
        Using cmd As IDbCommand = ConnectionObject.CreateCommand()
            cmd.CommandType = CommandType.Text
            cmd.CommandText = SQL
            Return cmd.ExecuteScalar()
        End Using
    End Function

    <Extension>
    Public Function ExecNonQuery(ConnectionObject As IDbConnection, SQL As String) As Integer
        Using cmd As IDbCommand = ConnectionObject.CreateCommand()
            cmd.CommandType = CommandType.Text
            cmd.CommandText = SQL
            Return cmd.ExecuteNonQuery()
        End Using
    End Function

End Module
