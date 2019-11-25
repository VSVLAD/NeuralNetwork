Option Explicit On
Option Strict On

Imports NeuralProject.Interfaces


Namespace NeuralProject.Interfaces

    ''' <summary>Интерфейс для управления нейроной сетью</summary>
    Public Interface INetworkControl

        ''' <summary>Передаём исходные данные и рассчитываем результат сети</summary>
        ''' <param name="InputValues">Массив исходных значений</param>
        ''' <returns>Возвращает массив значений выходных нейронов</returns>
        Function Predict(InputValues() As Double) As Double()

        ''' <summary>Передаём обучающий сет и тренируем сеть</summary>
        ''' <param name="InputValues">Массив исходных значений</param>
        ''' <param name="TargetValues">Массив ожидаемых ответов</param>
        Function Training(InputValues() As Double, TargetValues() As Double) As Integer

    End Interface

End Namespace