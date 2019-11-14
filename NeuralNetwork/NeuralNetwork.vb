Option Explicit On
Option Strict On

Namespace NeuralProject

    ''' <summary>Класс нейронной сети</summary>
    Public Class NeuralNetwork

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
        ''' (n) - значением является класс реализующий интерфейс активационной функции IFunctionActivator
        ''' </summary>
        Public Activators()() As IFunctionActivator

        ''' <summary>
        ''' Массив содержит слои и нейрон смещения (y)
        ''' y - номер слоя
        ''' (y) - значением является смещение. По-умолчанию 0, нет смещения
        ''' </summary>
        Public Biases() As Double

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
        ''' Эпоха
        ''' </summary>
        Public Epoch As Integer = 0

        ''' <summary>
        ''' Среднеквадратичная ошибка по тренировочному сету
        ''' </summary>
        Public AverageQuadError As Double = 0.0


        'Коллекция функций активаций
        Friend Functions As New Dictionary(Of String, IFunctionActivator)

        ' Рандом
        Private rand As Random

        ' Граница всех слоёв
        Private layerBound As Integer

        ' Граница всех весов
        Private weightBound As Integer


        ''' <summary>
        ''' Создаёт класс нейронной сети с выбранной конфигурацией.
        ''' NeuralNetwork[2, 4, 1] эта сеть для решения задачи XOR
        ''' </summary>
        ''' <param name="NeuronCount">
        ''' Первый элемент - количество нейронов в входном слое
        ''' Последний элемент - количество нейронов в выходном слое
        ''' Остальные элементы - количество слоёв и количество нейронов в них
        ''' </param>
        Public Sub New(ParamArray NeuronCount() As Integer)
            If NeuronCount.Length < 2 Then Throw New Exception("Неверная размерность слоёв. Должно быть минимум 2 слоя в сети")

            For Each xItem In NeuronCount
                If xItem <= 0 Then Throw New Exception("Количество нейронов в слое не может быть меньше или равное нулю")
            Next

            ' Находим границы
            layerBound = NeuronCount.Length - 1
            weightBound = NeuronCount.Length - 2

            ' Определяем размерность
            ReDim Bounds(layerBound)
            ReDim Neurons(layerBound)
            ReDim Errors(layerBound)
            ReDim Activators(layerBound)
            ReDim Biases(layerBound)
            ReDim Weights(weightBound)

            ' Находим все функции активации в приложении
            Dim typeIAF = GetType(IFunctionActivator)
            For Each xAssembly In AppDomain.CurrentDomain.GetAssemblies()
                For Each xType In xAssembly.GetTypes()
                    If xType.IsClass AndAlso Not xType.IsAbstract AndAlso typeIAF.IsAssignableFrom(xType) Then
                        Dim objFunction = CType(Activator.CreateInstance(xType), IFunctionActivator)
                        Functions.Add(objFunction.Name, objFunction)
                    End If
                Next
            Next
            ' Определяем размерность для нейронов
            For I = 0 To NeuronCount.Length - 1

                ' Инициализируем границу 
                Bounds(I) = NeuronCount(I) - 1

                ReDim Neurons(I)(Bounds(I))
                ReDim Errors(I)(Bounds(I))
                ReDim Activators(I)(Bounds(I))
                ReDim Biases(I)

                ' Нейрон смещения
                Biases(I) = 0

                ' Инициализируем активаторы и смещения
                For N = 0 To Activators(I).GetUpperBound(0)
                    Activators(I)(N) = Functions("SIGMOID")
                Next
            Next

            ' Определяем размерность для весов
            For I = 0 To weightBound
                ReDim Weights(I)(Neurons(I).Length - 1, Neurons(I + 1).Length - 1)

                ' Инициализируем веса случайными числами
                rand = New Random(Environment.TickCount)

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

        ''' <summary>Метод изменяет функцию активации у выбранного слоя</summary>
        ''' <param name="LayerIndex">Индекс слоя</param>
        ''' <param name="Activator">Функция активации</param>
        Public Sub changeActivatorFunction(LayerIndex As Integer, Activator As IFunctionActivator)
            For N = 0 To Bounds(LayerIndex)
                Activators(LayerIndex)(N) = Activator
            Next
        End Sub

        ''' <summary>Прямое распространнение для выбранных слоёв</summary>
        Private Sub Forward(FromLayerIndex As Integer, ToLayerIndex As Integer)

            ' По всем нейронам слоя "Куда"
            For ToN = 0 To Bounds(ToLayerIndex)
                Dim resultValue As Double = 0

                ' По всем нейронам слоя "Откуда"
                For FromN = 0 To Bounds(FromLayerIndex)
                    resultValue += Neurons(FromLayerIndex)(FromN) * Weights(FromLayerIndex)(FromN, ToN) + Biases(FromLayerIndex)
                Next

                ' Выполняем активацию
                resultValue = Activators(ToLayerIndex)(ToN).Activate(resultValue)

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

                ' Получаем значение нейрона
                Dim gradient As Double = Neurons(ToLayerIndex)(ToN)

                ' Вычисляем производную (градиент)
                gradient = Activators(ToLayerIndex)(ToN).Deriviate(gradient)

                ' По всем нейронам слоя "Откуда"
                For FromN = 0 To Bounds(FromLayerIndex)

                    ' Корректируем вес
                    Weights(FromLayerIndex)(FromN, ToN) += Neurons(FromLayerIndex)(FromN) * Errors(ToLayerIndex)(ToN) * gradient * LeaningRate

                Next
            Next
        End Sub

    End Class


End Namespace