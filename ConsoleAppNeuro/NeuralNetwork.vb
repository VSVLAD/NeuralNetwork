Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Namespace NeuralProject

    ''' <summary>Тип активационной функции</summary>
    Public Enum ActivatorFunctions
        SIGMOID
        RELU
        LINEAR
        HEAVISIDE
        HYPERTAN
    End Enum


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
        ''' (n) - значением является делегат активационной функции
        ''' </summary>
        Public Activators()() As ActivatorFunctions

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
                ReDim Biases(I)

                ' Нейрон смещения
                Biases(I) = 0

                ' Инициализируем активаторы и смещения
                For N = 0 To Activators(I).GetUpperBound(0)
                    Activators(I)(N) = ActivatorFunctions.SIGMOID
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
        Public Sub changeActivatorFunction(LayerIndex As Integer, Activator As ActivatorFunctions)
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


    ''' <summary>Класс предоставляем методы для сериализации и десериализации нейросети</summary>
    Public Class NeuralState

        Private Shared regExpSection As New Regex("\[(.*?)\]([\W\w]+?(?:\r{2,}|\n{2,}|$))", RegexOptions.Compiled Or RegexOptions.Multiline)
        Private Shared regExpNetwork As New Regex("\s*Layers\s*=\s*(.*)$\nLearningRate\s*=\s*(.*)$\nEpoch\s*=\s*(.*)", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        Private Shared regExpArrayAF As New Regex("\((\d)\)\((\d)\)\s*=\s*([A-z]+)", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        Private Shared regExpArray1D As New Regex("\((\d)\)\((\d)\)\s*=\s*([\d|\.]+)", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        Private Shared regExpArray2D As New Regex("\((\d)\)\((\d),(\d)\)\s*=\s*([\d|\.]+)", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)

        ' Метод сериализует массив формата Double()()
        Private Shared Function NeuronSerializer(Writer As StringBuilder, Section As String, Data As Double()()) As StringBuilder
            Writer.AppendLine($"[{Section}]")

            For idxLayer = 0 To Data.GetUpperBound(0)
                For idxNeuron = 0 To Data(idxLayer).GetUpperBound(0)
                    Writer.AppendLine($"({idxLayer})({idxNeuron})={Data(idxLayer)(idxNeuron)}")
                Next
            Next

            Return Writer
        End Function

        ' Метод сериализует массив формата Double()(,)
        Private Shared Function WeightSerializer(Writer As StringBuilder, Section As String, Data As Double()(,)) As StringBuilder
            Writer.AppendLine($"[{Section}]")

            For idxLayer = 0 To Data.GetUpperBound(0)
                For M = 0 To Data(idxLayer).GetUpperBound(0)
                    For N = 0 To Data(idxLayer).GetUpperBound(1)
                        Writer.AppendLine($"({idxLayer})({M},{N})={Data(idxLayer)(M, N)}")
                    Next
                Next
            Next

            Return Writer
        End Function

        ' Метод сериализует массив формата ActivatorFunction()()
        Private Shared Function ActivatorSerializer(Writer As StringBuilder, Section As String, Data As ActivatorFunctions()()) As StringBuilder
            Writer.AppendLine($"[{Section}]")

            For idxLayer = 0 To Data.GetUpperBound(0)
                For idxNeuron = 0 To Data(idxLayer).GetUpperBound(0)
                    Writer.AppendLine($"({idxLayer})({idxNeuron})={Data(idxLayer)(idxNeuron)}")
                Next
            Next

            Return Writer
        End Function

        ' Метод сериализует параметры сети
        Private Shared Function NetworkSerializer(Writer As StringBuilder, Section As String, Layers As String, LearningRate As Double, Epoch As Integer) As StringBuilder
            Writer.AppendLine($"[{Section}]")
            Writer.AppendLine($"Layers={Layers}")
            Writer.AppendLine($"LearningRate={LearningRate}")
            Writer.AppendLine($"Epoch={Epoch}")

            Return Writer
        End Function

        ' Метод десериализует параметры сети в сеть
        Private Shared Function NetworkDeserializer(SectionBody As String) As NeuralNetwork
            Dim mNetwork = regExpNetwork.Match(SectionBody)

            Dim Layers = mNetwork.Groups(1).Value.Replace(vbCr, "").Split(","c).Select(Function(x) CInt(x)).ToArray()
            Dim LearningRate = CDbl(mNetwork.Groups(2).Value)
            Dim Epoch = CInt(mNetwork.Groups(3).Value)

            ' Первичное создание сети
            Dim retValue = New NeuralNetwork(Layers)
            retValue.LeaningRate = LearningRate
            retValue.Epoch = Epoch

            Return retValue
        End Function

        ' Метод десериализует массив Double()()
        Private Shared Function NeuronDeserializer(Network As NeuralNetwork, SectionBody As String) As NeuralNetwork
            For Each xMatch As Match In regExpArray1D.Matches(SectionBody)
                Network.Neurons(CInt(xMatch.Groups(1).Value))(CInt(xMatch.Groups(2).Value)) = CDbl(xMatch.Groups(3).Value)
            Next

            Return Network
        End Function

        ' Метод десериализует массив Double()()
        Private Shared Function ErrorDeserializer(Network As NeuralNetwork, SectionBody As String) As NeuralNetwork
            For Each xMatch As Match In regExpArray1D.Matches(SectionBody)
                Network.Errors(CInt(xMatch.Groups(1).Value))(CInt(xMatch.Groups(2).Value)) = CDbl(xMatch.Groups(3).Value)
            Next

            Return Network
        End Function

        ' Метод десериализует массив Double()(,)
        Private Shared Function WeightDeserializer(Network As NeuralNetwork, SectionBody As String) As NeuralNetwork
            For Each xMatch As Match In regExpArray2D.Matches(SectionBody)
                Network.Weights(CInt(xMatch.Groups(1).Value))(CInt(xMatch.Groups(2).Value), CInt(xMatch.Groups(3).Value)) = CDbl(xMatch.Groups(4).Value)
            Next

            Return Network
        End Function

        ' Метод десериализует массив ActivatorFunction()()
        Private Shared Function ActivatorDeserializer(Network As NeuralNetwork, SectionBody As String) As NeuralNetwork
            Dim ActivatorFunctionName = New Dictionary(Of String, ActivatorFunctions)
            Dim arrNames() = [Enum].GetNames(GetType(ActivatorFunctions))
            Dim arrValues() = CType([Enum].GetValues(GetType(ActivatorFunctions)), ActivatorFunctions())

            ' Готовим словарь со списком функций
            For idx = 0 To arrNames.GetUpperBound(0)
                ActivatorFunctionName.Add(arrNames(idx), arrValues(idx))
            Next

            For Each xMatch As Match In regExpArrayAF.Matches(SectionBody)
                Network.Activators(CInt(xMatch.Groups(1).Value))(CInt(xMatch.Groups(2).Value)) = ActivatorFunctionName(xMatch.Groups(3).Value)
            Next

            Return Network
        End Function

        ''' <summary>Сохранить параметры нейросети в файл</summary>
        Public Shared Sub Save(FileName As String, Network As NeuralNetwork)

            'Меняем формат для потока, для корректной записи чисел
            Dim myCulture = Globalization.CultureInfo.CurrentCulture
            Threading.Thread.CurrentThread.CurrentCulture = Globalization.CultureInfo.InvariantCulture

            ' Собираем параметры сети в строку инициализации
            Dim getLayers As Func(Of NeuralNetwork, String) =
                Function(source As NeuralNetwork) As String
                    Dim ret As String = String.Empty
                    For n = 0 To source.Neurons.GetUpperBound(0)
                        If n <> source.Neurons.GetUpperBound(0) Then
                            ret &= source.Neurons(n).Length & ", "
                        Else
                            ret &= source.Neurons(n).Length
                        End If
                    Next
                    Return ret
                End Function

            ' Собираем секции
            Dim sbWriter As New StringBuilder

            sbWriter = NetworkSerializer(sbWriter, "NeuralNetwork", getLayers(Network), Network.LeaningRate, Network.Epoch)
            sbWriter.AppendLine()

            sbWriter = NeuronSerializer(sbWriter, "Neurons", Network.Neurons)
            sbWriter.AppendLine()

            sbWriter = NeuronSerializer(sbWriter, "Errors", Network.Errors)
            sbWriter.AppendLine()

            sbWriter = WeightSerializer(sbWriter, "Weights", Network.Weights)
            sbWriter.AppendLine()

            sbWriter = ActivatorSerializer(sbWriter, "Activators", Network.Activators)
            sbWriter.AppendLine()

            File.WriteAllText(FileName, sbWriter.ToString(), Encoding.UTF8)

            'Возвращаем формат для потока
            Threading.Thread.CurrentThread.CurrentCulture = myCulture
        End Sub

        ''' <summary>Загрузить параметры нейросети из файла</summary>
        Public Shared Function Load(FileName As String) As NeuralNetwork

            'Меняем формат для потока, для корректного чтения чисел
            Dim myCulture = Globalization.CultureInfo.CurrentCulture
            Threading.Thread.CurrentThread.CurrentCulture = Globalization.CultureInfo.InvariantCulture

            Dim fileBody = File.ReadAllText(FileName, Encoding.UTF8)
            Dim resultNetwork As NeuralNetwork = Nothing

            ' Смотрим все найденные секции
            For Each mSection As Match In regExpSection.Matches(fileBody.Replace(vbLf, String.Empty))

                ' Тело секции. { Костыль для регулярки секций: замена LF на пустоту, замена CR на CRLF}
                Dim sectionBody As String = mSection.Groups(2).Value.Replace(vbCr, vbCrLf)

                ' По имени секции
                Select Case mSection.Groups(1).Value

                    Case "NeuralNetwork"
                        resultNetwork = NetworkDeserializer(sectionBody)

                    Case "Neurons"
                        resultNetwork = NeuronDeserializer(resultNetwork, sectionBody)

                    Case "Activators"
                        resultNetwork = ActivatorDeserializer(resultNetwork, sectionBody)

                    Case "Errors"
                        resultNetwork = ErrorDeserializer(resultNetwork, sectionBody)

                    Case "Weights"
                        resultNetwork = WeightDeserializer(resultNetwork, sectionBody)

                End Select
            Next

            'Возвращаем формат для потока
            Threading.Thread.CurrentThread.CurrentCulture = myCulture

            Return resultNetwork
        End Function

    End Class


End Namespace