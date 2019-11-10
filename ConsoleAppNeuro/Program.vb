Imports NeuralProject


Module Program

    Private rand As New Random(Environment.TickCount)

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


    Public Sub Main()
        Dim NN As New NeuralNetwork(5, 5)
        If NN.LeaningRate = 0 Then NN.LeaningRate = 0.005

        ' Генерируем трейнинг сеты
        Dim arrSets(999) As MotoSet
        For I = 1 To arrSets.GetUpperBound(0)
            arrSets(I) = CreateMotoSet(rand.Next(50, 241), rand.Next(50, 241), rand.Next(50, 241), rand.Next(50, 241), rand.Next(50, 241))
        Next

        ' По всем эпохам
        For I = NN.Epoch To 10000000
            NN.Epoch = I
            NN.AverageQuadError = 0

            ' По всем тренировочным сетам
            For K = 1 To arrSets.GetUpperBound(0)
                NN.TrainingSet(arrSets(K).Input, arrSets(K).Output)
            Next

            NN.AverageQuadError = NN.AverageQuadError / arrSets.Length
            If NN.AverageQuadError < 0.00000001 Then Exit For

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", NN.Epoch, NN.AverageQuadError.ToString("##0.################"))


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
                        NN.LeaningRate += 0.001
                        Console.Title = "LeaningRate: " & NN.LeaningRate

                    Case ConsoleKey.A
                        NN.LeaningRate -= 0.001
                        If NN.LeaningRate <= 0 Then NN.LeaningRate = 0.001

                        Console.Title = "LeaningRate: " & NN.LeaningRate
                    Case ConsoleKey.F1
                        NeuralState.Save("Moto.txt", NN)

                    Case ConsoleKey.F2
                        NN = NeuralState.Load("Moto.txt")

                End Select
            End If
        Next


    End Sub


    Public Sub TaskSquareTriangle()
        Dim NN As NeuralNetwork

        NN = NeuralState.Load("SquareTriangle.txt")
        'NN = New NeuralNetwork(2, 6, 6, 1)
        'NN.LeaningRate = 0.1

        ' Обучаем
        For Each Epoch In Enumerable.Range(NN.Epoch, 20000000)
            NN.Epoch = Epoch
            NN.AverageQuadError = 0.0

            NN.TrainingSet({0.014, 0.007}, {0.049})
            NN.TrainingSet({0.011, 0.022}, {0.121})
            NN.TrainingSet({0.011, 0.024}, {0.132})
            NN.TrainingSet({0.022, 0.023}, {0.253})
            NN.TrainingSet({0.016, 0.013}, {0.104})
            NN.TrainingSet({0.022, 0.011}, {0.121})
            NN.TrainingSet({0.02, 0.017}, {0.17})
            NN.TrainingSet({0.019, 0.022}, {0.209})
            NN.TrainingSet({0.02, 0.008}, {0.08})
            NN.TrainingSet({0.018, 0.022}, {0.198})
            NN.TrainingSet({0.024, 0.018}, {0.216})
            NN.TrainingSet({0.016, 0.022}, {0.176})
            NN.TrainingSet({0.025, 0.024}, {0.3})
            NN.TrainingSet({0.015, 0.007}, {0.0525})
            NN.TrainingSet({0.015, 0.016}, {0.12})
            NN.TrainingSet({0.025, 0.022}, {0.275})
            NN.TrainingSet({0.011, 0.017}, {0.0935})
            NN.TrainingSet({0.019, 0.023}, {0.2185})
            NN.TrainingSet({0.017, 0.011}, {0.0935})
            NN.TrainingSet({0.014, 0.014}, {0.098})

            NN.AverageQuadError = NN.AverageQuadError / 20
            If NN.AverageQuadError < 0.0000001 Then Exit For

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", Epoch, NN.AverageQuadError.ToString("##0.##########"))

            ' Управление
            If Console.KeyAvailable Then
                Select Case Console.ReadKey().Key
                    Case ConsoleKey.Z
                        Exit For
                    Case ConsoleKey.S
                        NN.LeaningRate += 0.01
                        Console.Title = "LeaningRate: " & NN.LeaningRate
                    Case ConsoleKey.A
                        NN.LeaningRate -= 0.01
                        Console.Title = "LeaningRate: " & NN.LeaningRate
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
        NN.LeaningRate = 0.005

        ' Обучаем
        For Each Epoch In Enumerable.Range(1, 50000)
            NN.AverageQuadError = 0.0

            NN.TrainingSet({0, 0}, {0})
            NN.TrainingSet({0, 1}, {1})
            NN.TrainingSet({1, 0}, {1})
            NN.TrainingSet({1, 1}, {0})

            NN.AverageQuadError = NN.AverageQuadError / 4
            If NN.AverageQuadError < 0.001 Then Exit For

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", Epoch, NN.AverageQuadError.ToString("##0.##########"))
        Next

        ' Тестируем по сетам
        Console.WriteLine("=== Проверка работы сети ===")
        Console.WriteLine("Проверка на значениях: 0 и 0. Ожидаем 0. Решение {0}", NN.Predict({0, 0})(0))
        Console.WriteLine("Проверка на значениях: 0 и 1. Ожидаем 1. Решение {0}", NN.Predict({0, 1})(0))
        Console.WriteLine("Проверка на значениях: 1 и 0. Ожидаем 1. Решение {0}", NN.Predict({1, 0})(0))
        Console.WriteLine("Проверка на значениях: 1 и 1. Ожидаем 0. Решение {0}", NN.Predict({1, 1})(0))
        Console.ReadLine()
    End Sub

End Module
