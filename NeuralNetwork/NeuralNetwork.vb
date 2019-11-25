Option Explicit On
Option Strict On

Imports NeuralProject.Interfaces


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
        ''' Массив со списком весов (y)(m)(n)
        ''' y - номер между текущим слоем и следующим
        ''' m - номер нейрона из текущего слоя
        ''' n - номер нейрона из следующего слоя
        ''' (m)(n) - значением является переданный вес
        ''' </summary>
        Public Weights()()() As Double

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
        ''' Массив со списком задействованных нейронов (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - 1 - сигнал нейрона будет использован, 0 - сигнал будет обнулён
        ''' </summary>
        Public Activities()() As Double

        ''' <summary>
        ''' Массив содержит слои и нейрон смещения (y)
        ''' y - номер слоя
        ''' (y) - значением является смещение. По-умолчанию 0, нет смещения
        ''' </summary>
        Public Biases() As Double

        ''' <summary>
        ''' Массив со списком границ массивов у слоёв (y)
        ''' y - номер слоя
        ''' (y) - значением является число = кол-во элементов в массиве - 1
        ''' </summary>
        Private Bounds() As Integer

        ''' <summary>
        ''' Коэффициент скорости обучения
        ''' </summary>
        Public LeaningRate As Double = 0.01

        ''' <summary>
        ''' Текущая эпоха
        ''' </summary>
        Public Epoch As Integer = 0

        ''' <summary>
        ''' Среднеквадратичная ошибка по тренировочному сету
        ''' </summary>
        Public AverageQuadError As Double = 0.0

        'Коллекция функций активаций
        Friend Functions As IDictionary(Of String, IFunctionActivator)

        ' Граница всех слоёв
        Private layerBound As Integer

        ' Граница всех весов
        Private weightBound As Integer


        ''' <summary>Метод заполняем случайными числами веса в сети</summary>
        Public Sub regenerateWeights()

            ' Инициализируем рандомизатор
            Dim rand As New Random(Environment.TickCount)

            ' Определяем размерность для весов
            For I = 0 To weightBound
                For M = 0 To Weights(I).GetUpperBound(0)
                    For N = 0 To Weights(I)(M).GetUpperBound(0)
                        Weights(I)(M)(N) = Math.Round(-0.5 + rand.NextDouble() * 1.5, 4)
                    Next
                Next
            Next
        End Sub

        ''' <summary>Метод инициализирует словарь с функциями активации</summary>
        Public Function regenerateFunctionList() As Dictionary(Of String, IFunctionActivator)
            Dim result As New Dictionary(Of String, IFunctionActivator)
            Dim typeIAF = GetType(IFunctionActivator)

            For Each xAssembly In AppDomain.CurrentDomain.GetAssemblies()
                For Each xType In xAssembly.GetTypes()
                    If xType.IsClass AndAlso Not xType.IsAbstract AndAlso typeIAF.IsAssignableFrom(xType) Then
                        Dim objFunction = CType(Activator.CreateInstance(xType), IFunctionActivator)
                        result.Add(objFunction.Name, objFunction)
                    End If
                Next
            Next

            Return result
        End Function

        ''' <summary>Метод создаёт базовую структуру из массивов для нейронной сети</summary>
        ''' <param name="NeuronCount">Массив с количеством нейронов в слоях</param>
        Public Sub regenerateNetworkStructure(ParamArray NeuronCount() As Integer)

            ' Находим границы
            layerBound = NeuronCount.Length - 1
            weightBound = NeuronCount.Length - 2

            ' Определяем размерность
            ReDim Bounds(layerBound)
            ReDim Neurons(layerBound)
            ReDim Errors(layerBound)
            ReDim Activators(layerBound)
            ReDim Activities(layerBound)
            ReDim Biases(layerBound)
            ReDim Weights(weightBound)

            ' Инициализируем сигналы, ошибки и активаторы под количество нейронов в каждом слое
            For Y = 0 To layerBound

                ' Граница массива у каждого слоя (количество нейронов) в целях оптимизации доступа
                Bounds(Y) = NeuronCount(Y) - 1

                ReDim Neurons(Y)(Bounds(Y))
                ReDim Errors(Y)(Bounds(Y))
                ReDim Activators(Y)(Bounds(Y))
                ReDim Activities(Y)(Bounds(Y))
                ReDim Biases(Y)

                ' Нейрон смещения
                Biases(Y) = 0

                For N = 0 To Bounds(Y)
                    ' Функция активации по-умолчанию сигмойда
                    Activators(Y)(N) = Functions("SIGMOID")

                    ' Нейрон задействован
                    Activities(Y)(N) = 1
                Next
            Next

            ' Определяем размерность для весов
            For Y = 0 To weightBound
                ReDim Weights(Y)(Bounds(Y)) ' Инициализируем массив, содержащий нейроны текущего слоя (M)

                For M = 0 To Weights(Y).GetUpperBound(0)
                    ReDim Weights(Y)(M)(Bounds(Y + 1)) ' Инициализируем массив, содержащий нейроны следующего слоя (N)

                Next
            Next
        End Sub

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

            ' Находим все функции активации в приложении
            Me.Functions = regenerateFunctionList()

            ' Создаём структуру
            regenerateNetworkStructure(NeuronCount)

            ' Заполяем веса
            regenerateWeights()
        End Sub

        ''' <summary>Передаём исходные данные и рассчитываем результат сети</summary>
        ''' <param name="InputValues">Массив исходных значений</param>
        ''' <returns>Возвращает массив значений выходных нейронов</returns>
        Public Function Predict(InputValues() As Double) As Double()
            For N = 0 To InputValues.GetUpperBound(0)
                Neurons(0)(N) = InputValues(N)
            Next

            For Y = 0 To layerBound - 1
                ForwardSignals(Y, Y + 1)
            Next

            Return Neurons(layerBound)
        End Function

        ''' <summary>Передаём обучающий сет и тренируем сеть</summary>
        ''' <param name="InputValues">Массив исходных значений</param>
        ''' <param name="TargetValues">Массив ожидаемых ответов</param>
        Public Sub TrainingSet(InputValues() As Double, TargetValues() As Double)

            ' Инициализируем все нейроны во входном слое
            For N = 0 To InputValues.GetUpperBound(0)
                Me.Neurons(0)(N) = InputValues(N)
            Next

            ' Выполняем прямое распространнение =>
            For Y = 0 To layerBound - 1
                ForwardSignals(Y, Y + 1)
            Next

            ' Рассчитываем итоговую ошибку по всем нейронам в выходном слое
            For N = 0 To Bounds(layerBound)
                Me.Errors(layerBound)(N) = TargetValues(N) - Neurons(layerBound)(N)
                Me.AverageQuadError += Errors(layerBound)(N) ^ 2 ' Рассчитываем среднеквадратичную ошибку
            Next

            ' Выполняем обратное распространнение <=
            For Y = layerBound To 1 Step -1
                BackwardErrors(Y - 1, Y)
            Next

        End Sub

        ''' <summary>Метод изменяет функцию активации у выбранного слоя</summary>
        ''' <param name="LayerIndex">Индекс слоя</param>
        ''' <param name="FunctionName">Название функции активации</param>
        Public Sub ChangeActivator(LayerIndex As Integer, FunctionName As String)
            For NeuronIndex = 0 To Bounds(LayerIndex)
                ChangeActivator(LayerIndex, NeuronIndex, FunctionName)
            Next
        End Sub

        ''' <summary>Метод изменяет функцию активации у выбранного слоя и нейрона</summary>
        ''' <param name="LayerIndex">Индекс слоя</param>
        ''' <param name="NeuronIndex">Индекс слоя</param>
        ''' <param name="FunctionName">Название функции активации</param>
        Public Sub ChangeActivator(LayerIndex As Integer, NeuronIndex As Integer, FunctionName As String)
            Activators(LayerIndex)(NeuronIndex) = Functions(FunctionName)
        End Sub

        ''' <summary>Прямое распространнение для выбранных слоёв</summary>
        ''' <param name="FromLayerIndex">Индекс слоя откуда (Y)</param>
        ''' <param name="ToLayerIndex">Индекс слоя куда (Y + 1)</param>
        Private Sub ForwardSignals(FromLayerIndex As Integer, ToLayerIndex As Integer)

            ' По всем нейронам слоя "Куда"
            For toN = 0 To Bounds(ToLayerIndex)
                Dim resultValue As Double = 0

                ' По всем нейронам слоя "Откуда"
                For fromN = 0 To Bounds(FromLayerIndex)
                    resultValue += Neurons(FromLayerIndex)(fromN) * Weights(FromLayerIndex)(fromN)(toN)
                Next

                ' Добавляем нейрон смещения при необходимости
                resultValue += Biases(FromLayerIndex)

                ' Выполняем активацию и выставляем переданный сигнал в нейрон
                Neurons(ToLayerIndex)(toN) = Activators(ToLayerIndex)(toN).Activate(resultValue) * Activities(ToLayerIndex)(toN)
            Next
        End Sub

        ''' <summary>Обратное распространение ошибки для выбранных слоёв</summary>
        ''' <param name="FromLayerIndex">Индекс слоя откуда (Y - 1)</param>
        ''' <param name="ToLayerIndex">Индекс слоя куда (Y)</param>
        Private Sub BackwardErrors(FromLayerIndex As Integer, ToLayerIndex As Integer)

            ' По всем нейронам слоя "Откуда", это предыдущий слой
            For fromN = 0 To Bounds(FromLayerIndex)

                ' Обнуляем ошибку
                Errors(FromLayerIndex)(fromN) = 0

                ' По всем нейронам слоя "Куда", это текущий слой
                For toN = 0 To Bounds(ToLayerIndex)
                    Errors(FromLayerIndex)(fromN) += Errors(ToLayerIndex)(toN) * Weights(FromLayerIndex)(fromN)(toN)
                Next
            Next

            ' По всем нейронам слоя "Куда"
            For toN = 0 To Bounds(ToLayerIndex)

                ' Получаем значение нейрона
                Dim gradient As Double = Neurons(ToLayerIndex)(toN)

                ' Вычисляем производную (градиент)
                gradient = Activators(ToLayerIndex)(toN).Deriviate(gradient)

                ' По всем нейронам слоя "Откуда"
                For FromN = 0 To Bounds(FromLayerIndex)

                    ' Корректируем вес
                    Weights(FromLayerIndex)(FromN)(toN) += LeaningRate * Errors(ToLayerIndex)(toN) * gradient * Neurons(FromLayerIndex)(FromN)
                Next
            Next
        End Sub

    End Class


End Namespace