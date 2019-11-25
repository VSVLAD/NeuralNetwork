Option Explicit On
Option Strict On

Imports NeuralProject.Interfaces


Namespace NeuralProject.Activators

    ''' <summary>Функция ReLU</summary>
    Public Class FunctionRelu
        Implements IFunctionActivator

        Public ReadOnly Property Name As String = "RELU" Implements IFunctionActivator.Name

        Public Function Activate(Value As Double) As Double Implements IFunctionActivator.Activate
            Return If(Value >= 0, Value, 0)
        End Function

        Public Function Deriviate(Value As Double) As Double Implements IFunctionActivator.Deriviate
            Return If(Value >= 0, 1, 0)
        End Function

    End Class

End Namespace