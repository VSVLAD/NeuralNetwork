Option Explicit On
Option Strict On

Imports NeuralProject.Interfaces


Namespace NeuralProject.Activators

    ''' <summary>Логистическая функция Сигмоида. Значения [0, 1]</summary>
    Public Class FunctionSigmoid
        Implements IFunction

        Public ReadOnly Property Name As String = "SIGMOID" Implements IFunction.Name

        Public Function Activate(Value As Double) As Double Implements IFunction.Activate
            Return 1.0 / (1.0 + Math.Exp(-Value))
        End Function

        Public Function Deriviate(Value As Double) As Double Implements IFunction.Deriviate
            Return Value * (1.0 - Value)
        End Function

    End Class

End Namespace