Option Explicit On
Option Strict On

Namespace NeuralProject

    ''' <summary>Логистическая функция Сигмоида. Значения [0, 1]</summary>
    Public Class FunctionSigmoid
        Implements IFunctionActivator

        Public ReadOnly Property Name As String = "SIGMOID" Implements IFunctionActivator.Name

        Public Function Activate(Value As Double) As Double Implements IFunctionActivator.Activate
            Return 1 / (1 + Math.Exp(-Value))
        End Function

        Public Function Deriviate(Value As Double) As Double Implements IFunctionActivator.Deriviate
            Return Value * (1 - Value)
        End Function

    End Class

End Namespace