Option Explicit On
Option Strict On

Namespace NeuralProject.Interfaces

    ''' <summary>Интерфейс для реализации функций активации и производной</summary>
    Public Interface IFunctionActivator

        ''' <summary>Название функции</summary>
        ReadOnly Property Name As String

        ''' <summary>Функция активации</summary>
        ''' <param name="Value">Переданное значение</param>
        ''' <returns>Результат вычисления функции</returns>
        Function Activate(Value As Double) As Double

        ''' <summary>Функция производной</summary>
        ''' <param name="Value">Переданное значение</param>
        ''' <returns>Результат вычисления функции</returns>
        Function Deriviate(Value As Double) As Double

    End Interface

End Namespace