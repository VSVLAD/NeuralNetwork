Imports System.Runtime.CompilerServices
Imports NeuralProject
Imports NeuralProject.Interfaces

Module Program

    Private rand As New Random(Environment.TickCount)

    Public Sub Main()
        TaskBinToDec()
    End Sub


    'Public Sub TaskChocolate()
    '    Dim network As New NeuralNetwork(8, 200, 100, 7)
    '    network.LeaningRate = 0.1
    '    network.ChangeActivator(3, "RELU")

    '    ' По всем эпохам
    '    Dim N As Integer
    '    Do While N < 10000000
    '        N += 1

    '        network.Epoch = N
    '        network.AverageQuadError = 0
    '        network.AverageQuadError = network.AverageQuadError / 265
    '        If network.AverageQuadError < 0.00000001 Then Exit Do

    '        Console.WriteLine("Эпоха: {0}. Ошибка {1}", N, network.averageQuadError.ToString("##0.################"))

    '        ' Управление
    '        If Console.KeyAvailable Then
    '            Select Case Console.ReadKey().Key
    '                Case ConsoleKey.Z

    '                    ' Тестируем, запускаем машину
    '                    Console.WriteLine("=== Проверка работы сети ===")

    '                    ' Days
    '                    Dim toHardChoco = 0.7
    '                    Dim processData() = {0.041667, 0.25, 0.1, 0.1, 0.000204, 1.235, toHardChoco, 0.003}

    '                    For d = 1 To 31
    '                        For h = 1 To 24
    '                            Console.WriteLine($"День: {d} Час: {h}")

    '                            Dim result() = network.Predict(processData)
    '                            result(0) = Math.Round(result(0), 2)
    '                            result(1) = Math.Round(result(1), 2)
    '                            result(2) = Math.Round(result(2), 2)
    '                            result(3) = Math.Round(result(3), 2)
    '                            result(4) = Math.Round(result(4), 2)
    '                            result(5) = Math.Round(result(5), 2)
    '                            result(6) = Math.Round(result(6), 2)

    '                            Console.WriteLine($"Расчитана горкость: {Math.Round(processData(6) * 100, 2)}. Ожидаемая горкость: {toHardChoco * 100}")
    '                            Console.WriteLine($"Расчитана пропорция: {Math.Round(processData(7) * 100, 2)}. Ожидаемо 100%")

    '                            ' Включен ли двигатель
    '                            If result(0) > 0.8 Then

    '                                'Какао
    '                                If result(1) > 0.8 Then processData(1) += 1
    '                                If result(2) > 0.8 Then processData(1) -= 1

    '                                'Молоко
    '                                If result(3) > 0.8 Then processData(2) += 1
    '                                If result(4) > 0.8 Then processData(2) -= 1

    '                                'Сахар
    '                                If result(5) > 0.8 Then processData(3) += 1
    '                                If result(6) > 0.8 Then processData(3) -= 1
    '                            Else
    '                                Console.WriteLine("Не рабочий час")
    '                            End If

    '                            ' Следующий день
    '                            processData(0) = h / 24
    '                            Threading.Thread.Sleep(100)
    '                        Next
    '                    Next

    '                    Console.ReadKey()

    '                Case ConsoleKey.S
    '                    network.LeaningRate += 0.01
    '                    Console.Title = "LeaningRate: " & network.LeaningRate

    '                Case ConsoleKey.A
    '                    network.LeaningRate -= 0.01
    '                    If network.LeaningRate <= 0 Then network.LeaningRate = 0.01
    '                    Console.Title = "LeaningRate: " & network.LeaningRate

    '                Case ConsoleKey.F1
    '                    NeuralState.Save("Choco.txt", network)

    '                Case ConsoleKey.F2
    '                    network = NeuralState.Load("Choco.txt")
    '                    N = network.Epoch

    '            End Select
    '        End If
    '    Loop


    'End Sub


    'Public Sub TaskForecast()
    '    Dim sqlite As New SQLite.SQLiteConnection("Data Source=""X:\NeuralNetwork\KrasnodarCast.db""")
    '    sqlite.Open()

    '    Dim network As New NeuralNetwork(1, 20, 10, 1)
    '    'network = NeuralState.Load("Forecast_2.txt")
    '    network.Epoch = 0
    '    network.LeaningRate = 0.8

    '    ' Создаём тренировочные сеты
    '    Dim rowCount As Integer = sqlite.SelectCell(" select count(*) from weather where strftime('%Y', ondate) = '2018' order by ondate asc ")
    '    Dim rowIndex As Integer

    '    Dim trainingData(rowCount - 1, 1) As Double

    '    For Each row In sqlite.SelectRows(" select * from weather where strftime('%Y', ondate) = '2018' order by ondate asc ")
    '        Dim scaledDate = NeuralConvert.Scaler(NeuralConvert.ToUnixTime(row("ondate")), 1420059600000, 1577826000000, 0, 1)
    '        Dim scaledTemp = NeuralConvert.Scaler(row("temperature"), -100, 100, 0, 1)

    '        trainingData(rowIndex, 0) = scaledDate
    '        trainingData(rowIndex, 1) = scaledTemp

    '        rowIndex += 1
    '    Next

    '    ' Перемешиваем данные
    '    Dim swap1 As Double, swap2 As Double
    '    Dim r1 As Integer, r2 As Integer
    '    Randomize()

    '    For i = 0 To 10000
    '        r1 = 1 + (Rnd() * (rowIndex - 2))
    '        r2 = 1 + (Rnd() * (rowIndex - 2))

    '        swap1 = trainingData(r1, 0)
    '        swap2 = trainingData(r1, 0)
    '        trainingData(r1, 0) = trainingData(r2, 0)
    '        trainingData(r1, 1) = trainingData(r2, 1)
    '        trainingData(r2, 0) = swap1
    '        trainingData(r2, 1) = swap2
    '    Next

    '    ' По эпохам
    '    For epoch = 1 To 9000000
    '        network.AverageQuadError = 0

    '        For r = 0 To rowIndex - 1
    '            network.Training({trainingData(r, 0)}, {trainingData(r, 1)})
    '        Next

    '        network.AverageQuadError = network.AverageQuadError / rowCount
    '        Console.WriteLine("Эпоха: {0}. Ошибка {1}", epoch, network.AverageQuadError.ToString("##0.################"))

    '        If epoch Mod 100 Then NeuralState.Save("Forecast.txt", network)
    '        If network.AverageQuadError < 0.0001 Then Exit For
    '    Next


    '    Dim testDate As Date = "05.07.2018"
    '    Dim predictDate = NeuralConvert.Scaler(NeuralConvert.ToUnixTime(testDate), 1420059600000, 1577826000000, 0, 1)
    '    Dim predictTemp = NeuralConvert.Scaler(network.Predict({predictDate})(0), 0, 1, -273, 10000)

    '    Console.WriteLine("На дату {0} спрогнозировали температуру = {1}", testDate.ToShortDateString(), predictTemp)

    '    For Each row In sqlite.SelectRows($" select * from weather where '{testDate.ToString("yyyy-MM-dd")}' between datetime(ondate, '-3 days') and datetime(ondate, '+3 days') order by ondate asc ")
    '        Console.WriteLine("Фактическая температура за {0} = {1}", row("ondate"), row("temperature"))
    '    Next

    '    Console.ReadKey()
    'End Sub



    Private Class MotoSet
        Public Input(4) As Double
        Public Output(4) As Double
    End Class

    ' Фунция создаёт трейнсет для мотоциклов
    Private Function CreateMotoSet(P1Speed As Integer, P2Speed As Integer, P3Speed As Integer, P4Speed As Integer, P5Speed As Integer) As MotoSet
        Dim m As New MotoSet
        m.Input(0) = P1Speed / 250
        m.Input(1) = P2Speed / 250
        m.Input(2) = P3Speed / 250
        m.Input(3) = P4Speed / 250
        m.Input(4) = P5Speed / 250

        Dim maxValue = m.Input.Max()
        If m.Input(0) = maxValue Then
            m.Output(0) = 1
        ElseIf m.Input(1) = maxValue Then
            m.Output(1) = 1
        ElseIf m.Input(2) = maxValue Then
            m.Output(2) = 1
        ElseIf m.Input(3) = maxValue Then
            m.Output(3) = 1
        ElseIf m.Input(4) = maxValue Then
            m.Output(4) = 1
        End If

        Return m
    End Function

    Public Sub TaskMoto()
        Dim NN As New NeuralNetwork(5, 25, 25, 5)

        Dim LeaningRate As Double = 0.05
        Dim AvgError As Double = 0

        ' Генерируем трейнинг сеты
        Dim arrSets(499) As MotoSet
        For I = 1 To arrSets.GetUpperBound(0)
            arrSets(I) = CreateMotoSet(rand.Next(50, 241), rand.Next(50, 241), rand.Next(50, 241), rand.Next(50, 241), rand.Next(50, 241))
        Next

        ' По всем эпохам
        Dim N As Integer
        Do While N < 10000000
            N += 1

            ' По всем тренировочным сетам
            For K = 1 To arrSets.GetUpperBound(0)
                AvgError = NN.Training(arrSets(K).Input, arrSets(K).Output, LeaningRate)
            Next

            AvgError = AvgError / arrSets.Length
            If AvgError < 0.00000001 Then Exit Do

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", N, AvgError.ToString("##0.################"))

            ' Управление
            If Console.KeyAvailable Then
                Select Case Console.ReadKey().Key
                    Case ConsoleKey.Z

                        ' Тестируем по сетам
                        Console.WriteLine("=== Проверка работы сети ===")

                        Dim result() = NN.Predict({0.5, 0.53, 0.57, 0.6, 0.63})
                        result(0) = Math.Round(result(0), 2)
                        result(1) = Math.Round(result(1), 2)
                        result(2) = Math.Round(result(2), 2)
                        result(3) = Math.Round(result(3), 2)
                        result(4) = Math.Round(result(4), 2)
                        Console.WriteLine("Проверка: 150, 160, 170, 180, 190. Ожидаем 5-ый выгрышный. " & vbCrLf &
                                          "Решение: {0} | {1} | {2} | {3} | {4}" & vbCrLf, result(0), result(1), result(2), result(3), result(4))

                        result = NN.Predict({0.8, 0.78, 0.72, 0.79, 0.33})
                        result(0) = Math.Round(result(0), 2)
                        result(1) = Math.Round(result(1), 2)
                        result(2) = Math.Round(result(2), 2)
                        result(3) = Math.Round(result(3), 2)
                        result(4) = Math.Round(result(4), 2)
                        Console.WriteLine("Проверка: 240, 235, 215, 238, 100. Ожидаем 1-ый выгрышный. " & vbCrLf &
                                          "Решение: {0} | {1} | {2} | {3} | {4}" & vbCrLf, result(0), result(1), result(2), result(3), result(4))

                        result = NN.Predict({0.03, 0.03, 0.04, 0.03, 0.03})
                        result(0) = Math.Round(result(0), 2)
                        result(1) = Math.Round(result(1), 2)
                        result(2) = Math.Round(result(2), 2)
                        result(3) = Math.Round(result(3), 2)
                        result(4) = Math.Round(result(4), 2)
                        Console.WriteLine("Проверка: 10, 10, 11, 10, 9. Ожидаем 3-ый выгрышный. " & vbCrLf &
                                          "Решение: {0} | {1} | {2} | {3} | {4}" & vbCrLf, result(0), result(1), result(2), result(3), result(4))
                        Console.ReadKey()

                    Case ConsoleKey.S
                        LeaningRate += 0.01
                        Console.Title = "LeaningRate: " & LeaningRate

                    Case ConsoleKey.A
                        LeaningRate -= 0.01
                        If LeaningRate <= 0 Then LeaningRate = 0.01

                        Console.Title = "LeaningRate: " & LeaningRate
                    Case ConsoleKey.F1
                        NeuralState.Save("Moto.txt", NN)

                    Case ConsoleKey.F2
                        NN = NeuralState.Load("Moto.txt")
                End Select
            End If
        Loop


    End Sub


    Public Sub TaskSquareTriangle()
        Dim NN As INetwork
        Dim LeaningRate = 0.1
        Dim AvgQuadError = 0.0

        'NN = NeuralState.Load("SquareTriangle.txt")
        NN = New NeuralNetwork(2, 6, 4, 1)

        ' Обучаем
        For Each Epoch In Enumerable.Range(1, 20000000)
            AvgQuadError = 0.0

            AvgQuadError += NN.Training({0.014, 0.007}, {0.049}, LeaningRate)
            AvgQuadError += NN.Training({0.011, 0.022}, {0.121}, LeaningRate)
            AvgQuadError += NN.Training({0.011, 0.024}, {0.132}, LeaningRate)
            AvgQuadError += NN.Training({0.022, 0.023}, {0.253}, LeaningRate)
            AvgQuadError += NN.Training({0.016, 0.013}, {0.104}, LeaningRate)
            AvgQuadError += NN.Training({0.022, 0.011}, {0.121}, LeaningRate)
            AvgQuadError += NN.Training({0.02, 0.017}, {0.17}, LeaningRate)
            AvgQuadError += NN.Training({0.019, 0.022}, {0.209}, LeaningRate)
            AvgQuadError += NN.Training({0.02, 0.008}, {0.08}, LeaningRate)
            AvgQuadError += NN.Training({0.018, 0.022}, {0.198}, LeaningRate)
            AvgQuadError += NN.Training({0.024, 0.018}, {0.216}, LeaningRate)
            AvgQuadError += NN.Training({0.016, 0.022}, {0.176}, LeaningRate)
            AvgQuadError += NN.Training({0.025, 0.024}, {0.3}, LeaningRate)
            AvgQuadError += NN.Training({0.015, 0.007}, {0.0525}, LeaningRate)
            AvgQuadError += NN.Training({0.015, 0.016}, {0.12}, LeaningRate)
            AvgQuadError += NN.Training({0.025, 0.022}, {0.275}, LeaningRate)
            AvgQuadError += NN.Training({0.011, 0.017}, {0.0935}, LeaningRate)
            AvgQuadError += NN.Training({0.019, 0.023}, {0.2185}, LeaningRate)
            AvgQuadError += NN.Training({0.017, 0.011}, {0.0935}, LeaningRate)
            AvgQuadError += NN.Training({0.014, 0.014}, {0.098}, LeaningRate)

            AvgQuadError = AvgQuadError / 20
            If AvgQuadError < 0.0000001 Then Exit For

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", Epoch, AvgQuadError.ToString("##0.##########"))

            ' Управление
            If Console.KeyAvailable Then
                Select Case Console.ReadKey().Key
                    Case ConsoleKey.Z
                        Exit For
                    Case ConsoleKey.S
                        LeaningRate += 0.01
                        Console.Title = "LeaningRate: " & LeaningRate
                    Case ConsoleKey.A
                        LeaningRate -= 0.01
                        Console.Title = "LeaningRate: " & LeaningRate
                End Select
            End If
        Next

        ' Сохраняем нейронку
        NeuralState.Save("SquareTriangle.txt", NN)

        ' Тестируем по сетам
        Console.WriteLine("=== Проверка работы сети ===")
        Console.WriteLine("Проверка на значениях: 20 и 9. Ожидаем 90. Решение {0}", NN.Predict({0.02, 0.009})(0) * 1000)
        Console.WriteLine("Проверка на значениях: 10 и 17. Ожидаем 85. Решение {0}", NN.Predict({0.01, 0.017})(0) * 1000)
        Console.ReadLine()
    End Sub

    Public Sub TaskXOR()
        ' 0  0    0
        ' 0  1    1
        ' 1  0    1
        ' 1  1    0

        Dim NN As New NeuralNetwork(2, 4, 1)
        Dim LeaningRate = 0.2
        Dim AvgQuadError = 0.0

        'NN.ChangeActivator(2, 0, "RELU")

        If IO.File.Exists("XOR.txt") Then NN = NeuralState.Load("XOR.txt")

        ' Обучаем
        For Each Epoch In Enumerable.Range(1, 1000000)
            AvgQuadError = 0.0
            AvgQuadError += NN.Training({0, 0}, {0}, LeaningRate)
            AvgQuadError += NN.Training({0, 1}, {1}, LeaningRate)
            AvgQuadError += NN.Training({1, 0}, {1}, LeaningRate)
            AvgQuadError += NN.Training({1, 1}, {0}, LeaningRate)

            AvgQuadError = AvgQuadError / 4
            If AvgQuadError < 0.0001 Then Exit For

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", Epoch, AvgQuadError.ToString("##0.################"))
        Next

        ' Тестируем по сетам
        Console.WriteLine("=== Проверка работы сети ===")
        Console.WriteLine("Проверка на значениях: 0 и 0. Ожидаем 0. Решение {0}", NN.Predict({0, 0})(0).ToString("##0.################"))
        Console.WriteLine("Проверка на значениях: 0 и 1. Ожидаем 1. Решение {0}", NN.Predict({0, 1})(0).ToString("##0.################"))
        Console.WriteLine("Проверка на значениях: 1 и 0. Ожидаем 1. Решение {0}", NN.Predict({1, 0})(0).ToString("##0.################"))
        Console.WriteLine("Проверка на значениях: 1 и 1. Ожидаем 0. Решение {0}", NN.Predict({1, 1})(0).ToString("##0.################"))

        NeuralState.Save("XOR.txt", NN)
        Console.ReadLine()
    End Sub




    Public Sub TaskEven()
        Dim NN As New NeuralNetwork(1, 12, 1)
        Dim LearningRate = 0.3
        Dim AvgQuadError = 0.0

        ' Генерируем сеты
        Dim rand As New Random(Environment.TickCount)
        Dim Trainings As New List(Of Tuple(Of Double, Double))

        For I = 1 To 1000 Step 10
            Trainings.Add(New Tuple(Of Double, Double)(I / 1000, Math.Sqrt(I) / 1000))
        Next

        ' Обучаем
        For Each Epoch In Enumerable.Range(1, 300000)
            AvgQuadError = 0.0

            For Each nset In Trainings
                AvgQuadError += NN.Training({nset.Item1}, {nset.Item2}, LearningRate)
            Next

            AvgQuadError = AvgQuadError / Trainings.Count
            If AvgQuadError < 0.00000001 Then Exit For

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", Epoch, AvgQuadError.ToString("##0.################"))
            If Epoch Mod 5000 = 0 Then NeuralState.Save("Even.txt", NN)
        Next


        ' Тестируем по сетам
        Console.WriteLine("=== Проверка работы сети ===")
        Console.WriteLine("Проверка на значениях: 25. Ожидаем 5. Решение {0}", (NN.Predict({25 / 1000})(0) * 1000).ToString("##0.################"))
        Console.WriteLine("Проверка на значениях: 100. Ожидаем 10. Решение {0}", (NN.Predict({100 / 1000})(0) * 1000).ToString("##0.################"))
        Console.WriteLine("Проверка на значениях: 125. Ожидаем 11,1. Решение {0}", (NN.Predict({125 / 1000})(0) * 1000).ToString("##0.################"))
        Console.WriteLine("Проверка на значениях: 256. Ожидаем 16. Решение {0}", (NN.Predict({256 / 1000})(0) * 1000).ToString("##0.################"))
        Console.WriteLine("Проверка на значениях: 700. Ожидаем 26,45. Решение {0}", (NN.Predict({1024 / 1000})(0) * 1000).ToString("##0.################"))
        Console.ReadLine()

    End Sub


    Public Sub TaskBinToDec()
        Dim NN As New NeuralNetwork(3, 16, 8)
        Dim LeaningRate = 0.5
        Dim AvgQuadError = 0.0

        ' Обучаем
        For Each Epoch In Enumerable.Range(1, 100000)
            AvgQuadError = 0.0

            AvgQuadError += NN.Training({0, 0, 0}, {1, 0, 0, 0, 0, 0, 0, 0}, LeaningRate)
            AvgQuadError += NN.Training({0, 0, 1}, {0, 1, 0, 0, 0, 0, 0, 0}, LeaningRate)
            AvgQuadError += NN.Training({0, 1, 0}, {0, 0, 1, 0, 0, 0, 0, 0}, LeaningRate)
            AvgQuadError += NN.Training({0, 1, 1}, {0, 0, 0, 1, 0, 0, 0, 0}, LeaningRate)
            AvgQuadError += NN.Training({1, 0, 0}, {0, 0, 0, 0, 1, 0, 0, 0}, LeaningRate)
            AvgQuadError += NN.Training({1, 0, 1}, {0, 0, 0, 0, 0, 1, 0, 0}, LeaningRate)
            AvgQuadError += NN.Training({1, 1, 0}, {0, 0, 0, 0, 0, 0, 1, 0}, LeaningRate)
            AvgQuadError += NN.Training({1, 1, 1}, {0, 0, 0, 0, 0, 0, 0, 1}, LeaningRate)

            AvgQuadError = AvgQuadError / 8
            If AvgQuadError < 0.0001 Then Exit For

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", Epoch, AvgQuadError.ToString("##0.################"))
        Next

        ' Тестируем по сетам
        Console.WriteLine("=== Проверка работы сети ===")
        Console.WriteLine("Проверка на значениях: 0, 0, 0. Ожидаем 0. Решение {0}", NN.Predict({0, 0, 0}).ToStringDouble(0))
        Console.WriteLine("Проверка на значениях: 0, 0, 1. Ожидаем 1. Решение {0}", NN.Predict({0, 0, 1}).ToStringDouble(0))
        Console.WriteLine("Проверка на значениях: 0, 1, 0. Ожидаем 2. Решение {0}", NN.Predict({0, 1, 0}).ToStringDouble(0))
        Console.WriteLine("Проверка на значениях: 0, 1, 1. Ожидаем 3. Решение {0}", NN.Predict({0, 1, 1}).ToStringDouble(0))
        Console.WriteLine("Проверка на значениях: 1, 0, 0. Ожидаем 4. Решение {0}", NN.Predict({1, 0, 0}).ToStringDouble(0))
        Console.WriteLine("Проверка на значениях: 1, 0, 1. Ожидаем 5. Решение {0}", NN.Predict({1, 0, 1}).ToStringDouble(0))
        Console.WriteLine("Проверка на значениях: 1, 1, 0. Ожидаем 6. Решение {0}", NN.Predict({1, 1, 0}).ToStringDouble(0))
        Console.WriteLine("Проверка на значениях: 1, 1, 1. Ожидаем 7. Решение {0}", NN.Predict({1, 1, 1}).ToStringDouble(0))
        Console.WriteLine("")
        Console.WriteLine("=== Убьём пару нейронов в сети ===")
        NN.Activities(1)(1) = 0
        NN.Activities(1)(2) = 0
        Console.WriteLine("Проверка на значениях: 0, 0, 0. Ожидаем 0. Решение {0}", NN.Predict({0, 0, 0}).ToStringDouble(1))
        Console.WriteLine("Проверка на значениях: 0, 0, 1. Ожидаем 1. Решение {0}", NN.Predict({0, 0, 1}).ToStringDouble(1))
        Console.WriteLine("Проверка на значениях: 0, 1, 0. Ожидаем 2. Решение {0}", NN.Predict({0, 1, 0}).ToStringDouble(1))
        Console.WriteLine("Проверка на значениях: 0, 1, 1. Ожидаем 3. Решение {0}", NN.Predict({0, 1, 1}).ToStringDouble(1))
        Console.WriteLine("Проверка на значениях: 1, 0, 0. Ожидаем 4. Решение {0}", NN.Predict({1, 0, 0}).ToStringDouble(1))
        Console.WriteLine("Проверка на значениях: 1, 0, 1. Ожидаем 5. Решение {0}", NN.Predict({1, 0, 1}).ToStringDouble(1))
        Console.WriteLine("Проверка на значениях: 1, 1, 0. Ожидаем 6. Решение {0}", NN.Predict({1, 1, 0}).ToStringDouble(1))
        Console.WriteLine("Проверка на значениях: 1, 1, 1. Ожидаем 7. Решение {0}", NN.Predict({1, 1, 1}).ToStringDouble(1))

        Console.ReadLine()

    End Sub

    <Extension>
    Public Function ToStringDouble(ByVal Numbers() As Double, Optional MaxDecimal As Integer = 16) As String
        Dim numarr() = Numbers.Select(Function(n) n.ToString("##0." & New String("0", MaxDecimal)).Replace(",", ".")).ToArray()
        Dim numstring = String.Join(", ", numarr)
        Return $"[ {numstring} ]"
    End Function

End Module
