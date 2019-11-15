Option Explicit On
Option Strict On

Namespace NeuralProject

    ''' <summary>Функция Гиперболического тангенса. Значения [-1, 1]</summary>
    Public Class FunctionHypertan
        Implements IFunctionActivator

        Public ReadOnly Property Name As String = "HYPERTAN" Implements IFunctionActivator.Name

        Public Function Activate(Value As Double) As Double Implements IFunctionActivator.Activate
            Return (2 / (1 + Math.Exp(-2 * Value))) - 1
        End Function

        Public Function Deriviate(Value As Double) As Double Implements IFunctionActivator.Deriviate
            Return 1.0 - Value ^ 2
        End Function

    End Class

End Namespace