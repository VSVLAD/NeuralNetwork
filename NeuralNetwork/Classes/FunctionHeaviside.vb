Option Explicit On
Option Strict On

Namespace NeuralProject

    ''' <summary>Ступенчатая функция Heaviside</summary>
    Public Class FunctionHeaviside
        Implements IFunctionActivator

        Public ReadOnly Property Name As String = "HEAVISIDE" Implements IFunctionActivator.Name

        Public Function Activate(Value As Double) As Double Implements IFunctionActivator.Activate
            Return If(Value >= 0, 1, 0)
        End Function

        Public Function Deriviate(Value As Double) As Double Implements IFunctionActivator.Deriviate
            Return If(Value <> 0, 0, -1)
        End Function

    End Class

End Namespace