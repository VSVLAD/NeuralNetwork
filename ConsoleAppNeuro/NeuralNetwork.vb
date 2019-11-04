﻿Namespace NeuralProject

    ''' <summary>Тип активационной функции</summary>
    Public Enum ActivatorFunctions
        SIGMOID
        RELU
        LINEAR
        HEAVISIDE
        HYPERTAN
    End Enum

    ''' <summary>Класс нейронной сети</summary>
    Public Class Network

        ''' <summary>
        ''' Массив со списком значений нейронов (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - значением является итоговое значение нейрона
        ''' </summary>
        Public Neurons()() As Double

        ''' <summary>
        ''' Массив со списком весов (y)(m, n)
        ''' y - номер между текущим слоем и следующим
        ''' m - номер нейрона из текущего слоя
        ''' n - номер нейрона из следующего слоя
        ''' (m, n) - значением является переданный вес
        ''' </summary>
        Public Weights()(,) As Double

        ''' <summary>
        ''' Массив со списком ошибок (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - значением является переданная ошибка
        ''' </summary>
        Public Errors()() As Double

        ''' <summary>
        ''' Массив со списком активационных функций (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - значением является делегат активационной функции
        ''' </summary>
        Public Activators()() As ActivatorFunctions

        ''' <summary>
        ''' Массив со списком смещений (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - значением является смещение
        ''' </summary>
        Public Biases()() As Double

        ''' <summary>
        ''' Массив со списком границ массивов (y)
        ''' y - номер слоя
        ''' (y) - значением является число - кол-во элементов в массиве - 1
        ''' </summary>
        Private Bounds() As Integer

        ''' <summary>
        ''' Коэффициент скорости обучения
        ''' </summary>
        Public LeaningRate As Double = 0.01

        ''' <summary>
        ''' Среднеквадратичная ошибка по тренировочному сету
        ''' </summary>
        Public AverageQuadError As Double = 0.0


        ' Рандом
        Private rand As New Random(Environment.TickCount)

        ' Граница всех слоёв
        Private layerBound As Integer

        ' Граница всех весов
        Private weightBound As Integer


        ''' <summary>Создаём нейронную сеть.
        ''' Первый элемент - количество нейронов в входном слое
        ''' Последний элемент - количество нейронов в выходном слое
        ''' Остальные элементы - количество слоёв и количество нейронов в них</summary>
        ''' <param name="NeuronCount"></param>
        ''' <example>Network(2, 2, 1) ' Пример сети для решения задачи XOR</example>
        Public Sub New(ParamArray NeuronCount() As Integer)
            If NeuronCount.Length < 2 Then Throw New Exception("Неверная размерность слоёв. Должно быть минимум 2 слоя в сети")

            For Each xItem In NeuronCount
                If xItem <= 0 Then Throw New Exception("Количество нейронов в слое не может быть меньше или равное нулю")
            Next

            layerBound = NeuronCount.Length - 1
            weightBound = NeuronCount.Length - 2

            ' Определяем размерность
            ReDim Bounds(layerBound)
            ReDim Neurons(layerBound)
            ReDim Errors(layerBound)
            ReDim Activators(layerBound)
            ReDim Biases(layerBound)
            ReDim Weights(weightBound)

            ' Определяем размерность для нейронов
            For I = 0 To NeuronCount.Length - 1

                ' Инициализируем границу
                Bounds(I) = NeuronCount(I) - 1

                ReDim Neurons(I)(Bounds(I))
                ReDim Errors(I)(Bounds(I))
                ReDim Activators(I)(Bounds(I))
                ReDim Biases(I)(Bounds(I))

                ' Инициализируем активаторы и смещения
                For N = 0 To Activators(I).GetUpperBound(0)
                    Activators(I)(N) = ActivatorFunctions.SIGMOID
                    Biases(I)(N) = 1
                Next
            Next

            ' Определяем размерность для весов
            For I = 0 To weightBound
                ReDim Weights(I)(Neurons(I).Length - 1, Neurons(I + 1).Length - 1)

                ' Инициализируем веса случайными числами
                For M = 0 To Weights(I).GetUpperBound(0)
                    For N = 0 To Weights(I).GetUpperBound(1)
                        Weights(I)(M, N) = Math.Round(-0.5 + rand.NextDouble() * 1.5, 4)
                    Next
                Next
            Next

        End Sub

        ''' <summary>Передаём исходные данные и рассчитываем результат сети</summary>
        ''' <param name="InputValues">Массив исходных значений</param>
        ''' <returns>Возвращает массив значений выходных нейронов</returns>
        Public Function Predict(InputValues() As Double) As Double()
            For I = 0 To InputValues.GetUpperBound(0)
                Neurons(0)(I) = InputValues(I)
            Next

            For Y = 0 To layerBound - 1
                Forward(Y, Y + 1)
            Next

            Return Neurons(layerBound)
        End Function

        ''' <summary>Передаём обучающий сет и тренируем сеть</summary>
        ''' <param name="InputValues">Массив исходных значений</param>
        ''' <param name="OutputValue">Массив ожидаемых ответов</param>
        Public Sub TrainingSet(InputValues() As Double, OutputValue() As Double)

            ' Инициализируем все нейроны входного слоя
            For N = 0 To InputValues.GetUpperBound(0)
                Neurons(0)(N) = InputValues(N)
            Next

            ' Выполняем прямое распространнение по всем слоям =>
            For Y = 0 To layerBound - 1
                Forward(Y, Y + 1)
            Next

            ' Рассчитываем ошибку по всем нейронам в выходном слое
            For N = 0 To Bounds(layerBound)
                Errors(layerBound)(N) = OutputValue(N) - Neurons(layerBound)(N)
                AverageQuadError += Errors(layerBound)(N) ^ 2 ' Рассчитываем итоговую среднеквадратичную ошибку
            Next

            ' Выполняем обратное распространнение ошибки <=
            For Y = layerBound To 1 Step -1
                FindErrors(Y, Y - 1)
            Next

            ' Корректируем веса =>
            For Y = 0 To layerBound - 1
                Backward(Y, Y + 1)
            Next
        End Sub

        ''' <summary>Прямое распространнение для выбранных слоёв</summary>
        Private Sub Forward(FromLayerIndex As Integer, ToLayerIndex As Integer)

            ' По всем нейронам слоя "Куда"
            For ToN = 0 To Bounds(ToLayerIndex)
                Dim resultValue As Double = 0

                ' По всем нейронам слоя "Откуда"
                For FromN = 0 To Bounds(FromLayerIndex)
                    resultValue += Neurons(FromLayerIndex)(FromN) * Weights(FromLayerIndex)(FromN, ToN)
                Next

                ' Выполняем активацию
                Select Case Activators(ToLayerIndex)(ToN)
                    Case ActivatorFunctions.SIGMOID
                        resultValue = 1 / (1 + Math.Exp(-resultValue))

                    Case ActivatorFunctions.RELU
                        resultValue = If(resultValue >= 0, resultValue, 0)

                    Case ActivatorFunctions.HYPERTAN
                        resultValue = (2 / (1 + Math.Exp(-2 * resultValue))) - 1

                    Case ActivatorFunctions.HEAVISIDE
                        resultValue = If(resultValue >= 0, 1, 0)

                    Case ActivatorFunctions.LINEAR
                        resultValue = resultValue

                End Select

                ' Выставляем переданное значение нейрону
                Neurons(ToLayerIndex)(ToN) = resultValue
            Next
        End Sub

        ''' <summary>Обратное распространение ошибки для выбранных слоёв</summary>
        Private Sub FindErrors(FromLayerIndex As Integer, ToLayerIndex As Integer)
            If ToLayerIndex = 0 Then Return ' Ошибку не передаём во входной слой, это не нужно

            ' По всем нейронам слоя "Куда"
            For ToN = 0 To Bounds(ToLayerIndex)
                Errors(ToLayerIndex)(ToN) = 0 ' Обнуляем ошибку

                ' По всем нейронам слоя "Откуда"
                For FromN = 0 To Bounds(FromLayerIndex)
                    Errors(ToLayerIndex)(ToN) += Errors(FromLayerIndex)(FromN) * Weights(ToLayerIndex)(ToN, FromN)
                Next
            Next
        End Sub

        ''' <summary>Корректировка весов для выбранных слоёв</summary>
        Private Sub Backward(FromLayerIndex As Integer, ToLayerIndex As Integer)

            ' По всем нейронам слоя "Куда"
            For ToN = 0 To Bounds(ToLayerIndex)

                ' Вычисляем производную (градиент)
                Dim gradient As Double = Neurons(ToLayerIndex)(ToN)

                Select Case Activators(ToLayerIndex)(ToN)
                    Case ActivatorFunctions.SIGMOID
                        gradient = gradient * (1 - gradient)

                    Case ActivatorFunctions.RELU
                        gradient = If(gradient >= 0, 1, 0)

                    Case ActivatorFunctions.HYPERTAN
                        gradient = 1.0 - gradient ^ 2

                    Case ActivatorFunctions.HEAVISIDE
                        gradient = If(gradient <> 0, 0, -1)

                    Case ActivatorFunctions.LINEAR
                        gradient = 1

                End Select

                ' По всем нейронам слоя "Откуда"
                For FromN = 0 To Bounds(FromLayerIndex)

                    ' Корректируем вес
                    Weights(FromLayerIndex)(FromN, ToN) += Neurons(FromLayerIndex)(FromN) * Errors(ToLayerIndex)(ToN) * gradient * LeaningRate

                Next
            Next
        End Sub

    End Class


End Namespace