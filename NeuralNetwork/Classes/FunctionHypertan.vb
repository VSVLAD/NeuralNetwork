Option Explicit On
Option Strict On

Imports NeuralProject.Interfaces


Namespace NeuralProject.Activators

    ''' <summary>Функция Гиперболического тангенса. Значения [-1, 1]</summary>
    Public Class FunctionHypertan
        Implements IFunction

        Public ReadOnly Property Name As String = "HYPERTAN" Implements IFunction.Name

        Public Function Activate(Value As Double) As Double Implements IFunction.Activate
            Return (2.0 / (1.0 + Math.Exp(-2.0 * Value))) - 1.0
        End Function

        Public Function Deriviate(Value As Double) As Double Implements IFunction.Deriviate
            Return 1.0 - Value ^ 2.0
        End Function

    End Class

End Namespace