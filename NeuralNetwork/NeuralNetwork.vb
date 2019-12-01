Option Explicit On
Option Strict On

Imports NeuralProject.Interfaces


Namespace NeuralProject

    ''' <summary>Класс нейронной сети</summary>
    Public Class NeuralNetwork
        Implements INetwork

        ''' <summary>
        ''' Массив со списком границ массивов у слоёв (y)
        ''' y - номер слоя
        ''' (y) - значением является число = кол-во элементов в массиве - 1
        ''' </summary>
        Private bounds() As Integer

        ' Граница всех слоёв
        Private layerBound As Integer

        ' Граница всех весов
        Private weightBound As Integer

        ' Список функций в приложении
        Private functionList As Dictionary(Of String, IFunction)

        Private _neurons()() As Double
        Private _weights()()() As Double
        Private _errors()() As Double
        Private _activators()() As IFunction
        Private _activities()() As Double
        Private _biases() As Double

        ''' <summary>
        ''' Массив со списком значений нейронов (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - значением является итоговое значение нейрона
        ''' </summary>
        Public Property Neurons As Double()() Implements INetwork.Neurons
            Get
                Return _neurons
            End Get
            Set(value As Double()())
                _neurons = value
            End Set
        End Property

        ''' <summary>
        ''' Массив со списком весов (y)(m)(n)
        ''' y - номер между текущим слоем и следующим
        ''' m - номер нейрона из текущего слоя
        ''' n - номер нейрона из следующего слоя
        ''' (m)(n) - значением является переданный вес
        ''' </summary>
        Public Property Weights As Double()()() Implements INetwork.Weights
            Get
                Return _weights
            End Get
            Set(value As Double()()())
                _weights = value
            End Set
        End Property

        ''' <summary>
        ''' Массив со списком ошибок (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - значением является переданная ошибка
        ''' </summary>
        Public Property Errors As Double()() Implements INetwork.Errors
            Get
                Return _errors
            End Get
            Set(value As Double()())
                _errors = value
            End Set
        End Property

        ''' <summary>
        ''' Массив со списком активационных функций (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - значением является класс реализующий интерфейс активационной функции IFunction
        ''' </summary>
        Public Property Activators As IFunction()() Implements INetwork.Activators
            Get
                Return _activators
            End Get
            Set(value As IFunction()())
                _activators = value
            End Set
        End Property

        ''' <summary>
        ''' Массив со списком задействованных нейронов (y)(n)
        ''' y - номер слоя
        ''' n - номер нейрона
        ''' (n) - 1 - сигнал нейрона будет использован, 0 - сигнал будет обнулён
        ''' </summary>
        Public Property Activities As Double()() Implements INetwork.Activities
            Get
                Return _activities
            End Get
            Set(value As Double()())
                _activities = value
            End Set
        End Property

        ''' <summary>
        ''' Массив содержит слои и нейрон смещения (y)
        ''' y - номер слоя
        ''' (y) - значением является смещение. По-умолчанию 0, нет смещения
        ''' </summary>
        Public Property Biases As Double() Implements INetwork.Biases
            Get
                Return _biases
            End Get
            Set(value As Double())
                _biases = value
            End Set
        End Property


        ''' <summary>Метод инициализирует словарь с функциями активации</summary>
        Friend Shared Function RegenerateFunctionList() As Dictionary(Of String, IFunction)
            Dim result As New Dictionary(Of String, IFunction)
            Dim typeIAF = GetType(IFunction)

            For Each xAssembly In AppDomain.CurrentDomain.GetAssemblies()
                For Each xType In xAssembly.GetTypes()
                    If xType.IsClass AndAlso Not xType.IsAbstract AndAlso typeIAF.IsAssignableFrom(xType) Then
                        Dim objFunction = CType(Activator.CreateInstance(xType), IFunction)
                        result.Add(objFunction.Name, objFunction)
                    End If
                Next
            Next

            Return result
        End Function

        ''' <summary>Метод заполняем случайными числами веса в сети</summary>
        Public Sub RegenerateWeights()

            ' Инициализируем рандомизатор
            Dim rand As New Random(Environment.TickCount)

            ' Определяем размерность для весов
            For I = 0 To weightBound
                For M = 0 To _weights(I).GetUpperBound(0)
                    For N = 0 To _weights(I)(M).GetUpperBound(0)
                        _weights(I)(M)(N) = Math.Round(-0.5 + rand.NextDouble() * 1.5, 4)
                    Next
                Next
            Next
        End Sub

        ''' <summary>Метод создаёт базовую структуру из массивов для нейронной сети</summary>
        ''' <param name="NeuronCount">Массив с количеством нейронов в слоях</param>
        Public Function Regenerate(ParamArray NeuronCount() As Integer) As Integer Implements INetwork.Regenerate

            ' Находим список всех возможных функций в приложении
            functionList = RegenerateFunctionList()

            ' Находим границы
            layerBound = NeuronCount.Length - 1
            weightBound = NeuronCount.Length - 2

            ' Определяем размерность
            ReDim bounds(layerBound)
            ReDim _neurons(layerBound)
            ReDim _errors(layerBound)
            ReDim _activators(layerBound)
            ReDim _activities(layerBound)
            ReDim _biases(layerBound)
            ReDim _weights(weightBound)

            ' Инициализируем сигналы, ошибки и активаторы под количество нейронов в каждом слое
            For Y = 0 To layerBound

                ' Граница массива у каждого слоя (количество нейронов) в целях оптимизации доступа
                bounds(Y) = NeuronCount(Y) - 1

                ReDim _neurons(Y)(bounds(Y))
                ReDim _errors(Y)(bounds(Y))
                ReDim _activators(Y)(bounds(Y))
                ReDim _activities(Y)(bounds(Y))
                ReDim _biases(Y)

                ' Нейрон смещения
                _biases(Y) = 0

                For N = 0 To bounds(Y)
                    ' Функция активации по-умолчанию сигмойда
                    _activators(Y)(N) = functionList("SIGMOID")

                    ' Нейрон задействован
                    _activities(Y)(N) = 1
                Next
            Next

            ' Определяем размерность для весов
            For Y = 0 To weightBound
                ReDim _weights(Y)(bounds(Y)) ' Инициализируем массив, содержащий нейроны текущего слоя (M)

                For M = 0 To _weights(Y).GetUpperBound(0)
                    ReDim _weights(Y)(M)(bounds(Y + 1)) ' Инициализируем массив, содержащий нейроны следующего слоя (N)

                Next
            Next

            Return NeuronCount.Length
        End Function


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

            ' Создаём структуру
            Regenerate(NeuronCount)

            ' Заполяем веса
            RegenerateWeights()
        End Sub

        ''' <summary>Передаём исходные данные и рассчитываем результат сети</summary>
        ''' <param name="InputValues">Массив исходных значений</param>
        ''' <returns>Возвращает массив значений выходных нейронов</returns>
        Public Function Predict(InputValues() As Double) As Double() Implements INetwork.Predict
            For N = 0 To InputValues.GetUpperBound(0)
                _neurons(0)(N) = InputValues(N)
            Next

            For Y = 0 To layerBound - 1
                ForwardSignals(Y, Y + 1)
            Next

            Return _neurons(layerBound)
        End Function

        ''' <summary>Передаём обучающий сет и тренируем сеть</summary>
        ''' <param name="InputValues">Массив исходных значений</param>
        ''' <param name="TargetValues">Массив ожидаемых ответов</param>
        Public Function Training(InputValues() As Double, TargetValues() As Double, LearningRate As Double) As Double Implements INetwork.Training

            ' Инициализируем все нейроны во входном слое
            For N = 0 To InputValues.GetUpperBound(0)
                _neurons(0)(N) = InputValues(N)
            Next

            ' Выполняем прямое распространнение =>
            For Y = 0 To layerBound - 1
                ForwardSignals(Y, Y + 1)
            Next

            ' Рассчитываем итоговую ошибку по всем нейронам в выходном слое
            Dim averageQuadError = 0.0

            For N = 0 To bounds(layerBound)
                _errors(layerBound)(N) = TargetValues(N) - _neurons(layerBound)(N)
                averageQuadError += _errors(layerBound)(N) ^ 2 ' Рассчитываем среднеквадратичную ошибку
            Next

            ' Выполняем обратное распространнение <=
            For Y = layerBound To 1 Step -1
                BackwardErrors(Y - 1, Y, LearningRate)
            Next

            Return averageQuadError
        End Function

        ''' <summary>Метод изменяет функцию активации у всех нейронов в указанном слое</summary>
        ''' <param name="LayerIndex">Индекс слоя</param>
        ''' <param name="FunctionName">Название функции активации</param>
        Public Sub ChangeActivator(LayerIndex As Integer, FunctionName As String)
            For NeuronIndex = 0 To bounds(LayerIndex)
                ChangeActivator(LayerIndex, NeuronIndex, FunctionName)
            Next
        End Sub

        ''' <summary>Метод изменяет функцию активации у указанного слоя и нейрона</summary>
        ''' <param name="LayerIndex">Индекс слоя</param>
        ''' <param name="NeuronIndex">Индекс слоя</param>
        ''' <param name="FunctionName">Название функции активации</param>
        Public Sub ChangeActivator(LayerIndex As Integer, NeuronIndex As Integer, FunctionName As String)
            _activators(LayerIndex)(NeuronIndex) = functionList(FunctionName)
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
                    resultValue += _neurons(FromLayerIndex)(fromN) * _weights(FromLayerIndex)(fromN)(toN)
                Next

                ' Добавляем нейрон смещения при необходимости
                resultValue += _biases(FromLayerIndex)

                ' Выполняем активацию и выставляем переданный сигнал в нейрон
                _neurons(ToLayerIndex)(toN) = _activators(ToLayerIndex)(toN).Activate(resultValue) * _activities(ToLayerIndex)(toN)
            Next
        End Sub

        ''' <summary>Обратное распространение ошибки для выбранных слоёв</summary>
        ''' <param name="FromLayerIndex">Индекс слоя откуда (Y - 1)</param>
        ''' <param name="ToLayerIndex">Индекс слоя куда (Y)</param>
        ''' <param name="LearningRate">Коэфициент обучения</param>
        Private Sub BackwardErrors(FromLayerIndex As Integer, ToLayerIndex As Integer, LearningRate As Double)

            ' По всем нейронам слоя "Откуда", это предыдущий слой
            For fromN = 0 To bounds(FromLayerIndex)

                ' Обнуляем ошибку
                _errors(FromLayerIndex)(fromN) = 0

                ' По всем нейронам слоя "Куда", это текущий слой
                For toN = 0 To bounds(ToLayerIndex)
                    _errors(FromLayerIndex)(fromN) += _errors(ToLayerIndex)(toN) * _weights(FromLayerIndex)(fromN)(toN)
                Next
            Next

            ' По всем нейронам слоя "Куда"
            For toN = 0 To bounds(ToLayerIndex)

                ' Получаем значение нейрона
                Dim gradient As Double = _neurons(ToLayerIndex)(toN)

                ' Вычисляем производную (градиент)
                gradient = _activators(ToLayerIndex)(toN).Deriviate(gradient)

                ' По всем нейронам слоя "Откуда"
                For FromN = 0 To bounds(FromLayerIndex)

                    ' Корректируем вес
                    _weights(FromLayerIndex)(FromN)(toN) += LearningRate * _errors(ToLayerIndex)(toN) * gradient * _neurons(FromLayerIndex)(FromN)
                Next
            Next
        End Sub


    End Class


End Namespace